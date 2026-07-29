using Catchy;
using CatchyTestHelpers;
using static Catchy.StatelessAlias;

namespace CatchyCecilTests
{
    /// <summary>
    /// Integration tests for Cecil-based assembly dependency assertions.
    /// Tests real assembly references using Mono.Cecil.
    /// </summary>
    public class AssemblyDependencyTests
    {
        [Fact]
        public async Task Assembly_References_CoreFX_Positive()
        {
            // Test that this assembly references System.Runtime (core .NET assembly)
            var thisAssembly = typeof(AssemblyDependencyTests).Assembly;

            await Check.That(thisAssembly).References("System.Runtime");
        }

        [Fact]
        public async Task Assembly_References_CoreFX_Negative()
        {
            // Test failure when assembly doesn't reference expected assembly
            var thisAssembly = typeof(AssemblyDependencyTests).Assembly;

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That(thisAssembly).References("Non.Existent.Assembly"));

            Assert.Contains("Expected assembly", message);
        }

        [Fact]
        public async Task Assembly_DoesNotReference_InvalidAssembly_Positive()
        {
            // Test that this assembly doesn't reference a non-existent assembly
            var thisAssembly = typeof(AssemblyDependencyTests).Assembly;

            await Check.That(thisAssembly).DoesNotReference("Non.Existent.Assembly.xyz");
        }

        [Fact]
        public async Task Assembly_DoesNotReference_InvalidAssembly_Negative()
        {
            // Test failure when assembly references something it shouldn't
            var thisAssembly = typeof(AssemblyDependencyTests).Assembly;

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That(thisAssembly).DoesNotReference("System.Runtime"));

            Assert.Contains("Expected assembly", message);
        }

        [Fact]
        public async Task Assembly_References_CatchyCore_Positive()
        {
            // Test that this assembly references Catchy core
            var thisAssembly = typeof(AssemblyDependencyTests).Assembly;

            await Check.That(thisAssembly).References("Catchy");
        }

        [Fact]
        public async Task Assembly_DoesNotReference_XUnitExecution_Positive()
        {
            // Test that this test assembly doesn't directly reference xunit.execution
            // (it should only reference xunit.core)
            var thisAssembly = typeof(AssemblyDependencyTests).Assembly;

            await Check.That(thisAssembly).DoesNotReference("xunit.execution");
        }

        [Fact]
        public async Task Assembly_DoesNotReference_XUnitExecution_Negative()
        {
            // Test failure when assembly references something it should
            var thisAssembly = typeof(AssemblyDependencyTests).Assembly;
            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That(thisAssembly).DoesNotReference("xunit"));

            Assert.Contains("Expected assembly", message);
        }

        [Fact]
        public async Task Type_DoesNotHaveDependencyOnIL_Positive()
        {
            // Test that types in this assembly don't depend on System.Web (ASP.NET)
            // This verifies architectural layer separation using IL analysis
            var types = new[] { typeof(AssemblyDependencyTests) };

            await Check.ThatEachOf(types).DoNotHaveDependencyOnIL("System.Web");
        }

        [Fact]
        public async Task Type_DoesNotHaveDependencyOnIL_Negative()
        {
            // Test failure when types have unwanted dependencies
            var types = new[] { typeof(AssemblyDependencyTests) };

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.ThatEachOf(types).DoNotHaveDependencyOnIL("System"));

            Assert.Contains("Expected no types to depend on", message);
        }

        [Fact]
        public async Task MultipleTypes_DoNotHaveDependencyOnIL_Positive()
        {
            // Test that multiple types don't depend on UI layers
            var types = new[] {
                typeof(AssemblyDependencyTests),
                typeof(Program) // Helper class
            };

            await Check.ThatEachOf(types).DoNotHaveDependencyOnIL("System.Windows.Forms");
            await Check.ThatEachOf(types).DoNotHaveDependencyOnIL("PresentationCore"); // WPF
        }

        [Fact]
        public async Task CatchyCore_DoesNotReference_TestAssemblies_Positive()
        {
            // Test that Catchy core doesn't reference test execution frameworks
            // This verifies proper layer separation
            var catchyAssembly = typeof(Assert).Assembly;

            await Check.That(catchyAssembly).DoesNotReference("xunit.execution");
            await Check.That(catchyAssembly).DoesNotReference("CatchyCecilTests");
        }

        [Fact]
        public async Task CatchyCore_DoesNotReference_TestAssemblies_Negative()
        {
            // Test failure when core references something it should
            var catchyAssembly = typeof(Assert).Assembly;

            var message = await TestHelpers.ShouldFailWithMessageAsync(async () =>
                await Check.That(catchyAssembly).DoesNotReference("System.Runtime"));

            Assert.Contains("Expected assembly", message);
        }
    }

    // Helper class to test type dependencies
    public class Program { }
}
