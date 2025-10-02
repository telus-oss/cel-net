using System;
using System.Collections.Generic;
using System.Linq;
using Cel.Tests.Clr;

namespace Cel.Tests.Clr
{
    /// <summary>
    /// Aggregates all CLR arithmetic test cases for easy discovery and execution.
    /// </summary>
    public static class ComprehensiveArithmeticTestCases
    {
        /// <summary>
        /// Returns all arithmetic test cases from all CLR types.
        /// </summary>
        public static IEnumerable<ClrTestCase> TestCases()
        {
            return DecimalTestCases.TestCases()
                .Union(SingleTestCases.TestCases())
                .Union(Int32TestCases.TestCases())
                .Union(SignedIntegerArithmeticTestCases.TestCases())
                .Union(UnsignedIntegerArithmeticTestCases.TestCases())
                .Union(IntegerEdgeCaseTestCases.TestCases())
                .Union(StringTestCases.TestCases())
                .Union(TimestampConversionTestCases.TestCases());
        }

        /// <summary>
        /// Provides a summary of test coverage by type and operation.
        /// </summary>
        public static Dictionary<string, int> GetTestCoverageSummary()
        {
            var allTests = TestCases().ToList();
            var summary = new Dictionary<string, int>();

            var typeGroups = allTests.GroupBy(t => GetTypeFromTestName(t.Name));
            foreach (var typeGroup in typeGroups)
            {
                var operationGroups = typeGroup.GroupBy(t => GetOperationFromTestName(t.Name));
                foreach (var operationGroup in operationGroups)
                {
                    var key = $"{typeGroup.Key}_{operationGroup.Key}";
                    summary[key] = operationGroup.Count();
                }
            }

            // Add totals
            summary["Total_Tests"] = allTests.Count;
            summary["Total_Types"] = typeGroups.Count();

            return summary;
        }

        private static string GetTypeFromTestName(string testName)
        {
            if (string.IsNullOrEmpty(testName)) return "unknown";
            
            var parts = testName.Split('/');
            if (parts.Length >= 2)
            {
                return parts[1]; // e.g., "clr/int32/addition" -> "int32"
            }
            return "unknown";
        }

        private static string GetOperationFromTestName(string testName)
        {
            if (string.IsNullOrEmpty(testName)) return "unknown";
            
            var parts = testName.Split('/');
            if (parts.Length >= 3)
            {
                return parts[2]; // e.g., "clr/int32/addition" -> "addition"
            }
            return "unknown";
        }
    }
}