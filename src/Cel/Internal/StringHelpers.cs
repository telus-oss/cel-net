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

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace Cel.Helpers;

public static class StringHelpers
{
    public static Encoding Encoding = new UTF8Encoding(false, true);

    #region String Parsing

    /// <summary>
    ///     Unescape takes a quoted string, unquotes, and unescapes it.
    ///     This function performs escaping compatible with GoogleSQL.
    /// </summary>
    public static object Unescape(string value, bool isBytes)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        // All strings normalize newlines to the \n representation.
        value = value.Replace("\r\n", "\n").Replace("\r", "\n");

        if (value.Length < 2)
        {
            throw new CelStringParsingException("Unable to unescape string.");
        }

        // Raw string preceded by the 'r|R' prefix.
        var isRawLiteral = false;

        if (value[0] == 'r' || value[0] == 'R')
        {
            value = value.Substring(1);
            isRawLiteral = true;
        }

        // Quoted string of some form, must have same first and last char.

        if (value[0] != value[value.Length - 1] || (value[0] != '"' && value[0] != '\''))
        {
            throw new CelStringParsingException("Unable to unescape string.");
        }


        // Normalize the multi-line CEL string representation to a standard
        // Go quoted string.
        if (value.Length >= 6)
        {
            if (value.StartsWith("'''", StringComparison.Ordinal))
            {
                if (!value.EndsWith("'''", StringComparison.Ordinal))
                {
                    throw new CelStringParsingException("Unable to unescape string.");
                }

                value = "\"" + value.Substring(3, value.Length - 6) + "\"";
            }

            else if (value.StartsWith("\"\"\"", StringComparison.Ordinal))
            {
                if (!value.EndsWith("\"\"\"", StringComparison.Ordinal))
                {
                    throw new CelStringParsingException("Unable to unescape string.");
                }

                value = "\"" + value.Substring(3, value.Length - 6) + "\"";
            }
        }

        value = value.Substring(1, value.Length - 2);

        // If there is nothing to escape, then return.
        if (isRawLiteral || !value.Contains('\\'))
        {
            if (isBytes)
            {
                //we need a byte array back
                return ByteString.CopyFromUtf8(value);
            }

            return value;
        }


        using (var ms = new MemoryStream())
        {
            while (value.Length > 0)
            {
                var (decodedChar, tail, encode) = UnescapeChar(value, isBytes);
                value = tail;

                decodedChar.WriteTo(ms);
            }

            if (isBytes)
            {
                //return a byte array
                return ByteString.CopyFrom(ms.ToArray());
            }

            try
            {
                //return a string
                return Encoding.UTF8.GetString(ms.ToArray());
            }
            catch (Exception x)
            {
                throw new CelStringParsingException("Unable to unescape string.", x);
            }
        }
    }

    public static (ByteString results, string tail, bool encode) UnescapeChar(string s, bool isBytes)
    {
        var encode = false;
        ByteString result;

        if (s[0] != '\\')
        {
            result = ByteString.CopyFromUtf8(s.Substring(0, 1));
            return (result, s.Substring(1), false);
        }

        if (s.Length <= 1)
        {
            throw new CelStringParsingException("Unable to unescape string.  Found '\' as last character.");
        }

        var v = 0;
        var n = s.Length;
        var c = s[1];
        var tail = s.Substring(2);

        switch (c)
        {
            case 'a':
                result = ByteString.CopyFromUtf8("\a");
                break;
            case 'b':
                result = ByteString.CopyFromUtf8("\b");
                break;
            case 'f':
                result = ByteString.CopyFromUtf8("\f");
                break;
            case 'n':
                result = ByteString.CopyFromUtf8("\n");
                break;
            case 'r':
                result = ByteString.CopyFromUtf8("\r");
                break;
            case 't':
                result = ByteString.CopyFromUtf8("\t");
                break;
            case 'v':
                result = ByteString.CopyFromUtf8("\v");
                break;
            case '\\':
                result = ByteString.CopyFromUtf8("\\");
                break;
            case '\'':
                result = ByteString.CopyFromUtf8("\'");
                break;
            case '"':
                result = ByteString.CopyFromUtf8("\"");
                break;
            case '`':
                result = ByteString.CopyFromUtf8("`");
                break;
            case '?':
                result = ByteString.CopyFromUtf8("?");
                break;

            // 4. Unicode escape sequences, reproduced from `strconv/quote.go`
            case 'x':
            case 'X':
            case 'u':
            case 'U':
                n = 0;
                encode = true;
                switch (c)
                {
                    case 'x':
                    case 'X':
                        n = 2;
                        encode = !isBytes;
                        break;
                    case 'u':
                        n = 4;
                        if (isBytes)
                        {
                            throw new CelStringParsingException("Unable to unescape string.");
                        }

                        break;
                    case 'U':
                        n = 8;
                        if (isBytes)
                        {
                            throw new CelStringParsingException("Unable to unescape string.");
                        }

                        break;
                }

                if (s.Length < n)
                {
                    throw new CelStringParsingException("Unable to unescape string.");
                }

                for (var j = 0; j < n; j++)
                {
                    //add 2 so that we skip the /u character.
                    var x = Unhex(s[j + 2]);
                    if (!x.HasValue)
                    {
                        throw new CelStringParsingException("Unable to unescape string.");
                    }

                    v = (v << 4) | x.Value;
                }

                tail = s.Substring(n + 2);

#if NETCOREAPP3_1_OR_GREATER
                if (!isBytes && !Rune.IsValid(v))
                {
                    throw new CelStringParsingException("Unable to unescape string.");
                }
#endif

                if (isBytes)
                {
                    //we used /x or /X
                    result = ByteString.CopyFrom((byte)v);
                }
                else
                {
                    //we used /x, /X, /u or /U
                    result = ByteString.CopyFromUtf8(char.ConvertFromUtf32(v));
                }

                break;
            case '0':
            case '1':
            case '2':
            case '3':
                // 5. Octal escape sequences, must be three digits \[0-3][0-7][0-7]
                if (s.Length < 2)
                {
                    throw new CelStringParsingException("Unable to unescape octal sequence in string.");
                }

                v = 0;
                n = 3;
                for (var j = 0; j < n; j++)
                {
                    var x = s[j + 1];
                    if (x < '0' || x > '7')
                    {
                        throw new CelStringParsingException("Unable to unescape octal sequence in string.");
                    }

                    v = v * 8 + (x - '0');
                }

#if NETCOREAPP3_1_OR_GREATER
                if (!isBytes && !Rune.IsValid(v))
                {
                    throw new CelStringParsingException("Unable to unescape octal sequence in string.");
                }
#endif

                if (isBytes)
                {
                    result = ByteString.CopyFrom((byte)v);
                }
                else
                {
                    result = ByteString.CopyFromUtf8(char.ConvertFromUtf32(v));
                }

                tail = s.Substring(3 + 1);
                encode = !isBytes;
                break;
            default:
                // Unknown escape sequence.
                throw new CelStringParsingException("Unable to unescape string.");
        }

        return (result, tail, encode);
    }

    private static int? Unhex(char c)
    {
        if ('0' <= c && c <= '9')
        {
            return c - '0';
        }

        if ('a' <= c && c <= 'f')
        {
            return c - 'a' + 10;
        }

        if ('A' <= c && c <= 'F')
        {
            return c - 'A' + 10;
        }

        return null;
    }

    #endregion

    #region Compare

    public static int CompareString(string value, object? otherValue)
    {
        if (otherValue == null)
        {
            return -1;
        }

        if (otherValue is double doubleOtherValue)
        {
            return CompareStringDouble(value, doubleOtherValue);
        }

        if (otherValue is long longOtherValue)
        {
            return CompareStringInt(value, longOtherValue);
        }

        if (otherValue is ulong ulongOtherValue)
        {
            return CompareStringUInt(value, ulongOtherValue);
        }

        if (otherValue is string stringOtherValue)
        {
            return CompareStringString(value, stringOtherValue);
        }

        if (otherValue is ByteString byteStringOtherValue)
        {
            return CompareStringBytes(value, byteStringOtherValue);
        }

        throw new CelNoSuchOverloadException($"Cannot compare type '{value?.GetType().FullName ?? "null"}' to '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static int CompareStringDouble(string value, double otherValue)
    {
        return -2;
    }

    public static int CompareStringInt(string value, long otherValue)
    {
        return -2;
    }

    public static int CompareStringUInt(string value, ulong otherValue)
    {
        return -2;
    }

    public static int CompareStringString(string value, string otherValue)
    {
        return string.Compare(value, otherValue, StringComparison.Ordinal);
    }

    public static int CompareStringBytes(string value, ByteString otherValue)
    {
        var otherString = otherValue.ToStringUtf8();

        return string.Compare(value, otherString, StringComparison.Ordinal);
    }

    #endregion

    #region Convert

    public static string? ConvertString(object? value)
    {
        if (value is string strValue)
        {
            return strValue;
        }

        if (value is bool boolValue)
        {
            return ConvertStringBool(boolValue);
        }

        if (value is ByteString bytesValue)
        {
            return ConvertStringBytes(bytesValue);
        }

        if (value is double doubleValue)
        {
            return ConvertStringDouble(doubleValue);
        }

        if (value is long longValue)
        {
            return ConvertStringInt(longValue);
        }

        if (value is ulong ulongValue)
        {
            return ConvertStringUInt(ulongValue);
        }

        if (value is Timestamp timestampValue)
        {
            return ConvertStringTimestamp(timestampValue);
        }

        if (value is DateTimeOffset dateTimeOffsetValue)
        {
            return ConvertStringTimestamp(dateTimeOffsetValue);
        }

        if (value is DateTime dateTimeValue)
        {
            return ConvertStringTimestamp(dateTimeValue);
        }

        if (value is TimeSpan timeSpanValue)
        {
            return ConvertStringDuration(timeSpanValue);
        }

        if (value is Duration durationValue)
        {
            return ConvertStringDuration(durationValue);
        }

        throw new CelNoSuchOverloadException($"Could not convert type '{value?.GetType().FullName ?? "null"}' to string.");
    }

    public static string ConvertStringBool(bool value)
    {
        if (value)
        {
            return "true";
        }

        return "false";
    }

    public static string? ConvertStringBytes(ByteString value)
    {
        try
        {
            var array = value.ToArray();
            return Encoding.GetString(array, 0, array.Length);
        }
        catch (DecoderFallbackException x)
        {
            //invalid encoding
            throw new CelInvalidUtf8Exception($"Could not convert type '{value?.GetType().FullName ?? "null"}' to string because the bytes are not valid utf-8.", x);
        }
    }

    public static string ConvertStringDouble(double value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ConvertStringInt(long value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ConvertStringUInt(ulong value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public static string ConvertStringDuration(TimeSpan value)
    {
        var totalSeconds = Convert.ToInt64(Math.Truncate(value.TotalSeconds));
        return totalSeconds.ToString(CultureInfo.InvariantCulture) + "s";
    }

    public static string ConvertStringDuration(Duration value)
    {
        return value.ToString().Trim('\"');
    }

    public static string ConvertStringTimestamp(Timestamp value)
    {
        return value.ToString();
    }

    public static string ConvertStringTimestamp(DateTimeOffset value)
    {
        string serializedValue;
        if (value.Millisecond == 0)
        {
            serializedValue = value.ToString("yyyy-MM-ddTHH:mm:ssK", CultureInfo.InvariantCulture);
        }
        else
        {
            serializedValue = value.ToString("yyyy-MM-ddTHH:mm:ss'.'fffffffK", CultureInfo.InvariantCulture);
        }

        return serializedValue.Replace("+00:00", "Z");
    }

    #endregion

    #region Add

    public static string AddString(string value, object? otherValue)
    {
        if (otherValue is string stringOtherValue)
        {
            return AddStringString(value, stringOtherValue);
        }

        throw new CelNoSuchOverloadException($"No overload exists to ADD string and type '{otherValue?.GetType().FullName ?? "null"}'.");
    }

    public static string AddStringString(string value, string otherValue)
    {
        return string.Concat(value, otherValue);
    }

    #endregion

    #region CEL Functions

    public static bool ContainsString(string value1, string value2)
    {
        return value1.Contains(value2);
    }

    public static bool MatchesString(string value1, string value2)
    {
        var regex = new Regex(value2);
        return regex.IsMatch(value1);
    }

    public static int SizeString(string value)
    {
        // we want the number of characters in the string
        // if we have emoji, then string.Length returns the byte count, not the
        // symbol count.
        // so we have to use StringInfo.
        var stringInfo = new StringInfo(value);
        return stringInfo.LengthInTextElements;
    }

    public static bool EndsWithString(string value1, string value2)
    {
        return value1.EndsWith(value2, StringComparison.Ordinal);
    }

    public static bool StartsWithString(string value1, string value2)
    {
        return value1.StartsWith(value2, StringComparison.Ordinal);
    }

    public static string CharAt(string value, long index)
    {
        if (index > int.MaxValue)
        {
            return "";
        }

        if (index < 0 || index >= value.Length)
        {
            return "";
        }

        return value[(int)index].ToString();
    }

    public static long IndexOf(string value, string searchString, long? startIndex)
    {
        if (string.IsNullOrWhiteSpace(searchString))
        {
            return -1;
        }

        if (startIndex != null)
        {
            var startIndexLong = Int64Helpers.ConvertInt(startIndex);
            if (startIndexLong > int.MaxValue)
            {
                return -1;
            }

            if (startIndexLong < 0 || startIndexLong >= value.Length)
            {
                return -1;
            }

            return value.IndexOf(searchString, (int)startIndexLong, StringComparison.Ordinal);
        }

        return value.IndexOf(searchString, StringComparison.Ordinal);
    }

    public static long LastIndexOf(string value, string searchString, long? startIndex)
    {
        var startIndexInt = value.Length;

        if (startIndex.HasValue)
        {
            var startIndexLong = Int64Helpers.ConvertInt(startIndex.Value);
            if (startIndexLong > int.MaxValue)
            {
                return -1;
            }

            if (startIndexLong < 0 || startIndexLong >= value.Length)
            {
                return -1;
            }

            startIndexInt = (int)startIndexLong;
        }

        if (string.IsNullOrEmpty(searchString))
        {
            return startIndexInt;
        }

        return value.LastIndexOf(searchString, startIndexInt, StringComparison.Ordinal);
    }

    #endregion
}