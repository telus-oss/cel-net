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
using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;

namespace Cel.Helpers;

public static class TypeHelpers
{
    #region Compare

    public static int CompareCelType(CelType value, object? otherValue)
    {
        if (otherValue is CelType otherCelTypeValue)
        {
            return CompareCelTypeCelType(value, otherCelTypeValue);
        }

        throw new CelNoSuchOverloadException($"Cannot compare type '{value?.GetType().FullName ?? "null"}' to '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static int CompareCelTypeCelType(CelType value, CelType otherValue)
    {
        if (value.Equals(otherValue))
        {
            return 0;
        }

        return -1;
    }

    #endregion

    #region Convert

    public static CelType ConvertType(object? arg)
    {
        if (arg == null)
        {
            return new CelType("null_type");
        }

        if (arg is CelType)
        {
            return new CelType("type");
        }

        if (arg is EnumValueDescriptor argEnumValueDescriptor)
        {
            return new CelType(argEnumValueDescriptor.EnumDescriptor.FullName);
        }

        if (arg is bool)
        {
            return new CelType("bool");
        }

        if (arg is ByteString)
        {
            return new CelType("bytes");
        }

        if (arg is ulong)
        {
            return new CelType("uint");
        }

        if (arg is long)
        {
            return new CelType("int");
        }

        if (arg is double)
        {
            return new CelType("double");
        }

        if (arg is string)
        {
            return new CelType("string");
        }

        if (arg is TimeSpan)
        {
            return new CelType("google.protobuf.Duration");
        }

        if (arg is Duration)
        {
            return new CelType("google.protobuf.Duration");
        }

        if (arg is DateTimeOffset)
        {
            return new CelType("google.protobuf.Timestamp");
        }

        if (arg is DateTime)
        {
            return new CelType("google.protobuf.Timestamp");
        }

        if (arg is Timestamp)
        {
            return new CelType("google.protobuf.Timestamp");
        }

        if (arg is IDictionary)
        {
            return new CelType("map");
        }

        if (arg is IEnumerable)
        {
            return new CelType("list");
        }

        if (arg is IMessage argIMessage)
        {
            return new CelType(argIMessage.Descriptor.FullName);
        }

        throw new CelNoSuchOverloadException($"Cannot determine CEL type for object '{arg.GetType().FullName ?? "null"}.");
    }

    #endregion
}