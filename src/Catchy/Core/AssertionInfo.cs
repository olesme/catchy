using System.Text;
using Catchy.Sdk;

namespace Catchy
{
    public enum AssertionStatus { Passed, Failed, Skipped }

    public sealed class AssertionInfo(IReadOnlyList<string> links, SourceLocation source, object? actual, object? expected,
        string? userMessage, AssertionStatus status,
        TimeSpan duration = default, Exception? exception = null)
    {
        private string? _rendered;
        public IReadOnlyList<string> Links { get; } = links;
        public SourceLocation Source { get; } = source;
        public object? Actual { get; } = actual; 
        public object? Expected { get; } = expected;
        public string? UserMessage { get; } = userMessage; 
        public AssertionStatus Status { get; } = status;
        public TimeSpan Duration { get; } = duration; 
        public Exception? Exception { get; } = exception;

        public override string ToString() => _rendered ??= Build();

        private string Build()
        {
            var sb = new StringBuilder();
            var (chain, trunc) = ChainRenderer.Render(Links);
            if (!string.IsNullOrEmpty(chain))
            {
                sb.AppendLine(Status switch
                {
                    AssertionStatus.Passed => "Assertion passed:",
                    AssertionStatus.Skipped => "Assertion skipped:",
                    _ => "Assertion failed:"
                });
                sb.Append("  "); sb.AppendLine(chain);
                sb.AppendLine();
                if (!string.IsNullOrEmpty(UserMessage)) sb.AppendLine($"Because  : {UserMessage}");
                if (Exception is not null) sb.AppendLine(Exception.Message);
                if (trunc.Count > 0)
                {
                    sb.AppendLine(trunc.Count == 1 ? "One chain link was truncated:"
                        : $"{trunc.Count} chain links were truncated:");
                    foreach (var (ph, full) in trunc) sb.AppendLine($"  {ph} => \"{full}\"");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(UserMessage)) sb.AppendLine($"Because  : {UserMessage}");
                if (Exception is not null) sb.AppendLine(Exception.Message);
            }
            return sb.ToString().TrimEnd();
        }
    }
}
