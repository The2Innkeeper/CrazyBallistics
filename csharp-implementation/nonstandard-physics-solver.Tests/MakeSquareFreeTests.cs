using Utils;

namespace Polynomial.Tests;

public class MakeSquareFreeTests
{
    [Fact]
    public void TestMakeSquareFree()
    {
        var polyCoeffs = new List<float> { 1, 0, 1 }; // Coefficients for the polynomial x^2 + 1, which is already square-free

        var expected = new List<float> { 1, 0, 1 };

        var actual = Polynomial.MakeSquareFree(polyCoeffs);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestWithNonSquareFreePolynomial()
    {
        var polyCoeffs = new List<float> { 4, 0, -4, 0, 1 }; // Coefficients for the polynomial x^4 - 4x^2 + 4
        var expected = new List<float> { -2, 0, 1 }; // Expected square-free version: x^2 - 2

        var actual = Polynomial.MakeSquareFree(polyCoeffs);

        var actualNormalized = actual.Select(c => c / actual[^1]).ToList();
        var expectedNormalized = expected.Select(c => c / expected[^1]).ToList();

        AssertExtensions.AssertEqualWithRelativeError(expectedNormalized, actualNormalized);
    }

    [Fact]
    public void TestWithHighDegreePolynomialWithMultipleSquareFactors()
    {
        var polyCoeffs = new List<float> { 1, -2, 1, 0, 0, 0, 1 }; // Coefficients for the polynomial x^6 + (x-1)^2
        var expected = new List<float> { 1, -2, 1, 0, 0, 0, 1 }; // The polynomial is already square-free as is

        var actual = Polynomial.MakeSquareFree(polyCoeffs);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestWithPolynomialBecomingConstantAfterSquareFree()
    {
        var polyCoeffs = new List<float> { 0, 0, 1 }; // Coefficients for the polynomial x^2
        var expected = new List<float> { 0, 1 }; // Square-free version is x

        var actual = Polynomial.MakeSquareFree(polyCoeffs);

        var actualNormalized = actual.Select(c => c / actual[^1]).ToList();
        var expectedNormalized = expected.Select(c => c / expected[^1]).ToList();

        AssertExtensions.AssertEqualWithRelativeError(expectedNormalized, actualNormalized);
    }
}
