using NonstandardPhysicsSolver.Polynomials.LaurentPolynomial;
using NonstandardPhysicsSolver.Tests.TestUtils;

namespace NonstandardPhysicsSolver.Tests.LaurentPolynomialTests;

public class MultiplyByXPowerTests
{
    [Fact]
    public void MultiplyByXPower_WithPositivePower_ReturnsCorrectLaurentPolynomial()
    {
        // Arrange
        LaurentPolynomial input = new LaurentPolynomial([1, 2, 3], [4, 5, 6]);
        int power = 5;
        LaurentPolynomial expected = new LaurentPolynomial([], [0, 0, 1, 2, 3, 4, 5, 6]);

        // Act
        LaurentPolynomial actual = input.MultiplyByXPower(power);

        // Assert
        AssertExtensions.ArraysEqual(expected.NegativeDegreeCoefficients, actual.NegativeDegreeCoefficients);
        AssertExtensions.ArraysEqual(expected.PositiveDegreeCoefficients, actual.PositiveDegreeCoefficients);
    }

    [Fact]
    public void MultiplyByXPower_WithNegativePower_ReturnsCorrectLaurentPolynomial()
    {
        // Arrange
        LaurentPolynomial input = new LaurentPolynomial([1, 2, 3], [4, 5, 6]);
        int power = -5;
        LaurentPolynomial expected = new LaurentPolynomial([1, 2, 3, 4, 5, 6, 0, 0], []);


        // Act
        LaurentPolynomial actual = input.MultiplyByXPower(power);

        // Assert
        AssertExtensions.ArraysEqual(expected.NegativeDegreeCoefficients, actual.NegativeDegreeCoefficients);
        AssertExtensions.ArraysEqual(expected.PositiveDegreeCoefficients, actual.PositiveDegreeCoefficients);
    }

    [Fact]
    public void MultiplyByXPower_WithPartialNegativeShift_AdjustsPositiveAndNegativeDegrees()
    {
        // Arrange
        LaurentPolynomial input = new LaurentPolynomial([1, 2, 3], [4, 5, 6]);
        int power = -2;
        LaurentPolynomial expected = new LaurentPolynomial([1, 2, 3, 4, 5], [6]);

        // Act
        LaurentPolynomial actual = input.MultiplyByXPower(power);

        // Assert
        AssertExtensions.ArraysEqual(expected.NegativeDegreeCoefficients, actual.NegativeDegreeCoefficients);
        AssertExtensions.ArraysEqual(expected.PositiveDegreeCoefficients, actual.PositiveDegreeCoefficients);
    }

    [Fact]
    public void MultiplyByXPower_WithPartialPositiveShift_AdjustsPositiveAndNegativeDegrees()
    {
        // Arrange
        LaurentPolynomial input = new LaurentPolynomial([1, 2, 3], [4, 5, 6]);
        int power = 2;
        LaurentPolynomial expected = new LaurentPolynomial([1], [2, 3, 4, 5, 6]);

        // Act
        LaurentPolynomial actual = input.MultiplyByXPower(power);

        // Assert
        AssertExtensions.ArraysEqual(expected.NegativeDegreeCoefficients, actual.NegativeDegreeCoefficients);
        AssertExtensions.ArraysEqual(expected.PositiveDegreeCoefficients, actual.PositiveDegreeCoefficients);
    }
}
