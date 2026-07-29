using Catchy;

namespace CatchyTestHelpers
{
    public static class AssertionFailureAsserts
    {
        public static void ContainsAll(string message, params string[] expectedParts)
        {
            if (message is null)
            {
                throw new AssertionException("Expected failure message to be non-null.");
            }

            foreach (var part in expectedParts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    continue;
                }

                if (!message.Contains(part, StringComparison.Ordinal))
                {
                    throw new AssertionException(
                        $"Expected failure message to contain '{part}', but it was:\n{message}");
                }
            }
        }

        public static void ContainsAny(string message, params string[] expectedParts)
        {
            if (message is null)
            {
                throw new AssertionException("Expected failure message to be non-null.");
            }

            foreach (var part in expectedParts)
            {
                if (string.IsNullOrEmpty(part))
                {
                    continue;
                }

                if (message.Contains(part, StringComparison.Ordinal))
                {
                    return;
                }
            }

            throw new AssertionException(
                $"Expected failure message to contain one of [{string.Join(", ", expectedParts)}], but it was:\n{message}");
        }

        public static void ContainsChain(string message, params string[] chainLinks)
            => ContainsAll(message, chainLinks);

        public static void ContainsDiffHint(string message)
            => ContainsAny(message, "Diff at index", "differ", "difference", "diff");
    }
}
