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
using System.Text.RegularExpressions;
using Google.Protobuf.WellKnownTypes;

namespace Cel.Helpers;

public static class DurationHelpers
{
    #region Compare

    public static int CompareDuration(Duration value, object? otherValue)
    {
        if (otherValue == null)
        {
            return -1;
        }

        if (otherValue is Duration otherValueDuration)
        {
            return value.CompareTo(otherValueDuration);
        }

        if (otherValue is TimeSpan otherValueTimeSpan)
        {
            return value.CompareTo(Duration.FromTimeSpan(otherValueTimeSpan));
        }

        throw new CelNoSuchOverloadException($"No overload exists to compare 'Google.Protobuf.WellKnownTypes.Duration' to type '{value.GetType().FullName ?? "null"}'.");
    }

    public static int CompareDuration(TimeSpan value, object? otherValue)
    {
        if (otherValue == null)
        {
            return -1;
        }

        if (otherValue is TimeSpan otherTimeSpan)
        {
            return value.CompareTo(otherTimeSpan);
        }

        if (otherValue is Duration otherValueDuration)
        {
            var timespan = otherValueDuration.ToTimeSpan();
            return value.CompareTo(timespan);
        }

        throw new CelNoSuchOverloadException($"No overload exists to compare 'System.TimeSpan' to type '{value.GetType().FullName ?? "null"}'.");
    }

    #endregion

    #region Convert

    public static Duration ConvertDuration(object value)
    {
        if (value is string valueString)
        {
            return ConvertDurationString(valueString);
        }

        if (value is Duration valueDuration)
        {
            return valueDuration;
        }

        if (value is TimeSpan valueTimeSpan)
        {
            return Duration.FromTimeSpan(valueTimeSpan);
        }

        throw new CelNoSuchOverloadException($"No overload exists to convert type '{value.GetType().FullName ?? "null"}' to duration.");
    }

    public static Duration ConvertDurationString(string value)
    {
        if (value == "0")
        {
            return new Duration();
        }

        value = value.Trim();

        decimal nanos = 0;
        var sign = 1;

        if (!Regex.IsMatch(value, "^[-+]?([0-9]*(\\.[0-9]*)?[a-z]+)+$"))
        {
            throw new CelStringParsingException("Invalid duration.");
        }

        if (value.StartsWith("-", StringComparison.Ordinal))
        {
            sign = -1;
            value = value.Substring(1);
        }
        else if (value.StartsWith("+", StringComparison.Ordinal))
        {
            sign = -1;
            value = value.Substring(1);
        }

        var regex = new Regex("([0-9]*(\\.[0-9]*)?)([a-z]+)");
        var matches = regex.Matches(value);

        if (matches == null)
        {
            //compiler want's a check here incase matches is null.
            throw new CelStringParsingException("Invalid duration.");
        }

        foreach (Match? match in matches)
        {
            if (match == null || !match.Success)
            {
                throw new CelStringParsingException("Invalid duration.");
            }

            var scale = match.Groups[1].Value;
            var units = match.Groups[3].Value;

            if (!decimal.TryParse(scale, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var scaleDecimal))
            {
                throw new CelStringParsingException("Invalid duration.");
            }

            var unitNanos = GetNanos(units);

            nanos += scaleDecimal * unitNanos;
        }


        var result = new Duration
        {
            Seconds = Convert.ToInt64(sign * Math.Truncate(nanos / 1000000000)),
            Nanos = Convert.ToInt32(sign * (nanos % 1000000000))
        };
        ValidateDurationRange(result);
        return result;
    }

    public static void ValidateDurationRange(TimeSpan timeSpan)
    {
        if (timeSpan < TimeSpan.FromSeconds(-315576000000) || timeSpan > TimeSpan.FromSeconds(315576000000))
        {
            throw new CelArgumentRangeException($"Timespan ticks value '{timeSpan.Ticks}' exceeds the valid CEL duration range.");
        }
    }

    public static void ValidateDurationRange(Duration duration)
    {
        if (duration.Seconds < -315576000000 || duration.Seconds > 315576000000)
        {
            throw new CelArgumentRangeException($"Duration value seconds value '{duration.Seconds}' exceeds the valid CEL duration range.");
        }
    }

    private static decimal GetTicks(string units)
    {
        if (units == "ns")
        {
            return 0.01m;
        }

        if (units == "us")
        {
            return 10;
        }

        if (units == "ms")
        {
            return 10000;
        }

        if (units == "s")
        {
            return 10000000;
        }

        if (units == "m")
        {
            return 600000000;
        }

        if (units == "h")
        {
            return 36000000000;
        }

        throw new CelStringParsingException("Invalid duration units.");
    }

    private static decimal GetNanos(string units)
    {
        if (units == "ns")
        {
            return 1m;
        }

        if (units == "us")
        {
            return 1000;
        }

        if (units == "ms")
        {
            return 1000000;
        }

        if (units == "s")
        {
            return 1000000000;
        }

        if (units == "m")
        {
            return 60000000000;
        }

        if (units == "h")
        {
            return 3600000000000;
        }

        throw new CelStringParsingException("Invalid duration units.");
    }

    #endregion

    #region Add

    public static object? AddDuration(Duration value, object? otherValue)
    {
        if (otherValue is TimeSpan)
        {
            return AddDurationDuration(value, (TimeSpan)otherValue);
        }

        if (otherValue is Duration)
        {
            return AddDurationDuration(value, (Duration)otherValue);
        }

        if (otherValue is DateTimeOffset)
        {
            return AddDurationTimestamp(value, (DateTimeOffset)otherValue);
        }

        if (otherValue is DateTime)
        {
            return AddDurationTimestamp(value, (DateTime)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to ADD 'Google.Protobuf.WellKnownTypes.Duration' and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static object? AddDuration(TimeSpan value, object? otherValue)
    {
        if (otherValue is TimeSpan)
        {
            return AddDurationDuration(value, (TimeSpan)otherValue);
        }

        if (otherValue is Duration)
        {
            return AddDurationDuration(value, (Duration)otherValue);
        }

        if (otherValue is DateTimeOffset)
        {
            return AddDurationTimestamp(value, (DateTimeOffset)otherValue);
        }

        if (otherValue is DateTime)
        {
            return AddDurationTimestamp(value, (DateTime)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to ADD 'System.TimeSpan' and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static Duration AddDurationDuration(Duration a, TimeSpan b)
    {
        var result = a + Duration.FromTimeSpan(b);
        ValidateDurationRange(result!);
        return result!;
    }

    public static Duration AddDurationDuration(Duration a, Duration b)
    {
        var result = a + b;
        ValidateDurationRange(result!);
        return result!;
    }

    public static TimeSpan AddDurationDuration(TimeSpan a, TimeSpan b)
    {
        var result = a.Add(b);
        ValidateDurationRange(result);
        return result;
    }

    public static TimeSpan AddDurationDuration(TimeSpan a, Duration b)
    {
        var result = a.Add(b.ToTimeSpan());
        ValidateDurationRange(result);
        return result;
    }

    public static DateTimeOffset AddDurationTimestamp(TimeSpan a, DateTimeOffset b)
    {
        var result = b.Add(a);

        return result;
    }

    public static DateTimeOffset AddDurationTimestamp(Duration a, DateTimeOffset b)
    {
        var result = b.Add(a.ToTimeSpan());

        return result;
    }

    #endregion

    #region Subtract

    public static object? SubtractDuration(TimeSpan value, object? otherValue)
    {
        if (otherValue is TimeSpan)
        {
            return SubtractDurationDuration(value, (TimeSpan)otherValue);
        }

        if (otherValue is Duration)
        {
            return SubtractDurationDuration(value, (Duration)otherValue);
        }

        if (otherValue is DateTimeOffset)
        {
            return SubtractDurationTimestamp(value, (DateTimeOffset)otherValue);
        }

        if (otherValue is DateTime)
        {
            return SubtractDurationTimestamp(value, (DateTime)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to SUBTRACT 'System.TimeSpan' and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static object? SubtractDuration(Duration value, object? otherValue)
    {
        if (otherValue is TimeSpan)
        {
            return SubtractDurationDuration(value, (TimeSpan)otherValue);
        }

        if (otherValue is Duration)
        {
            return SubtractDurationDuration(value, (Duration)otherValue);
        }

        if (otherValue is DateTimeOffset)
        {
            return SubtractDurationTimestamp(value, (DateTimeOffset)otherValue);
        }

        if (otherValue is DateTime)
        {
            return SubtractDurationTimestamp(value, (DateTime)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to SUBTRACT 'Google.Protobuf.WellKnownTypes.Duration' and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static Duration SubtractDurationDuration(Duration a, Duration b)
    {
        var result = a - b;
        ValidateDurationRange(result!);
        return result!;
    }

    public static Duration SubtractDurationDuration(Duration a, TimeSpan b)
    {
        var result = a - Duration.FromTimeSpan(b);
        ValidateDurationRange(result!);
        return result!;
    }

    public static TimeSpan SubtractDurationDuration(TimeSpan a, TimeSpan b)
    {
        var result = a.Subtract(b);
        ValidateDurationRange(result);
        return result;
    }

    public static TimeSpan SubtractDurationDuration(TimeSpan a, Duration b)
    {
        var result = a.Subtract(b.ToTimeSpan());
        ValidateDurationRange(result);
        return result;
    }

    public static DateTimeOffset SubtractDurationTimestamp(TimeSpan a, DateTimeOffset b)
    {
        return b.Subtract(a);
    }

    public static DateTimeOffset SubtractDurationTimestamp(Duration a, DateTimeOffset b)
    {
        return b.Subtract(a.ToTimeSpan());
    }

    #endregion

    #region Duration Components

    public static long GetHoursDuration(TimeSpan timeSpan)
    {
        return (long)timeSpan.TotalHours;
    }

    public static long GetSecondsDuration(TimeSpan timeSpan)
    {
        return (long)timeSpan.TotalSeconds;
    }

    public static long GetMinutesDuration(TimeSpan timeSpan)
    {
        return (long)timeSpan.TotalMinutes;
    }

    public static long GetMillisecondsDuration(TimeSpan timeSpan)
    {
        return timeSpan.Milliseconds;
    }

    #endregion
}