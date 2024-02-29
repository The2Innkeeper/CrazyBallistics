namespace NonstandardPhysicsSolver.Polynomials.Tests;

using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;
using System;


public class PolynomialBisectionTests
{
    [Theory]
    [InlineData(-1, 1, 0)] // Simple root at 0 for x^3 - x
    [InlineData(-2, 0, -1)] // Negative root for x^3 - x, demonstrating tolerance
    [InlineData(0, 2, 1)] // Positive root for x^3 - x, demonstrating tolerance
    [InlineData(0, 0, 0)] // Equal bounds
    public void RefineIntervalBisection_WithRootInInterval_FindsRoot(float leftBound, float rightBound, float expectedRoot)
    {
        // Arrange
        var coeffs = new List<float> { 0, -1, 0, 1 }; // Coefficients for x^3 - x
        var polynomial = new Polynomial(coeffs);
        float tolerance = 0.0001f;

        // Act
        float? foundRoot = polynomial.RefineIntervalBisection(leftBound, rightBound, tolerance);

        // Assert
        Assert.NotNull(foundRoot);
        float actualRoot = (float)foundRoot;
        AssertExtensions.FloatsApproximatelyEqual(expectedRoot, actualRoot, tolerance);
    }

    [Fact]
    public void RefineIntervalBisection_WithNoRootInInterval_ThrowsArgumentException()
    {
        // Arrange
        var coeffs = new List<float> { 1, 0, 0, 1 }; // Coefficients for x^3 + 1, no real root between -0.5 and 1 (real root at x=-1
        var polynomial = new Polynomial(coeffs);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => polynomial.RefineIntervalBisection(-0.5f, 1));
    }

    [Fact]
    public void RefineIntervalBisection_MaxIterationsReached_ReturnsNull()
    {
        // Arrange: Use a polynomial and interval known to contain a root, e.g., x^2 - 1 with roots at x = -1, x = 1
        var coeffs = new List<float> { -1, 0, 1 }; // Coefficients for x^2 - 1
        var polynomial = new Polynomial(coeffs);
        int maxIterations = 3; // Low number to force termination before convergence

        // Act
        float? result = polynomial.RefineIntervalBisection(-0.5f, 2, 1e-5f, maxIterations);

        // Assert
        Assert.Null(result); // Expect null due to max iterations without converging
    }
}