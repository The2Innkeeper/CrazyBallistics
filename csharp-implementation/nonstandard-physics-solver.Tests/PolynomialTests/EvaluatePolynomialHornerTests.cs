namespace NonstandardPhysicsSolver.Polynomials.Tests;

public class EvaluatePolynomialHornerTests
{
    [Fact]
    public void TestEvaluatePolynomialHorner()
    {
        var polynomial = new Polynomial([3, 2, 1]); // Coefficients for the polynomial x^2 + 2x + 3
        float x = 2;
        float expected = 11; // 2^2 + 2*2 + 3 = 11

        float actual = polynomial.EvaluatePolynomialHorner(x);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TestWithLargeXValue()
    {
        var polynomial = new Polynomial([1, 3, -2, 1]); // Coefficients for the polynomial x^3 - 2x^2 + 3x + 1
        float x = 1000;
        float expected = 1 + 3 * x - 2 * (float)Math.Pow(x, 2) + (float)Math.Pow(x, 3);
        float actual = polynomial.EvaluatePolynomialHorner(x);
        Assert.Equal(expected, actual, 1e6f);
    }

    [Fact]
    public void TestWithAllNegativeCoefficients()
    {
        var polynomial = new Polynomial([-4, -3, -2, -1]); // Coefficients for the polynomial -x^3 - 2x^2 - 3x - 4
        float x = 2;
        float expected = -4 - 3 * 2 - 2 * (float)Math.Pow(2, 2) - (float)Math.Pow(2, 3);
        float actual = polynomial.EvaluatePolynomialHorner(x);
        Assert.Equal(expected, actual, 0.05f);
    }

    [Fact]
    public void TestWithConstantPolynomial()
    {
        var polynomial = new Polynomial([5]); // Coefficients for the polynomial 5
        float x = 10; // Any x value should return the constant value
        float expected = 5;
        float actual = polynomial.EvaluatePolynomialHorner(x);
        Assert.Equal(expected, actual);
    }
}