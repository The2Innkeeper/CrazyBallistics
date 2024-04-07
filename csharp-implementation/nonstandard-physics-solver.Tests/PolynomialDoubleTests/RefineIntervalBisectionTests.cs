namespace NonstandardPhysicsSolver.Tests.PolynomialDoubleTests;

using NonstandardPhysicsSolver.Intervals;
using NonstandardPhysicsSolver.Tests.TestUtils.TestUtilsDouble;
using System;

public class PolynomialBisectionTests
{
    [Theory]
    [InlineData(-0.9f, 0.9f, 0f)] // Simple root at 0 for x^3 - x
    [InlineData(-2f, -0.01f, -1f)] // Negative root for x^3 - x, demonstrating tolerance
    [InlineData(0.1f, 2f, 1f)] // Positive root for x^3 - x, demonstrating tolerance
    [InlineData(0f, 0f, 0f)] // Equal bounds
    public void RefineIntervalBisection_WithRootInInterval_FindsRoot(double leftBound, double rightBound, double expectedRoot)
    {
        // Arrange
        var coeffs = new double[] { 0, -1, 0, 1 }; // Coefficients for x^3 - x
        var polynomial = new PolynomialDouble(coeffs);
        double tolerance = 0.0001f;

        // Act
        double? foundRoot = IntervalDouble.RefineRootIntervalBisection(polynomial.EvaluatePolynomialAccurate, leftBound, rightBound, tolerance);

        // Assert
        Assert.NotNull(foundRoot);
        double actualRoot = (double)foundRoot;
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedRoot, actualRoot, tolerance);
    }

    [Fact]
    public void RefineIntervalBisection_WithNoRootInInterval_ThrowsArgumentException()
    {
        // Arrange
        var coeffs = new double[] { 1, 0, 0, 1 }; // Coefficients for x^3 + 1, no real root between -0.5 and 1 (real root at x=-1
        var polynomial = new PolynomialDouble(coeffs);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => IntervalDouble.RefineRootIntervalBisection(polynomial.EvaluatePolynomialAccurate, -0.5f, 1));
    }

    [Fact]
    public void RefineIntervalBisection_MaxIterationsReached_ReturnsNull()
    {
        // Arrange: Use a polynomial and interval known to contain a root, e.g., x^2 - 1 with roots at x = -1, x = 1
        var coeffs = new double[] { -1, 0, 1 }; // Coefficients for x^2 - 1
        var polynomial = new PolynomialDouble(coeffs);
        int maxIterations = 3; // Low number to force termination before convergence

        // Act
        double result = IntervalDouble.RefineRootIntervalBisection(polynomial.EvaluatePolynomialAccurate, -0.5f, 2, 1e-5f, maxIterations);

        // Assert
        Assert.True(double.IsNaN(result), $"Invalid interval did not return double.NaN"); // Expect null due to max iterations without converging
    }
}