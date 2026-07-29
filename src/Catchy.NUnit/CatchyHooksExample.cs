using System.Threading.Tasks;
using NUnit.Framework;

[assembly: NonTestAssembly]
namespace Catchy.NUnit
{
    [TestFixture]
    public static class CatchyHooksExample
    {
        [OneTimeSetUp]
        public static void SetUpAmbientState()
        {
            NUnitSoftAsserterProvider.Register();
        }

        [SetUp]
        public static void ClearAmbientState()
        {
            AmbientAsserterSource.Clear();
        }

        [TearDown]
        public static async Task FlushIfNeeded()
        {
            await Ambient.Assert.Soft.SoftState.FlushIfNeeded();
        }
    }
}
