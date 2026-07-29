using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <c>char</c> value (treated as nullable char? for uniform null handling).</summary>
        public static ValueAssertions<char?> That(this Asserter a, char value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<char?>(p, value);
        }

        /// <summary>Starts assertions for a nullable <c>char?</c> value.</summary>
        public static ValueAssertions<char?> That(this Asserter a, char? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<char?>(p, value);
        }
    }

    public static class CharAssertExtensions
    {
        /// <summary>Asserts that the character equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> Is(this ValueAssertions<char?> a, char? expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        {
            a.Link("Is", expr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() == expected,
                () => $"Expected '{expected}', but was '{a.GetValue()}'",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a decimal digit.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsDigit(this ValueAssertions<char?> a)
        {
            a.Link("IsDigit");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsDigit(c),
                () => $"Expected '{a.GetValue()}' to be a digit",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is not a decimal digit.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsNotDigit(this ValueAssertions<char?> a)
        {
            a.Link("IsNotDigit");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && !char.IsDigit(c),
                () => $"Expected '{a.GetValue()}' not to be a digit",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a number character (includes digits and numeric fractions/superscripts).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsNumber(this ValueAssertions<char?> a)
        {
            a.Link("IsNumber");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsNumber(c),
                () => $"Expected '{a.GetValue()}' to be a number",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a Unicode letter.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsLetter(this ValueAssertions<char?> a)
        {
            a.Link("IsLetter");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsLetter(c),
                () => $"Expected '{a.GetValue()}' to be a letter",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a letter or a decimal digit.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsLetterOrDigit(this ValueAssertions<char?> a)
        {
            a.Link("IsLetterOrDigit");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsLetterOrDigit(c),
                () => $"Expected '{a.GetValue()}' to be a letter or digit",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is whitespace.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsWhiteSpace(this ValueAssertions<char?> a)
        {
            a.Link("IsWhiteSpace");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsWhiteSpace(c),
                () => $"Expected '{a.GetValue()}' to be whitespace",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is an uppercase letter.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsUpper(this ValueAssertions<char?> a)
        {
            a.Link("IsUpper");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsUpper(c),
                () => $"Expected '{a.GetValue()}' to be uppercase",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a lowercase letter.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsLower(this ValueAssertions<char?> a)
        {
            a.Link("IsLower");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsLower(c),
                () => $"Expected '{a.GetValue()}' to be lowercase",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a punctuation mark.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsPunctuation(this ValueAssertions<char?> a)
        {
            a.Link("IsPunctuation");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsPunctuation(c),
                () => $"Expected '{a.GetValue()}' to be punctuation",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a symbol character.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsSymbol(this ValueAssertions<char?> a)
        {
            a.Link("IsSymbol");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsSymbol(c),
                () => $"Expected '{a.GetValue()}' to be a symbol",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a control character.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsControl(this ValueAssertions<char?> a)
        {
            a.Link("IsControl");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsControl(c),
                () => $"Expected '{a.GetValue()}' to be a control character",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is in the ASCII range (code point &lt;= 127).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsAscii(this ValueAssertions<char?> a)
        {
            a.Link("IsAscii");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && c <= 127,
                () => $"Expected '{a.GetValue()}' to be an ASCII character",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a Unicode separator.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsSeparator(this ValueAssertions<char?> a)
        {
            a.Link("IsSeparator");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsSeparator(c),
                () => $"Expected '{a.GetValue()}' to be a separator",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a Unicode surrogate (high or low).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsSurrogate(this ValueAssertions<char?> a)
        {
            a.Link("IsSurrogate");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsSurrogate(c),
                () => $"Expected '{a.GetValue()}' to be a surrogate",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a high surrogate.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsHighSurrogate(this ValueAssertions<char?> a)
        {
            a.Link("IsHighSurrogate");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsHighSurrogate(c),
                () => $"Expected '{a.GetValue()}' to be a high surrogate",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is a low surrogate.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsLowSurrogate(this ValueAssertions<char?> a)
        {
            a.Link("IsLowSurrogate");
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsLowSurrogate(c),
                () => $"Expected '{a.GetValue()}' to be a low surrogate",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character and <paramref name="other"/> form a valid UTF-16 surrogate pair.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsSurrogatePairWith(this ValueAssertions<char?> a, char other,
            [CallerArgumentExpression(nameof(other))] string? otherExpr = null)
        {
            a.Link("IsSurrogatePairWith", otherExpr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && char.IsSurrogatePair(c, other),
                () => $"Expected '{a.GetValue()}' to be a surrogate pair with '{other}'",
                a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the character is in the inclusive range [<paramref name="min"/>, <paramref name="max"/>].</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<char?> IsInRange(this ValueAssertions<char?> a, char min, char max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
        {
            a.Link("IsInRange", minExpr, maxExpr);
            a.Op(a => CheckOperation.Sync(
                () => a.GetValue() is { } c && c >= min && c <= max,
                () => $"Expected '{a.GetValue()}' to be in range ['{min}', '{max}']",
                a.IsSkipped()));
            return a;
        }
    }
}

