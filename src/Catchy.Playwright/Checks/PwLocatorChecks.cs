using System.Text.RegularExpressions;
using Microsoft.Playwright;
using PW = Microsoft.Playwright.Assertions;

namespace Catchy.Sdk
{
    public static class PwLocatorChecks
    {
        // State (PW-delegating)
        public static CheckOperation IsVisible(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToBeVisibleOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToBeVisibleAsync(o) : PW.Expect(loc).ToBeVisibleAsync(o); }, not ? "Expected locator not to be visible" : "Expected locator to be visible");

        public static CheckOperation IsHidden(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToBeHiddenOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToBeHiddenAsync(o) : PW.Expect(loc).ToBeHiddenAsync(o); }, not ? "Expected locator not to be hidden" : "Expected locator to be hidden");

        public static CheckOperation IsAttached(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToBeAttachedOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToBeAttachedAsync(o) : PW.Expect(loc).ToBeAttachedAsync(o); }, not ? "Expected locator not to be attached" : "Expected locator to be attached");

        public static CheckOperation IsChecked(ILocator loc, bool not, bool? checkedValue, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToBeCheckedOptions { Timeout = timeoutMsGetter(), Checked = checkedValue }; return not ? PW.Expect(loc).Not.ToBeCheckedAsync(o) : PW.Expect(loc).ToBeCheckedAsync(o); }, not ? "Expected locator not to be checked" : "Expected locator to be checked");

        public static CheckOperation IsDisabled(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToBeDisabledOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToBeDisabledAsync(o) : PW.Expect(loc).ToBeDisabledAsync(o); }, not ? "Expected locator not to be disabled" : "Expected locator to be disabled");

        public static CheckOperation IsEnabled(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToBeEnabledOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToBeEnabledAsync(o) : PW.Expect(loc).ToBeEnabledAsync(o); }, not ? "Expected locator not to be enabled" : "Expected locator to be enabled");

        public static CheckOperation IsEditable(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToBeEditableOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToBeEditableAsync(o) : PW.Expect(loc).ToBeEditableAsync(o); }, not ? "Expected locator not to be editable" : "Expected locator to be editable");

        public static CheckOperation IsEmpty(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToBeEmptyOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToBeEmptyAsync(o) : PW.Expect(loc).ToBeEmptyAsync(o); }, not ? "Expected locator not to be empty" : "Expected locator to be empty");

        public static CheckOperation IsFocused(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToBeFocusedOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToBeFocusedAsync(o) : PW.Expect(loc).ToBeFocusedAsync(o); }, not ? "Expected locator not to be focused" : "Expected locator to be focused");

        // IsInViewport - lazy slot читання
        public static CheckOperation IsInViewport(ILocator loc, bool not, Func<float?> timeoutMsGetter, Func<float?> ratioGetter, bool isSkipped)
            => PwOp(isSkipped, () =>
            {
                var ratio = ratioGetter(); // LAZY!
                var o = new LocatorAssertionsToBeInViewportOptions { Timeout = timeoutMsGetter(), Ratio = ratio };
                return not ? PW.Expect(loc).Not.ToBeInViewportAsync(o) : PW.Expect(loc).ToBeInViewportAsync(o);
            }, not ? "Expected locator not to be in viewport" : "Expected locator to be in viewport");

        // Text (with lazy slots for UseInnerText)
        public static CheckOperation HasText(ILocator loc, string expected, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMsGetter, Func<bool?> useInnerTextGetter, bool isSkipped)
        {
            return PwOp(isSkipped, () =>
            {
                var o = new LocatorAssertionsToHaveTextOptions
                {
                    Timeout = timeoutMsGetter(),
                    IgnoreCase = IsIgnoreCase(cmp()),
                    UseInnerText = useInnerTextGetter() // LAZY!
                };
                return not ? PW.Expect(loc).Not.ToHaveTextAsync(expected, o)
                           : PW.Expect(loc).ToHaveTextAsync(expected, o);
            }, not ? $"Expected locator not to have text \"{expected}\"" : $"Expected locator to have text \"{expected}\"");
        }

        public static CheckOperation HasText(ILocator loc, Regex pattern, bool not,
            Func<float?> timeoutMsGetter, Func<bool?> useInnerTextGetter, bool isSkipped)
        {
            return PwOp(isSkipped, () =>
            {
                var o = new LocatorAssertionsToHaveTextOptions
                {
                    Timeout = timeoutMsGetter(),
                    UseInnerText = useInnerTextGetter() // LAZY!
                };
                return not ? PW.Expect(loc).Not.ToHaveTextAsync(pattern, o)
                           : PW.Expect(loc).ToHaveTextAsync(pattern, o);
            }, not ? $"Expected locator not to match text /{pattern}/" : $"Expected locator to match text /{pattern}/");
        }

        public static CheckOperation HasText(ILocator loc, IEnumerable<string> expected, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMsGetter, Func<bool?> useInnerTextGetter, bool isSkipped)
        {
            return PwOp(isSkipped, () =>
            {
                var o = new LocatorAssertionsToHaveTextOptions
                {
                    Timeout = timeoutMsGetter(),
                    IgnoreCase = IsIgnoreCase(cmp()),
                    UseInnerText = useInnerTextGetter() // LAZY!
                };
                return not ? PW.Expect(loc).Not.ToHaveTextAsync(expected, o)
                           : PW.Expect(loc).ToHaveTextAsync(expected, o);
            }, not ? "Expected locator not to have specified texts" : "Expected locator to have specified texts");
        }

        public static CheckOperation ContainsText(ILocator loc, string expected, bool not,
            Func<StringComparison> cmp, Func<float?> timeoutMsGetter, Func<bool?> useInnerTextGetter, bool isSkipped)
        {
            return PwOp(isSkipped, () =>
            {
                var o = new LocatorAssertionsToContainTextOptions
                {
                    Timeout = timeoutMsGetter(),
                    IgnoreCase = IsIgnoreCase(cmp()),
                    UseInnerText = useInnerTextGetter() // LAZY!
                };
                return not ? PW.Expect(loc).Not.ToContainTextAsync(expected, o)
                           : PW.Expect(loc).ToContainTextAsync(expected, o);
            }, not ? $"Expected locator not to contain \"{expected}\"" : $"Expected locator to contain \"{expected}\"");
        }

        public static CheckOperation ContainsText(ILocator loc, Regex pattern, bool not,
            Func<float?> timeoutMsGetter, Func<bool?> useInnerTextGetter, bool isSkipped)
        {
            return PwOp(isSkipped, () =>
            {
                var o = new LocatorAssertionsToContainTextOptions
                {
                    Timeout = timeoutMsGetter(),
                    UseInnerText = useInnerTextGetter() // LAZY!
                };
                return not ? PW.Expect(loc).Not.ToContainTextAsync(pattern, o)
                           : PW.Expect(loc).ToContainTextAsync(pattern, o);
            }, not ? $"Expected locator not to match /{pattern}/" : $"Expected locator to match /{pattern}/");
        }

        // Attributes
        public static CheckOperation HasAttribute(ILocator loc, string name, string value, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveAttributeOptions { Timeout = timeoutMsGetter(), IgnoreCase = IsIgnoreCase(cmp()) }; return not ? PW.Expect(loc).Not.ToHaveAttributeAsync(name, value, o) : PW.Expect(loc).ToHaveAttributeAsync(name, value, o); }, not ? $"Expected locator not to have attribute {name}=\"{value}\"" : $"Expected locator to have attribute {name}=\"{value}\"");

        public static CheckOperation HasAttribute(ILocator loc, string name, Regex pattern, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveAttributeOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveAttributeAsync(name, pattern, o) : PW.Expect(loc).ToHaveAttributeAsync(name, pattern, o); }, not ? $"Expected locator not to have attribute {name} matching /{pattern}/" : $"Expected locator to have attribute {name} matching /{pattern}/");

        public static CheckOperation AttributePresent(ILocator loc, string name, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? val = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { val = await loc.GetAttributeAsync(name).ConfigureAwait(false); return not ? val is null : val is not null; }, timeoutMsGetter ).ConfigureAwait(false),
                () => not ? $"Expected attribute \"{name}\" not to be present" : $"Expected attribute \"{name}\" to be present", isSkipped);
        }

        // Class / CSS / ID
        public static CheckOperation HasClass(ILocator loc, string expected, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveClassOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveClassAsync(expected, o) : PW.Expect(loc).ToHaveClassAsync(expected, o); }, not ? $"Expected locator not to have class \"{expected}\"" : $"Expected locator to have class \"{expected}\"");

        public static CheckOperation HasClass(ILocator loc, Regex pattern, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveClassOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveClassAsync(pattern, o) : PW.Expect(loc).ToHaveClassAsync(pattern, o); }, not ? $"Expected locator not to have class matching /{pattern}/" : $"Expected locator to have class matching /{pattern}/");

        public static CheckOperation ContainsClass(ILocator loc, string className, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToContainClassOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToContainClassAsync(className, o) : PW.Expect(loc).ToContainClassAsync(className, o); }, not ? $"Expected locator not to contain class \"{className}\"" : $"Expected locator to contain class \"{className}\"");

        public static CheckOperation HasCss(ILocator loc, string name, string value, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveCSSOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveCSSAsync(name, value, o) : PW.Expect(loc).ToHaveCSSAsync(name, value, o); }, not ? $"Expected locator not to have CSS {name}: {value}" : $"Expected locator to have CSS {name}: {value}");

        public static CheckOperation HasCss(ILocator loc, string name, Regex pattern, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveCSSOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveCSSAsync(name, pattern, o) : PW.Expect(loc).ToHaveCSSAsync(name, pattern, o); }, not ? $"Expected locator not to have CSS {name} matching /{pattern}/" : $"Expected locator to have CSS {name} matching /{pattern}/");

        public static CheckOperation HasId(ILocator loc, string id, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveIdOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveIdAsync(id, o) : PW.Expect(loc).ToHaveIdAsync(id, o); }, not ? $"Expected locator not to have id \"{id}\"" : $"Expected locator to have id \"{id}\"");

        public static CheckOperation HasId(ILocator loc, Regex pattern, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveIdOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveIdAsync(pattern, o) : PW.Expect(loc).ToHaveIdAsync(pattern, o); }, not ? $"Expected locator id not to match /{pattern}/" : $"Expected locator id to match /{pattern}/");

        // Count
        public static CheckOperation HasCount(ILocator loc, int count, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveCountOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveCountAsync(count, o) : PW.Expect(loc).ToHaveCountAsync(count, o); }, not ? $"Expected locator count not to be {count}" : $"Expected locator count to be {count}");

        public static CheckOperation CountGreaterThan(ILocator loc, int count, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            int actual = 0;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.CountAsync().ConfigureAwait(false); return not ? actual <= count : actual > count; }, timeoutMsGetter ).ConfigureAwait(false),
                () => not ? $"Expected locator count ≤ {count}, but was {actual}" : $"Expected locator count > {count}, but was {actual}", isSkipped);
        }

        public static CheckOperation CountGreaterThanOrEqual(ILocator loc, int count, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            int actual = 0;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.CountAsync().ConfigureAwait(false); return not ? actual < count : actual >= count; }, timeoutMsGetter ).ConfigureAwait(false),
                () => not ? $"Expected locator count < {count}, but was {actual}" : $"Expected locator count ≥ {count}, but was {actual}", isSkipped);
        }

        public static CheckOperation CountLessThan(ILocator loc, int count, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            int actual = 0;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.CountAsync().ConfigureAwait(false); return not ? actual >= count : actual < count; }, timeoutMsGetter ).ConfigureAwait(false),
                () => not ? $"Expected locator count ≥ {count}, but was {actual}" : $"Expected locator count < {count}, but was {actual}", isSkipped);
        }

        public static CheckOperation CountInRange(ILocator loc, int min, int max, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            int actual = 0;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.CountAsync().ConfigureAwait(false); bool r = actual >= min && actual <= max; return not ? !r : r; }, timeoutMsGetter ).ConfigureAwait(false),
                () => not ? $"Expected locator count not in [{min}, {max}], but was {actual}" : $"Expected locator count in [{min}, {max}], but was {actual}", isSkipped);
        }

        public static CheckOperation IsUnique(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            int actual = 0;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.CountAsync().ConfigureAwait(false); return not ? actual != 1 : actual == 1; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? $"Expected locator not to be unique, but matched {actual}" : $"Expected locator to match exactly 1 element, but matched {actual}", isSkipped);
        }

        // Value / JS property
        public static CheckOperation HasValue(ILocator loc, string value, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveValueOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveValueAsync(value, o) : PW.Expect(loc).ToHaveValueAsync(value, o); }, not ? $"Expected locator not to have value \"{value}\"" : $"Expected locator to have value \"{value}\"");

        public static CheckOperation HasValue(ILocator loc, Regex pattern, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveValueOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveValueAsync(pattern, o) : PW.Expect(loc).ToHaveValueAsync(pattern, o); }, not ? $"Expected locator value not to match /{pattern}/" : $"Expected locator value to match /{pattern}/");

        public static CheckOperation HasValues(ILocator loc, IEnumerable<string> values, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveValuesOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveValuesAsync(values, o) : PW.Expect(loc).ToHaveValuesAsync(values, o); }, not ? "Expected locator not to have specified values" : "Expected locator to have specified values");

        public static CheckOperation HasJsProperty(ILocator loc, string name, object value, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveJSPropertyOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveJSPropertyAsync(name, value, o) : PW.Expect(loc).ToHaveJSPropertyAsync(name, value, o); }, not ? $"Expected locator not to have JS property \"{name}\"" : $"Expected locator to have JS property \"{name}\"");

        // ARIA / Accessibility
        public static CheckOperation HasAccessibleName(ILocator loc, string expected, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveAccessibleNameOptions { Timeout = timeoutMsGetter(), IgnoreCase = IsIgnoreCase(cmp()) }; return not ? PW.Expect(loc).Not.ToHaveAccessibleNameAsync(expected, o) : PW.Expect(loc).ToHaveAccessibleNameAsync(expected, o); }, not ? $"Expected locator not to have accessible name \"{expected}\"" : $"Expected locator to have accessible name \"{expected}\"");

        public static CheckOperation HasAccessibleName(ILocator loc, Regex pattern, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveAccessibleNameOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveAccessibleNameAsync(pattern, o) : PW.Expect(loc).ToHaveAccessibleNameAsync(pattern, o); }, not ? $"Expected accessible name not to match /{pattern}/" : $"Expected accessible name to match /{pattern}/");

        public static CheckOperation HasAccessibleDescription(ILocator loc, string expected, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveAccessibleDescriptionOptions { Timeout = timeoutMsGetter(), IgnoreCase = IsIgnoreCase(cmp()) }; return not ? PW.Expect(loc).Not.ToHaveAccessibleDescriptionAsync(expected, o) : PW.Expect(loc).ToHaveAccessibleDescriptionAsync(expected, o); }, not ? $"Expected locator not to have accessible description \"{expected}\"" : $"Expected locator to have accessible description \"{expected}\"");

        public static CheckOperation HasAccessibleDescription(ILocator loc, Regex pattern, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveAccessibleDescriptionOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveAccessibleDescriptionAsync(pattern, o) : PW.Expect(loc).ToHaveAccessibleDescriptionAsync(pattern, o); }, not ? $"Expected accessible description not to match /{pattern}/" : $"Expected accessible description to match /{pattern}/");

        public static CheckOperation HasAccessibleErrorMessage(ILocator loc, string message, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveAccessibleErrorMessageOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveAccessibleErrorMessageAsync(message, o) : PW.Expect(loc).ToHaveAccessibleErrorMessageAsync(message, o); }, not ? $"Expected locator not to have accessible error message \"{message}\"" : $"Expected locator to have accessible error message \"{message}\"");

        public static CheckOperation HasRole(ILocator loc, AriaRole role, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToHaveRoleOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToHaveRoleAsync(role, o) : PW.Expect(loc).ToHaveRoleAsync(role, o); }, not ? $"Expected locator not to have role {role}" : $"Expected locator to have role {role}");

        public static CheckOperation MatchesAriaSnapshot(ILocator loc, string template, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => PwOp(isSkipped, () => { var o = new LocatorAssertionsToMatchAriaSnapshotOptions { Timeout = timeoutMsGetter() }; return not ? PW.Expect(loc).Not.ToMatchAriaSnapshotAsync(template, o) : PW.Expect(loc).ToMatchAriaSnapshotAsync(template, o); }, not ? "Expected locator not to match aria snapshot" : "Expected locator to match aria snapshot");

        // Custom polling checks
        public static CheckOperation InnerTextIs(ILocator loc, string expected, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped, AssertionPipeline pipeline)
        {
            string? actual = null;
            return CheckOperation.Async(
                async () =>
                {
                    actual = null;
                    async Task<bool> predicate()
                    {
                        actual = await loc.InnerTextAsync().ConfigureAwait(false);
                        if (actual is null) return false;
                        bool eq = string.Equals(actual, expected, cmp());
                        return not ? !eq : eq;
                    }
                    return await PwPolling.PollUntilAsync(predicate, timeoutMsGetter, pipeline).ConfigureAwait(false);
                },
                () => not ? $"Expected inner text not to equal \"{expected}\", but was \"{actual}\"" : $"Expected inner text \"{expected}\", but was \"{actual ?? "null"}\"",
                isSkipped);
        }

        public static CheckOperation InnerTextContains(ILocator loc, string substring, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped, AssertionPipeline pipeline)
        {
            string? actual = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.InnerTextAsync().ConfigureAwait(false); bool ok = actual is not null && actual.Contains(substring, cmp()); return not ? !ok : ok; }, timeoutMsGetter, pipeline).ConfigureAwait(false),
                () => not ? $"Expected inner text not to contain \"{substring}\", but was \"{actual}\"" : $"Expected inner text to contain \"{substring}\", but was \"{actual ?? "null"}\"", isSkipped);
        }

        public static CheckOperation InnerTextMatches(ILocator loc, Regex pattern, bool not, Func<float?> timeoutMsGetter, bool isSkipped, AssertionPipeline pipeline)
        {
            string? actual = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.InnerTextAsync().ConfigureAwait(false); bool ok = actual is not null && pattern.IsMatch(actual); return not ? !ok : ok; }, timeoutMsGetter, pipeline).ConfigureAwait(false),
                () => not ? $"Expected inner text not to match /{pattern}/, but was \"{actual}\"" : $"Expected inner text to match /{pattern}/, but was \"{actual ?? "null"}\"", isSkipped);
        }

        public static CheckOperation InputValueIs(ILocator loc, string expected, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped, AssertionPipeline pipeline)
        {
            string? actual = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.InputValueAsync().ConfigureAwait(false); bool eq = string.Equals(actual, expected, cmp()); return not ? !eq : eq; }, timeoutMsGetter, pipeline).ConfigureAwait(false),
                () => not ? $"Expected input value not to equal \"{expected}\", but was \"{actual}\"" : $"Expected input value \"{expected}\", but was \"{actual ?? "null"}\"", isSkipped);
        }

        public static CheckOperation InputValueContains(ILocator loc, string substring, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped, AssertionPipeline pipeline)
        {
            string? actual = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.InputValueAsync().ConfigureAwait(false); bool ok = actual != null && actual.Contains(substring, cmp()); return not ? !ok : ok; }, timeoutMsGetter, pipeline).ConfigureAwait(false),
                () => not ? $"Expected input value not to contain \"{substring}\", but was \"{actual}\"" : $"Expected input value to contain \"{substring}\", but was \"{actual ?? "null"}\"", isSkipped);
        }

        public static CheckOperation InputValueMatches(ILocator loc, Regex pattern, bool not, Func<float?> timeoutMsGetter, bool isSkipped, AssertionPipeline pipeline)
        {
            string? actual = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.InputValueAsync().ConfigureAwait(false); bool ok = actual is not null && pattern.IsMatch(actual); return not ? !ok : ok; }, timeoutMsGetter, pipeline).ConfigureAwait(false),
                () => not ? $"Expected input value not to match /{pattern}/, but was \"{actual}\"" : $"Expected input value to match /{pattern}/, but was \"{actual ?? "null"}\"", isSkipped);
        }

        public static CheckOperation TextContentIs(ILocator loc, string expected, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped, AssertionPipeline pipeline)
        {
            string? actual = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.TextContentAsync().ConfigureAwait(false); bool eq = string.Equals(actual, expected, cmp()); return not ? !eq : eq; }, timeoutMsGetter, pipeline).ConfigureAwait(false),
                () => not ? $"Expected text content not to equal \"{expected}\", but was \"{actual}\"" : $"Expected text content \"{expected}\", but was \"{actual ?? "null"}\"", isSkipped);
        }

        public static CheckOperation TextContentContains(ILocator loc, string substring, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped, AssertionPipeline pipeline)
        {
            string? actual = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = (await loc.TextContentAsync().ConfigureAwait(false)) ?? ""; bool ok = actual.Contains(substring, cmp()); return not ? !ok : ok; }, timeoutMsGetter, pipeline).ConfigureAwait(false),
                () => not ? $"Expected text content not to contain \"{substring}\", but was \"{actual}\"" : $"Expected text content to contain \"{substring}\", but was \"{actual}\"", isSkipped);
        }

        public static CheckOperation InnerHtmlContains(ILocator loc, string substring, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped, AssertionPipeline pipeline)
        {
            string? actual = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.InnerHTMLAsync().ConfigureAwait(false); bool ok = actual.Contains(substring, cmp()); return not ? !ok : ok; }, timeoutMsGetter, pipeline).ConfigureAwait(false),
                () => not ? $"Expected inner HTML not to contain \"{substring}\"" : $"Expected inner HTML to contain \"{substring}\"", isSkipped);
        }

        // Layout / BoundingBox
        public static CheckOperation HasBoundingBox(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { var box = await loc.BoundingBoxAsync().ConfigureAwait(false); return not ? box is null : box is not null; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? "Expected element not to have a bounding box" : "Expected element to have a bounding box (be rendered)", isSkipped);

        public static CheckOperation HasWidth(ILocator loc, float expected, float tolerance, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            float actual = 0;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { var box = await loc.BoundingBoxAsync().ConfigureAwait(false); if (box is null) return not; actual = box.Width; bool ok = Math.Abs(actual - expected) <= tolerance; return not ? !ok : ok; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? $"Expected element width not to be {expected}±{tolerance}, but was {actual}" : $"Expected element width {expected}±{tolerance}, but was {actual}", isSkipped);
        }

        public static CheckOperation HasHeight(ILocator loc, float expected, float tolerance, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            float actual = 0;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { var box = await loc.BoundingBoxAsync().ConfigureAwait(false); if (box is null) return not; actual = box.Height; bool ok = Math.Abs(actual - expected) <= tolerance; return not ? !ok : ok; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? $"Expected element height not to be {expected}±{tolerance}, but was {actual}" : $"Expected element height {expected}±{tolerance}, but was {actual}", isSkipped);
        }

        // Scroll
        public static CheckOperation IsScrolledToTop(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            double scrollTop = 0;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { scrollTop = await loc.EvaluateAsync<double>("el => el.scrollTop").ConfigureAwait(false); bool ok = scrollTop == 0; return not ? !ok : ok; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? "Expected element not to be scrolled to top" : $"Expected element to be scrolled to top, but scrollTop was {scrollTop}", isSkipped);
        }

        public static CheckOperation IsScrolledToBottom(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { bool r = await loc.EvaluateAsync<bool>("el => el.scrollTop + el.clientHeight >= el.scrollHeight - 1").ConfigureAwait(false); return not ? !r : r; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? "Expected element not to be scrolled to bottom" : "Expected element to be scrolled to bottom", isSkipped);

        // Eval / Computed Style
        public static CheckOperation EvalIsTruthy(ILocator loc, string expression, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
            => CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { bool r = await loc.EvaluateAsync<bool>(expression).ConfigureAwait(false); return not ? !r : r; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? $"Expected eval({expression}) not to be truthy" : $"Expected eval({expression}) to be truthy", isSkipped);

        public static CheckOperation EvalIs(ILocator loc, string expression, string expected, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? actual = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.EvaluateAsync<string>(expression).ConfigureAwait(false); bool eq = string.Equals(actual, expected, cmp()); return not ? !eq : eq; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? $"Expected eval({expression}) not to equal \"{expected}\", but was \"{actual}\"" : $"Expected eval({expression}) = \"{expected}\", but was \"{actual ?? "null"}\"", isSkipped);
        }

        public static CheckOperation HasComputedStyle(ILocator loc, string property, string value, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? actual = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { actual = await loc.EvaluateAsync<string>($"el => getComputedStyle(el)['{property}']").ConfigureAwait(false); bool eq = string.Equals(actual, value, cmp()); return not ? !eq : eq; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? $"Expected computed style '{property}' not to be \"{value}\", but was \"{actual}\"" : $"Expected computed style '{property}' = \"{value}\", but was \"{actual ?? "null"}\"", isSkipped);
        }

        public static CheckOperation HasOpacity(ILocator loc, double expected, double tolerance, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? raw = null; double actual = 0;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () =>
            {
                raw = await loc.EvaluateAsync<string>("el => getComputedStyle(el).opacity").ConfigureAwait(false);
                bool parsed = double.TryParse(raw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out actual);
                bool ok = parsed && Math.Abs(actual - expected) <= tolerance;
                return not ? !ok : ok;
            }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? $"Expected opacity not to be {expected}±{tolerance}, but was \"{raw}\"" : $"Expected opacity {expected}±{tolerance}, but was \"{raw ?? "null"}\"", isSkipped);
        }

        // Form elements
        public static CheckOperation HasSelectedOption(ILocator loc, string textOrValue, bool not, Func<StringComparison> cmp, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? selectedText = null; string? selectedValue = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () =>
            {
                selectedText = await loc.EvaluateAsync<string>("el => el.options[el.selectedIndex]?.text ?? ''").ConfigureAwait(false);
                selectedValue = await loc.EvaluateAsync<string>("el => el.value").ConfigureAwait(false);
                var c = cmp();
                bool ok = string.Equals(selectedText, textOrValue, c) || string.Equals(selectedValue, textOrValue, c);
                return not ? !ok : ok;
            }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? $"Expected selected option not to be \"{textOrValue}\"" : $"Expected selected option \"{textOrValue}\", but text was \"{selectedText}\" / value was \"{selectedValue}\"", isSkipped);
        }

        public static CheckOperation IsRequired(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? val = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { val = await loc.GetAttributeAsync("required").ConfigureAwait(false); return not ? val is null : val is not null; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? "Expected element not to be required" : "Expected element to be required", isSkipped);
        }

        public static CheckOperation IsReadonly(ILocator loc, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? val = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { val = await loc.GetAttributeAsync("readonly").ConfigureAwait(false); return not ? val is null : val is not null; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? "Expected element not to be readonly" : "Expected element to be readonly", isSkipped);
        }

        public static CheckOperation HasMaxLength(ILocator loc, int length, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? val = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { val = await loc.GetAttributeAsync("maxlength").ConfigureAwait(false); bool ok = int.TryParse(val, out int a) && a == length; return not ? !ok : ok; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? $"Expected maxlength not to be {length}, but was \"{val}\"" : $"Expected maxlength = {length}, but was \"{val ?? "none"}\"", isSkipped);
        }

        public static CheckOperation HasTabIndex(ILocator loc, int index, bool not, Func<float?> timeoutMsGetter, bool isSkipped)
        {
            string? val = null;
            return CheckOperation.Async(async () => await PwPolling.PollUntilAsync(async () => { val = await loc.GetAttributeAsync("tabindex").ConfigureAwait(false); bool ok = int.TryParse(val, out int a) && a == index; return not ? !ok : ok; }, timeoutMsGetter).ConfigureAwait(false),
                () => not ? $"Expected tabindex not to be {index}, but was \"{val}\"" : $"Expected tabindex = {index}, but was \"{val ?? "none"}\"", isSkipped);
        }

        // Helpers
        public static CheckOperation PwOp(bool isSkipped, Func<Task> fn, string defaultFail)
        {
            string? captured = null;
            return CheckOperation.Async(async () =>
            {
                captured = null;
                try { await fn().ConfigureAwait(false); return true; }
                catch (PlaywrightException ex) { captured = ex.Message.Trim(); return false; }
            }, () => captured ?? defaultFail, isSkipped);
        }

        public static bool IsIgnoreCase(StringComparison c) =>
            c is StringComparison.OrdinalIgnoreCase
               or StringComparison.CurrentCultureIgnoreCase
               or StringComparison.InvariantCultureIgnoreCase;
    }
}
