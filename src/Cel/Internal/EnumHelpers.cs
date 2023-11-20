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

using Google.Protobuf.Reflection;

namespace Cel.Helpers;

public static class EnumHelpers
{
    #region Compare

    public static int CompareEnum(EnumValueDescriptor value, object? otherValue)
    {
        if (otherValue == null)
        {
            return -2;
        }

        if (otherValue is EnumValueDescriptor otherValueEnumValueDescriptor)
        {
            if (value == otherValueEnumValueDescriptor)
            {
                return 0;
            }

            return -2;
        }

        if (otherValue is int otherValueInt)
        {
            if (value.Number == otherValueInt)
            {
                return 0;
            }

            return -2;
        }

        if (otherValue is long otherValueLong)
        {
            if (value.Number == otherValueLong)
            {
                return 0;
            }

            return -2;
        }

        throw new CelNoSuchOverloadException($"Cannot compare type '{value?.GetType().FullName ?? "null"}' to '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    #endregion
}