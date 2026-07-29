using Microsoft.Playwright;
using SkiaSharp;

namespace Catchy.Sdk
{
    public static class PwVisualChecks
    {
        // Page

        public static CheckOperation MatchesScreenshot(
            IPage page,
            string name,
            bool not,
            Func<float?> maxDiffPctGetter,
            Func<string?> snapshotsDirGetter,
            Func<bool?> updateBaselineGetter,
            Func<IReadOnlyList<ScreenshotRegion>?> maskRegionsGetter,
            Func<bool?> fullPageGetter,
            bool isSkipped)
        {
            float actualDiff = 0f;
            string? failReason = null;
            float resolvedMax = 0f;

            return CheckOperation.Async(async () =>
            {
                resolvedMax = maxDiffPctGetter() ?? 0.1f;
                string dir = ResolveDir(snapshotsDirGetter());
                bool update = updateBaselineGetter() ?? false;
                bool fullPage = fullPageGetter() ?? false;
                var masks = maskRegionsGetter();

                Directory.CreateDirectory(dir);
                string safeName = SanitizeName(name);
                string baselinePath = Path.Combine(dir, $"{safeName}.png");
                string actualPath = Path.Combine(dir, $"{safeName}.actual.png");
                string diffPath = Path.Combine(dir, $"{safeName}.diff.png");

                byte[] currentBytes = await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Type = ScreenshotType.Png,
                    FullPage = fullPage
                }).ConfigureAwait(false);

                // Create / update baseline
                if (update || !File.Exists(baselinePath))
                {
#if NETSTANDARD2_0
                    File.WriteAllBytes(baselinePath, currentBytes);
#else
                    await File.WriteAllBytesAsync(baselinePath, currentBytes).ConfigureAwait(false);
#endif
                    CleanSidecar(actualPath);
                    CleanSidecar(diffPath);
                    if (not) { failReason = $"Baseline saved/updated for \"{name}\" — cannot assert 'not matches' on a fresh baseline"; return false; }
                    return true;
                }

#if NETSTANDARD2_0
                byte[] baselineBytes = File.ReadAllBytes(baselinePath);
#else
                byte[] baselineBytes = await File.ReadAllBytesAsync(baselinePath).ConfigureAwait(false);
#endif

                using var baselineBmp = SKBitmap.Decode(baselineBytes);
                using var currentBmp = SKBitmap.Decode(currentBytes);

                if (baselineBmp is null || currentBmp is null)
                {
                    failReason = $"Failed to decode screenshot images for \"{name}\"";
                    return not;
                }

                if (baselineBmp.Width != currentBmp.Width || baselineBmp.Height != currentBmp.Height)
                {
                    actualDiff = 100f;
                    failReason =
                        $"Screenshot size mismatch for \"{name}\": " +
                        $"baseline {baselineBmp.Width}×{baselineBmp.Height}, " +
                        $"actual {currentBmp.Width}×{currentBmp.Height}";
                    PersistActual(currentBytes, actualPath);
                    return not;
                }

                using var diffBmp = new SKBitmap(baselineBmp.Width, baselineBmp.Height);
                actualDiff = CalculateDiff(baselineBmp, currentBmp, diffBmp, masks);

                bool passed = actualDiff <= resolvedMax;

                if (!passed)
                {
                    PersistActual(currentBytes, actualPath);
                    SaveDiffImage(diffBmp, diffPath);
                }
                else
                {
                    CleanSidecar(actualPath);
                    CleanSidecar(diffPath);
                }

                return not ? !passed : passed;
            },
            () => failReason ?? (not
                ? $"Expected screenshot \"{name}\" NOT to match, but diff was only {actualDiff:F3}% (threshold {resolvedMax}%)"
                : $"Expected screenshot \"{name}\" to match within {resolvedMax}%, but diff was {actualDiff:F3}%"),
            isSkipped);
        }

        // Locator

        public static CheckOperation ElementMatchesScreenshot(
            ILocator locator,
            string name,
            bool not,
            Func<float?> maxDiffPctGetter,
            Func<string?> snapshotsDirGetter,
            Func<bool?> updateBaselineGetter,
            Func<IReadOnlyList<ScreenshotRegion>?> maskRegionsGetter,
            bool isSkipped)
        {
            float actualDiff = 0f;
            string? failReason = null;
            float resolvedMax = 0f;

            return CheckOperation.Async(async () =>
            {
                resolvedMax = maxDiffPctGetter() ?? 0.1f;
                string dir = ResolveDir(snapshotsDirGetter());
                bool update = updateBaselineGetter() ?? false;
                var masks = maskRegionsGetter();

                Directory.CreateDirectory(dir);
                string safeName = SanitizeName(name);
                string baselinePath = Path.Combine(dir, $"{safeName}.png");
                string actualPath = Path.Combine(dir, $"{safeName}.actual.png");
                string diffPath = Path.Combine(dir, $"{safeName}.diff.png");

                byte[] currentBytes = await locator.ScreenshotAsync(new LocatorScreenshotOptions
                {
                    Type = ScreenshotType.Png
                }).ConfigureAwait(false);

                if (update || !File.Exists(baselinePath))
                {
#if NETSTANDARD2_0
                    File.WriteAllBytes(baselinePath, currentBytes);
#else
                    await File.WriteAllBytesAsync(baselinePath, currentBytes).ConfigureAwait(false);
#endif
                    CleanSidecar(actualPath);
                    CleanSidecar(diffPath);
                    if (not) { failReason = $"Baseline saved/updated for \"{name}\""; return false; }
                    return true;
                }

#if NETSTANDARD2_0
                byte[] baselineBytes = File.ReadAllBytes(baselinePath);
#else
                byte[] baselineBytes = await File.ReadAllBytesAsync(baselinePath).ConfigureAwait(false);
#endif

                using var baselineBmp = SKBitmap.Decode(baselineBytes);
                using var currentBmp = SKBitmap.Decode(currentBytes);

                if (baselineBmp is null || currentBmp is null)
                {
                    failReason = $"Failed to decode images for \"{name}\"";
                    return not;
                }

                if (baselineBmp.Width != currentBmp.Width || baselineBmp.Height != currentBmp.Height)
                {
                    actualDiff = 100f;
                    failReason =
                        $"Element screenshot size mismatch for \"{name}\": " +
                        $"baseline {baselineBmp.Width}×{baselineBmp.Height}, " +
                        $"actual {currentBmp.Width}×{currentBmp.Height}";
                    PersistActual(currentBytes, actualPath);
                    return not;
                }

                using var diffBmp = new SKBitmap(baselineBmp.Width, baselineBmp.Height);
                actualDiff = CalculateDiff(baselineBmp, currentBmp, diffBmp, masks);

                bool passed = actualDiff <= resolvedMax;

                if (!passed)
                {
                    PersistActual(currentBytes, actualPath);
                    SaveDiffImage(diffBmp, diffPath);
                }
                else
                {
                    CleanSidecar(actualPath);
                    CleanSidecar(diffPath);
                }

                return not ? !passed : passed;
            },
            () => failReason ?? (not
                ? $"Expected element screenshot \"{name}\" NOT to match, but diff was only {actualDiff:F3}% (threshold {resolvedMax}%)"
                : $"Expected element screenshot \"{name}\" to match within {resolvedMax}%, but diff was {actualDiff:F3}%"),
            isSkipped);
        }

        // Pixel-diff core

        private static float CalculateDiff(
            SKBitmap baseline,
            SKBitmap current,
            SKBitmap diff,
            IReadOnlyList<ScreenshotRegion>? masks)
        {
            int w = baseline.Width;
            int h = baseline.Height;
            long diffPixels = 0;
            long total = (long)w * h;

            // Neutral grey used for masked regions in the diff image
            var maskedColor = new SKColor(128, 128, 128, 128);

            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (IsInMask(x, y, masks))
                    {
                        diff.SetPixel(x, y, maskedColor);
                        continue;
                    }

                    var b = baseline.GetPixel(x, y);
                    var c = current.GetPixel(x, y);

                    if (b == c)
                    {
                        // Fade-out baseline in diff: semi-transparent version of original
                        diff.SetPixel(x, y, b.WithAlpha(64));
                    }
                    else
                    {
                        diffPixels++;
                        diff.SetPixel(x, y, SKColors.Red);
                    }
                }
            }

            return total == 0 ? 0f : (float)diffPixels / total * 100f;
        }

        private static bool IsInMask(int x, int y, IReadOnlyList<ScreenshotRegion>? masks)
        {
            if (masks is null) return false;
            foreach (var m in masks)
                if (x >= m.X && x < m.X + m.Width && y >= m.Y && y < m.Y + m.Height)
                    return true;
            return false;
        }

        // Helpers

        private static string ResolveDir(string? configured)
        {
            if (!string.IsNullOrWhiteSpace(configured))
                return configured!;

            // Convention: __snapshots__ next to the test assembly
            return Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "__snapshots__");
        }

        private static string SanitizeName(string name)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name;
        }

        private static void PersistActual(byte[] bytes, string path)
        {
            try { File.WriteAllBytes(path, bytes); } catch { /* best-effort */ }
        }

        private static void SaveDiffImage(SKBitmap diff, string path)
        {
            try
            {
                using var data = diff.Encode(SKEncodedImageFormat.Png, 100);
                using var fs = File.OpenWrite(path);
                data.SaveTo(fs);
            }
            catch { /* best-effort */ }
        }

        private static void CleanSidecar(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); } catch { /* best-effort */ }
        }
    }
}
