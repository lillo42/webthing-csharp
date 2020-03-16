using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Mozilla.IoT.WebThing.WebSockets
{
    /// <summary>
    /// Execute request action
    /// </summary>
    public class RequestAction : IWebSocketAction
    {
        private static readonly  ArraySegment<byte> s_errorMessage = new ArraySegment<byte>(Encoding.UTF8.GetBytes(@"{""messageType"": ""error"",""data"": {""status"": ""400 Bad Request"",""message"": ""Invalid action request""}}"));

        private readonly ILogger<RequestAction> _logger;

        /// <summary>
        /// Initialize a new instance of <see cref="RequestAction"/>.
        /// </summary>
        /// <param name="logger"></param>
        public RequestAction(ILogger<RequestAction> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The Action name. This value should be unique.
        /// </summary>
        public string Action => "requestAction";

        /// <summary>
        /// Execute this action when web socket request action where action name match with <see cref="Action"/>
        /// </summary>
        /// <param name="socket">The <see cref="WebSocket"/> origin of this action.</param>
        /// <param name="thing">The <see cref="Thing"/> associated with action.</param>
        /// <param name="data">The <see cref="JsonElement"/> request with this action.</param>
        /// <param name="provider">The <see cref="IServiceProvider"/> for this action. Every request is generate new scope.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns></returns>
        public ValueTask ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, JsonElement data,
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            var option = provider.GetRequiredService<JsonSerializerOptions>();
            foreach (var property in data.EnumerateObject())
            {
                if (!thing.ThingContext.Actions.TryGetValue(property.Name, out var actions))
                {
                    continue;
                }

                if (!actions.TryAdd(property.Value, out var action))
                {
                    _logger.LogInformation("{actionName} Action has invalid parameters. [Name: {thingName}]", property.Name, thing.Name);
                    socket.SendAsync(s_errorMessage, WebSocketMessageType.Text, true, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                }

                action.Thing = thing;
                
                _logger.LogInformation("Going to execute {actionName} action. [Name: {thingName}]", action.GetActionName(), thing.Name);
                
                var namePolicy = option.PropertyNamingPolicy;
                action.Href = $"/things/{namePolicy.ConvertName(thing.Name)}/actions/{namePolicy.ConvertName(action.GetActionName())}/{action.GetId()}";
                
                action.ExecuteAsync(thing, provider)
                    .ConfigureAwait(false);
            }
            
            return new ValueTask();
        }
    }
}
