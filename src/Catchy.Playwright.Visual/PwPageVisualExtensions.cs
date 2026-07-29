using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Catchy.Sdk;
using Microsoft.Playwright;

namespace Catchy
{
    /// <summary>
    /// Visual / screenshot assertion extensions for <see cref="ValueAssertions{IPage}"/>.
    /// Chain modifiers before or after the assertion — all slots are resolved lazily at execution time.
    /// <code>
    /// await Stateless.Verify.That(page)
    ///     .MatchesScreenshot("home")
    ///     .WithMaxDiffPercent(0.5f)
    ///     .WithSnapshotsDir("__snapshots__/home");
    /// </code>
    /// </summary>
    public static class PwPageVisualExtensions
    {
        /// <summary>Maximum allowed pixel-diff percentage (0–100). DefaultStateless: 0.1.</summary>
        public static ValueAssertions<IPage> WithMaxDiffPercent(this ValueAssertions<IPage> a, float percent,
            [CallerArgumentExpression(nameof(percent))] string? expr = null)
        {
            a.GetPipeline().Slots.Set(PwVisualSlots.MaxDiffPercent, percent);
            a.Link("WithMaxDiffPercent", expr);
            return a;
        }

        /// <summary>Directory in which baseline / actual / diff PNGs are stored.</summary>
        public static ValueAssertions<IPage> WithSnapshotsDir(this ValueAssertions<IPage> a, string dir,
            [CallerArgumentExpression(nameof(dir))] string? expr = null)
        {
            a.GetPipeline().Slots.Set(PwVisualSlots.SnapshotsDir, dir);
            a.Link("WithSnapshotsDir", expr);
            return a;
        }

        /// <summary>
        /// When <c>true</c>, saves/overwrites the baseline instead of comparing.
        /// Useful during initial authoring or after intentional UI changes.
        /// </summary>
        public static ValueAssertions<IPage> UpdatingBaseline(this ValueAssertions<IPage> a, bool update = true)
        {
            a.GetPipeline().Slots.Set(PwVisualSlots.UpdateBaseline, update);
            a.Link("UpdatingBaseline");
            return a;
        }

        /// <summary>Captures the full scrollable page instead of only the visible viewport.</summary>
        public static ValueAssertions<IPage> FullPage(this ValueAssertions<IPage> a, bool fullPage = true)
        {
            a.GetPipeline().Slots.Set(PwVisualSlots.FullPage, fullPage);
            a.Link("FullPage");
            return a;
        }

        /// <summary>
        /// Excludes one or more rectangular regions from the pixel diff.
        /// Masked pixels are painted with a neutral grey in the diff image.
        /// </summary>
        public static ValueAssertions<IPage> WithMaskRegions(this ValueAssertions<IPage> a,
            IReadOnlyList<ScreenshotRegion> regions,
            [CallerArgumentExpression(nameof(regions))] string? expr = null)
        {
            a.GetPipeline().Slots.Set(PwVisualSlots.MaskRegions, regions);
            a.Link("WithMaskRegions", expr);
            return a;
        }

        /// <inheritdoc cref="WithMaskRegions(ValueAssertions{IPage}, IReadOnlyList{ScreenshotRegion}, string?)"/>
        public static ValueAssertions<IPage> WithMaskRegions(this ValueAssertions<IPage> a,
            params ScreenshotRegion[] regions)
            => a.WithMaskRegions((IReadOnlyList<ScreenshotRegion>)regions);

        /// <summary>Asserts that the page screenshot matches the baseline named <paramref name="name"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> MatchesScreenshot(this ValueAssertions<IPage> a,
            string name,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        {
            a.Link("MatchesScreenshot", expr);
            var slots = a.GetPipeline().Slots;
            a.Op(PwVisualChecks.MatchesScreenshot(
                page: a.GetValue(),
                name: name,
                not: false,
                maxDiffPctGetter: () => slots.Get(PwVisualSlots.MaxDiffPercent),
                snapshotsDirGetter: () => slots.Get(PwVisualSlots.SnapshotsDir),
                updateBaselineGetter: () => slots.Get(PwVisualSlots.UpdateBaseline),
                maskRegionsGetter: () => slots.Get(PwVisualSlots.MaskRegions),
                fullPageGetter: () => slots.Get(PwVisualSlots.FullPage),
                isSkipped: a.IsSkipped()));
            return a;
        }

        /// <summary>Asserts that the page screenshot does not match the baseline named <paramref name="name"/>.</summary>
        [DebuggerHidden, StackTraceHidden, AssertionMethod]
        public static ValueAssertions<IPage> DoesNotMatchScreenshot(this ValueAssertions<IPage> a,
            string name,
            [CallerArgumentExpression(nameof(name))] string? expr = null)
        {
            a.Link("DoesNotMatchScreenshot", expr);
            var slots = a.GetPipeline().Slots;
            a.Op(PwVisualChecks.MatchesScreenshot(
                page: a.GetValue(),
                name: name,
                not: true,
                maxDiffPctGetter: () => slots.Get(PwVisualSlots.MaxDiffPercent),
                snapshotsDirGetter: () => slots.Get(PwVisualSlots.SnapshotsDir),
                updateBaselineGetter: () => slots.Get(PwVisualSlots.UpdateBaseline),
                maskRegionsGetter: () => slots.Get(PwVisualSlots.MaskRegions),
                fullPageGetter: () => slots.Get(PwVisualSlots.FullPage),
                isSkipped: a.IsSkipped()));
            return a;
        }
    }
}
