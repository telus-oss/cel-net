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

public static class MapHelpers
{
    #region Compare

    public static int CompareMap(IDictionary dict, object? otherValue, TypeRegistry typeRegistry)
    {
        if (otherValue == null)
        {
            return -1;
        }

        if (otherValue is IDictionary otherDict)
        {
            var mapEntryCountCompare = dict.Count.CompareTo(otherDict.Count);
            if (mapEntryCountCompare != 0)
            {
                return mapEntryCountCompare;
            }

            foreach (var entryKey in dict.Keys)
            {
                if (entryKey == null)
                {
                    //can't have a null key.
                    return -1;
                }

                var entryValue = dict[entryKey];

                if (otherDict.Contains(entryKey))
                {
                    var otherEntryValue = otherDict[entryKey];

                    try
                    {
                        var itemCompareResult = CompareFunctions.Compare(entryValue, otherEntryValue, typeRegistry);
                        if (itemCompareResult != 0)
                        {
                            return itemCompareResult;
                        }
                    }
                    catch (CelNoSuchOverloadException)
                    {
                        return -1;
                    }
                }
                else
                {
                    //the item wasn't found.
                    return -1;
                }
            }

            return 0;
        }

        throw new CelNoSuchOverloadException($"Could not compare map to type '{otherValue.GetType().FullName}'.");
    }

    #endregion

    #region Size

    public static int SizeMap(IDictionary value)
    {
        return value.Count;
    }

    #endregion
}