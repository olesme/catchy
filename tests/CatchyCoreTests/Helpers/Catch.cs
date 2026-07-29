using Catchy;

namespace CatchyCoreTests.Helpers
{
    /// <summary>
    /// Helper to capture assertion failure messages
    /// </summary>
    public static class Catch
    {
        public static async Task<string> FailureOf(Func<Task> assertion)
        {
            try
            {
                await assertion();
                throw new InvalidOperationException("Expected assertion to fail");
            }
            catch (AssertionException ex)
            {
                return ex.Message;
            }
        }
    }
}
