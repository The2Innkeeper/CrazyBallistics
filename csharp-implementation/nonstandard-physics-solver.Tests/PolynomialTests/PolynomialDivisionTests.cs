namespace NonstandardPhysicsSolver.Polynomials.Tests;

using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;

public class PolynomialDivisionTests
{
    private static void RunTest(Polynomial dividend, Polynomial divisor, Polynomial expectedQuotient, Polynomial expectedRemainder, bool expectException)
    {
        if (expectException)
        {
            Assert.Throws<DivideByZeroException>(() => Polynomial.PolynomialDivision(dividend, divisor));
        }
        else
        {
            var (quotient, remainder) = Polynomial.PolynomialDivision(dividend, divisor);

            AssertExtensions.ListsApproximatelyEqual(expectedQuotient.Coefficients, quotient.Coefficients);
            AssertExtensions.ListsApproximatelyEqual(expectedRemainder.Coefficients, remainder.Coefficients);
        }
    }

    [Fact]
    public void TestPolynomialDivision_NormalCase()
    {
        // Arrange
        var dividend = new Polynomial([1, 0, 1]); // x^2 + 1
        var divisor = new Polynomial([0, 1]); // x
        var expectedQuotient = new Polynomial([0, 1]); // x
        var expectedRemainder = new Polynomial([1]); // 1

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }

    [Fact]
    public void TestPolynomialDivision_DividendIsZero()
    {
        // Arrange
        var dividend = new Polynomial([0]);
        var divisor = new Polynomial([1, 1]); // x + 1
        var expectedQuotient = new Polynomial([0]);
        var expectedRemainder = new Polynomial([0]);

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }

    [Fact]
    public void TestPolynomialDivision_DivisorIsZero()
    {
        // Arrange
        var dividend = new Polynomial([0, 0, 1]); // x^2
        var divisor = new Polynomial([0]); // 0

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => Polynomial.PolynomialDivision(dividend, divisor));
    }

    [Fact]
    public void TestPolynomialDivision_DividendDegreeLessThanDivisor()
    {
        // Arrange
        var dividend = new Polynomial([0, 1]); // x
        var divisor = new Polynomial([0, 0, 1]); // x^2
        var expectedQuotient = new Polynomial([0]);
        var expectedRemainder = new Polynomial([0, 1]); // x

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }

    [Fact]
    public void TestPolynomialDivision_IdenticalPolynomials()
    {
        // Arrange
        var dividend = new Polynomial([1, 1]); // x + 1
        var divisor = new Polynomial([1, 1]); // x + 1
        var expectedQuotient = new Polynomial([1]); // 1
        var expectedRemainder = new Polynomial([0]); // 0

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }
}
