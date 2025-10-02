using System;
using Cel.Tests;
using Cel.Tests.Clr;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;

namespace Cel.Tests
{
    [TestFixture]
    public class TimestampConversionIntegrationTests
    {
        private CelEnvironment mSut;

        [SetUp]
        public void SetUp()
        {
            var fileDescriptors = new FileDescriptor[] { };
            var celEnvironment = new CelEnvironment(fileDescriptors, string.Empty);
            mSut = celEnvironment;
        }

        [Test]
        public void ConvertIntFromDateTime_UnixEpoch_ShouldReturnZero()
        {
            // Arrange
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var variables = new Dictionary<string, object>
            {
                { "epochTime", epoch }
            };

            // Act
            var result = mSut.Program("int(epochTime)", variables);

            // Assert
            Assert.That(result, Is.EqualTo(0L));
        }

        [Test]
        public void ConvertIntFromDateTimeOffset_Year2000_ShouldReturnCorrectUnixTimestamp()
        {
            // Arrange
            var year2000 = new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var expectedUnixSeconds = year2000.ToUnixTimeSeconds();
            var variables = new Dictionary<string, object>
            {
                { "year2000", year2000 }
            };

            // Act
            var result = mSut.Program("int(year2000)", variables);

            // Assert
            Assert.That(result, Is.EqualTo(expectedUnixSeconds));
        }

        [Test]
        public void ConvertIntFromTimestamp_SpecificDate_ShouldReturnCorrectUnixTimestamp()
        {
            // Arrange
            var specificDate = new DateTime(2023, 6, 15, 10, 30, 0, DateTimeKind.Utc);
            var timestamp = Timestamp.FromDateTime(specificDate);
            var expectedUnixSeconds = timestamp.ToDateTimeOffset().ToUnixTimeSeconds();
            var variables = new Dictionary<string, object>
            {
                { "timestamp", timestamp }
            };

            // Act
            var result = mSut.Program("int(timestamp)", variables);

            // Assert
            Assert.That(result, Is.EqualTo(expectedUnixSeconds));
        }

        [Test]
        public void TimestampArithmetic_ShouldWorkWithConvertedInts()
        {
            // Arrange - Two dates 24 hours apart
            var date1 = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var date2 = new DateTime(2023, 1, 2, 0, 0, 0, DateTimeKind.Utc);
            var expectedDifferenceInSeconds = 24 * 60 * 60; // 24 hours in seconds
            
            var variables = new Dictionary<string, object>
            {
                { "date1", date1 },
                { "date2", date2 }
            };

            // Act
            var result = mSut.Program("int(date2) - int(date1)", variables);

            // Assert
            Assert.That(result, Is.EqualTo((long)expectedDifferenceInSeconds));
        }

        [Test]
        public void TimestampComparison_ShouldWorkWithConvertedInts()
        {
            // Arrange
            var earlierDate = new DateTime(2023, 6, 1, 10, 0, 0, DateTimeKind.Utc);
            var laterDate = new DateTime(2023, 6, 1, 11, 0, 0, DateTimeKind.Utc);
            
            var variables = new Dictionary<string, object>
            {
                { "earlierDate", earlierDate },
                { "laterDate", laterDate }
            };

            // Act
            var result = mSut.Program("int(laterDate) > int(earlierDate)", variables);

            // Assert
            Assert.That(result, Is.EqualTo(true));
        }
    }
}