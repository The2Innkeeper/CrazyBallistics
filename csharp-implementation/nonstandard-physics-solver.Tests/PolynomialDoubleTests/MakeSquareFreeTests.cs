namespace NonstandardPhysicsSolver.Tests.PolynomialDoubleTests;

using NonstandardPhysicsSolver.Tests.TestUtils.TestUtilsDouble;
using Xunit;

public class MakeSquareFreeTests
{
    private static void AssertSquareFreeTransformation(double[] originalCoefficients, double[] expectedCoefficients)
    {
        // Arrange
        var polynomial = new PolynomialDouble(originalCoefficients);

        // Act
        var squareFreePolynomial = polynomial.MakeSquarefree();
        var actualCoefficients = PolynomialUtils.NormalizedCoefficients(squareFreePolynomial);

        // Assert
        AssertExtensionsDouble.ArraysApproximatelyEqual(expectedCoefficients, actualCoefficients);
    }

    [Fact]
    public void TestMakeSquareFree()
    {
        AssertSquareFreeTransformation([1f, 0f, 1f], [1f, 0f, 1f]);
    }

    [Fact]
    public void TestWithNonSquareFreePolynomial()
    {
        AssertSquareFreeTransformation([4f, 0f, -4f, 0f, 1f], [-2f, 0f, 1f]);
    }

    [Fact]
    public void TestWithHighDegreePolynomialWithMultipleSquareFactors()
    {
        AssertSquareFreeTransformation([1f, -2f, 1f, 0f, 0f, 0f, 1f], [1f, -2f, 1f, 0f, 0f, 0f, 1f]);
    }

    [Fact]
    public void TestWithPolynomialPerfectSquare()
    {
        AssertSquareFreeTransformation([0f, 0f, 1f], [0, 1f]);
    }
}
