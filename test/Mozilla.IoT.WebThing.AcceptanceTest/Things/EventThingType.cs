using System;

namespace Mozilla.IoT.WebThing.AcceptanceTest.Things
{
    public class EventTypeThing : Thing
    {
        public override string Name => "event-type";
        
        public event EventHandler<bool> Bool;
        public event EventHandler<byte> Byte;
        public event EventHandler<sbyte> SByte;
        public event EventHandler<short> Short;
        public event EventHandler<ushort> UShort;
        public event EventHandler<int> Int;
        public event EventHandler<uint> UInt;
        public event EventHandler<long> Long;
        public event EventHandler<ulong> ULong;
        public event EventHandler<float> Float;
        public event EventHandler<double> Double;
        public event EventHandler<decimal> Decimal;
        public event EventHandler<string> String;
        public event EventHandler<DateTime> DateTime;
        public event EventHandler<DateTimeOffset> DateTimeOffset;
        
        public event EventHandler<bool?> NullableBool;
        public event EventHandler<byte?> NullableByte;
        public event EventHandler<sbyte?> NullableSByte;
        public event EventHandler<short?> NullableShort;
        public event EventHandler<ushort?> NullableUShort;
        public event EventHandler<int?> NullableInt;
        public event EventHandler<uint?> NullableUInt;
        public event EventHandler<long?> NullableLong;
        public event EventHandler<ulong?> NullableULong;
        public event EventHandler<float?> NullableFloat;
        public event EventHandler<double?> NullableDouble;
        public event EventHandler<decimal?> NullableDecimal;
        public event EventHandler<string?> NullableString;
        public event EventHandler<DateTime?> NullableDateTime;
        public event EventHandler<DateTimeOffset?> NullableDateTimeOffset;
        
        
        public void Run(
            bool @bool,
            byte @byte,
            sbyte @sbyte,
            short @short,
            ushort @ushort,
            int @int,
            uint @uint,
            long @long,
            ulong @ulong,
            double @double,
            float @float,
            decimal @decimal,
            string @string,
            DateTime @dateTime,
            DateTimeOffset @dateTimeOffset
        )
        {
            Bool?.Invoke(this, @bool);
            Byte?.Invoke(this, @byte);
            SByte?.Invoke(this, @sbyte);
            Short?.Invoke(this, @short);
            UShort?.Invoke(this, @ushort);
            Int?.Invoke(this, @int);
            UInt?.Invoke(this, @uint);
            Long?.Invoke(this, @long);
            ULong?.Invoke(this, @ulong);
            Float?.Invoke(this, @float);
            Double?.Invoke(this, @double);
            Decimal?.Invoke(this, @decimal);
            String?.Invoke(this, @string);
            DateTime?.Invoke(this, dateTime);
            DateTimeOffset?.Invoke(this, dateTimeOffset);
        }
        
        public void RunNull(
            bool? @bool,
            byte? @byte,
            sbyte? @sbyte,
            short? @short,
            ushort? @ushort,
            int? @int,
            uint? @uint,
            long? @long,
            ulong? @ulong,
            double? @double,
            float? @float,
            decimal? @decimal,
            string? @string,
            DateTime? @dateTime,
            DateTimeOffset? @dateTimeOffset
        )
        {   
            NullableBool?.Invoke(this, @bool);
            NullableByte?.Invoke(this, @byte);
            NullableSByte?.Invoke(this, @sbyte);
            NullableShort?.Invoke(this, @short);
            NullableUShort?.Invoke(this, @ushort);
            NullableInt?.Invoke(this, @int);
            NullableUInt?.Invoke(this, @uint);
            NullableLong?.Invoke(this, @long);
            NullableULong?.Invoke(this, @ulong);
            NullableFloat?.Invoke(this, @float);
            NullableDouble?.Invoke(this, @double);
            NullableDecimal?.Invoke(this, @decimal);
            NullableString?.Invoke(this, @string);
            NullableDateTime?.Invoke(this, dateTime);
            NullableDateTimeOffset?.Invoke(this, dateTimeOffset);
        }
    }
}
