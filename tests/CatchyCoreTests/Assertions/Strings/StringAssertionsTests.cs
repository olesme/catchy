using Catchy;
using System.Text.RegularExpressions;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Assertions.Strings
{
    /// <summary>
    /// Integration tests for StringAssertions.
    /// Covers equality, containment, position, regex, length, and case.
    /// </summary>
    public class StringAssertionsTests
    {
        // ===== Equality =====

        [Fact]
        public async Task Is_IdenticalStrings_Passes()
        {
            // Arrange
            string value = "Hello";

            // Act & Verify
            await Stateless.Assert.That(value).Is("Hello");
        }

        [Fact]
        public async Task Is_DifferentStrings_Throws()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("Hello").Is("World"));
            Assert.Contains("to equal \"World\"", msg);
        }

        [Fact]
        public async Task Is_DifferentCase_CaseSensitive_Throws()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("hello").Is("HELLO"));
            Assert.Contains("to equal \"HELLO\"", msg);
        }

        [Fact]
        public async Task Is_DifferentCase_CaseInsensitive_Passes()
        {
            // Arrange
            string value = "hello";

            // Act & Verify
            await Stateless.Assert.That(value).Is("HELLO").UsingOrdinal().IgnoringCase();
        }

        [Fact]
        public async Task IsNot_DifferentStrings_Passes()
        {
            // Arrange
            string value = "Hello";

            // Act & Verify
            await Stateless.Assert.That(value).IsNot("World");
        }

        // ===== Containment =====

        [Fact]
        public async Task Contains_SubstringExists_Passes()
        {
            // Arrange
            string value = "Hello, World!";

            // Act & Verify
            await Stateless.Assert.That(value).Contains("World");
        }

        [Fact]
        public async Task Contains_SubstringMissing_Throws()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("Hello, World!").Contains("xyz"));
            Assert.Contains("to contain \"xyz\"", msg);
        }

        [Fact]
        public async Task Contains_CaseInsensitive_Passes()
        {
            // Arrange
            string value = "Hello, World!";

            // Act & Verify
            await Stateless.Assert.That(value).Contains("world").UsingOrdinal().IgnoringCase();
        }

        [Fact]
        public async Task DoesNotContain_SubstringMissing_Passes()
        {
            // Arrange
            string value = "Hello";

            // Act & Verify
            await Stateless.Assert.That(value).DoesNotContain("xyz");
        }

        // ===== Position =====

        [Fact]
        public async Task StartsWith_MatchingPrefix_Passes()
        {
            // Arrange
            string value = "Hello, World!";

            // Act & Verify
            await Stateless.Assert.That(value).StartsWith("Hello");
        }

        [Fact]
        public async Task StartsWith_NoMatch_Throws()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("Hello, World!").StartsWith("World"));
            Assert.Contains("to start with \"World\"", msg);
        }

        [Fact]
        public async Task EndsWith_MatchingSuffix_Passes()
        {
            // Arrange
            string value = "Hello, World!";

            // Act & Verify
            await Stateless.Assert.That(value).EndsWith("World!");
        }

        [Fact]
        public async Task DoesNotEndWith_NoMatch_Passes()
        {
            // Arrange
            string value = "Hello";

            // Act & Verify
            await Stateless.Assert.That(value).DoesNotEndWith("World");
        }

        // ===== Regex =====

        [Fact]
        public async Task Matches_ValidPattern_Passes()
        {
            // Arrange
            string value = "user@example.com";
            var pattern = new Regex(@"^\w+@\w+\.\w+$");

            // Act & Verify
            await Stateless.Assert.That(value).Matches(pattern);
        }

        [Fact]
        public async Task Matches_InvalidPattern_Throws()
        {
            var pattern = new Regex(@"^\w+@\w+\.\w+$");
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("not-an-email").Matches(pattern));
            Assert.Contains("to match /", msg);
        }

        [Fact]
        public async Task DoesNotMatch_NoMatch_Passes()
        {
            // Arrange
            string value = "hello";
            var pattern = new Regex(@"\d");  // Digits

            // Act & Verify
            await Stateless.Assert.That(value).DoesNotMatch(pattern);
        }

        // ===== Length =====

        [Fact]
        public async Task HasLength_CorrectLength_Passes()
        {
            // Arrange
            string value = "Hello";  // 5 chars

            // Act & Verify
            await Stateless.Assert.That(value).HasLength(5);
        }

        [Fact]
        public async Task HasLength_IncorrectLength_Throws()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("Hello").HasLength(3));
            Assert.Contains("Expected string length 3, but was 5", msg);
        }

        [Fact]
        public async Task HasLengthAtLeast_SufficientLength_Passes()
        {
            // Arrange
            string value = "Hello";  // 5 chars

            // Act & Verify
            await Stateless.Assert.That(value).HasLengthAtLeast(3);
        }

        [Fact]
        public async Task HasLengthAtMost_SufficientLength_Passes()
        {
            // Arrange
            string value = "Hello";  // 5 chars

            // Act & Verify
            await Stateless.Assert.That(value).HasLengthAtMost(10);
        }

        [Fact]
        public async Task IsEmpty_EmptyString_Passes()
        {
            // Arrange
            string value = "";

            // Act & Verify
            await Stateless.Assert.That(value).IsEmpty();
        }

        [Fact]
        public async Task IsNotEmpty_NonEmptyString_Passes()
        {
            // Arrange
            string value = "Hello";

            // Act & Verify
            await Stateless.Assert.That(value).IsNotEmpty();
        }

        // ===== Case =====

        [Fact]
        public async Task IsUpperCase_AllUpper_Passes()
        {
            // Arrange
            string value = "HELLO";

            // Act & Verify
            await Stateless.Assert.That(value).IsUpperCase();
        }

        [Fact]
        public async Task IsUpperCase_Mixed_Throws()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("Hello").IsUpperCase());
            Assert.Contains("to be upper-case", msg);
        }

        [Fact]
        public async Task IsLowerCase_AllLower_Passes()
        {
            // Arrange
            string value = "hello";

            // Act & Verify
            await Stateless.Assert.That(value).IsLowerCase();
        }

        [Fact]
        public async Task IsLowerCase_Mixed_Throws()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("Hello").IsLowerCase());
            Assert.Contains("to be lower-case", msg);
        }

        // ===== Modifiers =====

        [Fact]
        public async Task Contains_WithIgnoringCaseModifier_Passes()
        {
            // Arrange
            string value = "Hello, World!";

            // Act & Verify
            await Stateless.Assert.That(value)
                .Contains("world")
                .IgnoringCase();
        }

        [Fact]
        public async Task Contains_WithBecause_IncludesMessage()
        {
            // Arrange
            string value = "Hello";
            AssertionException? ex = null;

            // Act
            try
            {
                await Stateless.Assert.That(value)
                    .Contains("xyz")
                    .Because("substring must exist");
            }
            catch (AssertionException e)
            {
                ex = e;
            }

            // Verify
            await Stateless.Assert.That(ex).IsNotNull();
            await Stateless.Assert.That(ex!.Message).Contains("substring must exist");
        }

        // ===== Nullable =====

        [Fact]
        public async Task StringAssertions_WithNull_IsNull_Passes()
        {
            // Arrange
            string? value = null;

            // Act & Verify
            await Stateless.Assert.That(value).IsNull();
        }

        [Fact]
        public async Task StringAssertions_SoftMode_AccumulatesFailures()
        {
            // Arrange
            var verify = Asserter.NewSoft();

            // Act
            await verify.That("hello").Is("hello");  // Pass
            await verify.That("hello").Is("world");  // Fail
            await verify.That("hello").Contains("x");  // Fail

            // Verify
            await Stateless.Assert.That(verify.ErrorCount).Is(2);
        }

        // ===== Additional tests from XUnit =====

        [Fact]
        public async Task Is_case_insensitive_passes()
            => await Stateless.Assert.That("Hello").Is("hello").UsingOrdinal().IgnoringCase();

        [Fact]
        public async Task Contains_case_insensitive_passes()
            => await Stateless.Assert.That("Hello World").Contains("world").UsingOrdinal().IgnoringCase();

        [Fact]
        public async Task Trailing_modifier_applies_to_full_string_chain()
            => await Stateless.Assert.That("Hello World")
                .Contains("world")
                .And().StartsWith("hello")
                .And().EndsWith("WORLD")
                .UsingOrdinal()
                .IgnoringCase();

        [Fact]
        public async Task String_chain_passes()
            => await Stateless.Assert.That("hello world")
                .IsNotNullOrEmpty()
                .And().StartsWith("hello")
                .And().EndsWith("world")
                .And().Contains(" ")
                .And().HasLength(11);

        [Fact]
        public async Task Is_fails_shows_diff()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("hello").Is("hxllo"));
            // Should contain diff info
            Assert.Contains("Diff at index", msg);
        }

        [Fact]
        public async Task StartsWith_fails()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("hello").StartsWith("world"));
            Assert.Contains("start with", msg.ToLower());
        }

        [Fact]
        public async Task HasLength_fails_shows_lengths()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("hi").HasLength(10));
            Assert.Contains("2", msg);  // actual length
            Assert.Contains("10", msg); // expected length
        }

        [Fact]
        public async Task IsUpperCase_fails()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("Hello").IsUpperCase());
            Assert.Contains("upper", msg.ToLower());
        }

        [Fact]
        public async Task Matches_fails()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("hello").Matches(@"^\d+$"));
            Assert.Contains("match", msg.ToLower());
        }

        [Fact]
        public async Task IsOneOf_fails()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("z").IsOneOf(["a", "b", "c"]));
            Assert.Contains("one of", msg.ToLower());
        }

        [Fact]
        public async Task IsGuid_passes() => await Stateless.Assert.That(Guid.NewGuid().ToString()).IsGuid();

        [Fact]
        public async Task IsNullOrEmpty_passes_null() => await Stateless.Assert.That((string?)null).IsNullOrEmpty();

        [Fact]
        public async Task IsNullOrEmpty_passes_empty() => await Stateless.Assert.That("").IsNullOrEmpty();

        [Fact]
        public async Task IsNotNullOrEmpty_passes() => await Stateless.Assert.That("x").IsNotNullOrEmpty();

        [Fact]
        public async Task IsNullOrWhiteSpace_passes() => await Stateless.Assert.That("   ").IsNullOrWhiteSpace();

        [Fact]
        public async Task IsNotNullOrWhiteSpace_passes() => await Stateless.Assert.That("x").IsNotNullOrWhiteSpace();

        [Fact]
        public async Task IsNullOrWhiteSpace_fails_for_non_whitespace()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("x").IsNullOrWhiteSpace());
            Assert.Contains("whitespace", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task IsNotNullOrEmpty_fails_for_null()
        {
            string? value = null;
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That(value).IsNotNullOrEmpty());
            Assert.Contains("null or empty", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task IsNullOrEmpty_fails_for_non_empty()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("x").IsNullOrEmpty());
            Assert.Contains("null or empty", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task DoesNotMatch_passes() => await Stateless.Assert.That("abc").DoesNotMatch(@"^\d+$");

        [Fact]
        public async Task IsOneOf_IEnumerable_passes() => await Stateless.Assert.That("b").IsOneOf(["a", "b", "c"]);

        [Fact]
        public async Task IsOneOf_fails_when_value_missing()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("z").IsOneOf(["a", "b", "c"]));
            Assert.Contains("one of", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task HasLengthGreaterThan_passes() => await Stateless.Assert.That("hello").HasLengthGreaterThan(3);

        [Fact]
        public async Task HasLengthLessThan_passes() => await Stateless.Assert.That("hi").HasLengthLessThan(5);

        [Fact]
        public async Task IsTrimmed_passes() => await Stateless.Assert.That("hello").IsTrimmed();

        [Fact]
        public async Task DoesNotStartWith_passes()
            => await Stateless.Assert.That("hello world").DoesNotStartWith("world");

        [Fact]
        public async Task DoesNotStartWith_fails()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("hello world").DoesNotStartWith("hello"));
            Assert.Contains("not to start", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task ContainsAll_passes()
            => await Stateless.Assert.That("alpha beta gamma").ContainsAll("alpha", "gamma");

        [Fact]
        public async Task ContainsAll_fails_when_missing_one()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("alpha beta").ContainsAll("alpha", "gamma"));
            Assert.Contains("all", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task ContainsAny_passes_when_one_present()
            => await Stateless.Assert.That("alpha beta").ContainsAny("zzz", "beta");

        [Fact]
        public async Task ContainsAny_fails_when_none_present()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("alpha beta").ContainsAny("zzz", "yyy"));
            Assert.Contains("any", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task IsAlpha_passes()
            => await Stateless.Assert.That("HelloWorld").IsAlpha();

        [Fact]
        public async Task IsAlpha_fails_for_digits()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("abc123").IsAlpha());
            Assert.Contains("alpha", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task IsNumeric_passes()
            => await Stateless.Assert.That("123456").IsNumeric();

        [Fact]
        public async Task IsNumeric_fails_for_letters()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("12ab").IsNumeric());
            Assert.Contains("numeric", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task IsAlphanumeric_passes()
            => await Stateless.Assert.That("abc123").IsAlphanumeric();

        [Fact]
        public async Task IsAlphanumeric_fails_for_symbols()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("abc-123").IsAlphanumeric());
            Assert.Contains("alphanumeric", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task IsBase64_passes()
            => await Stateless.Assert.That(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("hello"))).IsBase64();

        [Fact]
        public async Task IsValidJson_passes()
            => await Stateless.Assert.That("{ \"name\": \"Alice\" }").IsValidJson();

        [Fact]
        public async Task IsValidXml_passes()
            => await Stateless.Assert.That("<root><name>Alice</name></root>").IsValidXml();

        [Fact]
        public async Task ContainsLine_passes()
            => await Stateless.Assert.That("one\ntwo\nthree").ContainsLine("two");

        [Fact]
        public async Task HasNoLeadingWhiteSpace_passes()
            => await Stateless.Assert.That("hello").HasNoLeadingWhiteSpace();

        [Fact]
        public async Task HasNoTrailingWhiteSpace_passes()
            => await Stateless.Assert.That("hello").HasNoTrailingWhiteSpace();

        [Fact]
        public async Task HasLineCount_passes()
            => await Stateless.Assert.That("one\ntwo\nthree").HasLineCount(3);

        [Fact]
        public async Task HasLineCount_fails_when_count_differs()
        {
            var msg = await Catch.FailureOf(async () => await Stateless.Assert.That("one\ntwo\nthree").HasLineCount(2));
            Assert.Contains("line", msg.ToLowerInvariant());
        }

        [Fact]
        public async Task IsJsonEquivalentTo_passes()
            => await Stateless.Assert.That("{ \"name\": \"Alice\", \"age\": 30 }").IsJsonEquivalentTo("{ \"age\": 30, \"name\": \"Alice\" }");

        [Fact]
        public async Task IsJsonSerializable_passes()
            => await Stateless.Assert.That("{ \"name\": \"Alice\" }").IsJsonSerializable<object>();
    }
}





