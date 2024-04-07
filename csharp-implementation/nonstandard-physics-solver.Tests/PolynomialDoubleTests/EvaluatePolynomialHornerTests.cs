namespace NonstandardPhysicsSolver.Tests.PolynomialDoubleTests;

public class EvaluatePolynomialHornerTests
{
    [Fact]
    public void TestEvaluatePolynomialHorner()
    {
        var polynomial = new PolynomialDouble([3, 2, 1]); // Coefficients for the polynomial x^2 + 2x + 3
        double x = 2;
        double expected = 11; // 2^2 + 2*2 + 3 = 11

        double actual = polynomial.EvaluatePolynomialHorner(x);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestWithLargeXValue()
    {
        var polynomial = new PolynomialDouble([1, 3, -2, 1]); // Coefficients for the polynomial x^3 - 2x^2 + 3x + 1
        double x = 1000;
        double expected = 1 + 3 * x - 2 * (double)Math.Pow(x, 2) + (double)Math.Pow(x, 3);
        double actual = polynomial.EvaluatePolynomialHorner(x);
        Assert.Equal(expected, actual, 1e6f);
    }

    [Fact]
    public void TestWithAllNegativeCoefficients()
    {
        var polynomial = new PolynomialDouble([-4, -3, -2, -1]); // Coefficients for the polynomial -x^3 - 2x^2 - 3x - 4
        double x = 2;
        double expected = -4 - 3 * 2 - 2 * (double)Math.Pow(2, 2) - (double)Math.Pow(2, 3);
        double actual = polynomial.EvaluatePolynomialHorner(x);
        Assert.Equal(expected, actual, 0.05f);
    }

    [Fact]
    public void TestWithConstantPolynomial()
    {
        var polynomial = new PolynomialDouble([5]); // Coefficients for the polynomial 5
        double x = 10; // Any x value should return the constant value
        double expected = 5;
        double actual = polynomial.EvaluatePolynomialHorner(x);
        Assert.Equal(expected, actual);
    }
}