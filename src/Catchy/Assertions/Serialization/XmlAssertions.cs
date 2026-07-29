using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Catchy.Sdk;

namespace Catchy
{
    internal static class XmlPathContextSlots
    {
        internal static readonly SlotKey<string> RawXml = new();
        internal static readonly SlotKey<string> CurrentPath = new();

        internal static string GetCurrentPath(AssertionPipeline p)
            => p.Slots.TryGet(CurrentPath, out string path) ? path : string.Empty;

        internal static string GetRawXml(AssertionPipeline p)
            => p.Slots.TryGet(RawXml, out string xml) ? xml : string.Empty;
    }

    public static partial class StringAssertExtensions
    {
        /// <summary>Selects an XML element using <paramref name="xpath"/> from the source XML string.</summary>
        public static ValueAssertions<XElement?> AtXPath(this ValueAssertions<string?> a, string xpath,
            [CallerArgumentExpression(nameof(xpath))] string? expr = null)
        {
            a.Link("AtXPath", expr);
            var pipeline = a.GetPipeline();
            var rawXml = a.GetValue() ?? string.Empty;
            pipeline.Slots.Set(XmlPathContextSlots.RawXml, rawXml);
            pipeline.Slots.Set(XmlPathContextSlots.CurrentPath, xpath);
            return new ValueAssertions<XElement?>(pipeline, XPathNavigator.TryEvaluate(rawXml, xpath));
        }
    }

    public static class XmlAssertExtensions
    {
        /// <summary>Asserts that the selected XML element exists.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> Exists(this ValueAssertions<XElement?> a)
        { a.Link("Exists"); a.Op(a => XmlChecks.Exists(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected XML element does not exist.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> DoesNotExist(this ValueAssertions<XElement?> a)
        { a.Link("DoesNotExist"); a.Op(a => XmlChecks.DoesNotExist(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected XML value equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> HasValue(this ValueAssertions<XElement?> a, string expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasValue", expr); a.Op(a => XmlChecks.HasValue(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected XML value does not equal <paramref name="unexpected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> DoesNotHaveValue(this ValueAssertions<XElement?> a, string unexpected,
            [CallerArgumentExpression(nameof(unexpected))] string? expr = null)
        { a.Link("DoesNotHaveValue", expr); a.Op(a => XmlChecks.DoesNotHaveValue(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), unexpected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected XML element has attribute <paramref name="attributeName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> HasAttribute(this ValueAssertions<XElement?> a, string attributeName,
            [CallerArgumentExpression(nameof(attributeName))] string? expr = null)
        { a.Link("HasAttribute", expr); a.Op(a => XmlChecks.HasAttribute(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), attributeName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected XML element does not have attribute <paramref name="attributeName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> DoesNotHaveAttribute(this ValueAssertions<XElement?> a, string attributeName,
            [CallerArgumentExpression(nameof(attributeName))] string? expr = null)
        { a.Link("DoesNotHaveAttribute", expr); a.Op(a => XmlChecks.DoesNotHaveAttribute(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), attributeName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that attribute <paramref name="attributeName"/> equals <paramref name="expected"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> HasAttributeValue(this ValueAssertions<XElement?> a, string attributeName, string expected,
            [CallerArgumentExpression(nameof(attributeName))] string? nameExpr = null,
            [CallerArgumentExpression(nameof(expected))] string? valueExpr = null)
        { a.Link("HasAttributeValue", nameExpr, valueExpr); a.Op(a => XmlChecks.HasAttributeValue(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), attributeName, expected, a.IsSkipped(), nameExpr, valueExpr)); return a; }

        /// <summary>Asserts that the selected XML element has <paramref name="expected"/> direct children.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> HasChildCount(this ValueAssertions<XElement?> a, int expected,
            [CallerArgumentExpression(nameof(expected))] string? expr = null)
        { a.Link("HasChildCount", expr); a.Op(a => XmlChecks.HasChildCount(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), expected, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected XML element has a child named <paramref name="childName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> HasChild(this ValueAssertions<XElement?> a, string childName,
            [CallerArgumentExpression(nameof(childName))] string? expr = null)
        { a.Link("HasChild", expr); a.Op(a => XmlChecks.HasChild(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), childName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected XML element does not have a child named <paramref name="childName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> DoesNotHaveChild(this ValueAssertions<XElement?> a, string childName,
            [CallerArgumentExpression(nameof(childName))] string? expr = null)
        { a.Link("DoesNotHaveChild", expr); a.Op(a => XmlChecks.DoesNotHaveChild(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), childName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected XML element has no child elements.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> IsEmpty(this ValueAssertions<XElement?> a)
        { a.Link("IsEmpty"); a.Op(a => XmlChecks.IsEmpty(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected XML element has at least one child element.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> IsNotEmpty(this ValueAssertions<XElement?> a)
        { a.Link("IsNotEmpty"); a.Op(a => XmlChecks.IsNotEmpty(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), a.IsSkipped())); return a; }

        /// <summary>Asserts that the selected XML element name equals <paramref name="expectedName"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> HasName(this ValueAssertions<XElement?> a, string expectedName,
            [CallerArgumentExpression(nameof(expectedName))] string? expr = null)
        { a.Link("HasName", expr); a.Op(a => XmlChecks.HasName(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), expectedName, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected XML namespace equals <paramref name="expectedNs"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> HasNamespace(this ValueAssertions<XElement?> a, string expectedNs,
            [CallerArgumentExpression(nameof(expectedNs))] string? expr = null)
        { a.Link("HasNamespace", expr); a.Op(a => XmlChecks.HasNamespace(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), expectedNs, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected XML text contains <paramref name="text"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> ContainsText(this ValueAssertions<XElement?> a, string text,
            [CallerArgumentExpression(nameof(text))] string? expr = null)
        { a.Link("ContainsText", expr); a.Op(a => XmlChecks.ContainsText(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), text, a.IsSkipped(), expr)); return a; }

        /// <summary>Asserts that the selected XML text does not contain <paramref name="text"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<XElement?> DoesNotContainText(this ValueAssertions<XElement?> a, string text,
            [CallerArgumentExpression(nameof(text))] string? expr = null)
        { a.Link("DoesNotContainText", expr); a.Op(a => XmlChecks.DoesNotContainText(a.GetValue(), XmlPathContextSlots.GetCurrentPath(a.GetPipeline()), text, a.IsSkipped(), expr)); return a; }

        /// <summary>Selects a nested XML element using <paramref name="relativePath"/> from the current XML context.</summary>
        public static ValueAssertions<XElement?> AtPath(this ValueAssertions<XElement?> a, string relativePath,
            [CallerArgumentExpression(nameof(relativePath))] string? expr = null)
        {
            a.Link("AtPath", expr);
            var pipeline = a.GetPipeline();
            var currentPath = XmlPathContextSlots.GetCurrentPath(pipeline);
            var rawXml = XmlPathContextSlots.GetRawXml(pipeline);
            var newPath = string.IsNullOrEmpty(currentPath) || currentPath == "/" || currentPath == "." ? relativePath : $"{currentPath}/{relativePath}";
            pipeline.Slots.Set(XmlPathContextSlots.CurrentPath, newPath);
            return new ValueAssertions<XElement?>(pipeline, XPathNavigator.TryEvaluate(rawXml, newPath));
        }
    }
}






