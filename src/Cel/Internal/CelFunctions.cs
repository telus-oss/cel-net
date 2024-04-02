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
using System.Collections.Concurrent;
using Cel.Helpers;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Enum = System.Enum;

namespace Cel.Internal;

public static class CelFunctions
{
    public static void InitializeFunctions(ConcurrentDictionary<string, List<FunctionRegistration>> functions)
    {
        functions.RegisterFunction("bool", new[] { typeof(bool) }, ConvertBool);

        functions.RegisterFunction("bytes", new[] { typeof(ByteString) }, ConvertBytes);
        functions.RegisterFunction("bytes", new[] { typeof(string) }, ConvertBytes);
        
        functions.RegisterFunction("double", new[] { typeof(byte) }, ConvertDouble);
        functions.RegisterFunction("double", new[] { typeof(short) }, ConvertDouble);
        functions.RegisterFunction("double", new[] { typeof(ushort) }, ConvertDouble);
        functions.RegisterFunction("double", new[] { typeof(int) }, ConvertDouble);
        functions.RegisterFunction("double", new[] { typeof(uint) }, ConvertDouble);
        functions.RegisterFunction("double", new[] { typeof(long) }, ConvertDouble);
        functions.RegisterFunction("double", new[] { typeof(ulong) }, ConvertDouble);
        functions.RegisterFunction("double", new[] { typeof(double) }, ConvertDouble);
        functions.RegisterFunction("double", new[] { typeof(float) }, ConvertDouble);
        functions.RegisterFunction("double", new[] { typeof(double) }, ConvertDouble);
        functions.RegisterFunction("double", new[] { typeof(string) }, ConvertDouble);
        
        functions.RegisterFunction("decimal", new[] { typeof(byte) }, ConvertDecimal);
        functions.RegisterFunction("decimal", new[] { typeof(short) }, ConvertDecimal);
        functions.RegisterFunction("decimal", new[] { typeof(ushort) }, ConvertDecimal);
        functions.RegisterFunction("decimal", new[] { typeof(int) }, ConvertDecimal);
        functions.RegisterFunction("decimal", new[] { typeof(uint) }, ConvertDecimal);
        functions.RegisterFunction("decimal", new[] { typeof(long) }, ConvertDecimal);
        functions.RegisterFunction("decimal", new[] { typeof(ulong) }, ConvertDecimal);
        functions.RegisterFunction("decimal", new[] { typeof(double) }, ConvertDecimal);
        functions.RegisterFunction("decimal", new[] { typeof(float) }, ConvertDecimal);
        functions.RegisterFunction("decimal", new[] { typeof(decimal) }, ConvertDecimal);
        functions.RegisterFunction("decimal", new[] { typeof(string) }, ConvertDecimal);
        
        functions.RegisterFunction("int", new[] { typeof(byte) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(short) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(ushort) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(int) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(long) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(ulong) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(double) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(decimal) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(string) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(Enum) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(EnumValueDescriptor) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(DateTimeOffset) }, ConvertInt);
        functions.RegisterFunction("int", new[] { typeof(DateTime) }, ConvertInt);

        functions.RegisterFunction("uint", new[] { typeof(byte) }, ConvertUInt);
        functions.RegisterFunction("uint", new[] { typeof(short) }, ConvertUInt);
        functions.RegisterFunction("uint", new[] { typeof(ushort) }, ConvertUInt);
        functions.RegisterFunction("uint", new[] { typeof(int) }, ConvertUInt);
        functions.RegisterFunction("uint", new[] { typeof(uint) }, ConvertUInt);
        functions.RegisterFunction("uint", new[] { typeof(long) }, ConvertUInt);
        functions.RegisterFunction("uint", new[] { typeof(ulong) }, ConvertUInt);
        functions.RegisterFunction("uint", new[] { typeof(decimal) }, ConvertUInt);
        functions.RegisterFunction("uint", new[] { typeof(double) }, ConvertUInt);
        functions.RegisterFunction("uint", new[] { typeof(float) }, ConvertUInt);
        functions.RegisterFunction("uint", new[] { typeof(string) }, ConvertUInt);

        functions.RegisterFunction("string", new[] { typeof(string) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(bool) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(ByteString) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(double) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(decimal) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(float) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(long) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(ulong) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(decimal) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(DateTimeOffset) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(DateTime) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(TimeSpan) }, ConvertString);
        functions.RegisterFunction("string", new[] { typeof(Duration) }, ConvertString);

        functions.RegisterFunction("timestamp", new[] { typeof(string) }, ConvertTimestamp);
        functions.RegisterFunction("timestamp", new[] { typeof(long) }, ConvertTimestamp);
        functions.RegisterFunction("timestamp", new[] { typeof(Timestamp) }, ConvertTimestamp);
        functions.RegisterFunction("timestamp", new[] { typeof(DateTimeOffset) }, ConvertTimestamp);
        functions.RegisterFunction("timestamp", new[] { typeof(DateTime) }, ConvertTimestamp);

        functions.RegisterFunction("duration", new[] { typeof(Duration) }, ConvertDuration);
        functions.RegisterFunction("duration", new[] { typeof(TimeSpan) }, ConvertDuration);
        functions.RegisterFunction("duration", new[] { typeof(string) }, ConvertDuration);

        functions.RegisterFunction("type", new[] { typeof(object) }, ConvertType);
        functions.RegisterFunction("dyn", new[] { typeof(object) }, Dyn);

        functions.RegisterFunction("getHours", new[] { typeof(TimeSpan) }, GetHours);
        functions.RegisterFunction("getHours", new[] { typeof(TimeSpan), typeof(string) }, GetHours);
        functions.RegisterFunction("getHours", new[] { typeof(Duration) }, GetHours);
        functions.RegisterFunction("getHours", new[] { typeof(Duration), typeof(string) }, GetHours);
        functions.RegisterFunction("getHours", new[] { typeof(DateTimeOffset) }, GetHours);
        functions.RegisterFunction("getHours", new[] { typeof(DateTimeOffset), typeof(string) }, GetHours);
        functions.RegisterFunction("getHours", new[] { typeof(DateTime) }, GetHours);
        functions.RegisterFunction("getHours", new[] { typeof(DateTime), typeof(string) }, GetHours);
        functions.RegisterFunction("getHours", new[] { typeof(Timestamp) }, GetHours);
        functions.RegisterFunction("getHours", new[] { typeof(Timestamp), typeof(string) }, GetHours);

        functions.RegisterFunction("getMinutes", new[] { typeof(TimeSpan) }, GetMinutes);
        functions.RegisterFunction("getMinutes", new[] { typeof(TimeSpan), typeof(string) }, GetMinutes);
        functions.RegisterFunction("getMinutes", new[] { typeof(Duration) }, GetMinutes);
        functions.RegisterFunction("getMinutes", new[] { typeof(Duration), typeof(string) }, GetMinutes);
        functions.RegisterFunction("getMinutes", new[] { typeof(DateTimeOffset) }, GetMinutes);
        functions.RegisterFunction("getMinutes", new[] { typeof(DateTimeOffset), typeof(string) }, GetMinutes);
        functions.RegisterFunction("getMinutes", new[] { typeof(DateTime) }, GetMinutes);
        functions.RegisterFunction("getMinutes", new[] { typeof(DateTime), typeof(string) }, GetMinutes);
        functions.RegisterFunction("getMinutes", new[] { typeof(Timestamp) }, GetMinutes);
        functions.RegisterFunction("getMinutes", new[] { typeof(Timestamp), typeof(string) }, GetMinutes);

        functions.RegisterFunction("getSeconds", new[] { typeof(TimeSpan) }, GetSeconds);
        functions.RegisterFunction("getSeconds", new[] { typeof(TimeSpan), typeof(string) }, GetSeconds);
        functions.RegisterFunction("getSeconds", new[] { typeof(Duration) }, GetSeconds);
        functions.RegisterFunction("getSeconds", new[] { typeof(Duration), typeof(string) }, GetSeconds);
        functions.RegisterFunction("getSeconds", new[] { typeof(DateTimeOffset) }, GetSeconds);
        functions.RegisterFunction("getSeconds", new[] { typeof(DateTimeOffset), typeof(string) }, GetSeconds);
        functions.RegisterFunction("getSeconds", new[] { typeof(DateTime) }, GetSeconds);
        functions.RegisterFunction("getSeconds", new[] { typeof(DateTime), typeof(string) }, GetSeconds);
        functions.RegisterFunction("getSeconds", new[] { typeof(Timestamp) }, GetSeconds);
        functions.RegisterFunction("getSeconds", new[] { typeof(Timestamp), typeof(string) }, GetSeconds);

        functions.RegisterFunction("getMilliseconds", new[] { typeof(TimeSpan) }, GetMilliseconds);
        functions.RegisterFunction("getMilliseconds", new[] { typeof(TimeSpan), typeof(string) }, GetMilliseconds);
        functions.RegisterFunction("getMilliseconds", new[] { typeof(Duration) }, GetMilliseconds);
        functions.RegisterFunction("getMilliseconds", new[] { typeof(Duration), typeof(string) }, GetMilliseconds);
        functions.RegisterFunction("getMilliseconds", new[] { typeof(DateTimeOffset) }, GetMilliseconds);
        functions.RegisterFunction("getMilliseconds", new[] { typeof(DateTimeOffset), typeof(string) }, GetMilliseconds);
        functions.RegisterFunction("getMilliseconds", new[] { typeof(DateTime) }, GetMilliseconds);
        functions.RegisterFunction("getMilliseconds", new[] { typeof(DateTime), typeof(string) }, GetMilliseconds);
        functions.RegisterFunction("getMilliseconds", new[] { typeof(Timestamp) }, GetMilliseconds);
        functions.RegisterFunction("getMilliseconds", new[] { typeof(Timestamp), typeof(string) }, GetMilliseconds);

        functions.RegisterFunction("getFullYear", new[] { typeof(DateTimeOffset) }, GetFullYear);
        functions.RegisterFunction("getFullYear", new[] { typeof(DateTimeOffset), typeof(string) }, GetFullYear);
        functions.RegisterFunction("getFullYear", new[] { typeof(DateTime) }, GetFullYear);
        functions.RegisterFunction("getFullYear", new[] { typeof(DateTime), typeof(string) }, GetFullYear);
        functions.RegisterFunction("getFullYear", new[] { typeof(Timestamp) }, GetFullYear);
        functions.RegisterFunction("getFullYear", new[] { typeof(Timestamp), typeof(string) }, GetFullYear);

        functions.RegisterFunction("getMonth", new[] { typeof(DateTimeOffset) }, GetMonth);
        functions.RegisterFunction("getMonth", new[] { typeof(DateTimeOffset), typeof(string) }, GetMonth);
        functions.RegisterFunction("getMonth", new[] { typeof(DateTime) }, GetMonth);
        functions.RegisterFunction("getMonth", new[] { typeof(DateTime), typeof(string) }, GetMonth);
        functions.RegisterFunction("getMonth", new[] { typeof(Timestamp) }, GetMonth);
        functions.RegisterFunction("getMonth", new[] { typeof(Timestamp), typeof(string) }, GetMonth);

        functions.RegisterFunction("getDayOfYear", new[] { typeof(DateTimeOffset) }, GetDayOfYear);
        functions.RegisterFunction("getDayOfYear", new[] { typeof(DateTimeOffset), typeof(string) }, GetDayOfYear);
        functions.RegisterFunction("getDayOfYear", new[] { typeof(DateTime) }, GetDayOfYear);
        functions.RegisterFunction("getDayOfYear", new[] { typeof(DateTime), typeof(string) }, GetDayOfYear);
        functions.RegisterFunction("getDayOfYear", new[] { typeof(Timestamp) }, GetDayOfYear);
        functions.RegisterFunction("getDayOfYear", new[] { typeof(Timestamp), typeof(string) }, GetDayOfYear);

        functions.RegisterFunction("getDayOfMonth", new[] { typeof(DateTimeOffset) }, GetDayOfMonth);
        functions.RegisterFunction("getDayOfMonth", new[] { typeof(DateTimeOffset), typeof(string) }, GetDayOfMonth);
        functions.RegisterFunction("getDayOfMonth", new[] { typeof(DateTime) }, GetDayOfMonth);
        functions.RegisterFunction("getDayOfMonth", new[] { typeof(DateTime), typeof(string) }, GetDayOfMonth);
        functions.RegisterFunction("getDayOfMonth", new[] { typeof(Timestamp) }, GetDayOfMonth);
        functions.RegisterFunction("getDayOfMonth", new[] { typeof(Timestamp), typeof(string) }, GetDayOfMonth);

        functions.RegisterFunction("getDayOfWeek", new[] { typeof(DateTimeOffset) }, GetDayOfWeek);
        functions.RegisterFunction("getDayOfWeek", new[] { typeof(DateTimeOffset), typeof(string) }, GetDayOfWeek);
        functions.RegisterFunction("getDayOfWeek", new[] { typeof(DateTime) }, GetDayOfWeek);
        functions.RegisterFunction("getDayOfWeek", new[] { typeof(DateTime), typeof(string) }, GetDayOfWeek);
        functions.RegisterFunction("getDayOfWeek", new[] { typeof(Timestamp) }, GetDayOfWeek);
        functions.RegisterFunction("getDayOfWeek", new[] { typeof(Timestamp), typeof(string) }, GetDayOfWeek);

        functions.RegisterFunction("getDate", new[] { typeof(DateTimeOffset) }, GetDate);
        functions.RegisterFunction("getDate", new[] { typeof(DateTimeOffset), typeof(string) }, GetDate);
        functions.RegisterFunction("getDate", new[] { typeof(DateTime) }, GetDate);
        functions.RegisterFunction("getDate", new[] { typeof(DateTime), typeof(string) }, GetDate);
        functions.RegisterFunction("getDate", new[] { typeof(Timestamp) }, GetDate);
        functions.RegisterFunction("getDate", new[] { typeof(Timestamp), typeof(string) }, GetDate);

        functions.RegisterFunction("contains", new[] { typeof(string), typeof(string) }, Contains);
        functions.RegisterFunction("endsWith", new[] { typeof(string), typeof(string) }, EndsWith);
        functions.RegisterFunction("startsWith", new[] { typeof(string), typeof(string) }, StartsWith);
        functions.RegisterFunction("matches", new[] { typeof(string), typeof(string) }, Matches);

        functions.RegisterFunction("size", new[] { typeof(string) }, Size);
        functions.RegisterFunction("size", new[] { typeof(ByteString) }, Size);
        functions.RegisterFunction("size", new[] { typeof(IDictionary) }, Size);
        functions.RegisterFunction("size", new[] { typeof(IList) }, Size);
        functions.RegisterFunction("size", new[] { typeof(object[]) }, Size);

        functions.RegisterFunction("has", new[] { typeof(object) }, Has);
    }

    private static object ConvertBool(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Bool function requires 1 argument.");
        }

        return BooleanHelpers.ConvertBool(value[0]);
    }


    public static object? ConvertBytes(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Bytes function requires 1 argument.");
        }

        return ByteArrayHelpers.ConvertBytes(value[0]);
    }

    public static object? ConvertDouble(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Double function requires 1 argument.");
        }

        return DoubleHelpers.ConvertDouble(value[0]);
    }
    public static object? ConvertDecimal(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Decimal function requires 1 argument.");
        }

        return DecimalHelpers.ConvertDecimal(value[0]);
    }
    public static object? ConvertInt(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Int function requires 1 argument.");
        }

        return Int64Helpers.ConvertInt(value[0]);
    }

    public static object? ConvertUInt(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Uint function requires 1 argument.");
        }

        return UInt64Helpers.ConvertUInt(value[0]);
    }

    public static object? ConvertString(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("String function requires 1 argument.");
        }

        return StringHelpers.ConvertString(value[0]);
    }

    public static object? ConvertTimestamp(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Timestamp function requires 1 argument.");
        }

        var arg = value[0];

        if (arg is string strValue)
        {
            return TimestampHelpers.ConvertTimestampString(strValue);
        }

        if (arg is long longValue)
        {
            return TimestampHelpers.ConvertTimestampInt(longValue);
        }

        if (arg is Timestamp timestampValue)
        {
            return timestampValue;
        }

        if (arg is DateTimeOffset dateTimeOffsetValue)
        {
            return Timestamp.FromDateTimeOffset(dateTimeOffsetValue);
        }

        if (arg is DateTime dateTimeValue)
        {
            return Timestamp.FromDateTime(dateTimeValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to convert type '{value?.GetType().FullName ?? "null"}' to timestamp.");
    }

    public static object? ConvertDuration(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Duration function requires 1 argument.");
        }

        var arg = value[0];

        if (arg is string strValue)
        {
            return DurationHelpers.ConvertDurationString(strValue);
        }

        if (arg is Duration durationValue)
        {
            return durationValue;
        }

        if (arg is TimeSpan timespanValue)
        {
            return Duration.FromTimeSpan(timespanValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to convert type '{value?.GetType().FullName ?? "null"}' to duration.");
    }

    private static object? ConvertType(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Type function requires 1 argument.");
        }

        return TypeHelpers.ConvertType(value[0]);
    }


    public static object? Dyn(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new CelExpressionParserException("Dyn function requires 1 argument.");
        }

        var value = args[0];
        if (value is IMessage message)
        {
            value = MessageHelpers.UnwrapWellKnownMessage(message);
        }

        return value;
    }

    public static object? Contains(object?[] value)
    {
        if (value.Length != 2)
        {
            throw new CelExpressionParserException("Contains function requires 2 arguments.");
        }

        if (value[0] is string stringValue1 && value[1] is string stringValue2)
        {
            return StringHelpers.ContainsString(stringValue1, stringValue2);
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'contains' function with types '{value[0]?.GetType().FullName ?? "null"} and '{value[1]?.GetType().FullName ?? "null"}'.");
    }

    public static object? GetFullYear(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelExpressionParserException("GetFullYear function requires 1 or 2 arguments.");
        }

        var argument = value[0];
        try
        {
            object? timeZoneValue = null;
            if (value.Length == 2)
            {
                timeZoneValue = value[1];
            }

            if (argument is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.GetFullYearTimestamp(dateTimeOffsetValue, timeZoneValue);
            }

            if (argument is DateTime dateTimeValue)
            {
                return TimestampHelpers.GetFullYearTimestamp(dateTimeValue, timeZoneValue);
            }

            if (argument is Timestamp timestampValue)
            {
                return TimestampHelpers.GetFullYearTimestamp(timestampValue.ToDateTimeOffset(), timeZoneValue);
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

        if (value.Length == 2)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'getFullYear' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'getFullYear' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? GetDate(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelExpressionParserException("GetDate function requires 1 or 2 arguments.");
        }

        var argument = value[0];
        try
        {
            object? timeZoneValue = null;
            if (value.Length == 2)
            {
                timeZoneValue = value[1];
            }

            if (argument is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.GetDateTimestamp(dateTimeOffsetValue, timeZoneValue);
            }

            if (argument is DateTime dateTimeValue)
            {
                return TimestampHelpers.GetDateTimestamp(dateTimeValue, timeZoneValue);
            }

            if (argument is Timestamp timestampValue)
            {
                return TimestampHelpers.GetDateTimestamp(timestampValue.ToDateTimeOffset(), timeZoneValue);
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

        if (value.Length == 2)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'getDate' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'getDate' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? GetDayOfMonth(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelExpressionParserException("GetDayOfMonth function requires 1 or 2 arguments.");
        }

        var argument = value[0];
        try
        {
            object? timeZoneValue = null;
            if (value.Length == 2)
            {
                timeZoneValue = value[1];
            }

            if (argument is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.GetDayOfMonthTimestamp(dateTimeOffsetValue, timeZoneValue);
            }

            if (argument is DateTime dateTimeValue)
            {
                return TimestampHelpers.GetDayOfMonthTimestamp(dateTimeValue, timeZoneValue);
            }

            if (argument is Timestamp timestampValue)
            {
                return TimestampHelpers.GetDayOfMonthTimestamp(timestampValue.ToDateTimeOffset(), timeZoneValue);
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

        if (value.Length == 2)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'getDayOfMonth' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'getDayOfMonth' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? GetDayOfWeek(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelExpressionParserException("GetDayOfWeek function requires 1 or 2 arguments.");
        }

        var argument = value[0];
        try
        {
            object? timeZoneValue = null;
            if (value.Length == 2)
            {
                timeZoneValue = value[1];
            }

            if (argument is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.GetDayOfWeekTimestamp(dateTimeOffsetValue, timeZoneValue);
            }

            if (argument is DateTime dateTimeValue)
            {
                return TimestampHelpers.GetDayOfWeekTimestamp(dateTimeValue, timeZoneValue);
            }

            if (argument is Timestamp timestampValue)
            {
                return TimestampHelpers.GetDayOfWeekTimestamp(timestampValue.ToDateTimeOffset(), timeZoneValue);
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

        if (value.Length == 2)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'getDayOfWeek' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'getDayOfWeek' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? GetDayOfYear(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelExpressionParserException("GetDayOfYear function requires 1 or 2 arguments.");
        }

        var argument = value[0];
        try
        {
            object? timeZoneValue = null;
            if (value.Length == 2)
            {
                timeZoneValue = value[1];
            }

            if (argument is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.GetDayOfYearTimestamp(dateTimeOffsetValue, timeZoneValue);
            }

            if (argument is DateTime dateTimeValue)
            {
                return TimestampHelpers.GetDayOfYearTimestamp(dateTimeValue, timeZoneValue);
            }

            if (argument is Timestamp timestampValue)
            {
                return TimestampHelpers.GetDayOfYearTimestamp(timestampValue.ToDateTimeOffset(), timeZoneValue);
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

        if (value.Length == 2)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'getDayOfYear' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'getDayOfYear' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? GetMonth(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelExpressionParserException("GetMonth function requires 1 or 2 arguments.");
        }

        var argument = value[0];
        try
        {
            object? timeZoneValue = null;
            if (value.Length == 2)
            {
                timeZoneValue = value[1];
            }

            if (argument is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.GetMonthTimestamp(dateTimeOffsetValue, timeZoneValue);
            }

            if (argument is DateTime dateTimeValue)
            {
                return TimestampHelpers.GetMonthTimestamp(dateTimeValue, timeZoneValue);
            }

            if (argument is Timestamp timestampValue)
            {
                return TimestampHelpers.GetMonthTimestamp(timestampValue.ToDateTimeOffset(), timeZoneValue);
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

        if (value.Length == 2)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'getMonth' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'getMonth' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? GetHours(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelExpressionParserException("GetHours function requires 1 or 2 arguments.");
        }

        var argument = value[0];
        try
        {
            if (argument is TimeSpan timeSpanValue)
            {
                return DurationHelpers.GetHoursDuration(timeSpanValue);
            }

            if (argument is Duration durationValue)
            {
                return DurationHelpers.GetHoursDuration(durationValue.ToTimeSpan());
            }

            object? timeZoneValue = null;
            if (value.Length == 2)
            {
                timeZoneValue = value[1];
            }

            if (argument is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.GetHoursTimestamp(dateTimeOffsetValue, timeZoneValue);
            }

            if (argument is DateTime dateTimeValue)
            {
                return TimestampHelpers.GetHoursTimestamp(dateTimeValue, timeZoneValue);
            }

            if (argument is Timestamp timestampValue)
            {
                return TimestampHelpers.GetHoursTimestamp(timestampValue.ToDateTimeOffset(), timeZoneValue);
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

        if (value.Length == 2)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'getHours' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'getHours' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? GetMinutes(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelExpressionParserException("GetMinutes function requires 1 or 2 arguments.");
        }

        var argument = value[0];
        try
        {
            if (argument is TimeSpan timeSpanValue)
            {
                return DurationHelpers.GetMinutesDuration(timeSpanValue);
            }

            if (argument is Duration durationValue)
            {
                return DurationHelpers.GetMinutesDuration(durationValue.ToTimeSpan());
            }

            object? timeZoneValue = null;
            if (value.Length == 2)
            {
                timeZoneValue = value[1];
            }

            if (argument is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.GetMinutesTimestamp(dateTimeOffsetValue, timeZoneValue);
            }

            if (argument is DateTime dateTimeValue)
            {
                return TimestampHelpers.GetMinutesTimestamp(dateTimeValue, timeZoneValue);
            }

            if (argument is Timestamp timestampValue)
            {
                return TimestampHelpers.GetMinutesTimestamp(timestampValue.ToDateTimeOffset(), timeZoneValue);
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

        if (value.Length == 2)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'getMinutes' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'getMinutes' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? GetSeconds(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelExpressionParserException("GetSeconds function requires 1 or 2 arguments.");
        }

        var argument = value[0];
        try
        {
            if (argument is TimeSpan timeSpanValue)
            {
                return DurationHelpers.GetSecondsDuration(timeSpanValue);
            }

            if (argument is Duration durationValue)
            {
                return DurationHelpers.GetSecondsDuration(durationValue.ToTimeSpan());
            }

            object? timeZoneValue = null;
            if (value.Length == 2)
            {
                timeZoneValue = value[1];
            }

            if (argument is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.GetSecondsTimestamp(dateTimeOffsetValue, timeZoneValue);
            }

            if (argument is DateTime dateTimeValue)
            {
                return TimestampHelpers.GetSecondsTimestamp(dateTimeValue, timeZoneValue);
            }

            if (argument is Timestamp timestampValue)
            {
                return TimestampHelpers.GetSecondsTimestamp(timestampValue.ToDateTimeOffset(), timeZoneValue);
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

        if (value.Length == 2)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'getSeconds' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'getSeconds' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? GetMilliseconds(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelExpressionParserException("GetMilliseconds function requires 1 or 2 arguments.");
        }

        var argument = value[0];
        try
        {
            if (argument is TimeSpan timeSpanValue)
            {
                return DurationHelpers.GetMillisecondsDuration(timeSpanValue);
            }

            if (argument is Duration durationValue)
            {
                return DurationHelpers.GetMillisecondsDuration(durationValue.ToTimeSpan());
            }

            object? timeZoneValue = null;
            if (value.Length == 2)
            {
                timeZoneValue = value[1];
            }

            if (argument is DateTimeOffset dateTimeOffsetValue)
            {
                return TimestampHelpers.GetMillisecondsTimestamp(dateTimeOffsetValue, timeZoneValue);
            }

            if (argument is DateTime dateTimeValue)
            {
                return TimestampHelpers.GetMillisecondsTimestamp(dateTimeValue, timeZoneValue);
            }

            if (argument is Timestamp timestampValue)
            {
                return TimestampHelpers.GetMillisecondsTimestamp(timestampValue.ToDateTimeOffset(), timeZoneValue);
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

        if (value.Length == 2)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'getMilliseconds' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'getMilliseconds' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? Matches(object?[] value)
    {
        if (value.Length != 2)
        {
            throw new CelExpressionParserException("Matches function requires 2 arguments.");
        }

        if (value[0] is string stringValue1 && value[1] is string stringValue2)
        {
            return StringHelpers.MatchesString(stringValue1, stringValue2);
        }


        throw new CelNoSuchOverloadException($"No overload exists for 'matches' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
    }

    public static object? Size(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Size function requires 1 argument.");
        }

        if (value[0] == null)
        {
            return 0;
        }

        if (value[0] is string stringValue)
        {
            return StringHelpers.SizeString(stringValue);
        }

        if (value[0] is ByteString byteArrayValue)
        {
            return ByteArrayHelpers.SizeBytes(byteArrayValue);
        }

        if (value[0] is IDictionary mapValue)
        {
            return MapHelpers.SizeMap(mapValue);
        }

        if (value[0] is object[] objectArrayValue)
        {
            return ListHelpers.SizeList(objectArrayValue);
        }

        if (value[0] is IList objectListValue)
        {
            return ListHelpers.SizeList(objectListValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'size' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    public static object? EndsWith(object?[] value)
    {
        if (value.Length != 2)
        {
            throw new CelExpressionParserException("EndsWith function requires 2 arguments.");
        }

        if (value[0] is string stringValue1 && value[1] is string stringValue2)
        {
            return StringHelpers.EndsWithString(stringValue1, stringValue2);
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'endsWith' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
    }

    public static object? StartsWith(object?[] value)
    {
        if (value.Length != 2)
        {
            throw new CelExpressionParserException("StartsWith function requires 2 arguments.");
        }

        if (value[0] is string stringValue1 && value[1] is string stringValue2)
        {
            return StringHelpers.StartsWithString(stringValue1, stringValue2);
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'startsWith' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
    }

    private static object Has(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelExpressionParserException("Has function requires 1 argument.");
        }

        var type = value[0]?.GetType();
        if (type == null)
        {
            return false;
        }

        if (value[0] is CelNoSuchField)
        {
            return false;
        }

        if (value[0] is IList valueList)
        {
            return valueList.Count > 0;
        }

        //these return true if they are set
        //non-set variables will be null (when using proto2 optional, etc)
        if (value[0] is bool
            || value[0] is int
            || value[0] is uint
            || value[0] is long
            || value[0] is ulong
            || value[0] is float
            || value[0] is double)
        {
            return true;
        }

        if (value[0] is string valueString)
        {
            return !string.IsNullOrEmpty(valueString);
        }

        if (value[0] is Enum)
        {
            return ((int?)value[0]).GetValueOrDefault() != 0;
        }

        if (value[0] is EnumValueDescriptor)
        {
            return true;
        }

        if (value[0] is ICollection valueCollection)
        {
            return valueCollection.Count > 0;
        }

        if (value[0] is IDictionary valueDict)
        {
            return valueDict.Count > 0;
        }

        if (type.IsClass)
        {
            return value[0] is not null;
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'has' function with argument type '{value[0]?.GetType().FullName ?? "null"}'.");
    }
}