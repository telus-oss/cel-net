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
using Google.Protobuf.WellKnownTypes;
using TimeZoneConverter;

namespace Cel.Helpers;

public static class TimestampHelpers
{
    #region Compare

    public static int CompareTimestamp(Timestamp value, object? otherValue)
    {
        if (otherValue == null)
        {
            return -1;
        }

        if (otherValue is DateTimeOffset otherDateTimeOffset)
        {
            return value.CompareTo(Timestamp.FromDateTimeOffset(otherDateTimeOffset));
        }

        if (otherValue is DateTime otherDateTime)
        {
            return value.CompareTo(Timestamp.FromDateTime(otherDateTime));
        }

        if (otherValue is Timestamp otherTimestamp)
        {
            return value.CompareTo(otherTimestamp);
        }

        throw new CelNoSuchOverloadException($"No overload exists to compare type 'Google.Protobuf.WellKnownTypes.Timestamp' AND '{value.GetType().FullName ?? "null"}'.");
    }

    public static int CompareTimestamp(DateTimeOffset value, object? otherValue)
    {
        if (otherValue == null)
        {
            return -1;
        }

        if (otherValue is DateTimeOffset otherDateTimeOffset)
        {
            return value.CompareTo(otherDateTimeOffset);
        }

        if (otherValue is DateTime otherDateTime)
        {
            return value.CompareTo(otherDateTime);
        }

        if (otherValue is Timestamp otherTimestamp)
        {
            return value.CompareTo(otherTimestamp.ToDateTimeOffset());
        }

        throw new CelNoSuchOverloadException($"No overload exists to compare type 'System.DateTimeOffset' AND '{value.GetType().FullName ?? "null"}'.");
    }

    #endregion

    #region Convert

    public static DateTimeOffset ConvertTimestampInt(long value)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(value);
        return dateTimeOffset;
    }

    public static DateTimeOffset ConvertTimestampString(string value)
    {
        value = value.Trim();

        var dateSplit = value.Split('.');
        if (dateSplit.Length == 2)
        {
            //.Net only handles 7 fractional milliseconds, so if more are provided, we need to chop it down to 7.
            var fractionalSeconds = dateSplit[dateSplit.Length - 1];
            if (fractionalSeconds.EndsWith("Z") && fractionalSeconds.Length > 8)
            {
                fractionalSeconds = fractionalSeconds.Substring(0, 7) + "Z";
            }

            dateSplit[dateSplit.Length - 1] = fractionalSeconds ?? "";
            value = string.Join(".", dateSplit);
        }

        var formats = new[]
        {
            "yyyy-MM-ddTHH:mm:ss'.'fffffffK",
            "yyyy-MM-ddTHH:mm:ss'.'ffffffK",
            "yyyy-MM-ddTHH:mm:ss'.'fffffK",
            "yyyy-MM-ddTHH:mm:ss'.'ffffK",
            "yyyy-MM-ddTHH:mm:ss'.'fffK",
            "yyyy-MM-ddTHH:mm:ss'.'ffK",
            "yyyy-MM-ddTHH:mm:ss'.'fK",
            "yyyy-MM-ddTHH:mm:ssK"
        };
        try
        {
            var dateTimeOffset = DateTimeOffset.ParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
            return dateTimeOffset;
        }
        catch (Exception x)
        {
            throw new CelArgumentRangeException($"Could not convert '{value}' to Timestamp.", x);
        }
    }

    #endregion

    #region Add

    public static DateTimeOffset AddTimestamp(DateTimeOffset value, object? otherValue)
    {
        if (otherValue is TimeSpan otherValueTimeSpan)
        {
            return AddTimestampDuration(value, otherValueTimeSpan);
        }

        if (otherValue is Duration otherValueDuration)
        {
            var timeSpan = otherValueDuration.ToTimeSpan();
            return AddTimestampDuration(value, timeSpan);
        }

        throw new CelNoSuchOverloadException($"No overload exists to ADD timestamp and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static DateTimeOffset AddTimestampDuration(DateTimeOffset Timestamp, TimeSpan duration)
    {
        return Timestamp.Add(duration);
    }

    #endregion

    #region Subtract

    public static object SubtractTimestamp(DateTimeOffset value, object? otherValue)
    {
        if (otherValue is TimeSpan)
        {
            return SubtractTimestampDuration(value, (TimeSpan)otherValue);
        }

        if (otherValue is DateTimeOffset)
        {
            return SubtractTimestampTimestamp(value, (DateTimeOffset)otherValue);
        }

        if (otherValue is DateTime)
        {
            return SubtractTimestampTimestamp(value, (DateTime)otherValue);
        }

        if (otherValue is Duration otherValueDuration)
        {
            var timeSpan = otherValueDuration.ToTimeSpan();
            return SubtractTimestampDuration(value, timeSpan);
        }

        throw new CelNoSuchOverloadException($"No overload exists to SUBTRACT timestamp and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static DateTimeOffset SubtractTimestampDuration(DateTimeOffset timestamp, TimeSpan duration)
    {
        return timestamp.Subtract(duration);
    }

    public static TimeSpan SubtractTimestampTimestamp(DateTimeOffset a, DateTimeOffset b)
    {
        var timeSpan = a.Subtract(b);
        DurationHelpers.ValidateDurationRange(timeSpan);
        return timeSpan;
    }

    #endregion

    #region Timestamp Components

    public static long GetFullYearTimestamp(DateTimeOffset dateTimeOffset, object? timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);
        var dateTimeOffsetInTimeZone = dateTimeOffset.ToOffset(timeZoneInfo.GetUtcOffset(dateTimeOffset));

        return dateTimeOffsetInTimeZone.Year;
    }

    public static long GetDateTimestamp(DateTimeOffset dateTimeOffset, object? timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);
        var dateTimeOffsetInTimeZone = dateTimeOffset.ToOffset(timeZoneInfo.GetUtcOffset(dateTimeOffset));

        return dateTimeOffsetInTimeZone.Day;
    }

    public static long GetSecondsTimestamp(DateTimeOffset dateTimeOffset, object? timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);
        var dateTimeOffsetInTimeZone = dateTimeOffset.ToOffset(timeZoneInfo.GetUtcOffset(dateTimeOffset));

        return dateTimeOffsetInTimeZone.Second;
    }

    public static long GetMillisecondsTimestamp(DateTimeOffset dateTimeOffset, object? timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);
        var dateTimeOffsetInTimeZone = dateTimeOffset.ToOffset(timeZoneInfo.GetUtcOffset(dateTimeOffset));

        return dateTimeOffsetInTimeZone.Millisecond;
    }

    public static long GetHoursTimestamp(DateTimeOffset dateTimeOffset, object? timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);
        var dateTimeOffsetInTimeZone = dateTimeOffset.ToOffset(timeZoneInfo.GetUtcOffset(dateTimeOffset));

        return dateTimeOffsetInTimeZone.Hour;
    }

    public static long GetMinutesTimestamp(DateTimeOffset dateTimeOffset, object? timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);
        var dateTimeOffsetInTimeZone = dateTimeOffset.ToOffset(timeZoneInfo.GetUtcOffset(dateTimeOffset));

        return dateTimeOffsetInTimeZone.Minute;
    }

    public static long GetMonthTimestamp(DateTimeOffset dateTimeOffset, object? timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);
        var dateTimeOffsetInTimeZone = dateTimeOffset.ToOffset(timeZoneInfo.GetUtcOffset(dateTimeOffset));

        //CEL spec ranges months from 0-11, not 1-12.
        return dateTimeOffsetInTimeZone.Month - 1;
    }

    public static long GetDayOfYearTimestamp(DateTimeOffset dateTimeOffset, object? timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);
        var dateTimeOffsetInTimeZone = dateTimeOffset.ToOffset(timeZoneInfo.GetUtcOffset(dateTimeOffset));


        //zero based indexing
        return dateTimeOffsetInTimeZone.DayOfYear - 1;
    }

    public static long GetDayOfWeekTimestamp(DateTimeOffset dateTimeOffset, object? timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);
        var dateTimeOffsetInTimeZone = dateTimeOffset.ToOffset(timeZoneInfo.GetUtcOffset(dateTimeOffset));

        return (int)dateTimeOffsetInTimeZone.DayOfWeek;
    }

    public static long GetDayOfMonthTimestamp(DateTimeOffset dateTimeOffset, object? timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);
        var dateTimeOffsetInTimeZone = dateTimeOffset.ToOffset(timeZoneInfo.GetUtcOffset(dateTimeOffset));

        //zero based indexing
        return dateTimeOffsetInTimeZone.Day - 1;
    }

    public static TimeZoneInfo GetTimeZoneInfo(object? ianaTimeZoneArg)
    {
        if (!(ianaTimeZoneArg is string ianaTimeZoneArgString))
        {
            return TimeZoneInfo.Utc;
        }

        if (string.IsNullOrWhiteSpace(ianaTimeZoneArgString))
        {
            return TimeZoneInfo.Utc;
        }

        if (string.Equals("UTC", ianaTimeZoneArgString, StringComparison.Ordinal))
        {
            return TimeZoneInfo.Utc;
        }

        var ianaTimeZoneArgStringToParse = ianaTimeZoneArgString;
        if (ianaTimeZoneArgString.StartsWith("+", StringComparison.Ordinal) || ianaTimeZoneArgString.StartsWith("-", StringComparison.Ordinal))
        {
            ianaTimeZoneArgStringToParse = ianaTimeZoneArgString.Substring(1);
        }

        //we have an offset.
        if (DateTimeOffset.TryParseExact(ianaTimeZoneArgStringToParse, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var timeZoneOffsetDate))
        {
            var timeSpanOffset = timeZoneOffsetDate.TimeOfDay;
            if (ianaTimeZoneArgString.StartsWith("-", StringComparison.Ordinal))
            {
                timeSpanOffset = TimeSpan.Zero.Subtract(timeSpanOffset);
            }

            var timeZoneInfo = TimeZoneInfo.CreateCustomTimeZone(ianaTimeZoneArgString, timeSpanOffset, ianaTimeZoneArgString, ianaTimeZoneArgString);
            return timeZoneInfo;
        }

        try
        {
            //maybe throw an exception here?
            return TZConvert.GetTimeZoneInfo(ianaTimeZoneArgString);
        }
        catch (Exception x)
        {
            throw new CelNoSuchOverloadException($"Could not parse timezone '{ianaTimeZoneArg}'.", x);
        }
    }

    #endregion
}