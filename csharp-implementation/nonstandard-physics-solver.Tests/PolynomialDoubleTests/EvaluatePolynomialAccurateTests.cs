namespace NonstandardPhysicsSolver.Polynomials.Tests.PolynomialDoubleTests;

public class EvaluatePolynomialAccurateTests
{
    [Fact]
    public void TestEvaluatePolynomialAccurate()
    {
        var polynomial = new PolynomialDouble([1, 2, 3]); // x^2 + 2x + 3
        double x = 2;
        double expected = 17; // 1 + 2*2 + 3*2^2 = 17

        double actual = polynomial.EvaluatePolynomialAccurate(x);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestHighDegreePolynomial()
    {
        var polynomial = new PolynomialDouble([0.5f, -1, 0, 2, -0.5f]); // 0.5 - x + 0x^2 + 2x^3 - 0.5x^4
        double x = 1.5f;
        double expected = 0.5f - 1 * 1.5f + 0 * (double)Math.Pow(1.5, 2) + 2 * (double)Math.Pow(1.5, 3) - 0.5f * (double)Math.Pow(1.5, 4);

        double actual = polynomial.EvaluatePolynomialAccurate(x);

        Assert.Equal(expected, actual, precision: 2);
    }

    [Fact]
    public void TestWithZeroCoefficients()
    {
        var polynomial = new PolynomialDouble([2, 0, 4, 0, 5]); // 2 + 0x + 4x^2 + 0x^3 + 5x^4
        double x = 3;
        double expected = 2 + 0 * 3 + 4 * (double)Math.Pow(3, 2) + 0 * (double)Math.Pow(3, 3) + 5 * (double)Math.Pow(3, 4);

        double actual = polynomial.EvaluatePolynomialAccurate(x);

        Assert.Equal(expected, actual, precision: 2);
    }

    [Fact]
    public void TestWithNegativeXValues()
    {
        var polynomial = new PolynomialDouble([1, -2, 3]); // 1 - 2x + 3x^2
        double x = -2;
        double expected = 1 - 2 * -2 + 3 * (double)Math.Pow(-2, 2);

        double actual = polynomial.EvaluatePolynomialAccurate(x);

        Assert.Equal(expected, actual, precision: 2);
    }
}
