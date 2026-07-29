using System.Text;

namespace Catchy.Sdk
{
    public static class StringDiff
    {
        private const int ContextChars = 20;
        private const int MaxDiffDisplay = 80;

        public static string? Build(string? expected, string? actual)
        {
            if (expected is null || actual is null) return null;
            if (expected == actual) return null;

            int firstDiff = FirstDiffIndex(expected, actual);
            if (firstDiff < 0) return null;

            var sb = new StringBuilder("\n  Diff at index ");
            sb.Append(firstDiff);
            sb.Append(":\n");

            AppendContext(sb, "  expected: ", expected, firstDiff);
            AppendContext(sb, "  actual:   ", actual, firstDiff);

            // Arrow pointer
            int paddingStart = "  expected: ".Length + Math.Min(firstDiff, ContextChars);
            sb.Append(new string(' ', paddingStart));
            sb.Append('^');

            return sb.ToString();
        }

        private static void AppendContext(StringBuilder sb, string label, string s, int diffIdx)
        {
            sb.Append(label);
            int start = Math.Max(0, diffIdx - ContextChars);
            int end = Math.Min(s.Length, diffIdx + ContextChars);
            if (start > 0) sb.Append("…");
            var slice = s.Substring(start, end - start);
            if (slice.Length > MaxDiffDisplay) slice = slice.Substring(0, MaxDiffDisplay);
            sb.Append(slice.Replace("\n", "⏎").Replace("\r", "⏎").Replace("\t", "→"));
            if (end < s.Length) sb.Append("…");
            sb.AppendLine();
        }

        private static int FirstDiffIndex(string a, string b)
        {
            int len = Math.Min(a.Length, b.Length);
            for (int i = 0; i < len; i++)
                if (a[i] != b[i]) return i;
            if (a.Length != b.Length) return len;
            return -1;
        }
    }
}
