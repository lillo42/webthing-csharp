
using System.Collections.Generic;
using Mozilla.IoT.WebThing.Activator;
#if DEBUG
using System.Linq;
using System;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Mozilla.IoT.WebThing.Collections;
using NSubstitute;
using Xunit;

using static Xunit.Assert;

namespace Mozilla.IoT.WebThing.Test.Activator
{
    public class ThingActivatorTest
    {
        private readonly Fixture _fixture;
        private readonly ThingBindingOption _option;
        private readonly ThingActivator _activator;
        private readonly IServiceProvider _service;

        public ThingActivatorTest()
        {
            _fixture = new Fixture();
            _service = Substitute.For<IServiceProvider>();
            _option = new ThingBindingOption();
            
            _activator = new ThingActivator(_option);
        }

        [Fact]
        public void Register_Should_RegisterInstanceAndNotUpdateHref_When_IsSingle()
        {
            var thing = _fixture.Create<Thing>();

            _service.GetService(Arg.Any<Type>())
                .Returns(called =>
                {
                    var type = called.Args()[0] as Type;

                    return type == typeof(IObservableCollection<Event>) ? 
                        new DefaultObservableCollection<Event>() : null;
                });

            _option.IsSingleThing = true;
            
            _activator.Register(_service, thing);

            _activator._thingType.Should().HaveCount(1);
            _activator._thingType.First().Key.Should().Be(thing.Name);
            _activator._thingType.First().Value.Should().Be(typeof(Thing));
            _activator._typeActivatorCache.Should().HaveCount(1);
            _activator._typeActivatorCache.First().Key.Should().Be(typeof(Thing));
            _activator._typeActivatorCache.First().Value.Should().Be(thing);

            thing.Events.Should().NotBeNull();
        }
        
        [Fact]
        public void Register_Should_RegisterInstanceAndUpdateHref_When_IsNotSingle()
        {
            var thing = _fixture.Create<Thing>();

            _service.GetService(Arg.Any<Type>())
                .Returns(called =>
                {
                    var type = called.Args()[0] as Type;

                    return type == typeof(IObservableCollection<Event>) ? 
                        new DefaultObservableCollection<Event>() : null;
                });

            _option.IsSingleThing = false;
            
            _activator.Register(_service, thing);

            _activator._thingType.Should().HaveCount(1);
            _activator._thingType.First().Key.Should().Be(thing.Name);
            _activator._thingType.First().Value.Should().Be(typeof(Thing));
            _activator._typeActivatorCache.Should().HaveCount(1);
            _activator._typeActivatorCache.First().Key.Should().Be(typeof(Thing));
            _activator._typeActivatorCache.First().Value.Should().Be(thing);

            thing.Events.Should().NotBeNull();
            thing.HrefPrefix.Should().Be($"/things/{thing.Name}");
        }
        
        [Fact]
        public void Register_Should_RegisterWithCustomNameAndNotUpdateHref_When_IsSingle()
        {
            _service.GetService(Arg.Any<Type>())
                .Returns(called =>
                {
                    var type = called.Args()[0] as Type;

                    return type == typeof(IObservableCollection<Event>) ? 
                        new DefaultObservableCollection<Event>() : null;
                });

            string name = _fixture.Create<string>();

            _option.IsSingleThing = true;
            
            _activator.Register<Thing>(_service, name);

            _activator._thingType.Should().HaveCount(1);
            _activator._thingType.First().Key.Should().Be(name);
            _activator._thingType.First().Value.Should().Be(typeof(Thing));
            _activator._typeActivatorCache.Should().HaveCount(1);
            _activator._typeActivatorCache.First().Key.Should().Be(typeof(Thing));
        }
        
        [Fact]
        public void Register_Should_RegisterWithCustomNameAndUpdateHref_When_IsNotSingle()
        {
            _service.GetService(Arg.Any<Type>())
                .Returns(called =>
                {
                    var type = called.Args()[0] as Type;
                    
                    return type == typeof(IObservableCollection<Event>) ? 
                        new DefaultObservableCollection<Event>() : null;
                });

            _option.IsSingleThing = false;
            
            string name = _fixture.Create<string>();
            
            _activator.Register<Thing>(_service, name);

            _activator._thingType.Should().HaveCount(1);
            _activator._thingType.First().Key.Should().Be(name);
            _activator._thingType.First().Value.Should().Be(typeof(Thing));
            _activator._typeActivatorCache.Should().HaveCount(1);
            _activator._typeActivatorCache.First().Key.Should().Be(typeof(Thing));
            var thing =  _activator._typeActivatorCache.First().Value;

            thing.HrefPrefix.Should().Be($"/things/{name}");
        }
        
        [Fact]
        public void Register_Should_RegisterWTypeAndNotUpdateHref_When_IsSingle()
        {
            _service.GetService(Arg.Any<Type>())
                .Returns(called =>
                {
                    var type = called.Args()[0] as Type;

                    return type == typeof(IObservableCollection<Event>) ? 
                        new DefaultObservableCollection<Event>() : null;
                });

            _option.IsSingleThing = true;
            
            _activator.Register<SampleThing>(_service);

            _activator._thingType.Should().HaveCount(1);
            _activator._thingType.First().Key.Should().Be("Sample");
            _activator._thingType.First().Value.Should().Be(typeof(SampleThing));
            _activator._typeActivatorCache.Should().HaveCount(1);
            _activator._typeActivatorCache.First().Key.Should().Be(typeof(SampleThing));
        }
        
        [Fact]
        public void Register_Should_RegisterTypeAndUpdateHref_When_IsNotSingle()
        {
            _service.GetService(Arg.Any<Type>())
                .Returns(called =>
                {
                    var type = called.Args()[0] as Type;
                    
                    if (type == typeof(IObservableCollection<Event>))
                    {
                        return new DefaultObservableCollection<Event>();
                    }

                    return null;
                });

            _option.IsSingleThing = false;
            
            _activator.Register<SampleThing>(_service);

            _activator._thingType.Should().HaveCount(1);
            _activator._thingType.First().Key.Should().Be("Sample");
            _activator._thingType.First().Value.Should().Be(typeof(SampleThing));
            _activator._typeActivatorCache.Should().HaveCount(1);
            _activator._typeActivatorCache.First().Key.Should().Be(typeof(SampleThing));
            var thing =  _activator._typeActivatorCache.First().Value;

            thing.HrefPrefix.Should().Be($"/things/Sample");
        }

        [Fact]
        public void CreateInstance_Should_Throw_When_ServiceIsNull()
        {
            Throws<ArgumentNullException>(() => _activator.CreateInstance(null, _fixture.Create<string>()));
        }
        
        [Fact]
        public void CreateInstance_Should_Throw_When_ThingNameIsNullAndHaveNotType()
        {
            Throws<ArgumentNullException>(() => _activator.CreateInstance(_service, null));
        }
        
        [Fact]
        public void CreateInstance_Should_Throw_When_ThingNameIsNullAndHaveMoreThenType()
        {
            _activator._thingType.Add(_fixture.Create<string>(), null);
            _activator._thingType.Add(_fixture.Create<string>(), null);
            Throws<ArgumentNullException>(() => _activator.CreateInstance(_service, null));
        }
        
        [Fact]
        public void CreateInstance_Should_ReturnInstance_When_InstanceIsCache()
        {
            string name = _fixture.Create<string>();
            var thing = _fixture.Create<SampleThing>();
            
            _activator._thingType.Add(name, typeof(SampleThing));
            _activator._typeActivatorCache.TryAdd(typeof(SampleThing), thing);
            
            var result = _activator.CreateInstance(_service, name);
            result.Should().NotBeNull();
            result.Should().Be(thing); 
        }
        
        [Fact]
        public void CreateInstance_Should_ReturnNewInstance_When_InstanceIsNotCache()
        {
            _service.GetService(Arg.Any<Type>())
                .Returns(called =>
                {
                    var type = called.Args()[0] as Type;

                    return type == typeof(IObservableCollection<Event>) ? 
                        new DefaultObservableCollection<Event>() : null;
                });
            
            string name = _fixture.Create<string>();
            
            _activator._thingType.Add(name, typeof(SampleThing));
            
            var result = _activator.CreateInstance(_service, name);
            result.Should().NotBeNull();
        }
        
        [Fact]
        public void CreateInstance_Should_ReturnInstance_When_InstanceIsCacheAndHaveOnly1AndNameIsNull()
        {
            _service.GetService(Arg.Any<Type>())
                .Returns(called =>
                {
                    var type = called.Args()[0] as Type;

                    return type == typeof(IObservableCollection<Event>) ? 
                        new DefaultObservableCollection<Event>() : null;
                });
            
            string name = _fixture.Create<string>();
            
            _activator._thingType.Add(name, typeof(SampleThing));
            
            var result = _activator.CreateInstance(_service, name);
            result.Should().NotBeNull();
            
            result = _activator.CreateInstance(_service, null);
            result.Should().NotBeNull();
        }

        [Fact]
        public void GetEnumerator_Should_ReturnAllInstance()
        {
            _service.GetService(Arg.Any<Type>())
                .Returns(called =>
                {
                    var type = called.Args()[0] as Type;

                    return type == typeof(IObservableCollection<Event>) ? 
                        new DefaultObservableCollection<Event>() : null;
                });
            
            string name = _fixture.Create<string>();
            
            _activator._thingType.Add(name, typeof(SampleThing));
            
            _activator.CreateInstance(_service, name);

            foreach (Thing thing in _activator)
            {
                thing.Should().NotBeNull();
            }
        }

        private class SampleThing : Thing
        {
            
        }
    }
}
#endif
