using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace Mozilla.IoT.WebThing.Factories.Generator
{
    public class ValidationGeneration
    {
        private static readonly MethodInfo s_getLength = typeof(string).GetProperty(nameof(string.Length)).GetMethod;

        public static void AddValidation(IlFactory factory, Validation validation, LocalBuilder field, int returnValue, FieldBuilder? regex)
        {
            if (IsNumber(field.LocalType))
            {
                AddNumberValidation(factory, validation, field, returnValue);
            }
            else if (IsString(field.LocalType))
            {
                AddStringValidation(factory, validation, field, returnValue, regex);
            }
        }

        private static void AddNumberValidation(IlFactory factory, Validation validation, LocalBuilder field, int returnValue)
        {
            if (validation.Minimum.HasValue)
            {
                factory.IfIsLessThan(field, validation.Minimum.Value);
                factory.Return(returnValue);
                factory.EndIf();
            }

            if (validation.Maximum.HasValue)
            {
                factory.IfIsGreaterThan(field, validation.Maximum.Value);
                factory.Return(returnValue);
                factory.EndIf();
            }

            if (validation.ExclusiveMinimum.HasValue)
            {
                factory.IfIsLessOrEqualThan(field, validation.ExclusiveMinimum.Value);
                factory.Return(returnValue);
                factory.EndIf();
            }

            if (validation.ExclusiveMaximum.HasValue)
            {
                factory.IfIsGreaterOrEqualThan(field, validation.ExclusiveMaximum.Value);
                factory.Return(returnValue);
                factory.EndIf();
            }

            if (validation.MultipleOf.HasValue)
            {
                factory.IfIsNotMultipleOf(field, validation.MultipleOf.Value);
                factory.Return(returnValue);
                factory.EndIf();
            }

            if(validation.Enums != null && validation.Enums.Length > 0)
            {
                factory.IfIsDifferent(field, validation.Enums);
                factory.Return(returnValue);
                factory.EndIf();
            }
        }

        private static void AddStringValidation(IlFactory factory, Validation validation, LocalBuilder field, int returnValue, FieldBuilder? regex)
        {
            if (validation.MinimumLength.HasValue)
            {
                factory.IfIsLessThan(field, s_getLength, validation.MinimumLength.Value);
                factory.Return(returnValue);
                factory.EndIf();
            }

            if (validation.MaximumLength.HasValue)
            {
                factory.IfIsGreaterThan(field, s_getLength, validation.MaximumLength.Value);
                factory.Return(returnValue);
                factory.EndIf();
            }

            if (validation.Enums != null && validation.Enums.Length > 0)
            {
                factory.IfIsDifferent(field, validation.Enums);
                factory.Return(returnValue);
                factory.EndIf();
            }

            if (validation.Pattern != null)
            {
                factory.IfNotMatchWithRegex(field, regex, validation.Pattern);
                factory.Return(returnValue);
                factory.EndIf();
            }
        }

        private static bool IsString(Type type)
            => type == typeof(string);

        private static bool IsNumber(Type type)
            => type == typeof(int)
               || type == typeof(uint)
               || type == typeof(long)
               || type == typeof(ulong)
               || type == typeof(short)
               || type == typeof(ushort)
               || type == typeof(double)
               || type == typeof(float)
               || type == typeof(decimal)
               || type == typeof(byte)
               || type == typeof(sbyte);
    }

    public readonly struct Validation
    {
        public Validation(double? minimum, double? maximum,
            double? exclusiveMinimum, double? exclusiveMaximum, double? multipleOf,
            int? minimumLength, int? maximumLength, string? pattern, object[]? enums)
        {
            Minimum = minimum;
            Maximum = maximum;
            ExclusiveMinimum = exclusiveMinimum;
            ExclusiveMaximum = exclusiveMaximum;
            MultipleOf = multipleOf;
            MinimumLength = minimumLength;
            MaximumLength = maximumLength;
            Pattern = pattern;
            Enums = enums;
        }

        public double? Minimum { get; }
        public double? Maximum { get; }
        public double? ExclusiveMinimum { get; }
        public double? ExclusiveMaximum { get; }
        public double? MultipleOf { get; }
        public int? MinimumLength { get; }
        public int? MaximumLength { get; }
        public string? Pattern { get; }
        public object[]? Enums { get; }

        public bool HasValidation
            => Minimum.HasValue
               || Maximum.HasValue
               || ExclusiveMinimum.HasValue
               || ExclusiveMaximum.HasValue
               || MultipleOf.HasValue
               || MinimumLength.HasValue
               || MaximumLength.HasValue
               || Pattern != null
               || (Enums != null && Enums.Length > 0);

        public bool HasNullValueOnEnum
            => Enums != null && Enums.Contains(null);
    }
}
