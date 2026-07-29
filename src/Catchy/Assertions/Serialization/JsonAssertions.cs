using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Catchy.Sdk;

namespace Catchy
{
    internal static class JsonPathContextSlots
    {
        internal static readonly SlotKey<string> RawJson = new();
        internal static readonly SlotKey<string> CurrentPath = new();

        internal static string GetCurrentPath(AssertionPipeline p)
            => p.Slots.TryGet(CurrentPath, out string path) ? path : string.Empty;

        internal static string GetRawJson(AssertionPipeline p)
            => p.Slots.TryGet(RawJson, out string json) ? json : string.Empty;
    }

    /// <summary>String-based JSON entry points.</summary>
    public static partial class StringAssertExtensions
    {
        /// <summary>Projects a JSON element selected by JSONPath from the current string value.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<JsonElement?> AtJsonPath(this ValueAssertions<string?> a, string path,
            [CallerArgumentExpression(nameof(path))] string? expr = null)
        {
            a.Link("AtJsonPath", expr);
            var pipeline = a.GetPipeline();
            var rawJson = a.GetValue() ?? string.Empty;
            pipeline.Slots.Set(JsonPathContextSlots.RawJson, rawJson);
            pipeline.Slots.Set(JsonPathContextSlots.CurrentPath, path);
            return new ValueAssertions<JsonElement?>(pipeline, JsonPathNavigator.TryEvaluate(rawJson, path));
        }
    }

    /// <summary>Provides fluent JSON path-based assertions for <see cref="JsonElement"/> values.</summary>
    public static class JsonAssertExtensions
    {
        /// <summary>Asserts that the current JSON path resolves to an element.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> Exists(this ValueAssertions<JsonElement?> a)
        { a.Link("Exists"); a.Op(a => JsonChecks.Exists(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the current JSON path does not resolve to an element.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> DoesNotExist(this ValueAssertions<JsonElement?> a)
        { a.Link("DoesNotExist"); a.Op(a => JsonChecks.DoesNotExist(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected JSON value equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> HasValue(this ValueAssertions<JsonElement?> a, object? expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasValue", expr); a.Op(a => JsonChecks.HasValue(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected JSON value does not equal <paramref name="unexpected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> DoesNotHaveValue(this ValueAssertions<JsonElement?> a, object? unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("DoesNotHaveValue", expr); a.Op(a => JsonChecks.DoesNotHaveValue(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), unexpected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected JSON value is null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> IsNull(this ValueAssertions<JsonElement?> a)
        { a.Link("IsNull"); a.Op(a => JsonChecks.IsNull(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected JSON value is not null.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> IsNotNull(this ValueAssertions<JsonElement?> a)
        { a.Link("IsNotNull"); a.Op(a => JsonChecks.IsNotNull(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected JSON value is a string.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> IsString(this ValueAssertions<JsonElement?> a)
        { a.Link("IsString"); a.Op(a => JsonChecks.IsString(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected JSON value is a number.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> IsNumber(this ValueAssertions<JsonElement?> a)
        { a.Link("IsNumber"); a.Op(a => JsonChecks.IsNumber(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected JSON value is a boolean.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> IsBoolean(this ValueAssertions<JsonElement?> a)
        { a.Link("IsBoolean"); a.Op(a => JsonChecks.IsBoolean(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected JSON value is an array.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> IsArray(this ValueAssertions<JsonElement?> a)
        { a.Link("IsArray"); a.Op(a => JsonChecks.IsArray(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected JSON value is an object.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> IsObject(this ValueAssertions<JsonElement?> a)
        { a.Link("IsObject"); a.Op(a => JsonChecks.IsObject(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected JSON array has the expected length.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> HasArrayLength(this ValueAssertions<JsonElement?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasArrayLength", expr); a.Op(a => JsonChecks.HasArrayLength(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected JSON value is <c>true</c>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> IsTrue(this ValueAssertions<JsonElement?> a)
        { a.Link("IsTrue"); a.Op(a => JsonChecks.IsTrue(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected JSON value is <c>false</c>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<JsonElement?> IsFalse(this ValueAssertions<JsonElement?> a)
        { a.Link("IsFalse"); a.Op(a => JsonChecks.IsFalse(a.GetValue(), JsonPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Projects a nested JSON element using a path relative to the current selection.</summary>
        [DebuggerHidden, StackTraceHidden]
        public static ValueAssertions<JsonElement?> AtPath(this ValueAssertions<JsonElement?> a, string relativePath,
            [CallerArgumentExpression(nameof(relativePath))] string? expr = null)
        {
            a.Link("AtPath", expr);
            var pipeline = a.GetPipeline();
            var currentPath = JsonPathContextSlots.GetCurrentPath(pipeline);
            var rawJson = JsonPathContextSlots.GetRawJson(pipeline);
            var newPath = string.IsNullOrEmpty(currentPath) ? relativePath : $"{currentPath}.{relativePath}";
            pipeline.Slots.Set(JsonPathContextSlots.CurrentPath, newPath);
            return new ValueAssertions<JsonElement?>(pipeline, JsonPathNavigator.TryEvaluate(rawJson, newPath));
        }
    }
}

