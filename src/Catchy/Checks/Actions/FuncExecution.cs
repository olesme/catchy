namespace Catchy.Sdk
{
    public static class FuncExecution
    {
        private static readonly SlotKey<bool> _executed = new();
        private static readonly SlotKey<Exception?> _caught = new();

        public static async Task<(bool success, Exception? caught)> EnsureAsync(
            Func<Task> func, SlotContainer slots)
        {
            if (slots.TryGet(_executed, out bool done) && done)
            {
                slots.TryGet(_caught, out Exception? cached);
                return (cached is null, cached);
            }

            Exception? caught = null;
            try { await func().ConfigureAwait(false); }
            catch (Exception ex) { caught = ex; }

            slots.Set(_executed, true);
            slots.Set(_caught, caught);
            return (caught is null, caught);
        }

        public static void Reset(SlotContainer slots)
        {
            slots.Set(_executed, false);
            slots.Set(_caught, (Exception?)null);
        }
    }
}
