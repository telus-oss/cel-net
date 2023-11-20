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

        if (otherValue is long)
        {
            return CompareDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return CompareDoubleUInt(value, (ulong)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to compare double to type '{value.GetType().FullName ?? "null"}'.");
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

        return CompareDoubleDouble(d, i);
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

        return CompareDoubleDouble(d, u);
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

        if (value is long intValue)
        {
            return ConvertDoubleInt(intValue);
        }

        if (value is ulong uintValue)
        {
            return ConvertDoubleUInt(uintValue);
        }

        if (value is string strValue)
        {
            return ConvertDoubleString(strValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to convert type '{value.GetType().FullName ?? "null"}' to double.");
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

        if (otherValue is long)
        {
            return AddDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return AddDoubleUInt(value, (ulong)otherValue);
        }


        throw new CelNoSuchOverloadException($"No overload exists to ADD double and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static double AddDoubleDouble(double a, double b)
    {
        return a + b;
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

        if (otherValue is long)
        {
            return SubtractDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return SubtractDoubleUInt(value, (ulong)otherValue);
        }


        throw new CelNoSuchOverloadException($"No overload exists to SUBTRACT double and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static double SubtractDoubleDouble(double a, double b)
    {
        return a - b;
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

        if (otherValue is long)
        {
            return MultiplyDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return MultiplyDoubleUInt(value, (ulong)otherValue);
        }


        throw new CelNoSuchOverloadException($"No overload exists to MULTIPLY double and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static double MultiplyDoubleDouble(double a, double b)
    {
        return a * b;
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

        if (otherValue is long)
        {
            return DivideDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return DivideDoubleUInt(value, (ulong)otherValue);
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

        if (otherValue is long)
        {
            return ModulusDoubleInt(value, (long)otherValue);
        }

        if (otherValue is ulong)
        {
            return ModulusDoubleUInt(value, (ulong)otherValue);
        }


        throw new CelNoSuchOverloadException($"No overload exists to MODULUS double and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static double ModulusDoubleDouble(double a, double b)
    {
        throw new CelNoSuchOverloadException("No overload exists to MODULUS double and double.");
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