namespace NonstandardPhysicsSolver.Tests.PolynomialDoubleTests;

using NonstandardPhysicsSolver.Tests.TestUtils.TestUtilsDouble;

public class PolynomialTransformationsTests
{
    [Fact]
    public void ScaleInput_WithPositiveScaleFactor_ScalesCoefficientsCorrectly()
    {
        // Arrange
        var polynomial = new PolynomialDouble([2, 3, 7, 13]);
        double scaleFactor = 5f;

        // Act
        var scaledPolynomial = polynomial.ScaleInput(scaleFactor);

        // Assert
        double[] expected = [2, 15, 175, 1625];
        double[] actual = scaledPolynomial.Coefficients;
        AssertExtensionsDouble.ArraysEqual(expected, actual);
    }
}
