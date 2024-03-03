namespace NonstandardPhysicsSolver.Polynomials.Tests.PolynomialTests;

using NonstandardPhysicsSolver.Intervals;
using System;
using Xunit;

public class MobiusTransformationTests
{
    //[Fact]
    //public void Constructor_ValidParameters_ShouldNotThrow()
    //{
    //    static MobiusTransformation action() => new(1, 2, 3, 4);
    //    Assert.Null(Record.Exception((Func<MobiusTransformation>)action));
    //}

    [Fact]
    public void Constructor_InvalidParameters_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new MobiusTransformation(1, 2, 2, 4));
    }

    [Fact]
    public void ProcessUnitInterval_CorrectTransformation()
    {
        var transformation = new MobiusTransformation(2, 3, 5, 11).ProcessUnitInterval();
        Assert.Equal(new MobiusTransformation(3, 5, 11, 16), transformation);
    }

    [Fact]
    public void TransformedForLowerInterval_CorrectTransformation()
    {
        var transformation = new MobiusTransformation(2, 3, 5, 11).TransformedForLowerInterval(7);
        Assert.Equal(new MobiusTransformation(3, 17, 11, 46), transformation);
    }

    [Fact]
    public void PositiveDomainImage_CorrectInterval()
    {
        var transformation = new MobiusTransformation(1, 2, 3, 4);
        var interval = transformation.PositiveDomainImage();
        Assert.True(interval.LeftBound == 1f / 3f && interval.RightBound == 2f / 4f);
    }

    [Fact]
    public void TaylorShift_CorrectTransformation()
    {
        var transformation = new MobiusTransformation(2, 3, 7, 13).TaylorShift(5);
        Assert.Equal(new MobiusTransformation(2, 13, 7, 48), transformation);
    }

    [Fact]
    public void Inverted_CorrectInversion()
    {
        var transformation = new MobiusTransformation(1, 2, 3, 4).ReciprocalInput();
        Assert.Equal(new MobiusTransformation(2, 1, 4, 3), transformation);
    }

    [Fact]
    public void Scale_CorrectScaling()
    {
        var transformation = new MobiusTransformation(1, 2, 3, 4).Scale(2);
        Assert.Equal(new MobiusTransformation(2, 2, 6, 4), transformation);
    }
}
