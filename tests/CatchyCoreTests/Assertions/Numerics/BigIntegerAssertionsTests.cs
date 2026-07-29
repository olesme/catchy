using System.Numerics;
using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Assertions.Numerics
{
    public class BigIntegerAssertionsTests
    {
        [Fact]
        public async Task Is_WithMatchingValue_Passes()
        {
            await Stateless.Assert.That(BigInteger.Parse("12345678901234567890")).Is(BigInteger.Parse("12345678901234567890"));
        }

        [Fact]
        public async Task Is_WithDifferentValue_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(new BigInteger(42)).Is(new BigInteger(99))
            );

            Assert.Contains("42", message);
            Assert.Contains("99", message);
        }

        [Fact]
        public async Task IsGreaterThan_WithLargerValue_Passes()
        {
            await Stateless.Assert.That(new BigInteger(1000)).IsGreaterThan(new BigInteger(999));
        }

        [Fact]
        public async Task IsBetween_WithValueInRange_Passes()
        {
            await Stateless.Assert.That(new BigInteger(50)).IsBetween(new BigInteger(1), new BigInteger(100));
        }

        [Fact]
        public async Task IsBetween_WithValueOutOfRange_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(new BigInteger(101)).IsBetween(new BigInteger(1), new BigInteger(100))
            );

            Assert.Contains("101", message);
        }

        [Fact]
        public async Task IsPositive_WithPositiveValue_Passes()
        {
            await Stateless.Assert.That(new BigInteger(1)).IsPositive();
        }

        [Fact]
        public async Task IsZero_WithZero_Passes()
        {
            await Stateless.Assert.That(BigInteger.Zero).IsZero();
        }

        [Fact]
        public async Task IsNegative_WithNegativeValue_Passes()
        {
            await Stateless.Assert.That(new BigInteger(-1)).IsNegative();
        }

        [Fact]
        public async Task NullableBigInteger_IsNull_WithNull_Passes()
        {
            BigInteger? value = null;
            await Stateless.Assert.That(value).IsNull();
        }

        [Fact]
        public async Task NullableBigInteger_IsNotNull_WithValue_Passes()
        {
            BigInteger? value = new BigInteger(7);
            await Stateless.Assert.That(value).IsNotNull();
        }
    }
}
