using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Mozilla.IoT.WebThing.Descriptor;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.Notify;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Notify
{
    public class NotifySubscribesOnActionAddedTest
    {
        private readonly Fixture _fixture;
        private readonly NotifySubscribesOnActionAdded _notify;
        private readonly IJsonSerializer _serializer;
        private readonly IJsonSerializerSettings _serializerSettings;
        private readonly IDescriptor<Action> _descriptor;

        public NotifySubscribesOnActionAddedTest()
        {
            _fixture = new Fixture();
            _serializer = Substitute.For<IJsonSerializer>();
            _serializerSettings = Substitute.For<IJsonSerializerSettings>();
            _descriptor = Substitute.For<IDescriptor<Action>>();
            _notify = new NotifySubscribesOnActionAdded(_descriptor, _serializer, _serializerSettings);
        }

        [Theory]
        [InlineData(NotifyCollectionChangedAction.Move)]
        [InlineData(NotifyCollectionChangedAction.Remove)]
        [InlineData(NotifyCollectionChangedAction.Replace)]
        [InlineData(NotifyCollectionChangedAction.Reset)]
        public void Notify_Should_DoNothing_When_IsNotAdd(NotifyCollectionChangedAction action)
        {
            var @event = action switch
            {
                NotifyCollectionChangedAction.Move => new NotifyCollectionChangedEventArgs(action, _fixture.Create<object>(), _fixture.Create<int>(), _fixture.Create<int>()),
                NotifyCollectionChangedAction.Replace => new NotifyCollectionChangedEventArgs(action, _fixture.Create<object>(), _fixture.Create<object>()),
                NotifyCollectionChangedAction.Remove => new NotifyCollectionChangedEventArgs(action, _fixture.Create<object>()),
                _ => new NotifyCollectionChangedEventArgs(action)
            };

            _notify.Notify(_fixture.Create<object>(), @event);
            
            _serializer
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings));
        }

        [Fact]
        public void Notify_Should_DoNothing_When_SendIsNotAction()
        {
            _notify.Notify(_fixture.Create<object>(),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new object()));
            
            _serializer
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings));
        }
        
        [Fact]
        public void Notify_Should_DoNothing_When_ActionHaveNotSubscribe()
        {
            var thing = Substitute.For<Thing>();

            thing.Subscribers
                .Returns(new ConcurrentDictionary<Guid, WebSocket>());
            
            var action = Substitute.For<Action>();
            action.Thing.Returns(thing);

            _notify.Notify(_fixture.Create<object>(),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, action));

            _serializer
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings));
        }
        
        
        [Fact]
        public void Notify_Should_SendData_When_ExistSubscribes()
        {
            var thing = Substitute.For<Thing>();

            var webSocket = Substitute.For<WebSocket>();

            thing.Subscribers
                .Returns(new ConcurrentDictionary<Guid, WebSocket>
                {
                    [_fixture.Create<Guid>()] = webSocket
                });
            
            var action = Substitute.For<Action>();
            action.Thing.Returns(thing);

            var buffer = _fixture.Create<byte[]>();
            _serializer
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings))
                .Returns(buffer);

            webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);

            _notify.Notify(_fixture.Create<object>(),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, action));

            _serializer
                .Received(1)
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings));

            webSocket
                .Received(1)
                .SendAsync(buffer, WebSocketMessageType.Text, true, Arg.Any<CancellationToken>());
        }
    }
}
