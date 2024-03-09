namespace NonstandardPhysicsSolver.Polynomials.Tests;

using NonstandardPhysicsSolver.Intervals;
using TestUtils;
using Xunit;

public class PolynomialToleranceTests
{
    [Theory]
    // x^2 + 4x + 3 has roots x=1, x=3
    [InlineData(0.01f, 1.0)] // Higher tolerance
    [InlineData(0.001f, 1.0)] // Medium tolerance
    [InlineData(0.0001f, 1.0)] // Lower tolerance
    public void RefineIntervalITP_VariousTolerances_ReturnsRootWithinTolerance(float tol, float expectedRoot)
    {
        PolynomialFloat polynomial = new([3, -4, 1]);
        float leftBound = 0;
        float rightBound = 2; // Interval containing the root at x=1

        float actualRoot = Interval.RefineRootIntervalITP(polynomial.EvaluatePolynomialAccurate, leftBound, rightBound, tol);

        // Assert that the actual root is within tolerance of the expected root
        string failureMessage = $"Expected root close to {expectedRoot} with tolerance {tol}, but got {actualRoot}";
        AssertExtensions.FloatsApproximatelyEqual(expectedRoot, actualRoot, tol);
    }
}