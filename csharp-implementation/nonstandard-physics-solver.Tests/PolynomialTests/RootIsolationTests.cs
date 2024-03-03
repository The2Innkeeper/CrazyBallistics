namespace NonstandardPhysicsSolver.Polynomials.Tests.PolynomialTests;

using NonstandardPhysicsSolver.Intervals;
using NonstandardPhysicsSolver.Polynomials;
using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;
using Xunit;

public class RootIsolationTests
{
    [Fact]
    public void TestRootContainedInInterval()
    {
        // Define a simple polynomial with a root at x = 3
        Polynomial poly = new Polynomial(new float[] { -3, 1 }); // Represents x - 3

        // Create an interval that includes the root, e.g., [2, 4]
        Interval interval = new Interval(2, 4);

        // Known root of the polynomial
        float knownRoot = 3;

        // Check if the interval correctly identifies that it contains the root
        bool containsRoot = interval.ContainsValue(knownRoot);

        // Assert that containsRoot is true
        Assert.True(containsRoot, $"Interval {interval} does not contain the known root: {knownRoot}.");
    }

    [Fact]
    public void TestNoRoots()
    {
        // Polynomial with no real roots, e.g., x^2 + 1
        var poly = new Polynomial([1, 0, 1]);
        var intervals = poly.IsolateRootIntervalsContinuedFractions();
        Assert.Empty(intervals); // Expect no intervals for a polynomial with no real roots
    }

    [Fact]
    public void TestSingleRoot()
    {
        // Polynomial with a single root, e.g., (x - 1)^2 = x^2 - 2x + 1
        var poly = new Polynomial([1, -2, 1]);
        var intervals = poly.IsolateRootIntervalsContinuedFractions();
        AssertExtensions.AssertIntervalsContainRoots(intervals, [1]);
    }

    [Fact]
    public void TestMultipleRoots()
    {
        // Polynomial with multiple distinct roots, e.g., x^3 - 6x^2 + 11x - 6 = (x-1)(x-2)(x-3)
        var poly = new Polynomial([-6, 11, -6, 1]);
        var intervals = poly.IsolateRootIntervalsContinuedFractions();
        AssertExtensions.AssertIntervalsContainRoots(intervals, [1, 2, 3]);
    }

    [Fact]
    public void TestRootAtZero()
    {
        // Polynomial with a root at 0, e.g., x^2
        var poly = new Polynomial([0, 0, 1]);
        var intervals = poly.IsolateRootIntervalsContinuedFractions();
        Assert.Single(intervals); // Expect one interval for a polynomial with a root at 0
        // Check that the interval correctly identifies 0 as a root
        AssertExtensions.AssertIntervalsContainRoots(intervals, [0]);
    }

    [Fact]
    public void TestCloseRoots()
    {
        // Polynomial with roots very close to each other, requiring accurate interval isolation
        // Example: (x - 0.001)(x - 0.002) = x^2 - 0.003x + 0.000002
        var poly = new Polynomial([0.000002f, -0.003f, 1]);
        var intervals = poly.IsolateRootIntervalsContinuedFractions();
        // Transform each interval into a string representation
        var intervalsStr = intervals.Select(interval => $"[{interval.LeftBound}, {interval.RightBound}]").ToArray();

        // Join the string representations with ", " as separator
        string intervalsJoined = string.Join(", ", intervalsStr);

        List<float> expectedRoots = [0.001f, 0.002f];
        AssertExtensions.AssertIntervalsContainRoots(intervals, expectedRoots);
    }
}
