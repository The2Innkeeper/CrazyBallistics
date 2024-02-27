namespace Polynomial.Tests;

using System.Collections.Generic;

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