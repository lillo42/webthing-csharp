using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Things
{
    public class PropertyEnumThing : Thing
    {
        public override string Name => "property-enum-type";
        
        private bool _bool;
        
        [ThingProperty(Enum =  new object[]{ true, false})]
        public bool Bool
        {
            get => _bool;
            set
            {
                _bool = value;
                OnPropertyChanged();
            }
        }
        
        private byte _numberByte;
        
        
        [ThingProperty(Enum =  new object[]{ 0, byte.MaxValue, byte.MinValue })]
        public byte NumberByte
        {
            get => _numberByte;
            set
            {
                _numberByte = value;
                OnPropertyChanged();
            }
        }
        
        private sbyte _numberSByte;

        [ThingProperty(Enum =  new object[]{ 0, sbyte.MaxValue, sbyte.MinValue })]
        public sbyte NumberSByte
        {
            get => _numberSByte;
            set
            {
                _numberSByte = value;
                OnPropertyChanged();
            }
        }
        
        private short _numberShort;
        
        [ThingProperty(Enum =  new object[]{ 0, short.MaxValue, short.MinValue })]
        public short NumberShort
        {
            get => _numberShort;
            set
            {
                _numberShort = value;
                OnPropertyChanged();
            }
        }
        
        private ushort _numberUShort;
        
        [ThingProperty(Enum =  new object[]{ 0, ushort.MaxValue, ushort.MinValue })]
        public ushort NumberUShort
        {
            get => _numberUShort;
            set
            {
                _numberUShort = value;
                OnPropertyChanged();
            }
        }
        
        private int _numberInt;
        
        [ThingProperty(Enum =  new object[]{ 0, int.MaxValue, int.MinValue })]
        public int NumberInt
        {
            get => _numberInt;
            set
            {
                _numberInt = value;
                OnPropertyChanged();
            }
        }
        
        private uint _numberUInt;

        [ThingProperty(Enum =  new object[]{ 0, uint.MaxValue, uint.MinValue })]
        public uint NumberUInt
        {
            get => _numberUInt;
            set
            {
                _numberUInt = value;
                OnPropertyChanged();
            }
        }
        
        private long _numberLong;

        [ThingProperty(Enum =  new object[]{ 0, long.MaxValue, long.MinValue })]
        public long NumberLong
        {
            get => _numberLong;
            set
            {
                _numberLong = value;
                OnPropertyChanged();
            }
        }
        
        private ulong _numberULong;
        
        [ThingProperty(Enum =  new object[]{ 0, ulong.MaxValue, ulong.MinValue })]
        public ulong NumberULong
        {
            get => _numberULong;
            set
            {
                _numberULong = value;
                OnPropertyChanged();
            }
        }

        private double _numberDouble;
        
        [ThingProperty(Enum =  new object[]{ 0, double.MaxValue, double.MinValue })]
        public double NumberDouble
        {
            get => _numberDouble;
            set
            {
                _numberDouble = value;
                OnPropertyChanged();
            }
        }
        
        private float _numberFloat;
        
        [ThingProperty(Enum =  new object[]{ 0, float.MaxValue, float.MinValue })]
        public float NumberFloat
        {
            get => _numberFloat;
            set
            {
                _numberFloat = value;
                OnPropertyChanged();
            }
        }
        
        private decimal _numberDecimal;
        
        [ThingProperty(Enum =  new object[]{ 0, 1d, 100 })]
        public decimal NumberDecimal
        {
            get => _numberDecimal;
            set
            {
                _numberDecimal = value;
                OnPropertyChanged();
            }
        }
        
        private string _text;
        
        [ThingProperty(Enum =  new object[]{ "ola", "ass", "aaa" })]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
    }
}
