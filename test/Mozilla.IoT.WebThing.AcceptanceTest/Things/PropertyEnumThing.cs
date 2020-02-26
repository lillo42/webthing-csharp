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
        
        private bool? _nullableBool;
        [ThingProperty(Enum =  new object[]{ null, true, false})]
        public bool? NullableBool
        {
            get => _nullableBool;
            set
            {
                _nullableBool = value;
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
        
        private byte? _nullableByte;
        [ThingProperty(Enum =  new object[]{ null, byte.MaxValue, byte.MinValue })]
        public byte? NullableByte
        {
            get => _nullableByte;
            set
            {
                _nullableByte = value;
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
        
        private sbyte? _nullableSByte;
        [ThingProperty(Enum =  new object[]{ null, sbyte.MaxValue, sbyte.MinValue })]
        public sbyte? NullableSByte
        {
            get => _nullableSByte;
            set
            {
                _nullableSByte = value;
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
        
        private short? _nullableShort;
        [ThingProperty(Enum =  new object[]{ null, short.MaxValue, short.MinValue })]
        public short? NullableShort
        {
            get => _nullableShort;
            set
            {
                _nullableShort = value;
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
        
        private ushort? _nullableUShort;
        [ThingProperty(Enum =  new object[]{ null, ushort.MaxValue, ushort.MinValue })]
        public ushort? NullableUShort
        {
            get => _nullableUShort;
            set
            {
                _nullableUShort = value;
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
        
        private int? _nullableInt;
        [ThingProperty(Enum =  new object[]{ null, int.MaxValue, int.MinValue })]
        public int? NullableInt
        {
            get => _nullableInt;
            set
            {
                _nullableInt = value;
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
        
        private uint? _nullableUInt;
        [ThingProperty(Enum =  new object[]{ null, uint.MaxValue, uint.MinValue })]
        public uint? NullableUInt
        {
            get => _nullableUInt;
            set
            {
                _nullableUInt = value;
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
        
        private long? _nullableLong;
        [ThingProperty(Enum =  new object[]{ null, long.MaxValue, long.MinValue })]
        public long? NullableLong
        {
            get => _nullableLong;
            set
            {
                _nullableLong = value;
                OnPropertyChanged();
            }
        }
        
        private ulong _numberULong;
        [ThingProperty(Enum =  new object[]{ 0, ulong.MaxValue, ulong.MinValue})]
        public ulong NumberULong
        {
            get => _numberULong;
            set
            {
                _numberULong = value;
                OnPropertyChanged();
            }
        }
        
        private ulong? _nullableULong;
        [ThingProperty(Enum =  new object[]{ null, ulong.MaxValue, ulong.MinValue })]
        public ulong? NullableULong
        {
            get => _nullableULong;
            set
            {
                _nullableULong = value;
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
        
        private double? _nullableDouble;
        [ThingProperty(Enum =  new object[]{ null, double.MaxValue, double.MinValue })]
        public double? NullableDouble
        {
            get => _nullableDouble;
            set
            {
                _nullableDouble = value;
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
        
        private float? _nullableFloat;
        [ThingProperty(Enum =  new object[]{ null, float.MaxValue, float.MinValue })]
        public float? NullableFloat
        {
            get => _nullableFloat;
            set
            {
                _nullableFloat = value;
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
        
        private decimal? _nullableDecimal;
        [ThingProperty(Enum =  new object[]{ null, 1d, 100 })]
        public decimal? NullableDecimal
        {
            get => _nullableDecimal;
            set
            {
                _nullableDecimal = value;
                OnPropertyChanged();
            }
        }
        
        
        private string _text;
        
        [ThingProperty(Enum =  new object[]{ null, "ola", "ass", "aaa" })]
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
