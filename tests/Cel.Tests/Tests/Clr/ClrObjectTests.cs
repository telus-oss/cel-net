using Cel.Expr;
using Cel.Tests;
using Cel.Tests.Clr;
using Google.Protobuf.Reflection;
using NUnit.Framework;

namespace Cel.Tests.Clr;

[TestFixture]
public class ClrObjectTests
{
    [SetUp]
    public void SetUp()
    {
        var fileDescriptors = new FileDescriptor[] { };

        var celEnvironment = new CelEnvironment(fileDescriptors, string.Empty);

        mSut = celEnvironment;
    }

    private CelEnvironment mSut;


    [Test]
    [TestCaseSource(nameof(AllTestCases))]
    public void ClrTest(ClrTestCase clrTestCase)
    {
        if (clrTestCase == null) throw new ArgumentNullException(nameof(clrTestCase));
        try
        {
            //build the dictionary
            var variables = clrTestCase.Variables ?? new Dictionary<string, object>();

            //compute the result
            var result = mSut.Program(clrTestCase.Expr, variables);

            if (!string.IsNullOrEmpty(clrTestCase.Exception))
            {
                Assert.Fail("Exception should have been thrown.");
                return;
            }

            //assert the result.
            Assert.That(result, Is.EqualTo(clrTestCase.ExpectedResult));
        }
        catch (CelException x)
        {
            if (string.Equals(x.GetType().Name, clrTestCase.Exception, StringComparison.OrdinalIgnoreCase))
            {
                //we're good
            }
            else
            {
                throw;
            }
        }
    }

    private static IEnumerable<ClrTestCase> AllTestCases()
    {
        return StringTestCases.TestCases()
            .Union(Int32TestCases.TestCases())
            .Union(DecimalTestCases.TestCases())
            .Union(SingleTestCases.TestCases())
            .Union(SignedIntegerArithmeticTestCases.TestCases())
            .Union(UnsignedIntegerArithmeticTestCases.TestCases())
            .Union(IntegerEdgeCaseTestCases.TestCases())
            .Union(TimestampConversionTestCases.TestCases())
            .ToList();
    }

}