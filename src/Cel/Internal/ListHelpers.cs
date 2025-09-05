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
        if (otherValue is IList)
        {
            return AddListList(value, (IList)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to ADD list and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }
    public static object? AddList(IList value, object? otherValue)
    {
        if (otherValue is IList)
        {
            return AddListList(value, (IList)otherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to ADD list and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }
    public static object?[] AddListList(Array a, IList b)
    {
        var newArray = new object?[a!.Length + b!.Count];
        Array.ConstrainedCopy(a, 0, newArray, 0, a.Length);
        for (int i = 0; i < b.Count; i++)
        {
            newArray[a.Length + i] = b[i];
        }

        return newArray;
    }
    public static List<object> AddListList(IList a, IList b)
    { 
        var list = new List<object>(a.Count + b.Count);
        list.AddRange(a.Cast<object>());
        list.AddRange(b.Cast<object>());
        return list;
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