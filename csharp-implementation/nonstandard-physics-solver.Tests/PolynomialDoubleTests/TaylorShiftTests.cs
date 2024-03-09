namespace NonstandardPhysicsSolver.Polynomials.Tests.PolynomialDoubleTests;

using NonstandardPhysicsSolver.Polynomials;
using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;
using Xunit;

public class TaylorShiftTests
{
    [Theory]
    [InlineData(new double[] { 1 }, 0, new double[] { 1 })] // Test shifting by 0
    [InlineData(new double[] { 1, 2, 1 }, 1, new double[] { 4, 4, 1 })] // (x^2 + 2x + 1) shifted by +1 = x^2 + 4x + 4
    [InlineData(new double[] { 0, 1 }, -1, new double[] { -1, 1 })] // x shifted by -1
    [InlineData(new double[] { 2, 3, 5, 11 }, 7, new double[] { 4041, 1690, 236, 11 })]
    public void TaylorShiftQuadratic_ShiftsCorrectly(double[] coefficients, double shift, double[] expected)
    {
        // Arrange
        var polynomial = new PolynomialDouble(coefficients);

        // Act
        var result = polynomial.TaylorShift(shift);

        // Assert
        Assert.Equal(expected, result.Coefficients);
    }

    [Fact]
    public void TaylorShiftQuadratic_WithLargeShift_ChecksPrecision()
    {
        // Large shifts might introduce precision issues, test accordingly
        double[] coefficients = [-1, 3, -3, 1]; // (x-1)^3
        double shift = 10.0; // (x-1+10)^3 = x^3 + 27x^2 + 243x + 729
        double[] expected = [729, 243, 27, 1]; // Expected might need adjustment based on actual shift impact
        var polynomial = new PolynomialDouble(coefficients);

        var result = polynomial.TaylorShift(shift);

        // This assertion might need adjustment based on the precision of the floating-point operations
        AssertExtensionsDouble.ArraysEqual(expected, result.Coefficients);
    }

    // Additional tests can be added here to cover more cases, such as:
    // - Testing with larger polynomials
    // - Testing with negative coefficients
    // - Testing the caching mechanism of the Binomial method if it's exposed or indirectly through performance measurements
}
