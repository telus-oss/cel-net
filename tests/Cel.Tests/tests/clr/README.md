# CLR Arithmetic Test Cases

This directory contains comprehensive test cases for CLR arithmetic operations across all integer types.

## Test Coverage

The test suite covers the following CLR integer types:

### Signed Integer Types
- **int (Int32)**: 32-bit signed integer (-2,147,483,648 to 2,147,483,647)
- **long (Int64)**: 64-bit signed integer (-9,223,372,036,854,775,808 to 9,223,372,036,854,775,807)
- **short (Int16)**: 16-bit signed integer (-32,768 to 32,767)
- **sbyte (SByte)**: 8-bit signed integer (-128 to 127)

### Unsigned Integer Types
- **uint (UInt32)**: 32-bit unsigned integer (0 to 4,294,967,295)
- **ulong (UInt64)**: 64-bit unsigned integer (0 to 18,446,744,073,709,551,615)
- **ushort (UInt16)**: 16-bit unsigned integer (0 to 65,535)
- **byte (Byte)**: 8-bit unsigned integer (0 to 255)

## Arithmetic Operations Tested

For each integer type, the following operations are tested:

- **Addition** (`+`): Adding two values
- **Subtraction** (`-`): Subtracting one value from another
- **Multiplication** (`*`): Multiplying two values
- **Division** (`/`): Dividing one value by another
- **Modulus** (`%`): Finding the remainder of division
- **Negation** (`-`): Unary negation (signed types only)

## Test File Structure

### Core Test Files

- **`SignedIntegerArithmeticTestCases.cs`**: Tests for signed integer types (int, long, short, sbyte)
- **`UnsignedIntegerArithmeticTestCases.cs`**: Tests for unsigned integer types (uint, ulong, ushort, byte)
- **`IntegerEdgeCaseTestCases.cs`**: Edge cases, boundary values, and error conditions
- **`Int32TestCases.cs`**: Extended Int32 tests including comparisons
- **`ComprehensiveArithmeticTestCases.cs`**: Aggregates all test cases for easy discovery

### Supporting Files

- **`ClrTestData.cs`**: Test data class containing properties for all integer types

## Test Categories

### Basic Arithmetic
- Simple operations with typical values
- Mixed-type operations (e.g., int + long)
- Zero value handling

### Boundary Value Testing
- Maximum and minimum values for each type
- Operations at type boundaries
- Overflow conditions

### Edge Cases
- Division by zero
- Modulus by zero
- Zero operands
- Negative operands (for signed types)

### Error Conditions
- Operations that should throw exceptions
- Type conversion edge cases

## Usage

Each test case follows the standard `ClrTestCase` format with:
- **Name**: Descriptive test name following the pattern `clr/{type}/{operation}`
- **Expr**: CEL expression to evaluate
- **Variables**: Test data setup
- **ExpectedResult**: Expected result value
- **Exception**: Expected exception type (for error cases)

## Example Test Case

```csharp
yield return new ClrTestCase
{
    Name = "clr/int32/addition",
    Expr = "clrTestData.Int32Value + 5",
    Variables = { { "clrTestData", new ClrTestData { Int32Value = 10 } } },
    ExpectedResult = 15L
};
```

## Running the Tests

Use the `ComprehensiveArithmeticTestCases.TestCases()` method to get all arithmetic tests, or run individual test case classes as needed.

The `GetTestCoverageSummary()` method provides a breakdown of test coverage by type and operation.