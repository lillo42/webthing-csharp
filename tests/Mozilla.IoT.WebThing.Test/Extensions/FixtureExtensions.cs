using System;
using AutoFixture;

namespace Mozilla.IoT.WebThing.Test.Extensions
{
    public static class FixtureExtensions
    {
        public static object GetValue(this Fixture fixture, Type type)
        {
            if (type == typeof(bool))
            {
                return fixture.Create<bool>();
            }

            if (type == typeof(byte))
            {
                return fixture.Create<byte>();
            }
            
            if (type == typeof(sbyte))
            {
                return fixture.Create<sbyte>();
            }
            
            if (type == typeof(short))
            {
                return fixture.Create<short>();
            }
            
            if (type == typeof(ushort))
            {
                return fixture.Create<ushort>();
            }
            
            if (type == typeof(int))
            {
                return fixture.Create<int>();
            }
            
            if (type == typeof(uint))
            {
                return fixture.Create<uint>();
            }
            
            if (type == typeof(long))
            {
                return fixture.Create<long>();
            }
            
            if (type == typeof(ulong))
            {
                return fixture.Create<ulong>();
            }
            
            if (type == typeof(float))
            {
                return fixture.Create<float>();
            }
            
            if (type == typeof(double))
            {
                return fixture.Create<double>();
            }
            
            if (type == typeof(decimal))
            {
                return fixture.Create<decimal>();
            }

            if (type == typeof(DateTime))
            {
                return fixture.Create<DateTime>();
            }
            
            if (type == typeof(DateTimeOffset))
            {
                return fixture.Create<DateTimeOffset>();
            }

            return fixture.Create<string>();
        }
    }
}
