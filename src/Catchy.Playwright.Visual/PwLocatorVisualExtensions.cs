using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;
using Microsoft.Playwright;

namespace Catchy
{
    /// <summary>
    /// Visual / screenshot assertion extensions for <see cref="ValueAssertions{ILocator}"/>.
    /// Chain modifiers before or after the assertion — all slots are resolved lazily at execution time.
    /// <code>
    /// await Stateless.Verify.That(page.Locator(".hero"))
    ///     .MatchesScreenshot("hero-banner")
    ///     .WithMaxDiffPercent(1f);
    /// </code>
    /// </summary>
    public static class PwLocatorVisualExtensions
    {
        /// <summary>Maximum allowed pixel-diff percentage (0–100). DefaultStateless: 0.1.</summary>
        public static ValueAssertions<ILocator> WithMaxDiffPercent(this ValueAssertions<ILocator> a, float percent,
            [CallerArgumentExpression(nameof(percent))] string? expr = null)
        {
            a.GetPipeline().Slots.Set(PwVisualSlots.MaxDiffPercent, percent);
            a.Link("WithMaxDiffPercent", expr);
            return a;
        }

        /// <summary>Directory in which baseline / actual / diff PNGs are stored.</summary>
        public static ValueAssertions<ILocator> WithSnapshotsDir(this ValueAssertions<ILocator> a, string dir,
            [CallerArgumentExpression(nameof(dir))] string? expr = null)
        {
            a.GetPipeline().Slots.Set(PwVisualSlots.SnapshotsDir, dir);
            a.Link("WithSnapshotsDir", expr);
            return a;
        }

        /// <summary>
        /// When <c>true</c>, saves/overwrites the baseline instead of comparing.
        /// </summary>
        public static ValueAssertions<ILocator> UpdatingBaseline(this ValueAssertions<ILocator> a, bool update = true)
        {
            a.GetPipeline().Slots.Set(PwVisualSlots.UpdateBaseline, update);
            a.Link("UpdatingBaseline");
            return a;
        }

        /// <summary>
        /// Excludes one or more rectangular regions from the pixel diff.
        /// Masked pixels are painted with a neutral grey in the diff image.
        /// Coordinates are relative to the element's bounding box.
        /// </summary>
        public static ValueAssertions<ILocator> WithMaskRegions(this ValueAssertions<ILocator> a,
            IReadOnlyList<ScreenshotRegion> regions,
            [CallerArgumentExpression(nameof(regions))] string? expr = null)
        {
            a.GetPipeline().Slots.Set(PwVisualSlots.MaskRegions, regions);
            a.Link("WithMaskRegions", expr);
            return a;
        }

        /// <inheritdoc cref="WithMaskRegions(ValueAssertions{ILocator}, IReadOnlyList{ScreenshotRegion}, string?)"/>
        public static ValueAssertions<ILocator> WithMaskRegions(this ValueAssertions<ILocator> a,
            params ScreenshotRegion[] regions)
            => a.WithMaskRegions((IReadOnlyList<ScreenshotRegion>)regions);

        /// <summary>Asserts that the locator screenshot matches the baseline named <paramref name="name"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ILocator> MatchesScreenshot(this ValueAssertions<ILocator> a,
            string name,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        {
            a.Link("MatchesScreenshot", expr);
            var slots = a.GetPipeline().Slots;
            a.Op(PwVisualChecks.ElementMatchesScreenshot(
                locator: a.GetValue(),
                name: name,
                not: false,
                maxDiffPctGetter: () => slots.Get(PwVisualSlots.MaxDiffPercent),
                snapshotsDirGetter: () => slots.Get(PwVisualSlots.SnapshotsDir),
                updateBaselineGetter: () => slots.Get(PwVisualSlots.UpdateBaseline),
                maskRegionsGetter: () => slots.Get(PwVisualSlots.MaskRegions),
                isSkipped: a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the locator screenshot does not match the baseline named <paramref name="name"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<ILocator> DoesNotMatchScreenshot(this ValueAssertions<ILocator> a,
            string name,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        {
            a.Link("DoesNotMatchScreenshot", expr);
            var slots = a.GetPipeline().Slots;
            a.Op(PwVisualChecks.ElementMatchesScreenshot(
                locator: a.GetValue(),
                name: name,
                not: true,
                maxDiffPctGetter: () => slots.Get(PwVisualSlots.MaxDiffPercent),
                snapshotsDirGetter: () => slots.Get(PwVisualSlots.SnapshotsDir),
                updateBaselineGetter: () => slots.Get(PwVisualSlots.UpdateBaseline),
                maskRegionsGetter: () => slots.Get(PwVisualSlots.MaskRegions),
                isSkipped: a.IsSkipped()));
            return a;
        }
    }
}
