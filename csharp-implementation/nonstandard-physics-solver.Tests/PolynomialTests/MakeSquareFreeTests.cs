namespace NonstandardPhysicsSolver.Polynomials.Tests;

using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;
using PolynomialUtils;
using Xunit;

public class MakeSquareFreeTests
{
    private static void AssertSquareFreeTransformation(List<float> originalCoefficients, List<float> expectedCoefficients)
    {
        // Arrange
        var polynomial = new Polynomial(originalCoefficients);

        // Act
        var squareFreePolynomial = polynomial.MakeSquareFree();
        var actualCoefficients = PolynomialUtils.NormalizedCoefficients(squareFreePolynomial);

        // Assert
        AssertExtensions.ListsApproximatelyEqual(expectedCoefficients, actualCoefficients);
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
