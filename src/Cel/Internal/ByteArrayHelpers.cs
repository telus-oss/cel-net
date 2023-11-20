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

using System.Text;
using Google.Protobuf;

namespace Cel.Helpers;

public static class ByteArrayHelpers
{
    private static readonly Encoding Encoding = new UTF8Encoding(false);

    #region Compare

    public static int CompareBytes(ByteString value, object? otherValue)
    {
        if (otherValue is ByteString bytesOtherValue)
        {
            if (value.Length != bytesOtherValue.Length)
            {
                return value.Length.CompareTo(bytesOtherValue.Length);
            }

            for (var i = 0; i < value.Length; i++)
            {
                var compareResult = value[i].CompareTo(bytesOtherValue[i]);
                if (compareResult != 0)
                {
                    return compareResult;
                }
            }

            return 0;
        }

        throw new CelNoSuchOverloadException($"No overload exists to compare bytes to type '{value.GetType().FullName ?? "null"}'.");
    }

    #endregion

    #region Convert

    public static ByteString ConvertBytes(object? value)
    {
        if (value is ByteString bytesValue)
        {
            return bytesValue;
        }

        if (value is string strValue)
        {
            return ConvertBytesString(strValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to convert type '{value?.GetType().FullName ?? "null"}' to bytes.");
    }

    public static ByteString ConvertBytesString(string value)
    {
        return ByteString.CopyFromUtf8(value);
    }

    #endregion

    #region Add

    public static ByteString AddBytes(ByteString value, object? otherValue)
    {
        if (otherValue is ByteString)
        {
            return AddBytesBytes(value, (ByteString)otherValue);
        }


        throw new CelNoSuchOverloadException($"No overload exists to ADD bytes and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static ByteString AddBytesBytes(ByteString a, ByteString b)
    {
        var newArray = new byte[a.Length + b.Length];
        Array.ConstrainedCopy(a.ToByteArray(), 0, newArray, 0, a.Length);
        Array.ConstrainedCopy(b.ToByteArray(), 0, newArray, a.Length, b.Length);
        return ByteString.CopyFrom(newArray);
    }

    #endregion

    #region Size

    public static int SizeBytes(ByteString value)
    {
        return value.Length;
    }

    #endregion
}