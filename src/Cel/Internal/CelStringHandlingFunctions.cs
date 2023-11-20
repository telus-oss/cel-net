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

using Cel.Helpers;

namespace Cel.Internal;

public static class CelStringHandlingFunctions
{
    public static void RegisterStringExtensionFunctions(this CelEnvironment celEnvironment)
    {
        celEnvironment.RegisterFunction("charAt", new[] { typeof(string), typeof(long) }, CharAt);
        celEnvironment.RegisterFunction("indexOf", new[] { typeof(string), typeof(string) }, IndexOf);
        celEnvironment.RegisterFunction("indexOf", new[] { typeof(string), typeof(string), typeof(long) }, IndexOf);
        celEnvironment.RegisterFunction("join", new[] { typeof(object?[]) }, Join);
        celEnvironment.RegisterFunction("join", new[] { typeof(object?[]), typeof(string) }, Join);
        celEnvironment.RegisterFunction("lastIndexOf", new[] { typeof(string), typeof(string) }, LastIndexOf);
        celEnvironment.RegisterFunction("lastIndexOf", new[] { typeof(string), typeof(string), typeof(long) }, LastIndexOf);
        celEnvironment.RegisterFunction("replace", new[] { typeof(string), typeof(string), typeof(string) }, Replace);
        celEnvironment.RegisterFunction("split", new[] { typeof(string), typeof(string) }, Split);
        celEnvironment.RegisterFunction("substring", new[] { typeof(string), typeof(long) }, Substring);
        celEnvironment.RegisterFunction("substring", new[] { typeof(string), typeof(long), typeof(long) }, Substring);
        celEnvironment.RegisterFunction("toLowerCase", new[] { typeof(string) }, ToLowerCase);
        celEnvironment.RegisterFunction("toUpperCase", new[] { typeof(string) }, ToUpperCase);
        celEnvironment.RegisterFunction("trim", new[] { typeof(string) }, Trim);
        celEnvironment.RegisterFunction("trimStart", new[] { typeof(string) }, TrimStart);
        celEnvironment.RegisterFunction("trimEnd", new[] { typeof(string) }, TrimEnd);
    }

    private static object? CharAt(object?[] value)
    {
        if (value.Length != 2)
        {
            throw new CelNoSuchOverloadException("CharAt function requires 2 arguments.");
        }

        var index = Int64Helpers.ConvertInt(value[1]);

        if (value[0] is string stringValue)
        {
            if (index < 0 || index >= stringValue.Length)
            {
                throw new CelArgumentRangeException("Argument is out of range for 'charAt' function.");
            }

            return StringHelpers.CharAt(stringValue, index);
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'charAt' function with types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}'.");
    }

    private static object? Substring(object?[] value)
    {
        if (value.Length != 2 && value.Length != 3)
        {
            throw new CelNoSuchOverloadException("Substring function requires 2 or 3 arguments.");
        }

        var startIndex = (int)Int64Helpers.ConvertInt(value[1]);
        int? length = null;
        if (value.Length == 3)
        {
            length = (int)Int64Helpers.ConvertInt(value[2]);
        }

        if (value[0] is string stringValue)
        {
            if (startIndex < 0 || startIndex >= stringValue.Length)
            {
                throw new CelArgumentRangeException("Index argument is out of range for 'substring' function.");
            }

            if (length.HasValue)
            {
                if (length.Value >= stringValue.Length - startIndex)
                {
                    throw new CelArgumentRangeException("Index argument is out of range for 'substring' function.");
                }

                return stringValue.Substring(startIndex, length.Value);
            }

            return stringValue.Substring(startIndex);
        }

        if (value.Length == 3)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'substring' function with types '{value[0]?.GetType().FullName ?? "null"}', '{value[1]?.GetType().FullName ?? "null"}', and '{value[2]?.GetType().FullName ?? "null"}'.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'substring' function with types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}'.");
    }

    private static object? Join(object?[] value)
    {
        if (value.Length != 1 && value.Length != 2)
        {
            throw new CelNoSuchOverloadException("Join function requires 1 or 2 arguments.");
        }

        var separator = string.Empty;

        if (value.Length == 2)
        {
            if (value[1] is string valueString)
            {
                separator = valueString;
            }
            else
            {
                throw new CelNoSuchOverloadException($"No overload exists for 'join' separator argument with type '{value[1]?.GetType().FullName ?? "null"}'.");
            }
        }

        if (value[0] is object?[] valueList)
        {
            if (valueList.Any(c => !(c is string)))
            {
                throw new CelNoSuchOverloadException("No overload exists for list value.");
            }

            return string.Join(separator, valueList);
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'join' function with types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}'.");
    }

    private static object? Split(object?[] value)
    {
        if (value.Length != 2)
        {
            throw new CelNoSuchOverloadException("Split function requires 2 arguments.");
        }

        if (value[1] is string separator)
        {
            //this is good.      
        }
        else
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'split' separator argument with type '{value[1]?.GetType().FullName ?? "null"}'.");
        }

        if (value[0] is string valueString)
        {
            return valueString.Split(new[] { separator }, StringSplitOptions.None).Cast<object?>().ToArray();
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'join' function with types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}'.");
    }

    private static object? Replace(object?[] value)
    {
        if (value.Length != 3)
        {
            throw new CelNoSuchOverloadException("Replace function requires 3 arguments.");
        }

        if (value[1] is string oldString)
        {
            //this is good.      
        }
        else
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'replace' argument with type '{value[1]?.GetType().FullName ?? "null"}'.");
        }

        if (value[2] is string newString)
        {
            //this is good.      
        }
        else
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'replace' argument with type '{value[2]?.GetType().FullName ?? "null"}'.");
        }

        if (value[0] is string valueString)
        {
            return valueString.Replace(oldString, newString);
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'replace' function with types '{value[0]?.GetType().FullName ?? "null"}', '{value[1]?.GetType().FullName ?? "null"}', and '{value[2]?.GetType().FullName ?? "null"}'.");
    }

    private static object? ToUpperCase(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelNoSuchOverloadException("ToUpperCase function requires 1 argument.");
        }

        if (value[0] is string stringValue)
        {
            return stringValue.ToUpper();
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'toUpper' function with type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    private static object? ToLowerCase(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelNoSuchOverloadException("ToLowerCase function requires 1 argument.");
        }

        if (value[0] is string stringValue)
        {
            return stringValue.ToLower();
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'toLowerCase' function with type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    private static object? Trim(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelNoSuchOverloadException("Trim function requires 1 argument.");
        }

        if (value[0] is string stringValue)
        {
            return stringValue.Trim();
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'trim' function with type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    private static object? TrimStart(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelNoSuchOverloadException("TrimStart function requires 1 argument.");
        }

        if (value[0] is string stringValue)
        {
            return stringValue.TrimStart();
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'trimStart' function with type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    private static object? TrimEnd(object?[] value)
    {
        if (value.Length != 1)
        {
            throw new CelNoSuchOverloadException("TrimEnd function requires 1 argument.");
        }

        if (value[0] is string stringValue)
        {
            return stringValue.TrimEnd();
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'trimEnd' function with type '{value[0]?.GetType().FullName ?? "null"}'.");
    }

    private static object? IndexOf(object?[] value)
    {
        if (value.Length != 2 && value.Length != 3)
        {
            throw new CelNoSuchOverloadException("IndexOf function requires 2 or 3 arguments.");
        }


        long? startIndex = null;
        if (value.Length == 3)
        {
            if (value[2] is long valueLong)
            {
                startIndex = valueLong;
            }
            else
            {
                throw new CelNoSuchOverloadException($"No overload exists for 'indexOf' function argument types '{value[2]?.GetType().FullName ?? "null"}'.");
            }
        }

        if (value[1] is string stringSearchString)
        {
            //we expect a string type.
        }
        else
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'indexOf' function argument types '{value[1]?.GetType().FullName ?? "null"}'.");
        }

        if (value[0] is string stringValue1)
        {
            if (startIndex.HasValue)
            {
                if (startIndex.Value < 0 || startIndex.Value >= stringValue1.Length)
                {
                    throw new CelArgumentRangeException("Argument is out of range for 'indexOf' function.");
                }

                return StringHelpers.IndexOf(stringValue1, stringSearchString, startIndex.Value);
            }

            return StringHelpers.IndexOf(stringValue1, stringSearchString, null);
        }

        if (value.Length == 3)
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'indexOf' function with argument types '{value[0]?.GetType().FullName ?? "null"}', '{value[1]?.GetType().FullName ?? "null"}', and '{value[2]?.GetType().FullName ?? "null"}'.");
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'indexOf' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}'.");
    }

    private static object? LastIndexOf(object?[] value)
    {
        if (value.Length != 2 && value.Length != 3)
        {
            throw new CelNoSuchOverloadException("LastIndexOf function requires 2 or 3 arguments.");
        }

        long? startIndex = null;
        if (value.Length == 3)
        {
            if (value[2] is long valueLong)
            {
                startIndex = valueLong;
            }
            else
            {
                throw new CelNoSuchOverloadException($"No overload exists for 'lastIndexOf' function argument types '{value[2]?.GetType().FullName ?? "null"}'.");
            }
        }

        if (value[1] is string stringSearchString)
        {
            //we expect a string type.
        }
        else
        {
            throw new CelNoSuchOverloadException($"No overload exists for 'lastIndexOf' function argument types '{value[1]?.GetType().FullName ?? "null"}'.");
        }

        if (value[0] is string stringValue1)
        {
            if (startIndex.HasValue)
            {
                if (startIndex.Value < 0 || startIndex.Value >= stringValue1.Length)
                {
                    throw new CelArgumentRangeException("Argument is out of range for 'lastIndexOf' function.");
                }

                return StringHelpers.LastIndexOf(stringValue1, stringSearchString, startIndex.Value);
            }

            return StringHelpers.LastIndexOf(stringValue1, stringSearchString, null);
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'lastIndexOf' function with argument types '{value[0]?.GetType().FullName ?? "null"}' and '{value[1]?.GetType().FullName ?? "null"}.");
    }
}