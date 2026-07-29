// ═══════════════════════════════════════════════════════════════════════════════
// VARIANT 1 – AssertEntry
//
// [AssertEntry] on a base class generates:
//   Asserter.That<T>(T value) where T : UiElement  →  ValueAssertions<T>
//
// Because T is preserved, extension methods such as
//   IsVisible(this ValueAssertions<T> a) where T : IClickable
// appear in IntelliSense only for element types that satisfy the extra constraint.
// The test layer never imports InternalLocator.
// ═══════════════════════════════════════════════════════════════════════════════

using Catchy;

namespace PlaywrightAbstractionsDemo.PageAbstractions;

// Interface markers that give elements their capability

/// <summary>This element has a visible/hidden state.</summary>
public interface IHasVisibility { }

/// <summary>This element can be clicked (implies visibility).</summary>
public interface IClickable : IHasVisibility { }

/// <summary>This element has a text value that can be asserted.</summary>
public interface IHasText { }

/// <summary>This element can be checked or unchecked.</summary>
public interface ICheckable { }

// Base element type – tells the generator to emit a generic entry point

/// <summary>
/// Root of the UI element hierarchy in the interaction layer.
/// <para>
/// <c>[AssertEntry]</c> instructs the source generator to emit:<br/>
/// <code>public static ValueAssertions&lt;T&gt; That&lt;T&gt;(this Asserter a, T value)
///     where T : UiElement</code>
/// </para>
/// </summary>
[AssertEntry]
public abstract class UiElement(InternalLocator locator)
{
    internal InternalLocator Locator { get; } = locator;
}

// Concrete element types used in tests

/// <summary>A button — clickable and visible.</summary>
public sealed class ButtonElement(InternalLocator locator) : UiElement(locator), IClickable
{
}

/// <summary>A text paragraph — has text content and visibility.</summary>
public sealed class TextElement(InternalLocator locator) : UiElement(locator), IHasText, IHasVisibility
{
}

/// <summary>A checkbox — checkable and visible.</summary>
public sealed class CheckboxElement(InternalLocator locator) : UiElement(locator), ICheckable, IHasVisibility
{
}
