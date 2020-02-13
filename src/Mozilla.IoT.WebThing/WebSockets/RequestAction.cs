using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Mozilla.IoT.WebThing.Actions;

namespace Mozilla.IoT.WebThing.WebSockets
{
    public class RequestAction : IWebSocketAction
    {
        private static readonly  ArraySegment<byte> s_errorMessage = new ArraySegment<byte>(Encoding.UTF8.GetBytes(@"{""messageType"": ""error"",""data"": {""status"": ""400 Bad Request"",""message"": ""Invalid action request""}}"));

        private readonly ILogger<RequestAction> _logger;

        public RequestAction(ILogger<RequestAction> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string Action => "requestAction";

        public Task ExecuteAsync(System.Net.WebSockets.WebSocket socket, Thing thing, JsonElement data, JsonSerializerOptions options,
            IServiceProvider provider, CancellationToken cancellationToken)
        {
            foreach (var (actionName, actionContext) in thing.ThingContext.Actions)
            {
                if(!data.TryGetProperty(actionName, out var json))
                {
                    continue;
                }
                
                _logger.LogTrace("{actionName} Action found. [Name: {thingName}]", actionName, thing.Name);
                var actionInfo = (ActionInfo)JsonSerializer.Deserialize(json.GetBytesFromBase64(), actionContext.ActionType, options);
                
                if (!actionInfo.IsValid())
                {
                    _logger.LogInformation("{actionName} Action has invalid parameters. [Name: {thingName}]", actionName, thing.Name);
                    socket.SendAsync(s_errorMessage, WebSocketMessageType.Text, true, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                }
                
                _logger.LogInformation("Going to execute {actionName} action. [Name: {thingName}]", actionName, thing.Name);
                
                actionInfo.ExecuteAsync(thing, provider)
                    .ConfigureAwait(false);
                
                thing.ThingContext.Actions[actionInfo.GetActionName()].Actions.Add(actionInfo.Id, actionInfo);
            }
            
            return Task.CompletedTask;
        }
    }
}
