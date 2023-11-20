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

using System.Globalization;
using Google.Protobuf.Reflection;

namespace Cel.Helpers;

public static class Int64Helpers
{
    #region Compare

    public static int CompareInt(long value, object? otherValue)
    {
        if (otherValue == null)
        {
            return -1;
        }

        if (otherValue is double)
        {
            return CompareIntDouble(value, (double)otherValue);
        }

        if (otherValue is long)
        {
            return CompareIntInt(value, (long)otherValue);
        }

        if (otherValue is int)
        {
            return CompareIntInt(value, (int)otherValue);
        }

        if (otherValue is short)
        {
            return CompareIntInt(value, (short)otherValue);
        }

        if (otherValue is ulong)
        {
            return CompareIntUInt(value, (ulong)otherValue);
        }

        if (otherValue is Enum)
        {
            return CompareIntInt(value, (int)otherValue);
        }

        throw new CelNoSuchOverloadException($"Could not compare int64 values '{value}' with type '{value.GetType()}' and '{otherValue}' with type '{otherValue.GetType()}'.");
    }

    public static int CompareIntInt(long a, long b)
    {
        if (a < b)
        {
            return -1;
        }

        if (a > b)
        {
            return 1;
        }

        return 0;
    }

    public static int CompareIntDouble(long i, double d)
    {
        return -DoubleHelpers.CompareDoubleInt(d, i);
    }

    public static int CompareIntUInt(long i, ulong u)
    {
        if (i < 0 || u > long.MaxValue)
        {
            return -1;
        }

        var cmp = i - (long)u;

        if (cmp < 0)
        {
            return -1;
        }

        if (cmp > 0)
        {
            return 1;
        }

        return 0;
    }

    #endregion

    #region Convert

    public static long ConvertInt(object? value)
    {
        if (value == null)
        {
            throw new CelNoSuchOverloadException($"No overload exists to convert type '{value?.GetType().FullName ?? "null"}' to double.");
        }

        if (value is byte byteValue)
        {
            return byteValue;
        }

        if (value is short shortValue)
        {
            return shortValue;
        }

        if (value is int intValue)
        {
            return intValue;
        }

        if (value is long longValue)
        {
            return longValue;
        }

        if (value is ulong uLongValue)
        {
            return ConvertIntUInt(uLongValue);
        }

        if (value is double doubleValue)
        {
            return ConvertIntDouble(doubleValue);
        }

        if (value is string strValue)
        {
            return ConvertIntString(strValue);
        }

        if (value is Enum)
        {
            return (int)value;
        }

        if (value is EnumValueDescriptor enumValueDescriptorValue)
        {
            return enumValueDescriptorValue.Number;
        }

        if (value is DateTimeOffset dateTimeOffsetValue)
        {
            return ConvertIntTimestamp(dateTimeOffsetValue);
        }

        if (value is DateTime dateTimeValue)
        {
            return ConvertIntTimestamp(dateTimeValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to convert type '{value.GetType().FullName ?? "null"}' to int64.");
    }

    public static long ConvertIntTimestamp(DateTimeOffset dateTimeOffsetValue)
    {
        return dateTimeOffsetValue.ToUnixTimeSeconds();
    }

    public static long ConvertIntUInt(ulong uintValue)
    {
        if (uintValue > long.MaxValue)
        {
            throw new CelArgumentRangeException($"Cannot convert uint64 value '{uintValue}' to int64 because the value exceeds the maximum int64 value.");
        }

        return (long)uintValue;
    }

    public static long ConvertIntDouble(double value)
    {
        try
        {
            if (value <= long.MinValue)
            {
                //from the spec
                //Double to int conversions are limited to (minInt, maxInt) non-inclusive.
                throw new CelArgumentRangeException($"Could not convert '{value}' to Int.");
            }

            if (value >= long.MaxValue)
            {
                //from the spec
                //Double to int conversions are limited to (minInt, maxInt) non-inclusive.
                throw new CelArgumentRangeException($"Could not convert '{value}' to Int.");
            }

            value = Math.Truncate(value);

            return Convert.ToInt64(value);
        }
        catch (CelArgumentRangeException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new CelArgumentRangeException($"Could not convert '{value}' to Int.");
        }
    }

    public static long ConvertIntString(string token)
    {
        if (token.StartsWith("0x"))
        {
            if (token.Length <= 2)
            {
                throw new CelStringParsingException($"Could not parse uint value '{token}'.");
            }

            //hex value
            token = token.Substring(2);

            if (long.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var intValue))
            {
                return intValue;
            }
        }

        else if (token.StartsWith("-0x"))
        {
            if (token.Length <= 2)
            {
                throw new CelStringParsingException($"Could not parse uint value '{token}'.");
            }

            //hex value
            token = token.Substring(3);

            if (long.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var intValue))
            {
                return -intValue;
            }
        }
        else
        {
            //regular integer
            if (long.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue))
            {
                return intValue;
            }
        }

        throw new CelStringParsingException($"Could not convert '{token}' to Int.");
    }

    #endregion

    #region Add

    public static long AddInt(long value, object? otherValue)
    {
        if (otherValue is Enum)
        {
            return AddIntInt(value, (int)otherValue);
        }

        if (otherValue is EnumValueDescriptor otherValueEnumValueDescriptor)
        {
            return AddIntInt(value, otherValueEnumValueDescriptor.Number);
        }

        if (otherValue is int otherValueInt32)
        {
            return AddIntInt(value, otherValueInt32);
        }

        if (otherValue is long otherValueLong)
        {
            return AddIntInt(value, otherValueLong);
        }

        throw new CelNoSuchOverloadException($"No overload exists to ADD int64 and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static long AddIntInt(long a, long b)
    {
        return checked(a + b);
    }

    #endregion

    #region Subtract

    public static long SubtractInt(long value, object? otherValue)
    {
        if (otherValue is int otherValueInt32)
        {
            return SubtractIntInt(value, otherValueInt32);
        }

        if (otherValue is long)
        {
            return SubtractIntInt(value, (long)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to SUBTRACT int64 and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static long SubtractIntInt(long a, long b)
    {
        return checked(a - b);
    }

    #endregion

    #region Multiply

    public static long MultiplyInt(long value, object? otherValue)
    {
        if (otherValue is long)
        {
            return MultiplyIntInt(value, (long)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to MULTIPLY int64 and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static long MultiplyIntInt(long a, long b)
    {
        return checked(a * b);
    }

    #endregion

    #region Divide

    public static long DivideInt(long value, object? otherValue)
    {
        if (otherValue is long)
        {
            return DivideIntInt(value, (long)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to DIVIDE int64 and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static long DivideIntInt(long a, long b)
    {
        if (b == 0)
        {
            throw new CelDivideByZeroException("Cannot divide value by zero.");
        }

        return a / b;
    }

    #endregion

    #region Modulus

    public static long ModulusInt(long value, object? otherValue)
    {
        if (otherValue is long)
        {
            return ModulusIntInt(value, (long)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to MODULUS int64 and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static long ModulusIntInt(long a, long b)
    {
        if (b == 0)
        {
            throw new CelModulusByZeroException("Cannot calculate modulus of value by zero.");
        }

        return a % b;
    }

    #endregion

    #region Negate

    public static object? NegateInt(long longValue)
    {
        if (longValue == long.MinValue)
        {
            //negative min value cannot become positive.
            throw new CelOverflowException($"Could not negate int64 value '{longValue}'.");
        }

        return checked(-longValue);
    }

    #endregion
}