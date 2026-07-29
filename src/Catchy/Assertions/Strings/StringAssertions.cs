using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Catchy.Sdk;

namespace Catchy
{
        public static partial class AsserterExtensions
        {
            /// <summary>
            /// Starts assertions for a string value.
            /// </summary>
            public static ValueAssertions<string?> That(this Asserter a, string? value, __._ _ = default,
                [CallerArgumentExpression(nameof(a))] string? aExpr = null,
                [CallerArgumentExpression(nameof(value))] string? vExpr = null,
                [CallerFilePath] string? file = null,
                [CallerLineNumber] int line = 0,
                [CallerMemberName] string? member = null)
            {
                var p = a.NewPipeline(
                            asserterExpr: aExpr,
                            methodName: "That",
                            valueExpr: vExpr,
                            file: file, line: line, member: member);
                return new ValueAssertions<string?>(p, value);
        }
    }

    public static partial class StringAssertionsExtensions
    {
        /// <summary>
        /// Asserts that the string equals <paramref name="expected"/> using the current comparison mode.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> Is(this ValueAssertions<string?> a, string? expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            a.Link("Is", expr);
            // If assertion has an async provider, perform async/polled retrieval matching pipeline timeouts.
            if (a._asyncProvider is not null)
            {
                a.Op(a => CheckOperation.Async(async () =>
                {
                    var v = await a.GetValueAsync().ConfigureAwait(false);
                    return string.Equals(v, expected, a.GetEffectiveComparison());
                }, () => $"Expected value to equal {ExprFormat.Inline(expected, expr)}", a.IsSkipped()));
            }
            else
            {
                a.Op(a => StringChecks.EqualTo(a.GetValue(), expected, a.GetEffectiveComparison, a.IsSkipped(), expr));
            }
            return a;
        }
        /// <summary>Asserts that the string is empty (equal to <c>""</c>).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsEmpty(this ValueAssertions<string?> a,
            [CallerArgumentExpression(nameof(a))] string? expr = null)
        {
            a.Link("IsEmpty", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() == "",
                () => $"Expected {ExprFormat.Inline(a.GetValue())} to be empty",
                a.IsSkipped()));
            return a;
        }
        /// <summary>
        /// Asserts that the string is null or empty.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsNullOrEmpty(this ValueAssertions<string?> a,
            [CallerArgumentExpression(nameof(a))] string? expr = null)
        {
            a.Link("IsNullOrEmpty", expr);
            a.Op(a => CheckOperation.Sync(
                () => string.IsNullOrEmpty(a.GetValue()),
                () => $"Expected {ExprFormat.Inline(a.GetValue())} to be null or empty",
                a.IsSkipped()));
            return a;
        }
        /// <summary>
        /// Asserts that the string is neither null nor empty.
        /// </summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsNotNullOrEmpty(this ValueAssertions<string?> a,
            [CallerArgumentExpression(nameof(a))] string? expr = null)
        {
            a.Link("IsNotNullOrEmpty", expr);
            a.Op(a => CheckOperation.Sync(
                () => !string.IsNullOrEmpty(a.GetValue()),
                () => $"Expected {ExprFormat.Inline(a.GetValue())} not to be null or empty",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string is not empty.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsNotEmpty(this ValueAssertions<string?> a,
            [CallerArgumentExpression(nameof(a))] string? expr = null)
        {
            a.Link("IsNotEmpty", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() != "",
                () => $"Expected {ExprFormat.Inline(a.GetValue())} not to be empty",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string is not equal to <paramref name="unexpected"/> using the current comparison mode.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsNot(this ValueAssertions<string?> a, string? unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        {
            a.Link("IsNot", expr);
            a.Op(a => CheckOperation.Sync(
                () => !string.Equals(a.GetValue(), unexpected, a.GetEffectiveComparison()),
                () => $"Expected {ExprFormat.Inline(a.GetValue())} not to equal {ExprFormat.Inline(unexpected, expr)}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string contains <paramref name="substring"/> using the current comparison mode.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> Contains(this ValueAssertions<string?> a, string substring,
            [CallerArgumentExpression(nameof(substring))] string? expr = null)
        {
            a.Link("Contains", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue()?.IndexOf(substring, a.GetEffectiveComparison()) >= 0,
                () => $"Expected {ExprFormat.Inline(a.GetValue())} to contain {ExprFormat.Inline(substring, expr)}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string does not contain <paramref name="substring"/> using the current comparison mode.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> DoesNotContain(this ValueAssertions<string?> a, string substring,
            [CallerArgumentExpression(nameof(substring))] string? expr = null)
        {
            a.Link("DoesNotContain", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is null || a.GetValue()!.IndexOf(substring, a.GetEffectiveComparison()) < 0,
                () => $"Expected {ExprFormat.Inline(a.GetValue())} not to contain {ExprFormat.Inline(substring, expr)}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string starts with <paramref name="prefix"/> using the current comparison mode.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> StartsWith(this ValueAssertions<string?> a, string prefix,
            [CallerArgumentExpression(nameof(prefix))] string? expr = null)
        {
            a.Link("StartsWith", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue()?.StartsWith(prefix, a.GetEffectiveComparison()) == true,
                () => $"Expected {ExprFormat.Inline(a.GetValue())} to start with {ExprFormat.Inline(prefix, expr)}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string does not start with <paramref name="prefix"/> using the current comparison mode.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> DoesNotStartWith(this ValueAssertions<string?> a, string prefix,
            [CallerArgumentExpression(nameof(prefix))] string? expr = null)
        {
            a.Link("DoesNotStartWith", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is null || !a.GetValue()!.StartsWith(prefix, a.GetEffectiveComparison()),
                () => $"Expected {ExprFormat.Inline(a.GetValue())} not to start with {ExprFormat.Inline(prefix, expr)}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string ends with <paramref name="suffix"/> using the current comparison mode.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> EndsWith(this ValueAssertions<string?> a, string suffix,
            [CallerArgumentExpression(nameof(suffix))] string? expr = null)
        {
            a.Link("EndsWith", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue()?.EndsWith(suffix, a.GetEffectiveComparison()) == true,
                () => $"Expected {ExprFormat.Inline(a.GetValue())} to end with {ExprFormat.Inline(suffix, expr)}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string does not end with <paramref name="suffix"/> using the current comparison mode.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> DoesNotEndWith(this ValueAssertions<string?> a, string suffix,
            [CallerArgumentExpression(nameof(suffix))] string? expr = null)
        {
            a.Link("DoesNotEndWith", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is null || !a.GetValue()!.EndsWith(suffix, a.GetEffectiveComparison()),
                () => $"Expected {ExprFormat.Inline(a.GetValue())} not to end with {ExprFormat.Inline(suffix, expr)}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string has exactly <paramref name="expected"/> characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> HasLength(this ValueAssertions<string?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            a.Link("HasLength", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue()?.Length == expected,
                () => $"Expected string length {expected}, but was {a.GetValue()?.Length.ToString() ?? "null"}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string has more than <paramref name="length"/> characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> HasLengthGreaterThan(this ValueAssertions<string?> a, int length,
            [CallerArgumentExpression(nameof(length))] string? expr = null)
        {
            a.Link("HasLengthGreaterThan", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue()?.Length > length,
                () => $"Expected string length > {length}, but was {a.GetValue()?.Length.ToString() ?? "null"}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string has fewer than <paramref name="length"/> characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> HasLengthLessThan(this ValueAssertions<string?> a, int length,
            [CallerArgumentExpression(nameof(length))] string? expr = null)
        {
            a.Link("HasLengthLessThan", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is not null && a.GetValue()!.Length < length,
                () => $"Expected string length < {length}, but was {a.GetValue()?.Length.ToString() ?? "null"}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string matches the given <paramref name="regex"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> Matches(this ValueAssertions<string?> a, Regex regex)
        {
            a.Link("Matches", regex.ToString());
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is not null && regex.IsMatch(a.GetValue()!),
                () => $"Expected {ExprFormat.Inline(a.GetValue())} to match /{regex}/",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string matches the given regex <paramref name="pattern"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> Matches(this ValueAssertions<string?> a, string pattern) => a.Matches(new Regex(pattern));
        /// <summary>Asserts that the string does not match the given <paramref name="regex"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> DoesNotMatch(this ValueAssertions<string?> a, Regex regex)
        {
            a.Link("DoesNotMatch", regex.ToString());
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is null || !regex.IsMatch(a.GetValue()!),
                () => $"Expected {ExprFormat.Inline(a.GetValue())} not to match /{regex}/",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string does not match the given regex <paramref name="pattern"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> DoesNotMatch(this ValueAssertions<string?> a, string pattern) => a.DoesNotMatch(new Regex(pattern));
        /// <summary>Asserts that the string consists entirely of upper-case characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsUpperCase(this ValueAssertions<string?> a)
        {
            a.Link("IsUpperCase");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is not null && string.Equals(a.GetValue(), a.GetValue()!.ToUpperInvariant(), StringComparison.Ordinal),
                () => $"Expected {ExprFormat.Inline(a.GetValue())} to be upper-case",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string consists entirely of lower-case characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsLowerCase(this ValueAssertions<string?> a)
        {
            a.Link("IsLowerCase");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is not null && string.Equals(a.GetValue(), a.GetValue()!.ToLowerInvariant(), StringComparison.Ordinal),
                () => $"Expected {ExprFormat.Inline(a.GetValue())} to be lower-case",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string has no leading or trailing whitespace.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsTrimmed(this ValueAssertions<string?> a)
        {
            a.Link("IsTrimmed");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is not null && string.Equals(a.GetValue(), a.GetValue()!.Trim(), StringComparison.Ordinal),
                () => "Expected string to be trimmed",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string is a valid GUID.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsGuid(this ValueAssertions<string?> a)
        {
            a.Link("IsGuid");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is not null && Guid.TryParse(a.GetValue(), out _),
                () => $"Expected {ExprFormat.Inline(a.GetValue())} to be a valid GUID",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string is null or consists only of whitespace characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsNullOrWhiteSpace(this ValueAssertions<string?> a)
        {
            a.Link("IsNullOrWhiteSpace");
            a.Op(a => CheckOperation.Sync(
                () => string.IsNullOrWhiteSpace(a.GetValue()),
                () => "Expected string to be null or whitespace",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string is not null and is not entirely whitespace.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsNotNullOrWhiteSpace(this ValueAssertions<string?> a)
        {
            a.Link("IsNotNullOrWhiteSpace");
            a.Op(a => CheckOperation.Sync(
                () => !string.IsNullOrWhiteSpace(a.GetValue()),
                () => $"Expected non-null non-whitespace string, but was {ExprFormat.Inline(a.GetValue())}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string equals one of <paramref name="values"/>.</summary>
        [GenerateArityOverloads(target: "values")]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsOneOf(this ValueAssertions<string?> a, IEnumerable<string> values,
            [CallerArgumentExpression(nameof(values))] string? valuesExpr = null)
        {
            a.Link("IsOneOf", valuesExpr);
            a.Op(a => StringChecks.IsOneOf(a.GetValue(), values, a.GetEffectiveComparison, a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string has at least <paramref name="minLength"/> characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> HasLengthAtLeast(this ValueAssertions<string?> a, int minLength,
            [CallerArgumentExpression(nameof(minLength))] string? expr = null)
        {
            a.Link("HasLengthAtLeast", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue()?.Length >= minLength,
                () => $"Expected string length at least {minLength}, but was {a.GetValue()?.Length.ToString() ?? "null"}",
                a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string has at most <paramref name="maxLength"/> characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> HasLengthAtMost(this ValueAssertions<string?> a, int maxLength,
            [CallerArgumentExpression(nameof(maxLength))] string? expr = null)
        {
            a.Link("HasLengthAtMost", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is not null && a.GetValue()!.Length <= maxLength,
                () => $"Expected string length at most {maxLength}, but was {a.GetValue()?.Length.ToString() ?? "null"}",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the string contains all values from <paramref name="substrings"/>.</summary>
        [GenerateArityOverloads(target: "substrings")]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> ContainsAll(this ValueAssertions<string?> a, IEnumerable<string> substrings,
            [CallerArgumentExpression(nameof(substrings))] string? expr = null)
        {
            a.Link("ContainsAll", expr);
            a.Op(a => StringChecks.ContainsAll(a.GetValue(), substrings, a.GetEffectiveComparison(), a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the string contains at least one value from <paramref name="substrings"/>.</summary>
        [GenerateArityOverloads(target: "substrings")]
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> ContainsAny(this ValueAssertions<string?> a, IEnumerable<string> substrings,
            [CallerArgumentExpression(nameof(substrings))] string? expr = null)
        {
            a.Link("ContainsAny", expr);
            a.Op(a => StringChecks.ContainsAny(a.GetValue(), substrings, a.GetEffectiveComparison(), a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string contains only alphabetic characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsAlpha(this ValueAssertions<string?> a)
        {
            a.Link("IsAlpha");
            a.Op(a => StringChecks.IsAlpha(a.GetValue(), a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string contains only numeric digit characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsNumeric(this ValueAssertions<string?> a)
        {
            a.Link("IsNumeric");
            a.Op(a => StringChecks.IsNumeric(a.GetValue(), a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string contains only alphanumeric characters.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsAlphanumeric(this ValueAssertions<string?> a)
        {
            a.Link("IsAlphanumeric");
            a.Op(a => StringChecks.IsAlphanumeric(a.GetValue(), a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string is a valid Base-64 encoded value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsBase64(this ValueAssertions<string?> a)
        {
            a.Link("IsBase64");
            a.Op(a => StringChecks.IsBase64(a.GetValue(), a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string is well-formed XML.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsValidXml(this ValueAssertions<string?> a)
        {
            a.Link("IsValidXml");
            a.Op(a => StringChecks.IsValidXml(a.GetValue(), a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string does not begin with whitespace.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> HasNoLeadingWhiteSpace(this ValueAssertions<string?> a)
        {
            a.Link("HasNoLeadingWhiteSpace");
            a.Op(a => StringChecks.HasNoLeadingWhiteSpace(a.GetValue(), a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string does not end with whitespace.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> HasNoTrailingWhiteSpace(this ValueAssertions<string?> a)
        {
            a.Link("HasNoTrailingWhiteSpace");
            a.Op(a => StringChecks.HasNoTrailingWhiteSpace(a.GetValue(), a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string contains exactly <paramref name="expected"/> lines.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> HasLineCount(this ValueAssertions<string?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            a.Link("HasLineCount", expr);
            a.Op(a => StringChecks.HasLineCount(a.GetValue(), expected, a.IsSkipped(), expr));
            return a;
        }
        /// <summary>Asserts that the string contains <paramref name="line"/> as a complete line.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> ContainsLine(this ValueAssertions<string?> a, string line,
            StringComparison? comparison = null,
            [CallerArgumentExpression(nameof(line))] string? expr = null)
        {
            a.Link("ContainsLine", expr);
            var cmp = comparison ?? a.GetEffectiveComparison();
            a.Op(a => StringChecks.ContainsLine(a.GetValue(), line, cmp, a.IsSkipped(), expr));
            return a;
        }
        /// <summary>Asserts that the string is valid JSON.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsValidJson(this ValueAssertions<string?> a)
        {
            a.Link("IsValidJson");
            a.Op(a => StringChecks.IsValidJson(a.GetValue(), a.IsSkipped()));
            return a;
        }
        /// <summary>Asserts that the string is semantically equivalent to <paramref name="expectedJson"/> (ignoring whitespace and key order).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsJsonEquivalentTo(this ValueAssertions<string?> a, string expectedJson,
            [CallerArgumentExpression(nameof(expectedJson))] string? expr = null)
        {
            a.Link("IsJsonEquivalentTo", expr);
            a.Op(a => StringChecks.IsJsonEquivalentTo(a.GetValue(), expectedJson, a.IsSkipped(), expr));
            return a;
        }
        /// <summary>Asserts that the string can be deserialized to <typeparamref name="T"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<string?> IsJsonSerializable<T>(this ValueAssertions<string?> a)
        {
            a.Link("IsJsonSerializable", typeof(T));
            a.Op(a => StringChecks.IsJsonSerializable<T>(a.GetValue(), a.IsSkipped()));
            return a;
        }
    }

    namespace Sdk
    {
        public static class StringAssertionsAccessors
        {
            public static StringComparison GetEffectiveComparison(this IAssertions a)
                => a.GetPipeline().Settings.DefaultStringComparison;
        }
    }
}























