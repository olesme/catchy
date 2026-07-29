using System.Reflection;

namespace CatchyCoreTests.Quality
{
    public class AssertionCoverageGateTests
    {
        [Fact]
        public void Core_assertion_methods_have_curated_pass_fail_message_coverage_map()
        {
            var allTests = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.IsClass)
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                    .Where(m => m.GetCustomAttributes().Any(a => a.GetType().Name == "FactAttribute"))
                    .Select(m => $"{t.Name}.{m.Name}"))
                .ToHashSet(StringComparer.Ordinal);

            var requiredByArea = new Dictionary<string, string[]>(StringComparer.Ordinal)
            {
                ["Strings.Is"] =
                [
                    "StringAssertionsTests.Is_IdenticalStrings_Passes",
                    "StringAssertionsTests.Is_DifferentStrings_Throws",
                    "StringAssertionsTests.Is_fails_shows_diff"
                ],
                ["Strings.Contains"] =
                [
                    "StringAssertionsTests.Contains_SubstringExists_Passes",
                    "StringAssertionsTests.Contains_SubstringMissing_Throws",
                    "StringAssertionsTests.Contains_WithBecause_IncludesMessage"
                ],
                ["Nullable.IsNull"] =
                [
                    "NullableAssertionsTests.IsNull_WithNull_Passes",
                    "NullableAssertionsTests.IsNull_WithNonNull_Throws",
                    "NullableAssertionsTests.IsNull_FailureMessage_IsDescriptive"
                ],
                ["Nullable.IsNotNull"] =
                [
                    "NullableAssertionsTests.IsNotNull_WithNonNull_Passes",
                    "NullableAssertionsTests.IsNotNull_WithNull_Throws"
                ],
                ["Collections.PredicateCapture"] =
                [
                    "CollectionAssertionsTests.Contains_WithPredicate_MatchFound_Passes",
                    "CollectionAssertionsTests.Contains_WithPredicate_NoMatch_Throws",
                    "CollectionAssertionsTests.HasFirst_NonEmpty_Passes",
                    "CollectionAssertionsTests.HasLast_NonEmpty_Passes"
                ],
                ["Collections.Count"] =
                [
                    "CollectionAssertionsTests.HasCount_CorrectCount_Passes",
                    "CollectionAssertionsTests.HasCount_IncorrectCount_Throws"
                ],
                ["Collections.QuantifiedNumeric"] =
                [
                    "QuantifiedAssertionTests.ThatEachOf_IsInRange_passes",
                    "QuantifiedAssertionTests.ThatAnyOf_IsInRange_passes_when_any_matches",
                    "QuantifiedAssertionTests.ThatEachOf_IsCloseTo_passes_for_doubles",
                    "QuantifiedAssertionTests.ThatEachOf_IsCloseTo_fails_shows_indexed_reason",
                    "QuantifiedAssertionTests.ThatEachOf_IsMultipleOf_passes",
                    "QuantifiedAssertionTests.ThatAnyOf_IsMultipleOf_passes_when_any_matches",
                    "QuantifiedAssertionTests.ThatEachOf_IsEven_fails_shows_failing_item"
                ],
                ["Objects.TypeIs"] =
                [
                    "StructuralAssertionsTests.Type_Is_WithSameType_Passes",
                    "StructuralAssertionsTests.Type_Is_WithDifferentType_Throws",
                    "StructuralAssertionsTests.Type_Is_WithBecause_IncludesMessage",
                    "StructuralAssertionsTests.Type_IsNot_WithDifferentType_Passes"
                ],
                ["Objects.ReferenceEquality"] =
                [
                    "StructuralAssertionsTests.Object_Is_WithSameObject_Passes",
                    "StructuralAssertionsTests.Object_Is_WithDifferentObjects_Throws",
                    "StructuralAssertionsTests.Object_IsNot_WithDifferentObjects_Passes"
                ],
                ["Objects.Nullability"] =
                [
                    "StructuralAssertionsTests.Object_IsNull_WithNull_Passes",
                    "StructuralAssertionsTests.Object_IsNull_WithNonNullObject_Throws",
                    "StructuralAssertionsTests.Object_IsNotNull_WithNonNullObject_Passes"
                ],
                ["Core.MessageAndDiff"] =
                [
                    "MessageAndDiffTests.Diff_ShouldShowBothActualAndExpectedValues",
                    "MessageAndDiffTests.Message_ShouldContainAssertionChain_WithHardAssertAndThat",
                    "MessageAndDiffTests.Message_ShouldContainBecauseReason_WhenBecauseIsChained"
                ],
                ["Core.Because"] =
                [
                    "BecauseTests.Because_appends_reason_to_error",
                    "BecauseTests.Because_appears_in_chain_links"
                ]
            };

            var missing = requiredByArea
                .SelectMany(area => area.Value
                    .Where(testName => !allTests.Contains(testName))
                    .Select(testName => $"[{area.Key}] {testName}"))
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToArray();

            Assert.True(missing.Length == 0,
                $"Quality gate failed. Missing required curated scenarios:\n - {string.Join("\n - ", missing)}");
        }
    }
}
