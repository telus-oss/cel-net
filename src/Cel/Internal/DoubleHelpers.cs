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

public static class DoubleHelpers
{
    #region Compare

    public static int CompareDouble(double value, object? otherValue)
    {
        if (otherValue is double)
        {
            return CompareDoubleDouble(value, (double)otherValue);
        }

        if (otherValue is float)
        {
            return CompareDoubleFloat(value, (float)otherValue);
        }

        if (otherValue is decimal)
        {
            return CompareDoubleDouble(value, (decimal)otherValue);
        }

        if (otherValue is long)
        {
            return CompareDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return CompareDoubleUInt(value, (ulong)otherValue);
        }

        if (otherValue is int)
        {
            return CompareDoubleInt(value, (long)(int)otherValue);
        }

        if (otherValue is uint)
        {
            return CompareDoubleUInt(value, (ulong)(uint)otherValue);
        }

        if (otherValue is short)
        {
            return CompareDoubleInt(value, (long)(short)otherValue);
        }

        if (otherValue is ushort)
        {
            return CompareDoubleUInt(value, (ulong)(ushort)otherValue);
        }

        if (otherValue is byte)
        {
            return CompareDoubleUInt(value, (ulong)(byte)otherValue);
        }

        if (otherValue is sbyte)
        {
            return CompareDoubleInt(value, (long)(sbyte)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to compare double to type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static int CompareDoubleDouble(double a, double b)
    {
        if (double.IsNaN(a) || double.IsNaN(b))
        {
            return -1;
        }

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

    public static int CompareDoubleFloat(double a, float b)
    {
        if (double.IsNaN(a) || float.IsNaN(b))
        {
            return -1;
        }

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

    public static int CompareDoubleDouble(double a, decimal b)
    {
        const double decimalMin = (double)decimal.MinValue;
        const double decimalMax = (double)decimal.MaxValue;
        if (a < decimalMin)
        {
            return -1;
        }

        if (a > decimalMax)
        {
            return 1;
        }
        return ((decimal)a).CompareTo(b);
    }

    public static int CompareDoubleInt(double d, long i)
    {
        if (d < long.MinValue)
        {
            return -1;
        }

        if (d > long.MaxValue)
        {
            return 1;
        }

        return CompareDoubleDouble(d, (double)i);
    }

    public static int CompareDoubleUInt(double d, ulong u)
    {
        if (d < 0)
        {
            return -1;
        }

        if (d > ulong.MaxValue)
        {
            return 1;
        }

        return CompareDoubleDouble(d, (double)u);
    }

    #endregion

    #region Convert

    public static double ConvertDouble(object? value)
    {
        if (value == null)
        {
            return -1;
        }

        if (value is double doubleValue)
        {
            return doubleValue;
        }

        if (value is float floatValue)
        {
            return ConvertDoubleFloat(floatValue);
        }

        if (value is long intValue)
        {
            return ConvertDoubleInt(intValue);
        }

        if (value is ulong uintValue)
        {
            return ConvertDoubleUInt(uintValue);
        }

        if (value is int int32Value)
        {
            return ConvertDoubleInt(int32Value);
        }

        if (value is uint uint32Value)
        {
            return ConvertDoubleUInt(uint32Value);
        }

        if (value is short int16Value)
        {
            return ConvertDoubleInt(int16Value);
        }

        if (value is ushort uint16Value)
        {
            return ConvertDoubleUInt(uint16Value);
        }

        if (value is byte byteValue)
        {
            return ConvertDoubleUInt(byteValue);
        }

        if (value is sbyte sbyteValue)
        {
            return ConvertDoubleInt(sbyteValue);
        }

        if (value is decimal decimalValue)
        {
            return ConvertDoubleDecimal(decimalValue);
        }

        if (value is string strValue)
        {
            return ConvertDoubleString(strValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to convert type '{value.GetType().FullName ?? "null"}' to double.");
    }

    public static double ConvertDoubleFloat(float value)
    {
        return value;
    }

    public static double ConvertDoubleDecimal(decimal value)
    {
        return (double)value;
    }

    public static double ConvertDoubleInt(long value)
    {
        return value;
    }

    public static double ConvertDoubleUInt(ulong value)
    {
        return value;
    }

    public static double ConvertDoubleString(string value)
    {
        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleValue))
        {
            return doubleValue;
        }

        throw new CelStringParsingException($"Could not parse double value '{value}'");
    }

    #endregion

    #region Add

    public static double AddDouble(double value, object? otherValue)
    {
        if (otherValue is double)
        {
            return AddDoubleDouble(value, (double)otherValue);
        }

        if (otherValue is float)
        {
            return AddDoubleFloat(value, (float)otherValue);
        }

        if (otherValue is decimal)
        {
            return AddDoubleDecimal(value, (decimal)otherValue);
        }

        if (otherValue is long)
        {
            return AddDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return AddDoubleUInt(value, (ulong)otherValue);
        }

        if (otherValue is int)
        {
            return AddDoubleInt(value, (long)(int)otherValue);
        }

        if (otherValue is uint)
        {
            return AddDoubleUInt(value, (ulong)(uint)otherValue);
        }

        if (otherValue is short)
        {
            return AddDoubleInt(value, (long)(short)otherValue);
        }

        if (otherValue is ushort)
        {
            return AddDoubleUInt(value, (ulong)(ushort)otherValue);
        }

        if (otherValue is byte)
        {
            return AddDoubleUInt(value, (ulong)(byte)otherValue);
        }

        if (otherValue is sbyte)
        {
            return AddDoubleInt(value, (long)(sbyte)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to ADD double and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static double AddDoubleDouble(double a, double b)
    {
        return a + b;
    }
    public static double AddDoubleFloat(double a, float b)
    {
        return a + b;
    }
    public static double AddDoubleDecimal(double a, decimal b)
    {
        return a + (double)b;
    }
    public static double AddDoubleInt(double d, long i)
    {
        return d + i;
    }

    public static double AddDoubleUInt(double d, ulong u)
    {
        return d + u;
    }

    #endregion

    #region Subtract

    public static double SubtractDouble(double value, object? otherValue)
    {
        if (otherValue is double)
        {
            return SubtractDoubleDouble(value, (double)otherValue);
        }

        if (otherValue is float)
        {
            return SubtractDoubleFloat(value, (float)otherValue);
        }

        if (otherValue is decimal)
        {
            return SubtractDoubleDecimal(value, (decimal)otherValue);
        }

        if (otherValue is long)
        {
            return SubtractDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return SubtractDoubleUInt(value, (ulong)otherValue);
        }

        if (otherValue is int)
        {
            return SubtractDoubleInt(value, (long)(int)otherValue);
        }

        if (otherValue is uint)
        {
            return SubtractDoubleUInt(value, (ulong)(uint)otherValue);
        }

        if (otherValue is short)
        {
            return SubtractDoubleInt(value, (long)(short)otherValue);
        }

        if (otherValue is ushort)
        {
            return SubtractDoubleUInt(value, (ulong)(ushort)otherValue);
        }

        if (otherValue is byte)
        {
            return SubtractDoubleUInt(value, (ulong)(byte)otherValue);
        }

        if (otherValue is sbyte)
        {
            return SubtractDoubleInt(value, (long)(sbyte)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to SUBTRACT double and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static double SubtractDoubleDouble(double a, double b)
    {
        return a - b;
    }

    public static double SubtractDoubleFloat(double a, float b)
    {
        return a - b;
    }

    public static double SubtractDoubleDecimal(double a, decimal b)
    {
        return a - (double)b;
    }

    public static double SubtractDoubleInt(double d, long i)
    {
        return d - i;
    }

    public static double SubtractDoubleUInt(double d, ulong u)
    {
        return d - u;
    }

    #endregion

    #region Multiply

    public static double MultiplyDouble(double value, object? otherValue)
    {
        if (otherValue is double)
        {
            return MultiplyDoubleDouble(value, (double)otherValue);
        }

        if (otherValue is float)
        {
            return MultiplyDoubleFloat(value, (float)otherValue);
        }

        if (otherValue is decimal)
        {
            return MultiplyDoubleDecimal(value, (decimal)otherValue);
        }

        if (otherValue is long)
        {
            return MultiplyDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return MultiplyDoubleUInt(value, (ulong)otherValue);
        }

        if (otherValue is int)
        {
            return MultiplyDoubleInt(value, (long)(int)otherValue);
        }

        if (otherValue is uint)
        {
            return MultiplyDoubleUInt(value, (ulong)(uint)otherValue);
        }

        if (otherValue is short)
        {
            return MultiplyDoubleInt(value, (long)(short)otherValue);
        }

        if (otherValue is ushort)
        {
            return MultiplyDoubleUInt(value, (ulong)(ushort)otherValue);
        }

        if (otherValue is byte)
        {
            return MultiplyDoubleUInt(value, (ulong)(byte)otherValue);
        }

        if (otherValue is sbyte)
        {
            return MultiplyDoubleInt(value, (long)(sbyte)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to MULTIPLY double and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static double MultiplyDoubleDouble(double a, double b)
    {
        return a * b;
    }
    public static double MultiplyDoubleFloat(double a, float b)
    {
        return a * b;
    }

    public static double MultiplyDoubleDecimal(double a, decimal b)
    {
        return a * (double)b;
    }

    public static double MultiplyDoubleInt(double d, long i)
    {
        return d * i;
    }

    public static double MultiplyDoubleUInt(double d, ulong u)
    {
        return d * u;
    }

    #endregion

    #region Divide

    public static double DivideDouble(double value, object? otherValue)
    {
        if (otherValue is double)
        {
            return DivideDoubleDouble(value, (double)otherValue);
        }

        if (otherValue is float)
        {
            return DivideDoubleFloat(value, (float)otherValue);
        }

        if (otherValue is decimal)
        {
            return DivideDoubleDecimal(value, (decimal)otherValue);
        }

        if (otherValue is long)
        {
            return DivideDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return DivideDoubleUInt(value, (ulong)otherValue);
        }

        if (otherValue is int)
        {
            return DivideDoubleInt(value, (long)(int)otherValue);
        }

        if (otherValue is uint)
        {
            return DivideDoubleUInt(value, (ulong)(uint)otherValue);
        }

        if (otherValue is short)
        {
            return DivideDoubleInt(value, (long)(short)otherValue);
        }

        if (otherValue is ushort)
        {
            return DivideDoubleUInt(value, (ulong)(ushort)otherValue);
        }

        if (otherValue is byte)
        {
            return DivideDoubleUInt(value, (ulong)(byte)otherValue);
        }

        if (otherValue is sbyte)
        {
            return DivideDoubleInt(value, (long)(sbyte)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to DIVIDE double and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static double DivideDoubleDouble(double a, double b)
    {
        if (b == 0)
        {
            return double.NaN;
        }

        return a / b;
    }
    public static double DivideDoubleFloat(double a, float b)
    {
        if (b == 0)
        {
            return double.NaN;
        }

        return a / b;
    }
    public static double DivideDoubleDecimal(double a, decimal b)
    {
        if (b == 0)
        {
            return double.NaN;
        }

        return a / (double)b;
    }
    public static double DivideDoubleInt(double d, long i)
    {
        if (i == 0)
        {
            throw new CelDivideByZeroException("Cannot divide value by zero.");
        }

        return d / i;
    }

    public static double DivideDoubleUInt(double d, ulong u)
    {
        if (u == 0)
        {
            throw new CelDivideByZeroException("Cannot divide value by zero.");
        }

        return d / u;
    }

    #endregion

    #region Modulus

    public static double ModulusDouble(double value, object? otherValue)
    {
        if (otherValue is double)
        {
            return ModulusDoubleDouble(value, (double)otherValue);
        }

        if (otherValue is float)
        {
            return ModulusDoubleFloat(value, (float)otherValue);
        }

        if (otherValue is decimal)
        {
            return ModulusDoubleDecimal(value, (decimal)otherValue);
        }

        if (otherValue is long)
        {
            return ModulusDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return ModulusDoubleUInt(value, (ulong)otherValue);
        }

        if (otherValue is int)
        {
            return ModulusDoubleInt(value, (long)(int)otherValue);
        }

        if (otherValue is uint)
        {
            return ModulusDoubleUInt(value, (ulong)(uint)otherValue);
        }

        if (otherValue is short)
        {
            return ModulusDoubleInt(value, (long)(short)otherValue);
        }

        if (otherValue is ushort)
        {
            return ModulusDoubleUInt(value, (ulong)(ushort)otherValue);
        }

        if (otherValue is byte)
        {
            return ModulusDoubleUInt(value, (ulong)(byte)otherValue);
        }

        if (otherValue is sbyte)
        {
            return ModulusDoubleInt(value, (long)(sbyte)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to MODULUS double and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static double ModulusDoubleDouble(double a, double b)
    {
        throw new CelNoSuchOverloadException("No overload exists to MODULUS double and double.");
    }

    public static double ModulusDoubleFloat(double a, float b)
    {
        throw new CelNoSuchOverloadException("No overload exists to MODULUS double and float.");
    }

    public static double ModulusDoubleDecimal(double a, decimal b)
    {
        throw new CelNoSuchOverloadException("No overload exists to MODULUS double and decimal.");
    }

    public static double ModulusDoubleInt(double d, long i)
    {
        if (i == 0)
        {
            throw new CelModulusByZeroException("Cannot calculate modulus of value by zero.");
        }

        return d % i;
    }

    public static double ModulusDoubleUInt(double d, ulong u)
    {
        if (u == 0)
        {
            throw new CelModulusByZeroException("Cannot calculate modulus of value by zero.");
        }

        return d % u;
    }

    #endregion

    #region Negate

    public static object? NegateDouble(double doubleValue)
    {
        return -doubleValue;
    }

    #endregion
}