using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using AutoFixture;
using Mozilla.IoT.WebThing.Json;
using Mozilla.IoT.WebThing.Notify;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Notify
{
    public class NotifySubscribesOnPropertyChangedTest
    {
        private readonly Fixture _fixture;
        private readonly IJsonSerializer _serializer;
        private readonly IJsonSerializerSettings _serializerSettings;
        private readonly NotifySubscribesOnPropertyChanged _notify;

        public NotifySubscribesOnPropertyChangedTest()
        {
            _fixture = new Fixture();
            _serializer = Substitute.For<IJsonSerializer>();
            _serializerSettings = Substitute.For<IJsonSerializerSettings>();
            _notify = new NotifySubscribesOnPropertyChanged(_serializer, _serializerSettings);
        }

        [Fact]
        public void Notify_Should_DoNothing_When_SenderIsNotProperty()
        {
            _notify.Notify(_fixture.Create<object>(), new ValueChangedEventArgs(_fixture.Create<object>()));

            _serializer
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Any<IJsonSerializerSettings>());
        }
        
        
        [Fact]
        public void Notify_Should_DoNothing_When_HaveNotSubscribers()
        {
            var property = _fixture.Build<Property>()
                .With(x => x.Thing, new Thing())
                .Create();
            
            _notify.Notify(property, new ValueChangedEventArgs(_fixture.Create<object>()));

            _serializer
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Any<IJsonSerializerSettings>());
        }
        
        
        [Fact]
        public void Notify_Should_Send_When_HaveSubscribers()
        {
            var thing = _fixture.Create<Thing>();
            var property = _fixture.Build<Property>()
                .With(x => x.Thing, thing)
                .Create();

            var socket = Substitute.For<WebSocket>();
            thing.Subscribers.TryAdd(_fixture.Create<Guid>(), socket);
            
            var buffer = _fixture.Create<byte[]>();
            _serializer.Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings))
                .Returns(buffer);
            
            _notify.Notify(property, new ValueChangedEventArgs(_fixture.Create<object>()));

            _serializer
                .Received(1)
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Is(_serializerSettings));
            
            socket
                .Received(1)
                .SendAsync(buffer, WebSocketMessageType.Text, true, Arg.Any<CancellationToken>());
        }
    }
}
