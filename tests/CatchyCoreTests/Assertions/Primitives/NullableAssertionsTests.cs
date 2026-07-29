using Catchy;

namespace CatchyCoreTests.Assertions.Primitives
{
    /// <summary>
    /// Integration tests for null/nullable assertions.
    /// Covers IsNull(), IsNotNull() across all types.
    /// </summary>
    public class NullableAssertionsTests
    {
        // ===== IsNull Tests =====

        [Fact]
        public async Task IsNull_WithNull_Passes()
        {
            // Arrange
            object? value = null;

            // Act & Verify
            await Stateless.Assert.That(value).IsNull();
        }

        [Fact]
        public async Task IsNull_WithNonNull_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
            // Arrange
            object? value = "not null";

            // Act
            await Stateless.Assert.That(value).IsNull();
            });
        }

        [Fact]
        public async Task IsNull_WithNullableInt_Null_Passes()
        {
            // Arrange
            int? value = null;

            // Act & Verify
            await Stateless.Assert.That(value).IsNull();
        }

        [Fact]
        public async Task IsNull_WithNullableInt_NonNull_Throws()
        {
            // Arrange
            int? value = 42;

            // Act
            try
            {
                await Stateless.Assert.That(value).IsNull();
                Assert.Fail("Expected AssertionException");
            }
            catch (AssertionException ex)
            {
                // Expected
                await Stateless.Assert.That(ex.Message).Contains("null");
            }
        }

        // ===== IsNotNull Tests =====

        [Fact]
        public async Task IsNotNull_WithNonNull_Passes()
        {
            // Arrange
            object? value = "not null";

            // Act & Verify
            await Stateless.Assert.That(value).IsNotNull();
        }

        [Fact]
        public async Task IsNotNull_WithNull_Throws()
        {
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
            // Arrange
            object? value = null;

            // Act
            await Stateless.Assert.That(value).IsNotNull();
            });
        }

        [Fact]
        public async Task IsNotNull_WithNullableInt_NonNull_Passes()
        {
            // Arrange
            int? value = 42;

            // Act & Verify
            await Stateless.Assert.That(value).IsNotNull();
        }

        [Fact]
        public async Task IsNotNull_WithNullableInt_Null_Throws()
        {
            // Arrange
            int? value = null;

            // Act
            try
            {
                await Stateless.Assert.That(value).IsNotNull();
                await Stateless.Assert.Fail("Expected AssertionException");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        // ===== IsDefault Tests =====

        [Fact]
        public async Task IsDefault_WithDefaultInt_Passes()
        {
            // Arrange
            int? value = default;  // 0

            // Act & Verify
            await Stateless.Assert.That(value).IsDefault();
        }

        [Fact]
        public async Task IsDefault_WithNonDefaultInt_Throws()
        {
            // Arrange
            int? value = 42;

            // Act
            try
            {
                await Stateless.Assert.That(value).IsDefault();
                await Stateless.Assert.Fail("Expected AssertionException");
            }
            catch (AssertionException)
            {
                // Expected
            }
        }

        [Fact]
        public async Task IsDefault_WithNullString_Passes()
        {
            // Arrange
            string? value = default;  // null

            // Act & Verify
            await Stateless.Assert.That(value).IsDefault();
        }

        [Fact]
        public async Task HasValue_WithNonNullValue_Passes()
        {
            // Arrange
            int? value = 42;

            // Act & Verify
            await Stateless.Assert.That(value).HasValue();
        }

        [Fact]
        public async Task HasValue_WithNullValue_Throws()
        {
            // Arrange
            int? value = null;

            // Act
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                await Stateless.Assert.That(value).HasValue();
            });
        }

        [Fact]
        public async Task IsDefault_WithNonNullString_Throws()
        {
            // Arrange
            string? value = "x";

            // Act
            await Assert.ThrowsAsync<AssertionException>(async () =>
            {
                await Stateless.Assert.That(value).IsDefault();
            });
        }

        // ===== Chaining =====

        [Fact]
        public async Task IsNotNull_ChainedWithOtherAssertions_Works()
        {
            // Arrange
            int? value = 42;

            // Act & Verify
            await Stateless.Assert.That(value)
                .IsNotNull()
                .And()
                .Is(42);
        }

        // ===== Error Messages =====

        [Fact]
        public async Task IsNull_FailureMessage_IsDescriptive()
        {
            // Arrange
            object? value = "not null";
            AssertionException? ex = null;

            // Act
            try
            {
                await Stateless.Assert.That(value).IsNull();
            }
            catch (AssertionException e)
            {
                ex = e;
            }

            // Verify
            await Stateless.Assert.That(ex).IsNotNull();
            await Stateless.Assert.That(ex!.Message).Contains("Expected");
            await Stateless.Assert.That(ex.Message).Contains("IsNull");
            await Stateless.Assert.That(ex.Message).Contains("null");
        }
    }
}





