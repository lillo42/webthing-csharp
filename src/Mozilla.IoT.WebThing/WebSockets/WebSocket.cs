using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Json;

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

            var actions = service.GetRequiredService<IEnumerable<IWebSocketAction>>()
                .ToDictionary(x => x.Action,
                    x => x);

            var converter = service.GetRequiredService<IJsonConvert>();
            var webSocketOption = service.GetRequiredService<IOptions<WebSocketOptions>>().Value;

            try
            {
                while (!socket.CloseStatus.HasValue && !cancellation.IsCancellationRequested)
                {
                    if (buffer != null)
                    {
                        s_pool.Return(buffer, true);
                    }

                    buffer = s_pool.Rent(webSocketOption.ReceiveBufferSize);
                    var segment = new ArraySegment<byte>(buffer);
                    var received = await socket
                        .ReceiveAsync(segment, cancellation)
                        .ConfigureAwait(false);

                    if (received.CloseStatus.HasValue)
                    {
                        logger.LogInformation("Request by client to close socket. [Thing: {name}][Close status: {closeStatus}]", 
                            thing.Name, received.CloseStatus);
                        continue;
                    }

                    var messageType = string.Empty;
                    try
                    {
                        var command = converter.Deserialize<RequestCommand>(segment.Slice(0, received.Count));

                        messageType = command.MessageType;
                        if (string.IsNullOrEmpty(command.MessageType))
                        {
                            logger.LogInformation("Web Socket request without messageType");
                            
                            await socket.SendAsync(s_error, WebSocketMessageType.Text, true, cancellation)
                                .ConfigureAwait(false);
                            continue;
                        }

                        if (command.Data == null)
                        {
                            logger.LogInformation("Web Socket request without data. [Message Type: {messageType}]", command.MessageType);
                            
                            await socket.SendAsync(s_error, WebSocketMessageType.Text, true, cancellation)
                                .ConfigureAwait(false);
                            continue;
                        }
                        
                        if (!actions.TryGetValue(command.MessageType, out var action))
                        {
                            logger.LogInformation("Invalid Message Type. [Message Type: {messageType}]", command.MessageType);
                            await socket.SendAsync(s_error, WebSocketMessageType.Text, true, cancellation)
                                .ConfigureAwait(false);
                            continue;
                        }

                        using var scope = service.CreateScope();
                        await action.ExecuteAsync(socket, thing, command.Data, scope.ServiceProvider, cancellation)
                            .ConfigureAwait(false);

                        messageType = string.Empty;
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error to execute Web Socket Action. [Message Type: {messageType}]", messageType);
                    }
                }
                
                if (socket.CloseStatus.HasValue)
                {
                    logger.LogInformation("Going to close socket. [Thing: {name}]", thing.Name);
                    await socket
                        .CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None)
                        .ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Close connection by client. [Thing: {name}]", thing.Name);
                await socket
                    .CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error to execute WebSocket, going to close connection. [Thing: {name}]", thing.Name);
                await socket
                        .CloseAsync(WebSocketCloseStatus.InternalServerError, ex.ToString(), CancellationToken.None)
                        .ConfigureAwait(false);
            }

            thing.ThingContext.Sockets.TryRemove(id, out _);
            foreach (var (_, subscribes) in thing.ThingContext.EventsSubscribes)
            {
                subscribes.TryRemove(id, out _);
            }
            
            if (buffer != null)
            {
                s_pool.Return(buffer, true);
            }
        }
    }
}
