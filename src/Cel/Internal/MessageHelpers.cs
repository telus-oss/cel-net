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

namespace Cel.Internal;

public static class MessageHelpers
{
    #region Compare

    public static int CompareMessage(IMessage value, object? otherValue, TypeRegistry typeRegistry)
    {
        //we have to unpack Any messages before we can compare them.
        if (value is Any valueAny)
        {
            if (!string.IsNullOrWhiteSpace(valueAny.TypeUrl))
            {
                //only unpack if we have a type url.
                value = valueAny.Unpack(typeRegistry);
            }
        }

        if (otherValue is Any otherValueAny)
        {
            if (!string.IsNullOrWhiteSpace(otherValueAny.TypeUrl))
            {
                //only unpack if we have a type url.
                otherValue = otherValueAny.Unpack(typeRegistry);
            }
        }

        if (value == null && otherValue == null)
        {
            return 0;
        }

        if (value == null || otherValue == null)
        {
            return -1;
        }


        if (otherValue is IMessage otherValueMessage)
        {
            return CompareMessageMessage(value, otherValueMessage, typeRegistry);
        }

        throw new CelNoSuchOverloadException($"Cannot compare type '{value?.GetType().FullName ?? "null"}' to '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static int CompareMessageMessage(IMessage value, IMessage otherValue, TypeRegistry typeRegistry)
    {
        if (!value.Descriptor.Equals(otherValue.Descriptor))
        {
            return -1;
        }

        foreach (var fieldDescriptor in value.Descriptor.Fields.InFieldNumberOrder())
        {
            var otherFieldDescriptor = otherValue.Descriptor.Fields[fieldDescriptor.FieldNumber];
            if (otherFieldDescriptor == null)
            {
                return -1;
            }

            if (fieldDescriptor.FullName != otherFieldDescriptor.FullName)
            {
                return -1;
            }

            if (!fieldDescriptor.Equals(otherFieldDescriptor))
            {
                return -1;
            }

            var fieldValue = fieldDescriptor.Accessor.GetValue(value);
            var otherFieldValue = otherFieldDescriptor.Accessor.GetValue(otherValue);

            var compareResult = CompareFunctions.Compare(fieldValue, otherFieldValue, typeRegistry);
            if (compareResult != 0)
            {
                return -1;
            }
        }

        return 0;
    }

    #endregion

    #region Getters and Setters

    public static object? GetMessageValue(IMessage message, string fieldName, bool checkPresence, bool instantiateObject)
    {
        var fieldDescriptor = message.Descriptor.FindFieldByName(fieldName);
        if (fieldDescriptor == null)
        {
            throw new CelNoSuchFieldException($"Could not find field name '{fieldName}' in descriptor for type '{message.Descriptor.Name}'.");
        }

        var value = fieldDescriptor.Accessor.GetValue(message);

        if (value == null && fieldDescriptor.FieldType == FieldType.Message && instantiateObject)
        {
            var fieldMessage = (IMessage?)Activator.CreateInstance(fieldDescriptor.MessageType.ClrType);
            if (fieldMessage == null)
            {
                throw new CelTypeCreationException($"Could not instantiate type '{fieldDescriptor.MessageType.ClrType.FullName}'.");
            }

            return fieldMessage;
        }

        if (checkPresence)
        {
            if (fieldDescriptor.HasPresence)
            {
                if (!fieldDescriptor.Accessor.HasValue(message))
                {
                    return null;
                }
            }
            else
            {
                //if we don't have presence so if the value is the default, then return null 
                if (value is bool valueBool && !valueBool)
                {
                    return null;
                }

                if (value is int valueInt && valueInt == 0)
                {
                    return null;
                }

                if (value is uint valueUInt && valueUInt == 0)
                {
                    return null;
                }

                if (value is long valueLong && valueLong == 0)
                {
                    return null;
                }

                if (value is ulong valueULong && valueULong == 0)
                {
                    return null;
                }

                if (value is float valueFloat && valueFloat == 0)
                {
                    return null;
                }

                if (value is double valueDouble && valueDouble == 0)
                {
                    return null;
                }
            }
        }

        if (fieldDescriptor.FieldType == FieldType.Enum)
        {
            if (value == null)
            {
                return null;
            }

            //we have an enum descriptor for this field.
            var enumValueDescriptor = fieldDescriptor.EnumType.FindValueByNumber((int)value);
            if (enumValueDescriptor == null)
            {
                return value;
            }

            if (checkPresence && !fieldDescriptor.HasPresence && enumValueDescriptor.Number == 0)
            {
                return null;
            }

            return enumValueDescriptor;
        }

        return value;
    }

    public static void SetMessageValues(IMessage message, Dictionary<string, object?> values)
    {
        foreach (var fieldKeyValuePair in values)
        {
            var fieldDescriptor = message.Descriptor.FindFieldByName(fieldKeyValuePair.Key);
            if (fieldDescriptor == null)
            {
                throw new CelNoSuchFieldException($"Could not find field name '{fieldKeyValuePair.Key}' in descriptor for type '{message.Descriptor.Name}'.");
            }

            if (fieldDescriptor.IsMap)
            {
                var map = (IDictionary)fieldDescriptor.Accessor.GetValue(message);

                if (fieldKeyValuePair.Value is Dictionary<string, object?> valueDict)
                {
                    foreach (var valueDictEntry in valueDict)
                    {
                        map[valueDictEntry.Key] = valueDictEntry.Value;
                    }
                }
                else if (fieldKeyValuePair.Value is Struct valueStruct)
                {
                    foreach (var fieldEntry in valueStruct.Fields)
                    {
                        map[fieldEntry.Key] = fieldEntry.Value;
                    }
                }
                else
                {
                    throw new CelNoSuchOverloadException($"Could not set map value on message '{message.GetType().FullName}' field '{fieldKeyValuePair.Key}' because the value was type '{fieldKeyValuePair.Value?.GetType().FullName ?? "null"}'.");
                }
            }
            else if (fieldDescriptor.IsRepeated)
            {
                var list = (IList)fieldDescriptor.Accessor.GetValue(message);

                if (fieldKeyValuePair.Value is object?[] valueList)
                {
                    var mappedValues = valueList.Select(c => ConvertMessageValue(c, fieldDescriptor)).ToArray();

                    foreach (var mappedValue in mappedValues)
                    {
                        list.Add(mappedValue);
                    }
                }
                else
                {
                    throw new CelNoSuchOverloadException($"Could not set map value on message '{message.GetType().FullName}' field '{fieldKeyValuePair.Key}' because the value was type '{fieldKeyValuePair.Value?.GetType().FullName ?? "null"}'.");
                }
            }
            else
            {
                //single value
                var value = ConvertMessageValue(fieldKeyValuePair.Value, fieldDescriptor);
                fieldDescriptor.Accessor.SetValue(message, value);
            }
        }
    }

    private static object? ConvertMessageValue(object? value, FieldDescriptor fieldDescriptor)
    {
        if (fieldDescriptor.FieldType == FieldType.Enum)
        {
            if (value == null)
            {
                //default enum value is 0.
                return 0;
            }

            value = ConvertToInt32(value);
        }

        if (fieldDescriptor.FieldType == FieldType.UInt32
            || fieldDescriptor.FieldType == FieldType.Fixed32)
        {
            value = ConvertToUInt32(value);
        }

        if (fieldDescriptor.FieldType == FieldType.Int32
            || fieldDescriptor.FieldType == FieldType.SInt32
            || fieldDescriptor.FieldType == FieldType.SFixed32)
        {
            value = ConvertToInt32(value);
        }

        if (fieldDescriptor.FieldType == FieldType.Float)
        {
            value = ConvertToFloat(value);
        }

        if (fieldDescriptor.FieldType == FieldType.Message)
        {
            if (fieldDescriptor.MessageType.FullName == "google.protobuf.Any")
            {
                if (value is Any)
                {
                    //do nothing
                }
                else if (value is IMessage valueMessage)
                {
                    value = Any.Pack(valueMessage);
                }
                else
                {
                    value = Any.Pack(ConvertToWellKnownValue(value));
                }
            }
            else if (fieldDescriptor.MessageType.FullName == "google.protobuf.Int32Value")
            {
                //downcast
                value = ConvertToInt32(value);
            }
            else if (fieldDescriptor.MessageType.FullName == "google.protobuf.UInt32Value")
            {
                //downcast
                value = ConvertToUInt32(value);
            }
            else if (fieldDescriptor.MessageType.FullName == "google.protobuf.FloatValue")
            {
                //downcast
                value = ConvertToFloat(value);
            }
            else if (fieldDescriptor.MessageType.FullName == "google.protobuf.Duration")
            {
                if (value is TimeSpan valueTimeSpan)
                {
                    value = Duration.FromTimeSpan(valueTimeSpan);
                }
            }
            else if (fieldDescriptor.MessageType.FullName == "google.protobuf.Timestamp")
            {
                if (value is DateTimeOffset valueDateTimeOffset)
                {
                    value = Timestamp.FromDateTimeOffset(valueDateTimeOffset);
                }
            }
            else if (fieldDescriptor.MessageType.FullName == "google.protobuf.Value")
            {
                value = ConvertToWellKnownValue(value);
            }
            else if (fieldDescriptor.MessageType.FullName == "google.protobuf.ListValue")
            {
                if (value is object[] valueObject)
                {
                    var mappedValues = valueObject.Select(ConvertToWellKnownValue).ToArray();

                    var listValue = new ListValue();
                    listValue.Values.AddRange(mappedValues);

                    value = listValue;
                }
                else
                {
                    throw new CelNoSuchOverloadException($"Could not convert type '{value?.GetType().FullName ?? "null"} to type 'google.protobuf.ListValue'.");
                }
            }
            else if (fieldDescriptor.MessageType.FullName == "google.protobuf.Struct")
            {
                if (value is Dictionary<string, object?> valueDict)
                {
                    var mappedValues = valueDict.ToDictionary(c => c.Key, c => ConvertToWellKnownValue(c.Value));

                    var structValue = new Struct();
                    structValue.Fields.Add(mappedValues);

                    value = structValue;
                }
                else
                {
                    throw new CelNoSuchOverloadException($"Could not convert type '{value?.GetType().FullName ?? "null"} to type 'google.protobuf.Struct'.");
                }
            }
        }

        return value;
    }

    private static Value ConvertToWellKnownValue(object? value)
    {
        if (value == null)
        {
            return Value.ForNull();
        }

        if (value is string valueString)
        {
            return Value.ForString(valueString);
        }

        if (value is bool valueBool)
        {
            return Value.ForBool(valueBool);
        }

        if (value is ByteString valueBytes)
        {
            return Value.ForString(valueBytes.ToStringUtf8());
        }

        if (value is double valueDouble)
        {
            return Value.ForNumber(valueDouble);
        }

        if (value is long valueLong)
        {
            return Value.ForNumber(valueLong);
        }

        if (value is ulong valueULong)
        {
            return Value.ForNumber(valueULong);
        }

        if (value is object[] valueObject)
        {
            var mappedValues = valueObject.Select(c => ConvertToWellKnownValue(c)).ToArray();
            return Value.ForList(mappedValues);
        }

        if (value is Dictionary<string, object?> valueDict)
        {
            var mappedValues = valueDict.ToDictionary(c => c.Key, c => ConvertToWellKnownValue(c.Value));

            var structValue = new Struct();
            structValue.Fields.Add(mappedValues);

            return Value.ForStruct(structValue);
        }

        throw new NotImplementedException($"Could not convert value '{value}' to a protobuf well known value.");
    }

    #endregion

    #region Type Conversion

    public static float ConvertToFloat(object? value)
    {
        var doubleValue = DoubleHelpers.ConvertDouble(value);

        if (doubleValue < float.MinValue)
        {
            return float.NegativeInfinity;
        }

        if (doubleValue > float.MaxValue)
        {
            return float.PositiveInfinity;
        }

        if (double.IsNaN(doubleValue))
        {
            return float.NaN;
        }

        try
        {
            return Convert.ToSingle(value);
        }
        catch
        {
            throw new CelArgumentRangeException($"Value '{value}' cannot be converted to float.");
        }
    }

    public static int ConvertToInt32(object? value)
    {
        if (value is EnumValueDescriptor valueEnumValueDescriptor)
        {
            return valueEnumValueDescriptor.Number;
        }

        var int64Value = Int64Helpers.ConvertInt(value);
        if (int64Value >= int.MinValue && int64Value <= int.MaxValue)
        {
            return Convert.ToInt32(value);
        }

        throw new CelArgumentRangeException($"Value '{value}' cannot be converted to int32.");
    }

    public static uint ConvertToUInt32(object? value)
    {
        var uint64Value = UInt64Helpers.ConvertUInt(value);
        if (uint64Value <= uint.MaxValue)
        {
            return Convert.ToUInt32(value);
        }

        throw new CelArgumentRangeException($"Value '{value}' cannot be converted to uint32.");
    }

    #endregion


    #region Wrapping

    public static object? UnwrapWellKnownMessage(IMessage value)
    {
        if (value is BoolValue boolValue)
        {
            return boolValue.Value;
        }

        if (value is BytesValue bytesValue)
        {
            return bytesValue.Value;
        }

        if (value is Int32Value int32Value)
        {
            return int32Value.Value;
        }

        if (value is Int64Value int64Value)
        {
            return int64Value.Value;
        }

        if (value is UInt32Value uint32Value)
        {
            return uint32Value.Value;
        }

        if (value is UInt64Value uInt64Value)
        {
            return uInt64Value.Value;
        }

        if (value is DoubleValue doubleValue)
        {
            return doubleValue.Value;
        }

        if (value is FloatValue floatValue)
        {
            return floatValue.Value;
        }

        if (value is StringValue stringValue)
        {
            return stringValue.Value;
        }

        if (value is Value wellKnownValue)
        {
            return UnpackWellKnownValue(wellKnownValue);
        }

        return value;
    }

    public static object? UnpackWellKnownValue(Value? wellKnownValue)
    {
        if (wellKnownValue == null)
        {
            return null;
        }

        if (wellKnownValue.KindCase == Value.KindOneofCase.BoolValue)
        {
            return wellKnownValue.BoolValue;
        }

        if (wellKnownValue.KindCase == Value.KindOneofCase.NullValue)
        {
            return wellKnownValue.NullValue;
        }

        if (wellKnownValue.KindCase == Value.KindOneofCase.StructValue)
        {
            var dictionary = new Dictionary<string, object?>();
            foreach (var field in wellKnownValue.StructValue.Fields)
            {
                var key = StringHelpers.ConvertString(field.Key);
                var value = UnpackWellKnownValue(field.Value);
                if (key != null)
                {
                    dictionary[key] = value;
                }
                else
                {
                    throw new CelNoSuchOverloadException("Cannot unpack struct because the key was null.");
                }
            }

            return dictionary;
        }

        if (wellKnownValue.KindCase == Value.KindOneofCase.StringValue)
        {
            return wellKnownValue.StringValue;
        }

        if (wellKnownValue.KindCase == Value.KindOneofCase.NumberValue)
        {
            return wellKnownValue.NumberValue;
        }

        if (wellKnownValue.KindCase == Value.KindOneofCase.ListValue)
        {
            return wellKnownValue.ListValue;
        }

        if (wellKnownValue.KindCase == Value.KindOneofCase.None)
        {
            return null;
        }

        return null;
    }

    #endregion
}