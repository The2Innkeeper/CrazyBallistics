namespace Polynomial.Tests;

using System.Collections.Generic;
using Utils;
using Xunit;

public class EvaluatePolynomialAccurateTests
{
    [Fact]
    public void TestEvaluatePolynomialAccurate()
    {
        var coefficients = new List<float> { 1, 2, 3 }; // Coefficients for the polynomial x^2 + 2x + 3
        float x = 2;
        float expected = 17; // 1 + 2*2 + 3*2^2 = 17

        float actual = Polynomial.EvaluatePolynomialAccurate(coefficients, x);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestHighDegreePolynomial()
    {
        var coefficients = new List<float> { 0.5f, -1, 0, 2, -0.5f }; // Coefficients for the polynomial 0.5 - x + 0x^2 + 2x^3 - 0.5x^4
        float x = 1.5f;
        float expected = 0.5f - 1 * 1.5f + 0 * (float)Math.Pow(1.5, 2) + 2 * (float)Math.Pow(1.5, 3) - 0.5f * (float)Math.Pow(1.5, 4);
        float actual = Polynomial.EvaluatePolynomialAccurate(coefficients, x);
        Assert.Equal(expected, actual, 0.05f);
    }

    [Fact]
    public void TestWithZeroCoefficients()
    {
        var coefficients = new List<float> { 2, 0, 4, 0, 5 }; // Coefficients for the polynomial 2 + 0x + 4x^2 + 0x^3 + 5x^4
        float x = 3;
        float expected = 2 + 0 * 3 + 4 * (float)Math.Pow(3, 2) + 0 * (float)Math.Pow(3, 3) + 5 * (float)Math.Pow(3, 4);
        float actual = Polynomial.EvaluatePolynomialAccurate(coefficients, x);
        Assert.Equal(expected, actual, 0.05f);
    }

    [Fact]
    public void TestWithNegativeXValues()
    {
        var coefficients = new List<float> { 1, -2, 3 }; // Coefficients for the polynomial 1 - 2x + 3x^2
        float x = -2;
        float expected = 1 - 2 * (-2) + 3 * (float)Math.Pow(-2, 2);
        float actual = Polynomial.EvaluatePolynomialAccurate(coefficients, x);
        Assert.Equal(expected, actual, 0.05f);
    }
}

public class EvaluatePolynomialHornerTests
{
    [Fact]
    public void TestEvaluatePolynomialHorner()
    {
        var coefficients = new float[] { 3, 2, 1 }; // Coefficients for the polynomial x^2 + 2x + 3
        float x = 2;
        float expected = 11; // 2^2 + 2*2 + 3 = 11

        float actual = Polynomial.EvaluatePolynomialHorner(x, coefficients);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestWithVeryLargeXValue()
    {
        var coefficients = new float[] { 1, 3, -2, 1 }; // Coefficients for the polynomial x^3 - 2x^2 + 3x + 1
        float x = 1000;
        float expected = 1 + 3 * x - 2 * (float)Math.Pow(x, 2) + (float)Math.Pow(x, 3);
        float actual = Polynomial.EvaluatePolynomialHorner(x, coefficients);
        Assert.Equal(expected, actual, 1e6f);
    }

    [Fact]
    public void TestWithAllNegativeCoefficients()
    {
        var coefficients = new float[] { -4, -3, -2, -1 }; // Coefficients for the polynomial -x^3 - 2x^2 - 3x - 4
        float x = 2;
        float expected = -4 - 3 * 2 - 2 * (float)Math.Pow(2, 2) - (float)Math.Pow(2, 3);
        float actual = Polynomial.EvaluatePolynomialHorner(x, coefficients);
        Assert.Equal(expected, actual, 0.05f);
    }

    [Fact]
    public void TestWithConstantPolynomial()
    {
        var coefficients = new float[] { 5 }; // Coefficients for the polynomial 5
        float x = 10; // Any x value should return the constant value
        float expected = 5;
        float actual = Polynomial.EvaluatePolynomialHorner(x, coefficients);
        Assert.Equal(expected, actual);
    }
}

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

public class PolynomialDivisionTests
{
    /// <summary>
    /// Tests the division of two polynomials in a normal case.
    /// </summary>
    /// <remarks>
    /// (x^2 + 1) / x = x + 1/x
    /// </remarks>
    [Fact]
    public void TestPolynomialDivision_NormalCase()
    {
        // Arrange
        var dividend = new List<float> { 1, 0, 1 };
        var divisor = new List<float> { 0, 1 };
        var expectedQuotient = new List<float> { 0, 1 };
        var expectedRemainder = new List<float> { 1 };
        var expectedException = false;

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectedException);
    }

    /// <summary>
    /// Tests the scenario where the dividend is zero.
    /// </summary>
    /// <remarks>
    /// 0 / (x + 1) = 0
    /// </remarks>
    [Fact]
    public void TestPolynomialDivision_DividendIsZero()
    {
        // Arrange
        var dividend = new List<float> { 0 };
        var divisor = new List<float> { 1, 1 };
        var expectedQuotient = new List<float> { 0 };
        var expectedRemainder = new List<float> { 0 };
        var expectedException = false;

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectedException);
    }

    /// <summary>
    /// Tests the division of a polynomial by zero, expecting an exception.
    /// </summary>
    /// <remarks>
    /// x^2 / 0 => exception
    /// </remarks>
    [Fact]
    public void TestPolynomialDivision_DivisorIsZero()
    {
        // Arrange
        var dividend = new List<float> { 0, 0, 1 };
        var divisor = new List<float> { 0 };
        var expectedException = true;

        // Act & Assert
        if (expectedException)
        {
            Assert.Throws<DivideByZeroException>(() => Polynomial.PolynomialDivision(dividend, divisor));
        }
    }

    /// <summary>
    /// Tests the case where the dividend degree is less than the divisor degree.
    /// </summary>
    /// <remarks>
    /// x / x^2 = 1/x
    /// </remarks>
    [Fact]
    public void TestPolynomialDivision_DividendDegreeLessThanDivisor()
    {
        // Arrange
        var dividend = new List<float> { 0, 1 };
        var divisor = new List<float> { 0, 0, 1 };
        var expectedQuotient = new List<float> { 0 };
        var expectedRemainder = new List<float> { 0, 1 };
        var expectedException = false;

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectedException);
    }

    /// <summary>
    /// Tests the division of two identical polynomials.
    /// </summary>
    /// <remarks>
    /// (x + 1) / (x + 1) = 1
    /// </remarks>
    [Fact]
    public void TestPolynomialDivision_IdenticalPolynomials()
    {
        // Arrange
        var dividend = new List<float> { 1, 1 };
        var divisor = new List<float> { 1, 1 };
        var expectedQuotient = new List<float> { 1 };
        var expectedRemainder = new List<float> { 0 };
        var expectedException = false;

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectedException);
    }

    private static void RunTest(List<float> dividend, List<float> divisor, List<float> expectedQuotient, List<float> expectedRemainder, bool expectedException)
    {
        if (expectedException)
        {
            Assert.Throws<DivideByZeroException>(() => Polynomial.PolynomialDivision(dividend, divisor));
        }
        else
        {
            var (quotient, remainder) = Polynomial.PolynomialDivision(dividend, divisor);
            Assert.Equal(expectedQuotient, quotient);
            Assert.Equal(expectedRemainder, remainder);
        }
    }
}

public class GcdPolynomialsTest
{
    [Fact]
    public void TestGcdPolynomialsWithGCD1()
    {
        var a = new List<float> { 1, 0, 0 }; // Coefficients for the polynomial x^2
        var b = new List<float> { 1, 1 }; // Coefficients for the polynomial x + 1

        var expected = new List<float> { 1 }; // Coefficients for the GCD 1

        var actual = Polynomial.GcdPolynomials(a, b);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestGcdPolynomialsWithGCDNot1()
    {
        var a = new List<float> { -1, 0, 1 }; // Coefficients for the polynomial x^2 -1
        var b = new List<float> { 1, 1 }; // Coefficients for the polynomial x + 1

        var expected = new List<float> { 1, 1 }; // Coefficients for the GCD x + 1

        var actual = Polynomial.GcdPolynomials(a, b);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestGcdPolynomialsWithHigherDegree()
    {
        var a = new List<float> { 0, 0, 3, -6, 3 }; // Coefficients for the polynomial 3x^4 - 6x^3 + 3x^2
        var b = new List<float> { 0, 6, -12, 6 }; // Coefficients for the polynomial 6x^3 - 12x^2 + 6x

        // Expected GCD: x^3 - 2x^2 + x, with positive leading coefficient but ignored value for deterministic purposes
        var expected = new List<float> { 0, 1, -2, 1 }; // Coefficients for the GCD, normalized

        var actual = Polynomial.GcdPolynomials(a, b);
        actual = Polynomial.GcdPolynomials(actual, expected); // Don't care about leading coefficient

        Assert.Equal(expected, actual);
    }
}