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
using Cel.Internal;
using Google.Api.Expr.Test.V1.Proto2;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;
using Enum = System.Enum;
using Value = Cel.Expr.Value;

namespace Cel.Tests;

[TestFixture]
public class SimpleTestTests
{
    private static object GetValue(Value value, TypeRegistry typeRegistry)
    {
        if (value == null)
        {
            return null;
        }

        if (value.KindCase == Value.KindOneofCase.BoolValue)
        {
            return value.BoolValue;
        }

        if (value.KindCase == Value.KindOneofCase.BytesValue)
        {
            return value.BytesValue;
        }

        if (value.KindCase == Value.KindOneofCase.Int64Value)
        {
            return value.Int64Value;
        }

        if (value.KindCase == Value.KindOneofCase.Uint64Value)
        {
            return value.Uint64Value;
        }

        if (value.KindCase == Value.KindOneofCase.DoubleValue)
        {
            return value.DoubleValue;
        }

        if (value.KindCase == Value.KindOneofCase.StringValue)
        {
            return value.StringValue;
        }

        if (value.KindCase == Value.KindOneofCase.NullValue)
        {
            return null;
        }

        if (value.KindCase == Value.KindOneofCase.TypeValue)
        {
            return new CelType(value.TypeValue);
        }

        if (value.KindCase == Value.KindOneofCase.EnumValue)
        {
            return value.EnumValue.Value;
        }

        if (value.KindCase == Value.KindOneofCase.ListValue)
        {
            var listValue = value.ListValue;
            if (listValue?.Values == null || listValue.Values.Count == 0)
            {
                return null;
            }

            var result = new List<object>();
            foreach (var entry in listValue.Values)
            {
                var entryValue = GetValue(entry, typeRegistry);
                result.Add(entryValue);
            }

            return result.ToArray();
        }

        if (value.KindCase == Value.KindOneofCase.MapValue)
        {
            if (value.MapValue?.Entries == null || value?.MapValue.Entries.Count == 0)
            {
                return null;
            }

            var result = new Dictionary<string, object>();
            foreach (var entry in value!.MapValue.Entries)
            {
                var entryKey = GetValue(entry.Key, typeRegistry)?.ToString();
                if (entryKey == null)
                {
                    throw new Exception("Map key is invalid.");
                }

                var entryValue = GetValue(entry.Value, typeRegistry);
                result[entryKey] = entryValue;
            }

            return result;
        }

        if (value.KindCase == Value.KindOneofCase.ObjectValue)
        {
            //Unpack Wrappers
            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.BoolValue")
            {
                return value.ObjectValue.Unpack<BoolValue>().Value;
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.BytesValue")
            {
                return value.ObjectValue.Unpack<BytesValue>().Value;
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.DoubleValue")
            {
                return value.ObjectValue.Unpack<DoubleValue>().Value;
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.FloatValue")
            {
                return value.ObjectValue.Unpack<FloatValue>().Value;
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.Int32Value")
            {
                return value.ObjectValue.Unpack<Int32Value>().Value;
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.Int64Value")
            {
                return value.ObjectValue.Unpack<Int64Value>().Value;
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.UInt32Value")
            {
                return value.ObjectValue.Unpack<UInt32Value>().Value;
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.UInt64Value")
            {
                return value.ObjectValue.Unpack<UInt64Value>().Value;
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.StringValue")
            {
                return value.ObjectValue.Unpack<StringValue>().Value;
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.Duration")
            {
                return value.ObjectValue.Unpack<Duration>();
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.Int32Value")
            {
                return value.ObjectValue.Unpack<Int32Value>();
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.Value")
            {
                var valueValue = value.ObjectValue.Unpack<Google.Protobuf.WellKnownTypes.Value>();
                return valueValue.Unwrap();
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.Struct")
            {
                var structValue = value.ObjectValue.Unpack<Struct>();
                return structValue.UnwrapToDictionary();
            }

            if (value.ObjectValue.TypeUrl == "type.googleapis.com/google.protobuf.ListValue")
            {
                var listValue = value.ObjectValue.Unpack<ListValue>();
                return listValue.UnwrapToArray();
            }

            //we don't have a wrapper type
            return value.ObjectValue.Unpack(typeRegistry);
        }

        throw new Exception($"Value kind {value.KindCase} is not supported by SimpleTest suite.");
    }

    private static object UnpackWellKnownTypes(object value, TypeRegistry typeRegistry)
    {
        if (value is Any valueAny)
        {
            if (valueAny.Value.Length == 0)
            {
                return null;
            }

            value = valueAny.Unpack(typeRegistry);
        }

        if (value is Enum)
        {
            return (int)value;
        }

        if (value is EnumValueDescriptor valueEnumValueDescriptor)
        {
            value = valueEnumValueDescriptor.Number;
        }

        if (value is Google.Protobuf.WellKnownTypes.Value valueValue)
        {
            value = valueValue.Unwrap();
        }

        if (value is ListValue valueListValue)
        {
            value = valueListValue.UnwrapToArray();
        }

        if (value is Struct valueStruct)
        {
            value = valueStruct.UnwrapToDictionary();
        }

        if (value is ICollection valueCollection && valueCollection.Count == 0)
        {
            value = null;
        }

        if (value is IDictionary valueDictionary && valueDictionary.Count == 0)
        {
            value = null;
        }

        return value;
    }


    [Test]
    [TestCaseSource(typeof(SimpleTestDataParser), nameof(SimpleTestDataParser.GetTestCases), Category = "Simple Tests")]
    public void SimpleTest(SimpleTestDataLoader testCase)
    {
        var test = testCase.Test;

        if (!string.IsNullOrWhiteSpace(test.IgnoreReason))
        {
            Assert.Ignore(test.IgnoreReason);
            return;
        }

        var fileDescriptors = new[]
        {
            TestAllTypesReflection.Descriptor,
            Google.Api.Expr.Test.V1.Proto3.TestAllTypesReflection.Descriptor
        };
        var typeRegistry = TypeRegistry.FromFiles(fileDescriptors);

        var variables = new Dictionary<string, object>();
        foreach (var binding in test.Bindings)
        {
            variables[binding.Key] = GetValue(binding.Value.Value, typeRegistry);
        }

        var celEnvironment = new CelEnvironment(fileDescriptors, test.Container);
        celEnvironment.RegisterStringExtensionFunctions();


        var context = celEnvironment.Parse(test.Expr);

        Assert.That(context, Is.Not.Null);

        //check the expression parsed properly
        if (!test.DisableCheck)
        {
            celEnvironment.Compile(context);
        }

        try
        {
            //now evaluate the expression

            var programResult = celEnvironment.Program(context, variables);

            var expectedValue = GetValue(test.Value, typeRegistry);

            programResult = UnpackWellKnownTypes(programResult, typeRegistry);


            Assert.That(programResult, Is.EqualTo(expectedValue));
        }
        catch (CelArgumentRangeException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            var message = evalError?.Message ?? "";
            if (message == "range"
                || message == "range error"
                || message == "invalid"
                || message == "invalid_argument"
                || message == "no such key"
                || message.Contains("index out of range"))
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelArgumentRangeException was thrown but not expected.  {x}");
            }
        }
        catch (CelUndeclaredReferenceException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            var message = evalError?.Message ?? "";
            if (message.Contains("undeclared reference"))
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelUndeclaredReferenceException was thrown but not expected.  {x}");
            }
        }
        catch (CelUnboundFunctionException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            var message = evalError?.Message ?? "";
            if (message.Contains( "unbound function"))
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelUnboundFunctionException was thrown but not expected.  {x}");
            }
        }
        catch (CelOverflowException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            if (evalError?.Message == "return error for overflow")
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelOverflowException was thrown but not expected.  {x}");
            }
        }
        catch (CelNoSuchFieldException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            var message = evalError?.Message ?? "";
            if (message.Contains("no such key")
                || message.Contains("no_such_field")
                || message.Contains("no_matching_overload"))
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelNoSuchFieldException was thrown but not expected.  {x}");
            }
        }
        catch (CelTypeDoesNotSupportFieldSelectionException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            var message = evalError?.Message ?? "";
            if (message.Contains("does not support field selection")
                || message.Contains("no_matching_overload"))
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelTypeDoesNotSupportFieldSelectionException was thrown but not expected.  {x}");
            }
        }
        catch (CelTypeCreationException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            var evalErrorMessage = evalError?.Message ?? "";

            if (evalErrorMessage == "conversion")
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelTypeCreationException was thrown but not expected.  {x}");
            }
        }
        catch (CelConversionException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            var evalErrorMessage = evalError?.Message ?? "";

            if (evalErrorMessage == "conversion")
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelMessageConversionException was thrown but not expected.  {x}");
            }
        }
        catch (CelMapUnsupportedKeyTypeException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            var evalErrorMessage = evalError?.Message ?? "";

            if (evalErrorMessage == "unsupported key type")
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelMapUnsupportedKeyTypeException was thrown but not expected.  {x}");
            }
        }
        catch (CelMapDuplicateKeyException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            var evalErrorMessage = evalError?.Message ?? "";

            if (evalErrorMessage == "Failed with repeated key")
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelMapDuplicateKeyException was thrown but not expected.  {x}");
            }
        }
        catch (CelNoSuchOverloadException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            var evalErrorMessage = evalError?.Message ?? "";

            if (evalErrorMessage == "no such overload"
                || evalErrorMessage == "no_such_overload"
                || evalErrorMessage == "no_matching_overload"
                || evalErrorMessage.Contains("no matching overload"))
            {
                //this is good
            }
            else
            {
                Assert.Fail($"NoSuchOverloadException was thrown but not expected.  {x}");
            }
        }
        catch (CelDivideByZeroException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            if (evalError?.Message == "divide by zero"
                || evalError?.Message == "division by zero")
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelDivideByZeroException was thrown but not expected.  {x}");
            }
        }
        catch (CelModulusByZeroException x)
        {
            var evalError = test.EvalError?.Errors?.FirstOrDefault();
            if (evalError?.Message == "modulus by zero")
            {
                //this is good
            }
            else
            {
                Assert.Fail($"CelModulusByZeroException was thrown but not expected.  {x}");
            }
        }
    }
}