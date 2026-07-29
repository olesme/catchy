using Catchy;
using CatchyCoreTests.Helpers;

namespace CatchyCoreTests.Assertions.Numerics;

/// <summary>
/// Integration tests for numeric assertions (int, long, float, double, decimal, BigInteger, Int128).
/// Covers comparison, range, between, zero/positive/negative, NaN/Infinity, and quantified checks.
/// </summary>
public class NumericAssertionsTests
{
    // ===== Integer Assertions =====

    [Fact]
    public async Task Integer_Is_WithSameValue_Passes()
    {
        await Stateless.Assert.That(42).Is(42);
    }

    [Fact]
    public async Task Integer_Is_WithDifferentValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(42).Is(99);
        });
    }

    [Fact]
    public async Task Integer_IsNot_WithDifferentValue_Passes()
    {
        await Stateless.Assert.That(42).IsNot(99);
    }

    [Fact]
    public async Task Integer_IsGreaterThan_WithGreaterValue_Passes()
    {
        await Stateless.Assert.That(10).IsGreaterThan(5);
    }

    [Fact]
    public async Task Integer_IsGreaterThan_WithLesserValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(5).IsGreaterThan(10);
        });
    }

    [Fact]
    public async Task Integer_IsLessThan_WithLesserValue_Passes()
    {
        await Stateless.Assert.That(5).IsLessThan(10);
    }

    [Fact]
    public async Task Integer_IsAtLeast_WithEqualValue_Passes()
    {
        await Stateless.Assert.That(10).IsAtLeast(10);
    }

    [Fact]
    public async Task Integer_IsAtLeast_WithGreaterValue_Passes()
    {
        await Stateless.Assert.That(11).IsAtLeast(10);
    }

    [Fact]
    public async Task Integer_IsAtLeast_WithLesserValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(9).IsAtLeast(10);
        });
    }

    [Fact]
    public async Task Integer_IsAtMost_WithEqualValue_Passes()
    {
        await Stateless.Assert.That(10).IsAtMost(10);
    }

    [Fact]
    public async Task Integer_IsAtMost_WithLesserValue_Passes()
    {
        await Stateless.Assert.That(9).IsAtMost(10);
    }

    [Fact]
    public async Task Integer_IsAtMost_WithGreaterValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(11).IsAtMost(10);
        });
    }

    // ===== Between =====

    [Fact]
    public async Task Integer_IsBetween_WithValueInRange_Passes()
    {
        await Stateless.Assert.That(5).IsBetween(1, 10);
    }

    [Fact]
    public async Task Integer_IsBetween_WithValueAtMin_Passes()
    {
        await Stateless.Assert.That(1).IsBetween(1, 10);
    }

    [Fact]
    public async Task Integer_IsBetween_WithValueAtMax_Passes()
    {
        await Stateless.Assert.That(10).IsBetween(1, 10);
    }

    [Fact]
    public async Task Integer_IsBetween_WithValueBelowMin_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(0).IsBetween(1, 10);
        });
    }

    [Fact]
    public async Task Integer_IsBetween_WithValueAboveMax_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(11).IsBetween(1, 10);
        });
    }

    [Fact]
    public async Task Integer_IsBetweenExclusively_WithValueInRange_Passes()
    {
        await Stateless.Assert.That(5).IsBetween(1, 10).Exclusively();
    }

    [Fact]
    public async Task Integer_IsBetweenExclusively_WithValueAtMin_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(1).IsBetween(1, 10).Exclusively();
        });
    }

    [Fact]
    public async Task Integer_IsBetweenExclusively_WithValueAtMax_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(10).IsBetween(1, 10).Exclusively();
        });
    }

    // ===== Sign Checks =====

    [Fact]
    public async Task Integer_IsPositive_WithPositiveValue_Passes()
    {
        await Stateless.Assert.That(1).IsPositive();
    }

    [Fact]
    public async Task Integer_IsPositive_WithZero_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(0).IsPositive();
        });
    }

    [Fact]
    public async Task Integer_IsPositive_WithNegativeValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(-1).IsPositive();
        });
    }

    [Fact]
    public async Task Integer_IsNegative_WithNegativeValue_Passes()
    {
        await Stateless.Assert.That(-1).IsNegative();
    }

    [Fact]
    public async Task Integer_IsNegative_WithZero_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(0).IsNegative();
        });
    }

    [Fact]
    public async Task Integer_IsNegative_WithPositiveValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(1).IsNegative();
        });
    }

    [Fact]
    public async Task Integer_IsZero_WithZero_Passes()
    {
        await Stateless.Assert.That(0).IsZero();
    }

    [Fact]
    public async Task Integer_IsZero_WithPositiveValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(1).IsZero();
        });
    }

    // ===== Even/Odd =====

    [Fact]
    public async Task Integer_IsEven_WithEvenValue_Passes()
    {
        await Stateless.Assert.That(4).IsEven();
    }

    [Fact]
    public async Task Integer_IsEven_WithOddValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(3).IsEven();
        });
    }

    [Fact]
    public async Task Integer_IsOdd_WithOddValue_Passes()
    {
        await Stateless.Assert.That(3).IsOdd();
    }

    [Fact]
    public async Task Integer_IsOdd_WithEvenValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(4).IsOdd();
        });
    }

    // ===== Multiple =====

    [Fact]
    public async Task Integer_IsMultipleOf_WithMultiple_Passes()
    {
        await Stateless.Assert.That(12).IsMultipleOf(3);
    }

    [Fact]
    public async Task Integer_IsMultipleOf_WithNonMultiple_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(7).IsMultipleOf(3);
        });
    }

    // ===== Long/Float/Double/Decimal =====

    [Fact]
    public async Task Long_IsBetween_Passes()
    {
        await Stateless.Assert.That(100L).IsBetween(50L, 200L);
    }

    [Fact]
    public async Task Float_IsBetween_Passes()
    {
        await Stateless.Assert.That(2.5f).IsBetween(1f, 5f);
    }

    [Fact]
    public async Task Double_IsBetween_Passes()
    {
        await Stateless.Assert.That(3.14).IsBetween(3.0, 4.0);
    }

    [Fact]
    public async Task Decimal_IsBetween_Passes()
    {
        await Stateless.Assert.That(5.5m).IsBetween(1m, 10m);
    }

    [Fact]
    public async Task UInt_IsBetween_Passes()
    {
        await Stateless.Assert.That(5u).IsBetween(1u, 10u);
    }

    [Fact]
    public async Task Short_IsBetween_Passes()
    {
        await Stateless.Assert.That((short)5).IsBetween((short)1, (short)10);
    }

    // ===== Nullable Numerics =====

    [Fact]
    public async Task NullableInt_IsBetween_WithValue_Passes()
    {
        int? value = 5;
        await Stateless.Assert.That(value).IsBetween(1, 10);
    }

    [Fact]
    public async Task NullableInt_IsNull_WithNull_Passes()
    {
        int? value = null;
        await Stateless.Assert.That(value).IsNull();
    }

    [Fact]
    public async Task NullableInt_IsNotNull_WithValue_Passes()
    {
        int? value = 5;
        await Stateless.Assert.That(value).IsNotNull();
    }

    // ===== Floating-Point Special Values =====

    [Fact]
    public async Task Double_IsNaN_WithNaN_Passes()
    {
        await Stateless.Assert.That(double.NaN).IsNaN();
    }

    [Fact]
    public async Task Double_IsNaN_WithRegularValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(3.14).IsNaN();
        });
    }

    [Fact]
    public async Task Double_IsNotNaN_WithRegularValue_Passes()
    {
        await Stateless.Assert.That(3.14).IsNotNaN();
    }

    [Fact]
    public async Task Double_IsInfinity_WithPositiveInfinity_Passes()
    {
        await Stateless.Assert.That(double.PositiveInfinity).IsInfinity();
    }

    [Fact]
    public async Task Double_IsInfinity_WithNegativeInfinity_Passes()
    {
        await Stateless.Assert.That(double.NegativeInfinity).IsInfinity();
    }

    [Fact]
    public async Task Double_IsInfinity_WithRegularValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(3.14).IsInfinity();
        });
    }

    [Fact]
    public async Task Double_IsNotInfinity_WithRegularValue_Passes()
    {
        await Stateless.Assert.That(3.14).IsNotInfinity();
    }

    [Fact]
    public async Task Double_IsPositiveInfinity_Passes()
    {
        await Stateless.Assert.That(double.PositiveInfinity).IsPositiveInfinity();
    }

    [Fact]
    public async Task Double_IsPositiveInfinity_WithNegativeInfinity_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(double.NegativeInfinity).IsPositiveInfinity();
        });
    }

    [Fact]
    public async Task Double_IsNegativeInfinity_Passes()
    {
        await Stateless.Assert.That(double.NegativeInfinity).IsNegativeInfinity();
    }

    [Fact]
    public async Task Float_IsNaN_WithNaN_Passes()
    {
        await Stateless.Assert.That(float.NaN).IsNaN();
    }

    [Fact]
    public async Task Float_IsInfinity_WithInfinity_Passes()
    {
        await Stateless.Assert.That(float.PositiveInfinity).IsInfinity();
    }

    // ===== Modifiers & Chaining =====

    [Fact]
    public async Task Integer_IsGreaterThan_WithBecause_IncludesMessage()
    {
        AssertionException? ex = null;
        try
        {
            await Stateless.Assert.That(5).IsGreaterThan(10).Because("value must be at least 10");
        }
        catch (AssertionException e)
        {
            ex = e;
        }

        if (ex == null) throw new AssertionException("Expected AssertionException");
        if (!ex.Message.Contains("value must be at least 10")) throw new AssertionException($"Message does not contain expected reason. Got: {ex.Message}");
    }

    [Fact]
    public async Task Integer_IsBetween_WithWhen_SkipsWhenConditionFalse()
    {
        // Should not throw even though 50 is not between 1-10, because When is false
        await Stateless.Assert.That(50).When(false).IsBetween(1, 10);
    }

    [Fact]
    public async Task Integer_IsGreaterThan_WithAnd_Chains()
    {
        await Stateless.Assert.That(15).IsGreaterThan(10).And().IsLessThan(20);
    }

    // ===== Additional Numeric API Coverage =====

    [Fact]
    public async Task Integer_IsGreaterThanOrEqualTo_WithEqualValue_Passes()
    {
        await Stateless.Assert.That(10).IsGreaterThanOrEqualTo(10);
    }

    [Fact]
    public async Task Integer_IsLessThanOrEqualTo_WithEqualValue_Passes()
    {
        await Stateless.Assert.That(10).IsLessThanOrEqualTo(10);
    }

    [Fact]
    public async Task Double_IsCloseTo_WithTolerance_Passes()
    {
        await Stateless.Assert.That(10.05).IsCloseTo(10.0, 0.1);
    }

    [Fact]
    public async Task Double_IsCloseTo_OutsideTolerance_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(10.3).IsCloseTo(10.0, 0.1);
        });
    }

    [Fact]
    public async Task Double_IsApproximately_WithTolerance_Passes()
    {
        await Stateless.Assert.That(9.95).IsApproximately(10.0, 0.1);
    }

    [Fact]
    public async Task Integer_IsDivisibleBy_WithDivisibleValue_Passes()
    {
        await Stateless.Assert.That(12).IsDivisibleBy(3);
    }

    [Fact]
    public async Task Integer_IsNotDivisibleBy_WithNonDivisibleValue_Passes()
    {
        await Stateless.Assert.That(10).IsNotDivisibleBy(3);
    }

    [Fact]
    public async Task Integer_IsNotDivisibleBy_WithDivisibleValue_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(12).IsNotDivisibleBy(3);
        });
    }

    [Fact]
    public async Task ULong_IsDivisibleBy_WithDivisibleValue_Passes()
    {
        await Stateless.Assert.That(12UL).IsDivisibleBy(3UL);
    }

    [Fact]
    public async Task NullableInt_IsEven_WithValue_Passes()
    {
        int? value = 4;
        await Stateless.Assert.That(value).IsEven();
    }

    // ===== AmbientSoft Mode =====

    [Fact]
    public async Task Numeric_SoftMode_AccumulatesFailures()
    {
        var verify = Asserter.NewSoft();

        await verify.That(5).IsGreaterThan(10);  // Fail
        await verify.That(20).IsLessThan(30);    // Pass
        await verify.That(3).IsEven();           // Fail

        if (verify.ErrorCount != 2) throw new AssertionException($"Expected 2 errors, got {verify.ErrorCount}");
    }

    // ===== Error Messages =====

    [Fact]
    public async Task Integer_Is_FailureMessage_IncludesActualAndExpected()
    {
        try
        {
            await Stateless.Assert.That(42).Is(99);
            throw new AssertionException("Expected AssertionException");
        }
        catch (AssertionException ex)
        {
            if (!ex.Message.Contains("42")) throw new AssertionException($"Message missing actual value. Got: {ex.Message}");
            if (!ex.Message.Contains("99")) throw new AssertionException($"Message missing expected value. Got: {ex.Message}");
        }
    }
}





