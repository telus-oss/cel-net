using Google.Protobuf.Reflection;
using NUnit.Framework;

namespace Cel.Tests.tests;

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

    private static IEnumerable<(ClrTestData, string)> ClrTestCaseSource()
    {
        var clrTestData = new ClrTestData
        {
            StringValue = "jlksdjflkasnemrnw",
            BoolValue = true,
            DoubleValue = 90872983.234,
            IntValue = 79828730
        };

        yield return (clrTestData, $"clrTestData.StringValue == \"{clrTestData.StringValue}\"");
        yield return (clrTestData, $"clrTestData.BoolValue == {clrTestData.BoolValue.ToString().ToLowerInvariant()}");
        yield return (clrTestData, $"clrTestData.DoubleValue == {clrTestData.DoubleValue}");
        yield return (clrTestData, $"clrTestData.IntValue == {clrTestData.IntValue}");
    }

    [Test]
    [TestCaseSource(nameof(ClrTestCaseSource))]
    public void ClrTestData_String_Should_Return_String((ClrTestData, string) testData)
    {
        //build the dictionary
        var variables = new Dictionary<string, object>();
        variables.Add("clrTestData", testData.Item1);

        //compute the result
        var result = mSut.Program(testData.Item2, variables);

        //assert the result.
        Assert.That(result, Is.True);
    }
}