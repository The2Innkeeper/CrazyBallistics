namespace Polynomial.Tests;

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

