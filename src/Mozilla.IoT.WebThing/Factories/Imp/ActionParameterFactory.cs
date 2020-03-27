using System;
using System.Linq;
using Mozilla.IoT.WebThing.Actions;
using Mozilla.IoT.WebThing.Actions.Parameters.Boolean;
using Mozilla.IoT.WebThing.Actions.Parameters.Number;
using Mozilla.IoT.WebThing.Actions.Parameters.String;
using Mozilla.IoT.WebThing.Builders;

namespace Mozilla.IoT.WebThing.Factories
{
    /// <inheritdoc/>
    public class ActionParameterFactory : IActionParameterFactory
    {
        /// <inheritdoc/>
        public IActionParameter Create(Type parameterType, Information information)
        {
            if (parameterType == typeof(bool))
            {
                return new ParameterBoolean(information.IsNullable);
            }

            if (parameterType == typeof(string))
            {
                return new ParameterString(information.IsNullable,
                    information.MinimumLength, information.MaximumLength, information.Pattern,
                    information.Enums?.Where(x => x != null).Select(Convert.ToString).ToArray()!);
            }

            if (parameterType == typeof(char))
            {
                return new ParameterChar(information.IsNullable,
                    information.Enums?.Where(x => x != null).Select(Convert.ToChar).ToArray());
            }

            if (parameterType.IsEnum)
            {
                return new ParameterEnum(information.IsNullable, parameterType);
            }

            if (parameterType == typeof(Guid))
            {
                return new ParameterGuid(information.IsNullable,
                    information.Enums?.Where(x => x != null).Select(x => Guid.Parse(x.ToString()!)).ToArray());
            }

            if (parameterType == typeof(TimeSpan))
            {
                return new ParameterTimeSpan(information.IsNullable,
                    information.Enums?.Where(x => x != null).Select(x => TimeSpan.Parse(x.ToString()!)).ToArray());
            }

            if (parameterType == typeof(DateTime))
            {
                return new ParameterDateTime(information.IsNullable,
                    information.Enums?.Where(x => x != null).Select(Convert.ToDateTime).ToArray());
            }

            if (parameterType == typeof(DateTimeOffset))
            {
                return new ParameterDateTimeOffset(information.IsNullable,
                    information.Enums?.Where(x => x != null).Select(x => DateTimeOffset.Parse(x.ToString()!)).ToArray());
            }
            else
            {
                var minimum = information.Minimum;
                var maximum = information.Maximum;
                var multipleOf = information.MultipleOf;
                var enums = information.Enums;

                if (information.ExclusiveMinimum.HasValue)
                {
                    minimum = information.ExclusiveMinimum!.Value + 1;
                }

                if (information.ExclusiveMaximum.HasValue)
                {
                    maximum = information.ExclusiveMaximum!.Value - 1;
                }

                if (parameterType == typeof(byte))
                {
                    var min = minimum.HasValue ? new byte?(Convert.ToByte(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new byte?(Convert.ToByte(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf!.Value)) : null;

                    return new ParameterByte(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToByte).ToArray());
                }
                
                if (parameterType == typeof(sbyte))
                {
                    var min = minimum.HasValue ? new sbyte?(Convert.ToSByte(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new sbyte?(Convert.ToSByte(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new sbyte?(Convert.ToSByte(multipleOf!.Value)) : null;

                    return new ParameterSByte(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToSByte).ToArray());
                }
                
                if (parameterType == typeof(short))
                {
                    var min = minimum.HasValue ? new short?(Convert.ToInt16(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new short?(Convert.ToInt16(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new short?(Convert.ToInt16(multipleOf!.Value)) : null;

                    return new ParameterShort(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToInt16).ToArray());
                }
                
                if (parameterType == typeof(ushort))
                {
                    var min = minimum.HasValue ? new ushort?(Convert.ToUInt16(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new ushort?(Convert.ToUInt16(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf!.Value)) : null;

                    return new ParameterUShort(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToUInt16).ToArray());
                }
                
                if (parameterType == typeof(int))
                {
                    var min = minimum.HasValue ? new int?(Convert.ToInt32(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new int?(Convert.ToInt32(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new int?(Convert.ToInt32(multipleOf!.Value)) : null;

                    return new ParameterInt(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToInt32).ToArray());
                }
                
                if (parameterType == typeof(uint))
                {
                    var min = minimum.HasValue ? new uint?(Convert.ToUInt32(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new uint?(Convert.ToUInt32(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new uint?(Convert.ToUInt32(multipleOf!.Value)) : null;

                    return new ParameterUInt(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToUInt32).ToArray());
                } 
                
                if (parameterType == typeof(long))
                {
                    var min = minimum.HasValue ? new long?(Convert.ToInt64(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new long?(Convert.ToInt64(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new long?(Convert.ToInt64(multipleOf!.Value)) : null;

                    return new ParameterLong(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToInt64).ToArray());
                }
                
                if (parameterType == typeof(ulong))
                {
                    var min = minimum.HasValue ? new ulong?(Convert.ToUInt64(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new ulong?(Convert.ToUInt64(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new byte?(Convert.ToByte(multipleOf!.Value)) : null;

                    return new ParameterULong(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToUInt64).ToArray());
                }
                
                if (parameterType == typeof(float))
                {
                    var min = minimum.HasValue ? new float?(Convert.ToSingle(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new float?(Convert.ToSingle(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new float?(Convert.ToSingle(multipleOf!.Value)) : null;

                    return new ParameterFloat(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToSingle).ToArray());
                }
                
                if (parameterType == typeof(double))
                {
                    var min = minimum.HasValue ? new double?(Convert.ToDouble(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new double?(Convert.ToDouble(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new double?(Convert.ToDouble(multipleOf!.Value)) : null;

                    return new ParameterDouble(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToDouble).ToArray());
                }
                else
                {
                    var min = minimum.HasValue ? new decimal?(Convert.ToDecimal(minimum!.Value)) : null;
                    var max = maximum.HasValue ? new decimal?(Convert.ToDecimal(maximum!.Value)) : null;
                    var multi = multipleOf.HasValue ? new decimal?(Convert.ToDecimal(multipleOf!.Value)) : null;

                    return new ParameterDecimal(information.IsNullable,
                        min, max, multi, enums?.Where(x => x != null).Select(Convert.ToDecimal).ToArray());
                }
            }
        }
    }
}
