// Copyright 2023 TELUS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;
using Cel.Helpers;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Enum = System.Enum;

namespace Cel.Internal;

public static class ArithmeticFunctions
{
    public static object? Add(object? value, object? otherValue)
    {
        if (value == null || otherValue == null)
        {
            return 0;
        }

        try
        {
            if (value is double doubleValue)
            {
                return DoubleHelpers.AddDouble(doubleValue, otherValue);
            }

            if (value is decimal decimalValue)
            {
                return DecimalHelpers.AddDecimal(decimalValue, otherValue);
            }

            if (value is long longValue)
            {
                return Int64Helpers.AddInt(longValue, otherValue);
            }

            if (value is ulong ulongValue)
            {
                return UInt64Helpers.AddUInt(ulongValue, otherValue);
            }
            if (value is int intValue)
            {
                return Int64Helpers.AddInt(intValue, otherValue);
            }

            if (value is uint uintValue)
            {
                return UInt64Helpers.AddUInt(uintValue, otherValue);
            }

            if (value is short shortValue)
            {
                return Int64Helpers.AddInt(shortValue, otherValue);
            }

            if (value is ushort ushortValue)
            {
                return UInt64Helpers.AddUInt(ushortValue, otherValue);
            }

            if (value is byte byteValue)
            {
                return UInt64Helpers.AddUInt(byteValue, otherValue);
            }

            if (value is sbyte sbyteValue)
            {
                return Int64Helpers.AddInt(sbyteValue, otherValue);
            }

            if (value is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.AddTimestamp(dateTimeOffsetValue, otherValue);
            }

            if (value is DateTime dateTimeValue)
            {
                return TimestampHelpers.AddTimestamp(dateTimeValue, otherValue);
            }

            if (value is TimeSpan timeSpanValue)
            {
                return DurationHelpers.AddDuration(timeSpanValue, otherValue);
            }

            if (value is Duration durationValue)
            {
                return DurationHelpers.AddDuration(durationValue, otherValue);
            }

            if (value is ByteString byteArrayValue)
            {
                return ByteArrayHelpers.AddBytes(byteArrayValue, otherValue);
            }

            if (value is object[] objectArrayValue)
            {
                return ListHelpers.AddList(objectArrayValue, otherValue);
            }

            if (value is IList objectArrayList)
            {
                return ListHelpers.AddList(objectArrayList, otherValue);
            }

            if (value is string stringValue)
            {
                return StringHelpers.AddString(stringValue, otherValue);
            }

            if (value is Enum)
            {
                return Int64Helpers.AddInt((int)value, otherValue);
            }

            if (value is EnumValueDescriptor enumValueDescriptorValue)
            {
                return Int64Helpers.AddInt(enumValueDescriptorValue.Number, otherValue);
            }
        }
        catch (OverflowException x)
        {
            throw new CelOverflowException(x.Message);
        }
        catch (ArgumentOutOfRangeException x)
        {
            throw new CelArgumentRangeException(x.Message);
        }

        throw new CelNoSuchOverloadException($"No overload exists to ADD types '{value?.GetType().FullName ?? "null"}' and '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static object? Subtract(object? value, object? otherValue)
    {
        if (value == null && otherValue == null)
        {
            return 0;
        }

        try
        {
            if (value is double doubleValue)
            {
                return DoubleHelpers.SubtractDouble(doubleValue, otherValue);
            }

            if (value is decimal decimalValue)
            {
                return DecimalHelpers.SubtractDecimal(decimalValue, otherValue);
            }

            if (value is long longValue)
            {
                return Int64Helpers.SubtractInt(longValue, otherValue);
            }

            if (value is ulong ulongValue)
            {
                return UInt64Helpers.SubtractUInt(ulongValue, otherValue);
            }

            if (value is int intValue)
            {
                return Int64Helpers.SubtractInt(intValue, otherValue);
            }

            if (value is uint uintValue)
            {
                return UInt64Helpers.SubtractUInt(uintValue, otherValue);
            }
            if (value is short shortValue)
            {
                return Int64Helpers.SubtractInt(shortValue, otherValue);
            }

            if (value is ushort ushortValue)
            {
                return UInt64Helpers.SubtractUInt(ushortValue, otherValue);
            }

            if (value is byte byteValue)
            {
                return UInt64Helpers.SubtractUInt(byteValue, otherValue);
            }

            if (value is sbyte sbyteValue)
            {
                return Int64Helpers.SubtractInt(sbyteValue, otherValue);
            }

            if (value is TimeSpan timeSpanValue)
            {
                return DurationHelpers.SubtractDuration(timeSpanValue, otherValue);
            }

            if (value is Duration durationValue)
            {
                return DurationHelpers.SubtractDuration(durationValue, otherValue);
            }

            if (value is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.SubtractTimestamp(dateTimeOffsetValue, otherValue);
            }

            if (value is DateTime dateTimeValue)
            {
                return TimestampHelpers.SubtractTimestamp(dateTimeValue, otherValue);
            }
        }
        catch (OverflowException x)
        {
            throw new CelOverflowException(x.Message);
        }
        catch (ArgumentOutOfRangeException x)
        {
            throw new CelArgumentRangeException(x.Message);
        }

        throw new CelNoSuchOverloadException($"No overload exists to SUBTRACT types '{value?.GetType().FullName ?? "null"}' and '{otherValue?.GetType().FullName ?? "null"}'.");
    }


    public static object? Multiply(object? value, object? otherValue)
    {
        if (value == null && otherValue == null)
        {
            return 0;
        }

        try
        {
            if (value is double doubleValue)
            {
                return DoubleHelpers.MultiplyDouble(doubleValue, otherValue);
            }

            if (value is decimal decimalValue)
            {
                return DecimalHelpers.MultiplyDecimal(decimalValue, otherValue);
            }

            if (value is long longValue)
            {
                return Int64Helpers.MultiplyInt(longValue, otherValue);
            }

            if (value is ulong ulongValue)
            {
                return UInt64Helpers.MultiplyUInt(ulongValue, otherValue);
            }

            if (value is int intValue)
            {
                return Int64Helpers.MultiplyInt(intValue, otherValue);
            }

            if (value is uint uintValue)
            {
                return UInt64Helpers.MultiplyUInt(uintValue, otherValue);
            }

            if (value is short shortValue)
            {
                return Int64Helpers.MultiplyInt(shortValue, otherValue);
            }

            if (value is ushort ushortValue)
            {
                return UInt64Helpers.MultiplyUInt(ushortValue, otherValue);
            }

            if (value is byte byteValue)
            {
                return UInt64Helpers.MultiplyUInt(byteValue, otherValue);
            }

            if (value is sbyte sbyteValue)
            {
                return Int64Helpers.MultiplyInt(sbyteValue, otherValue);
            }
        }
        catch (OverflowException x)
        {
            throw new CelOverflowException(x.Message);
        }
        catch (ArgumentOutOfRangeException x)
        {
            throw new CelArgumentRangeException(x.Message);
        }

        throw new CelNoSuchOverloadException($"No overload exists to MULTIPLY types '{value?.GetType().FullName ?? "null"}' and '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static object? Divide(object? value, object? otherValue)
    {
        if (value == null && otherValue == null)
        {
            return 0;
        }

        try
        {
            if (value is double doubleValue)
            {
                return DoubleHelpers.DivideDouble(doubleValue, otherValue);
            }

            if (value is decimal decimalValue)
            {
                return DecimalHelpers.DivideDecimal(decimalValue, otherValue);
            }

            if (value is long longValue)
            {
                return Int64Helpers.DivideInt(longValue, otherValue);
            }

            if (value is ulong ulongValue)
            {
                return UInt64Helpers.DivideUInt(ulongValue, otherValue);
            }

            if (value is int intValue)
            {
                return Int64Helpers.DivideInt(intValue, otherValue);
            }

            if (value is uint uintValue)
            {
                return UInt64Helpers.DivideUInt(uintValue, otherValue);
            }

            if (value is short shortValue)
            {
                return Int64Helpers.DivideInt(shortValue, otherValue);
            }

            if (value is ushort ushortValue)
            {
                return UInt64Helpers.DivideUInt(ushortValue, otherValue);
            }

            if (value is byte byteValue)
            {
                return UInt64Helpers.DivideUInt(byteValue, otherValue);
            }

            if (value is sbyte sbyteValue)
            {
                return Int64Helpers.DivideInt(sbyteValue, otherValue);
            }
        }
        catch (OverflowException x)
        {
            throw new CelOverflowException(x.Message);
        }
        catch (ArgumentOutOfRangeException x)
        {
            throw new CelArgumentRangeException(x.Message);
        }

        throw new CelNoSuchOverloadException($"No overload exists to DIVIDE types '{value?.GetType().FullName ?? "null"}' and '{otherValue?.GetType().FullName ?? "null"}'.");
    }


    public static object? Modulus(object? value, object? otherValue)
    {
        if (value == null && otherValue == null)
        {
            return 0;
        }

        try
        {
            if (value is double doubleValue)
            {
                return DoubleHelpers.ModulusDouble(doubleValue, otherValue);
            }

            if (value is decimal decimalValue)
            {
                return DecimalHelpers.ModulusDecimal(decimalValue, otherValue);
            }

            if (value is int intValue)
            {
                return Int64Helpers.ModulusInt(intValue, otherValue);
            }

            if (value is long longValue)
            {
                return Int64Helpers.ModulusInt(longValue, otherValue);
            }

            if (value is ulong ulongValue)
            {
                return UInt64Helpers.ModulusUInt(ulongValue, otherValue);
            }

            if (value is uint uintValue)
            {
                return UInt64Helpers.ModulusUInt(uintValue, otherValue);
            }

            if (value is short shortValue)
            {
                return Int64Helpers.ModulusInt(shortValue, otherValue);
            }

            if (value is ushort ushortValue)
            {
                return UInt64Helpers.ModulusUInt(ushortValue, otherValue);
            }

            if (value is byte byteValue)
            {
                return UInt64Helpers.ModulusUInt(byteValue, otherValue);
            }

            if (value is sbyte sbyteValue)
            {
                return Int64Helpers.ModulusInt(sbyteValue, otherValue);
            }
        }
        catch (OverflowException x)
        {
            throw new CelOverflowException(x.Message);
        }
        catch (ArgumentOutOfRangeException x)
        {
            throw new CelArgumentRangeException(x.Message);
        }

        throw new CelNoSuchOverloadException($"No overload exists to MODULUS types '{value?.GetType().FullName ?? "null"}' and '{otherValue?.GetType().FullName ?? "null"}'.");
    }


    public static object? LogicalNot(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is bool boolValue)
        {
            return BooleanHelpers.LogicalNotBool(boolValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to LOGICAL NOT type '{value?.GetType().FullName ?? "null"}'.");
    }

    public static object? Negate(object? value)
    {
        if (value == null)
        {
            return null;
        }

        if (value is double doubleValue)
        {
            return DoubleHelpers.NegateDouble(doubleValue);
        }

        if (value is decimal decimalValue)
        {
            return DecimalHelpers.NegateDecimal(decimalValue);
        }

        if (value is long longValue)
        {
            return Int64Helpers.NegateInt(longValue);
        }

        if (value is ulong ulongValue)
        {
            return UInt64Helpers.NegateUInt(ulongValue);
        }

        if (value is int intValue)
        {
            return Int64Helpers.NegateInt(intValue);
        }

        if (value is uint uintValue)
        {
            return UInt64Helpers.NegateUInt(uintValue);
        }

        if (value is short shortValue)
        {
            return Int64Helpers.NegateInt(shortValue);
        }

        if (value is ushort ushortValue)
        {
            return UInt64Helpers.NegateUInt(ushortValue);
        }

        if (value is byte byteValue)
        {
            return UInt64Helpers.NegateUInt(byteValue);
        }

        if (value is sbyte sbyteValue)
        {
            return Int64Helpers.NegateInt(sbyteValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to NEGATE type '{value?.GetType().FullName ?? "null"}'.");
    }
}