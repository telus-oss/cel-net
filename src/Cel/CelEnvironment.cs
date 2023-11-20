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

using Antlr4.Runtime;
using Cel.Internal;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using static Cel.Internal.CelParser;
using Type = System.Type;

namespace Cel;

public delegate object? CelProgramDelegate(IDictionary<string, object?> variables);

public class CelEnvironment : ICelEnvironment
{
    private CelVisitor CelVisitor { get; }

    public CelEnvironment(IEnumerable<FileDescriptor>? fileDescriptors, string? messageNamespace)
    {
        var allFileDescriptors = GetWellKnownFileDescriptors().Union(fileDescriptors ?? Array.Empty<FileDescriptor>()).ToArray();
        CelVisitor = new CelVisitor(allFileDescriptors, messageNamespace ?? "");
    }

    public void RegisterFunction(string functionName, Type[] argTypes, CelFunctionDelegate functionDelegate)
    {
        if (string.IsNullOrWhiteSpace(functionName))
        {
            throw new ArgumentNullException(nameof(functionName));
        }

        if (argTypes == null)
        {
            throw new ArgumentNullException(nameof(argTypes));
        }

        if (functionDelegate == null)
        {
            throw new ArgumentNullException(nameof(functionDelegate));
        }

        CelVisitor.RegisterFunction(functionName, argTypes, functionDelegate);
    }

    public StartContext Parse(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentNullException(nameof(expression));
        }

        var inputStream = new AntlrInputStream(expression);
        var celLexer = new CelLexer(inputStream);
        var commonTokenStream = new CommonTokenStream(celLexer);
        var celParser = new CelParser(commonTokenStream);

        var startContext = celParser.start();

        return startContext;
    }

    public CelProgramDelegate Compile(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentNullException(nameof(expression));
        }

        var context = Parse(expression);
        return Compile(context);
    }

    public CelProgramDelegate Compile(StartContext context)
    {
        var expression = CelVisitor.Visit(context);

        return dict => expression.Invoke(dict.TryGetValue);
    }

    public object? Program(string expression, IDictionary<string, object?> variables)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentNullException(nameof(expression));
        }

        var context = Parse(expression);
        return Program(context, variables);
    }

    public object? Program(StartContext context, IDictionary<string, object?> variables)
    {
        var resultExpression = CelVisitor.Visit(context);

        var tryGetVariables = new TryGetVariableDelegate((string variableName, out object? value) => TryGetVariable(variables, variableName, out value));

        var result = resultExpression.Invoke(tryGetVariables);

        if (result is CelNoSuchField celNoSuchField)
        {
            throw new CelNoSuchFieldException(celNoSuchField.Message);
        }

        return result;
    }

    private static bool TryGetVariable(IDictionary<string, object?> variables, string variableName, out object? value)
    {
        value = null;

        if (variables.TryGetValue(variableName, out var internalVariableValue))
        {
            value = internalVariableValue;
            return true;
        }

        if (variables.TryGetValue(variableName, out var externalVariableValue))
        {
            value = externalVariableValue;
            return true;
        }

        return false;
    }

    public bool StrictTypeComparison
    {
        get => CelVisitor.StrictTypeComparison;
        set => CelVisitor.StrictTypeComparison = value;
    }

    private static FileDescriptor[] GetWellKnownFileDescriptors()
    {
        return new[]
        {
            AnyReflection.Descriptor,
            ApiReflection.Descriptor,
            DurationReflection.Descriptor,
            EmptyReflection.Descriptor,
            FieldMaskReflection.Descriptor,
            SourceContextReflection.Descriptor,
            StructReflection.Descriptor,
            TimestampReflection.Descriptor,
            TypeReflection.Descriptor,
            WrappersReflection.Descriptor
        };
    }
}