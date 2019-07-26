using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Mozilla.IoT.WebThing.Json;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing
{
    public class ActionActivator : IActionActivator
    {
        private readonly IServiceProvider _service;
        private readonly IJsonConvert _jsonConvert;
        private readonly IJsonSerializerSettings _jsonSettings;

        public ActionActivator(IServiceProvider service, IJsonConvert jsonConvert, IJsonSerializerSettings jsonSettings)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _jsonConvert = jsonConvert ?? throw new ArgumentNullException(nameof(jsonConvert));
            _jsonSettings = jsonSettings ?? throw new ArgumentNullException(nameof(jsonSettings));
        }

        public ValueTask<Action> CreateAsync(Thing thing, string name, IDictionary<string, object> input,
            CancellationToken cancellation)
        {
            if (!thing.ActionsTypeInfo.ContainsKey(name))
            {
                return new ValueTask<Action>(result: null);
            }

            (Type type, _) = thing.ActionsTypeInfo[name];
            Action action = (Action)_service.GetService(type);

            action.Thing = thing;
            action.Name = name;
            action.HrefPrefix = thing.HrefPrefix;
            action.Input = input;
            action.Href = $"/actions/{name}/{action.Id}";
            thing.Actions[name].AddLast(action);

            return !thing.Subscribers.IsEmpty ? new ValueTask<Action>(NotifySubscribersAsync(thing, action, cancellation)) 
                : new ValueTask<Action>(action);
        }

        private async Task<Action> NotifySubscribersAsync(Thing thing, Action action, CancellationToken cancellation)
        {
            var message = new Dictionary<string, object>
            {
                [INPUT] = action.Input,
                [HREF] = action.HrefPrefix.JoinUrl(action.Href),
                [STATUS] = action.Status.ToString().ToLower()
            };

            byte[] json = _jsonConvert.Serialize(message, _jsonSettings);

            var buffer = new ArraySegment<byte>(json);
            foreach (WebSocket socket in thing.Subscribers.Values)
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, cancellation);
            }

            return action;
        }
    }
}
