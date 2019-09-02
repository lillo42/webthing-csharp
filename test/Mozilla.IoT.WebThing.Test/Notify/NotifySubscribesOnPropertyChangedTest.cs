using System.Collections.Generic;
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
        private readonly IJsonConvert _convert;
        private readonly IJsonSerializerSettings _serializerSettings;
        private readonly NotifySubscribesOnPropertyChanged _notify;

        public NotifySubscribesOnPropertyChangedTest()
        {
            _fixture = new Fixture();
            _convert = Substitute.For<IJsonConvert>();
            _serializerSettings = Substitute.For<IJsonSerializerSettings>();
            _notify = new NotifySubscribesOnPropertyChanged(_convert, _serializerSettings);
        }

        [Fact]
        public void Notify_Should_DoNothing_When_SenderIsNotProperty()
        {
            _notify.Notify(_fixture.Create<object>(), new ValueChangedEventArgs(_fixture.Create<object>()));

            _convert
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Any<IJsonSerializerSettings>());
        }
        
        
        [Fact]
        public void Notify_Should_DoNothing_When_HaveNotSubscribs()
        {
            var property = _fixture.Build<Property>()
                .With(x => x.Thing, new Thing())
                .Create();
            
            _notify.Notify(property, new ValueChangedEventArgs(_fixture.Create<object>()));

            _convert
                .DidNotReceive()
                .Serialize(Arg.Any<IDictionary<string, object>>(), Arg.Any<IJsonSerializerSettings>());
        }
    }
}
