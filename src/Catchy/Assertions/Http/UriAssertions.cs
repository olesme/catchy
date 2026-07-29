using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;

namespace Catchy
{
    /// <summary>Entry point for URI assertions.</summary>
    public static partial class AsserterExtensions
    {
        /// <summary>Starts assertions for a <see cref="Uri"/> value.</summary>
        public static ValueAssertions<Uri?> That(this Asserter a, Uri? value,
            [CallerArgumentExpression(nameof(a))] string? aExpr = null,
            [CallerArgumentExpression(nameof(value))] string? vExpr = null,
            [CallerFilePath] string? file = null,
            [CallerLineNumber] int line = 0,
            [CallerMemberName] string? member = null)
        {
            var p = a.NewPipeline(asserterExpr: aExpr, methodName: "That", valueExpr: vExpr, file: file, line: line, member: member);
            return new ValueAssertions<Uri?>(p, value);
        }
    }

    /// <summary>Provides fluent assertions for URIs, schemes, hosts, ports, paths, and query parameters.</summary>
    public static class UriAssertExtensions
    {
        /// <summary>Asserts that the URI is absolute.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> IsAbsolute(this ValueAssertions<Uri?> a)
        { a.Link("IsAbsolute"); a.Op(a => UriChecks.IsAbsolute(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the URI is relative.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> IsRelative(this ValueAssertions<Uri?> a)
        { a.Link("IsRelative"); a.Op(a => UriChecks.IsRelative(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Asserts that the URI is absolute.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> IsAbsoluteUri(this ValueAssertions<Uri?> a)
        { a.Link("IsAbsoluteUri"); a.Op(a => UriChecks.IsAbsolute(a.GetValue(), a.IsSkipped())); return a; }

        /// <summary>Projects the URI scheme (e.g., "http", "https").</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string> Scheme(this ValueAssertions<Uri?> a)
        {
            a.Link("Scheme");
            return new ValueAssertions<string>(a.GetPipeline(), a.GetValue()?.Scheme!);
        }

        /// <summary>Projects the URI host name.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string> Host(this ValueAssertions<Uri?> a)
        {
            a.Link("Host");
            return new ValueAssertions<string>(a.GetPipeline(), a.GetValue()?.Host!);
        }

        /// <summary>Projects the URI query parameters as a collection of strings.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<IEnumerable<string>?> Query(this ValueAssertions<Uri?> a)
        {
            a.Link("Query");
            var q = a.GetValue()?.Query?.TrimStart('?');
            var items = string.IsNullOrEmpty(q) ? System.Array.Empty<string>() : q!.Split('&');
            return new ValueAssertions<IEnumerable<string>?>(a.GetPipeline(), items);
        }

        /// <summary>Projects the URI path.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string> Path(this ValueAssertions<Uri?> a)
        {
            a.Link("Path");
            return new ValueAssertions<string>(a.GetPipeline(), a.GetValue()?.AbsolutePath!);
        }

        /// <summary>Projects the original URI string.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<string?> OriginalString(this ValueAssertions<Uri?> a)
        {
            a.Link("OriginalString");
            return new ValueAssertions<string?>(a.GetPipeline(), a.GetValue()?.OriginalString);
        }

        /// <summary>Projects the URI port number.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<int?> Port(this ValueAssertions<Uri?> a)
        {
            a.Link("Port");
            var value = a.GetValue();
            return new ValueAssertions<int?>(a.GetPipeline(), value is null ? (int?)null : value.Port);
        }

        /// <summary>Asserts that the URI has the specified scheme.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> HasScheme(this ValueAssertions<Uri?> a, string scheme,
            [CallerArgumentExpression(nameof(scheme))] string? expr = null)
        { a.Link("HasScheme", expr); a.Op(a => UriChecks.HasScheme(a.GetValue(), scheme, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI does not have the specified scheme.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> DoesNotHaveScheme(this ValueAssertions<Uri?> a, string scheme,
            [CallerArgumentExpression(nameof(scheme))] string? expr = null)
        { a.Link("DoesNotHaveScheme", expr); a.Op(a => UriChecks.DoesNotHaveScheme(a.GetValue(), scheme, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI has the specified host.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> HasHost(this ValueAssertions<Uri?> a, string host,
            [CallerArgumentExpression(nameof(host))] string? expr = null)
        { a.Link("HasHost", expr); a.Op(a => UriChecks.HasHost(a.GetValue(), host, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI does not have the specified host.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> DoesNotHaveHost(this ValueAssertions<Uri?> a, string host,
            [CallerArgumentExpression(nameof(host))] string? expr = null)
        { a.Link("DoesNotHaveHost", expr); a.Op(a => UriChecks.DoesNotHaveHost(a.GetValue(), host, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI has the specified port.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> HasPort(this ValueAssertions<Uri?> a, int port,
            [CallerArgumentExpression(nameof(port))] string? expr = null)
        { a.Link("HasPort", expr); a.Op(a => UriChecks.HasPort(a.GetValue(), port, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI does not have the specified port.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> DoesNotHavePort(this ValueAssertions<Uri?> a, int port,
            [CallerArgumentExpression(nameof(port))] string? expr = null)
        { a.Link("DoesNotHavePort", expr); a.Op(a => UriChecks.DoesNotHavePort(a.GetValue(), port, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI has the specified path.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> HasPath(this ValueAssertions<Uri?> a, string path,
            [CallerArgumentExpression(nameof(path))] string? expr = null)
        { a.Link("HasPath", expr); a.Op(a => UriChecks.HasPath(a.GetValue(), path, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI does not have the specified path.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> DoesNotHavePath(this ValueAssertions<Uri?> a, string path,
            [CallerArgumentExpression(nameof(path))] string? expr = null)
        { a.Link("DoesNotHavePath", expr); a.Op(a => UriChecks.DoesNotHavePath(a.GetValue(), path, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI has the specified query parameter.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> HasQueryParameter(this ValueAssertions<Uri?> a, string paramName,
            [CallerArgumentExpression(nameof(paramName))] string? expr = null)
        { a.Link("HasQueryParameter", expr); a.Op(a => UriChecks.HasQueryParameter(a.GetValue(), paramName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI does not have the specified query parameter.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> DoesNotHaveQueryParameter(this ValueAssertions<Uri?> a, string paramName,
            [CallerArgumentExpression(nameof(paramName))] string? expr = null)
        { a.Link("DoesNotHaveQueryParameter", expr); a.Op(a => UriChecks.DoesNotHaveQueryParameter(a.GetValue(), paramName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI has a query parameter with the specified name and value.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> HasQueryParameterValue(this ValueAssertions<Uri?> a, string paramName, string expectedValue,
            [CallerArgumentExpression(nameof(paramName))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(expectedValue))] string? valueExpr = null)
        { a.Link("HasQueryParameterValue", nameExpr, valueExpr); a.Op(a => UriChecks.HasQueryParameterValue(a.GetValue(), paramName, expectedValue, a.IsSkipped(), nameExpr, valueExpr)); return a; }

        /// <summary>Asserts that the URI has the specified fragment.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> HasFragment(this ValueAssertions<Uri?> a, string fragment,
            [CallerArgumentExpression(nameof(fragment))] string? expr = null)
        { a.Link("HasFragment", expr); a.Op(a => UriChecks.HasFragment(a.GetValue(), fragment, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the URI does not have the specified fragment.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<Uri?> DoesNotHaveFragment(this ValueAssertions<Uri?> a, string fragment,
            [CallerArgumentExpression(nameof(fragment))] string? expr = null)
        { a.Link("DoesNotHaveFragment", expr); a.Op(a => UriChecks.DoesNotHaveFragment(a.GetValue(), fragment, a.IsSkipped(), expr)); return a; }
    }
}

