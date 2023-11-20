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

namespace Cel.Helpers;

public static class BooleanHelpers
{
    #region Compare

    public static int CompareBool(bool value, object? otherValue)
    {
        if (otherValue == null)
        {
            return -1;
        }

        if (otherValue is bool boolOtherValue)
        {
            if (value == boolOtherValue)
            {
                return 0;
            }

            if (!value && boolOtherValue)
            {
                return -1;
            }

            if (value && !boolOtherValue)
            {
                return 1;
            }
        }

        throw new CelNoSuchOverloadException($"No overload exists to compare boolean and '{otherValue.GetType().FullName}'.");
    }

    #endregion

    #region Convert

    public static bool ConvertBool(object? value)
    {
        if (value is bool boolValue)
        {
            return boolValue;
        }

        throw new CelNoSuchOverloadException($"No overload exists to convert type '{value?.GetType().FullName ?? "null"}' to boolean.");
    }

    #endregion


    #region LogicalNot

    public static bool LogicalNotBool(bool value)
    {
        return !value;
    }

    #endregion
}