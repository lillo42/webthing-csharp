using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mozilla.IoT.WebThing.Extensions;

namespace Mozilla.IoT.WebThing.WebSockets
{
    internal class WebSocket
    {
        private static readonly ArrayPool<byte> s_pool = ArrayPool<byte>.Create();
        private static readonly ArraySegment<byte> s_error = new ArraySegment<byte>(
            Encoding.UTF8.GetBytes(
                @"{""messageType"": ""error"", ""data"": {""status"": ""400 Bad Request"",""message"": ""Invalid message""}}"));
        public static async Task InvokeAsync(HttpContext context)
        {
            var service = context.RequestServices;
            var cancellation = context.RequestAborted;
            
            var logger = service.GetRequiredService<ILogger<WebSocket>>();
            
            var things = service.GetRequiredService<IEnumerable<Thing>>();
            var option = service.GetRequiredService<ThingOption>();
            
            var name = context.GetRouteData<string>("name");

            var thing = option.IgnoreCase switch
            {
                true => things.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)),
                _ => things.FirstOrDefault(x => x.Name == name)
            };

            if (thing == null)
            {
                logger.LogInformation("Thing not found. [Name: {name}]", name);
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }

            logger.LogInformation("Going to accept new Web Socket connection for {thing} Thing", name);
            var socket = await context.WebSockets.AcceptWebSocketAsync()
                .ConfigureAwait(false);

            
            var id = Guid.NewGuid();
            thing.ThingContext.Sockets.TryAdd(id, socket);

            byte[]? buffer = null;

            var actions = service.GetRequiredService<Dictionary<string, IWebSocketAction>>();

            var jsonOptions = service.GetRequiredService<JsonSerializerOptions>();
            
            var webSocketOption = service.GetService<IOptions<WebSocketOptions>>();
            
            var observer = new ThingObserver(service.GetRequiredService<ILogger<ThingObserver>>(),
                jsonOptions, socket, cancellation, thing);
            
            try
            {
                BindActions(thing, observer);
                BindPropertyChanged(thing, observer);
                
                while (!socket.CloseStatus.HasValue && !cancellation.IsCancellationRequested)
                {
                    if (buffer != null)
                    {
                        s_pool.Return(buffer, true);
                    }
                    
                    buffer = s_pool.Rent(webSocketOption.Value.ReceiveBufferSize);
                    
                    await socket
                        .ReceiveAsync(new ArraySegment<byte>(buffer), cancellation)
                        .ConfigureAwait(false);

                    var json = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(buffer, jsonOptions);

                    if (!json.ContainsKey("messageType"))
                    {
                        logger.LogInformation("Web Socket request without messageType");
                        await socket.SendAsync(s_error, WebSocketMessageType.Text, true, cancellation)
                            .ConfigureAwait(false);
                        continue;
                    }
                    
                    if (!json.ContainsKey("data"))
                    {
                        logger.LogInformation("Web Socket request without data. [Message Type: {messageType}]", json["messageType"].GetString());
                        await socket.SendAsync(s_error, WebSocketMessageType.Text, true, cancellation)
                            .ConfigureAwait(false);
                        continue;
                    }

                    if (!actions.TryGetValue(json["messageType"].GetString(), out var action))
                    {
                        logger.LogInformation("Invalid Message Type: {messageType}", json["messageType"].GetString());
                        await socket.SendAsync(s_error, WebSocketMessageType.Text, true, cancellation)
                            .ConfigureAwait(false);
                        continue;
                    }

                    try
                    {
                        using var scope = service.CreateScope();
                        scope.ServiceProvider.GetRequiredService<ThingObserverResolver>().Observer = observer;
                        await action.ExecuteAsync(socket, thing, json["data"], jsonOptions, scope.ServiceProvider, cancellation)
                            .ConfigureAwait(false);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error to execute Web Socket Action: {action}", json["messageType"].GetString());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error to execute WebSocket, going to close connection");

                await socket.CloseAsync(WebSocketCloseStatus.InternalServerError, ex.ToString(),
                    CancellationToken.None)
                    .ConfigureAwait(false);
            }

            thing.ThingContext.Sockets.TryRemove(id, out _);
            
            if (buffer != null)
            {
                s_pool.Return(buffer, true);
            }

            UnbindActions(thing, observer);
            UnbindPropertyChanged(thing, observer);
            UnbindEvent(thing, observer);
        }

        private static void BindActions(Thing thing, ThingObserver observer)
        {
            foreach (var (_, actionContext) in thing.ThingContext.Actions)
            {
                actionContext.Actions.Change += observer.OnActionChange;
            }
        }

        private static void BindPropertyChanged(Thing thing, ThingObserver observer) 
            => thing.PropertyChanged += observer.OnPropertyChanged;
        
        private static void UnbindActions(Thing thing, ThingObserver observer)
        {
            foreach (var (_, actionContext) in thing.ThingContext.Actions)
            {
                actionContext.Actions.Change -= observer.OnActionChange;
            }
        }
        
        private static void UnbindPropertyChanged(Thing thing, ThingObserver observer) 
            => thing.PropertyChanged -= observer.OnPropertyChanged;
        
        private static void UnbindEvent(Thing thing, ThingObserver observer)
        {
            foreach (var @event in observer.EventsBind)
            {
                thing.ThingContext.Events[@event].Added -= observer.OnEvenAdded;
            }
        }
    }
}
