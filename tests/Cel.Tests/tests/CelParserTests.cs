using NUnit.Framework;

namespace Cel.Tests;

[TestFixture]
public class CelParserTests
{
    [Test]
    [TestCase("1+")]
    [TestCase("join('a', 'b'")]
    public void Incomplete_Expressions_Should_Throw_CelExpressionParserException(string expression)
    {
        var celEnvironment = new CelEnvironment(null, null);

        //test the parse function
        Assert.Throws<CelExpressionParserException>(() => celEnvironment.Parse(expression));

        //test the compile function
        Assert.Throws<CelExpressionParserException>(() => celEnvironment.Compile(expression));
    }
}