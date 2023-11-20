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
using Cel.Internal;
using Google.Protobuf.Reflection;

namespace Cel.Helpers;

public static class ListHelpers
{
    #region Compare

    public static int CompareList(IList value, object? otherValue, TypeRegistry typeRegistry)
    {
        if (otherValue == null)
        {
            return -1;
        }

        if (otherValue is IList otherArray)
        {
            var arrayLengthCompare = value.Count.CompareTo(otherArray.Count);
            if (arrayLengthCompare != 0)
            {
                return arrayLengthCompare;
            }

            for (var i = 0; i < value.Count; i++)
            {
                var itemCompareResult = CompareFunctions.Compare(value[i], otherArray[i], typeRegistry);
                if (itemCompareResult != 0)
                {
                    return itemCompareResult;
                }
            }

            return 0;
        }

        throw new CelNoSuchOverloadException($"Could not compare list values with type '{value.GetType()}' to type '{otherValue.GetType()}'.");
    }

    #endregion

    #region Add

    public static object? AddList(Array value, object? otherValue)
    {
        if (otherValue is Array)
        {
            return AddListList(value, (Array)otherValue);
        }


        throw new CelNoSuchOverloadException($"No overload exists to ADD list and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static object?[] AddListList(Array a, Array b)
    {
        var newArray = new object?[a.Length + b.Length];
        Array.ConstrainedCopy(a, 0, newArray, 0, a.Length);
        Array.ConstrainedCopy(b, 0, newArray, a.Length, b.Length);
        return newArray;
    }

    #endregion

    #region Size

    public static int SizeList(object?[] value)
    {
        return value.Length;
    }

    public static int SizeList(IList value)
    {
        return value.Count;
    }

    #endregion
}