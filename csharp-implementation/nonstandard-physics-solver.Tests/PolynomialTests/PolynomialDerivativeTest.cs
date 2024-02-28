namespace NonstandardPhysicsSolver.Polynomials.Tests;

public class PolynomialDerivativeTest
{
    private static void AssertPolynomialDerivative(List<float> originalCoefficients, IEnumerable<float> expectedDerivativeCoefficients)
    {
        // Arrange
        var polynomial = new Polynomial(originalCoefficients);
        var expected = expectedDerivativeCoefficients;

        // Act
        var derivative = polynomial.PolynomialDerivative();
        var actual = derivative.Coefficients;

        // Assert
        Assert.True(expected.SequenceEqual(actual), "The actual derivative coefficients do not match the expected coefficients.");
    }

    [Fact]
    public void TestPolynomialDerivativeCoefficients()
    {
        AssertPolynomialDerivative([3f, 2f, 1f], [2f, 2f]);
    }

    [Fact]
    public void TestWithHighDegreePolynomial()
    {
        AssertPolynomialDerivative([0f, 2f, 0f, -4f, 3f], [2f, 0f, -12f, 12f]);
    }

    [Fact]
    public void TestWithPolynomialIncludingZeroCoefficients()
    {
        AssertPolynomialDerivative([5f, 0f, 3f, 0f, 1f], [0f, 6f, 0f, 4f]);
    }

    [Fact]
    public void TestWithPolynomialResultingInZeroPolynomial()
    {
        AssertPolynomialDerivative([7f], [0f]);
    }
}
