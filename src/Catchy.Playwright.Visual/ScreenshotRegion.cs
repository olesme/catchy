namespace Catchy
{
    /// <summary>
    /// Defines a rectangular region to mask (exclude) during screenshot comparison.
    /// Coordinates are in CSS pixels relative to the top-left corner of the screenshot.
    /// </summary>
    public readonly record struct ScreenshotRegion(int X, int Y, int Width, int Height)
    {
        public static ScreenshotRegion FromLTRB(int left, int top, int right, int bottom)
            => new(left, top, right - left, bottom - top);
    }
}
