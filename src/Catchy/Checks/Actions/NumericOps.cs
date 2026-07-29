namespace Catchy.Sdk
{
    public static class NumericOps
    {
        public static object? Subtract(object? a, object? b)
        {
            if (a is null || b is null) return null;

            return a switch
            {
                int x when b is int y => x - y,
                long x when b is long y => x - y,
                double x when b is double y => x - y,
                float x when b is float y => x - y,
                decimal x when b is decimal y => x - y,
                short x when b is short y => (short)(x - y),
                byte x when b is byte y => (byte)(x - y),
                uint x when b is uint y => x - y,
                ulong x when b is ulong y => x - y,
                _ => throw new NotSupportedException($"Subtract not supported for {a.GetType().Name}")
            };
        }

        public static bool AreEqual(object? a, object? b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.Equals(b);
        }
    }
}
