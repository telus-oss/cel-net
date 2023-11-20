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

#if NETCOREAPP3_1_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.Collections.Concurrent;
using Cel.Internal;
using Google.Protobuf.WellKnownTypes;
using Type = System.Type;

namespace Cel;

public static class CelExtensions
{
    public static Dictionary<string, object?> UnwrapToDictionary(this Struct value)
    {
        return value.Fields.ToDictionary(c => c.Key, c => c.Value.Unwrap());
    }

    public static object?[] UnwrapToArray(this ListValue value)
    {
        return value.Values.Select(c => c.Unwrap()).ToArray();
    }

    public static object? Unwrap(this Value value)
    {
        if (value.KindCase == Value.KindOneofCase.None)
        {
            return null;
        }

        if (value.KindCase == Value.KindOneofCase.BoolValue)
        {
            return value.BoolValue;
        }

        if (value.KindCase == Value.KindOneofCase.NullValue)
        {
            return null;
        }

        if (value.KindCase == Value.KindOneofCase.NumberValue)
        {
            return value.NumberValue;
        }

        if (value.KindCase == Value.KindOneofCase.StringValue)
        {
            return value.StringValue;
        }

        if (value.KindCase == Value.KindOneofCase.ListValue)
        {
            if (value.ListValue != null)
            {
                return value.ListValue.UnwrapToArray();
            }

            return null;
        }

        if (value.KindCase == Value.KindOneofCase.StructValue)
        {
            if (value.StructValue != null)
            {
                return value.StructValue.UnwrapToDictionary();
            }

            return null;
        }

        throw new NotImplementedException();
    }

    public static void RegisterFunction(this ConcurrentDictionary<string, List<FunctionRegistration>> registry, string functionName, Type[] argTypes, CelFunctionDelegate functionDelegate)
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        if (string.IsNullOrWhiteSpace(functionName))
        {
            throw new ArgumentNullException(nameof(functionName));
        }

        if (argTypes == null)
        {
            throw new ArgumentNullException(nameof(argTypes));
        }

        if (functionDelegate == null)
        {
            throw new ArgumentNullException(nameof(functionDelegate));
        }

        var registration = new FunctionRegistration(argTypes, functionDelegate);


        registry.AddOrUpdate(functionName,
                             key => new List<FunctionRegistration> { registration },
                             (key, value) =>
                             {
                                 //this keeps it concurrent
                                 var copiedList = value.ToList();
                                 copiedList.Insert(0, registration);
                                 return copiedList;
                             });
    }

#if NETCOREAPP3_1_OR_GREATER
    public static bool TryGetFunctionWithArgValues(this ConcurrentDictionary<string, List<FunctionRegistration>> registry, string functionName, object?[] argValues, [NotNullWhen(true)] out CelFunctionDelegate? celFunction)
#else
    public static bool TryGetFunctionWithArgValues(this ConcurrentDictionary<string, List<FunctionRegistration>> registry, string functionName, object?[] argValues, out CelFunctionDelegate? celFunction)
#endif
    {
        var argTypes = argValues.Select(c => c?.GetType() ?? typeof(object)).ToArray();
        return registry.TryGetFunctionWithArgTypes(functionName, argTypes, out celFunction);
    }

#if NETCOREAPP3_1_OR_GREATER
    public static bool TryGetFunctionWithArgTypes(this ConcurrentDictionary<string, List<FunctionRegistration>> registry, string functionName, Type[] argTypes, [NotNullWhen(true)] out CelFunctionDelegate? celFunction)
#else
    public static bool TryGetFunctionWithArgTypes(this ConcurrentDictionary<string, List<FunctionRegistration>> registry, string functionName, Type[] argTypes, out CelFunctionDelegate? celFunction)
#endif
    {
        if (registry == null)
        {
            throw new ArgumentNullException(nameof(registry));
        }

        if (string.IsNullOrWhiteSpace(functionName))
        {
            throw new ArgumentNullException(nameof(functionName));
        }

        if (argTypes == null)
        {
            throw new ArgumentNullException(nameof(argTypes));
        }

        celFunction = null;

        if (!registry.TryGetValue(functionName, out var registrationList))
        {
            return false;
        }

        for (var i = 0; i < registrationList.Count; i++)
        {
            var registration = registrationList[i];

            if (argTypes.Length != registration.ArgsTypes.Length)
            {
                continue;
            }

            var hasMatchingArgs = true;

            for (var j = 0; j < argTypes.Length; j++)
            {
                var expectedArg = argTypes[j];
                var registeredArg = registration.ArgsTypes[j];
                if (!registeredArg.IsAssignableFrom(expectedArg))
                {
                    hasMatchingArgs = false;
                    break;
                }
            }

            if (hasMatchingArgs)
            {
                celFunction = registration.CelFunction;
                return true;
            }
        }

        var typeList = string.Join(", ", argTypes.Select(c => $"'{c.FullName}'"));

        throw new CelNoSuchOverloadException($"No overload exists for '{functionName}' function argument types {typeList}.");
    }
}