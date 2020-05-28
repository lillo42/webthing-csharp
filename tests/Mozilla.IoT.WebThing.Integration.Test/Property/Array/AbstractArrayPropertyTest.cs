using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mozilla.IoT.WebThing.Attributes;
using Mozilla.IoT.WebThing.Extensions;
using Mozilla.IoT.WebThing.Factories;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Mozilla.IoT.WebThing.Integration.Test.Property.Array
{
    public abstract class AbstractArrayPropertyTest<T>
    {
        protected IThingContextFactory Factory { get; }
        protected Fixture Fixture { get; }

        protected AbstractArrayPropertyTest()
        {
            var collection = new ServiceCollection();
            collection.AddThings();
            var provider = collection.BuildServiceProvider();
            Factory = provider.GetRequiredService<IThingContextFactory>();
            Fixture = new Fixture();
        }

        protected abstract JsonElement CreateJson(IEnumerable<T> values);
        protected abstract JsonElement[] CreateInvalidJson();

        protected virtual List<T> CreateValue(int arrayLength)
        {
            var values = new List<T>(arrayLength);

            for (var i = 0; i < arrayLength; i++)
            {
                values.Add(Fixture.Create<T>());
            }

            return values;
        }

        #region Serialization
        private void Serialize<TThing>()
            where TThing : Thing, new()
        {
            var thing = new TThing();
            var context = Factory.Create(thing, new ThingOption());

            var message = JsonSerializer.Serialize(context.Response,
                new ThingOption().ToJsonSerializerOptions());

            FluentAssertions.Json.JsonAssertionExtensions.Should(JToken.Parse(message))
                .BeEquivalentTo(JToken.Parse(RESPONSE.Replace("{0}", thing.Name)));
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
        private void ValidProperty<TThing>(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue,
            Func<List<T>, List<T>> transValue = null)
            where  TThing : Thing, new()
        {
            var thing = new TThing();
            var context = Factory.Create(thing, new ThingOption());
        
            thing.ThingContext = context;
        
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().ContainKey(propertyName);

            var values = CreateValue(arrayLength);

            if (uniqueItems)
            {
                values = values.ToHashSet().ToList();
            }
            
            var jsonElement = CreateJson(values);
        
            context.Properties[propertyName].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
            context.Properties[propertyName].TryGetValue(out var getValue).Should().BeTrue();

            if (transValue != null)
            {
                values = transValue(values);
            }
            
            getValue.Should().BeEquivalentTo(values);
        
            if(acceptedNullValue)
            { 
                jsonElement =  JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }").GetProperty("input");
                context.Properties[propertyName].TrySetValue(jsonElement).Should().Be(SetPropertyResult.Ok);
                context.Properties[propertyName].TryGetValue(out getValue).Should().BeTrue();
                getValue.Should().BeNull();
            }
        }
        
        [Theory]
        [InlineData(nameof(ArrayThing.Values), 4, false, true)]
        [InlineData(nameof(ArrayThing.NotNullable), 4, false, false)]
        [InlineData(nameof(ArrayThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(ArrayThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(ArrayThing.Unique), 2, true, true)]
        public void ValidArrayProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            ValidProperty<ArrayThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }
        
        [Theory]
        [InlineData(nameof(IEnumerableThing.Values), 4, false, true)]
        [InlineData(nameof(IEnumerableThing.NotNullable), 4, false, false)]
        [InlineData(nameof(IEnumerableThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(IEnumerableThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(IEnumerableThing.Unique), 2, true, true)]
        public void ValidIEnumerableProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            ValidProperty<IEnumerableThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }
        
        [Theory]
        [InlineData(nameof(ListThing.Values), 4, false, true)]
        [InlineData(nameof(ListThing.NotNullable), 4, false, false)]
        [InlineData(nameof(ListThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(ListThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(ListThing.Unique), 2, true, true)]
        public void ValidListProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            ValidProperty<ListThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }
        
        [Theory]
        [InlineData(nameof(IListThing.Values), 4, false, true)]
        [InlineData(nameof(IListThing.NotNullable), 4, false, false)]
        [InlineData(nameof(IListThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(IListThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(IListThing.Unique), 2, true, true)]
        public void ValidIListProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue) 
        {
            ValidProperty<IListThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }

        [Theory]
        [InlineData(nameof(ICollectionThing.Values), 4, false, true)]
        [InlineData(nameof(ICollectionThing.NotNullable), 4, false, false)]
        [InlineData(nameof(ICollectionThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(ICollectionThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(ICollectionThing.Unique), 2, true, true)]
        public void ValidICollectionProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue) 
        {
            ValidProperty<ICollectionThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }
        
        [Theory]
        [InlineData(nameof(ISetThing.Values), 4, false, true)]
        [InlineData(nameof(ISetThing.NotNullable), 4, false, false)]
        [InlineData(nameof(ISetThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(ISetThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(ISetThing.Unique), 2, true, true)]
        public void ValidISetProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            ValidProperty<ISetThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue, x => x.ToHashSet().ToList());
        }
        
        [Theory]
        [InlineData(nameof(HashSetThing.Values), 4, false, true)]
        [InlineData(nameof(HashSetThing.NotNullable), 4, false, false)]
        [InlineData(nameof(HashSetThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(HashSetThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(HashSetThing.Unique), 2, true, true)]
        public void ValidHashSetProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            ValidProperty<HashSetThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue, x => x.ToHashSet().ToList());
        }
        
        [Theory]
        [InlineData(nameof(LinkedListThing.Values), 4, false, true)]
        [InlineData(nameof(LinkedListThing.NotNullable), 4, false, false)]
        [InlineData(nameof(LinkedListThing.MinAndMax), 2, false, true)]
        [InlineData(nameof(LinkedListThing.MinAndMax), 3, false, true)]
        [InlineData(nameof(LinkedListThing.Unique), 2, true, true)]
        public void ValidLinkedListProperty(string propertyName, int arrayLength, bool uniqueItems, bool acceptedNullValue)
        {
            ValidProperty<LinkedListThing>(propertyName, arrayLength, uniqueItems, acceptedNullValue);
        }
        
        #endregion

        #region Invalid

        private void InvalidProperty<TThing>(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
            where  TThing : Thing, new()
        {
            var thing = new TThing();
            var context = Factory.Create(thing, new ThingOption());
        
            thing.ThingContext = context;
        
            context.Actions.Should().BeEmpty();
            context.Events.Should().BeEmpty();

            context.Properties.Should().NotBeEmpty();
            context.Properties.Should().ContainKey(propertyName);

            if(arrayLength.HasValue)
            {
                var values = CreateValue(arrayLength.Value);

                if (duplicateValue)
                {
                    values.AddRange(values);
                }

                var jsonElement = CreateJson(values);
            
                context.Properties[propertyName].TrySetValue(jsonElement).Should().Be(SetPropertyResult.InvalidValue);
                context.Properties[propertyName].TryGetValue(out var getValue).Should().BeTrue();
                getValue.Should().NotBeEquivalentTo(values);
            }

            var jsonElements = CreateInvalidJson();
            
            foreach (var element in jsonElements)
            {
                context.Properties[propertyName].TrySetValue(element).Should().Be(SetPropertyResult.InvalidValue);
            }
        
            if(executeNull)
            { 
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(@"{ ""input"": null }").GetProperty("input");
                context.Properties[propertyName].TrySetValue(jsonElement).Should().Be(SetPropertyResult.InvalidValue);
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
            InvalidProperty<ArrayThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        [Theory]
        [InlineData(nameof(IEnumerableThing.Values), null, false, false)]
        [InlineData(nameof(IEnumerableThing.NotNullable), null, false, true)]
        [InlineData(nameof(IEnumerableThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(IEnumerableThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(IEnumerableThing.Unique), 3, true, false)]
        public void InvalidIEnumerableThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidProperty<IEnumerableThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        [Theory]
        [InlineData(nameof(ListThing.Values), null, false, false)]
        [InlineData(nameof(ListThing.NotNullable), null, false, true)]
        [InlineData(nameof(ListThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(ListThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(ListThing.Unique), 3, true, false)]
        public void InvalidListThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidProperty<ListThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        [Theory]
        [InlineData(nameof(IListThing.Values), null, false, false)]
        [InlineData(nameof(IListThing.NotNullable), null, false, true)]
        [InlineData(nameof(IListThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(IListThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(IListThing.Unique), 3, true, false)]
        public void InvalidIListThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidProperty<IListThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        [Theory]
        [InlineData(nameof(ICollectionThing.Values), null, false, false)]
        [InlineData(nameof(ICollectionThing.NotNullable), null, false, true)]
        [InlineData(nameof(ICollectionThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(ICollectionThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(ICollectionThing.Unique), 3, true, false)]
        public void InvalidICollectionThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidProperty<ICollectionThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        [Theory]
        [InlineData(nameof(LinkedListThing.Values), null, false, false)]
        [InlineData(nameof(LinkedListThing.NotNullable), null, false, true)]
        [InlineData(nameof(LinkedListThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(LinkedListThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(LinkedListThing.Unique), 3, true, false)]
        public void InvalidLinkedListThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidProperty<LinkedListThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        
        [Theory]
        [InlineData(nameof(ISetThing.Values), null, false, false)]
        [InlineData(nameof(ISetThing.NotNullable), null, false, true)]
        [InlineData(nameof(ISetThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(ISetThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(ISetThing.Unique), 3, true, false)]
        public void InvalidISetThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidProperty<ISetThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }
        
        
        [Theory]
        [InlineData(nameof(HashSetThing.Values), null, false, false)]
        [InlineData(nameof(HashSetThing.NotNullable), null, false, true)]
        [InlineData(nameof(HashSetThing.MinAndMax), 1, false, false)]
        [InlineData(nameof(HashSetThing.MinAndMax), 4, false, false)]
        [InlineData(nameof(HashSetThing.Unique), 3, true, false)]
        public void InvalidHashSetThingProperty(string propertyName, int? arrayLength, bool duplicateValue, bool executeNull)
        {
            InvalidProperty<HashSetThing>(propertyName, arrayLength, duplicateValue, executeNull);
        }

        #endregion
        
        #region Things

        public class ArrayThing : Thing
        {
            public override string Name => "array-thing";
            
            public T[] Values { get; set; }
            
            [ThingProperty(IsNullable = false)]
            public T[] NotNullable { get; set; }
            
            [ThingProperty(MinimumItems = 2, MaximumItems = 3)]
            public T[] MinAndMax { get; set; }
            
            [ThingProperty(UniqueItems = true)]
            public T[] Unique { get; set; }
        }
        
        public class IEnumerableThing : Thing
        {
            public override string Name => "enumerable-thing";
            
            public IEnumerable<T> Values { get; set; }
            
            [ThingProperty(IsNullable = false)]
            public IEnumerable<T> NotNullable { get; set; }
            
            [ThingProperty(MinimumItems = 2, MaximumItems = 3)]
            public IEnumerable<T> MinAndMax { get; set; }
            
            [ThingProperty(UniqueItems = true)]
            public IEnumerable<T> Unique { get; set; }
        }

        public class ListThing : Thing
        {
            public override string Name => "list-thing";
            
            public List<T> Values { get; set; }
            
            [ThingProperty(IsNullable = false)]
            public List<T> NotNullable { get; set; }
            
            [ThingProperty(MinimumItems = 2, MaximumItems = 3)]
            public List<T> MinAndMax { get; set; }
            
            [ThingProperty(UniqueItems = true)]
            public List<T> Unique { get; set; }
        }
        
        public class IListThing : Thing
        {
            public override string Name => "list-thing";
            
            public IList<T> Values { get; set; }
            
            [ThingProperty(IsNullable = false)]
            public IList<T> NotNullable { get; set; }
            
            [ThingProperty(MinimumItems = 2, MaximumItems = 3)]
            public IList<T> MinAndMax { get; set; }
            
            [ThingProperty(UniqueItems = true)]
            public IList<T> Unique { get; set; }
        }
        
        public class ICollectionThing : Thing
        {
            public override string Name => "icollection-thing";
            
            public ICollection<T> Values { get; set; }
            
            [ThingProperty(IsNullable = false)]
            public ICollection<T> NotNullable { get; set; }
            
            [ThingProperty(MinimumItems = 2, MaximumItems = 3)]
            public ICollection<T> MinAndMax { get; set; }
            
            [ThingProperty(UniqueItems = true)]
            public ICollection<T> Unique { get; set; }
        }
        
        public class ISetThing : Thing
        {
            public override string Name => "iset-thing";
            
            public ISet<T> Values { get; set; }
            
            [ThingProperty(IsNullable = false)]
            public ISet<T> NotNullable { get; set; }
            
            [ThingProperty(MinimumItems = 2, MaximumItems = 3)]
            public ISet<T> MinAndMax { get; set; }
            
            [ThingProperty(UniqueItems = true)]
            public ISet<T> Unique { get; set; }
        }
        
        public class HashSetThing : Thing
        {
            public override string Name => "hash-set-thing";
            
            public HashSet<T> Values { get; set; }
            
            [ThingProperty(IsNullable = false)]
            public HashSet<T> NotNullable { get; set; }
            
            [ThingProperty(MinimumItems = 2, MaximumItems = 3)]
            public HashSet<T> MinAndMax { get; set; }
            
            [ThingProperty(UniqueItems = true)]
            public HashSet<T> Unique { get; set; }
        }
        
        public class LinkedListThing : Thing
        {
            public override string Name => "linked-list-thing";
            
            public LinkedList<T> Values { get; set; }
            
            [ThingProperty(IsNullable = false)]
            public LinkedList<T> NotNullable { get; set; }
            
            [ThingProperty(MinimumItems = 2, MaximumItems = 3)]
            public LinkedList<T> MinAndMax { get; set; }
            
            [ThingProperty(UniqueItems = true)]
            public LinkedList<T> Unique { get; set; }
        }
        
        #endregion

        private readonly string RESPONSE = $@"
{{
    ""@context"": ""https://iot.mozilla.org/schemas"",
    ""security"": ""nosec_sc"",
    ""securityDefinitions"": {{
        ""nosec_sc"": {{
            ""scheme"": ""nosec""
        }}
    }},
    ""properties"": {{
        ""values"": {{
            ""type"": ""array"",
            ""items"": {{
                ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}""
            }},
            ""links"": [
                {{
                    ""href"": ""/things/{{0}}/properties/values"",
                    ""rel"": ""property""
                }}
            ]
        }},
        ""notNullable"": {{
            ""type"": ""array"",
            ""items"": {{
                ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}""
            }},
            ""links"": [
                {{
                    ""href"": ""/things/{{0}}/properties/notNullable"",
                    ""rel"": ""property""
                }}
            ]
        }},
        ""minAndMax"": {{
            ""type"": ""array"",
            ""minItems"": 2,
            ""maxItems"": 3,
            ""items"": {{
                ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}""
            }},
            ""links"": [
                {{
                    ""href"": ""/things/{{0}}/properties/minAndMax"",
                    ""rel"": ""property""
                }}
            ]
        }},
        ""unique"": {{
            ""type"": ""array"",
            ""uniqueItems"": true,
            ""items"": {{
                ""type"": ""{typeof(T).ToJsonType().ToString().ToLower()}""
            }},
            ""links"": [
                {{
                    ""href"": ""/things/{{0}}/properties/unique"",
                    ""rel"": ""property""
                }}
            ]
        }}
    }},
    ""links"": [
        {{
            ""rel"": ""properties"",
            ""href"": ""/things/{{0}}/properties""
        }},
        {{
            ""rel"": ""actions"",
            ""href"": ""/things/{{0}}/actions""
        }},
        {{
            ""rel"": ""events"",
            ""href"": ""/things/{{0}}/events""
        }}
    ]
}}
";
    }
}
