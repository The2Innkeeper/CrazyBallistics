namespace NonstandardPhysicsSolver.Polynomials.Tests;

using NonstandardPhysicsSolver.Intervals;
using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;
using System;

public class PolynomialBisectionTests
{
    [Theory]
    [InlineData(-0.9f, 0.9f, 0f)] // Simple root at 0 for x^3 - x
    [InlineData(-2f, -0.01f, -1f)] // Negative root for x^3 - x, demonstrating tolerance
    [InlineData(0.1f, 2f, 1f)] // Positive root for x^3 - x, demonstrating tolerance
    [InlineData(0f, 0f, 0f)] // Equal bounds
    public void RefineIntervalBisection_WithRootInInterval_FindsRoot(float leftBound, float rightBound, float expectedRoot)
    {
        // Arrange
        var coeffs = new float[] { 0, -1, 0, 1 }; // Coefficients for x^3 - x
        var polynomial = new Polynomial(coeffs);
        float tolerance = 0.0001f;

        // Act
        float? foundRoot = Interval.RefineRootIntervalBisection(polynomial.EvaluatePolynomialAccurate, leftBound, rightBound, tolerance);

        // Assert
        Assert.NotNull(foundRoot);
        float actualRoot = (float)foundRoot;
        AssertExtensions.FloatsApproximatelyEqual(expectedRoot, actualRoot, tolerance);
    }

    [Fact]
    public void RefineIntervalBisection_WithNoRootInInterval_ThrowsArgumentException()
    {
        // Arrange
        var coeffs = new float[] { 1, 0, 0, 1 }; // Coefficients for x^3 + 1, no real root between -0.5 and 1 (real root at x=-1
        var polynomial = new Polynomial(coeffs);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => Interval.RefineRootIntervalBisection(polynomial.EvaluatePolynomialAccurate, -0.5f, 1));
    }

    [Fact]
    public void RefineIntervalBisection_MaxIterationsReached_ReturnsNull()
    {
        // Arrange: Use a polynomial and interval known to contain a root, e.g., x^2 - 1 with roots at x = -1, x = 1
        var coeffs = new float[] { -1, 0, 1 }; // Coefficients for x^2 - 1
        var polynomial = new Polynomial(coeffs);
        int maxIterations = 3; // Low number to force termination before convergence

        // Act
        float result = Interval.RefineRootIntervalBisection(polynomial.EvaluatePolynomialAccurate, -0.5f, 2, 1e-5f, maxIterations);

        // Assert
        Assert.True(float.IsNaN(result), $"Invalid interval did not return float.NaN"); // Expect null due to max iterations without converging
    }
}