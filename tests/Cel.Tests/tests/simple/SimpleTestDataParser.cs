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

using System.Reflection;
using Google.Api.Expr.Test.V1;

namespace Cel.Tests;

public class SimpleTestDataParser
{
    public static SimpleTestDataLoader[] GetTestCases()
    {
        var testCases = new List<SimpleTestDataLoader>();

        var resourceNames = GetResourceNames();
        foreach (var resourceName in resourceNames)
        {
            var simpleTestFile = ParseTextProtoFile(resourceName);
            if (simpleTestFile == null)
            {
                continue;
            }

            for (var i = 0; i < simpleTestFile.Section.Count; i++)
            {
                for (var j = 0; j < simpleTestFile.Section[i].Test.Count; j++)
                {
                    var section = simpleTestFile.Section[i];
                    var test = section.Test[j];

                    if (!string.IsNullOrEmpty(test.IgnoreReason))
                    {
                        continue;
                    }

                    var sectionName = !string.IsNullOrWhiteSpace(section.Name) ? section.Name : "Section " + (i + 1);
                    var testName = !string.IsNullOrWhiteSpace(test.Name) ? test.Name : "Test " + (j + 1);


#if NETFRAMEWORK
                    if (test.SkipNetFramework)
                    {
                        continue;
                    }
#endif

                    testCases.Add(new SimpleTestDataLoader(simpleTestFile.Name, sectionName, testName, test));
                }
            }
        }

        return testCases.ToArray();
    }

    public static string[] GetResourceNames()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();

        var simpleTestResourceNames = resourceNames.Where(c => c.StartsWith("Cel.Tests.tests.simple.testdata.binary.", StringComparison.Ordinal)
                                                               && c.EndsWith(".binpb", StringComparison.Ordinal));

        return simpleTestResourceNames.ToArray();
    }

    public static SimpleTestFile? ParseTextProtoFile(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                return null;
            }

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return SimpleTestFile.Parser.ParseFrom(memoryStream.ToArray());
            }
        }
    }
}