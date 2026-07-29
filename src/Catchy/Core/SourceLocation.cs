namespace Catchy
{
    public readonly struct SourceLocation(string? file, int line, string? member)
    {
        public string? File { get; } = file;
        public int Line { get; } = line;
        public string? Member { get; } = member;

        public override string ToString() =>
            $"{System.IO.Path.GetFileName(File)}:{Line} ({Member})";
    }
}
