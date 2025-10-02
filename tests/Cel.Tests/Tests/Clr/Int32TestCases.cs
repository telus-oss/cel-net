using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cel.Tests.Clr;

namespace Cel.Tests.Clr
{
    public class Int32TestCases
    {
        public static IEnumerable<ClrTestCase> TestCases()
        {
            // Existing test
            yield return new ClrTestCase
            {
                Name = "clr/int32/equality",
                Expr = "clrTestData.Int32Value == 10",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = true
            };

            // Additional comprehensive Int32 tests
            yield return new ClrTestCase
            {
                Name = "clr/int32/inequality",
                Expr = "clrTestData.Int32Value != 5",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = true
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/greater_than",
                Expr = "clrTestData.Int32Value > 5",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = true
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/less_than",
                Expr = "clrTestData.Int32Value < 15",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = true
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/greater_than_or_equal",
                Expr = "clrTestData.Int32Value >= 10",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = true
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/less_than_or_equal",
                Expr = "clrTestData.Int32Value <= 10",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = true
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/negative_comparison",
                Expr = "clrTestData.Int32Value < 0",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = -5 } } },
                ExpectedResult = true
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/zero_comparison",
                Expr = "clrTestData.Int32Value == 0",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 0 } } },
                ExpectedResult = true
            };
        }
    }
}
