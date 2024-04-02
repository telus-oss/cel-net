using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cel.Tests.tests.clr
{
    public static class StringTestCases
    {
        public static IEnumerable<ClrTestCase> TestCases()
        {
            yield return new ClrTestCase
            {
                Name = "clr/string/equality",
                Expr = "clrTestData.StringValue == \"abc\"",
                Variables = { { "clrTestData", new ClrTestData { StringValue = "abc" } } },
                ExpectedResult = true
            };
        }
    }
}
