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
    public class NotifySubscribesOnEventAddedTest
    {
        private readonly Thing _thing;
        private readonly Fixture _fixture;
        private readonly IDescriptor<Event> _descriptor;
        private readonly IJsonConvert _convert;
        private readonly IJsonSerializerSettings _serializerSettings;
        private readonly NotifySubscribesOnEventAdded _notify;

        public NotifySubscribesOnEventAddedTest()
        {
            _fixture = new Fixture();
            _thing = _fixture.Create<Thing>();
            _descriptor = Substitute.For<IDescriptor<Event>>();
            _convert = Substitute.For<IJsonConvert>();
            _serializerSettings = Substitute.For<IJsonSerializerSettings>();
            _notify = new NotifySubscribesOnEventAdded(_thing, _descriptor, _convert, _serializerSettings);
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
            
            _convert
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings));
        }
        
        [Fact]
        public void Notify_Should_DoNothing_When_SendIsNotEvent()
        {
            _notify.Notify(_fixture.Create<object>(),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new object()));
            
            _convert
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings));
        }
        
        [Fact]
        public void Notify_Should_DoNothing_When_NotHaveAvailableEvent()
        {
            var @event = Substitute.For<Event>();

            _descriptor.CreateDescription(@event)
                .Returns(new Dictionary<string, object>());
            
            _notify.Notify(_fixture.Create<object>(),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, @event));

            _convert
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings));

            _descriptor
                .Received(1)
                .CreateDescription(@event);
        }
        
        [Fact]
        public void Notify_Should_DoNothing_When_HaveNotSubscribe()
        {
            var eventName = _fixture.Create<string>();
            
            var @event = Substitute.For<Event>();
            @event.Name.Returns(eventName);

            _thing.AddEvent(eventName);

            _descriptor.CreateDescription(@event)
                .Returns(new Dictionary<string, object>());
            
            _notify.Notify(_fixture.Create<object>(),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, @event));

            _convert
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings));

            _descriptor
                .Received(1)
                .CreateDescription(@event);
        }
        
        [Fact]
        public void Notify_Should_Send_When_HaveSubscribe()
        {
            var eventName = _fixture.Create<string>();
            
            var @event = Substitute.For<Event>();
            @event.Name.Returns(eventName);

            _thing.AddEvent(eventName);

            var socket = Substitute.For<WebSocket>();
            _thing.AvailableEvent[eventName].Subscribers.Add(socket);

            _descriptor.CreateDescription(@event)
                .Returns(new Dictionary<string, object>());

            var buffer = _fixture.Create<byte[]>();
            _convert.Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings))
                .Returns(buffer);
            
            socket
                .SendAsync(buffer, WebSocketMessageType.Text, true, Arg.Any<CancellationToken>())
                .Returns(Task.CompletedTask);
            
            _notify.Notify(_fixture.Create<object>(),
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, @event));

            _descriptor
                .Received(1)
                .CreateDescription(@event);
            
            _convert
                .Received(1)
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings));

            socket
                .Received(1)
                .SendAsync(buffer, WebSocketMessageType.Text, true, Arg.Any<CancellationToken>());

        }
    }
}
