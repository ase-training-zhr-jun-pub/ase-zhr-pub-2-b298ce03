using Calvin.BookingService.Data;
using Calvin.BookingService.Domain;
using Xunit;

namespace Calvin.BookingService.Tests;

public class BookingRulesOverlapsTests
{
    // --- All-day (null) combinations ---

    [Fact]
    public void Overlaps_BothAllDay_ReturnsTrue()
    {
        // null/null vs null/null → both are all-day, always conflict
        Assert.True(BookingRules.Overlaps(null, null, null, null));
    }

    [Fact]
    public void Overlaps_AllDayVsTimed_ReturnsTrue()
    {
        // all-day conflicts with any timed booking
        Assert.True(BookingRules.Overlaps(null, null, new TimeOnly(9, 0), new TimeOnly(10, 0)));
    }

    [Fact]
    public void Overlaps_TimedVsAllDay_ReturnsTrue()
    {
        // timed conflicts with all-day booking
        Assert.True(BookingRules.Overlaps(new TimeOnly(9, 0), new TimeOnly(10, 0), null, null));
    }

    [Fact]
    public void Overlaps_NullTimeFrom_ReturnsTrue()
    {
        // one side has null TimeFrom only → treated as all-day
        Assert.True(BookingRules.Overlaps(null, new TimeOnly(10, 0), new TimeOnly(9, 0), new TimeOnly(11, 0)));
    }

    [Fact]
    public void Overlaps_NullTimeTo_ReturnsTrue()
    {
        // one side has null TimeTo only → treated as all-day
        Assert.True(BookingRules.Overlaps(new TimeOnly(9, 0), null, new TimeOnly(10, 0), new TimeOnly(11, 0)));
    }

    // --- Touching edges (no overlap expected) ---

    [Fact]
    public void Overlaps_TouchingEdge_AEndsWhenBStarts_ReturnsFalse()
    {
        // A: 09:00–10:00, B: 10:00–11:00 → edges touch but don't overlap
        var result = BookingRules.Overlaps(
            new TimeOnly(9, 0), new TimeOnly(10, 0),
            new TimeOnly(10, 0), new TimeOnly(11, 0));
        Assert.False(result);
    }

    [Fact]
    public void Overlaps_TouchingEdge_BEndsWhenAStarts_ReturnsFalse()
    {
        // B: 08:00–09:00, A: 09:00–10:00 → edges touch but don't overlap
        var result = BookingRules.Overlaps(
            new TimeOnly(9, 0), new TimeOnly(10, 0),
            new TimeOnly(8, 0), new TimeOnly(9, 0));
        Assert.False(result);
    }

    // --- Clear overlap cases ---

    [Fact]
    public void Overlaps_PartialOverlapAStartsBeforeB_ReturnsTrue()
    {
        // A: 09:00–11:00, B: 10:00–12:00 → overlap 10:00–11:00
        var result = BookingRules.Overlaps(
            new TimeOnly(9, 0), new TimeOnly(11, 0),
            new TimeOnly(10, 0), new TimeOnly(12, 0));
        Assert.True(result);
    }

    [Fact]
    public void Overlaps_PartialOverlapBStartsBeforeA_ReturnsTrue()
    {
        // A: 10:00–12:00, B: 09:00–11:00 → overlap 10:00–11:00
        var result = BookingRules.Overlaps(
            new TimeOnly(10, 0), new TimeOnly(12, 0),
            new TimeOnly(9, 0), new TimeOnly(11, 0));
        Assert.True(result);
    }

    [Fact]
    public void Overlaps_AContainsB_ReturnsTrue()
    {
        // A: 09:00–12:00, B: 10:00–11:00 → B is fully inside A
        var result = BookingRules.Overlaps(
            new TimeOnly(9, 0), new TimeOnly(12, 0),
            new TimeOnly(10, 0), new TimeOnly(11, 0));
        Assert.True(result);
    }

    [Fact]
    public void Overlaps_BContainsA_ReturnsTrue()
    {
        // A: 10:00–11:00, B: 09:00–12:00 → A is fully inside B
        var result = BookingRules.Overlaps(
            new TimeOnly(10, 0), new TimeOnly(11, 0),
            new TimeOnly(9, 0), new TimeOnly(12, 0));
        Assert.True(result);
    }

    [Fact]
    public void Overlaps_ExactSameTimeRange_ReturnsTrue()
    {
        // A and B are identical
        var result = BookingRules.Overlaps(
            new TimeOnly(10, 0), new TimeOnly(11, 0),
            new TimeOnly(10, 0), new TimeOnly(11, 0));
        Assert.True(result);
    }

    // --- No-overlap cases (fully disjoint) ---

    [Fact]
    public void Overlaps_ACompletelyBeforeB_ReturnsFalse()
    {
        // A: 08:00–09:00, B: 10:00–11:00 → no overlap
        var result = BookingRules.Overlaps(
            new TimeOnly(8, 0), new TimeOnly(9, 0),
            new TimeOnly(10, 0), new TimeOnly(11, 0));
        Assert.False(result);
    }

    [Fact]
    public void Overlaps_ACompletelyAfterB_ReturnsFalse()
    {
        // A: 14:00–15:00, B: 10:00–12:00 → no overlap
        var result = BookingRules.Overlaps(
            new TimeOnly(14, 0), new TimeOnly(15, 0),
            new TimeOnly(10, 0), new TimeOnly(12, 0));
        Assert.False(result);
    }
}

public class BookingRulesFindConflictTests
{
    private static Booking MakeBooking(
        string id,
        string resourceId,
        DateOnly date,
        TimeOnly? from = null,
        TimeOnly? to = null,
        string status = BookingStatus.Active)
        => new Booking
        {
            Id = id,
            ResourceId = resourceId,
            Date = date,
            TimeFrom = from,
            TimeTo = to,
            Status = status
        };

    private static readonly DateOnly TestDate = new DateOnly(2026, 6, 18);
    private static readonly string TestResource = "room-1";

    [Fact]
    public void FindConflict_EmptyList_ReturnsNull()
    {
        var result = BookingRules.FindConflict(
            [],
            TestResource,
            TestDate,
            new TimeOnly(10, 0),
            new TimeOnly(11, 0));
        Assert.Null(result);
    }

    [Fact]
    public void FindConflict_OverlappingActiveBooking_ReturnsConflict()
    {
        var existing = MakeBooking("CLVN-B-1001", TestResource, TestDate,
            new TimeOnly(9, 0), new TimeOnly(11, 0));

        var result = BookingRules.FindConflict(
            [existing],
            TestResource,
            TestDate,
            new TimeOnly(10, 0),
            new TimeOnly(12, 0));

        Assert.NotNull(result);
        Assert.Equal("CLVN-B-1001", result.Id);
    }

    [Fact]
    public void FindConflict_NonOverlappingActiveBooking_ReturnsNull()
    {
        var existing = MakeBooking("CLVN-B-1001", TestResource, TestDate,
            new TimeOnly(8, 0), new TimeOnly(9, 0));

        var result = BookingRules.FindConflict(
            [existing],
            TestResource,
            TestDate,
            new TimeOnly(10, 0),
            new TimeOnly(11, 0));

        Assert.Null(result);
    }

    [Fact]
    public void FindConflict_CancelledBookingWithOverlap_ReturnsNull()
    {
        // Cancelled bookings must be ignored even if they overlap
        var existing = MakeBooking("CLVN-B-1001", TestResource, TestDate,
            new TimeOnly(9, 0), new TimeOnly(11, 0),
            status: BookingStatus.Cancelled);

        var result = BookingRules.FindConflict(
            [existing],
            TestResource,
            TestDate,
            new TimeOnly(10, 0),
            new TimeOnly(12, 0));

        Assert.Null(result);
    }

    [Fact]
    public void FindConflict_ExcludeId_IgnoresMatchingBooking()
    {
        // PUT scenario: updating CLVN-B-1001 must not conflict with itself
        var existing = MakeBooking("CLVN-B-1001", TestResource, TestDate,
            new TimeOnly(9, 0), new TimeOnly(11, 0));

        var result = BookingRules.FindConflict(
            [existing],
            TestResource,
            TestDate,
            new TimeOnly(9, 0),
            new TimeOnly(11, 0),
            excludeId: "CLVN-B-1001");

        Assert.Null(result);
    }

    [Fact]
    public void FindConflict_ExcludeId_StillFindsOtherConflicts()
    {
        // Excluding one booking should not hide other conflicts
        var excluded = MakeBooking("CLVN-B-1001", TestResource, TestDate,
            new TimeOnly(9, 0), new TimeOnly(11, 0));
        var other = MakeBooking("CLVN-B-1002", TestResource, TestDate,
            new TimeOnly(10, 0), new TimeOnly(12, 0));

        var result = BookingRules.FindConflict(
            [excluded, other],
            TestResource,
            TestDate,
            new TimeOnly(9, 0),
            new TimeOnly(11, 0),
            excludeId: "CLVN-B-1001");

        Assert.NotNull(result);
        Assert.Equal("CLVN-B-1002", result.Id);
    }

    [Fact]
    public void FindConflict_DifferentResource_ReturnsNull()
    {
        var existing = MakeBooking("CLVN-B-1001", "room-2", TestDate,
            new TimeOnly(9, 0), new TimeOnly(11, 0));

        var result = BookingRules.FindConflict(
            [existing],
            TestResource,   // looking for room-1
            TestDate,
            new TimeOnly(10, 0),
            new TimeOnly(12, 0));

        Assert.Null(result);
    }

    [Fact]
    public void FindConflict_DifferentDate_ReturnsNull()
    {
        var existing = MakeBooking("CLVN-B-1001", TestResource,
            new DateOnly(2026, 6, 19),
            new TimeOnly(9, 0), new TimeOnly(11, 0));

        var result = BookingRules.FindConflict(
            [existing],
            TestResource,
            TestDate,   // different date
            new TimeOnly(10, 0),
            new TimeOnly(12, 0));

        Assert.Null(result);
    }

    [Fact]
    public void FindConflict_AllDayCandidateVsTimedActive_ReturnsConflict()
    {
        // Trying to book all-day on a resource that already has a timed booking
        var existing = MakeBooking("CLVN-B-1001", TestResource, TestDate,
            new TimeOnly(9, 0), new TimeOnly(10, 0));

        var result = BookingRules.FindConflict(
            [existing],
            TestResource,
            TestDate,
            timeFrom: null,
            timeTo: null);

        Assert.NotNull(result);
    }

    [Fact]
    public void FindConflict_AllDayExistingVsTimedCandidate_ReturnsConflict()
    {
        // Resource is already booked all-day; any new booking conflicts
        var existing = MakeBooking("CLVN-B-1001", TestResource, TestDate,
            from: null, to: null);

        var result = BookingRules.FindConflict(
            [existing],
            TestResource,
            TestDate,
            new TimeOnly(14, 0),
            new TimeOnly(15, 0));

        Assert.NotNull(result);
    }
}

public class BookingRulesNextBookingIdTests
{
    private static Booking MakeBooking(string id) => new Booking { Id = id };

    [Fact]
    public void NextBookingId_EmptyList_ReturnsSeedPlusOne()
    {
        // DefaultIfEmpty seeds with 1002, so first id should be CLVN-B-1003
        var result = BookingRules.NextBookingId([]);
        Assert.Equal("CLVN-B-1003", result);
    }

    [Fact]
    public void NextBookingId_SingleBooking_ReturnsIncremented()
    {
        var result = BookingRules.NextBookingId([MakeBooking("CLVN-B-1001")]);
        Assert.Equal("CLVN-B-1002", result);
    }

    [Fact]
    public void NextBookingId_MultipleBookings_ReturnsMaxPlusOne()
    {
        var bookings = new[]
        {
            MakeBooking("CLVN-B-1001"),
            MakeBooking("CLVN-B-1005"),
            MakeBooking("CLVN-B-1003"),
        };
        var result = BookingRules.NextBookingId(bookings);
        Assert.Equal("CLVN-B-1006", result);
    }

    [Fact]
    public void NextBookingId_NonNumericLastSegment_TreatedAsZero()
    {
        // If a booking id doesn't end with a number, it parses as 0
        var bookings = new[] { MakeBooking("CLVN-B-abc") };
        // max(0) + 1 = 1 < seed 1002, but seed only applies to DefaultIfEmpty
        // The list is not empty, so max = 0, result = CLVN-B-1
        var result = BookingRules.NextBookingId(bookings);
        Assert.Equal("CLVN-B-1", result);
    }

    [Fact]
    public void NextBookingId_MixedValidAndNonNumericIds_UsesHighestNumeric()
    {
        var bookings = new[]
        {
            MakeBooking("CLVN-B-abc"),  // parses as 0
            MakeBooking("CLVN-B-1010"), // parses as 1010
        };
        var result = BookingRules.NextBookingId(bookings);
        Assert.Equal("CLVN-B-1011", result);
    }

    [Fact]
    public void NextBookingId_IdWithMultipleSegments_UsesLastSegment()
    {
        // Only the last segment after '-' is parsed
        var bookings = new[] { MakeBooking("CLVN-B-9999") };
        var result = BookingRules.NextBookingId(bookings);
        Assert.Equal("CLVN-B-10000", result);
    }

    [Fact]
    public void NextBookingId_ReturnsCorrectFormat()
    {
        var result = BookingRules.NextBookingId([MakeBooking("CLVN-B-1001")]);
        Assert.StartsWith("CLVN-B-", result);
    }
}
