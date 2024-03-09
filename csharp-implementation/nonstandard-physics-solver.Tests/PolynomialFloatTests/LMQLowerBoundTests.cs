namespace NonstandardPhysicsSolver.Polynomials.Tests;

using NonstandardPhysicsSolver.Polynomials;
using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;

public class LMQLowerBoundTests
{
    [Theory]
    // Borrowing from the LMQUpperBound tests. According to the theory,
    // let P_reversed(x) = x^n * P(1/x) = a_n + a_{n-1} x + ... + a_0 x^n
    // Then LMQLowerBound(P) = 1 / LMQUpperBound(P_reversed)
    // Equivalently, LMQLowerBound(P_reversed) = 1 / LMQUpperBound(P) by switching P and P_reversed
    [InlineData(new float[] { 1, 100, -100, -1 }, 1f / 2f, 1f)]
    // - 6x^3 + 11x^2 - 6x + 1
    // Smallest positive real root: 1/3 = 0.33333
    [InlineData(new float[] { 1, -6, 11, -6 }, 1f / 12f, 0.333333f)]
    // x^4  - 4x^3 - x^2 + 2x + 3
    // As a reference, the smallest positive real root is x≈1.1137
    [InlineData(new float[] { 3, 2, -1, -4, 1 }, 1f / 1.74716093f, 1.1137f)]
    public void LMQLowerBound_WithValidCoefficients_ReturnsCorrectBound(float[] coeffs, float expectedBound, float smallestPositiveRoot)
    {
        // Arrange
        var polynomial = new PolynomialFloat(coeffs);

        // Act
        var bound = polynomial.LMQPositiveLowerBound();

        // Assert
        Assert.True(expectedBound < smallestPositiveRoot, $"The lower bound {expectedBound} > {smallestPositiveRoot}, the smallest positive real root.");
        AssertExtensions.FloatsApproximatelyEqual(expectedBound, bound, 1e-4f);
    }
}