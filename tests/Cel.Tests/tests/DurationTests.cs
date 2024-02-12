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

using Cel.Helpers;
using NUnit.Framework;

namespace Cel.Tests;

[TestFixture]
public class DurationParsingTests
{
    [TestCaseSource(nameof(GetTestCases))]
    public void TestDurationParsing(string value, TimeSpan expectedValue)
    {
        var actualValue = DurationHelpers.ConvertDurationString(value).ToTimeSpan();
        Assert.That(actualValue, Is.EqualTo(expectedValue));
    }

    private static IEnumerable<object[]> GetTestCases()
    {
        yield return new object[] { "0", TimeSpan.Zero };
        yield return new object[] { "1h", TimeSpan.FromHours(1) };
        yield return new object[] { "1m", TimeSpan.FromMinutes(1) };
        yield return new object[] { "1s", TimeSpan.FromSeconds(1) };
        yield return new object[] { "1ms", TimeSpan.FromMilliseconds(1) };
        yield return new object[] { "1us", TimeSpan.FromTicks(1 * 10) };
        yield return new object[] { "100ns", TimeSpan.FromTicks(1) };

        yield return new object[] { "1h1m1s", new TimeSpan(1, 1, 1) };
        yield return new object[] { "1.5h", TimeSpan.FromMinutes(90) };
    }
}