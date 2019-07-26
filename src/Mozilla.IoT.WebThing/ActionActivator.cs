using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Json;
using static Mozilla.IoT.WebThing.Const;

namespace Mozilla.IoT.WebThing
{
    public class ActionActivator : IActionActivator
    {
        private readonly IServiceProvider _service;

        private readonly Func<Type, ObjectFactory> _createFactory = type =>
            ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);

        private readonly ConcurrentDictionary<Type, ObjectFactory>
            _typeActivatorCache = new ConcurrentDictionary<Type, ObjectFactory>();

        public ActionActivator(IServiceProvider service)
        {
            _service = service;
        }

        public Action CreateInstance(Thing thing, string name, IDictionary<string, object> input)
        {
            if (!thing.ActionsTypeInfo.ContainsKey(name))
            {
                return null;
            }

            (Type type, _) = thing.ActionsTypeInfo[name];
            Action action = CreateAction(type);

            action.Thing = thing;
            action.Name = name;
            action.HrefPrefix = thing.HrefPrefix;
            action.Input = input;
            action.Href = $"/actions/{name}/{action.Id}";
            thing.Actions[name].AddLast(action);

            return action;
        }

        private Action CreateAction(Type implementationType)
        {
            var createFactory = _typeActivatorCache.GetOrAdd(implementationType, _createFactory);
            return (Action)createFactory(_service, arguments: null);
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
