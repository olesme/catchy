namespace Catchy.Sdk
{
    public sealed class BetweenOptions { public bool Exclusive { get; set; } }

    public static partial class WellKnownSlots
    {
        public static readonly SlotKey<BetweenOptions> BetweenMode = new();
    }
}
