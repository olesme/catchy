using Catchy;
using static Catchy.StatelessAlias;

// [AssertFor] in this smoke project is generated only because
// Catchy.SourceGenerator is referenced explicitly in the project file.

namespace NuGetPackageSmoke;

public sealed class DemoModel
{
    public string Name { get; init; } = string.Empty;
}

[AssertFor(typeof(DemoModel))]
public static partial class DemoModelAssertions
{
    [Assertion("have expected name")]
    public static bool HasName(DemoModel model, string expected)
        => string.Equals(model.Name, expected, StringComparison.Ordinal);
}

internal static class Program
{
    private static async Task Main()
    {
        await Check.That(new DemoModel { Name = "smoke" }).HasName("smoke");
        _ = Check.ThatAnyOf(1, 2, 3);
    }
}
