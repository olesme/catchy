using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="Stream"/> value.</summary>
        public static ValueAssertions<Stream?> That(this Asserter a, Stream? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<Stream?>(p, value);
        }
    }

    /// <summary>Provides fluent assertions and projections for <see cref="Stream"/> values.</summary>
    public static class StreamAssertExtensions
    {
        /// <summary>Asserts that the stream is readable.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsReadable(this ValueAssertions<Stream?> a)
        { a.Link("IsReadable"); a.Op(a => StreamChecks.IsReadable(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream is not readable.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsNotReadable(this ValueAssertions<Stream?> a)
        { a.Link("IsNotReadable"); a.Op(a => StreamChecks.IsNotReadable(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream is writable.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsWritable(this ValueAssertions<Stream?> a)
        { a.Link("IsWritable"); a.Op(a => StreamChecks.IsWritable(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream is not writable.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsNotWritable(this ValueAssertions<Stream?> a)
        { a.Link("IsNotWritable"); a.Op(a => StreamChecks.IsNotWritable(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream supports seeking.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsSeekable(this ValueAssertions<Stream?> a)
        { a.Link("IsSeekable"); a.Op(a => StreamChecks.IsSeekable(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream does not support seeking.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsNotSeekable(this ValueAssertions<Stream?> a)
        { a.Link("IsNotSeekable"); a.Op(a => StreamChecks.IsNotSeekable(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream is empty (length equals zero).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsEmpty(this ValueAssertions<Stream?> a)
        { a.Link("IsEmpty"); a.Op(a => StreamChecks.IsEmpty(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream is not empty (length is greater than zero).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsNotEmpty(this ValueAssertions<Stream?> a)
        { a.Link("IsNotEmpty"); a.Op(a => StreamChecks.IsNotEmpty(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream has exactly the given length in bytes.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> HasLength(this ValueAssertions<Stream?> a, long expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasLength", expr); a.Op(a => StreamChecks.HasLength(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the stream length is greater than <paramref name="expected"/> bytes.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> HasLengthGreaterThan(this ValueAssertions<Stream?> a, long expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasLengthGreaterThan", expr); a.Op(a => StreamChecks.HasLengthGreaterThan(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the stream length is less than <paramref name="expected"/> bytes.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> HasLengthLessThan(this ValueAssertions<Stream?> a, long expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasLengthLessThan", expr); a.Op(a => StreamChecks.HasLengthLessThan(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the stream length is between <paramref name="min"/> and <paramref name="max"/> bytes (inclusive).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> HasLengthInRange(this ValueAssertions<Stream?> a, long min, long max,
            [CallerArgumentExpression(nameof(min))] string? minExpr = null,
            [CallerArgumentExpression(nameof(max))] string? maxExpr = null)
        { a.Link("HasLengthInRange", minExpr, maxExpr); a.Op(a => StreamChecks.HasLengthInRange(a.GetValue(), min, max, a.IsSkipped(), minExpr, maxExpr)); return a; }

        /// <summary>Asserts that the stream position is at the start (zero).</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsAtStart(this ValueAssertions<Stream?> a)
        { a.Link("IsAtStart"); a.Op(a => StreamChecks.IsAtStart(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream position is not at the start.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsNotAtStart(this ValueAssertions<Stream?> a)
        { a.Link("IsNotAtStart"); a.Op(a => StreamChecks.IsNotAtStart(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream position is at the end.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsAtEnd(this ValueAssertions<Stream?> a)
        { a.Link("IsAtEnd"); a.Op(a => StreamChecks.IsAtEnd(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream position is not at the end.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsNotAtEnd(this ValueAssertions<Stream?> a)
        { a.Link("IsNotAtEnd"); a.Op(a => StreamChecks.IsNotAtEnd(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream position equals the given offset.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> HasPosition(this ValueAssertions<Stream?> a, long expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasPosition", expr); a.Op(a => StreamChecks.HasPosition(a.GetValue(), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the stream supports timeout operations.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> CanTimeout(this ValueAssertions<Stream?> a)
        { a.Link("CanTimeout"); a.Op(a => StreamChecks.CanTimeout(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream does not support timeout operations.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> CannotTimeout(this ValueAssertions<Stream?> a)
        { a.Link("CannotTimeout"); a.Op(a => StreamChecks.CannotTimeout(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream can be read.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> CanRead(this ValueAssertions<Stream?> a)
        { a.Link("CanRead"); a.Op(a => StreamChecks.CanRead(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream can be written.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> CanWrite(this ValueAssertions<Stream?> a)
        { a.Link("CanWrite"); a.Op(a => StreamChecks.CanWrite(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream supports seeking.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> CanSeek(this ValueAssertions<Stream?> a)
        { a.Link("CanSeek"); a.Op(a => StreamChecks.CanSeek(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the stream is closed.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Stream?> IsClosed(this ValueAssertions<Stream?> a)
        { a.Link("IsClosed"); a.Op(a => StreamChecks.IsClosed(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Projects the stream length.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<long?> Length(this ValueAssertions<Stream?> a)
        {
            a.Link("Length");
            return new ValueAssertions<long?>(a.GetPipeline(), a.GetValue()?.Length);
        }

        /// <summary>Projects the stream position.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<long?> Position(this ValueAssertions<Stream?> a)
        {
            a.Link("Position");
            return new ValueAssertions<long?>(a.GetPipeline(), a.GetValue()?.Position);
        }
    }
}

