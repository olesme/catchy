using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Catchy.NUnit
{
    /// <summary>
    /// Base class for NUnit tests that use Ambient assertions.
    /// Automatically flushes soft assertions after each test.
    /// </summary>
    public abstract class AmbientNUnitBase
    {
        [OneTimeSetUp]
        public static void AmbientOneTimeSetUp()
        {
            NUnitSoftAsserterProvider.Register();
        }

        [TearDown]
        public async Task AmbientTearDown()
        {
            var sofAsserter = Ambient.Assert.Soft;
            await sofAsserter!.SoftState.FlushIfNeeded();
        }
    }
}
