using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Catchy;

namespace Catchy.MSTest
{
    [TestClass]
    public class CatchyHooksExample
    {
        public TestContext TestContext { get; set; } = null!;

        [AssemblyInitialize]
        public static void CatchySetup(TestContext _)
        {
            MsTestStatefulAsserterProvider.Register();
        }

        [TestInitialize]
        public void TestSetup()
        {
            AmbientAsserterSource.Clear();
            MsTestStatefulAsserterProvider.Init(TestContext);
        }

        [TestCleanup]
        public async Task CatchyTeardown()
        {
            await Ambient.Assert.Soft.SoftState.FlushIfNeeded(); // Throw
        }
    }
}
