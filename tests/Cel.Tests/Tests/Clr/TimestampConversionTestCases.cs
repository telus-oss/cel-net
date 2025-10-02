using System;
using System.Collections.Generic;
using Cel.Tests.Clr;
using Google.Protobuf.WellKnownTypes;

namespace Cel.Tests.Clr
{
    public static class TimestampConversionTestCases
    {
        public static IEnumerable<ClrTestCase> TestCases()
        {
            return TestCases_ConvertIntFromDateTime()
                    .Union(TestCases_ConvertIntFromDateTimeOffset())
                    .Union(TestCases_ConvertIntFromTimestamp())
                    .Union(TestCases_ConvertIntFromTimestampEdgeCases());
        }

        public static IEnumerable<ClrTestCase> TestCases_ConvertIntFromDateTime()
        {
            // Unix epoch (January 1, 1970)
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetime_epoch",
                Expr = "int(clrTestData.DateTimeValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeValue = epoch } } },
                ExpectedResult = 0L
            };

            // January 1, 2000, 00:00:00 UTC
            var year2000 = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var year2000UnixSeconds = ((DateTimeOffset)year2000).ToUnixTimeSeconds();
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetime_year2000",
                Expr = "int(clrTestData.DateTimeValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeValue = year2000 } } },
                ExpectedResult = year2000UnixSeconds
            };

            // January 1, 2023, 12:30:45 UTC
            var specificDate = new DateTime(2023, 1, 1, 12, 30, 45, DateTimeKind.Utc);
            var specificDateUnixSeconds = ((DateTimeOffset)specificDate).ToUnixTimeSeconds();
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetime_specific",
                Expr = "int(clrTestData.DateTimeValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeValue = specificDate } } },
                ExpectedResult = specificDateUnixSeconds
            };

            // Test with local time (should be converted to UTC for Unix timestamp)
            var localTime = new DateTime(2023, 6, 15, 14, 30, 0, DateTimeKind.Local);
            var localTimeUnixSeconds = new DateTimeOffset(localTime).ToUnixTimeSeconds();
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetime_local",
                Expr = "int(clrTestData.DateTimeValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeValue = localTime } } },
                ExpectedResult = localTimeUnixSeconds
            };

            // Test with unspecified kind (treated as local)
            var unspecifiedTime = new DateTime(2023, 12, 25, 9, 15, 30, DateTimeKind.Unspecified);
            var unspecifiedTimeUnixSeconds = new DateTimeOffset(unspecifiedTime).ToUnixTimeSeconds();
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetime_unspecified",
                Expr = "int(clrTestData.DateTimeValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeValue = unspecifiedTime } } },
                ExpectedResult = unspecifiedTimeUnixSeconds
            };
        }

        public static IEnumerable<ClrTestCase> TestCases_ConvertIntFromDateTimeOffset()
        {
            // Unix epoch
            var epochOffset = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetimeoffset_epoch",
                Expr = "int(clrTestData.DateTimeOffsetValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeOffsetValue = epochOffset } } },
                ExpectedResult = 0L
            };

            // New Year 2000 UTC
            var year2000Offset = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetimeoffset_year2000",
                Expr = "int(clrTestData.DateTimeOffsetValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeOffsetValue = year2000Offset } } },
                ExpectedResult = year2000Offset.ToUnixTimeSeconds()
            };

            // Specific date with positive timezone offset (UTC+5)
            var dateWithPosOffset = new DateTimeOffset(2023, 7, 4, 10, 30, 0, TimeSpan.FromHours(5));
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetimeoffset_positive_offset",
                Expr = "int(clrTestData.DateTimeOffsetValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeOffsetValue = dateWithPosOffset } } },
                ExpectedResult = dateWithPosOffset.ToUnixTimeSeconds()
            };

            // Specific date with negative timezone offset (UTC-8)
            var dateWithNegOffset = new DateTimeOffset(2023, 11, 15, 16, 45, 30, TimeSpan.FromHours(-8));
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetimeoffset_negative_offset",
                Expr = "int(clrTestData.DateTimeOffsetValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeOffsetValue = dateWithNegOffset } } },
                ExpectedResult = dateWithNegOffset.ToUnixTimeSeconds()
            };

            // Test with UTC offset
            var utcDate = new DateTimeOffset(2023, 3, 15, 8, 20, 15, TimeSpan.Zero);
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetimeoffset_utc",
                Expr = "int(clrTestData.DateTimeOffsetValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeOffsetValue = utcDate } } },
                ExpectedResult = utcDate.ToUnixTimeSeconds()
            };
        }

        public static IEnumerable<ClrTestCase> TestCases_ConvertIntFromTimestamp()
        {
            // Unix epoch as Timestamp
            var epochTimestamp = Timestamp.FromDateTime(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_timestamp_epoch",
                Expr = "int(clrTestData.TimestampValue)",
                Variables = { { "clrTestData", new ClrTestData { TimestampValue = epochTimestamp } } },
                ExpectedResult = 0L
            };

            // Year 2000 as Timestamp
            var year2000Timestamp = Timestamp.FromDateTime(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_timestamp_year2000",
                Expr = "int(clrTestData.TimestampValue)",
                Variables = { { "clrTestData", new ClrTestData { TimestampValue = year2000Timestamp } } },
                ExpectedResult = year2000Timestamp.ToDateTimeOffset().ToUnixTimeSeconds()
            };

            // Specific date as Timestamp
            var specificTimestamp = Timestamp.FromDateTime(new DateTime(2023, 5, 20, 14, 25, 50, DateTimeKind.Utc));
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_timestamp_specific",
                Expr = "int(clrTestData.TimestampValue)",
                Variables = { { "clrTestData", new ClrTestData { TimestampValue = specificTimestamp } } },
                ExpectedResult = specificTimestamp.ToDateTimeOffset().ToUnixTimeSeconds()
            };

            // Test with DateTimeOffset conversion to Timestamp
            var dateTimeOffset = new DateTimeOffset(2023, 9, 10, 11, 40, 25, TimeSpan.FromHours(3));
            var timestampFromOffset = Timestamp.FromDateTimeOffset(dateTimeOffset);
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_timestamp_from_offset",
                Expr = "int(clrTestData.TimestampValue)",
                Variables = { { "clrTestData", new ClrTestData { TimestampValue = timestampFromOffset } } },
                ExpectedResult = timestampFromOffset.ToDateTimeOffset().ToUnixTimeSeconds()
            };

            // Test with current timestamp (relative to a fixed point for testing)
            var fixedTimestamp = Timestamp.FromDateTime(new DateTime(2023, 12, 1, 10, 0, 0, DateTimeKind.Utc));
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_timestamp_fixed",
                Expr = "int(clrTestData.TimestampValue)",
                Variables = { { "clrTestData", new ClrTestData { TimestampValue = fixedTimestamp } } },
                ExpectedResult = fixedTimestamp.ToDateTimeOffset().ToUnixTimeSeconds()
            };
        }

        public static IEnumerable<ClrTestCase> TestCases_ConvertIntFromTimestampEdgeCases()
        {
            // Test with DateTime minimum value (should handle gracefully)
            var minDateTime = DateTime.MinValue;
            var minDateTimeUnixSeconds = new DateTimeOffset(minDateTime).ToUnixTimeSeconds();
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetime_min",
                Expr = "int(clrTestData.DateTimeValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeValue = minDateTime } } },
                ExpectedResult = minDateTimeUnixSeconds
            };

            // Test with DateTime maximum value (should handle gracefully)
            var maxDateTime = new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc);
            var maxDateTimeUnixSeconds = new DateTimeOffset(maxDateTime).ToUnixTimeSeconds();
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetime_max",
                Expr = "int(clrTestData.DateTimeValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeValue = maxDateTime } } },
                ExpectedResult = maxDateTimeUnixSeconds
            };

            // Test with DateTimeOffset minimum value
            var minDateTimeOffset = DateTimeOffset.MinValue;
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetimeoffset_min",
                Expr = "int(clrTestData.DateTimeOffsetValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeOffsetValue = minDateTimeOffset } } },
                ExpectedResult = minDateTimeOffset.ToUnixTimeSeconds()
            };

            // Test with DateTimeOffset maximum value
            var maxDateTimeOffset = DateTimeOffset.MaxValue;
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetimeoffset_max",
                Expr = "int(clrTestData.DateTimeOffsetValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeOffsetValue = maxDateTimeOffset } } },
                ExpectedResult = maxDateTimeOffset.ToUnixTimeSeconds()
            };

            // Test with past date (before 1970)
            var pastDate = new DateTime(1950, 6, 15, 12, 30, 0, DateTimeKind.Utc);
            var pastDateUnixSeconds = ((DateTimeOffset)pastDate).ToUnixTimeSeconds();
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetime_before_epoch",
                Expr = "int(clrTestData.DateTimeValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeValue = pastDate } } },
                ExpectedResult = pastDateUnixSeconds
            };

            // Test with future date (far in the future)
            var futureDate = new DateTime(2050, 12, 31, 23, 59, 59, DateTimeKind.Utc);
            var futureDateUnixSeconds = ((DateTimeOffset)futureDate).ToUnixTimeSeconds();
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_from_datetime_far_future",
                Expr = "int(clrTestData.DateTimeValue)",
                Variables = { { "clrTestData", new ClrTestData { DateTimeValue = futureDate } } },
                ExpectedResult = futureDateUnixSeconds
            };

            // Test arithmetic with converted timestamps
            var date1 = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var date2 = new DateTime(2023, 1, 2, 0, 0, 0, DateTimeKind.Utc); // 24 hours later
            var expectedDifference = 24 * 60 * 60; // 24 hours in seconds
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_datetime_arithmetic",
                Expr = "int(date2) - int(date1)",
                Variables = { 
                    { "date1", date1 }, 
                    { "date2", date2 } 
                },
                ExpectedResult = (long)expectedDifference
            };

            // Test comparison of converted timestamps
            var earlierDate = new DateTime(2023, 6, 1, 10, 0, 0, DateTimeKind.Utc);
            var laterDate = new DateTime(2023, 6, 1, 11, 0, 0, DateTimeKind.Utc);
            
            yield return new ClrTestCase
            {
                Name = "clr/timestamp/convert_int_datetime_comparison",
                Expr = "int(laterDate) > int(earlierDate)",
                Variables = { 
                    { "earlierDate", earlierDate }, 
                    { "laterDate", laterDate } 
                },
                ExpectedResult = true
            };
        }
    }
}