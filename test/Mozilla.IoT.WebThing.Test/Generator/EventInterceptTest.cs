using System;
using System.Collections.Generic;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Converts;
using Mozilla.IoT.WebThing.Factories;
using Mozilla.IoT.WebThing.Factories.Generator.Events;
using NSubstitute;
using Xunit;

namespace Mozilla.IoT.WebThing.Test.Generator
{
    public class EventInterceptTest
    {
        private readonly Fixture _fixture;
        private readonly JsonSerializerOptions _options;

        public EventInterceptTest()
        {
            _fixture = new Fixture();
            _options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
        }


        [Fact]
        public void Valid()
        {
            var thing = new LampThing();
            var eventFactory = new EventInterceptFactory(thing, _options);
            
            CodeGeneratorFactory.Generate(thing, new []{ eventFactory });
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IPropertiesOld>(),
                eventFactory.Events,
                new Dictionary<string, ActionContext>());

            var @int = _fixture.Create<int>();
            thing.Emit(@int);
            var events = thing.ThingContext.Events[nameof(LampThing.Int)].ToArray();
            events.Should().HaveCount(1);
            events[0].Data.Should().Be(@int);

            var @decimal = _fixture.Create<decimal>();
            thing.Emit(@decimal);
            events = thing.ThingContext.Events[nameof(LampThing.Decimal)].ToArray();
            events.Should().HaveCount(1);
            events[0].Data.Should().Be(@decimal);
            
            thing.Emit((decimal?)null);
            events = thing.ThingContext.Events[nameof(LampThing.Decimal)].ToArray();
            events.Should().HaveCount(2);
            events[1].Data.Should().Be(null);
            
            var @dateTime = _fixture.Create<DateTime>();
            thing.Emit(dateTime);
            events = thing.ThingContext.Events[nameof(LampThing.DateTime)].ToArray();
            events.Should().HaveCount(1);
            events[0].Data.Should().Be(@dateTime);
            
            var @obj = _fixture.Create<object>();
            thing.Emit(obj);
            events = thing.ThingContext.Events[nameof(LampThing.Any)].ToArray();
            events.Should().HaveCount(1);
            events[0].Data.Should().Be(@obj);
        }
        
        [Fact]
        public void InvalidEvent()
        {
            var thing = new LampThing();
            var eventFactory = new EventInterceptFactory(thing, _options);
            
            CodeGeneratorFactory.Generate(thing, new []{ eventFactory });
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IPropertiesOld>(),
                eventFactory.Events,
                new Dictionary<string, ActionContext>());

            var @int = _fixture.Create<int>();
            thing.EmitInvalid(@int);
            var events = thing.ThingContext.Events[nameof(LampThing.Int)].ToArray();
            events = thing.ThingContext.Events[nameof(LampThing.Int)].ToArray();
            events.Should().BeEmpty();
        }
        
        [Fact]
        public void Ignore()
        {
            var thing = new LampThing();
            var eventFactory = new EventInterceptFactory(thing, _options);
            
            CodeGeneratorFactory.Generate(thing, new []{ eventFactory });
            
            thing.ThingContext = new Context(Substitute.For<IThingConverter>(),
                Substitute.For<IPropertiesOld>(),
                eventFactory.Events,
                new Dictionary<string, ActionContext>());

            var @int = _fixture.Create<int>();
            thing.EmitIgnore(@int);
            var events = thing.ThingContext.Events[nameof(LampThing.Int)].ToArray();
            events.Should().BeEmpty();
        }
        
        public class LampThing : Thing
        {
            public override string Name => nameof(LampThing);

            public event Action<int> InvalidEvent;

            [ThingEvent(Ignore = true)]
            public event EventHandler<int> Ignore;
            
            public event EventHandler<int> Int;
            public event EventHandler<DateTime> DateTime;
            public event EventHandler<decimal?> Decimal;
            public event EventHandler<object> Any;

            internal void EmitInvalid(int value)
                => InvalidEvent?.Invoke(value);
            
            internal void EmitIgnore(int value) 
                => Ignore?.Invoke(this, value);
            
            internal void Emit(int value) 
                => Int?.Invoke(this, value);
            
            internal void Emit(decimal? value) 
                => Decimal?.Invoke(this, value);
            
            internal void Emit(DateTime value) 
                => DateTime?.Invoke(this, value);
            
            internal void Emit(object value) 
                => Any?.Invoke(this, value);
        }
    }
}
