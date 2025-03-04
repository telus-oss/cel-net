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

namespace Cel.Internal;

public static class CelMacros
{
    public static void InitializeMacros(Dictionary<string, CelMacroDelegate> macros)
    {
        macros.Add("all", All);
        macros.Add("exists", Exists);
        macros.Add("exists_one", ExistsOne);
        macros.Add("filter", Filter);
        macros.Add("map", Map);
    }

    #region All

    private static object? All(object? value, string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (value is IList valueList)
        {
            return AllList(variableName, predicate, tryGetVariableDelegate, tryGetFunctionWithArgValuesDelegate, valueList);
        }

        if (value is IDictionary<string, object?> valueDict)
        {
            return AllList(variableName, predicate, tryGetVariableDelegate, tryGetFunctionWithArgValuesDelegate, valueDict.Keys.ToList());
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'all' macro with argument type '{value?.GetType().FullName ?? "null"}'.");
    }

    private static object? AllList(string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate, IList valueList)
    {
        var noSuchOverload = false;
        Exception? exception = null;

        for (var i = 0; i < valueList.Count; i++)
        {
            var itemValue = valueList[i];


            //create an inner variable lookup list so that we short circuit the lookup for our dummy variable
            var innerTryGetVariableDelegate = new TryGetVariableDelegate((string c_variableName, out object? c_value) =>
            {
                if (variableName == c_variableName)
                {
                    c_value = itemValue;
                    return true;
                }

                return tryGetVariableDelegate.Invoke(c_variableName, out c_value);
            });

            try
            {
                //evaluate the conditions
                var conditionValue = predicate.Invoke(innerTryGetVariableDelegate);

                //check that we have fields.
                if (conditionValue is CelNoSuchField celNoSuchField)
                {
                    throw new CelNoSuchFieldException(celNoSuchField.Message);
                }

                if (conditionValue is bool conditionValueBool)
                {
                    if (!conditionValueBool)
                    {
                        return false;
                    }
                }
                else
                {
                    noSuchOverload = true;
                }
            }
            catch (CelNoSuchFieldException)
            {
                throw;
            }
            catch (Exception x)
            {
                exception = x;
            }
        }

        if (exception != null)
        {
            return null;
        }

        if (noSuchOverload)
        {
            return null;
        }

        return true;
    }

    #endregion

    #region Exists

    private static object? Exists(object? value, string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (value is IList valueList)
        {
            return ExistsList(variableName, predicate, tryGetVariableDelegate, tryGetFunctionWithArgValuesDelegate, valueList);
        }

        if (value is IDictionary<string, object?> valueDict)
        {
            return ExistsList(variableName, predicate, tryGetVariableDelegate, tryGetFunctionWithArgValuesDelegate, valueDict.Keys.ToList());
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'exists' macro with argument type '{value?.GetType().FullName ?? "null"}'.");
    }

    private static object? ExistsList(string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate, ICollection valueList)
    {
        var matchCount = 0;

        foreach (var itemValue in valueList)
        {
            //create an inner variable lookup list so that we short circuit the lookup for our dummy variable
            var innerTryGetVariableDelegate = new TryGetVariableDelegate((string c_variableName, out object? c_value) =>
            {
                if (variableName == c_variableName)
                {
                    c_value = itemValue;
                    return true;
                }

                return tryGetVariableDelegate.Invoke(c_variableName, out c_value);
            });

            try
            {
                //evaluate the conditions
                var conditionValue = predicate.Invoke(innerTryGetVariableDelegate);

                //check that we have fields.
                if (conditionValue is CelNoSuchField celNoSuchField)
                {
                    throw new CelNoSuchFieldException(celNoSuchField.Message);
                }

                if (conditionValue is bool conditionValueBool)
                {
                    if (conditionValueBool)
                    {
                        matchCount += 1;
                    }
                }
            }
            catch (CelNoSuchOverloadException)
            {
                //do nothing.  this response just isn't a match.
            }
        }

        return matchCount > 0;
    }

    #endregion

    #region Exists

    private static object? ExistsOne(object? value, string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (value is IList valueList)
        {
            return ExistsOneList(variableName, predicate, tryGetVariableDelegate, tryGetFunctionWithArgValuesDelegate, valueList);
        }

        if (value is IDictionary<string, object?> valueDict)
        {
            return ExistsOneList(variableName, predicate, tryGetVariableDelegate, tryGetFunctionWithArgValuesDelegate, valueDict.Keys.ToList());
        }

        throw new CelNoSuchOverloadException($"No overload exists for 'exists_one' macro with argument type '{value?.GetType().FullName ?? "null"}'.");
    }

    private static object? ExistsOneList(string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate, ICollection valueList)
    {
        var noSuchOverload = false;
        Exception? exception = null;
        var matchCount = 0;


        foreach (var itemValue in valueList)
        {
            //create an inner variable lookup list so that we short circuit the lookup for our dummy variable
            var innerTryGetVariableDelegate = new TryGetVariableDelegate((string c_variableName, out object? c_value) =>
            {
                if (variableName == c_variableName)
                {
                    c_value = itemValue;
                    return true;
                }

                return tryGetVariableDelegate.Invoke(c_variableName, out c_value);
            });

            try
            {
                //evaluate the conditions
                var conditionValue = predicate.Invoke(innerTryGetVariableDelegate);

                //check that we have fields.
                if (conditionValue is CelNoSuchField celNoSuchField)
                {
                    throw new CelNoSuchFieldException(celNoSuchField.Message);
                }

                if (conditionValue is bool conditionValueBool)
                {
                    if (conditionValueBool)
                    {
                        matchCount += 1;
                    }
                }
                else
                {
                    noSuchOverload = true;
                }
            }
            catch (CelNoSuchFieldException)
            {
                throw;
            }
            catch (Exception x)
            {
                exception = x;
            }
        }

        if (exception != null)
        {
            return null;
        }

        if (noSuchOverload)
        {
            return null;
        }

        return matchCount == 1;
    }

    #endregion

    #region Filter

    private static object? Filter(object? value, string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (value is object?[] valueList)
        {
            return FilterList(variableName, predicate, tryGetVariableDelegate, tryGetFunctionWithArgValuesDelegate, valueList);
        }

        throw new CelNoSuchOverloadException($"No overload Filter for 'filter' macro with argument type '{value?.GetType().FullName ?? "null"}'.");
    }

    private static object FilterList(string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate, object?[] valueList)
    {
        var output = new List<object?>();

        foreach (var itemValue in valueList)
        {
            //create an inner variable lookup list so that we short circuit the lookup for our dummy variable
            var innerTryGetVariableDelegate = new TryGetVariableDelegate((string c_variableName, out object? c_value) =>
            {
                if (variableName == c_variableName)
                {
                    c_value = itemValue;
                    return true;
                }

                return tryGetVariableDelegate.Invoke(c_variableName, out c_value);
            });

            //evaluate the conditions
            var conditionValue = predicate.Invoke(innerTryGetVariableDelegate);

            //check that we have fields.
            if (conditionValue is CelNoSuchField celNoSuchField)
            {
                throw new CelNoSuchFieldException(celNoSuchField.Message);
            }

            if (conditionValue is bool conditionValueBool)
            {
                if (conditionValueBool)
                {
                    output.Add(itemValue);
                }
            }
            else
            {
                throw new CelExpressionParserException("Predicate expression must return a boolean value.");
            }
        }

        return output.ToArray();
    }

    #endregion

    #region Map

    private static object? Map(object? value, string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate)
    {
        if (predicate == null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        if (value is IEnumerable<object?> valueList)
        {
            return MapList(variableName, predicate, tryGetVariableDelegate, tryGetFunctionWithArgValuesDelegate, valueList);
        }

        throw new CelNoSuchOverloadException($"No overload Map for 'map' macro with argument type '{value?.GetType().FullName ?? "null"}'.");
    }

    private static object? MapList(string variableName, CelExpressionDelegate predicate, TryGetVariableDelegate tryGetVariableDelegate, TryGetFunctionWithArgValuesDelegate tryGetFunctionWithArgValuesDelegate, IEnumerable<object?>? valueList)
    {
        if (valueList == null)
        {
            return null;
        }

        var output = new List<object?>();

        foreach (var itemValue in valueList)
        {
            //create an inner variable lookup list so that we short circuit the lookup for our dummy variable
            var innerTryGetVariableDelegate = new TryGetVariableDelegate((string c_variableName, out object? c_value) =>
            {
                if (variableName == c_variableName)
                {
                    c_value = itemValue;
                    return true;
                }

                return tryGetVariableDelegate.Invoke(c_variableName, out c_value);
            });


            var mappedValue = predicate.Invoke(innerTryGetVariableDelegate);
            output.Add(mappedValue);
        }

        return output.ToArray();
    }

    #endregion
}