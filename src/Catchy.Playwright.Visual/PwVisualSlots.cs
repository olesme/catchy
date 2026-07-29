using System.Collections.Generic;
using Catchy.Sdk;

namespace Catchy
{
    public static class PwVisualSlots
    {
        /// <summary>Maximum allowed pixel-diff percentage (0–100). DefaultStateless: 0.1.</summary>
        public static readonly SlotKey<float?> MaxDiffPercent = new();

        /// <summary>Directory where baseline / diff / actual PNGs are stored.</summary>
        public static readonly SlotKey<string?> SnapshotsDir = new();

        /// <summary>When true, overwrites the baseline instead of comparing against it.</summary>
        public static readonly SlotKey<bool?> UpdateBaseline = new();

        /// <summary>Rectangular regions to ignore (filled with a neutral colour before diffing).</summary>
        public static readonly SlotKey<IReadOnlyList<ScreenshotRegion>?> MaskRegions = new();

        /// <summary>Capture the full scrollable page instead of only the viewport.</summary>
        public static readonly SlotKey<bool?> FullPage = new();
    }
}
