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

public static class DecimalHelpers
{
    #region Negate

    public static object? NegateDecimal(decimal decimalValue)
    {
        return -decimalValue;
    }

    #endregion

    #region Compare

    public static int CompareDecimal(decimal value, object? otherValue)
    {
        if (otherValue is decimal) return CompareDecimalDecimal(value, (decimal)otherValue);

        if (otherValue is double) return CompareDecimalDouble(value, (double)otherValue);

        if (otherValue is float) return CompareDecimalFloat(value, (float)otherValue);

        if (otherValue is byte) return CompareDecimalInt(value, (byte)otherValue);

        if (otherValue is short) return CompareDecimalInt(value, (short)otherValue);

        if (otherValue is ushort) return CompareDecimalUInt(value, (ushort)otherValue);

        if (otherValue is int) return CompareDecimalInt(value, (int)otherValue);

        if (otherValue is uint) return CompareDecimalUInt(value, (uint)otherValue);

        if (otherValue is long) return CompareDecimalInt(value, (long)otherValue);

        if (otherValue is ulong) return CompareDecimalUInt(value, (ulong)otherValue);

        throw new CelNoSuchOverloadException($"No overload exists to compare decimal to type '{value.GetType().FullName ?? "null"}'.");
    }

    public static int CompareDecimalDecimal(decimal a, decimal b)
    {
        if (a < b) return -1;

        if (a > b) return 1;

        return 0;
    }

    public static int CompareDecimalDouble(decimal a, double b)
    {
        const double decimalMin = (double)decimal.MinValue;
        const double decimalMax = (double)decimal.MaxValue;
        if (b.CompareTo(decimalMax) == 0) return 0;
        if (b.CompareTo(decimalMax) == 1) return -1;
        if (b.CompareTo(decimalMin) == 0) return 0;
        if (b.CompareTo(decimalMin) == -1) return 1;

        return a.CompareTo(Convert.ToDecimal(b));
    }

    public static int CompareDecimalFloat(decimal a, float b)
    {
        const float decimalMin = (float)decimal.MinValue;
        const float decimalMax = (float)decimal.MaxValue;
        if (b.CompareTo(decimalMax) == 0) return 0;
        if (b.CompareTo(decimalMax) == 1) return -1;
        if (b.CompareTo(decimalMin) == 0) return 0;
        if (b.CompareTo(decimalMin) == -1) return 1;

        return a.CompareTo(Convert.ToDecimal(b));
    }


    public static int CompareDecimalInt(decimal d, long i)
    {
        if (d < long.MinValue) return -1;

        if (d > long.MaxValue) return 1;

        return CompareDecimalDecimal(d, i);
    }

    public static int CompareDecimalUInt(decimal d, ulong u)
    {
        if (d < 0) return -1;

        if (d > ulong.MaxValue) return 1;

        return CompareDecimalDecimal(d, u);
    }

    #endregion

    #region Convert

    public static decimal ConvertDecimal(object? value)
    {
        if (value == null) return -1;

        if (value is decimal decimalValue) return decimalValue;

        if (value is byte byteValue) return ConvertDecimalInt(byteValue);

        if (value is short shortValue) return ConvertDecimalInt(shortValue);

        if (value is ushort ushortValue) return ConvertDecimalUInt(ushortValue);

        if (value is int intValue) return ConvertDecimalInt(intValue);

        if (value is uint uintValue) return ConvertDecimalUInt(uintValue);

        if (value is long int64Value) return ConvertDecimalInt(int64Value);

        if (value is ulong uint64Value) return ConvertDecimalUInt(uint64Value);

        if (value is double doubleValue) return ConvertDecimalDouble(doubleValue);

        if (value is float floatValue) return ConvertDecimalDouble(floatValue);

        if (value is string stringValue) return ConvertDecimalString(stringValue);

        throw new CelNoSuchOverloadException($"No overload exists to convert type '{value.GetType().FullName ?? "null"}' to decimal.");
    }

    public static decimal ConvertDecimalInt(long value)
    {
        return value;
    }

    public static decimal ConvertDecimalUInt(ulong value)
    {
        return value;
    }

    public static decimal ConvertDecimalString(string value)
    {
        if (decimal.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var decimalValue)) return decimalValue;

        throw new CelStringParsingException($"Could not parse decimal value '{value}'");
    }

    public static decimal ConvertDecimalDouble(double value)
    {
        const double decimalMin = (double)decimal.MinValue;
        const double decimalMax = (double)decimal.MaxValue;

        if (value < decimalMin || value > decimalMax) throw new CelArgumentRangeException("Double exceeds maximum range of decimal type.");

        return Convert.ToDecimal(value);
    }

    #endregion

    #region Add

    public static decimal AddDecimal(decimal value, object? otherValue)
    {
        if (otherValue is decimal) return AddDecimalDecimal(value, (decimal)otherValue);

        if (otherValue is double) return AddDecimalDouble(value, (double)otherValue);

        if (otherValue is float) return AddDecimalFloat(value, (float)otherValue);

        if (otherValue is byte) return AddDecimalInt(value, (long)otherValue);

        if (otherValue is short) return AddDecimalInt(value, (long)otherValue);

        if (otherValue is ushort) return AddDecimalUInt(value, (ulong)otherValue);

        if (otherValue is int) return AddDecimalInt(value, (long)otherValue);

        if (otherValue is uint) return AddDecimalUInt(value, (ulong)otherValue);

        if (otherValue is long) return AddDecimalInt(value, (long)otherValue);

        if (otherValue is ulong) return AddDecimalUInt(value, (ulong)otherValue);

        throw new CelNoSuchOverloadException($"No overload exists to ADD decimal and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static decimal AddDecimalDecimal(decimal a, decimal b)
    {
        return a + b;
    }

    public static decimal AddDecimalDouble(decimal a, double b)
    {
        const double decimalMin = (double)decimal.MinValue;
        const double decimalMax = (double)decimal.MaxValue;

        if (b < decimalMin || b > decimalMax) throw new CelArgumentRangeException("Double exceeds maximum range of decimal type.");

        return a + Convert.ToDecimal(b);
    }
    public static decimal AddDecimalFloat(decimal a, float b)
    {
        const float decimalMin = (float)decimal.MinValue;
        const float decimalMax = (float)decimal.MaxValue;

        if (b < decimalMin || b > decimalMax) throw new CelArgumentRangeException("Float exceeds maximum range of decimal type.");

        return a + Convert.ToDecimal(b);
    }

    public static decimal AddDecimalInt(decimal d, long i)
    {
        return d + i;
    }

    public static decimal AddDecimalUInt(decimal d, ulong u)
    {
        return d + u;
    }

    #endregion

    #region Subtract

    public static decimal SubtractDecimal(decimal value, object? otherValue)
    {
        if (otherValue is decimal) return SubtractDecimalDecimal(value, (decimal)otherValue);

        if (otherValue is double) return SubtractDecimalDouble(value, (double)otherValue);

        if (otherValue is float) return SubtractDecimalFloat(value, (float)otherValue);

        if (otherValue is byte) return SubtractDecimalInt(value, (long)otherValue);

        if (otherValue is short) return SubtractDecimalInt(value, (long)otherValue);

        if (otherValue is ushort) return SubtractDecimalUInt(value, (ulong)otherValue);

        if (otherValue is int) return SubtractDecimalInt(value, (long)otherValue);

        if (otherValue is uint) return SubtractDecimalUInt(value, (ulong)otherValue);

        if (otherValue is long) return SubtractDecimalInt(value, (long)otherValue);

        if (otherValue is ulong) return SubtractDecimalUInt(value, (ulong)otherValue);


        throw new CelNoSuchOverloadException($"No overload exists to SUBTRACT decimal and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static decimal SubtractDecimalDecimal(decimal a, decimal b)
    {
        return a - b;
    }

    public static decimal SubtractDecimalDouble(decimal a, double b)
    {
        const double decimalMin = (double)decimal.MinValue;
        const double decimalMax = (double)decimal.MaxValue;

        if (b < decimalMin || b > decimalMax) throw new CelArgumentRangeException("Double exceeds maximum range of decimal type.");

        return a - Convert.ToDecimal(b);
    }
    public static decimal SubtractDecimalFloat(decimal a, float b)
    {
        const float decimalMin = (float)decimal.MinValue;
        const float decimalMax = (float)decimal.MaxValue;

        if (b < decimalMin || b > decimalMax) throw new CelArgumentRangeException("Float exceeds maximum range of decimal type.");

        return a - Convert.ToDecimal(b);
    }
    public static decimal SubtractDecimalInt(decimal d, long i)
    {
        return d - i;
    }

    public static decimal SubtractDecimalUInt(decimal d, ulong u)
    {
        return d - u;
    }

    #endregion

    #region Multiply

    public static decimal MultiplyDecimal(decimal value, object? otherValue)
    {
        if (otherValue is decimal) return MultiplyDecimalDecimal(value, (decimal)otherValue);

        if (otherValue is double) return MultiplyDecimalDouble(value, (double)otherValue);

        if (otherValue is float) return MultiplyDecimalFloat(value, (float)otherValue);

        if (otherValue is byte) return MultiplyDecimalInt(value, (long)otherValue);

        if (otherValue is short) return MultiplyDecimalInt(value, (long)otherValue);

        if (otherValue is ushort) return MultiplyDecimalUInt(value, (ulong)otherValue);

        if (otherValue is int) return MultiplyDecimalInt(value, (long)otherValue);

        if (otherValue is uint) return MultiplyDecimalUInt(value, (ulong)otherValue);

        if (otherValue is long) return MultiplyDecimalInt(value, (long)otherValue);

        if (otherValue is ulong) return MultiplyDecimalUInt(value, (ulong)otherValue);

        throw new CelNoSuchOverloadException($"No overload exists to MULTIPLY decimal and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static decimal MultiplyDecimalDecimal(decimal a, decimal b)
    {
        return a * b;
    }

    public static decimal MultiplyDecimalDouble(decimal a, double b)
    {
        const double decimalMin = (double)decimal.MinValue;
        const double decimalMax = (double)decimal.MaxValue;

        if (b < decimalMin || b > decimalMax) throw new CelArgumentRangeException("Double exceeds maximum range of decimal type.");

        return a * Convert.ToDecimal(b);
    }
    public static decimal MultiplyDecimalFloat(decimal a, float b)
    {
        const float decimalMin = (float)decimal.MinValue;
        const float decimalMax = (float)decimal.MaxValue;

        if (b < decimalMin || b > decimalMax) throw new CelArgumentRangeException("Float exceeds maximum range of decimal type.");

        return a * Convert.ToDecimal(b);
    }
    public static decimal MultiplyDecimalInt(decimal d, long i)
    {
        return d * i;
    }

    public static decimal MultiplyDecimalUInt(decimal d, ulong u)
    {
        return d * u;
    }

    #endregion

    #region Divide

    public static decimal DivideDecimal(decimal value, object? otherValue)
    {
        if (otherValue is decimal) return DivideDecimalDecimal(value, (decimal)otherValue);

        if (otherValue is double) return DivideDecimalDouble(value, (double)otherValue);

        if (otherValue is float) return DivideDecimalFloat(value, (float)otherValue);

        if (otherValue is byte) return DivideDecimalInt(value, (long)otherValue);

        if (otherValue is short) return DivideDecimalInt(value, (long)otherValue);

        if (otherValue is ushort) return DivideDecimalUInt(value, (ulong)otherValue);

        if (otherValue is int) return DivideDecimalInt(value, (long)otherValue);

        if (otherValue is uint) return DivideDecimalUInt(value, (ulong)otherValue);

        if (otherValue is long) return DivideDecimalInt(value, (long)otherValue);

        if (otherValue is ulong) return DivideDecimalUInt(value, (ulong)otherValue);


        throw new CelNoSuchOverloadException($"No overload exists to DIVIDE decimal and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static decimal DivideDecimalDecimal(decimal a, decimal b)
    {
        if (b == 0) throw new CelDivideByZeroException("Cannot divide value by zero.");

        return a / b;
    }

    public static decimal DivideDecimalDouble(decimal a, double b)
    {
        const double decimalMin = (double)decimal.MinValue;
        const double decimalMax = (double)decimal.MaxValue;

        if (b < decimalMin || b > decimalMax) throw new CelArgumentRangeException("Double exceeds maximum range of decimal type.");

        var decimalB = Convert.ToDecimal(b);

        if (decimalB == 0) throw new CelDivideByZeroException("Cannot divide value by zero.");

        return a / decimalB;
    }
    public static decimal DivideDecimalFloat(decimal a, float b)
    {
        const float decimalMin = (float)decimal.MinValue;
        const float decimalMax = (float)decimal.MaxValue;

        if (b < decimalMin || b > decimalMax) throw new CelArgumentRangeException("Float exceeds maximum range of decimal type.");

        var decimalB = Convert.ToDecimal(b);

        if (decimalB == 0) throw new CelDivideByZeroException("Cannot divide value by zero.");

        return a / decimalB;
    }

    public static decimal DivideDecimalInt(decimal d, long i)
    {
        if (i == 0) throw new CelDivideByZeroException("Cannot divide value by zero.");

        return d / i;
    }

    public static decimal DivideDecimalUInt(decimal d, ulong u)
    {
        if (u == 0) throw new CelDivideByZeroException("Cannot divide value by zero.");

        return d / u;
    }

    #endregion

    #region Modulus

    public static decimal ModulusDecimal(decimal value, object? otherValue)
    {
        if (otherValue is decimal) return ModulusDecimalDecimal(value, (decimal)otherValue);

        if (otherValue is byte) return ModulusDecimalInt(value, (long)otherValue);

        if (otherValue is short) return ModulusDecimalInt(value, (long)otherValue);

        if (otherValue is ushort) return ModulusDecimalUInt(value, (ulong)otherValue);

        if (otherValue is int) return ModulusDecimalInt(value, (long)otherValue);

        if (otherValue is uint) return ModulusDecimalUInt(value, (ulong)otherValue);

        if (otherValue is long) return ModulusDecimalInt(value, (long)otherValue);

        if (otherValue is ulong) return ModulusDecimalUInt(value, (ulong)otherValue);


        throw new CelNoSuchOverloadException($"No overload exists to MODULUS decimal and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static decimal ModulusDecimalDecimal(decimal a, decimal b)
    {
        throw new CelNoSuchOverloadException("No overload exists to MODULUS decimal and decimal.");
    }

    public static decimal ModulusDecimalInt(decimal d, long i)
    {
        if (i == 0) throw new CelModulusByZeroException("Cannot calculate modulus of value by zero.");

        return d % i;
    }

    public static decimal ModulusDecimalUInt(decimal d, ulong u)
    {
        if (u == 0) throw new CelModulusByZeroException("Cannot calculate modulus of value by zero.");

        return d % u;
    }

    #endregion
}