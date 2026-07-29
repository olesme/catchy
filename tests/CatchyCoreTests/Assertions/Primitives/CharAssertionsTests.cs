using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Assertions.Primitives
{
    public class CharAssertionsTests
    {
        [Fact]
        public async Task Is_WithMatchingValue_Passes()
        {
            await Stateless.Assert.That('a').Is('a');
        }

        [Fact]
        public async Task Is_WithDifferentValue_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That('a').Is('b')
            );

            Assert.Contains("a", message.ToLower());
            Assert.Contains("b", message.ToLower());
        }

        [Fact]
        public async Task IsDigit_WithDigit_Passes()
        {
            await Stateless.Assert.That('7').IsDigit();
        }

        [Fact]
        public async Task IsDigit_WithLetter_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That('a').IsDigit()
            );

            Assert.Contains("digit", message.ToLower());
        }

        [Fact]
        public async Task IsNotDigit_WithLetter_Passes()
        {
            await Stateless.Assert.That('a').IsNotDigit();
        }

        [Fact]
        public async Task IsLetter_WithLetter_Passes()
        {
            await Stateless.Assert.That('x').IsLetter();
        }

        [Fact]
        public async Task IsLetterOrDigit_WithDigit_Passes()
        {
            await Stateless.Assert.That('9').IsLetterOrDigit();
        }

        [Fact]
        public async Task IsWhiteSpace_WithSpace_Passes()
        {
            await Stateless.Assert.That(' ').IsWhiteSpace();
        }

        [Fact]
        public async Task IsUpper_WithUppercase_Passes()
        {
            await Stateless.Assert.That('A').IsUpper();
        }

        [Fact]
        public async Task IsLower_WithLowercase_Passes()
        {
            await Stateless.Assert.That('z').IsLower();
        }

        [Fact]
        public async Task IsPunctuation_WithComma_Passes()
        {
            await Stateless.Assert.That(',').IsPunctuation();
        }

        [Fact]
        public async Task IsSymbol_WithPlus_Passes()
        {
            await Stateless.Assert.That('+').IsSymbol();
        }

        [Fact]
        public async Task IsControl_WithNewLine_Passes()
        {
            await Stateless.Assert.That('\n').IsControl();
        }

        [Fact]
        public async Task IsAscii_WithAsciiCharacter_Passes()
        {
            await Stateless.Assert.That('A').IsAscii();
        }

        [Fact]
        public async Task IsAscii_WithNonAsciiCharacter_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That('Ā').IsAscii()
            );

            Assert.Contains("ascii", message.ToLower());
        }

        [Fact]
        public async Task IsSeparator_WithParagraphSeparator_Passes()
        {
            await Stateless.Assert.That('\u2029').IsSeparator();
        }

        [Fact]
        public async Task IsSurrogate_WithHighSurrogate_Passes()
        {
            await Stateless.Assert.That('\uD83D').IsSurrogate();
        }

        [Fact]
        public async Task IsHighSurrogate_WithHighSurrogate_Passes()
        {
            await Stateless.Assert.That('\uD83D').IsHighSurrogate();
        }

        [Fact]
        public async Task IsLowSurrogate_WithLowSurrogate_Passes()
        {
            await Stateless.Assert.That('\uDE00').IsLowSurrogate();
        }

        [Fact]
        public async Task IsSurrogatePairWith_WithMatchingPair_Passes()
        {
            await Stateless.Assert.That('\uD83D').IsSurrogatePairWith('\uDE00');
        }

        [Fact]
        public async Task IsInRange_WithValueInsideRange_Passes()
        {
            await Stateless.Assert.That('m').IsInRange('a', 'z');
        }

        [Fact]
        public async Task IsInRange_WithValueOutsideRange_Fails()
        {
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That('0').IsInRange('a', 'z')
            );

            Assert.Contains("range", message.ToLower());
        }
    }
}
