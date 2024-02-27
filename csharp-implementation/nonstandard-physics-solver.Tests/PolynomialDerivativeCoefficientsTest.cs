namespace Polynomial.Tests;

public class PolynomialDerivativeCoefficientsTest
{
    [Fact]
    public void TestPolynomialDerivativeCoefficients()
    {
        var coeffs = new List<float> { 3, 2, 1 }; // Coefficients for the polynomial x^2 + 2x + 3

        var expected = new List<float> { 2, 2 }; // Coefficients for the derivative 2x + 2

        var actual = Polynomial.PolynomialDerivativeCoefficients(coeffs);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestWithHighDegreePolynomial()
    {
        var coeffs = new List<float> { 0, 2, 0, -4, 3 }; // Coefficients for the polynomial 3x^4 - 4x^3 + 2x
        var expected = new List<float> { 2, 0, -12, 12 }; // Derivative coefficients for 12x^3 - 12x^2 + 2

        var actual = Polynomial.PolynomialDerivativeCoefficients(coeffs);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestWithPolynomialIncludingZeroCoefficients()
    {
        var coeffs = new List<float> { 5, 0, 3, 0, 1 }; // Coefficients for the polynomial x^4 + 3x^2 + 5
        var expected = new List<float> { 0, 6, 0, 4 }; // Derivative coefficients for 4x^3 + 6x

        var actual = Polynomial.PolynomialDerivativeCoefficients(coeffs);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestWithPolynomialResultingInZeroPolynomial()
    {
        var coeffs = new List<float> { 7 }; // Coefficients for the constant polynomial 7
        var expected = new List<float> { }; // Derivative of a constant polynomial is a zero polynomial

        var actual = Polynomial.PolynomialDerivativeCoefficients(coeffs);

        Assert.True(expected.SequenceEqual(actual));
    }
}