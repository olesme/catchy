using System.Text.Json;

namespace Catchy.Sdk
{
    public static class JsonDocumentChecks
    {
        public static CheckOperation IsNull(JsonDocument? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is null,
                () => "Expected JsonDocument to be null",
                isSkipped);

        public static CheckOperation IsNotNull(JsonDocument? actual, bool isSkipped)
            => CheckOperation.Sync(
                () => actual is not null,
                () => "Expected JsonDocument to be non-null",
                isSkipped);

        public static CheckOperation Is(JsonDocument? actual, string? expectedJson, Func<StringComparison> getComparison, bool isSkipped, string? expectedExpr = null)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null && expectedJson is null) return true;
                    if (actual is null || expectedJson is null) return false;
                    try
                    {
                        using var expDoc = JsonDocument.Parse(expectedJson);
                        return JsonChecks.JsonElementsEqual(actual.RootElement, expDoc.RootElement);
                    }
                    catch
                    {
                        // fallback to string equality
                        return string.Equals(actual.RootElement.GetRawText(), expectedJson, getComparison());
                    }
                },
                () => actual is null
                    ? $"Expected JSON document to equal {ExprFormat.Inline(expectedJson, expectedExpr)}, but was null"
                    : $"Expected JSON document to equal {ExprFormat.Inline(expectedJson, expectedExpr)}",
                isSkipped);

        public static CheckOperation IsNot(JsonDocument? actual, string? unexpectedJson, Func<StringComparison> getComparison, bool isSkipped, string? unexpectedExpr = null)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null && unexpectedJson is null) return false;
                    if (actual is null || unexpectedJson is null) return true;
                    try
                    {
                        using var expDoc = JsonDocument.Parse(unexpectedJson);
                        return !JsonChecks.JsonElementsEqual(actual.RootElement, expDoc.RootElement);
                    }
                    catch
                    {
                        return !string.Equals(actual.RootElement.GetRawText(), unexpectedJson, getComparison());
                    }
                },
                () => $"Expected JSON document not to equal {ExprFormat.Inline(unexpectedJson, unexpectedExpr)}",
                isSkipped);

        public static CheckOperation IsEquivalentTo(JsonDocument? actual, object? expected, EqualsOptions? opts, DeepEqualRuleContainer? localRules, string? expectedExpr, bool isSkipped)
            => CheckOperation.Sync(
                () =>
                {
                    if (actual is null || expected is null) return actual is null && expected is null;
                    // If expected is provided as a JSON string, compare by JSON structure directly.
                    if (expected is string s)
                    {
                        try
                        {
                            using var expDoc = JsonDocument.Parse(s);
                            return JsonChecks.JsonElementsEqual(actual.RootElement, expDoc.RootElement);
                        }
                        catch
                        {
                            return string.Equals(actual.RootElement.GetRawText(), s, StringComparison.Ordinal);
                        }
                    }

                    // Otherwise, try to use the DeepEqual engine by registering a transient per-container rule
                    // that knows how to compare a JsonElement to the concrete expected type by serializing
                    // the expected object to JSON and comparing the resulting JsonElement structures.
                    var optsEffective = opts ?? new EqualsOptions();
                    var container = localRules?.Clone() ?? new DeepEqualRuleContainer();
                    var expectedType = expected.GetType();

                    var compareMethodDef = typeof(JsonDocumentChecks).GetMethod(nameof(CompareJsonElementToExpectedGeneric), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!;
                    var compareConcrete = compareMethodDef.MakeGenericMethod(expectedType);
                    var delType = typeof(Func<,,>).MakeGenericType(typeof(JsonElement), expectedType, typeof(bool));
                    var del = System.Delegate.CreateDelegate(delType, compareConcrete);

                    var addMethod = typeof(DeepEqualRuleContainer).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                        .First(m => m.Name == "Add" && m.IsGenericMethodDefinition && m.GetParameters().Length >= 1 && m.GetParameters()[0].ParameterType.IsGenericType && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<,,>));
                    var addConcrete = addMethod.MakeGenericMethod(typeof(JsonElement), expectedType);
                    addConcrete.Invoke(container, new object[] { del, true });

                    // Call into DeepEqualEngine with the JsonElement (boxed) and the expected object.
                    return DeepEqualEngine.AreEqualObjects(actual.RootElement, expected, optsEffective, container);
                },
                () =>
                {
                    if (actual is null) return "Expected a JSON document, but was null";
                    if (expected is null) return $"Expected equivalent to null, but was {ValueFormatter.Format(actual)}";
                    return $"Expected JSON document to be equivalent to {ExprFormat.Inline(expected, expectedExpr)}";
                },
                isSkipped);

        public static CheckOperation IsNotEquivalentTo(JsonDocument? actual, object? unexpected, EqualsOptions? opts, DeepEqualRuleContainer? localRules, string? unexpectedExpr, bool isSkipped)
            => CheckOperation.Sync(
                () => !IsEquivalentTo(actual, unexpected, opts, localRules, unexpectedExpr, isSkipped).PassesSync!(),
                () => $"Expected JSON document not to be equivalent to {ExprFormat.Inline(unexpected, unexpectedExpr)}",
                isSkipped);

        private static bool CompareJsonElementToExpectedGeneric<TExpected>(JsonElement left, TExpected expected)
        {
            if (expected is null) return left.ValueKind == JsonValueKind.Null;
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(expected);
                using var doc = JsonDocument.Parse(json);
                return JsonChecks.JsonElementsEqual(left, doc.RootElement);
            }
            catch
            {
                return false;
            }
        }
    }
}
