using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Catchy.MSTest
{
    /// <summary>
    /// Base class for MSTest tests that use Ambient assertions.
    /// Automatically flushes soft assertions after each test.
    /// </summary>
    public abstract class AmbientMSTestBase
    {
        public TestContext TestContext { get; set; } = null!;

        [TestInitialize]
        public void AmbientTestSetup()
        {
            MsTestStatefulAsserterProvider.Init(TestContext);
        }

        [TestCleanup]
        public async Task AmbientTestCleanup()
        {
            var sofAsserter = Ambient.Assert.Soft;
            await sofAsserter!.SoftState.FlushIfNeeded();
        }
    }
}
