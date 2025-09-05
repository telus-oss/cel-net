using System;
using System.Collections.Generic;

namespace Cel.Tests.tests.clr
{
    public class UnsignedIntegerArithmeticTestCases
    {
        public static IEnumerable<ClrTestCase> TestCases()
        {
            // UInt32 (uint) arithmetic tests
            yield return new ClrTestCase
            {
                Name = "clr/uint32/addition",
                Expr = "clrTestData.UInt32Value + 5u",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 10u } } },
                ExpectedResult = 15ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint32/subtraction",
                Expr = "clrTestData.UInt32Value - 3u",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 10u } } },
                ExpectedResult = 7ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint32/multiplication",
                Expr = "clrTestData.UInt32Value * 3u",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 4u } } },
                ExpectedResult = 12ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint32/division",
                Expr = "clrTestData.UInt32Value / 2u",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 10u } } },
                ExpectedResult = 5ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint32/modulus",
                Expr = "clrTestData.UInt32Value % 3u",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 10u } } },
                ExpectedResult = 1ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint32/large_value",
                Expr = "clrTestData.UInt32Value + 1000000000u",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 3000000000u } } },
                ExpectedResult = 4000000000ul
            };

            // UInt64 (ulong) arithmetic tests
            yield return new ClrTestCase
            {
                Name = "clr/uint64/addition",
                Expr = "clrTestData.UInt64Value + 1000000000u",
                Variables = { { "clrTestData", new ClrTestData { UInt64Value = 5000000000ul } } },
                ExpectedResult = 6000000000ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint64/subtraction",
                Expr = "clrTestData.UInt64Value - 1000000000u",
                Variables = { { "clrTestData", new ClrTestData { UInt64Value = 5000000000ul } } },
                ExpectedResult = 4000000000ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint64/multiplication",
                Expr = "clrTestData.UInt64Value * 2u",
                Variables = { { "clrTestData", new ClrTestData { UInt64Value = 1000000000ul } } },
                ExpectedResult = 2000000000ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint64/division",
                Expr = "clrTestData.UInt64Value / 3u",
                Variables = { { "clrTestData", new ClrTestData { UInt64Value = 9000000000ul } } },
                ExpectedResult = 3000000000ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint64/modulus",
                Expr = "clrTestData.UInt64Value % 7u",
                Variables = { { "clrTestData", new ClrTestData { UInt64Value = 22ul } } },
                ExpectedResult = 1ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint64/max_value",
                Expr = "clrTestData.UInt64Value + 1u",
                Variables = { { "clrTestData", new ClrTestData { UInt64Value = 18446744073709551614ul } } },
                ExpectedResult = 18446744073709551615ul
            };

            // UInt16 (ushort) arithmetic tests
            yield return new ClrTestCase
            {
                Name = "clr/uint16/addition",
                Expr = "clrTestData.UInt16Value + 100",
                Variables = { { "clrTestData", new ClrTestData { UInt16Value = 200 } } },
                ExpectedResult = 300ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint16/subtraction",
                Expr = "clrTestData.UInt16Value - 50",
                Variables = { { "clrTestData", new ClrTestData { UInt16Value = 100 } } },
                ExpectedResult = 50ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint16/multiplication",
                Expr = "clrTestData.UInt16Value * 4",
                Variables = { { "clrTestData", new ClrTestData { UInt16Value = 25 } } },
                ExpectedResult = 100ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint16/division",
                Expr = "clrTestData.UInt16Value / 5",
                Variables = { { "clrTestData", new ClrTestData { UInt16Value = 125 } } },
                ExpectedResult = 25ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint16/modulus",
                Expr = "clrTestData.UInt16Value % 7",
                Variables = { { "clrTestData", new ClrTestData { UInt16Value = 23 } } },
                ExpectedResult = 2ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint16/max_value",
                Expr = "clrTestData.UInt16Value + 1",
                Variables = { { "clrTestData", new ClrTestData { UInt16Value = 65534 } } },
                ExpectedResult = 65535ul
            };

            // Byte arithmetic tests
            yield return new ClrTestCase
            {
                Name = "clr/byte/addition",
                Expr = "clrTestData.ByteValue + 10",
                Variables = { { "clrTestData", new ClrTestData { ByteValue = 50 } } },
                ExpectedResult = 60ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/byte/subtraction",
                Expr = "clrTestData.ByteValue - 25",
                Variables = { { "clrTestData", new ClrTestData { ByteValue = 100 } } },
                ExpectedResult = 75ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/byte/multiplication",
                Expr = "clrTestData.ByteValue * 3",
                Variables = { { "clrTestData", new ClrTestData { ByteValue = 20 } } },
                ExpectedResult = 60ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/byte/division",
                Expr = "clrTestData.ByteValue / 4",
                Variables = { { "clrTestData", new ClrTestData { ByteValue = 80 } } },
                ExpectedResult = 20ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/byte/modulus",
                Expr = "clrTestData.ByteValue % 7",
                Variables = { { "clrTestData", new ClrTestData { ByteValue = 25 } } },
                ExpectedResult = 4ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/byte/max_value",
                Expr = "clrTestData.ByteValue + 1",
                Variables = { { "clrTestData", new ClrTestData { ByteValue = 254 } } },
                ExpectedResult = 255ul
            };

            // Mixed type operations (unsigned integers)
            yield return new ClrTestCase
            {
                Name = "clr/mixed/uint32_uint64_addition",
                Expr = "clrTestData.UInt32Value + clrTestData.UInt64Value",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 100u, UInt64Value = 5000000000ul } } },
                ExpectedResult = 5000000100ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/mixed/uint16_uint32_multiplication",
                Expr = "clrTestData.UInt16Value * clrTestData.UInt32Value",
                Variables = { { "clrTestData", new ClrTestData { UInt16Value = 200, UInt32Value = 3u } } },
                ExpectedResult = 600ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/mixed/byte_uint16_subtraction",
                Expr = "clrTestData.UInt16Value - clrTestData.ByteValue",
                Variables = { { "clrTestData", new ClrTestData { UInt16Value = 300, ByteValue = 50 } } },
                ExpectedResult = 250ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/mixed/byte_uint32_division",
                Expr = "clrTestData.UInt32Value / clrTestData.ByteValue",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 1000u, ByteValue = 10 } } },
                ExpectedResult = 100ul
            };

            // Edge cases and boundary tests
            yield return new ClrTestCase
            {
                Name = "clr/uint32/min_value",
                Expr = "clrTestData.UInt32Value + 0u",
                Variables = { { "clrTestData", new ClrTestData { UInt32Value = 0u } } },
                ExpectedResult = 0ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint64/min_value",
                Expr = "clrTestData.UInt64Value + 0u",
                Variables = { { "clrTestData", new ClrTestData { UInt64Value = 0ul } } },
                ExpectedResult = 0ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/uint16/min_value",
                Expr = "clrTestData.UInt16Value + 0",
                Variables = { { "clrTestData", new ClrTestData { UInt16Value = 0 } } },
                ExpectedResult = 0ul
            };

            yield return new ClrTestCase
            {
                Name = "clr/byte/min_value",
                Expr = "clrTestData.ByteValue + 0",
                Variables = { { "clrTestData", new ClrTestData { ByteValue = 0 } } },
                ExpectedResult = 0ul
            };
        }
    }
}