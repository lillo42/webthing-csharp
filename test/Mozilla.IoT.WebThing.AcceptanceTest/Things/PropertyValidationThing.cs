using Mozilla.IoT.WebThing.Attributes;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Things
{
    public class PropertyValidationThing : Thing
    {
        public override string Name => "property-validation-type";

        private byte _numberByte;
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(Minimum = 1, Maximum = 100)]
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
        [ThingProperty(MinimumLength = 1, MaximumLength = 100)]
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
        
        private string _email;
        [ThingProperty(Pattern = @"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$")]
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }
    }
}
