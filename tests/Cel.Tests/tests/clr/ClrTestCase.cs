using Cel.Expr;

namespace Cel.Tests;

public class ClrTestCase
{
    public string Name { get; set; }
    public string Expr { get; set; }
    public object ExpectedResult { get; set; }
    public Dictionary<string, object> Variables { get; set; } = new();
    public string Exception { get; set; }

    public override string ToString()
    {
        return Name;
    }
}