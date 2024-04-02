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

namespace Cel.Helpers;

public static class UInt64Helpers
{
    #region Compare

    public static int CompareUInt(ulong value, object? otherValue)
    {
        if (otherValue == null)
        {
            return -1;
        }

        if (otherValue is double)
        {
            return CompareUIntDouble(value, (double)otherValue);
        }

        if (otherValue is decimal)
        {
            return CompareUIntDecimal(value, (decimal)otherValue);
        }

        if (otherValue is long)
        {
            return CompareUIntInt(value, (long)otherValue);
        }

        if (otherValue is uint)
        {
            return CompareUIntUInt(value, (uint)otherValue);
        }

        if (otherValue is ulong)
        {
            return CompareUIntUInt(value, (ulong)otherValue);
        }

        throw new CelNoSuchOverloadException($"Could not compare uint64 to type '{otherValue.GetType().FullName}'.");
    }

    public static int CompareUIntInt(ulong u, long i)
    {
        return -Int64Helpers.CompareIntUInt(i, u);
    }

    public static int CompareUIntDouble(ulong u, double d)
    {
        return -DoubleHelpers.CompareDoubleUInt(d, u);
    }
    public static int CompareUIntDecimal(ulong u, decimal d)
    {
        return -DecimalHelpers.CompareDecimalUInt(d, u);
    }

    public static int CompareUIntUInt(ulong a, ulong b)
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

    #endregion

    #region Convert

    public static ulong ConvertUInt(object? value)
    {
        if (value is ulong ulongValue)
        {
            return ulongValue;
        }

        if (value is int intValue)
        {
            return ConvertUIntInt(intValue);
        }

        if (value is long longValue)
        {
            return ConvertUIntInt(longValue);
        }

        if (value is double doubleValue)
        {
            return ConvertUIntDouble(doubleValue);
        }

        if (value is string strValue)
        {
            return ConvertUIntString(strValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to convert type '{value?.GetType().FullName ?? "null"}' to uint64.");
    }

    private static ulong ConvertUIntInt(long intValue)
    {
        if (intValue < 0)
        {
            throw new CelArgumentRangeException($"Could not convert int64 value '{intValue}' to uint64.");
        }

        return (ulong)intValue;
    }

    private static ulong ConvertUIntDouble(double value)
    {
        try
        {
            if (value < ulong.MinValue)
            {
                throw new CelArgumentRangeException($"Could not convert double '{value}' to uint.");
            }

            if (value > ulong.MaxValue)
            {
                throw new CelArgumentRangeException($"Could not convert double '{value}' to uint.");
            }

            value = Math.Truncate(value);

            return Convert.ToUInt64(value);
        }
        catch (CelArgumentRangeException)
        {
            throw;
        }
        catch (Exception)
        {
            throw new CelArgumentRangeException($"Could not convert double '{value}' to uint.");
        }
    }

    private static ulong ConvertUIntString(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new CelStringParsingException("Could not parse blank uint value.");
        }

        if (token.EndsWith("u") || token.EndsWith("U"))
        {
            token = token.Substring(0, token.Length - 1);
        }

        if (token.StartsWith("0x"))
        {
            if (token.Length <= 2)
            {
                throw new CelStringParsingException($"Could not parse uint value '{token}'.");
            }

            //hex value
            token = token.Substring(2);

            if (ulong.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var uintValue))
            {
                return uintValue;
            }
        }
        else
        {
            //regular integer
            if (ulong.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out var uintValue))
            {
                return uintValue;
            }
        }

        throw new CelStringParsingException($"Could not parse uint value '{token}'.");
    }

    #endregion

    #region Add

    public static ulong AddUInt(ulong value, object? otherValue)
    {
        if (otherValue is ulong)
        {
            return AddUIntUInt(value, (ulong)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to ADD uint64 and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static ulong AddUIntUInt(ulong a, ulong b)
    {
        return checked(a + b);
    }

    #endregion

    #region Subtract

    public static ulong SubtractUInt(ulong value, object? otherValue)
    {
        if (otherValue is ulong)
        {
            return SubtractUIntUInt(value, (ulong)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to SUBTRACT uint64 and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static ulong SubtractUIntUInt(ulong a, ulong b)
    {
        return checked(a - b);
    }

    #endregion

    #region Multiply

    public static ulong MultiplyUInt(ulong value, object? otherValue)
    {
        if (otherValue is ulong)
        {
            return MultiplyUIntUInt(value, (ulong)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to MULTIPLY uint64 and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static ulong MultiplyUIntUInt(ulong a, ulong b)
    {
        return checked(a * b);
    }

    #endregion

    #region Divide

    public static ulong DivideUInt(ulong value, object? otherValue)
    {
        if (otherValue is ulong)
        {
            return DivideUIntUInt(value, (ulong)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to DIVIDE uint64 and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static ulong DivideUIntUInt(ulong a, ulong b)
    {
        if (b == 0)
        {
            throw new CelDivideByZeroException("Cannot divide value by zero.");
        }

        return a / b;
    }

    #endregion

    #region Modulus

    public static ulong ModulusUInt(ulong value, object? otherValue)
    {
        if (otherValue is ulong)
        {
            return ModulusUIntUInt(value, (ulong)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to MODULUS uint64 and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static ulong ModulusUIntUInt(ulong a, ulong b)
    {
        if (b == 0)
        {
            throw new CelModulusByZeroException("Cannot calculate modulus of value by zero.");
        }

        return a % b;
    }

    #endregion

    #region Negate

    public static object? NegateUInt(ulong ulongValue)
    {
        if (ulongValue == 0)
        {
            return 0;
        }

        throw new CelNoSuchOverloadException("No overload exists to NEGATE uint64.");
    }

    #endregion
}