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
using Cel.Helpers;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Enum = System.Enum;

namespace Cel.Internal;

public static class CompareFunctions
{
    public static int Compare(object? value, object? otherValue, TypeRegistry typeRegistry)
    {
        //returns -1, 0, +1 if the values are comparable
        //returns -2 if the values are not equal, but can't be sorted.

        if (value == null && otherValue == null)
        {
            return 0;
        }

        if (value == null && otherValue != null)
        {
            return -1;
        }

        if (value != null && otherValue == null)
        {
            return -1;
        }

        if (value is double doubleValue)
        {
            return DoubleHelpers.CompareDouble(doubleValue, otherValue);
        }

        if (value is float floatValue)
        {
            return DoubleHelpers.CompareDouble(floatValue, otherValue);
        }

        if (value is DoubleValue wrappedDoubleValue)
        {
            return DoubleHelpers.CompareDouble(wrappedDoubleValue.Value, otherValue);
        }

        if (value is FloatValue wrappedFloatValue)
        {
            return DoubleHelpers.CompareDouble(wrappedFloatValue.Value, otherValue);
        }
        
        if (value is decimal decimalValue)
        {
            return DecimalHelpers.CompareDecimal(decimalValue, otherValue);
        }
        
        if (value is long longValue)
        {
            return Int64Helpers.CompareInt(longValue, otherValue);
        }

        if (value is int intValue)
        {
            return Int64Helpers.CompareInt(intValue, otherValue);
        }

        if (value is Int32Value wrappedInt32Value)
        {
            return Int64Helpers.CompareInt(wrappedInt32Value.Value, otherValue);
        }

        if (value is Int64Value wrappedInt64Value)
        {
            return Int64Helpers.CompareInt(wrappedInt64Value.Value, otherValue);
        }

        if (value is uint uintValue)
        {
            return UInt64Helpers.CompareUInt(uintValue, otherValue);
        }

        if (value is ulong ulongValue)
        {
            return UInt64Helpers.CompareUInt(ulongValue, otherValue);
        }

        if (value is UInt32Value wrappedUInt32Value)
        {
            return UInt64Helpers.CompareUInt(wrappedUInt32Value.Value, otherValue);
        }

        if (value is UInt64Value wrappedUInt64Value)
        {
            return UInt64Helpers.CompareUInt(wrappedUInt64Value.Value, otherValue);
        }
        
        if (value is bool boolValue)
        {
            return BooleanHelpers.CompareBool(boolValue, otherValue);
        }

        if (value is BoolValue wrappedBoolValue)
        {
            return BooleanHelpers.CompareBool(wrappedBoolValue.Value, otherValue);
        }

        if (value is ByteString bytesValue)
        {
            return ByteArrayHelpers.CompareBytes(bytesValue, otherValue);
        }

        if (value is BytesValue wrappedBytesValue)
        {
            return ByteArrayHelpers.CompareBytes(wrappedBytesValue.Value, otherValue);
        }

        if (value is DateTimeOffset dateTimeOffsetValue)
        {
            return TimestampHelpers.CompareTimestamp(dateTimeOffsetValue, otherValue);
        }

        if (value is Timestamp timestampValue)
        {
            return TimestampHelpers.CompareTimestamp(timestampValue, otherValue);
        }

        if (value is TimeSpan timeSpanValue)
        {
            return DurationHelpers.CompareDuration(timeSpanValue, otherValue);
        }

        if (value is Duration durationValue)
        {
            return DurationHelpers.CompareDuration(durationValue, otherValue);
        }

        if (value is IList arrayValue)
        {
            return ListHelpers.CompareList(arrayValue, otherValue, typeRegistry);
        }

        if (value is Enum)
        {
            return Int64Helpers.CompareInt((int)value, otherValue);
        }

        if (value is string stringValue)
        {
            return StringHelpers.CompareString(stringValue, otherValue);
        }

        if (value is StringValue wrappedStringValue)
        {
            return StringHelpers.CompareString(wrappedStringValue.Value, otherValue);
        }

        if (value is CelType celTypeValue)
        {
            return TypeHelpers.CompareCelType(celTypeValue, otherValue);
        }

        if (value is IDictionary dictValue)
        {
            return MapHelpers.CompareMap(dictValue, otherValue, typeRegistry);
        }

        if (value is IMessage messageValue)
        {
            return MessageHelpers.CompareMessage(messageValue, otherValue, typeRegistry);
        }

        if (value is EnumValueDescriptor valueEnumValueDescriptor)
        {
            return EnumHelpers.CompareEnum(valueEnumValueDescriptor, otherValue);
        }

        throw new CelNoSuchOverloadException($"Cannot compare type '{value?.GetType().FullName ?? "null"}' to '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static bool Contains(object? value, object? otherValue, TypeRegistry typeRegistry)
    {
        //value is the value we want to find
        //otherValue is the array containing the value

        if (value == null && otherValue == null)
        {
            return false;
        }

        if (otherValue is IList arrayOtherValue)
        {
            for (var i = 0; i < arrayOtherValue.Count; i++)
            {
                if (Compare(value, arrayOtherValue[i], typeRegistry) == 0)
                {
                    return true;
                }
            }

            return false;
        }

        if (otherValue is IDictionary dictOtherValue)
        {
            var key = StringHelpers.ConvertString(value);
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            return dictOtherValue.Contains(key!);
        }

        throw new CelNoSuchOverloadException($"Cannot call contains function because type '{value?.GetType().FullName ?? "null"}' and type '{otherValue?.GetType().FullName ?? "null"}' are incompatible.");
    }
}