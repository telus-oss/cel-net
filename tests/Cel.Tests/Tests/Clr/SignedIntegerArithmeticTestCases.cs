using System;
using System.Collections.Generic;
using Cel.Tests.Clr;

namespace Cel.Tests.Clr
{
    public class SignedIntegerArithmeticTestCases
    {
        public static IEnumerable<ClrTestCase> TestCases()
        {
            // Int32 (int) arithmetic tests
            yield return new ClrTestCase
            {
                Name = "clr/int32/addition",
                Expr = "clrTestData.Int32Value + 5",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = 15L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/subtraction",
                Expr = "clrTestData.Int32Value - 3",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = 7L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/multiplication",
                Expr = "clrTestData.Int32Value * 3",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 4 } } },
                ExpectedResult = 12L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/division",
                Expr = "clrTestData.Int32Value / 2",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = 5L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/modulus",
                Expr = "clrTestData.Int32Value % 3",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = 1L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/negation",
                Expr = "-clrTestData.Int32Value",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 42 } } },
                ExpectedResult = -42L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int32/negative_addition",
                Expr = "clrTestData.Int32Value + (-5)",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
                ExpectedResult = 5L
            };

            // Int64 (long) arithmetic tests
            yield return new ClrTestCase
            {
                Name = "clr/int64/addition",
                Expr = "clrTestData.Int64Value + 1000000000",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = 5000000000L } } },
                ExpectedResult = 6000000000L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int64/subtraction",
                Expr = "clrTestData.Int64Value - 1000000000",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = 5000000000L } } },
                ExpectedResult = 4000000000L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int64/multiplication",
                Expr = "clrTestData.Int64Value * 2",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = 1000000000L } } },
                ExpectedResult = 2000000000L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int64/division",
                Expr = "clrTestData.Int64Value / 3",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = 9000000000L } } },
                ExpectedResult = 3000000000L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int64/modulus",
                Expr = "clrTestData.Int64Value % 7",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = 22L } } },
                ExpectedResult = 1L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int64/negation",
                Expr = "-clrTestData.Int64Value",
                Variables = { { "clrTestData", new ClrTestData { Int64Value = 9223372036854775807L } } },
                ExpectedResult = -9223372036854775807L
            };

            // Int16 (short) arithmetic tests
            yield return new ClrTestCase
            {
                Name = "clr/int16/addition",
                Expr = "clrTestData.Int16Value + 100",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = 200 } } },
                ExpectedResult = 300L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int16/subtraction",
                Expr = "clrTestData.Int16Value - 50",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = 100 } } },
                ExpectedResult = 50L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int16/multiplication",
                Expr = "clrTestData.Int16Value * 4",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = 25 } } },
                ExpectedResult = 100L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int16/division",
                Expr = "clrTestData.Int16Value / 5",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = 125 } } },
                ExpectedResult = 25L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int16/modulus",
                Expr = "clrTestData.Int16Value % 7",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = 23 } } },
                ExpectedResult = 2L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int16/negation",
                Expr = "-clrTestData.Int16Value",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = 32767 } } },
                ExpectedResult = -32767L
            };

            yield return new ClrTestCase
            {
                Name = "clr/int16/negative_value",
                Expr = "clrTestData.Int16Value + 100",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = -200 } } },
                ExpectedResult = -100L
            };

            // SByte arithmetic tests
            yield return new ClrTestCase
            {
                Name = "clr/sbyte/addition",
                Expr = "clrTestData.SByteValue + 10",
                Variables = { { "clrTestData", new ClrTestData { SByteValue = 50 } } },
                ExpectedResult = 60L
            };

            yield return new ClrTestCase
            {
                Name = "clr/sbyte/subtraction",
                Expr = "clrTestData.SByteValue - 25",
                Variables = { { "clrTestData", new ClrTestData { SByteValue = 100 } } },
                ExpectedResult = 75L
            };

            yield return new ClrTestCase
            {
                Name = "clr/sbyte/multiplication",
                Expr = "clrTestData.SByteValue * 3",
                Variables = { { "clrTestData", new ClrTestData { SByteValue = 20 } } },
                ExpectedResult = 60L
            };

            yield return new ClrTestCase
            {
                Name = "clr/sbyte/division",
                Expr = "clrTestData.SByteValue / 4",
                Variables = { { "clrTestData", new ClrTestData { SByteValue = 80 } } },
                ExpectedResult = 20L
            };

            yield return new ClrTestCase
            {
                Name = "clr/sbyte/modulus",
                Expr = "clrTestData.SByteValue % 7",
                Variables = { { "clrTestData", new ClrTestData { SByteValue = 25 } } },
                ExpectedResult = 4L
            };

            yield return new ClrTestCase
            {
                Name = "clr/sbyte/negation",
                Expr = "-clrTestData.SByteValue",
                Variables = { { "clrTestData", new ClrTestData { SByteValue = 127 } } },
                ExpectedResult = -127L
            };

            yield return new ClrTestCase
            {
                Name = "clr/sbyte/negative_value",
                Expr = "clrTestData.SByteValue + 50",
                Variables = { { "clrTestData", new ClrTestData { SByteValue = -100 } } },
                ExpectedResult = -50L
            };

            // Mixed type operations (signed integers)
            yield return new ClrTestCase
            {
                Name = "clr/mixed/int32_int64_addition",
                Expr = "clrTestData.Int32Value + clrTestData.Int64Value",
                Variables = { { "clrTestData", new ClrTestData { Int32Value = 100, Int64Value = 5000000000L } } },
                ExpectedResult = 5000000100L
            };

            yield return new ClrTestCase
            {
                Name = "clr/mixed/int16_int32_multiplication",
                Expr = "clrTestData.Int16Value * clrTestData.Int32Value",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = 200, Int32Value = 3 } } },
                ExpectedResult = 600L
            };

            yield return new ClrTestCase
            {
                Name = "clr/mixed/sbyte_int16_subtraction",
                Expr = "clrTestData.Int16Value - clrTestData.SByteValue",
                Variables = { { "clrTestData", new ClrTestData { Int16Value = 300, SByteValue = 50 } } },
                ExpectedResult = 250L
            };
        }
    }
}