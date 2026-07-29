using System.Xml.Linq;

namespace Catchy.Sdk
{
    public static class XmlChecks
    {
        public static CheckOperation Exists(XElement? element, string xpath, bool isSkipped)
            => CheckOperation.Sync(
                () => element is not null,
                () => $"Expected XPath '{xpath}' to exist",
                isSkipped);

        public static CheckOperation DoesNotExist(XElement? element, string xpath, bool isSkipped)
            => CheckOperation.Sync(
                () => element is null,
                () => $"Expected XPath '{xpath}' not to exist, but it does",
                isSkipped);

        public static CheckOperation HasValue(XElement? element, string xpath, string expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element is not null && element.Value == expected,
                () => element is null
                    ? $"Expected XPath '{xpath}' to exist"
                    : $"Expected XPath '{xpath}' to have value {ExprFormat.Inline(expected, expr)}, but was \"{element.Value}\"",
                isSkipped);

        public static CheckOperation DoesNotHaveValue(XElement? element, string xpath, string unexpected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element is null || element.Value != unexpected,
                () => $"Expected XPath '{xpath}' not to have value {ExprFormat.Inline(unexpected, expr)}",
                isSkipped);

        public static CheckOperation HasAttribute(XElement? element, string xpath, string attributeName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element?.Attribute(attributeName) is not null,
                () => element is null
                    ? $"Expected XPath '{xpath}' to exist"
                    : $"Expected element at '{xpath}' to have attribute {ExprFormat.Inline(attributeName, expr)}",
                isSkipped);

        public static CheckOperation DoesNotHaveAttribute(XElement? element, string xpath, string attributeName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element is null || element.Attribute(attributeName) is null,
                () => $"Expected element at '{xpath}' not to have attribute {ExprFormat.Inline(attributeName, expr)}",
                isSkipped);

        public static CheckOperation HasAttributeValue(XElement? element, string xpath, string attributeName, string expected, bool isSkipped, string? nameExpr = null, string? valueExpr = null)
            => CheckOperation.Sync(
                () => element?.Attribute(attributeName)?.Value == expected,
                () =>
                {
                    if (element is null) return $"Expected XPath '{xpath}' to exist";
                    var attr = element.Attribute(attributeName);
                    if (attr is null) return $"Expected element at '{xpath}' to have attribute {ExprFormat.Inline(attributeName, nameExpr)}";
                    return $"Expected attribute '{attributeName}' at '{xpath}' to be {ExprFormat.Inline(expected, valueExpr)}, but was \"{attr.Value}\"";
                },
                isSkipped);

        public static CheckOperation HasChildCount(XElement? element, string xpath, int expected, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element is not null && element.Elements().Count() == expected,
                () => element is null
                    ? $"Expected XPath '{xpath}' to exist"
                    : $"Expected element at '{xpath}' to have {ExprFormat.Inline(expected, expr)} children, but had {element.Elements().Count()}",
                isSkipped);

        public static CheckOperation HasChild(XElement? element, string xpath, string childName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element?.Elements().Any(e => e.Name.LocalName == childName) == true,
                () => element is null
                    ? $"Expected XPath '{xpath}' to exist"
                    : $"Expected element at '{xpath}' to have child {ExprFormat.Inline(childName, expr)}",
                isSkipped);

        public static CheckOperation DoesNotHaveChild(XElement? element, string xpath, string childName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element is null || !element.Elements().Any(e => e.Name.LocalName == childName),
                () => $"Expected element at '{xpath}' not to have child {ExprFormat.Inline(childName, expr)}",
                isSkipped);

        public static CheckOperation IsEmpty(XElement? element, string xpath, bool isSkipped)
            => CheckOperation.Sync(
                () => element is not null && !element.HasElements && string.IsNullOrEmpty(element.Value),
                () => element is null
                    ? $"Expected XPath '{xpath}' to exist"
                    : $"Expected element at '{xpath}' to be empty",
                isSkipped);

        public static CheckOperation IsNotEmpty(XElement? element, string xpath, bool isSkipped)
            => CheckOperation.Sync(
                () => element is not null && (element.HasElements || !string.IsNullOrEmpty(element.Value)),
                () => element is null
                    ? $"Expected XPath '{xpath}' to exist"
                    : $"Expected element at '{xpath}' not to be empty",
                isSkipped);

        public static CheckOperation HasName(XElement? element, string xpath, string expectedName, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element is not null && element.Name.LocalName == expectedName,
                () => element is null
                    ? $"Expected XPath '{xpath}' to exist"
                    : $"Expected element at '{xpath}' to have name {ExprFormat.Inline(expectedName, expr)}, but was \"{element.Name.LocalName}\"",
                isSkipped);

        public static CheckOperation HasNamespace(XElement? element, string xpath, string expectedNs, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element is not null && element.Name.NamespaceName == expectedNs,
                () => element is null
                    ? $"Expected XPath '{xpath}' to exist"
                    : $"Expected element at '{xpath}' to have namespace {ExprFormat.Inline(expectedNs, expr)}, but was \"{element.Name.NamespaceName}\"",
                isSkipped);

        public static CheckOperation ContainsText(XElement? element, string xpath, string text, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element is not null && element.Value.Contains(text, StringComparison.Ordinal),
                () => element is null
                    ? $"Expected XPath '{xpath}' to exist"
                    : $"Expected element at '{xpath}' to contain text {ExprFormat.Inline(text, expr)}",
                isSkipped);

        public static CheckOperation DoesNotContainText(XElement? element, string xpath, string text, bool isSkipped, string? expr = null)
            => CheckOperation.Sync(
                () => element is null || !element.Value.Contains(text, StringComparison.Ordinal),
                () => $"Expected element at '{xpath}' not to contain text {ExprFormat.Inline(text, expr)}",
                isSkipped);
    }

    public static class XPathNavigator
    {
        public static XElement? TryEvaluate(string xml, string xpath)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                return Navigate(doc.Root, xpath);
            }
            catch { return null; }
        }

        private static XElement? Navigate(XElement? root, string xpath)
        {
            if (root is null) return null;
            if (string.IsNullOrEmpty(xpath) || xpath == "/" || xpath == ".") return root;

            var path = xpath.TrimStart('/');
            var current = root;

            foreach (var segment in path.Split('/'))
            {
                if (string.IsNullOrEmpty(segment) || segment == ".") continue;

                var indexMatch = System.Text.RegularExpressions.Regex.Match(segment, @"^(.+)\[(\d+)\]$");
                if (indexMatch.Success)
                {
                    var name = indexMatch.Groups[1].Value;
                    var idx = int.Parse(indexMatch.Groups[2].Value) - 1; // XPath is 1-based
                    var elements = current.Elements().Where(e => e.Name.LocalName == name).ToArray();
                    if (idx < 0 || idx >= elements.Length) return null;
                    current = elements[idx];
                }
                else
                {
                    // Match by local name to handle default XML namespaces transparently
                    current = current.Elements().FirstOrDefault(e => e.Name.LocalName == segment);
                    if (current is null) return null;
                }
            }
            return current;
        }
    }
}
