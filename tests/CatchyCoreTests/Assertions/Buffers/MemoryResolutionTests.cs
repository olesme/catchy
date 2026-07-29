using Catchy;

namespace CatchyCoreTests.Assertions.Buffers
{
    public class MemoryResolutionTests
    {
        private static readonly int[] array = [1, 2, 3];
        private static readonly int[] array0 = [7, 8, 9];
        private static readonly int[] array1 = [4, 5, 6];

        [Fact]
        public void ReadOnlyMemory_Resolves_To_StructuralAssertions()
        {
            var memory = array.AsMemory();

            var assertions = Stateless.Assert.That(memory);

            Assert.IsType<ValueAssertions<Memory<int>>>(assertions);
        }

        [Fact]
        public void Memory_Resolves_To_StructuralAssertions()
        {
            Memory<int> memory = array1.AsMemory();

            var assertions = Stateless.Assert.That(memory);

            Assert.IsType<ValueAssertions<Memory<int>>>(assertions);
        }

        [Fact]
        public void ReadOnlyMemory_FromVariable_Resolves_To_StructuralAssertions()
        {
            ReadOnlyMemory<int> memory = array0.AsMemory();

            var assertions = Stateless.Assert.That(memory);

            Assert.IsType<ValueAssertions<ReadOnlyMemory<int>>>(assertions);
        }
    }
}
