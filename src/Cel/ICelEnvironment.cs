using Cel.Internal;

namespace Cel;

public interface ICelEnvironment
{
    public bool StrictTypeComparison { get; set; }
    void RegisterFunction(string functionName, Type[] argTypes, CelFunctionDelegate functionDelegate);
    CelParser.StartContext Parse(string expression);
    CelProgramDelegate Compile(string expression);
    CelProgramDelegate Compile(CelParser.StartContext context);
    object? Program(string expression, IDictionary<string, object?> variables);
    object? Program(CelParser.StartContext context, IDictionary<string, object?> variables);
}