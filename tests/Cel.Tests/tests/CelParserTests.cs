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

    [Test]
    [TestCase("`")]
    [TestCase("é")]
    [TestCase("`!@#$%%^^&*(_(_")]
    public void Invalid_Characters_In_Expression_Should_Throw_CelExpressionParserException(string expression)
    {
        var celEnvironment = new CelEnvironment(null, null);

        //test the parse function
        Assert.Throws<CelExpressionParserException>(() => celEnvironment.Parse(expression));

        //test the compile function
        Assert.Throws<CelExpressionParserException>(() => celEnvironment.Compile(expression));
    }


    [Test]
    [TestCase("True")]
    [TestCase("False")]
    [TestCase("TRUE")]
    [TestCase("FALSE")]
    [TestCase("real_variable == X")]
    [TestCase("x")]
    public void UndeclaredReference_In_Expression_Should_Throw_CelUndeclaredReferenceException(string expression)
    {
        var celEnvironment = new CelEnvironment(null, null);

        var variables = new Dictionary<string, object>();
        variables.Add("real_variable", 6);

        //test the program function
        Assert.Throws<CelUndeclaredReferenceException>(() => celEnvironment.Program(expression, variables));
    }

}
