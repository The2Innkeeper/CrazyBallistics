namespace NonstandardPhysicsSolver.Tests.PolynomialFloatTests;

using NonstandardPhysicsSolver.Tests.TestUtils;

public class PolynomialDivisionTests
{
    private static void RunTest(PolynomialFloat dividend, PolynomialFloat divisor, PolynomialFloat expectedQuotient, PolynomialFloat expectedRemainder, bool expectException)
    {
        if (expectException)
        {
            Assert.Throws<DivideByZeroException>(() => PolynomialFloat.PolynomialDivision(dividend, divisor));
        }
        else
        {
            var (quotient, remainder) = PolynomialFloat.PolynomialDivision(dividend, divisor);

            AssertExtensions.ArraysApproximatelyEqual(expectedQuotient.Coefficients, quotient.Coefficients);
            AssertExtensions.ArraysApproximatelyEqual(expectedRemainder.Coefficients, remainder.Coefficients);
        }
    }

    [Fact]
    public void TestPolynomialDivision_NormalCase()
    {
        // Arrange
        var dividend = new PolynomialFloat([1, 0, 1]); // x^2 + 1
        var divisor = new PolynomialFloat([0, 1]); // x
        var expectedQuotient = new PolynomialFloat([0, 1]); // x
        var expectedRemainder = new PolynomialFloat([1]); // 1

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }

    [Fact]
    public void TestPolynomialDivision_DividendIsZero()
    {
        // Arrange
        var dividend = new PolynomialFloat([0]);
        var divisor = new PolynomialFloat([1, 1]); // x + 1
        var expectedQuotient = new PolynomialFloat([0]);
        var expectedRemainder = new PolynomialFloat([0]);

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }

    [Fact]
    public void TestPolynomialDivision_DivisorIsZero()
    {
        // Arrange
        var dividend = new PolynomialFloat([0, 0, 1]); // x^2
        var divisor = new PolynomialFloat([0]); // 0

        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => PolynomialFloat.PolynomialDivision(dividend, divisor));
    }

    [Fact]
    public void TestPolynomialDivision_DividendDegreeLessThanDivisor()
    {
        // Arrange
        var dividend = new PolynomialFloat([0, 1]); // x
        var divisor = new PolynomialFloat([0, 0, 1]); // x^2
        var expectedQuotient = new PolynomialFloat([0]);
        var expectedRemainder = new PolynomialFloat([0, 1]); // x

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }

    [Fact]
    public void TestPolynomialDivision_IdenticalPolynomials()
    {
        // Arrange
        var dividend = new PolynomialFloat([1, 1]); // x + 1
        var divisor = new PolynomialFloat([1, 1]); // x + 1
        var expectedQuotient = new PolynomialFloat([1]); // 1
        var expectedRemainder = new PolynomialFloat([0]); // 0

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectException: false);
    }
}
