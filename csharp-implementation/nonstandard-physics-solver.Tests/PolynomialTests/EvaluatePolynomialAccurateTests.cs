namespace NonstandardPhysicsSolver.Polynomials.Tests;

public class EvaluatePolynomialAccurateTests
{
    [Fact]
    public void TestEvaluatePolynomialAccurate()
    {
        var polynomial = new Polynomial([1, 2, 3]); // x^2 + 2x + 3
        float x = 2;
        float expected = 17; // 1 + 2*2 + 3*2^2 = 17

        float actual = polynomial.EvaluatePolynomialAccurate(x);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestHighDegreePolynomial()
    {
        var polynomial = new Polynomial([0.5f, -1, 0, 2, -0.5f]); // 0.5 - x + 0x^2 + 2x^3 - 0.5x^4
        float x = 1.5f;
        float expected = 0.5f - 1 * 1.5f + 0 * (float)Math.Pow(1.5, 2) + 2 * (float)Math.Pow(1.5, 3) - 0.5f * (float)Math.Pow(1.5, 4);

        float actual = polynomial.EvaluatePolynomialAccurate(x);

        Assert.Equal(expected, actual, precision: 2);
    }

    [Fact]
    public void TestWithZeroCoefficients()
    {
        var polynomial = new Polynomial([2, 0, 4, 0, 5]); // 2 + 0x + 4x^2 + 0x^3 + 5x^4
        float x = 3;
        float expected = 2 + 0 * 3 + 4 * (float)Math.Pow(3, 2) + 0 * (float)Math.Pow(3, 3) + 5 * (float)Math.Pow(3, 4);

        float actual = polynomial.EvaluatePolynomialAccurate(x);

        Assert.Equal(expected, actual, precision: 2);
    }

    [Fact]
    public void TestWithNegativeXValues()
    {
        var polynomial = new Polynomial([1, -2, 3]); // 1 - 2x + 3x^2
        float x = -2;
        float expected = 1 - 2 * -2 + 3 * (float)Math.Pow(-2, 2);

        float actual = polynomial.EvaluatePolynomialAccurate(x);

        Assert.Equal(expected, actual, precision: 2);
    }
}
