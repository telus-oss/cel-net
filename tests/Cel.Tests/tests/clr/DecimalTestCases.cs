using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cel.Tests.tests.clr
{
    public static class DecimalTestCases
    {
        private static readonly double DoubleLessThanDecimalMin = ((double)decimal.MinValue) * 10;
        private static readonly double DoubleMoreThanDecimalMax = ((double)decimal.MaxValue) * 10;


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
                Name = "clr/decimal/negate",
                Expr = "-clrTestData.DecimalValue",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } } },
                ExpectedResult = -10.0M
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", 10.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_int8",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (byte)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_int16",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (short)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_uint16",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (ushort)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_int32",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (int)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_uint32",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (uint)10 } },
                ExpectedResult = true
            };

            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_int64",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (long)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_int64_min",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = long.MinValue } }, { "value", long.MinValue } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_int64_max",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = long.MaxValue } }, { "value", long.MaxValue } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_uint64",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (ulong)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_uint64_min",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = ulong.MinValue } }, { "value", ulong.MinValue } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_uint64_max",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = ulong.MaxValue } }, { "value", ulong.MaxValue } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_double",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (double)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_double_min",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } }, { "value", Convert.ToDouble(decimal.MinValue) } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_double_max",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } }, { "value", Convert.ToDouble(decimal.MaxValue) } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_float",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (float)10 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_float_min",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } }, { "value", Convert.ToSingle(decimal.MinValue) } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/equality_to_float_max",
                Expr = "clrTestData.DecimalValue == value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } }, { "value", Convert.ToSingle(decimal.MaxValue) } },
                ExpectedResult = true
            };

            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", 10.1m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int8_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (byte)11 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int16_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (short)11 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_uint16_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (ushort)11 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int32_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (int)11 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_uint32_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (uint)11 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int64_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (long)11 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int64_min_minus_one_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = (decimal)long.MinValue - 1 } }, { "value", long.MinValue } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int64_max_plus_one_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = (decimal)long.MaxValue + 1 } }, { "value", long.MaxValue } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int64_min_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = (decimal)long.MinValue } }, { "value", long.MinValue } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int64_max_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = (decimal)long.MaxValue } }, { "value", long.MaxValue } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_uint64_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (ulong)11 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_double_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (double)11 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_float_true",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (float)11 } },
                ExpectedResult = true
            };


            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", 9.5m } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int8_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (byte)9 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int16_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (short)9 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_uint16_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (ushort)9 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int32_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (int)9 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_uint32_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (uint)9 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_int64_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (long)9 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_uint64_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (ulong)9 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_double_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (double)9 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_float_false",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (float)9 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_double_larger_than_max",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } }, { "value", DoubleMoreThanDecimalMax } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_double_equal_to_max",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } }, { "value", Convert.ToDouble(decimal.MaxValue) } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_double_larger_than_max",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 0 } }, { "value", DoubleMoreThanDecimalMax } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_double_larger_than_min",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 0 } }, { "value", DoubleLessThanDecimalMin } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_double_larger_than_min",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } }, { "value", DoubleLessThanDecimalMin } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/lt_to_double_equal_to_min",
                Expr = "clrTestData.DecimalValue < value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } }, { "value", Convert.ToDouble(decimal.MinValue) } },
                ExpectedResult = false
            };

            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_true",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", 9.1m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_int8_true",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (byte)8 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_int16_true",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (short)8 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_uint16_true",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (ushort)8 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_int32_true",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (int)8 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_uint32_true",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (uint)8 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_int64_true",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (long)8 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_uint64_true",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (ulong)8 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_double_true",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (double)8 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_float_true",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (float)8 } },
                ExpectedResult = true
            };


            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_false",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", 12.5m } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_int8_false",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (byte)12 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_int16_false",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (short)12 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_uint16_false",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (ushort)12 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_int32_false",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (int)12 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_uint32_false",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (uint)12 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_int64_false",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (long)12 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_uint64_false",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (ulong)12 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_double_false",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (double)12 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_float_false",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (float)12 } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_double_larger_than_max",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } }, { "value", DoubleMoreThanDecimalMax } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_double_equal_to_max",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } }, { "value", Convert.ToDouble(decimal.MaxValue) } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_double_larger_than_max",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 0 } }, { "value", DoubleMoreThanDecimalMax } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_double_larger_than_min",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 0 } }, { "value", DoubleLessThanDecimalMin } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_double_larger_than_min",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } }, { "value", DoubleLessThanDecimalMin } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_double_equal_to_min",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } }, { "value", Convert.ToDouble(decimal.MinValue) } },
                ExpectedResult = false
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/gt_to_object",
                Expr = "clrTestData.DecimalValue > value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10.0m } }, { "value", new object() } },
                Exception = nameof(CelNoSuchOverloadException)
            };
        }
        public static IEnumerable<ClrTestCase> TestCases_Arithmetic()
        {

            yield return new ClrTestCase
            {
                Name = "clr/decimal/add_int",
                Expr = "clrTestData.DecimalValue + int(1) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 11.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/subtract_int",
                Expr = "clrTestData.DecimalValue - int(1) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 9.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/multiply_by_int",
                Expr = "clrTestData.DecimalValue * int(10) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 100.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_int",
                Expr = "clrTestData.DecimalValue / int(5) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 2.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_int_zero",
                Expr = "clrTestData.DecimalValue / int(0) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } } },
                Exception = nameof(CelDivideByZeroException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/modulus_by_int",
                Expr = "clrTestData.DecimalValue % int(3) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 1 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/add_uint",
                Expr = "clrTestData.DecimalValue + uint(1) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 11.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/subtract_uint",
                Expr = "clrTestData.DecimalValue - uint(1) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 9.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/multiply_by_uint",
                Expr = "clrTestData.DecimalValue * uint(10) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 100.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_uint",
                Expr = "clrTestData.DecimalValue / uint(5) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 2.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_uint_zero",
                Expr = "clrTestData.DecimalValue / uint(0) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } } },
                Exception = nameof(CelDivideByZeroException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/modulus_by_uint",
                Expr = "clrTestData.DecimalValue % uint(3) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 1 } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/add_double",
                Expr = "clrTestData.DecimalValue + double(1) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 11.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/add_double_max_range_overflow",
                Expr = "clrTestData.DecimalValue + double(1)",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } } },
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/add_double_min_range_overflow",
                Expr = "clrTestData.DecimalValue + double(-1)",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } } },
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/subtract_double",
                Expr = "clrTestData.DecimalValue - double(1) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 9.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/subtract_double_max_range_overflow",
                Expr = "clrTestData.DecimalValue - double(-1)",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } } },
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/subtract_double_min_range_overflow",
                Expr = "clrTestData.DecimalValue - double(1)",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } } },
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/multiply_by_double",
                Expr = "clrTestData.DecimalValue * double(10) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 100.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/multiply_by_double_max_overflow",
                Expr = "clrTestData.DecimalValue * double(10)",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } } },
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/multiply_by_double_min_overflow",
                Expr = "clrTestData.DecimalValue * double(10)",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } } },
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_double",
                Expr = "clrTestData.DecimalValue / double(5) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 2.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_double_zero",
                Expr = "clrTestData.DecimalValue / double(0) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } } },
                Exception = nameof(CelDivideByZeroException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/add_float",
                Expr = "clrTestData.DecimalValue + value == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", (float)2.0 }, { "expected_value", 12.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/add_float_max_range_overflow",
                Expr = "clrTestData.DecimalValue + float_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue }}, { "float_value", (float)1.0 } },
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/add_float_min_range_overflow",
                Expr = "clrTestData.DecimalValue + float_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } } , { "float_value", (float)-1.0 } },
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/subtract_float",
                Expr = "clrTestData.DecimalValue - float_value == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } },  { "float_value", (float)1.0 },{ "expected_value", 9.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/subtract_float_max_range_overflow",
                Expr = "clrTestData.DecimalValue - float_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } },  { "float_value", (float)-1.0 } },
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/subtract_float_min_range_overflow",
                Expr = "clrTestData.DecimalValue - float_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } } ,  { "float_value", (float)1.0 }},
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/multiply_by_float",
                Expr = "clrTestData.DecimalValue * float_value == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } },  { "float_value", (float)10.0 }, { "expected_value", 100.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/multiply_by_float_max_overflow",
                Expr = "clrTestData.DecimalValue * float_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MaxValue } },  { "float_value", (float)10.0 } },
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/multiply_by_float_min_overflow",
                Expr = "clrTestData.DecimalValue * float_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = decimal.MinValue } } ,  { "float_value", (float)10.0 }},
                Exception = nameof(CelOverflowException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_float",
                Expr = "clrTestData.DecimalValue / float_value == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } },  { "float_value", (float)5.0 }, { "expected_value", 2.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_float_zero",
                Expr = "clrTestData.DecimalValue / float_value == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } } ,  { "float_value", (float)0.0 }},
                Exception = nameof(CelDivideByZeroException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/add_decimal",
                Expr = "clrTestData.DecimalValue + decimal(1) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 11.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/subtract_decimal",
                Expr = "clrTestData.DecimalValue - decimal(1) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 9.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/multiply_by_decimal",
                Expr = "clrTestData.DecimalValue * decimal(10) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 100.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_decimal",
                Expr = "clrTestData.DecimalValue / decimal(5) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 2.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_decimal_zero",
                Expr = "clrTestData.DecimalValue / decimal(0) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } } },
                Exception = nameof(CelDivideByZeroException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/modulus_by_decimal",
                Expr = "clrTestData.DecimalValue % decimal(3) == expected_value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "expected_value", 1 } },
                Exception = nameof(CelNoSuchOverloadException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/add_by_object",
                Expr = "clrTestData.DecimalValue + value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", new object() } },
                Exception = nameof(CelNoSuchOverloadException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/subtract_by_object",
                Expr = "clrTestData.DecimalValue - value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", new object() } },
                Exception = nameof(CelNoSuchOverloadException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/multiply_by_object",
                Expr = "clrTestData.DecimalValue * value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", new object() } },
                Exception = nameof(CelNoSuchOverloadException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_object",
                Expr = "clrTestData.DecimalValue / value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", new object() } },
                Exception = nameof(CelNoSuchOverloadException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/divide_by_object",
                Expr = "clrTestData.DecimalValue / value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", new object() } },
                Exception = nameof(CelNoSuchOverloadException)
            };
            yield return new ClrTestCase
            {
                Name = "clr/decimal/modulus_by_object",
                Expr = "clrTestData.DecimalValue % value",
                Variables = { { "clrTestData", new ClrTestData { DecimalValue = 10 } }, { "value", new object() } },
                Exception = nameof(CelNoSuchOverloadException)
            };
        }
        public static IEnumerable<ClrTestCase> TestCases_Convert()
        {
            yield return new ClrTestCase
            {
                Name = "compare/decimal/convert_from_int",
                Expr = "decimal(int(10)) == expected_value",
                Variables = { { "expected_value", 10.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "compare/decimal/convert_from_uint",
                Expr = "decimal(uint(10)) == expected_value",
                Variables = { { "expected_value", 10.0m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "compare/decimal/convert_from_uint",
                Expr = "decimal(double(10.5)) == expected_value",
                Variables = { { "expected_value", 10.5m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "compare/decimal/convert_from_double",
                Expr = "decimal(double(10.5)) == expected_value",
                Variables = { { "expected_value", 10.5m } },
                ExpectedResult = true
            };
            yield return new ClrTestCase
            {
                Name = "compare/decimal/convert_from_string",
                Expr = "decimal(string(\"10.5\")) == expected_value",
                Variables = { { "expected_value", 10.5m } },
                ExpectedResult = true
            };

            yield return new ClrTestCase
            {
                Name = "convert/decimal/convert_from_int",
                Expr = "decimal(int(10))",
                ExpectedResult = 10.0m
            };
            yield return new ClrTestCase
            {
                Name = "convert/decimal/convert_from_uint",
                Expr = "decimal(uint(10))",
                ExpectedResult = 10.0m
            };
            yield return new ClrTestCase
            {
                Name = "convert/decimal/convert_from_uint",
                Expr = "decimal(double(10.5))",
                ExpectedResult = 10.5m
            };
            yield return new ClrTestCase
            {
                Name = "convert/decimal/convert_from_double",
                Expr = "decimal(double(10.5))",
                ExpectedResult = 10.5m
            };
            yield return new ClrTestCase
            {
                Name = "convert/decimal/convert_from_string",
                Expr = "decimal(string(\"10.5\"))",
                ExpectedResult = 10.5m
            };
        }
        public static IEnumerable<ClrTestCase> TestCases_Convert_Variable()
        {
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_decimal",
                Expr = "decimal(value)",
                Variables = { { "value", (decimal)10 } },
                ExpectedResult = 10.0m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_int8",
                Expr = "decimal(value)",
                Variables = { { "value", (byte)10 } },
                ExpectedResult = 10.0m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_int16",
                Expr = "decimal(value)",
                Variables = { { "value", (short)10 } },
                ExpectedResult = 10.0m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_uint16",
                Expr = "decimal(value)",
                Variables = { { "value", (ushort)10 } },
                ExpectedResult = 10.0m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_int32",
                Expr = "decimal(value)",
                Variables = { { "value", (int)10 } },
                ExpectedResult = 10.0m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_uint32",
                Expr = "decimal(value)",
                Variables = { { "value", (uint)10 } },
                ExpectedResult = 10.0m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_int64",
                Expr = "decimal(value)",
                Variables = { { "value", (long)10 } },
                ExpectedResult = 10.0m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_uint64",
                Expr = "decimal(value)",
                Variables = { { "value", (ulong)10 } },
                ExpectedResult = 10.0m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_double",
                Expr = "decimal(value)",
                Variables = { { "value", (double)10.5 } },
                ExpectedResult = 10.5m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_float",
                Expr = "decimal(value)",
                Variables = { { "value", (float)10.5 } },
                ExpectedResult = 10.5m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_string",
                Expr = "decimal(value)",
                Variables = { { "value", "10.5" } },
                ExpectedResult = 10.5m
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_string_invalid",
                Expr = "decimal(value)",
                Variables = { { "value", "not_a_decimal" } },
                Exception = nameof(CelStringParsingException)
            };
            yield return new ClrTestCase
            {
                Name = "convert_variable/decimal/convert_from_object",
                Expr = "decimal(value)",
                Variables = { { "value", new object() } },
                Exception = nameof(CelNoSuchOverloadException)
            };
        }
    }
}
