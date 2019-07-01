using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSubstitute;
using Xunit;

using static Xunit.Assert;

namespace Mozilla.IoT.WebThing.Test
{
    public class ThingTest
    {
        private readonly Fixture _fixture;

        public ThingTest()
        {
            _fixture = new Fixture();
        }

        #region Property

        [Fact]
        public void AddProperty()
        {
            var thing = _fixture.Create<Thing>();
            var property = _fixture.Create<Property<int>>();
            
            thing.AddProperty(property);

            True(thing.ContainsProperty(property.Name));
        }
        
        [Fact]
        public void RemoveProperty()
        {
            var thing = _fixture.Create<Thing>();
            var property = _fixture.Create<Property<int>>();
            
            thing.AddProperty(property);
            True(thing.ContainsProperty(property.Name));
            thing.RemoveProperty(property);
            False(thing.ContainsProperty(property.Name));
        }
        
        [Fact]
        public void ContainsProperty()
        {
            var thing = _fixture.Create<Thing>();
            var property = _fixture.Create<Property<int>>();
            
            thing.AddProperty(property);

            True(thing.ContainsProperty(property.Name));
            False(thing.ContainsProperty(_fixture.Create<string>()));
        }
        
        [Fact]
        public void FindProperty()
        {
            var thing = _fixture.Create<Thing>();
            var property = _fixture.Create<Property<int>>();
            
            thing.AddProperty(property);
            True(thing.ContainsProperty(property.Name));

            Property find = thing.FindProperty(property.Name);
            NotNull(find);

            find = thing.FindProperty(_fixture.Create<string>());
            Null(find);
            
            Property<int> findInt = thing.FindProperty<int>(property.Name);
            NotNull(findInt);
            
            findInt = thing.FindProperty<int>(_fixture.Create<string>());
            Null(findInt);
        }
        
        [Fact]
        public void GetProperty()
        {
            var thing = _fixture.Create<Thing>();
            var property = _fixture.Create<Property<int>>();
            property.Value = _fixture.Create<int>();
            
            thing.AddProperty(property);
            True(thing.ContainsProperty(property.Name));

            object value = thing.GetProperty(property.Name);
            NotNull(value);
            IsType<int>(value);
            Equal(property.Value, (int)value);

            int @int = thing.GetProperty<int>(property.Name);
            Equal(property.Value, @int);
        }

        [Fact]
        public void SetProperty()
        {
            var thing = _fixture.Create<Thing>();
            var property = _fixture.Create<Property<int>>();
            property.Value = _fixture.Create<int>();
            
            thing.AddProperty(property);
            
            int newValue = _fixture.Create<int>();
            int oldValue = property.Value;
            thing.SetProperty(property.Name, newValue);
            
            int value = thing.GetProperty<int>(property.Name);
            NotEqual(oldValue, value);
            Equal(newValue, value);
            
            thing.SetProperty(_fixture.Create<string>(), _fixture.Create<int>());
        }

        [Fact]
        public async Task PropertyNotifyAsync()
        {
            var thing = _fixture.Create<Thing>();
            var property = _fixture.Create<Property<int>>();
            property.Value = _fixture.Create<int>();
            
            thing.AddProperty(property);
            thing.AddSubscriber(Substitute.For<WebSocket>());

            await thing.PropertyNotifyAsync(property, CancellationToken.None);
        }

        #endregion

        #region Event

        [Fact]
        public async Task AddEventAsync_Without_AddAvailableEvent()
        {
            var thing = _fixture.Create<Thing>();
            await thing.AddEventAsync(_fixture.Create<Event>(), CancellationToken.None);
        }
        
        [Fact]
        public async Task AddEventAsync_With_AddAvailableEvent_Without_WebSocket()
        {
            var thing = _fixture.Create<Thing>();
            
            string name = _fixture.Create<string>();
            thing.AddAvailableEvent(name);
            
            var @event = new Event(thing, name); 
            await thing.AddEventAsync(@event, CancellationToken.None);
        }
        
        [Fact]
        public async Task AddEventAsync_With_AddAvailableEvent_With_WebSocket()
        {
            var thing = _fixture.Create<Thing>();
            
            string name = _fixture.Create<string>();
            thing.AddAvailableEvent(name);

            var ws = Substitute.For<WebSocket>();
            
            
            thing.AddEventSubscriber(name, ws);
            
            var @event = new Event(thing, name); 
            await thing.AddEventAsync(@event, CancellationToken.None);
        }
        
        [Fact]
        public void AddAvailableEvent()
        {
            var thing = _fixture.Create<Thing>();

            thing.AddAvailableEvent(_fixture.Create<string>());
            thing.AddAvailableEvent(_fixture.Create<string>(), new JObject());
        }

        [Fact]
        public void AddEventSubscriber()
        {
            var thing = _fixture.Create<Thing>();
            string name = _fixture.Create<string>();
            thing.AddAvailableEvent(name);
            
            thing.AddEventSubscriber(name, Substitute.For<WebSocket>());
        }
        
        [Fact]
        public void RemoveEventSubscriber()
        {
            var thing = _fixture.Create<Thing>();
            string name = _fixture.Create<string>();
            thing.AddAvailableEvent(name);

            var ws = Substitute.For<WebSocket>();
            thing.AddEventSubscriber(name, ws);
            
            thing.RemoveEventSubscriber(name, ws);
            thing.RemoveEventSubscriber(name, Substitute.For<WebSocket>());
        }
        
        [Fact]
        public async Task GetEventDescriptions()
        {
            var thing = _fixture.Create<Thing>();
            
            string name1 = _fixture.Create<string>();
            thing.AddAvailableEvent(name1);
            
            string name2 = _fixture.Create<string>();
            thing.AddAvailableEvent(name2);
            
            var @event1 = new Event(thing, name1); 
            await thing.AddEventAsync(@event1, CancellationToken.None);
            
            var @event2 = new Event(thing, name2); 
            await thing.AddEventAsync(@event2, CancellationToken.None);

            JArray array = thing.GetEventDescriptions();
            True(array.Count == 2);
            
            array = thing.GetEventDescriptions(name2);
            True(array.Count == 1);
        }

        #endregion
        
        #region Action

        [Fact]
        public void AddAvailableAction()
        {
            var thing = _fixture.Create<Thing>();
            thing.AddAvailableAction<TestAction>(_fixture.Create<string>());
        }
        
        [Fact]
        public void GetAction_Not_Exits()
        {
            var thing = _fixture.Create<Thing>(); 
            Null(thing.GetAction(_fixture.Create<string>(), _fixture.Create<string>()));
        }
        
        [Fact]
        public async Task RemoveAction_Exits_Action()
        {
            var thing = _fixture.Create<Thing>();
            string name = _fixture.Create<string>();
            thing.AddAvailableAction<TestAction>(name);
            
            var action =  await thing.PerformActionAsync(name, null, CancellationToken.None);
            True(thing.RemoveAction(name, action.Id));
        }
        
        [Fact]
        public void RemoveAction_Not_Exits_Action()
        {
            var thing = _fixture.Create<Thing>();
            thing.AddAvailableAction<TestAction>(_fixture.Create<string>());
            False(thing.RemoveAction(_fixture.Create<string>(), _fixture.Create<string>()));
        }

        [Fact]
        public async Task PerformActionAsync_Not_Add()
        {
            var thing = _fixture.Create<Thing>();
            thing.AddAvailableAction<TestAction>(_fixture.Create<string>());

            await thing.PerformActionAsync(_fixture.Create<string>(), null, CancellationToken.None);
        }
        
        [Fact]
        public async Task PerformActionAsync_Without_Subscribe()
        {
            var thing = _fixture.Create<Thing>();
            string name = _fixture.Create<string>();
            thing.AddAvailableAction<TestAction>(name);

            NotNull(await thing.PerformActionAsync(name, null, CancellationToken.None));
        }
        
        [Fact]
        public async Task PerformActionAsync_With_Subscribe()
        {
            var thing = _fixture.Create<Thing>();
            string name = _fixture.Create<string>();
            thing.AddAvailableAction<TestAction>(name);
            thing.AddSubscriber(Substitute.For<WebSocket>());
            
            var action = new TestAction(thing, null);

            NotNull(await thing.PerformActionAsync(name, null, CancellationToken.None));
        }
        #endregion

        #region Subscribe

        [Fact]
        public void RemoveSubscriber_Without_Any_Subscribe()
        {
            var thing = _fixture.Create<Thing>();
            thing.RemoveSubscriber(Substitute.For<WebSocket>());
        }
        
        [Fact]
        public void RemoveSubscriber_With_Subscribe()
        {
            var thing = _fixture.Create<Thing>();

            var ws = Substitute.For<WebSocket>();
            thing.AddSubscriber(ws);
            thing.RemoveSubscriber(ws);
        }
        
        [Fact]
        public void RemoveSubscriber_With_Subscribe_And_Event_With_Subscribe()
        {
            var thing = _fixture.Create<Thing>();
            
            var ws = Substitute.For<WebSocket>();
            thing.AddSubscriber(ws);
            thing.AddEventSubscriber(_fixture.Create<string>(), ws);
            
            thing.RemoveSubscriber(ws);
        }    

        #endregion

        #region Description
        [Fact]
        public void AsThingDescription_Without_Actions_Events_Description_UiHref()
        {
            var thing = _fixture.Create<Thing>();
            thing.UiHref = null;
            var description = thing.AsThingDescription();
            

            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""name"": ""{thing.Name}"",
                ""href"": ""{thing.HrefPrefix}"",
                ""@context"": ""{thing.Context}"",
                ""@type"": {thing.Type},
                ""properties"": {thing.GetPropertyDescriptions()}, 
                ""actions"": {{}},
                ""events"": {{}},
                ""links"": [{{
                    ""rel"": ""properties"",
                    ""href"": ""{thing.HrefPrefix}properties"",
                 }}, {{
                    ""rel"": ""actions"",
                    ""href"": ""{thing.HrefPrefix}actions"",
                 }}, {{
                    ""rel"": ""events"",
                    ""href"": ""{thing.HrefPrefix}events"",
                 }}]
            }}");
            
            True(JToken.DeepEquals(json, description));
        }
        
        [Fact]
        public void AsThingDescription_With_UiHref()
        {
            var thing = _fixture.Create<Thing>();
            thing.UiHref = _fixture.Create<string>();
            
            var description = thing.AsThingDescription();

            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""name"": ""{thing.Name}"",
                ""href"": ""{thing.HrefPrefix}"",
                ""@context"": ""{thing.Context}"",
                ""@type"": {thing.Type},
                ""properties"": {thing.GetPropertyDescriptions()}, 
                ""actions"": {{}},
                ""events"": {{}},
                ""links"": [{{
                    ""rel"": ""properties"",
                    ""href"": ""{thing.HrefPrefix}properties"",
                 }}, {{
                    ""rel"": ""actions"",
                    ""href"": ""{thing.HrefPrefix}actions"",
                 }}, {{
                    ""rel"": ""events"",
                    ""href"": ""{thing.HrefPrefix}events"",
                 }}, {{
                    ""rel"": ""alternate"",
                    ""mediaType"": ""text/html"",
                    ""href"": ""{thing.UiHref}"",
                 }}]
            }}");
            
            True(JToken.DeepEquals(json, description));
        }
        
        [Fact]
        public void AsThingDescription_With_Description()
        {
            var thing = new Thing(_fixture.Create<string>(), null, _fixture.Create<string>());
            var description = thing.AsThingDescription();

            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""name"": ""{thing.Name}"",
                ""href"": ""{thing.HrefPrefix}"",
                ""@context"": ""{thing.Context}"",
                ""@type"": {thing.Type},
                ""properties"": {thing.GetPropertyDescriptions()},
                ""description"": ""{thing.Description}"",
                ""actions"": {{}},
                ""events"": {{}},
                ""links"": [{{
                    ""rel"": ""properties"",
                    ""href"": ""{thing.HrefPrefix}properties"",
                 }}, {{
                    ""rel"": ""actions"",
                    ""href"": ""{thing.HrefPrefix}actions"",
                 }}, {{
                    ""rel"": ""events"",
                    ""href"": ""{thing.HrefPrefix}events"",
                 }}]
            }}");
            
            True(JToken.DeepEquals(json, description));
        }
        
        [Fact]
        public void AsThingDescription_With_Action()
        {
            var thing = new Thing(_fixture.Create<string>(), null, _fixture.Create<string>());
            string actionName = _fixture.Create<string>();
            thing.AddAvailableAction<TestAction>(actionName);
            var description = thing.AsThingDescription();

            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""name"": ""{thing.Name}"",
                ""href"": ""{thing.HrefPrefix}"",
                ""@context"": ""{thing.Context}"",
                ""@type"": {thing.Type},
                ""properties"": {thing.GetPropertyDescriptions()},
                ""description"": ""{thing.Description}"",
                ""actions"": {{
                    ""{actionName}"": {{
                        ""links"": [{{
                            ""rel"": ""action"",
                            ""href"": ""{thing.HrefPrefix}actions/{actionName}""
                        }}]
                    }}
                }},
                ""events"": {{}},
                ""links"": [{{
                    ""rel"": ""properties"",
                    ""href"": ""{thing.HrefPrefix}properties"",
                 }}, {{
                    ""rel"": ""actions"",
                    ""href"": ""{thing.HrefPrefix}actions"",
                 }}, {{
                    ""rel"": ""events"",
                    ""href"": ""{thing.HrefPrefix}events"",
                 }}]
            }}");
            
            True(JToken.DeepEquals(json, description));
        }
        
        [Fact]
        public async Task AsThingDescription_With_Events()
        {
            var thing = new Thing(_fixture.Create<string>(), null, _fixture.Create<string>());
            
            string actionName = _fixture.Create<string>();
            thing.AddAvailableAction<TestAction>(actionName);

            string eventName = _fixture.Create<string>();
            thing.AddAvailableEvent(eventName);
            await thing.AddEventAsync(new Event(thing, eventName), CancellationToken.None);
            
            var description = thing.AsThingDescription();

            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""name"": ""{thing.Name}"",
                ""href"": ""{thing.HrefPrefix}"",
                ""@context"": ""{thing.Context}"",
                ""@type"": {thing.Type},
                ""properties"": {thing.GetPropertyDescriptions()},
                ""description"": ""{thing.Description}"",
                ""actions"": {{
                    ""{actionName}"": {{
                        ""links"": [{{
                            ""rel"": ""action"",
                            ""href"": ""{thing.HrefPrefix}actions/{actionName}""
                        }}]
                    }}
                }},
                ""events"": {{
                    ""{eventName}"": {{
                        ""links"": [{{
                            ""rel"": ""event"",
                            ""href"": ""{thing.HrefPrefix}event/{eventName}""
                        }}]
                    }}
                }},
                ""links"": [{{
                    ""rel"": ""properties"",
                    ""href"": ""{thing.HrefPrefix}properties"",
                 }}, {{
                    ""rel"": ""actions"",
                    ""href"": ""{thing.HrefPrefix}actions"",
                 }}, {{
                    ""rel"": ""events"",
                    ""href"": ""{thing.HrefPrefix}events"",
                 }}]
            }}");
            
            True(JToken.DeepEquals(json, description));
        }
        
        [Fact]
        public async Task AsThingDescription_With_Property()
        {
            var thing = new Thing(_fixture.Create<string>(), null, _fixture.Create<string>());
            
            string actionName = _fixture.Create<string>();
            thing.AddAvailableAction<TestAction>(actionName);

            string eventName = _fixture.Create<string>();
            thing.AddAvailableEvent(eventName);
            await thing.AddEventAsync(new Event(thing, eventName), CancellationToken.None);

            var property = new Property<int>(thing, _fixture.Create<string>(), _fixture.Create<int>());
            thing.AddProperty(property);
            
            var description = thing.AsThingDescription();

            var json = JsonConvert.DeserializeObject<JObject>($@"{{
                ""name"": ""{thing.Name}"",
                ""href"": ""{thing.HrefPrefix}"",
                ""@context"": ""{thing.Context}"",
                ""@type"": {thing.Type},
                ""properties"": {thing.GetPropertyDescriptions()},
                ""description"": ""{thing.Description}"",
                ""actions"": {{
                    ""{actionName}"": {{
                        ""links"": [{{
                            ""rel"": ""action"",
                            ""href"": ""{thing.HrefPrefix}actions/{actionName}""
                        }}]
                    }}
                }},
                ""events"": {{
                    ""{eventName}"": {{
                        ""links"": [{{
                            ""rel"": ""event"",
                            ""href"": ""{thing.HrefPrefix}event/{eventName}""
                        }}]
                    }}
                }},
                ""links"": [{{
                    ""rel"": ""properties"",
                    ""href"": ""{thing.HrefPrefix}properties"",
                 }}, {{
                    ""rel"": ""actions"",
                    ""href"": ""{thing.HrefPrefix}actions"",
                 }}, {{
                    ""rel"": ""events"",
                    ""href"": ""{thing.HrefPrefix}events"",
                 }}]
            }}");
            
            True(JToken.DeepEquals(json, description));
        }

        [Fact]
        public void GetActionDescriptions()
        {
            var thing = _fixture.Create<Thing>();
            thing.AddAvailableAction<TestAction>(_fixture.Create<string>());

            var array = thing.GetActionDescriptions();
        }
        
        [Fact]
        public void GetActionDescriptions_With_Name()
        {
            var thing = _fixture.Create<Thing>();
            string name = _fixture.Create<string>();
            thing.AddAvailableAction<TestAction>(name);

            var array = thing.GetActionDescriptions(name);
        }
        
        [Fact]
        public void GetActionDescriptions_With_Not_Action_Register()
        {
            var thing = _fixture.Create<Thing>();
            string name = _fixture.Create<string>();
            thing.AddAvailableAction<TestAction>(name);
            
            var array = thing.GetActionDescriptions(_fixture.Create<string>());
        }
        #endregion
        
        private class TestAction : Mozilla.IoT.WebThing.Action
        {
            public TestAction(Thing thing, JObject input) : base(thing, input)
            {
            }

            public override string Id { get; } = Guid.NewGuid().ToString();
            public override string Name { get; } = "test";
            protected override Task PerformActionAsync(CancellationToken cancellation) 
                => Task.CompletedTask;
        }
    }
}
