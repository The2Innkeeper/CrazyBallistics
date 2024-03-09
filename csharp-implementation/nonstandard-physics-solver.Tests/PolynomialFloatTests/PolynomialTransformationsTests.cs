using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;

namespace NonstandardPhysicsSolver.Polynomials.Tests.PolynomialTests;

public class PolynomialTransformationsTests
{
    [Fact]
    public void ScaleInput_WithPositiveScaleFactor_ScalesCoefficientsCorrectly()
    {
        // Arrange
        var polynomial = new PolynomialFloat([2, 3, 7, 13]);
        float scaleFactor = 5f;

        // Act
        var scaledPolynomial = polynomial.ScaleInput(scaleFactor);

        // Assert
        float[] expected = [2, 15, 175, 1625];
        float[] actual = scaledPolynomial.Coefficients;
        AssertExtensions.ArraysEqual(expected, actual);
    }
}
