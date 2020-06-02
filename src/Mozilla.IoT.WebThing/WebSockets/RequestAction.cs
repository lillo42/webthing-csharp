using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Json;

namespace Mozilla.IoT.WebThing.WebSockets
{
    /// <summary>
    /// Execute request action
    /// </summary>
    public class RequestAction : IWebSocketAction
    {
        private static readonly  ArraySegment<byte> s_errorInvalidParameter = new ArraySegment<byte>(Encoding.UTF8.GetBytes(@"{""messageType"": ""error"",""data"": {""status"": ""400 Bad Request"",""message"": ""Invalid action request""}}"));
        private static readonly  ArraySegment<byte> s_errorActionNotFound = new ArraySegment<byte>(Encoding.UTF8.GetBytes(@"{""messageType"": ""error"",""data"": {""status"": ""404 Not Found"",""message"": ""Action not found""}}"));

        private readonly ILogger<RequestAction> _logger;

        /// <summary>
        /// Initialize a new instance of <see cref="RequestAction"/>.
        /// </summary>
        /// <param name="logger"></param>
        public RequestAction(ILogger<RequestAction> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public string Action => "requestAction";

        /// <inheritdoc/>
        public async ValueTask ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, object data,
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            var option = provider.GetRequiredService<ThingOption>();
            var convert = provider.GetRequiredService<IJsonConvert>();

            foreach (var (actionName, value) in convert.ToEnumerable(data))
            {
                if (!thing.ThingContext.Actions.TryGetValue(actionName, out var actions))
                {
                    _logger.LogInformation("Action not found. [Thing: {name}][Action: {actionName}]",
                        thing.Name, actionName);
                    
                    await socket.SendAsync(s_errorActionNotFound, WebSocketMessageType.Text, true, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                }
                
                if (!actions.TryAdd(value, out var action))
                {
                    _logger.LogInformation("Action has invalid parameters. [Thing: {name}][Action: {actionName}]", 
                        thing.Name, actionName);
                    
                    await socket.SendAsync(s_errorInvalidParameter, WebSocketMessageType.Text, true, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                }

                action.Thing = thing;
                var namePolicy = option.PropertyNamingPolicy;
                action.Href = $"/things/{namePolicy.ConvertName(thing.Name)}/actions/{namePolicy.ConvertName(actionName)}/{action.GetId()}";
                
                _logger.LogInformation("Going to execute action. [Thing: {name}][Action: {actionName}][Action Id: {actionId}]", 
                    thing.Name, actionName, action.GetId());
                _ = action.ExecuteAsync(thing, provider).ConfigureAwait(false);
            }
        }
    }
}
