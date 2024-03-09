namespace NonstandardPhysicsSolver.Polynomials.Tests.PolynomialDoubleTests;

using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;

public class PolynomialDivisionTests
{
    private static void RunTest(PolynomialDouble dividend, PolynomialDouble divisor, PolynomialDouble expectedQuotient, PolynomialDouble expectedRemainder, bool expectException)
    {
        if (expectException)
        {
            Assert.Throws<DivideByZeroException>(() => PolynomialDouble.PolynomialDivision(dividend, divisor));
        }
        else
        {
            var (quotient, remainder) = PolynomialDouble.PolynomialDivision(dividend, divisor);

            AssertExtensionsDouble.ArraysApproximatelyEqual(expectedQuotient.Coefficients, quotient.Coefficients);
            AssertExtensionsDouble.ArraysApproximatelyEqual(expectedRemainder.Coefficients, remainder.Coefficients);
        }
    }

    [Fact]
    public void TestPolynomialDivision_NormalCase()
    {
        // Arrange
        var dividend = new PolynomialDouble([1, 0, 1]); // x^2 + 1
        var divisor = new PolynomialDouble([0, 1]); // x
        var expectedQuotient = new PolynomialDouble([0, 1]); // x
        var expectedRemainder = new PolynomialDouble([1]); // 1

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }

    [Fact]
    public void TestPolynomialDivision_DividendIsZero()
    {
        // Arrange
        var dividend = new PolynomialDouble([0]);
        var divisor = new PolynomialDouble([1, 1]); // x + 1
        var expectedQuotient = new PolynomialDouble([0]);
        var expectedRemainder = new PolynomialDouble([0]);

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }

    [Fact]
    public void TestPolynomialDivision_DivisorIsZero()
    {
        // Arrange
        var dividend = new PolynomialDouble([0, 0, 1]); // x^2
        var divisor = new PolynomialDouble([0]); // 0

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => PolynomialDouble.PolynomialDivision(dividend, divisor));
    }

    [Fact]
    public void TestPolynomialDivision_DividendDegreeLessThanDivisor()
    {
        // Arrange
        var dividend = new PolynomialDouble([0, 1]); // x
        var divisor = new PolynomialDouble([0, 0, 1]); // x^2
        var expectedQuotient = new PolynomialDouble([0]);
        var expectedRemainder = new PolynomialDouble([0, 1]); // x

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }

    [Fact]
    public void TestPolynomialDivision_IdenticalPolynomials()
    {
        // Arrange
        var dividend = new PolynomialDouble([1, 1]); // x + 1
        var divisor = new PolynomialDouble([1, 1]); // x + 1
        var expectedQuotient = new PolynomialDouble([1]); // 1
        var expectedRemainder = new PolynomialDouble([0]); // 0

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }
}
