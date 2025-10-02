using System;
using System.Collections.Generic;
using System.Linq;
using Cel.Tests.Clr;

namespace Cel.Tests.Clr
{
    public static class SingleTestCases
    {
        public static IEnumerable<ClrTestCase> TestCases()
        {
            return TestCases_Compare()
                    .Union(TestCases_Arithmetic())
                    .Union(TestCases_Convert())
                    .Union(TestCases_Convert_Variable());
        }

        public static IEnumerable<ClrTestCase> TestCases_Compare()
        {
            yield return new ClrTestCase
            {
                Name = "clr/single/negate",
                Expr = "-clrTestData.SingleValue",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } } },
                ExpectedResult = -10.5
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/equality",
                Expr = "clrTestData.SingleValue == value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "value", 10.5f } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/equality_to_int8",
                Expr = "clrTestData.SingleValue == value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", (byte)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/equality_to_int16",
                Expr = "clrTestData.SingleValue == value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", (short)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/equality_to_uint16",
                Expr = "clrTestData.SingleValue == value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", (ushort)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/equality_to_int32",
                Expr = "clrTestData.SingleValue == value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", (int)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/equality_to_uint32",
                Expr = "clrTestData.SingleValue == value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", (uint)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/equality_to_int64",
                Expr = "clrTestData.SingleValue == value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", (long)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/equality_to_uint64",
                Expr = "clrTestData.SingleValue == value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", (ulong)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/equality_to_double",
                Expr = "clrTestData.SingleValue == value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "value", (double)10.5 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/equality_to_decimal",
                Expr = "clrTestData.SingleValue == value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "value", 10.5m } },
                ExpectedResult = true
            };

            yield return new ClrTestCase
            {
                Name = "clr/single/lt_true",
                Expr = "clrTestData.SingleValue < value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", 10.1f } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/lt_to_double_true",
                Expr = "clrTestData.SingleValue < value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", (double)11 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/lt_false",
                Expr = "clrTestData.SingleValue < value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", 9.5f } },
                ExpectedResult = false
            };

            yield return new ClrTestCase
            {
                Name = "clr/single/gt_true",
                Expr = "clrTestData.SingleValue > value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", 9.1f } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/gt_to_double_true",
                Expr = "clrTestData.SingleValue > value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", (double)8 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/gt_false",
                Expr = "clrTestData.SingleValue > value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", 12.5f } },
                ExpectedResult = false
            };
        }

        public static IEnumerable<ClrTestCase> TestCases_Arithmetic()
        {
            yield return new ClrTestCase
            {
                Name = "clr/single/add_int",
                Expr = "clrTestData.SingleValue + int(1) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "expected_value", 11.5 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/subtract_int",
                Expr = "clrTestData.SingleValue - int(1) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "expected_value", 9.5 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/multiply_by_int",
                Expr = "clrTestData.SingleValue * int(2) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "expected_value", 21.0 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/divide_by_int",
                Expr = "clrTestData.SingleValue / int(2) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "expected_value", 5.0 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/divide_by_int_zero",
                Expr = "clrTestData.SingleValue / int(0) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } } },
                Exception = nameof(CelDivideByZeroException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/add_float",
                Expr = "clrTestData.SingleValue + value == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "value", 2.5f }, { "expected_value", 13.0 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/subtract_float",
                Expr = "clrTestData.SingleValue - value == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "value", 1.5f }, { "expected_value", 9.0 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/multiply_by_float",
                Expr = "clrTestData.SingleValue * value == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", 2.5f }, { "expected_value", 25.0 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/divide_by_float",
                Expr = "clrTestData.SingleValue / value == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", 2.0f }, { "expected_value", 5.0 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/divide_by_float_zero",
                Expr = "clrTestData.SingleValue / value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", 0.0f } },
                ExpectedResult = double.NaN
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/add_double",
                Expr = "clrTestData.SingleValue + double(1.5) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "expected_value", 12.0 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/subtract_double",
                Expr = "clrTestData.SingleValue - double(1.5) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "expected_value", 9.0 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/multiply_by_double",
                Expr = "clrTestData.SingleValue * double(2.0) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.5f } }, { "expected_value", 21.0 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/divide_by_double",
                Expr = "clrTestData.SingleValue / double(2.0) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "expected_value", 5.0 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/single/add_by_object",
                Expr = "clrTestData.SingleValue + value",
                Variables = { { "clrTestData", new ClrTestData { SingleValue = 10.0f } }, { "value", new object() } },
                Exception = nameof(CelNoSuchOverloadException)
            };
        }

        public static IEnumerable<ClrTestCase> TestCases_Convert()
        {
            yield return new ClrTestCase
            {
                Name = "convert/single/convert_from_int",
                Expr = "double(int(10))",
                ExpectedResult = 10.0
            };
            yield return new ClrTestCase
            {
                Name = "convert/single/convert_from_uint",
                Expr = "double(uint(10))",
                ExpectedResult = 10.0
            };
            yield return new ClrTestCase
            {
                Name = "convert/single/convert_from_double",
                Expr = "double(double(10.5))",
                ExpectedResult = 10.5
            };
            yield return new ClrTestCase
            {
                Name = "convert/single/convert_from_string",
                Expr = "double(string(\"10.5\"))",
                ExpectedResult = 10.5
            };
        }

        public static IEnumerable<ClrTestCase> TestCases_Convert_Variable()
        {
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_float",
                Expr = "double(value)",
                Variables = { { "value", 10.5f } },
                ExpectedResult = 10.5
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_int8",
                Expr = "double(value)",
                Variables = { { "value", (byte)10 } },
                ExpectedResult = 10.0
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_int16",
                Expr = "double(value)",
                Variables = { { "value", (short)10 } },
                ExpectedResult = 10.0
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_uint16",
                Expr = "double(value)",
                Variables = { { "value", (ushort)10 } },
                ExpectedResult = 10.0
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_int32",
                Expr = "double(value)",
                Variables = { { "value", (int)10 } },
                ExpectedResult = 10.0
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_uint32",
                Expr = "double(value)",
                Variables = { { "value", (uint)10 } },
                ExpectedResult = 10.0
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_int64",
                Expr = "double(value)",
                Variables = { { "value", (long)10 } },
                ExpectedResult = 10.0
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_uint64",
                Expr = "double(value)",
                Variables = { { "value", (ulong)10 } },
                ExpectedResult = 10.0
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_double",
                Expr = "double(value)",
                Variables = { { "value", (double)10.5 } },
                ExpectedResult = 10.5
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_decimal",
                Expr = "double(value)",
                Variables = { { "value", 10.5m } },
                ExpectedResult = 10.5
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_string",
                Expr = "double(value)",
                Variables = { { "value", "10.5" } },
                ExpectedResult = 10.5
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_string_invalid",
                Expr = "double(value)",
                Variables = { { "value", "not_a_double" } },
                Exception = nameof(CelStringParsingException)
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/single/convert_from_object",
                Expr = "double(value)",
                Variables = { { "value", new object() } },
                Exception = nameof(CelNoSuchOverloadException)
            };
        }
    }
}