using Catchy;

namespace CatchyCoreTests.Assertions.Temporal;

/// <summary>
/// Integration tests for temporal assertions (DateTime, DateTimeOffset, TimeSpan, DateOnly, TimeOnly).
/// Covers comparison, range, specific values, and date/time components.
/// </summary>
public class TemporalAssertionsTests
{
    // ===== DateTime Assertions =====

    [Fact]
    public async Task DateTime_Is_WithSameDateTime_Passes()
    {
        var dt = new DateTime(2024, 1, 15, 10, 30, 0);
        await Stateless.Assert.That(dt).Is(new DateTime(2024, 1, 15, 10, 30, 0));
    }

    [Fact]
    public async Task DateTime_Is_WithDifferentDateTime_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var dt = new DateTime(2024, 1, 15, 10, 30, 0);
        await Stateless.Assert.That(dt).Is(new DateTime(2024, 1, 15, 10, 30, 1));
        });
    }

    [Fact]
    public async Task DateTime_IsGreaterThan_WithLaterDate_Passes()
    {
        var earlier = new DateTime(2024, 1, 15);
        var later = new DateTime(2024, 1, 16);
        await Stateless.Assert.That(later).IsGreaterThan(earlier);
    }

    [Fact]
    public async Task DateTime_IsLessThan_WithEarlierDate_Passes()
    {
        var earlier = new DateTime(2024, 1, 15);
        var later = new DateTime(2024, 1, 16);
        await Stateless.Assert.That(earlier).IsLessThan(later);
    }

    [Fact]
    public async Task DateTime_IsAtLeast_WithEqualDate_Passes()
    {
        var dt = new DateTime(2024, 1, 15);
        await Stateless.Assert.That(dt).IsAtLeast(dt);
    }

    [Fact]
    public async Task DateTime_IsAtMost_WithEqualDate_Passes()
    {
        var dt = new DateTime(2024, 1, 15);
        await Stateless.Assert.That(dt).IsAtMost(dt);
    }

    [Fact]
    public async Task DateTime_IsBetween_WithDateInRange_Passes()
    {
        var min = new DateTime(2024, 1, 1);
        var max = new DateTime(2024, 12, 31);
        var mid = new DateTime(2024, 6, 15);
        await Stateless.Assert.That(mid).IsBetween(min, max);
    }

    [Fact]
    public async Task DateTime_IsBetween_WithDateOutsideRange_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var min = new DateTime(2024, 1, 1);
        var max = new DateTime(2024, 12, 31);
        var outside = new DateTime(2025, 1, 1);
        await Stateless.Assert.That(outside).IsBetween(min, max);
        });
    }

    [Fact]
    public async Task DateTime_IsToday_WithTodayDate_Passes()
    {
        var today = DateTime.Today;
        await Stateless.Assert.That(today).IsToday();
    }

    [Fact]
    public async Task DateTime_IsToday_WithYesterdayDate_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var yesterday = DateTime.Today.AddDays(-1);
        await Stateless.Assert.That(yesterday).IsToday();
        });
    }

    [Fact]
    public async Task DateTime_Year_IsEqualTo_Passes()
    {
        var dt = new DateTime(2024, 6, 15);
        await Stateless.Assert.That(dt).Year().Is(2024);
    }

    [Fact]
    public async Task DateTime_Month_IsEqualTo_Passes()
    {
        var dt = new DateTime(2024, 6, 15);
        await Stateless.Assert.That(dt).Month().Is(6);
    }

    [Fact]
    public async Task DateTime_Day_IsEqualTo_Passes()
    {
        var dt = new DateTime(2024, 6, 15);
        await Stateless.Assert.That(dt).Day().Is(15);
    }

    [Fact]
    public async Task DateTime_Hour_IsEqualTo_Passes()
    {
        var dt = new DateTime(2024, 6, 15, 14, 30, 0);
        await Stateless.Assert.That(dt).Hour().Is(14);
    }

    [Fact]
    public async Task DateTime_Minute_IsEqualTo_Passes()
    {
        var dt = new DateTime(2024, 6, 15, 14, 30, 45);
        await Stateless.Assert.That(dt).Minute().Is(30);
    }

    [Fact]
    public async Task DateTime_Second_IsEqualTo_Passes()
    {
        var dt = new DateTime(2024, 6, 15, 14, 30, 45);
        await Stateless.Assert.That(dt).Second().Is(45);
    }

    [Fact]
    public async Task DateTime_Millisecond_IsEqualTo_Passes()
    {
        var dt = new DateTime(2024, 6, 15, 14, 30, 45, 500);
        await Stateless.Assert.That(dt).Millisecond().Is(500);
    }

    [Fact]
    public async Task DateTime_DayOfWeek_IsEqualTo_Passes()
    {
        var monday = new DateTime(2024, 1, 1);  // Jan 1, 2024 is Monday
        await Stateless.Assert.That(monday).DayOfWeek().Is(System.DayOfWeek.Monday);
    }

    // ===== DateTimeOffset Assertions =====

    [Fact]
    public async Task DateTimeOffset_Is_WithSameDateTimeOffset_Passes()
    {
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        await Stateless.Assert.That(dto).Is(new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2)));
    }

    [Fact]
    public async Task DateTimeOffset_IsGreaterThan_WithLaterDateTime_Passes()
    {
        var earlier = new DateTimeOffset(2024, 1, 15, 0, 0, 0, TimeSpan.Zero);
        var later = new DateTimeOffset(2024, 1, 16, 0, 0, 0, TimeSpan.Zero);
        await Stateless.Assert.That(later).IsGreaterThan(earlier);
    }

    [Fact]
    public async Task DateTimeOffset_IsBetween_WithDateTimeOffsetInRange_Passes()
    {
        var min = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var max = new DateTimeOffset(2024, 12, 31, 23, 59, 59, TimeSpan.Zero);
        var mid = new DateTimeOffset(2024, 6, 15, 12, 0, 0, TimeSpan.Zero);
        await Stateless.Assert.That(mid).IsBetween(min, max);
    }

    // ===== TimeSpan Assertions =====

    [Fact]
    public async Task TimeSpan_Is_WithSameTimeSpan_Passes()
    {
        var ts = TimeSpan.FromHours(2);
        await Stateless.Assert.That(ts).Is(TimeSpan.FromHours(2));
    }

    [Fact]
    public async Task TimeSpan_Is_WithDifferentTimeSpan_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        var ts = TimeSpan.FromHours(2);
        await Stateless.Assert.That(ts).Is(TimeSpan.FromHours(3));
        });
    }

    [Fact]
    public async Task TimeSpan_IsGreaterThan_WithLongerTimeSpan_Passes()
    {
        await Stateless.Assert.That(TimeSpan.FromHours(3)).IsGreaterThan(TimeSpan.FromHours(1));
    }

    [Fact]
    public async Task TimeSpan_IsLessThan_WithShorterTimeSpan_Passes()
    {
        await Stateless.Assert.That(TimeSpan.FromMinutes(30)).IsLessThan(TimeSpan.FromHours(1));
    }

    [Fact]
    public async Task TimeSpan_IsBetween_WithTimeSpanInRange_Passes()
    {
        var min = TimeSpan.FromSeconds(30);
        var max = TimeSpan.FromMinutes(5);
        var mid = TimeSpan.FromSeconds(90);
        await Stateless.Assert.That(mid).IsBetween(min, max);
    }

    [Fact]
    public async Task TimeSpan_IsPositive_WithPositiveTimeSpan_Passes()
    {
        await Stateless.Assert.That(TimeSpan.FromHours(1)).IsPositive();
    }

    [Fact]
    public async Task TimeSpan_IsPositive_WithZero_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
        await Stateless.Assert.That(TimeSpan.Zero).IsPositive();
        });
    }

    [Fact]
    public async Task TimeSpan_IsNegative_WithNegativeTimeSpan_Passes()
    {
        await Stateless.Assert.That(TimeSpan.FromHours(-1)).IsNegative();
    }

    [Fact]
    public async Task TimeSpan_IsZero_WithZero_Passes()
    {
        await Stateless.Assert.That(TimeSpan.Zero).IsZero();
    }

    [Fact]
    public async Task TimeSpan_IsPositive_WithNegativeTimeSpan_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
            await Stateless.Assert.That(TimeSpan.FromHours(-1)).IsPositive();
        });
    }

    [Fact]
    public async Task TimeSpan_IsZero_WithNonZero_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
            await Stateless.Assert.That(TimeSpan.FromSeconds(1)).IsZero();
        });
    }

    [Fact]
    public async Task TimeSpan_TotalSeconds_IsGreaterThan_Passes()
    {
        var ts = TimeSpan.FromSeconds(100);
        await Stateless.Assert.That(ts).TotalSeconds().IsGreaterThan(50);
    }

    [Fact]
    public async Task TimeSpan_TotalMilliseconds_IsGreaterThan_Passes()
    {
        var ts = TimeSpan.FromMilliseconds(5000);
        await Stateless.Assert.That(ts).TotalMilliseconds().IsGreaterThan(1000);
    }

    [Fact]
    public async Task TimeSpan_Days_IsEqualTo_Passes()
    {
        var ts = TimeSpan.FromDays(3);
        await Stateless.Assert.That(ts).TotalDays().Is(3);
    }

    [Fact]
    public async Task TimeSpan_Hours_IsEqualTo_Passes()
    {
        var ts = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30);
        await Stateless.Assert.That(ts).TotalHours().Is(5.5);
    }

    [Fact]
    public async Task TimeSpan_Minutes_IsEqualTo_Passes()
    {
        var ts = TimeSpan.FromMinutes(45);
        await Stateless.Assert.That(ts).TotalMinutes().Is(45);
    }

    // ===== DateOnly Assertions =====

    [Fact]
    public async Task DateOnly_Is_WithSameDateOnly_Passes()
    {
        var date = new DateOnly(2024, 6, 15);
        await Stateless.Assert.That(date).Is(new DateOnly(2024, 6, 15));
    }

    [Fact]
    public async Task DateOnly_IsGreaterThan_WithLaterDate_Passes()
    {
        var earlier = new DateOnly(2024, 1, 15);
        var later = new DateOnly(2024, 1, 16);
        await Stateless.Assert.That(later).IsGreaterThan(earlier);
    }

#if NET6_0_OR_GREATER
    [Fact]
    public async Task DateOnly_IsBetween_WithDateInRange_Passes()
    {
        var min = new DateOnly(2024, 1, 1);
        var max = new DateOnly(2024, 12, 31);
        var mid = new DateOnly(2024, 6, 15);
        await Stateless.Assert.That(mid).IsBetween(min, max);
    }

    [Fact]
    public async Task DateOnly_Year_IsEqualTo_Passes()
    {
        var date = new DateOnly(2024, 6, 15);
        await Stateless.Assert.That(date).Year().Is(2024);
    }

    [Fact]
    public async Task DateOnly_Month_IsEqualTo_Passes()
    {
        var date = new DateOnly(2024, 6, 15);
        await Stateless.Assert.That(date).Month().Is(6);
    }

    [Fact]
    public async Task DateOnly_Day_IsEqualTo_Passes()
    {
        var date = new DateOnly(2024, 6, 15);
        await Stateless.Assert.That(date).Day().Is(15);
    }

    [Fact]
    public async Task DateOnly_DayNumber_IsEqualTo_Passes()
    {
        var date = new DateOnly(2024, 6, 15);
        await Stateless.Assert.That(date).DayNumber().Is(739051);
    }
#endif

    // ===== TimeOnly Assertions =====

#if NET6_0_OR_GREATER
    [Fact]
    public async Task TimeOnly_Is_WithSameTimeOnly_Passes()
    {
        var time = new TimeOnly(14, 30, 45);
        await Stateless.Assert.That(time).Is(new TimeOnly(14, 30, 45));
    }

    [Fact]
    public async Task TimeOnly_IsGreaterThan_WithLaterTime_Passes()
    {
        var earlier = new TimeOnly(10, 0);
        var later = new TimeOnly(15, 0);
        await Stateless.Assert.That(later).IsGreaterThan(earlier);
    }

    [Fact]
    public async Task TimeOnly_IsBetween_WithTimeInRange_Passes()
    {
        var min = new TimeOnly(9, 0);
        var max = new TimeOnly(17, 0);
        var mid = new TimeOnly(12, 30);
        await Stateless.Assert.That(mid).IsBetween(min, max);
    }

    [Fact]
    public async Task TimeOnly_Hour_IsEqualTo_Passes()
    {
        var time = new TimeOnly(14, 30, 45);
        await Stateless.Assert.That(time).Hour().Is(14);
    }

    [Fact]
    public async Task TimeOnly_Minute_IsEqualTo_Passes()
    {
        var time = new TimeOnly(14, 30, 45);
        await Stateless.Assert.That(time).Minute().Is(30);
    }

    [Fact]
    public async Task TimeOnly_Second_IsEqualTo_Passes()
    {
        var time = new TimeOnly(14, 30, 45);
        await Stateless.Assert.That(time).Second().Is(45);
    }
#endif

    // ===== Additional Temporal API Coverage =====

    [Fact]
    public async Task DateTime_IsAtLeast_WithLaterDate_Passes()
    {
        await Stateless.Assert.That(new DateTime(2024, 1, 3)).IsAtLeast(new DateTime(2024, 1, 2));
    }

    [Fact]
    public async Task DateTime_IsAtMost_WithEarlierDate_Passes()
    {
        await Stateless.Assert.That(new DateTime(2024, 1, 2)).IsAtMost(new DateTime(2024, 1, 3));
    }

    [Fact]
    public async Task DateTime_IsInThePast_WithPastDate_Passes()
    {
        await Stateless.Assert.That(DateTime.Now.AddMinutes(-2)).IsInThePast();
    }

    [Fact]
    public async Task DateTime_IsInTheFuture_WithFutureDate_Passes()
    {
        await Stateless.Assert.That(DateTime.Now.AddMinutes(2)).IsInTheFuture();
    }

    [Fact]
    public async Task DateTime_IsInThePast_WithFutureDate_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
            await Stateless.Assert.That(DateTime.Now.AddMinutes(2)).IsInThePast();
        });
    }

    [Fact]
    public async Task DateTime_IsInTheFuture_WithPastDate_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
            await Stateless.Assert.That(DateTime.Now.AddMinutes(-2)).IsInTheFuture();
        });
    }

    [Fact]
    public async Task DateTimeOffset_HasOffset_Passes()
    {
        var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
        await Stateless.Assert.That(dto).HasOffset(TimeSpan.FromHours(2), null);
    }

    [Fact]
    public async Task DateTimeOffset_HasOffset_WithWrongOffset_Throws()
    {
        await Assert.ThrowsAsync<AssertionException>(async () =>
        {
            var dto = new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.FromHours(2));
            await Stateless.Assert.That(dto).HasOffset(TimeSpan.FromHours(1), null);
        });
    }

    [Fact]
    public async Task DateTimeOffset_IsInThePast_WithPastDate_Passes()
    {
        await Stateless.Assert.That(DateTimeOffset.Now.AddMinutes(-2)).IsInThePast();
    }

    [Fact]
    public async Task DateTimeOffset_IsInTheFuture_WithFutureDate_Passes()
    {
        await Stateless.Assert.That(DateTimeOffset.Now.AddMinutes(2)).IsInTheFuture();
    }

    // ===== Modifiers & Chaining =====

    [Fact]
    public async Task DateTime_IsGreaterThan_WithBecause_IncludesMessage()
    {
        var earlier = new DateTime(2024, 1, 15);
        var later = new DateTime(2024, 1, 14);

        try
        {
            await Stateless.Assert.That(later).IsGreaterThan(earlier).Because("date must be in the future");
            throw new AssertionException("Expected AssertionException");
        }
        catch (AssertionException ex)
        {
            if (!ex.Message.Contains("date must be in the future"))
                throw new AssertionException($"Message missing reason. Got: {ex.Message}");
        }
    }

    [Fact]
    public async Task TimeSpan_IsBetween_WithAnd_Chains()
    {
        var min = TimeSpan.FromSeconds(30);
        var max = TimeSpan.FromMinutes(5);
        await Stateless.Assert.That(TimeSpan.FromSeconds(90)).IsBetween(min, max).And().IsPositive();
    }

    // ===== AmbientSoft Mode =====

    [Fact]
    public async Task Temporal_SoftMode_AccumulatesFailures()
    {
        var verify = Asserter.NewSoft();
        var dt = new DateTime(2024, 6, 15);

        await verify.That(dt).IsGreaterThan(DateTime.Now);  // Fail
        await verify.That(dt).Month().Is(6);                 // Pass
        await verify.That(dt).Day().Is(20);                  // Fail

        if (verify.ErrorCount != 2) throw new AssertionException($"Expected 2 errors, got {verify.ErrorCount}");
    }

    // ===== Nullable Temporal =====

    [Fact]
    public async Task NullableDateTime_IsNull_WithNull_Passes()
    {
        DateTime? value = null;
        await Stateless.Assert.That(value).IsNull();
    }

    [Fact]
    public async Task NullableDateTime_IsNotNull_WithValue_Passes()
    {
        DateTime? value = new DateTime(2024, 6, 15);
        await Stateless.Assert.That(value).IsNotNull();
    }

    [Fact]
    public async Task NullableTimeSpan_IsBetween_WithValue_Passes()
    {
        TimeSpan? value = TimeSpan.FromSeconds(90);
        await Stateless.Assert.That(value).IsBetween(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(5));
    }
}




