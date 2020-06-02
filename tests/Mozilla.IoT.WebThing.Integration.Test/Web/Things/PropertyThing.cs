using System;
using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.Integration.Test.Web.Things
{
    public class PropertyThing : Thing
    {
        public override string Name => "property-thing";

        public Guid Id2 { get; } = Guid.Parse("77bd476e-469d-4954-83b5-d9eedb2543ff");

        private bool _modifyViaMethod = true;

        [ThingProperty(IsReadOnly = true)]
        public bool ModifyViaMethod
        {
            get => _modifyViaMethod;
            set
            {
                _modifyViaMethod = value;
                OnPropertyChanged();
            }
        }
        
        private bool _write = true;
        [ThingProperty(IsWriteOnly = true)]
        public bool Write
        {
            get => _write;
            set
            {
                _write = value;
                OnPropertyChanged();
            }
        }
        
        private bool _write2 = true;
        public bool Write2
        {
            private get => _write2;
            set
            {
                _write2 = value;
                OnPropertyChanged();
            }
        }

        private string _someText = "";

        [ThingProperty(IsNullable = false, Name = "text")]
        public string SomeText
        {
            get => _someText;
            set
            {
                _someText = value;
                OnPropertyChanged();
            }
        }

        private int _level;

        [ThingProperty(Minimum = 0, Maximum = 100)]
        public int Level
        {
            get => _level;
            set
            {
                _level = value;
                OnPropertyChanged();
            }
        }
        
        [ThingProperty(Ignore = true)]
        public decimal OtherValue { get; set; }

        private string[] _extraInformation = new string[0];
        
        [ThingProperty(Enum = new object[]{ "ABC", "DEF", "GHI"})]
        public string[] ExtraInformation
        {
            get => _extraInformation;
            set
            {
                _extraInformation = value;
                OnPropertyChanged();
            }
        }
    }

    public class WebSocketPropertyThing : PropertyThing
    {
        public override string Name => "web-socket-property-thing";
    }
}
