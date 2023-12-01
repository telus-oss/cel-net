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
using NUnit.Framework;

namespace Cel.Tests;

[TestFixture]
public class StringTests
{
    [Test]
    [TestCase("", 0)]
    [TestCase("a", 1)]
    [TestCase("ABCD  DEFG", 10)]
    [TestCase("\uD834\uDD61", 1)]
    [TestCase("ABC✋😉👍", 6)]
    [TestCase("👩🏽‍💻🧑🏾‍💻👨🏼‍💻", 12)]
    public void String_Size_Should_Return_Unicode_Codepoint_Count(string s, int expectedLength)
    {
        //string needs to return the number of unicode code points
        Assert.That(StringHelpers.SizeString(s), Is.EqualTo(expectedLength));
    }
}