namespace Mozilla.IoT.WebThing.Intregration.Test.Factories
{
    public class PropertyTest : IThingContextFactoryTest
    {
        public class PropertyThing<T> : Thing
            where T : struct
        {
            public override string Name => "property-thing";
            
            public T Value { get; set; }
            public T? NullableValue { get; set; }
        }
    }
}
