using Catchy;

namespace CatchyTestHelpers
{
    public static class TestHelpers
    {
        public static async Task ShouldPassAsync(Func<Task> assertion)
        {
            try
            {
                await assertion();
            }
            catch (Exception ex)
            {
                throw new AssertionException($"Expected pass but assertion threw: {ex.GetType().Name}: {ex.Message}");
            }
        }

        public static async Task<string> ShouldFailWithMessageAsync(Func<Task> assertion)
        {
            try
            {
                await assertion();
                throw new AssertionException("Expected assertion to fail but it passed.");
            }
            catch (AssertionException ex)
            {
                 return ex.Message;
            }
            catch (Exception ex)
            {
                throw new AssertionException($"Expected AssertionException but got {ex.GetType().Name}: {ex.Message}", ex);
            }
        }
    }
}
