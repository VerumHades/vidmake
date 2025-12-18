using System;
using Vidmake.src.positioning;
using Xunit;

public class IntervalExtremeTests
{
    #region Constructor Tests

    [Fact]
    public void Constructor_ValidIntegers_CreatesInterval()
    {
        var interval = new Interval<int>(5, 10);
        Assert.Equal(5, interval.Start);
        Assert.Equal(10, interval.End);
    }

    [Fact]
    public void Constructor_StartEqualsEnd_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Interval<int>(5, 5));
    }

    [Fact]
    public void Constructor_StartGreaterThanEnd_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Interval<int>(10, 5));
    }

    [Fact]
    public void Constructor_ValidDoubles_CreatesInterval()
    {
        var interval = new Interval<double>(0.1, 0.2);
        Assert.Equal(0.1, interval.Start);
        Assert.Equal(0.2, interval.End);
    }

    [Fact]
    public void Constructor_ExtremeLongValues_CreatesInterval()
    {
        var interval = new Interval<long>(long.MinValue, long.MaxValue);
        Assert.Equal(long.MinValue, interval.Start);
        Assert.Equal(long.MaxValue, interval.End);
    }

    #endregion

    #region Start/End Setter Tests

    [Fact]
    public void Start_SetValidValue_UpdatesStart()
    {
        var interval = new Interval<int>(5, 10);
        interval.Start = 6;
        Assert.Equal(6, interval.Start);
    }

    [Fact]
    public void Start_SetInvalidValue_Throws()
    {
        var interval = new Interval<int>(5, 10);
        Assert.Throws<ArgumentException>(() => interval.Start = 10);
        Assert.Throws<ArgumentException>(() => interval.Start = 15);
    }

    [Fact]
    public void End_SetValidValue_UpdatesEnd()
    {
        var interval = new Interval<int>(5, 10);
        interval.End = 15;
        Assert.Equal(15, interval.End);
    }

    [Fact]
    public void End_SetInvalidValue_Throws()
    {
        var interval = new Interval<int>(5, 10);
        Assert.Throws<ArgumentException>(() => interval.End = 5);
        Assert.Throws<ArgumentException>(() => interval.End = 0);
    }

    [Fact]
    public void StartAndEnd_ExtremeValues_Int()
    {
        var interval = new Interval<int>(int.MinValue, int.MaxValue);
        Assert.Equal(int.MinValue, interval.Start);
        Assert.Equal(int.MaxValue, interval.End);

        // changing start to max-1 is valid
        interval.Start = int.MaxValue - 1;
        Assert.Equal(int.MaxValue - 1, interval.Start);

        // changing end to min+1 throws
        Assert.Throws<ArgumentException>(() => interval.End = int.MinValue + 1);
    }

    [Fact]
    public void StartAndEnd_ExtremeValues_Double()
    {
        var interval = new Interval<double>(double.MinValue, double.MaxValue);
        Assert.Equal(double.MinValue, interval.Start);
        Assert.Equal(double.MaxValue, interval.End);

        interval.Start = -1e308;
        Assert.Equal(-1e308, interval.Start);

        interval.End = 1e308;
        Assert.Equal(1e308, interval.End);
    }

    #endregion

    #region Contains Tests

    [Fact]
    public void Contains_ValueInside_ReturnsTrue()
    {
        var interval = new Interval<int>(5, 10);
        Assert.True(interval.Contains(6));
        Assert.True(interval.Contains(9));
    }

    [Fact]
    public void Contains_ValueOnStartOrEnd_ReturnsFalse()
    {
        var interval = new Interval<int>(5, 10);
        Assert.False(interval.Contains(5));
        Assert.False(interval.Contains(10));
    }

    [Fact]
    public void Contains_ValueOutside_ReturnsFalse()
    {
        var interval = new Interval<int>(5, 10);
        Assert.False(interval.Contains(0));
        Assert.False(interval.Contains(11));
    }

    [Fact]
    public void Contains_ExtremeValues_Long()
    {
        var interval = new Interval<long>(long.MinValue, long.MaxValue);
        Assert.True(interval.Contains(0));
        Assert.False(interval.Contains(long.MinValue));
        Assert.False(interval.Contains(long.MaxValue));
    }

    [Fact]
    public void Contains_ExtremeValues_Double()
    {
        var interval = new Interval<double>(-1e308, 1e308);
        Assert.True(interval.Contains(0.0));
        Assert.False(interval.Contains(-1e308));
        Assert.False(interval.Contains(1e308));
    }

    #endregion

    #region ToString Tests

    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        var interval = new Interval<int>(5, 10);
        Assert.Equal("(5, 10)", interval.ToString());

        var intervalDouble = new Interval<double>(0.1, 0.2);
        Assert.Equal("(0,1, 0,2)", intervalDouble.ToString());
    }

    #endregion
}
