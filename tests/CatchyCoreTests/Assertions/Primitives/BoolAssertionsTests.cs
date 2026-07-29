using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Assertions.Primitives
{
    public class BoolAssertionsTests
    {
        [Fact]
        public async Task IsTrue_WithTrue_Pass()
        {
            bool value = true;
            await Stateless.Assert.That(value).IsTrue();
        }

        [Fact]
        public async Task IsTrue_WithFalse_Fail()
        {
            bool value = false;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).IsTrue()
            );
            Assert.Contains("true", message.ToLower());
            Assert.Contains("false", message.ToLower());
        }

        [Fact]
        public async Task IsFalse_WithFalse_Pass()
        {
            bool value = false;
            await Stateless.Assert.That(value).IsFalse();
        }

        [Fact]
        public async Task IsFalse_WithTrue_Fail()
        {
            bool value = true;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).IsFalse()
            );
            Assert.Contains("false", message.ToLower());
            Assert.Contains("true", message.ToLower());
        }

        [Fact]
        public async Task ImpliedBy_WithTrueCondition_AndTrueValue_Pass()
        {
            bool value = true;
            await Stateless.Assert.That(value).ImpliedBy(true);
        }

        [Fact]
        public async Task ImpliedBy_WithTrueCondition_AndFalseValue_Fail()
        {
            bool value = false;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).ImpliedBy(true)
            );
            Assert.Contains("true", message.ToLower());
            Assert.Contains("false", message.ToLower());
        }

        [Fact]
        public async Task Implies_WithTrueValue_AndTrueConsequence_Pass()
        {
            bool value = true;
            await Stateless.Assert.That(value).Implies(true);
        }

        [Fact]
        public async Task Implies_WithTrueValue_AndFalseConsequence_Fail()
        {
            bool value = true;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Implies(false)
            );
            Assert.Contains("true", message.ToLower());
            Assert.Contains("false", message.ToLower());
        }

        [Fact]
        public async Task IsTrue_WithBecause_Pass()
        {
            bool value = true;
            await Stateless.Assert.That(value).IsTrue().Because("test reason");
        }

        [Fact]
        public async Task IsTrue_WithBecause_Fail()
        {
            bool value = false;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).IsTrue().Because("custom reason")
            );
            Assert.Contains("custom reason", message);
        }

        [Fact]
        public async Task IsTrue_WithWhen_False_Skips()
        {
            bool value = false;
            await Stateless.Assert.That(value)
                .When(false)
                .IsTrue();
        }

        [Fact]
        public async Task IsTrue_WithWhen_True_Executes()
        {
            bool value = false;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).When(true).IsTrue()
            );
            Assert.NotEmpty(message);
        }

        [Fact]
        public async Task IsTrue_And_Chain_Pass()
        {
            bool value = true;
            await Stateless.Assert.That(value)
                .IsTrue()
                .And()
                .IsTrue();
        }

        [Fact]
        public async Task IsTrue_But_Chain_Pass()
        {
            bool value = true;
            await Stateless.Assert.That(value)
                .IsTrue()
                .But()
                .IsTrue();
        }

        [Fact]
        public async Task IsTrue_WithNullable_True_Pass()
        {
            bool? value = true;
            await Stateless.Assert.That(value).IsTrue();
        }

        [Fact]
        public async Task IsTrue_WithNullable_Null_Fail()
        {
            bool? value = null;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).IsTrue()
            );
            Assert.Contains("null", message.ToLower());
        }

        [Fact]
        public async Task IsTrue_WithNullable_False_Fail()
        {
            bool? value = false;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).IsTrue()
            );
            Assert.Contains("false", message.ToLower());
        }

        [Fact]
        public async Task IsFalse_WithNullable_False_Pass()
        {
            bool? value = false;
            await Stateless.Assert.That(value).IsFalse();
        }

        [Fact]
        public async Task IsFalse_WithNullable_True_Fail()
        {
            bool? value = true;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).IsFalse()
            );
            Assert.Contains("true", message.ToLower());
        }

        [Fact]
        public async Task IsFalse_WithNullable_Null_Fail()
        {
            bool? value = null;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).IsFalse()
            );
            Assert.Contains("null", message.ToLower());
        }

        [Fact]
        public async Task ImpliedBy_WithNullableTrueValue_AndTrueCondition_Pass()
        {
            bool? value = true;
            await Stateless.Assert.That(value).ImpliedBy(true);
        }

        [Fact]
        public async Task ImpliedBy_WithNullableFalseValue_AndTrueCondition_Fail()
        {
            bool? value = false;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).ImpliedBy(true)
            );
            Assert.Contains("true", message.ToLower());
            Assert.Contains("false", message.ToLower());
        }

        [Fact]
        public async Task Implies_WithNullableTrueValue_AndTrueConsequence_Pass()
        {
            bool? value = true;
            await Stateless.Assert.That(value).Implies(true);
        }

        [Fact]
        public async Task Implies_WithNullableTrueValue_AndFalseConsequence_Fail()
        {
            bool? value = true;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Implies(false)
            );
            Assert.Contains("true", message.ToLower());
            Assert.Contains("false", message.ToLower());
        }

        [Fact]
        public async Task IsTrue_SoftMode_Pass()
        {
            var verify = Asserter.NewSoft();
            await verify.That(true).IsTrue();
            Assert.False(verify.HasFailures);
        }

        [Fact]
        public async Task IsTrue_SoftMode_Fail()
        {
            var verify = Asserter.NewSoft();
            await verify.That(false).IsTrue();
            Assert.True(verify.HasFailures);
            Assert.Single(verify.Errors);
        }

        [Fact]
        public async Task Bool_SoftMode_MultipleFailures()
        {
            var verify = Asserter.NewSoft();
            await verify.That(false).IsTrue();
            await verify.That(false).IsTrue();
            await verify.That(true).IsFalse();
            Assert.Equal(3, verify.ErrorCount);
        }

        [Fact]
        public async Task IsFalse_SoftMode_Pass()
        {
            var verify = Asserter.NewSoft();
            await verify.That(false).IsFalse();
            Assert.False(verify.HasFailures);
        }

        [Fact]
        public async Task IsFalse_SoftMode_Fail()
        {
            var verify = Asserter.NewSoft();
            await verify.That(true).IsFalse();
            Assert.True(verify.HasFailures);
        }

        // Variable based versions

        [Fact]
        public async Task IsTrueVar_WithTrue_Pass()
        {
            bool value = true;
            bool expected = true;
            await Stateless.Assert.That(value).Is(expected);
        }

        [Fact]
        public async Task IsTrueVar_WithFalse_Fail()
        {
            bool value = false;
            bool expected = true;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Is(expected)
            );
            Assert.Contains("true", message.ToLower());
            Assert.Contains("false", message.ToLower());
        }

        [Fact]
        public async Task IsFalseVar_WithFalse_Pass()
        {
            bool value = false;
            bool expected = false;
            await Stateless.Assert.That(value).Is(expected);
        }

        [Fact]
        public async Task IsFalseVar_WithTrue_Fail()
        {
            bool value = true;
            bool expected = false;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Is(expected)
            );
            Assert.Contains("false", message.ToLower());
            Assert.Contains("true", message.ToLower());
        }

        [Fact]
        public async Task IsTrueVar_WithBecause_Pass()
        {
            bool value = true;
            bool expected = true;
            await Stateless.Assert.That(value).Is(expected).Because("test reason");
        }

        [Fact]
        public async Task IsTrueVar_WithBecause_Fail()
        {
            bool value = false;
            bool expected = true;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Is(expected).Because("custom reason")
            );
            Assert.Contains("custom reason", message);
        }

        [Fact]
        public async Task IsTrueVar_WithWhen_False_Skips()
        {
            bool value = false;
            bool expected = true;
            await Stateless.Assert.That(value)
                .When(false)
                .Is(expected);
        }

        [Fact]
        public async Task IsTrueVar_WithWhen_True_Executes()
        {
            bool value = false;
            bool expected = true;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).When(true).Is(expected)
            );
            Assert.NotEmpty(message);
        }

        [Fact]
        public async Task IsTrueVar_And_Chain_Pass()
        {
            bool value = true;
            bool expected = true;
            await Stateless.Assert.That(value)
                .Is(expected)
                .And()
                .Is(expected);
        }

        [Fact]
        public async Task IsTrueVar_But_Chain_Pass()
        {
            bool value = true;
            bool expected = true;
            await Stateless.Assert.That(value)
                .Is(expected)
                .But()
                .Is(expected);
        }

        [Fact]
        public async Task IsTrueVar_WithNullable_True_Pass()
        {
            bool? value = true;
            bool? expected = true;
            await Stateless.Assert.That(value).Is(expected);
        }

        [Fact]
        public async Task IsTrueVar_WithNullable_Null_Fail()
        {
            bool? value = null;
            bool? expected = true;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Is(expected)
            );
            Assert.Contains("null", message.ToLower());
        }

        [Fact]
        public async Task IsTrueVar_WithNullable_False_Fail()
        {
            bool? value = false;
            bool? expected = true;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Is(expected)
            );
            Assert.Contains("false", message.ToLower());
        }

        [Fact]
        public async Task IsFalseVar_WithNullable_False_Pass()
        {
            bool? value = false;
            bool? expected = false;
            await Stateless.Assert.That(value).Is(expected);
        }

        [Fact]
        public async Task IsFalseVar_WithNullable_True_Fail()
        {
            bool? value = true;
            bool? expected = false;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Is(expected)
            );
            Assert.Contains("true", message.ToLower());
        }

        [Fact]
        public async Task IsFalseVar_WithNullable_Null_Fail()
        {
            bool? value = null;
            bool? expected = false;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Is(expected)
            );
            Assert.Contains("null", message.ToLower());
        }

        [Fact]
        public async Task Is_Nullable_WithNull_Fail()
        {
            bool? value = true;
            bool? expected = null;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Is(expected)
            );
            Assert.Contains("null", message.ToLower());
        }

        [Fact]
        public async Task Is_WithNull_Fail()
        {
            bool value = false;
            bool? expected = null;
            var message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Is(expected)
            );
            Assert.Contains("but was False", message);
            value = true;
            message = await Catch.FailureOf(async () =>
                await Stateless.Assert.That(value).Is(expected)
            );
            Assert.Contains("but was True", message);
        }

        [Fact]
        public async Task IsTrueVar_SoftMode_Pass()
        {
            var verify = Asserter.NewSoft();
            bool? expected = true;
            await verify.That(true).Is(expected);
            Assert.False(verify.HasFailures);
        }

        [Fact]
        public async Task IsTrueVar_SoftMode_Fail()
        {
            var verify = Asserter.NewSoft();
            bool? expected = true;
            await verify.That(false).Is(expected);
            Assert.True(verify.HasFailures);
            Assert.Single(verify.Errors);
        }

        [Fact]
        public async Task BoolVar_SoftMode_MultipleFailures()
        {
            var verify = Asserter.NewSoft();
            var expectedTrue = true;
            var expectedFalse = false;
            await verify.That(false).Is(expectedTrue);
            await verify.That(false).Is(expectedTrue);
            await verify.That(true).Is(expectedFalse);
            Assert.Equal(3, verify.ErrorCount);
        }

        [Fact]
        public async Task IsFalseVar_SoftMode_Pass()
        {
            var expected = true;
            var verify = Asserter.NewSoft();
            await verify.That(false).Is(!expected);
            Assert.False(verify.HasFailures);
        }

        [Fact]
        public async Task IsFalseVar_SoftMode_Fail()
        {
            bool? expected = true;
            var verify = Asserter.NewSoft();
            await verify.That(true).Is(!expected);
            Assert.True(verify.HasFailures);
        }
    }
}
