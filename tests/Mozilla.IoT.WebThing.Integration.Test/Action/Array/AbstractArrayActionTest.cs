using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Action.Array
{
    public abstract class AbstractArrayActionTest<T>
    {
        protected IServiceProvider Provider { get; }
        protected IThingContextFactory Factory { get; }
        protected Fixture Fixture { get; }

        protected AbstractArrayActionTest()
        {
            var collection = new ServiceCollection();
            collection.AddThings();
            collection.AddLogging();
            
            Provider = collection.BuildServiceProvider().CreateScope().ServiceProvider;
            Factory = Provider.GetRequiredService<IThingContextFactory>();
            Fixture = new Fixture();
        }
        
        protected abstract JsonElement CreateJson(IEnumerable<T> values);
        protected abstract IEnumerable<JsonElement> CreateInvalidJson();

        #region Serialization
        private void Serialize<TThing>()
            where TThing : Thing, new()
        {
            var thing = new TThing();
            var context = Factory.Create(thing, new ThingOption());

            var message = JsonSerializer.Serialize(context.Response,
                new ThingOption().ToJsonSerializerOptions());

            var value = typeof(T).IsEnum
                ? $@", ""enum"": [""{string.Join(@""" , """, typeof(T).GetEnumNames())}""]"
                : string.Empty;
            
            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(string.Format(Response, thing.Name, typeof(T).ToJsonType().ToString().ToLower(), value)));
        }
        
        [Fact]
        public void SerializeArrayProperty()
            => Serialize<ArrayThing>();
        
        [Fact]
        public void SerializeIEnumerableProperty()
            => Serialize<IEnumerableThing>();
        
        [Fact]
        public void SerializeListProperty()
            => Serialize<ListThing>();
        
        [Fact]
        public void SerializeIListProperty()
            => Serialize<IListThing>();
        
        [Fact]
        public void SerializeICollectionProperty()
            => Serialize<ICollectionThing>();
        
        [Fact]
        public void SerializeISetProperty()
            => Serialize<ISetThing>();
        
        [Fact]
        public void SerializeHashSetProperty()
            => Serialize<HashSetThing>();
        
        [Fact]
        public void SerializeLinkedListProperty()
            => Serialize<LinkedListThing>();

        #endregion
        
        #region Valid

        private async Task ValidAction<TThing>(string actionName, int arrayLength, bool uniqueItems, bool acceptedNullValue,
            Func<List<T>, List<T>> transValue = null)
            where TThing : Thing, IThingCollection, new()
        {
            var thing = new TThing();
            var context = Factory.Create(thing, new ThingOption());
        
            thing.ThingContext = context;
        
            context.Properties.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(actionName);

            var values = CreateValue(arrayLength);

            if (uniqueItems)
            {
                values = values.ToHashSet().ToList();
            }
            
            var jsonElement = CreateJson(values);

            context.Actions[actionName].TryAdd(jsonElement, out var actionInformation).Should().BeTrue();
            
            await actionInformation.ExecuteAsync(thing, Provider);
            
            if (transValue != null)
            {
                values = transValue(values);
            }

            thing.Collection.Should().BeEquivalentTo(values);

            if(acceptedNullValue)
            { 
                jsonElement =JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action"); 
                context.Actions[actionName].TryAdd(jsonElement, out actionInformation).Should().BeTrue();
                await actionInformation.ExecuteAsync(thing, Provider);
                thing.Collection.Should().BeNull();
            }
        }
        
        [Theory]
        [InlineData(nameof(ArrayThing.Values), 4, false, true)]
        [InlineData(nameof(ArrayThing.NotNullable), 4, false, false)]
        [InlineData(nameof(ArrayThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(ArrayThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(ArrayThing.Unique), 2, true, true)]
        public async Task ValidArrayProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            await ValidAction<ArrayThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }
        
        [Theory]
        [InlineData(nameof(IEnumerableThing.Values), 4, false, true)]
        [InlineData(nameof(IEnumerableThing.NotNullable), 4, false, false)]
        [InlineData(nameof(IEnumerableThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(IEnumerableThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(IEnumerableThing.Unique), 2, true, true)]
        public async Task ValidIEnumerableProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            await ValidAction<IEnumerableThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }
        
        [Theory]
        [InlineData(nameof(ListThing.Values), 4, false, true)]
        [InlineData(nameof(ListThing.NotNullable), 4, false, false)]
        [InlineData(nameof(ListThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(ListThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(ListThing.Unique), 2, true, true)]
        public async Task ValidListProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            await ValidAction<ListThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }
        
        [Theory]
        [InlineData(nameof(IListThing.Values), 4, false, true)]
        [InlineData(nameof(IListThing.NotNullable), 4, false, false)]
        [InlineData(nameof(IListThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(IListThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(IListThing.Unique), 2, true, true)]
        public async Task ValidIListProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue) 
        {
            await ValidAction<IListThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }

        [Theory]
        [InlineData(nameof(ICollectionThing.Values), 4, false, true)]
        [InlineData(nameof(ICollectionThing.NotNullable), 4, false, false)]
        [InlineData(nameof(ICollectionThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(ICollectionThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(ICollectionThing.Unique), 2, true, true)]
        public async Task ValidICollectionProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue) 
        {
            await ValidAction<ICollectionThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }
        
        [Theory]
        [InlineData(nameof(ISetThing.Values), 4, false, true)]
        [InlineData(nameof(ISetThing.NotNullable), 4, false, false)]
        [InlineData(nameof(ISetThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(ISetThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(ISetThing.Unique), 2, true, true)]
        public async Task ValidISetProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            await ValidAction<ISetThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue, x => x.ToHashSet().ToList());
        }
        
        [Theory]
        [InlineData(nameof(HashSetThing.Values), 4, false, true)]
        [InlineData(nameof(HashSetThing.NotNullable), 4, false, false)]
        [InlineData(nameof(HashSetThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(HashSetThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(HashSetThing.Unique), 2, true, true)]
        public async Task ValidHashSetProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            await ValidAction<HashSetThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue, x => x.ToHashSet().ToList());
        }
        
        [Theory]
        [InlineData(nameof(LinkedListThing.Values), 4, false, true)]
        [InlineData(nameof(LinkedListThing.NotNullable), 4, false, false)]
        [InlineData(nameof(LinkedListThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(LinkedListThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(LinkedListThing.Unique), 2, true, true)]
        public async Task ValidLinkedListProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            await ValidAction<LinkedListThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }

        #endregion

        #region Invalid
        
        private void InvalidActions<TThing>(string actionMethod, int? arrayLength, bool duplicateValue, bool executeNull)
            where  TThing : Thing, IThingCollection, new()
        {
            var thing = new TThing();
            thing.Collection = Fixture.Create<IEnumerable<T>>();
            var context = Factory.Create(thing, new ThingOption());
        
            thing.ThingContext = context;
        
            context.Properties.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Actions.Should().NotBeEmpty();
            context.Actions.Should().ContainKey(actionMethod);

            if(arrayLength.HasValue)
            {
                var values = CreateValue(arrayLength.Value);

                if (duplicateValue)
                {
                    values.AddRange(values);
                }

                var jsonElement = CreateJson(values);
            
                context.Actions[actionMethod].TryAdd(jsonElement, out var t).Should().BeFalse();
                t.Should().BeNull();
                thing.Collection.Should().NotBeEquivalentTo(values);

            }

            var jsonElements = CreateInvalidJson();
            
            foreach (var element in jsonElements)
            {
                context.Actions[actionMethod].TryAdd(element, out _).Should().BeFalse();
            }
        
            if(executeNull)
            { 
                var jsonElement = JsonSerializer.Deserialize<JsonElement>($@"{{ ""action"": {{ ""input"": {{ ""value"": null }} }} }}").GetProperty("action"); 
                context.Actions[actionMethod].TryAdd(jsonElement, out _).Should().BeFalse();
            }
        }

        
        [Theory]
        [InlineData(nameof(ArrayThing.Values), null, false, false)]
        [InlineData(nameof(ArrayThing.NotNullable), null, false, true)]
        [InlineData(nameof(ArrayThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(ArrayThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(ArrayThing.Unique), 3, true, false)]
        public void InvalidArrayProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidActions<ArrayThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        [Theory]
        [InlineData(nameof(IEnumerableThing.Values), null, false, false)]
        [InlineData(nameof(IEnumerableThing.NotNullable), null, false, true)]
        [InlineData(nameof(IEnumerableThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(IEnumerableThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(IEnumerableThing.Unique), 3, true, false)]
        public void InvalidIEnumerableThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidActions<IEnumerableThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        [Theory]
        [InlineData(nameof(ListThing.Values), null, false, false)]
        [InlineData(nameof(ListThing.NotNullable), null, false, true)]
        [InlineData(nameof(ListThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(ListThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(ListThing.Unique), 3, true, false)]
        public void InvalidListThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidActions<ListThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        [Theory]
        [InlineData(nameof(IListThing.Values), null, false, false)]
        [InlineData(nameof(IListThing.NotNullable), null, false, true)]
        [InlineData(nameof(IListThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(IListThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(IListThing.Unique), 3, true, false)]
        public void InvalidIListThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidActions<IListThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        [Theory]
        [InlineData(nameof(ICollectionThing.Values), null, false, false)]
        [InlineData(nameof(ICollectionThing.NotNullable), null, false, true)]
        [InlineData(nameof(ICollectionThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(ICollectionThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(ICollectionThing.Unique), 3, true, false)]
        public void InvalidICollectionThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidActions<ICollectionThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        [Theory]
        [InlineData(nameof(LinkedListThing.Values), null, false, false)]
        [InlineData(nameof(LinkedListThing.NotNullable), null, false, true)]
        [InlineData(nameof(LinkedListThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(LinkedListThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(LinkedListThing.Unique), 3, true, false)]
        public void InvalidLinkedListThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidActions<LinkedListThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        
        [Theory]
        [InlineData(nameof(ISetThing.Values), null, false, false)]
        [InlineData(nameof(ISetThing.NotNullable), null, false, true)]
        [InlineData(nameof(ISetThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(ISetThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(ISetThing.Unique), 3, true, false)]
        public void InvalidISetThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidActions<ISetThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        
        [Theory]
        [InlineData(nameof(HashSetThing.Values), null, false, false)]
        [InlineData(nameof(HashSetThing.NotNullable), null, false, true)]
        [InlineData(nameof(HashSetThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(HashSetThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(HashSetThing.Unique), 3, true, false)]
        public void InvalidHashSetThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidActions<HashSetThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        #endregion

        protected virtual List<T> CreateValue(int arrayLength)
        {
            var values = new List<T>(arrayLength);

            for (var i = 0; i < arrayLength; i++)
            {
                values.Add(Fixture.Create<T>());
            }

            return values;
        }
        
        #region Things
        
        public interface IThingCollection
        {
            IEnumerable<T> Collection { get; set; }
        }
        
        public class ArrayThing : Thing, IThingCollection
        {
            public override string Name => "array-thing";
            
            [ThingProperty(Ignore = true)]
            public  IEnumerable<T> Collection { get; set; }
            
            public void Values(T[] value)
            {
                Collection = value;
            }
            
            public void NotNullable([ThingParameter(IsNullable = false)]T[] value)
            {
                Collection = value;
            }
            
            public void MinAndMax([ThingParameter(MinimumItems = 2, MaximumItems = 3)]T[] value)
            {
                Collection = value;
            }
            
            public void Unique([ThingParameter(UniqueItems = true)]T[] value)
            {
                Collection = value;
            }
        }
        
        public class IEnumerableThing : Thing, IThingCollection
        {
            public override string Name => "ienumerable-thing";
            
            [ThingProperty(Ignore = true)]
            public IEnumerable<T> Collection { get; set; }


            public void Values(IEnumerable<T> value)
            {
                Collection = value;
            }
            
            public void NotNullable([ThingParameter(IsNullable = false)]IEnumerable<T> value)
            {
                Collection = value;
            }
            
            public void MinAndMax([ThingParameter(MinimumItems = 2, MaximumItems = 3)]IEnumerable<T> value)
            {
                Collection = value;
            }
            
            public void Unique([ThingParameter(UniqueItems = true)]IEnumerable<T> value)
            {
                Collection = value;
            }
        }

        public class ListThing : Thing, IThingCollection
        {
            public override string Name => "list-thing";
            
            [ThingProperty(Ignore = true)]
            public IEnumerable<T> Collection { get; set; }


            public void Values(List<T> value)
            {
                Collection = value;
            }
            
            public void NotNullable([ThingParameter(IsNullable = false)]List<T> value)
            {
                Collection = value;
            }
            
            public void MinAndMax([ThingParameter(MinimumItems = 2, MaximumItems = 3)]List<T> value)
            {
                Collection = value;
            }
            
            public void Unique([ThingParameter(UniqueItems = true)]List<T> value)
            {
                Collection = value;
            }
        }

        public class IListThing : Thing, IThingCollection
        {
            public override string Name => "ilist-thing";
            
            [ThingProperty(Ignore = true)]
            public IEnumerable<T> Collection { get; set; }


            public void Values(IList<T> value)
            {
                Collection = value;
            }
            
            public void NotNullable([ThingParameter(IsNullable = false)]IList<T> value)
            {
                Collection = value;
            }
            
            public void MinAndMax([ThingParameter(MinimumItems = 2, MaximumItems = 3)]IList<T> value)
            {
                Collection = value;
            }
            
            public void Unique([ThingParameter(UniqueItems = true)]IList<T> value)
            {
                Collection = value;
            }
        }
        
        public class ICollectionThing : Thing, IThingCollection
        {
            public override string Name => "icollection-thing";
            
            [ThingProperty(Ignore = true)]
            public IEnumerable<T> Collection { get; set; }
            
            public void Values(ICollection<T> value)
            {
                Collection = value;
            }
            
            public void NotNullable([ThingParameter(IsNullable = false)]ICollection<T> value)
            {
                Collection = value;
            }
            
            public void MinAndMax([ThingParameter(MinimumItems = 2, MaximumItems = 3)]ICollection<T> value)
            {
                Collection = value;
            }
            
            public void Unique([ThingParameter(UniqueItems = true)]ICollection<T> value)
            {
                Collection = value;
            }
        }
        
        public class ISetThing : Thing, IThingCollection
        {
            public override string Name => "iset-thing";
            
            [ThingProperty(Ignore = true)]
            public IEnumerable<T> Collection { get; set; }
            
            public void Values(ISet<T> value)
            {
                Collection = value;
            }
            
            public void NotNullable([ThingParameter(IsNullable = false)]ISet<T> value)
            {
                Collection = value;
            }
            
            public void MinAndMax([ThingParameter(MinimumItems = 2, MaximumItems = 3)]ISet<T> value)
            {
                Collection = value;
            }
            
            public void Unique([ThingParameter(UniqueItems = true)]ISet<T> value)
            {
                Collection = value;
            }
        }
        
        public class HashSetThing : Thing, IThingCollection
        {
            public override string Name => "hashset-thing";
            
            [ThingProperty(Ignore = true)]
            public IEnumerable<T> Collection { get; set; }

            public void Values(HashSet<T> value)
            {
                Collection = value;
            }
            
            public void NotNullable([ThingParameter(IsNullable = false)]HashSet<T> value)
            {
                Collection = value;
            }
            
            public void MinAndMax([ThingParameter(MinimumItems = 2, MaximumItems = 3)]HashSet<T> value)
            {
                Collection = value;
            }
            
            public void Unique([ThingParameter(UniqueItems = true)]HashSet<T> value)
            {
                Collection = value;
            }
        }
        
        public class LinkedListThing : Thing, IThingCollection
        {
            public override string Name => "linked-list-thing";
            
            [ThingProperty(Ignore = true)]
            public IEnumerable<T> Collection { get; set; }

            public void Values(LinkedList<T> value)
            {
                Collection = value;
            }
            
            public void NotNullable([ThingParameter(IsNullable = false)]LinkedList<T> value)
            {
                Collection = value;
            }
            
            public void MinAndMax([ThingParameter(MinimumItems = 2, MaximumItems = 3)]LinkedList<T> value)
            {
                Collection = value;
            }
            
            public void Unique([ThingParameter(UniqueItems = true)]LinkedList<T> value)
            {
                Collection = value;
            }
        }
        
        #endregion

        private const string Response = @"
{{
  ""@context"": ""https://iot.mozilla.org/schemas"",
  ""security"": ""nosec_sc"",
    ""securityDefinitions"": {{
        ""nosec_sc"": {{
            ""scheme"": ""nosec""
        }}
  }},
  ""actions"": {{
    ""values"": {{
      ""links"": [
        {{
          ""href"": ""/things/{0}/actions/values"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""array"",
            ""items"": {{
              ""type"": ""{1}""
                {2}
            }}
          }}
        }}
      }}
    }},
    ""notNullable"": {{
      ""links"": [
        {{
          ""href"": ""/things/{0}/actions/notNullable"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""array"",
            ""items"": {{
              ""type"": ""{1}""
                {2}
            }}
          }}
        }}
      }}
    }},
    ""minAndMax"": {{
      ""links"": [
        {{
          ""href"": ""/things/{0}/actions/minAndMax"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""array"",
            ""minItems"": 2,
            ""maxItems"": 3,
            ""items"": {{
              ""type"": ""{1}""
                {2}
            }}
          }}
        }}
      }}
    }},
    ""unique"": {{
      ""links"": [
        {{
          ""href"": ""/things/{0}/actions/unique"",
          ""rel"": ""action""
        }}
      ],
      ""input"": {{
        ""type"": ""object"",
        ""properties"": {{
          ""value"": {{
            ""type"": ""array"",
            ""uniqueItems"": true,
            ""items"": {{
              ""type"": ""{1}""
                {2}
            }}
          }}
        }}
      }}
    }}
  }},
  ""links"": [
    {{
        ""rel"": ""properties"",
        ""href"": ""/things/{0}/properties""
    }},
    {{
        ""rel"": ""actions"",
        ""href"": ""/things/{0}/actions""
    }},
    {{
        ""rel"": ""events"",
        ""href"": ""/things/{0}/events""
    }}
  ]
}}
";
    }
}
