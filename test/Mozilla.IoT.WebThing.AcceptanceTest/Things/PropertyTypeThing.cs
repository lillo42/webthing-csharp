using System;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Things
{
    public class PropertyTypeThing : Thing
    {
        public override string Name => "property-type";
        
        private bool _bool;
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
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }
        
        private DateTime _data;
        public DateTime Data
        {
            get => _data;
            set
            {
                _data = value;
                OnPropertyChanged();
            }
        }
        
        private DateTimeOffset _dataOffset;
        public DateTimeOffset DataOffset
        {
            get => _dataOffset;
            set
            {
                _dataOffset = value;
                OnPropertyChanged();
            }
        }
    }
}
