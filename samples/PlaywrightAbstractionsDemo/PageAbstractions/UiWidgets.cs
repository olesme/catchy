// ═══════════════════════════════════════════════════════════════════════════════
// VARIANT 2 – AssertVia
//
// [AssertVia("Locator")] on a base class generates:
//   Asserter.That(UiWidget value)  →  ValueAssertions<InternalLocator>
//
// The generated entry point extracts value.Locator transparently.
// If you have existing assertion extensions for InternalLocator, they all work.
// ═══════════════════════════════════════════════════════════════════════════════

using Catchy;

namespace PlaywrightAbstractionsDemo.PageAbstractions;

/// <summary>
/// Alternative base type that uses property-based delegation.
/// <para>
/// <c>[AssertVia("Locator")]</c> instructs the source generator to emit:<br/>
/// <code>public static ValueAssertions&lt;InternalLocator&gt; That(this Asserter a, UiWidget value)</code>
/// which internally calls <c>value.Locator</c>.
/// </para>
/// All assertion extensions written for <c>InternalLocator</c> are immediately
/// available on the chain without any extra entry-point code.
/// </summary>
[AssertVia("Locator")]
public abstract class UiWidget(InternalLocator locator)
{
    internal InternalLocator Locator { get; } = locator;
}

/// <summary>A text field widget — delegates assertions to its internal locator.</summary>
public sealed class TextFieldWidget(InternalLocator locator) : UiWidget(locator)
{
}

/// <summary>A dropdown widget — delegates assertions to its internal locator.</summary>
public sealed class DropdownWidget(InternalLocator locator) : UiWidget(locator)
{
}
