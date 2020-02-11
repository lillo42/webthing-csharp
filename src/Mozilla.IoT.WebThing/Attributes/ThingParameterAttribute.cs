using System;

namespace Mozilla.IoT.WebThing.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = true)]
    public class ThingParameterAttribute : Attribute
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Unit { get; set; }
        internal int? MinimumValue { get; private set; }

        public int Minimum
        {
            get => MinimumValue ?? 0;
            set => MinimumValue = value;
        }
        
        internal int? MaximumValue { get; private set; }

        public int Maximum
        {
            get => MaximumValue ?? 0;
            set => MaximumValue = value;
        }


        internal int? MultipleOfValue { get; set; }

        public int MultipleOf
        {
            get => MultipleOfValue ?? 0;
            set => MultipleOfValue = value;
        }
    }
}
