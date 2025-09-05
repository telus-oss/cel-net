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

using System.Collections;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Cel.Helpers;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Type = System.Type;

namespace Cel.Internal;

public delegate object? CelFunctionDelegate(object?[] args);

public delegate object? CelMacroDelegate(object? arg, string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate);

public delegate bool TryGetFunctionWithArgValuesDelegate(string functionName, object?[] args, out CelFunctionDelegate? value);

public delegate bool TryGetVariableDelegate(string variableName, out object? value);

public delegate object? CelExpressionDelegate(TryGetVariableDelegate tryGetVariableDelegate);

internal class CelVisitor : CelBaseVisitor<CelExpressionDelegate>
{
    #region Declarations

    private static Dictionary<string, CelType?> CelAbstractTypes { get; } = new();

    private IList<FileDescriptor> FileDescriptors { get; }
    private Dictionary<string, CelMacroDelegate> InternalMacros { get; } = new();
    private ConcurrentDictionary<string, List<FunctionRegistration>> Functions { get; } = new();

    private TypeRegistry TypeRegistry { get; }
    private string? MessageNamespace { get; }
    public bool StrictTypeComparison { get; set; }

    #endregion

    #region Constructor

    static CelVisitor()
    {
        InitializeCelAbstractTypes();
    }

    public CelVisitor(IList<FileDescriptor> fileDescriptors, string? messageNamespace)
    {
        FileDescriptors = fileDescriptors;
        MessageNamespace = messageNamespace;
        TypeRegistry = TypeRegistry.FromFiles(fileDescriptors);

        CelFunctions.InitializeFunctions(Functions);
        CelMacros.InitializeMacros(InternalMacros);
    }

    #endregion

    #region Functions and Types

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

        Functions.RegisterFunction(functionName, argTypes, functionDelegate);
    }

    private static void InitializeCelAbstractTypes()
    {
        CelAbstractTypes.Add("null_type", new CelType("null_type"));
        CelAbstractTypes.Add("type", new CelType("type"));
        CelAbstractTypes.Add("bool", new CelType("bool"));
        CelAbstractTypes.Add("bytes", new CelType("bytes"));
        CelAbstractTypes.Add("int", new CelType("int"));
        CelAbstractTypes.Add("uint", new CelType("uint"));
        CelAbstractTypes.Add("double", new CelType("double"));
        CelAbstractTypes.Add("string", new CelType("string"));
        CelAbstractTypes.Add("list", new CelType("list"));
        CelAbstractTypes.Add("map", new CelType("map"));
        CelAbstractTypes.Add("dyn", null);
        CelAbstractTypes.Add("google.protobuf.Timestamp", new CelType("google.protobuf.Timestamp"));
        CelAbstractTypes.Add("google.protobuf.Duration", new CelType("google.protobuf.Duration"));
    }

    #endregion

    #region Visitors

    public override CelExpressionDelegate Visit(IParseTree tree)
    {
        WriteDebugLine($"Visiting node: {tree.GetType().Name}   {tree.GetText()}");

        var result = base.Visit(tree);

        WriteDebugLine($"Visited node: {tree.GetType().Name}   {tree.GetText()}");

        return result;
    }

    public override CelExpressionDelegate VisitChildren(IRuleNode node)
    {
        WriteDebugLine($"Visiting children node: {node.GetType().Name}   {node.GetText()}");

        var result = base.VisitChildren(node);

        WriteDebugLine($"Visited children node: {node.GetType().Name}   {node.GetText()}");

        return result;
    }

    public override CelExpressionDelegate VisitBoolFalse([NotNull] CelParser.BoolFalseContext context)
    {
        return tryGetVariable => false;
    }

    public override CelExpressionDelegate VisitBoolTrue([NotNull] CelParser.BoolTrueContext context)
    {
        return tryGetVariable => true;
    }

    public override CelExpressionDelegate VisitBytes([NotNull] CelParser.BytesContext context)
    {
        var value = context.tok.Text;

        if (value.Length <= 1)
        {
            throw new CelExpressionParserException("Could not parse bytes value.", context);
        }

        if (value[0] != 'b')
        {
            throw new CelExpressionParserException("Could not parse bytes value.", context);
        }


        object unescapedString;

        try
        {
            unescapedString = StringHelpers.Unescape(value.Substring(1), true);
        }
        catch (CelExpressionParserException x)
        {
            x.Node = context;
            throw;
        }

        if (unescapedString is not ByteString)
        {
            throw new CelExpressionParserException("Could not parse bytes value.", context);
        }

        return tryGetVariable => unescapedString;
    }

    public override CelExpressionDelegate VisitCalc([NotNull] CelParser.CalcContext context)
    {
        return tryGetVariable =>
        {
            if (context.op == null)
            {
                return base.VisitCalc(context).Invoke(tryGetVariable);
            }

            var left = context.calc(0);
            var right = context.calc(1);

            var leftResult = Visit(left).Invoke(tryGetVariable);
            var rightResult = Visit(right).Invoke(tryGetVariable);

            //check that we have fields.
            if (leftResult is CelNoSuchField celNoSuchFieldLeft)
            {
                throw new CelNoSuchFieldException(celNoSuchFieldLeft.Message);
            }

            if (rightResult is CelNoSuchField celNoSuchFieldRight)
            {
                throw new CelNoSuchFieldException(celNoSuchFieldRight.Message);
            }

            if (context.op != null)
            {
                if (context.op.Text == "+")
                {
                    return ArithmeticFunctions.Add(leftResult, rightResult);
                }

                if (context.op.Text == "-")
                {
                    return ArithmeticFunctions.Subtract(leftResult, rightResult);
                }

                if (context.op.Text == "*")
                {
                    return ArithmeticFunctions.Multiply(leftResult, rightResult);
                }

                if (context.op.Text == "/")
                {
                    return ArithmeticFunctions.Divide(leftResult, rightResult);
                }

                if (context.op.Text == "%")
                {
                    return ArithmeticFunctions.Modulus(leftResult, rightResult);
                }
            }

            throw new CelExpressionParserException("Could not parse op value.", context);
        };
    }

    public override CelExpressionDelegate VisitConditionalOr([NotNull] CelParser.ConditionalOrContext context)
    {
        if (context._e1 == null || context._e1.Count == 0)
        {
            var result = base.VisitConditionalOr(context);
            return result;
        }

        return tryGetVariable =>
        {
            var conditions = new List<CelParser.ConditionalAndContext>();
            //add the initial condition
            conditions.Add(context.e);
            //add the additional expressions.
            if (context._e1 != null)
            {
                conditions.AddRange(context._e1);
            }

            Exception? exception = null;
            var noSuchOverload = false;
            for (var i = 0; i < conditions.Count; i++)
            {
                try
                {
                    var conditionValue = Visit(conditions[i]).Invoke(tryGetVariable);
                    if (conditionValue is CelNoSuchField celNoSuchField)
                    {
                        throw new CelNoSuchFieldException(celNoSuchField.Message);
                    }

                    if (conditionValue is bool conditionValueBool)
                    {
                        if (conditionValueBool)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        noSuchOverload = true;
                    }
                }
                catch (CelNoSuchFieldException)
                {
                    throw;
                }
                catch (Exception x)
                {
                    if (StrictTypeComparison)
                    {
                        throw;
                    }

                    exception = x;
                }
            }

            if (exception != null)
            {
                throw new CelNoSuchOverloadException("Could not compare OR values.", exception);
            }

            if (noSuchOverload)
            {
                return null;
            }

            return false;
        };
    }

    public override CelExpressionDelegate VisitConditionalAnd([NotNull] CelParser.ConditionalAndContext context)
    {
        if (context._e1 == null || context._e1.Count == 0)
        {
            var result = base.VisitConditionalAnd(context);
            return result;
        }

        return tryGetVariable =>
        {
            //Logical Operators
            //In the conditional operator e ? e1 : e2, evaluates to e1 if e evaluates to true,
            //and e2 if e evaluates to false. The untaken branch is presumed to not be executed,
            //though that is an implementation detail.

            //In the boolean operators && and ||: if any of their operands uniquely determines
            //the result (false for && and true for ||) the other operand may or may not be evaluated,
            //and if that evaluation produces a runtime error, it will be ignored.
            //This makes those operators commutative (in contrast to traditional boolean
            //short-circuit operators).
            //The rationale for this behavior is to allow the boolean operators to be mapped
            //to indexed queries, and align better with SQL semantics.

            //To get traditional left-to-right short-circuiting evaluation of logical operators,
            //as in C or other languages (also called "McCarthy Evaluation"),
            //the expression e1 && e2 can be rewritten e1 ? e2 : false.
            //Similarly, e1 || e2 can be rewritten e1 ? true : e2.

            var conditions = new List<CelParser.RelationContext>();
            //add the initial condition
            conditions.Add(context.e);
            //add the additional expressions.
            if (context._e1 != null)
            {
                conditions.AddRange(context._e1);
            }

            Exception? exception = null;
            var noSuchOverload = false;

            for (var i = 0; i < conditions.Count; i++)
            {
                try
                {
                    var conditionValue = Visit(conditions[i]).Invoke(tryGetVariable);

                    //check that we have fields.
                    if (conditionValue is CelNoSuchField celNoSuchField)
                    {
                        throw new CelNoSuchFieldException(celNoSuchField.Message);
                    }

                    if (conditionValue is bool conditionValueBool)
                    {
                        if (!conditionValueBool)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        noSuchOverload = true;
                    }
                }
                catch (CelNoSuchFieldException)
                {
                    throw;
                }
                catch (Exception x)
                {
                    if (StrictTypeComparison)
                    {
                        throw;
                    }

                    exception = x;
                }
            }

            if (exception != null)
            {
                throw new CelNoSuchOverloadException("Could not compare AND values.", exception);
            }

            if (noSuchOverload)
            {
                return null;
            }

            return true;
        };
    }


    public override CelExpressionDelegate VisitConstantLiteral([NotNull] CelParser.ConstantLiteralContext context)
    {
        var result = base.VisitConstantLiteral(context);
        return result;
    }

    public override CelExpressionDelegate VisitCreateList([NotNull] CelParser.CreateListContext context)
    {
        if (context.elems == null)
        {
            return tryGetVariable => Array.Empty<object?>();
        }

        return tryGetVariable =>
        {
            var list = new object?[context.elems._elems.Count];

            for (var i = 0; i < context.elems._elems.Count; i++)
            {
                list[i] = Visit(context.elems._elems[i]).Invoke(tryGetVariable);
            }

            return list;
        };
    }

    public override CelExpressionDelegate VisitCreateMessage([NotNull] CelParser.CreateMessageContext context)
    {
        return tryGetVariable =>
        {
            var identifier = (context.leadingDot?.Text ?? "") + string.Join(".", context._ids.Select(c => c.Text));

            Dictionary<string, object?>? entries = null;

            //load the child entries
            if (context.entries != null)
            {
                entries = Visit(context.entries).Invoke(tryGetVariable) as Dictionary<string, object?>;
            }

            //handle special type for ListValue
            if (identifier == "google.protobuf.ListValue")
            {
                var list = new List<object?>();
                if (entries != null)
                {
                    if (entries.TryGetValue("values", out var valuesDictEntry))
                    {
                        if (valuesDictEntry is object?[] valuesList)
                        {
                            list.AddRange(valuesList);
                        }
                        else if (valuesDictEntry is IEnumerable<object?> valueEnumerableObject)
                        {
                            list.AddRange(valueEnumerableObject);
                        }
                        else if (valuesDictEntry is IEnumerable valueIEnumerable)
                        {
                            list.AddRange(valueIEnumerable.Cast<object?>());
                        }
                    }
                }

                return list.ToArray();
            }

            if (identifier == "google.protobuf.Struct")
            {
                if (entries != null)
                {
                    if (entries.TryGetValue("fields", out var valuesDictEntry))
                    {
                        if (valuesDictEntry is IDictionary<string, object?>)
                        {
                            return valuesDictEntry;
                        }
                    }
                }

                return new Dictionary<string, object?>();
            }

            var messageDescriptor = GetMessageDescriptor(identifier);

            if (messageDescriptor == null)
            {
                throw new CelExpressionParserException($"Cannot find constructor for identifier '{identifier}'.");
            }

            var message = (IMessage?)Activator.CreateInstance(messageDescriptor.ClrType);
            if (message == null)
            {
                throw new CelExpressionParserException($"Could not instantiate type '{messageDescriptor.ClrType.FullName}'.");
            }

            if (entries != null)
            {
                MessageHelpers.SetMessageValues(message, entries);
            }

            if (message is Any anyMessage)
            {
                //validate the Any message
                if (string.IsNullOrWhiteSpace(anyMessage.TypeUrl))
                {
                    throw new CelTypeCreationException("Could not create 'Any' message.");
                }
            }

            if (message is Value valueMessage)
            {
                //unwrap the value standard type
                return valueMessage.Unwrap();
            }

            var result = MessageHelpers.UnwrapWellKnownMessage(message);

            return result;
        };
    }

    public override CelExpressionDelegate VisitCreateStruct([NotNull] CelParser.CreateStructContext context)
    {
        if (context.entries == null)
        {
            return tryGetVariable => new Dictionary<string, object?>();
        }

        var entries = Visit(context.entries);
        return entries;
    }

    public override CelExpressionDelegate VisitDouble([NotNull] CelParser.DoubleContext context)
    {
        return tryGetVariable =>
        {
            var token = context.tok.Text;
            if (context.sign?.Text == "-")
            {
                token = "-" + token;
            }

            return DoubleHelpers.ConvertDouble(token);
        };
    }

    public override CelExpressionDelegate VisitErrorNode(IErrorNode node)
    {
        return base.VisitErrorNode(node);
    }

    public override CelExpressionDelegate VisitExpr([NotNull] CelParser.ExprContext context)
    {
        if (context.op?.Text == "?")
        {
            return tryGetVariable =>
            {
                var conditionResult = Visit(context.e).Invoke(tryGetVariable);

                if (conditionResult is bool boolExpressionResult)
                {
                    if (boolExpressionResult)
                    {
                        return Visit(context.e1).Invoke(tryGetVariable);
                    }

                    return Visit(context.e2).Invoke(tryGetVariable);
                }

                throw new CelNoSuchOverloadException($"Expression returned type '{conditionResult?.GetType().FullName ?? "null"} but boolean was expected.");
            };
        }

        var result = base.VisitExpr(context);
        return result;
    }

    public override CelExpressionDelegate VisitExprList([NotNull] CelParser.ExprListContext context)
    {
        if (context._e == null)
        {
            return base.VisitExprList(context);
        }

        return tryGetVariable =>
        {
            var list = new object?[context._e.Count];

            for (var i = 0; i < context._e.Count; i++)
            {
                list[i] = Visit(context._e[i]).Invoke(tryGetVariable);
            }

            return list.ToArray();
        };
    }

    public override CelExpressionDelegate VisitFieldInitializerList([NotNull] CelParser.FieldInitializerListContext context)
    {
        if (context._fields == null || context._values == null)
        {
            return base.VisitFieldInitializerList(context);
        }

        if (context._fields.Count != context._values.Count)
        {
            throw new CelExpressionParserException("Field initializer list has different number of keys and values.");
        }

        return tryGetVariable =>
        {
            var dictionary = new Dictionary<string, object?>();

            for (var i = 0; i < context._fields.Count; i++)
            {
                var key = Visit(context._fields[i]).Invoke(tryGetVariable);
                string keyString;

                if (key is string keyAsString)
                {
                    keyString = keyAsString;
                }
                else if (key is ulong keyUInt)
                {
                    keyString = StringHelpers.ConvertStringUInt(keyUInt);
                }
                else if (key is long keyInt)
                {
                    keyString = StringHelpers.ConvertStringInt(keyInt);
                }
                else if (key is bool keyBool)
                {
                    keyString = StringHelpers.ConvertStringBool(keyBool);
                }
                else
                {
                    throw new CelMapUnsupportedKeyTypeException("Cannot map item because key is not the required type.");
                }

                var value = Visit(context._values[i]).Invoke(tryGetVariable);

                if (dictionary.ContainsKey(keyString))
                {
                    throw new CelMapDuplicateKeyException($"Duplicate map key '{keyString}'.");
                }

                dictionary.Add(keyString, value);
            }

            return dictionary;
        };
    }

    public override CelExpressionDelegate VisitIdentOrGlobalCall([NotNull] CelParser.IdentOrGlobalCallContext context)
    {
        return tryGetVariable =>
        {
            var leadingDot = context.leadingDot;
            var id = context.id;
            var identifier = id.Text;
            var variableName = leadingDot?.Text ?? "" + id.Text;

            if (!string.IsNullOrWhiteSpace(identifier))
            {
                object? args = null;
                var exprList = context.exprList();

                if (exprList != null)
                {
                    //we have a function with arguments.
                    args = Visit(exprList).Invoke(tryGetVariable);
                    if (args is not object?[])
                    {
                        //wrap the args into an array if they aren't an array already.
                        args = new[] { args };
                    }
                }

                if (exprList == null)
                {
                    //we have a variable;
                    if (TryGetVariableWithNamespace(tryGetVariable, MessageNamespace, variableName, out var variableValue))
                    {
                        return variableValue;
                    }
                    if (CelAbstractTypes.TryGetValue(id.Text, out var internalVariableValue))
                    {
                        return internalVariableValue;
                    }
                }

                var enumDescriptor = GetEnumDescriptor(identifier);
                if (enumDescriptor != null)
                {
                    if (args is object[] argsArray && argsArray.Length == 1)
                    {
                        var number = Int64Helpers.ConvertInt(argsArray[0]);
                        var numberInt = MessageHelpers.ConvertToInt32(number);
                        var enumValueDescriptor = enumDescriptor.FindValueByNumber(numberInt);
                        if (enumValueDescriptor == null)
                        {
                            return number;
                        }

                        return enumValueDescriptor;
                    }
                }

                var nonNullArgs = (object?[])(args ?? Array.Empty<object?>());
                if (Functions.TryGetFunctionWithArgValues(identifier, nonNullArgs, out var internalFunction))
                {
                    return internalFunction!.Invoke(nonNullArgs);
                }

                if (context.Parent is CelParser.PrimaryExprContext && context.Parent?.Parent is CelParser.MemberCallContext)
                {
                    //this is to pass test SimpleTest(enums, strong_proto2, assign_standalone_int)
                    //with expression "TestAllTypes{standalone_enum: TestAllTypes.NestedEnum(1)}"
                    return base.VisitIdentOrGlobalCall(context);
                }


                if (exprList != null)
                {
                    throw new CelUnboundFunctionException($"Unbound function '{variableName}'.");
                }

                throw new CelUndeclaredReferenceException($"Undeclared reference to '{variableName}' in container '{MessageNamespace}'.");
            }

            throw new CelExpressionParserException("Identifier not specified.");
        };
    }

    public override CelExpressionDelegate VisitIndex([NotNull] CelParser.IndexContext context)
    {
        return tryGetVariable =>
        {
            var member = Visit(context.member()).Invoke(tryGetVariable);
            var index = Visit(context.expr()).Invoke(tryGetVariable);

            if (member is object?[] memberArray)
            {
                if (index is long indexAsLong) { }
                else if (index is ulong indexAsUInt)
                {
                    indexAsLong = Int64Helpers.ConvertInt(indexAsUInt);
                }
                else if (index is double indexAsDouble)
                {
                    if (indexAsDouble % 1 == 0)
                    {
                        //the double has no fraction
                        indexAsLong = Int64Helpers.ConvertIntDouble(indexAsDouble);
                    }
                    else
                    {
                        //the double has a fraction.
                        throw new CelArgumentRangeException("Index argument error.");
                    }
                }
                else
                {
                    throw new CelArgumentRangeException("Index argument error.");
                }
                if (indexAsLong < 0 || indexAsLong >= memberArray.Length)
                {
                    throw new CelArgumentRangeException("Index out of range.");
                }

                var result = memberArray[indexAsLong];
                return result;
            }

            if (member is IList memberIList)
            {
                if (index is long indexAsLong) { }
                else if (index is ulong indexAsUInt)
                {
                    indexAsLong = Int64Helpers.ConvertInt(indexAsUInt);
                }
                else if (index is double indexAsDouble)
                {
                    if (indexAsDouble % 1 == 0)
                    {
                        //the double has no fraction
                        indexAsLong = Int64Helpers.ConvertIntDouble(indexAsDouble);
                    }
                    else
                    {
                        //the double has a fraction.
                        throw new CelArgumentRangeException("Index argument error.");
                    }
                }
                else
                {
                    throw new CelArgumentRangeException("Index argument error.");
                }
                if (indexAsLong < 0 || indexAsLong >= memberIList.Count)
                {
                    throw new CelArgumentRangeException("Index out of range.");
                }

                var result = memberIList[(int)indexAsLong];
                return result;
            }

            if (member is Dictionary<string, object?> memberDict)
            {
                var indexAsString = StringHelpers.ConvertString(index);
                if (!string.IsNullOrEmpty(indexAsString) && memberDict.TryGetValue(indexAsString!, out var value))
                {
                    return value;
                }

                throw new CelArgumentRangeException("Key out of range.");
            }
            
            if (member is IDictionary memberIDictionary)
            {
                // Handle Google.Protobuf.Collections.MapField<TKey, TValue> with type conversion
                if (member != null && member.GetType().IsGenericType && 
                    member.GetType().GetGenericTypeDefinition().FullName == "Google.Protobuf.Collections.MapField`2")
                {
                    var genericArgs = member.GetType().GetGenericArguments();
                    var keyType = genericArgs[0];
                    
                    // Try to convert the index to the expected key type
                    object? convertedKey = TryConvertKeyForMapField(index, keyType);
                    if (convertedKey != null && memberIDictionary.Contains(convertedKey))
                    {
                        return memberIDictionary[convertedKey];
                    }
                }
                
                // For other IDictionary implementations, first try the index as-is
                if (index != null && memberIDictionary.Contains(index))
                {
                    return memberIDictionary[index];
                }

                // If the original index doesn't work, try converting to string for string-keyed dictionaries
                var indexAsString = StringHelpers.ConvertString(index);
                if (!string.IsNullOrEmpty(indexAsString) && memberIDictionary.Contains(indexAsString))
                {
                    return memberIDictionary[indexAsString];
                }

                throw new CelArgumentRangeException("Key out of range.");
            }
           
            if (member is IMessage memberMessage && index is string indexString)
            {
                var fieldDescriptor = memberMessage.Descriptor.FindFieldByName(indexString);
                if (fieldDescriptor == null)
                {
                    throw new CelArgumentRangeException("Key out of range.");
                }

                return fieldDescriptor.Accessor.GetValue(memberMessage);
            }

            if (member == null)
            {
                //return null;
                throw new CelExpressionParserException("List or map cannot be null.");
            }

            return base.VisitIndex(context);
        };
    }
    
    /// <summary>
    /// Attempts to convert the index to the expected key type for MapField access.
    /// Handles common type conversions between CEL types and Protocol Buffer key types.
    /// </summary>
    /// <param name="index">The index value from the CEL expression</param>
    /// <param name="targetKeyType">The expected key type of the MapField</param>
    /// <returns>The converted key, or null if conversion is not possible</returns>
    private static object? TryConvertKeyForMapField(object? index, Type targetKeyType)
    {
        if (index == null)
            return null;

        // If types already match, return as-is
        if (targetKeyType.IsAssignableFrom(index.GetType()))
            return index;

        try
        {
            // Handle common Protocol Buffer key type conversions
            if (targetKeyType == typeof(int))
            {
                return MessageHelpers.ConvertToInt32(index);
            }
            else if (targetKeyType == typeof(uint))
            {
                return MessageHelpers.ConvertToUInt32(index);
            }
            else if (targetKeyType == typeof(long))
            {
                return Int64Helpers.ConvertInt(index);
            }
            else if (targetKeyType == typeof(ulong))
            {
                return UInt64Helpers.ConvertUInt(index);
            }
            else if (targetKeyType == typeof(bool))
            {
                // Boolean keys should match exactly
                return index is bool ? index : null;
            }
            else if (targetKeyType == typeof(string))
            {
                return StringHelpers.ConvertString(index);
            }
            
            // For any other types, try direct conversion
            return Convert.ChangeType(index, targetKeyType);
        }
        catch
        {
            // If conversion fails, return null to indicate failure
            return null;
        }
    }

    public override CelExpressionDelegate VisitInt([NotNull] CelParser.IntContext context)
    {
        return tryGetVariable =>
        {
            var token = context.tok.Text;
            if (context.sign?.Text == "-")
            {
                token = "-" + token;
            }

            return Int64Helpers.ConvertInt(token);
        };
    }

    public override CelExpressionDelegate VisitListInit([NotNull] CelParser.ListInitContext context)
    {
        return base.VisitListInit(context);
    }

    public override CelExpressionDelegate VisitLogicalNot([NotNull] CelParser.LogicalNotContext context)
    {
        if (context._ops == null || context._ops.Count == 0)
        {
            var result = base.VisitLogicalNot(context);
            return result;
        }

        return tryGetVariable =>
        {
            var member = Visit(context.member()).Invoke(tryGetVariable);

            foreach (var op in context._ops)
            {
                if (op.Text == "!")
                {
                    member = ArithmeticFunctions.LogicalNot(member);
                }
            }

            return member;
        };
    }

    public override CelExpressionDelegate VisitMapInitializerList([NotNull] CelParser.MapInitializerListContext context)
    {
        if (context._keys == null || context._values == null)
        {
            return base.VisitMapInitializerList(context);
        }

        if (context._keys.Count != context._values.Count)
        {
            throw new CelExpressionParserException("Map list has different number of keys and values.");
        }

        return tryGetVariable =>
        {
            var dictionary = new Dictionary<string, object?>();

            for (var i = 0; i < context._keys.Count; i++)
            {
                var key = Visit(context._keys[i]).Invoke(tryGetVariable);
                string keyString;

                if (key is string keyAsString)
                {
                    keyString = keyAsString;
                }
                else if (key is ulong keyUInt)
                {
                    keyString = StringHelpers.ConvertStringUInt(keyUInt);
                }
                else if (key is long keyInt)
                {
                    keyString = StringHelpers.ConvertStringInt(keyInt);
                }
                else if (key is bool keyBool)
                {
                    keyString = StringHelpers.ConvertStringBool(keyBool);
                }
                else
                {
                    throw new CelMapUnsupportedKeyTypeException("Cannot map item because key is not the required type.");
                }

                var value = Visit(context._values[i]).Invoke(tryGetVariable);

                if (dictionary.ContainsKey(keyString))
                {
                    throw new CelMapDuplicateKeyException($"Duplicate map key '{keyString}'.");
                }

                dictionary.Add(keyString, value);
            }

            return dictionary;
        };
    }

    public override CelExpressionDelegate VisitMemberCall([NotNull] CelParser.MemberCallContext context)
    {
        return tryGetVariable =>
        {
            var id = context.id;
            var memberValue = Visit(context.member()).Invoke(tryGetVariable);

            if (!string.IsNullOrWhiteSpace(id.Text))
            {
                object? args = null;

                var exprList = context.exprList();


                if (exprList != null && exprList._e.Count == 2)
                {
                    if (InternalMacros.TryGetValue(id.Text, out var macroFunction))
                    {
                        var variableName = exprList._e[0].GetText();

                        //get the expression but do not evaluate it;
                        var expression = Visit(exprList._e[1]);

                        //wrap the TryGetVariable function so that it always uses the namespace.
                        var tryGetVariableWithNamespaceFunc = new TryGetVariableDelegate((string c_variableName, out object? c_value) =>
                                                                                             TryGetVariableWithNamespace(tryGetVariable, MessageNamespace, c_variableName, out c_value)
                                                                                        );

                        var tryGetFunction = new TryGetFunctionWithArgValuesDelegate((string name, object?[] argValues, out CelFunctionDelegate? funcDelegate) =>
                        {
                            if (Functions.TryGetFunctionWithArgValues(name, argValues, out funcDelegate))
                            {
                                return true;
                            }

                            funcDelegate = null;
                            return false;
                        });

                        return macroFunction.Invoke(memberValue, variableName, expression, tryGetVariableWithNamespaceFunc, tryGetFunction);
                    }
                }


                //build function list
                if (exprList != null)
                {
                    args = Visit(exprList).Invoke(tryGetVariable);
                }

                object?[] memberWithArgsArray;
                if (args is null)
                {
                    memberWithArgsArray = new[] { memberValue };
                }
                else if (args is object?[] argsArray)
                {
                    memberWithArgsArray = new[] { memberValue }.Concat(argsArray).ToArray();
                }
                else
                {
                    memberWithArgsArray = new[] { memberValue, args };
                }

                //try invoking function
                if (Functions.TryGetFunctionWithArgValues(id.Text, memberWithArgsArray, out var internalFunction))
                {
                    return internalFunction!.Invoke(memberWithArgsArray);
                }

                if (CelAbstractTypes.TryGetValue(id.Text, out var internalVariableValue))
                {
                    return internalVariableValue;
                }

                if (TryGetVariableWithNamespace(tryGetVariable, MessageNamespace, id.Text, out var variableValue))
                {
                    return variableValue;
                }


                if (memberValue == null && !string.IsNullOrWhiteSpace(context.member().GetText()) && exprList != null)
                {
                    //we could have an enum declaration here
                    args = Visit(exprList).Invoke(tryGetVariable);

                    var qualifiedMember = context.member().GetText() + "." + id.Text;
                    var enumDescriptor = GetEnumDescriptor(qualifiedMember);
                    if (enumDescriptor != null)
                    {
                        if (args is object?[] argsArray && argsArray.Length == 1)
                        {
                            if (argsArray[0] is long argsLong)
                            {
                                if (argsLong >= int.MinValue && argsLong <= int.MaxValue)
                                {
                                    var enumValueDescriptor = enumDescriptor.FindValueByNumber((int)argsLong);
                                    if (enumValueDescriptor == null)
                                    {
                                        //this line here is to handle unknown enum types.
                                        return argsLong;
                                    }

                                    return enumValueDescriptor;
                                }

                                throw new CelArgumentRangeException($"Could not instantiate enum '{enumDescriptor.FullName}' because the value '{argsLong}' is out of range.");
                            }

                            if (argsArray[0] is string argsString)
                            {
                                var enumValueDescriptor = enumDescriptor.FindValueByName(argsString);
                                if (enumValueDescriptor == null)
                                {
                                    throw new CelArgumentRangeException($"Could not instantiate enum '{enumDescriptor.FullName}' because the name '{argsString}' was not found in the descriptor.");
                                }

                                return enumValueDescriptor;
                            }
                        }

                        throw new CelNoSuchOverloadException($"Could not instantiate enum '{enumDescriptor.FullName}' because the value '{exprList?.GetText()}' was not found in the descriptor.");
                    }
                }

                throw new CelExpressionParserException($"Could not find evaluate member call because no function, variable or macro was found that matches identifier '{id.Text}'.");
            }


            return base.VisitMemberCall(context);
        };
    }

    public override CelExpressionDelegate VisitMemberExpr([NotNull] CelParser.MemberExprContext context)
    {
        var result = base.VisitMemberExpr(context);
        return result;
    }

    public override CelExpressionDelegate VisitNegate([NotNull] CelParser.NegateContext context)
    {
        if (context._ops == null || context._ops.Count == 0)
        {
            var result = base.VisitNegate(context);
            return result;
        }

        return tryGetVariable =>
        {
            var member = Visit(context.member()).Invoke(tryGetVariable);

            foreach (var op in context._ops)
            {
                if (op.Text == "-")
                {
                    member = ArithmeticFunctions.Negate(member);
                }
            }

            return member;
        };
    }

    public override CelExpressionDelegate VisitNested([NotNull] CelParser.NestedContext context)
    {
        var result = Visit(context.e);
        return result;
    }

    public override CelExpressionDelegate VisitNull([NotNull] CelParser.NullContext context)
    {
        return tryGetVariable => null;
    }

    public override CelExpressionDelegate VisitOptExpr([NotNull] CelParser.OptExprContext context)
    {
        return base.VisitOptExpr(context);
    }

    public override CelExpressionDelegate VisitOptField([NotNull] CelParser.OptFieldContext context)
    {
        return tryGetVariable => context.id?.Text;
    }

    public override CelExpressionDelegate VisitPrimaryExpr([NotNull] CelParser.PrimaryExprContext context)
    {
        var result = base.VisitPrimaryExpr(context);
        return result;
    }

    public override CelExpressionDelegate VisitRelation([NotNull] CelParser.RelationContext context)
    {
        return tryGetVariable =>
        {
            if (context.op == null)
            {
                var result = base.VisitRelation(context).Invoke(tryGetVariable);
                return result;
            }

            var left = context.children[0];
            var right = context.children[2];

            var leftResult = Visit(left).Invoke(tryGetVariable);
            var rightResult = Visit(right).Invoke(tryGetVariable);


            //check that we have fields.
            if (leftResult is CelNoSuchField celNoSuchFieldLeft)
            {
                throw new CelNoSuchFieldException(celNoSuchFieldLeft.Message);
            }

            if (rightResult is CelNoSuchField celNoSuchFieldRight)
            {
                throw new CelNoSuchFieldException(celNoSuchFieldRight.Message);
            }

            if (context.op.Text == "in")
            {
                return CompareFunctions.Contains(leftResult, rightResult, TypeRegistry);
            }

            var compareResult = CompareFunctions.Compare(leftResult, rightResult, TypeRegistry);

            if (context.op.Text == "==")
            {
                return compareResult == 0;
            }

            if (context.op.Text == "!=")
            {
                return compareResult != 0;
            }

            //we can't do  < or > comparisons on lists and maps.
            //we can only check equality
            if (leftResult is object?[]
                || rightResult is object[]
                || leftResult is IList
                || rightResult is IList
                || leftResult is Dictionary<string, object?>
                || rightResult is Dictionary<string, object?>
                || leftResult == null
                || rightResult == null
                || compareResult == -2)
            {
                throw new CelNoSuchOverloadException($"No overload exists for use of operator '{context.op.Text}' with types '{leftResult?.GetType().FullName ?? "null"}' and '{rightResult?.GetType().FullName ?? "null"}'.");
            }

            if (context.op.Text == ">=")
            {
                return compareResult >= 0;
            }

            if (context.op.Text == ">")
            {
                return compareResult > 0;
            }

            if (context.op.Text == "<=")
            {
                return compareResult <= 0;
            }

            if (context.op.Text == "<")
            {
                return compareResult < 0;
            }

            throw new CelExpressionParserException($"Relation operation '{context.op.Text}' is not supported.", context);
        };
    }

    public override CelExpressionDelegate VisitSelect([NotNull] CelParser.SelectContext context)
    {
        var memberName = context.member().GetText();
        var identifier = context.id.Text;

        WriteDebugLine($"Visiting Select Member: {memberName} Identifier: {identifier}");

        if (string.IsNullOrWhiteSpace(identifier))
        {
            return base.VisitSelect(context);
        }

        return tryGetVariable =>
        {
            //try to get the value as a variable.
            var variableName = context.GetText();
            if (tryGetVariable(variableName, out var variableValue))
            {
                return variableValue;
            }

            var enumDescriptor = GetEnumDescriptor(memberName);
            if (enumDescriptor != null)
            {
                var enumValueDescriptor = enumDescriptor.FindValueByName(identifier);
                if (enumValueDescriptor == null)
                {
                    throw new CelNoSuchFieldException($"Cannot find enum descriptor '{memberName}.{identifier}'.");
                }

                return enumValueDescriptor;
            }

            var member = Visit(context.member()).Invoke(tryGetVariable);
            if (member == null)
            {
                return null;
            }

            if (member is IMessage memberIMessage)
            {
                //trying to evaluate an expression like "TestAllTypes{}.single_nested_message.bb"
                //we need to instantiate the TestAllTypes{}.single_nested_message message.
                var needToInstantiate = context?.Parent is CelParser.SelectContext;
                var checkForFieldPresence = false;


                //if we are calling the "has" function, we need to check if the field is set or if it is default.
                var getIdentOrGlobalCallContextParentInTree = GetIdentOrGlobalCallContextParentInTree(context?.Parent);
                if (getIdentOrGlobalCallContextParentInTree != null)
                {
                    if (getIdentOrGlobalCallContextParentInTree.id?.Text == "has")
                    {
                        checkForFieldPresence = true;
                    }
                }

                //get the field value.
                var value = MessageHelpers.GetMessageValue(memberIMessage, identifier, checkForFieldPresence, needToInstantiate);
                return value;
            }

            if (member is Dictionary<string, object> memberDict)
            {
                if (memberDict.TryGetValue(identifier, out var value))
                {
                    return value;
                }

                return new CelNoSuchField($"Cannot find overload to select field '{identifier}' in variable '{memberName}'.");
            }

            //we have a plain old CLR object that we want to get info for.
            //this reflection is slow though so it is preferable to use IMessage
            var memberType = member.GetType();
            var memberPropertyInfo = memberType.GetProperty(identifier, BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);
            if (memberPropertyInfo != null && memberPropertyInfo.CanRead && memberPropertyInfo.GetIndexParameters().Length == 0)
            {
                return memberPropertyInfo.GetValue(member);
            }

            throw new CelTypeDoesNotSupportFieldSelectionException($"Type '{memberName}' does not support field selection for field '{identifier}'.");
        };
    }

    public override CelExpressionDelegate VisitStart([NotNull] CelParser.StartContext context)
    {
        var result = Visit(context.e);
        return result;
    }

    public override CelExpressionDelegate VisitString([NotNull] CelParser.StringContext context)
    {
        return tryGetVariable =>
        {
            var value = context.tok.Text;
            object unescapedString;

            try
            {
                unescapedString = StringHelpers.Unescape(value, false);
            }
            catch (CelExpressionParserException x)
            {
                x.Node = context;
                throw;
            }

            if (unescapedString is not string)
            {
                throw new CelExpressionParserException("Could not parse string value.", context);
            }

            return unescapedString;
        };
    }

    public override CelExpressionDelegate VisitUint([NotNull] CelParser.UintContext context)
    {
        return tryGetVariable =>
        {
            var token = context.tok.Text;
            return UInt64Helpers.ConvertUInt(token);
        };
    }

    #endregion

    #region Private Function

    private CelParser.IdentOrGlobalCallContext? GetIdentOrGlobalCallContextParentInTree(IParseTree? context)
    {
        if (context?.Parent == null)
        {
            return null;
        }

        if (context.Parent is CelParser.IdentOrGlobalCallContext identOrGlobalCallContext)
        {
            return identOrGlobalCallContext;
        }

        return GetIdentOrGlobalCallContextParentInTree(context.Parent);
    }


    private IEnumerable<EnumDescriptor> GetEnumDescriptors(MessageDescriptor messageDescriptor)
    {
        foreach (var descriptor in messageDescriptor.EnumTypes)
        {
            yield return descriptor;
        }
    }

    private IEnumerable<MessageDescriptor> GetMessageDescriptors(MessageDescriptor messageDescriptor)
    {
        foreach (var descriptor in messageDescriptor.NestedTypes)
        {
            yield return descriptor;
        }
    }


    private EnumDescriptor? GetEnumDescriptor(string identifier)
    {
        var messageEnumDescriptors = FileDescriptors.SelectMany(c => c.MessageTypes).SelectMany(GetEnumDescriptors);
        var globalEnumDescriptors = FileDescriptors.SelectMany(c => c.EnumTypes);

        var enumDescriptors = globalEnumDescriptors.Union(messageEnumDescriptors).ToList();

        if (identifier.StartsWith(".", StringComparison.Ordinal))
        {
            //function started with an explicit period, so we need to scope the resolution locally.
            return enumDescriptors.FirstOrDefault(c => c.FullName == identifier.Substring(1));
        }

        //find the name outright because we have no namespace
        if (string.IsNullOrWhiteSpace(MessageNamespace))
        {
            return enumDescriptors.FirstOrDefault(c => c.FullName == identifier);
        }

        //find fully qualified A.B.a.b
        var qualifiedNamespace = MessageNamespace + "." + identifier;
        var descriptor = enumDescriptors.FirstOrDefault(c => c.FullName == qualifiedNamespace);
        if (descriptor != null)
        {
            return descriptor;
        }


        //we didn't find A.B.a.b, so try A.a.b, if A.B had to terms.
        var namespaceSplit = MessageNamespace!.Split('.');
        if (namespaceSplit.Length > 1)
        {
            qualifiedNamespace = string.Join(".", namespaceSplit.Take(namespaceSplit.Length - 1)) + "." + identifier;
            descriptor = enumDescriptors.FirstOrDefault(c => c.FullName == qualifiedNamespace);
        }

        if (descriptor != null)
        {
            return descriptor;
        }


        if (identifier.StartsWith("protobuf.", StringComparison.Ordinal))
        {
            //this is required for some test that are creating "protobuf.Any(" and missing the "google." prefix.
            qualifiedNamespace = "google." + identifier;
            descriptor = enumDescriptors.FirstOrDefault(c => c.FullName == qualifiedNamespace);
        }

        //we may or may not have a descriptor here, so we could return null.
        return descriptor;
    }

    private MessageDescriptor? GetMessageDescriptor(string identifier)
    {
        // Name Resolution
        // A CEL expression is parsed in the scope of a specific protocol buffer package or message, which controls the interpretation of names.
        // The scope is set by the application context of an expression. A CEL expression can contain simple names as in a or qualified names as in a.b. The meaning of such expressions is a combination of one or more of:
        //
        // Variables and Functions: some simple names refer to variables in the execution context, standard functions, or other name bindings provided by the CEL application.
        //     Field selection: appending a period and identifier to an expression could indicate that we're accessing a field within a protocol buffer or map.
        //     Protocol buffer package names: a simple or qualified name could represent an absolute or relative name in the protocol buffer package namespace.
        //              Package names must be followed by a message type, enum type, or enum constant.
        //     Protocol buffer message types, enum types, and enum constants: following an optional protocol buffer package name, a simple or qualified name could
        //          refer to a message type, and enum type, or an enum constant in the package's namespace.

        // Resolution works as follows. If a.b is a name to be resolved in the context of a protobuf declaration with scope A.B, then resolution is attempted,
        //      in order, as A.B.a.b, A.a.b, and finally a.b. To override this behavior, one can use .a.b; this name will only be attempted to be resolved in the root scope, i.e. as a.b.

        var messageMessageDescriptors = FileDescriptors.SelectMany(c => c.MessageTypes).SelectMany(GetMessageDescriptors);
        var globalMessageDescriptors = FileDescriptors.SelectMany(c => c.MessageTypes);
        var messageDescriptors = globalMessageDescriptors.Union(messageMessageDescriptors).ToList();

        if (identifier.StartsWith(".", StringComparison.Ordinal))
        {
            //function started with an explicit period, so we need to scope the resolution locally.
            return messageDescriptors.FirstOrDefault(c => c.FullName == identifier.Substring(1));
        }

        //find the name outright because we have no namespace
        if (string.IsNullOrWhiteSpace(MessageNamespace))
        {
            return messageDescriptors.FirstOrDefault(c => c.FullName == identifier);
        }

        //find fully qualified A.B.a.b
        var qualifiedNamespace = MessageNamespace + "." + identifier;
        var descriptor = messageDescriptors.FirstOrDefault(c => c.FullName == qualifiedNamespace);
        if (descriptor != null)
        {
            return descriptor;
        }


        //we didn't find A.B.a.b, so try A.a.b, if A.B had to terms.
        var namespaceSplit = MessageNamespace!.Split('.');
        if (namespaceSplit.Length > 1)
        {
            qualifiedNamespace = string.Join(".", namespaceSplit.Take(namespaceSplit.Length - 1)) + "." + identifier;
            descriptor = messageDescriptors.FirstOrDefault(c => c.FullName == qualifiedNamespace);
        }

        if (descriptor != null)
        {
            return descriptor;
        }


        if (identifier.StartsWith("protobuf.", StringComparison.Ordinal))
        {
            //this is required for some test that are creating "protobuf.Any(" and missing the "google." prefix.
            qualifiedNamespace = "google." + identifier;
            descriptor = messageDescriptors.FirstOrDefault(c => c.FullName == qualifiedNamespace);
        }

        //we may or may not have a descriptor here, so we could return null.
        return descriptor;
    }

    private static bool TryGetVariableWithNamespace(TryGetVariableDelegate tryGetVariableFunc, string? messageNamespace, string variableName, out object? value)
    {
        if (variableName.StartsWith(".", StringComparison.Ordinal))
        {
            //function started with an explicit period, so we need to scope the resolution locally.
            return tryGetVariableFunc.Invoke(variableName.Substring(1), out value);
        }

        //find the name outright because we have no namespace
        if (string.IsNullOrWhiteSpace(messageNamespace))
        {
            return tryGetVariableFunc.Invoke(variableName, out value);
        }

        //find fully qualified A.B.a.b
        var qualifiedVariableNameWithNamespace = messageNamespace + "." + variableName;
        if (tryGetVariableFunc.Invoke(qualifiedVariableNameWithNamespace, out value))
        {
            return true;
        }

        //we didn't find A.B.a.b, so try A.a.b, if A.B had to terms.
        var namespaceSplit = messageNamespace!.Split('.');
        if (namespaceSplit.Length > 1)
        {
            qualifiedVariableNameWithNamespace = string.Join(".", namespaceSplit.Take(namespaceSplit.Length - 1)) + "." + variableName;
        }

        if (tryGetVariableFunc.Invoke(qualifiedVariableNameWithNamespace, out value))
        {
            return true;
        }

        //try again with no namespace qualification
        return tryGetVariableFunc.Invoke(variableName, out value);
    }

    #endregion

    #region Private

    [Conditional("DEBUG")]
    private static void WriteDebugLine(string message)
    {
        //uncomment this if you need help with debugging tests.
        //Console.WriteLine(message);
    }

    #endregion
}