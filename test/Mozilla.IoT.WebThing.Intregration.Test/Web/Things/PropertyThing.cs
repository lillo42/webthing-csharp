using System;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Intregration.Test.Web.Things
{
    public class ImmutablePropertyThing : Thing
    {
        public const string NAME = "immutable-property-thing";
        public override string Name => NAME;

        public bool IsEnable { get; set; }
        public int Level { get; set; } = 10;
        public string Value { get; set; } = "test";
    }
    
    public class PropertyThing : Thing
    {
        public override string Name => "property-thing";
        
        [ThingProperty(Ignore = true)]
        public string NoShow { get; set; }
        
        [ThingProperty(IsReadOnly = true)]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [ThingProperty(Name = "isEnable")]
        public bool Enable { get; set; }
        
        [ThingProperty(Minimum = 0, Maximum = 100)]
        public int Level { get; set; }

        [ThingProperty(MinimumLength = 1, MaximumLength = 100, 
            Pattern =  @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$")]
        public string Email { get; set; }
    }
}
