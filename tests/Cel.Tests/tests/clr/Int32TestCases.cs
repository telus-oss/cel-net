using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cel.Tests.tests.clr
{
    public class Int32TestCases
    {
        public static IEnumerable<ClrTestCase> TestCases()
        {
            yield return new ClrTestCase
            {
                Name = "clr/int32/equality",
                Expr = "clrTestData.Int32Value == 10",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = true
            };
        }
    }
}
