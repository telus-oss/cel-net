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

using NUnit.Framework;
using Google.Api.Expr.Test.V1.Proto3;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Cel.Tests
{
    /// <summary>
    /// Comprehensive test suite for array, list, and repeated field operations
    /// Combines basic CEL array/list functionality with protobuf repeated field testing
    /// covering edge cases, error handling, and complex expression scenarios
    /// </summary>
    [TestFixture]
    public class ArrayAndRepeatedTests
    {
        private CelEnvironment basicCelEnvironment;
        private CelEnvironment protobufCelEnvironment;

        [SetUp]
        public void SetUp()
        {
            // Basic environment without protobuf support
            var emptyFileDescriptors = new FileDescriptor[] { };
            basicCelEnvironment = new CelEnvironment(emptyFileDescriptors, string.Empty);

            // Protobuf-enabled environment
            var protobufFileDescriptors = new FileDescriptor[] { TestAllTypesReflection.Descriptor };
            protobufCelEnvironment = new CelEnvironment(protobufFileDescriptors, "google.api.expr.test.v1.proto3");
        }

        #region Basic Array and List Operations

        [TestFixture]
        public class BasicListOperations : ArrayAndRepeatedTests
        {
            [Test]
            public void List_Creation_Should_Work_Correctly()
            {
                var result = basicCelEnvironment.Program("[1, 2, 3]", new Dictionary<string, object>());
                Assert.That(result, Is.EquivalentTo(new object[] { 1L, 2L, 3L }));
            }

            [Test]
            public void Empty_List_Should_Work_Correctly()
            {
                var result = basicCelEnvironment.Program("[]", new Dictionary<string, object>());
                Assert.That(result, Is.EquivalentTo(new object[] { }));
            }

            [Test]
            public void List_Indexing_Should_Work_Correctly()
            {
                var variables = new Dictionary<string, object>
                {
                    ["list"] = new object[] { "a", "b", "c" }
                };

                Assert.That(basicCelEnvironment.Program("list[0]", variables), Is.EqualTo("a"));
                Assert.That(basicCelEnvironment.Program("list[1]", variables), Is.EqualTo("b"));
                Assert.That(basicCelEnvironment.Program("list[2]", variables), Is.EqualTo("c"));
            }

            [Test]
            public void List_Index_Out_Of_Range_Should_Throw_Exception()
            {
                var variables = new Dictionary<string, object>
                {
                    ["list"] = new object[] { "a", "b" }
                };

                Assert.Throws<CelArgumentRangeException>(() => 
                    basicCelEnvironment.Program("list[5]", variables));
            }

            [Test]
            public void List_In_Operator_Should_Work_Correctly()
            {
                var variables = new Dictionary<string, object>
                {
                    ["list"] = new object[] { 1L, 2L, 3L }
                };

                Assert.That(basicCelEnvironment.Program("1 in list", variables), Is.True);
                Assert.That(basicCelEnvironment.Program("4 in list", variables), Is.False);
            }

            [Test]
            public void Mixed_Type_List_Should_Work()
            {
                var variables = new Dictionary<string, object>
                {
                    ["mixed"] = new object[] { 1L, "hello", true, 3.14 }
                };

                Assert.That(basicCelEnvironment.Program("mixed[0]", variables), Is.EqualTo(1L));
                Assert.That(basicCelEnvironment.Program("mixed[1]", variables), Is.EqualTo("hello"));
                Assert.That(basicCelEnvironment.Program("mixed[2]", variables), Is.True);
                Assert.That(basicCelEnvironment.Program("mixed[3]", variables), Is.EqualTo(3.14));
            }
        }

        [TestFixture]
        public class BasicMapOperations : ArrayAndRepeatedTests
        {
            [Test]
            public void Map_Creation_Should_Work_Correctly()
            {
                var result = basicCelEnvironment.Program("{'key': 'value', 'num': 42}", new Dictionary<string, object>()) as Dictionary<string, object>;
                
                Assert.That(result, Is.Not.Null);
                Assert.That(result["key"], Is.EqualTo("value"));
                Assert.That(result["num"], Is.EqualTo(42L));
            }

            [Test]
            public void Empty_Map_Should_Work_Correctly()
            {
                var result = basicCelEnvironment.Program("{}", new Dictionary<string, object>()) as Dictionary<string, object>;
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(0));
            }

            [Test]
            public void Map_Access_Should_Work_Correctly()
            {
                var variables = new Dictionary<string, object>
                {
                    ["map"] = new Dictionary<string, object> { ["key1"] = "value1", ["key2"] = 42L }
                };

                Assert.That(basicCelEnvironment.Program("map['key1']", variables), Is.EqualTo("value1"));
                Assert.That(basicCelEnvironment.Program("map['key2']", variables), Is.EqualTo(42L));
            }

            [Test]
            public void Map_Key_Not_Found_Should_Throw_Exception()
            {
                var variables = new Dictionary<string, object>
                {
                    ["map"] = new Dictionary<string, object> { ["key1"] = "value1" }
                };

                Assert.Throws<CelArgumentRangeException>(() => 
                    basicCelEnvironment.Program("map['nonexistent']", variables));
            }

            [Test]
            public void Map_In_Operator_Should_Work_Correctly()
            {
                var variables = new Dictionary<string, object>
                {
                    ["map"] = new Dictionary<string, object> { ["key1"] = "value1", ["key2"] = "value2" }
                };

                Assert.That(basicCelEnvironment.Program("\"key1\" in map", variables), Is.True);
                Assert.That(basicCelEnvironment.Program("\"nonexistent\" in map", variables), Is.False);
            }
        }

        #endregion

        #region Protobuf Repeated Field Operations

        [TestFixture]
        public class ProtobufRepeatedFields : ArrayAndRepeatedTests
        {
            [Test]
            public void Repeated_Int32_Field_Access_Should_Work()
            {
                var testMessage = new TestAllTypes();
                testMessage.RepeatedInt32.AddRange(new[] { 1, 2, 3, 4, 5 });

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test access by index
                Assert.That(protobufCelEnvironment.Program("msg.repeated_int32[0]", variables), Is.EqualTo(1));
                Assert.That(protobufCelEnvironment.Program("msg.repeated_int32[2]", variables), Is.EqualTo(3));
                Assert.That(protobufCelEnvironment.Program("msg.repeated_int32[4]", variables), Is.EqualTo(5));

                // Test size function
                Assert.That(protobufCelEnvironment.Program("size(msg.repeated_int32)", variables), Is.EqualTo(5L));

                // Test in operator
                Assert.That(protobufCelEnvironment.Program("3 in msg.repeated_int32", variables), Is.True);
                Assert.That(protobufCelEnvironment.Program("6 in msg.repeated_int32", variables), Is.False);
            }

            [Test]
            public void Repeated_String_Field_Operations_Should_Work()
            {
                var testMessage = new TestAllTypes();
                testMessage.RepeatedString.AddRange(new[] { "hello", "world", "test", "cel" });

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test string access
                Assert.That(protobufCelEnvironment.Program("msg.repeated_string[0]", variables), Is.EqualTo("hello"));
                Assert.That(protobufCelEnvironment.Program("msg.repeated_string[1]", variables), Is.EqualTo("world"));

                // Test string concatenation with repeated field
                Assert.That(protobufCelEnvironment.Program("msg.repeated_string[0] + ' ' + msg.repeated_string[1]", variables), Is.EqualTo("hello world"));

                // Test membership
                Assert.That(protobufCelEnvironment.Program("'test' in msg.repeated_string", variables), Is.True);
                Assert.That(protobufCelEnvironment.Program("'missing' in msg.repeated_string", variables), Is.False);
            }

            [Test]
            public void Repeated_Nested_Message_Access_Should_Work()
            {
                var testMessage = new TestAllTypes();
                var nested1 = new TestAllTypes.Types.NestedMessage { Bb = 42 };
                var nested2 = new TestAllTypes.Types.NestedMessage { Bb = 100 };
                testMessage.RepeatedNestedMessage.AddRange(new[] { nested1, nested2 });

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test nested field access
                Assert.That(protobufCelEnvironment.Program("msg.repeated_nested_message[0].bb", variables), Is.EqualTo(42));
                Assert.That(protobufCelEnvironment.Program("msg.repeated_nested_message[1].bb", variables), Is.EqualTo(100));

                // Test arithmetic with nested fields
                Assert.That(protobufCelEnvironment.Program("msg.repeated_nested_message[0].bb + msg.repeated_nested_message[1].bb", variables), Is.EqualTo(142L));
            }

            [Test]
            public void Empty_Repeated_Fields_Should_Handle_Correctly()
            {
                var testMessage = new TestAllTypes();
                // Fields are empty by default

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test size of empty repeated field
                Assert.That(protobufCelEnvironment.Program("size(msg.repeated_int32)", variables), Is.EqualTo(0L));

                // Test membership in empty repeated field
                Assert.That(protobufCelEnvironment.Program("1 in msg.repeated_int32", variables), Is.False);
            }
        }

        #endregion

        #region Protobuf Map Field Operations

        [TestFixture]
        public class ProtobufMapFields : ArrayAndRepeatedTests
        {
            [Test]
            public void Map_String_String_Field_Should_Work()
            {
                var testMessage = new TestAllTypes();
                testMessage.MapStringString.Add("key1", "value1");
                testMessage.MapStringString.Add("key2", "value2");
                testMessage.MapStringString.Add("hello", "world");

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test map access
                Assert.That(protobufCelEnvironment.Program("msg.map_string_string['key1']", variables), Is.EqualTo("value1"));
                Assert.That(protobufCelEnvironment.Program("msg.map_string_string['key2']", variables), Is.EqualTo("value2"));
                Assert.That(protobufCelEnvironment.Program("msg.map_string_string['hello']", variables), Is.EqualTo("world"));

                // Test map size
                Assert.That(protobufCelEnvironment.Program("size(msg.map_string_string)", variables), Is.EqualTo(3L));

                // Test key membership (IN operator checks for key existence in maps)
                Assert.That(protobufCelEnvironment.Program("'key1' in msg.map_string_string", variables), Is.True);
                Assert.That(protobufCelEnvironment.Program("'nonexistent_key' in msg.map_string_string", variables), Is.False);
            }

            [Test]
            public void Map_Int32_String_Field_Should_Work()
            {
                var testMessage = new TestAllTypes();
                testMessage.MapInt32String.Add(1, "one");
                testMessage.MapInt32String.Add(2, "two");
                testMessage.MapInt32String.Add(42, "answer");

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test map access with integer keys
                Assert.That(protobufCelEnvironment.Program("msg.map_int32_string[1]", variables), Is.EqualTo("one"));
                Assert.That(protobufCelEnvironment.Program("msg.map_int32_string[2]", variables), Is.EqualTo("two"));
                Assert.That(protobufCelEnvironment.Program("msg.map_int32_string[42]", variables), Is.EqualTo("answer"));
            }

            [Test]
            public void Map_Bool_Int32_Field_Should_Work()
            {
                var testMessage = new TestAllTypes();
                testMessage.MapBoolInt32.Add(true, 100);
                testMessage.MapBoolInt32.Add(false, 200);

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test map access with boolean keys
                Assert.That(protobufCelEnvironment.Program("msg.map_bool_int32[true]", variables), Is.EqualTo(100));
                Assert.That(protobufCelEnvironment.Program("msg.map_bool_int32[false]", variables), Is.EqualTo(200));

                // Test arithmetic with map values
                Assert.That(protobufCelEnvironment.Program("msg.map_bool_int32[true] + msg.map_bool_int32[false]", variables), Is.EqualTo(300L));
            }

            [Test]
            public void Map_String_Nested_Message_Should_Work()
            {
                var testMessage = new TestAllTypes();
                var nested1 = new TestAllTypes.Types.NestedMessage { Bb = 123 };
                var nested2 = new TestAllTypes.Types.NestedMessage { Bb = 456 };
                testMessage.MapStringMessage.Add("first", nested1);
                testMessage.MapStringMessage.Add("second", nested2);

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test nested message field access in map
                Assert.That(protobufCelEnvironment.Program("msg.map_string_message['first'].bb", variables), Is.EqualTo(123));
                Assert.That(protobufCelEnvironment.Program("msg.map_string_message['second'].bb", variables), Is.EqualTo(456));

                // Test arithmetic with nested map values
                Assert.That(protobufCelEnvironment.Program("msg.map_string_message['first'].bb + msg.map_string_message['second'].bb", variables), Is.EqualTo(579L));
            }

            [Test]
            public void Empty_Map_Fields_Should_Handle_Correctly()
            {
                var testMessage = new TestAllTypes();
                // Fields are empty by default

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test size of empty map
                Assert.That(protobufCelEnvironment.Program("size(msg.map_string_string)", variables), Is.EqualTo(0L));
            }
        }

        #endregion

        #region Well-Known Types and Advanced Operations

        [TestFixture]
        public class AdvancedOperations : ArrayAndRepeatedTests
        {
            [Test]
            public void Well_Known_Types_Should_Work()
            {
                var testMessage = new TestAllTypes();
                testMessage.SingleDuration = Duration.FromTimeSpan(TimeSpan.FromSeconds(30));
                testMessage.SingleTimestamp = Timestamp.FromDateTime(DateTime.UtcNow);
                testMessage.SingleValue = Value.ForString("hello world");

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test well-known type field access
                var durationResult = protobufCelEnvironment.Program("msg.single_duration", variables);
                var timestampResult = protobufCelEnvironment.Program("msg.single_timestamp", variables);
                var valueResult = protobufCelEnvironment.Program("msg.single_value", variables);

                Assert.That(durationResult, Is.Not.Null);
                Assert.That(timestampResult, Is.Not.Null);
                Assert.That(valueResult, Is.TypeOf<Value>());
                Assert.That((valueResult as Value)?.StringValue, Is.EqualTo("hello world"));
            }

            [Test]
            public void Mixed_Repeated_And_Map_Operations_Should_Work()
            {
                var testMessage = new TestAllTypes();
                
                // Setup repeated fields
                testMessage.RepeatedInt32.AddRange(new[] { 1, 2, 3 });
                
                // Setup map fields
                testMessage.MapStringInt32.Add("one", 1);
                testMessage.MapStringInt32.Add("two", 2);
                testMessage.MapStringInt32.Add("three", 3);

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test combining repeated and map operations
                Assert.That(protobufCelEnvironment.Program("msg.repeated_int32[0] == msg.map_string_int32[\"one\"]", variables), Is.True);
                Assert.That(protobufCelEnvironment.Program("msg.repeated_int32[1] == msg.map_string_int32[\"two\"]", variables), Is.True);

                // Test size comparisons
                Assert.That(protobufCelEnvironment.Program("size(msg.repeated_int32) == size(msg.map_string_int32)", variables), Is.True);
            }

            [Test]
            public void List_And_Map_Combined_Should_Work()
            {
                var variables = new Dictionary<string, object>
                {
                    ["data"] = new object[] 
                    {
                        new Dictionary<string, object> { ["value"] = 10L },
                        new Dictionary<string, object> { ["value"] = 20L },
                        new Dictionary<string, object> { ["value"] = 30L }
                    }
                };

                var result = basicCelEnvironment.Program("data[1]['value']", variables);
                Assert.That(result, Is.EqualTo(20L));
            }
        }

        #endregion

        #region Conditional Expressions with Arrays and Maps

        [TestFixture]
        public class ConditionalExpressions : ArrayAndRepeatedTests
        {
            [Test]
            public void Conditional_With_Basic_Arrays_Should_Work()
            {
                var variables = new Dictionary<string, object>
                {
                    ["numbers"] = new object[] { 10L, 20L, 30L },
                    ["threshold"] = 15L
                };

                var result = basicCelEnvironment.Program(
                    "numbers[1] > threshold ? 'above' : 'below'", 
                    variables);
                Assert.That(result, Is.EqualTo("above"));
            }

            [Test]
            public void Conditional_With_Protobuf_Fields_Should_Work()
            {
                var testMessage = new TestAllTypes();
                testMessage.SingleInt32 = 42;
                testMessage.RepeatedInt32.AddRange(new[] { 10, 20, 30 });
                testMessage.MapStringInt32.Add("answer", 42);

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                // Test conditional expressions with protobuf fields
                var result = protobufCelEnvironment.Program(
                    "msg.single_int32 == msg.map_string_int32['answer'] ? 'correct' : 'incorrect'", 
                    variables);
                Assert.That(result, Is.EqualTo("correct"));

                // Test conditional with repeated field access
                var result2 = protobufCelEnvironment.Program(
                    "msg.repeated_int32[1] > 15 ? msg.repeated_int32[2] : msg.repeated_int32[0]", 
                    variables);
                Assert.That(result2, Is.EqualTo(30L));
            }

            [Test]
            public void Complex_Nested_Expression_Should_Work()
            {
                var variables = new Dictionary<string, object>
                {
                    ["user"] = new Dictionary<string, object> 
                    { 
                        ["age"] = 25L,
                        ["name"] = "John",
                        ["active"] = true,
                        ["scores"] = new object[] { 85L, 90L, 78L }
                    },
                    ["minAge"] = 18L,
                    ["minScore"] = 80L
                };

                var expression = "user['active'] && user['age'] >= minAge && user['scores'][0] >= minScore ? 'allowed' : 'denied'";
                var result = basicCelEnvironment.Program(expression, variables);
                Assert.That(result, Is.EqualTo("allowed"));
            }
        }

        #endregion

        #region Error Handling

        [TestFixture]
        public class ErrorHandling : ArrayAndRepeatedTests
        {
            [Test]
            public void Array_Index_Out_Of_Range_Should_Throw_Exception()
            {
                var variables = new Dictionary<string, object>
                {
                    ["arr"] = new object[] { 1L, 2L }
                };

                Assert.Throws<CelArgumentRangeException>(() => 
                    basicCelEnvironment.Program("arr[5]", variables));
            }

            [Test]
            public void Protobuf_Field_Index_Out_Of_Range_Should_Throw_Exception()
            {
                var testMessage = new TestAllTypes();
                testMessage.RepeatedInt32.AddRange(new[] { 1, 2 });

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                Assert.Throws<CelArgumentRangeException>(() => 
                    protobufCelEnvironment.Program("msg.repeated_int32[5]", variables));
            }

            [Test]
            public void Map_Key_Not_Found_Should_Throw_Exception()
            {
                var variables = new Dictionary<string, object>
                {
                    ["map"] = new Dictionary<string, object> { ["existing"] = "value" }
                };

                Assert.Throws<CelArgumentRangeException>(() => 
                    basicCelEnvironment.Program("map['nonexistent']", variables));
            }

            [Test]
            public void Protobuf_Map_Key_Not_Found_Should_Throw_Exception()
            {
                var testMessage = new TestAllTypes();
                testMessage.MapStringString.Add("existing", "value");

                var variables = new Dictionary<string, object>
                {
                    ["msg"] = testMessage
                };

                Assert.Throws<CelArgumentRangeException>(() => 
                    protobufCelEnvironment.Program("msg.map_string_string['nonexistent']", variables));
            }

            [Test]
            [TestCase("[1, 2, 3] + 'string'")]
            [TestCase("true * [1, 2]")]
            public void Invalid_Array_Operations_Should_Throw_Exception(string expression)
            {
                Assert.Throws<CelNoSuchOverloadException>(() => 
                    basicCelEnvironment.Program(expression, new Dictionary<string, object>()));
            }
            [Test]
            [TestCase("null[0]")]
            public void Null_Array_Operations_Should_Throw_Exception(string expression)
            {
                Assert.Throws<CelExpressionParserException>(() => 
                    basicCelEnvironment.Program(expression, new Dictionary<string, object>()));
            }
        }

        #endregion
    }
}