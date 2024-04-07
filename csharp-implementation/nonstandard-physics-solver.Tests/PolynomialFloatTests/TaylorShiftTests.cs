namespace NonstandardPhysicsSolver.Tests.PolynomialFloatTests;

using NonstandardPhysicsSolver.Polynomials;
using NonstandardPhysicsSolver.Tests.TestUtils;
using Xunit;

public class TaylorShiftTests
{
    [Theory]
    [InlineData(new float[] { 1 }, 0, new float[] { 1 })] // Test shifting by 0
    [InlineData(new float[] { 1, 2, 1 }, 1, new float[] { 4, 4, 1 })] // (x^2 + 2x + 1) shifted by +1 = x^2 + 4x + 4
    [InlineData(new float[] { 0, 1 }, -1, new float[] { -1, 1 })] // x shifted by -1
    [InlineData(new float[] { 2, 3, 5, 11 }, 7, new float[] { 4041, 1690, 236, 11 })]
    public void TaylorShiftQuadratic_ShiftsCorrectly(float[] coefficients, float shift, float[] expected)
    {
        // Arrange
        var polynomial = new PolynomialFloat(coefficients);

        // Act
        var result = polynomial.TaylorShift(shift);

        // Assert
        Assert.Equal(expected, result.Coefficients);
    }

    [Fact]
    public void TaylorShiftQuadratic_WithLargeShift_ChecksPrecision()
    {
        // Large shifts might introduce precision issues, test accordingly
        var coefficients = new float[] { -1, 3, -3, 1 }; // (x-1)^3
        var shift = 10f; // (x-1+10)^3 = x^3 + 27x^2 + 243x + 729
        var expected = new float[] { 729, 243, 27, 1 }; // Expected might need adjustment based on actual shift impact
        var polynomial = new PolynomialFloat(coefficients);

        var result = polynomial.TaylorShift(shift);

        // This assertion might need adjustment based on the precision of the floating-point operations
        AssertExtensions.ArraysEqual(expected, result.Coefficients);
    }

    // Additional tests can be added here to cover more cases, such as:
    // - Testing with larger polynomials
    // - Testing with negative coefficients
    // - Testing the caching mechanism of the Binomial method if it's exposed or indirectly through performance measurements
}
