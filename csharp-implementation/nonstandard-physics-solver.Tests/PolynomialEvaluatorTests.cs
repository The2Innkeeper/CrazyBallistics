using nonstandard_physics_solver;
namespace nonstandard_physics_solver.Tests;

using Xunit;
using System.Collections.Generic;

public class PolynomialTests
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
    public void TestEvaluatePolynomialHorner()
    {
        var coefficients = new float[] { 3, 2, 1 }; // Coefficients for the polynomial x^2 + 2x + 3
        float x = 2;
        float expected = 11; // 2^2 + 2*2 + 3 = 11

        float actual = Polynomial.EvaluatePolynomialHorner(x, coefficients);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestMakeSquareFree()
    {
        var polyCoeffs = new List<float> { 1, 0, 1 }; // Coefficients for the polynomial x^2 + 1, which is already square-free

        var expected = new List<float> { 1, 0, 1 };

        var actual = Polynomial.MakeSquareFree(polyCoeffs);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestPolynomialDerivativeCoefficients()
    {
        var coeffs = new List<float> { 3, 2, 1 }; // Coefficients for the polynomial x^2 + 2x + 3

        var expected = new List<float> { 2, 2 }; // Coefficients for the derivative 2x + 2

        var actual = Polynomial.PolynomialDerivativeCoefficients(coeffs);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestPolynomialDivision()
    {
        var dividend = new List<float> { 1, 0, 0 }; // Coefficients for the polynomial x^2
        var divisor = new List<float> { 1, 1 }; // Coefficients for the polynomial x + 1

        var expectedQuotient = new List<float> { 1, -1 }; // Coefficients for the quotient x - 1
        var expectedRemainder = new List<float> { 1 }; // Coefficients for the remainder 1

        var (actualQuotient, actualRemainder) = Polynomial.PolynomialDivision(dividend, divisor);

        Assert.Equal(expectedQuotient, actualQuotient);
        Assert.Equal(expectedRemainder, actualRemainder);
    }

    [Fact]
    public void TestGcdPolynomials()
    {
        var a = new List<float> { 1, 0, 0 }; // Coefficients for the polynomial x^2
        var b = new List<float> { 1, 1 }; // Coefficients for the polynomial x + 1

        var expected = new List<float> { 1 }; // Coefficients for the GCD 1

        var actual = Polynomial.GcdPolynomials(a, b);

        Assert.Equal(expected, actual);
    }
}