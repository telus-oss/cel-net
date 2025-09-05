using System;
using System.Collections.Generic;

namespace Cel.Tests.tests.clr
{
    public class IntegerEdgeCaseTestCases
    {
        public static IEnumerable<ClrTestCase> TestCases()
        {
            // Division by zero tests
            yield return new ClrTestCase
            {
                Name = "clr/int32/division_by_zero",
                Expr = "clrTestData.Int32Value / 0",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                Exception = "CelDivideByZeroException"
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint32/division_by_zero",
                Expr = "clrTestData.UInt32Value / 0u",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 10u } } },
                Exception = "CelDivideByZeroException"
            };

            // Modulus by zero tests
            yield return new ClrTestCase
            {
                Name = "clr/int64/modulus_by_zero",
                Expr = "clrTestData.Int64Value % 0",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = 10L } } },
                Exception = "CelModulusByZeroException"
            };

            // Boundary value tests - maximum values
            yield return new ClrTestCase
            {
                Name = "clr/int32/max_value",
                Expr = "clrTestData.Int32Value + 0",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 2147483647 } } },
                ExpectedResult = 2147483647L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/min_value",
                Expr = "clrTestData.Int32Value + 0",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = -2147483648 } } },
                ExpectedResult = -2147483648L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int64/max_value",
                Expr = "clrTestData.Int64Value + 0",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = 9223372036854775807L } } },
                ExpectedResult = 9223372036854775807L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int64/min_value",
                Expr = "clrTestData.Int64Value + 0",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = -9223372036854775808L } } },
                ExpectedResult = -9223372036854775808L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int16/max_value",
                Expr = "clrTestData.Int16Value + 0",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = 32767 } } },
                ExpectedResult = 32767L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int16/min_value",
                Expr = "clrTestData.Int16Value + 0",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = -32768 } } },
                ExpectedResult = -32768L
            };

            yield return new ClrTestCase
            {
                Name = "clr/sbyte/max_value",
                Expr = "clrTestData.SByteValue + 0",
                Variables = { { "clrTestData", new ClrTestData { SByteValue = 127 } } },
                ExpectedResult = 127L
            };

            yield return new ClrTestCase
            {
                Name = "clr/sbyte/min_value",
                Expr = "clrTestData.SByteValue + 0",
                Variables = { { "clrTestData", new ClrTestData { SByteValue = -128 } } },
                ExpectedResult = -128L
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint32/max_value",
                Expr = "clrTestData.UInt32Value + 0u",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 4294967295u } } },
                ExpectedResult = 4294967295ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint16/max_value",
                Expr = "clrTestData.UInt16Value + 0",
                Variables = { { "clrTestData", new ClrTestData { UInt16Value = 65535 } } },
                ExpectedResult = 65535ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/byte/max_value",
                Expr = "clrTestData.ByteValue + 0",
                Variables = { { "clrTestData", new ClrTestData { ByteValue = 255 } } },
                ExpectedResult = 255ul
            };

            // Zero value tests
            yield return new ClrTestCase
            {
                Name = "clr/int32/zero_addition",
                Expr = "clrTestData.Int32Value + 0",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 0 } } },
                ExpectedResult = 0L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/zero_multiplication",
                Expr = "clrTestData.Int32Value * 1000",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 0 } } },
                ExpectedResult = 0L
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint64/zero_division",
                Expr = "0u / clrTestData.UInt64Value",
                Variables = { { "clrTestData", new ClrTestData { UInt64Value = 1000ul } } },
                ExpectedResult = 0ul
            };

            // Negative zero tests (for signed types)
            yield return new ClrTestCase
            {
                Name = "clr/int32/negative_zero",
                Expr = "-clrTestData.Int32Value",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 0 } } },
                ExpectedResult = 0L
            };

            // One multiplication tests
            yield return new ClrTestCase
            {
                Name = "clr/int32/multiply_by_one",
                Expr = "clrTestData.Int32Value * 1",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 42 } } },
                ExpectedResult = 42L
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint64/multiply_by_one",
                Expr = "clrTestData.UInt64Value * 1u",
                Variables = { { "clrTestData", new ClrTestData { UInt64Value = 9876543210ul } } },
                ExpectedResult = 9876543210ul
            };

            // Division by one tests
            yield return new ClrTestCase
            {
                Name = "clr/int64/divide_by_one",
                Expr = "clrTestData.Int64Value / 1",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = 123456789L } } },
                ExpectedResult = 123456789L
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint32/divide_by_one",
                Expr = "clrTestData.UInt32Value / 1u",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 987654321u } } },
                ExpectedResult = 987654321ul
            };

            // Negative one multiplication tests
            yield return new ClrTestCase
            {
                Name = "clr/int32/multiply_by_negative_one",
                Expr = "clrTestData.Int32Value * (-1)",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 42 } } },
                ExpectedResult = -42L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int64/multiply_by_negative_one",
                Expr = "clrTestData.Int64Value * (-1)",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = -987654321L } } },
                ExpectedResult = 987654321L
            };

            // Complex expressions
            yield return new ClrTestCase
            {
                Name = "clr/complex/multiple_operations",
                Expr = "(clrTestData.Int32Value + 10) * 2 - 5",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 15 } } },
                ExpectedResult = 45L
            };

            yield return new ClrTestCase
            {
                Name = "clr/complex/mixed_signed_unsigned",
                Expr = "clrTestData.Int32Value + clrTestData.UInt32Value",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = -100, UInt32Value = 200u } } },
                ExpectedResult = 100ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/complex/parentheses_precedence",
                Expr = "clrTestData.Int16Value * (clrTestData.SByteValue + 5)",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = 10, SByteValue = 15 } } },
                ExpectedResult = 200L
            };
        }
    }
}