namespace Polynomial.Tests;

public class PolynomialDivisionTests
{
    /// <summary>
    /// Tests the division of two polynomials in a normal case.
    /// </summary>
    /// <remarks>
    /// (x^2 + 1) / x = x + 1/x
    /// </remarks>
    [Fact]
    public void TestPolynomialDivision_NormalCase()
    {
        // Arrange
        var dividend = new List<float> { 1, 0, 1 };
        var divisor = new List<float> { 0, 1 };
        var expectedQuotient = new List<float> { 0, 1 };
        var expectedRemainder = new List<float> { 1 };
        var expectedException = false;

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectedException);
    }

    /// <summary>
    /// Tests the scenario where the dividend is zero.
    /// </summary>
    /// <remarks>
    /// 0 / (x + 1) = 0
    /// </remarks>
    [Fact]
    public void TestPolynomialDivision_DividendIsZero()
    {
        // Arrange
        var dividend = new List<float> { 0 };
        var divisor = new List<float> { 1, 1 };
        var expectedQuotient = new List<float> { 0 };
        var expectedRemainder = new List<float> { 0 };
        var expectedException = false;

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectedException);
    }

    /// <summary>
    /// Tests the division of a polynomial by zero, expecting an exception.
    /// </summary>
    /// <remarks>
    /// x^2 / 0 => exception
    /// </remarks>
    [Fact]
    public void TestPolynomialDivision_DivisorIsZero()
    {
        // Arrange
        var dividend = new List<float> { 0, 0, 1 };
        var divisor = new List<float> { 0 };
        var expectedException = true;

        // Act & Assert
        if (expectedException)
        {
            Assert.Throws<DivideByZeroException>(() => Polynomial.PolynomialDivision(dividend, divisor));
        }
    }

    /// <summary>
    /// Tests the case where the dividend degree is less than the divisor degree.
    /// </summary>
    /// <remarks>
    /// x / x^2 = 1/x
    /// </remarks>
    [Fact]
    public void TestPolynomialDivision_DividendDegreeLessThanDivisor()
    {
        // Arrange
        var dividend = new List<float> { 0, 1 };
        var divisor = new List<float> { 0, 0, 1 };
        var expectedQuotient = new List<float> { 0 };
        var expectedRemainder = new List<float> { 0, 1 };
        var expectedException = false;

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectedException);
    }

    /// <summary>
    /// Tests the division of two identical polynomials.
    /// </summary>
    /// <remarks>
    /// (x + 1) / (x + 1) = 1
    /// </remarks>
    [Fact]
    public void TestPolynomialDivision_IdenticalPolynomials()
    {
        // Arrange
        var dividend = new List<float> { 1, 1 };
        var divisor = new List<float> { 1, 1 };
        var expectedQuotient = new List<float> { 1 };
        var expectedRemainder = new List<float> { 0 };
        var expectedException = false;

        // Act & Assert
        RunTest(dividend, divisor, expectedQuotient, expectedRemainder, expectedException);
    }

    private static void RunTest(List<float> dividend, List<float> divisor, List<float> expectedQuotient, List<float> expectedRemainder, bool expectedException)
    {
        if (expectedException)
        {
            Assert.Throws<DivideByZeroException>(() => Polynomial.PolynomialDivision(dividend, divisor));
        }
        else
        {
            var (quotient, remainder) = Polynomial.PolynomialDivision(dividend, divisor);
            Assert.Equal(expectedQuotient, quotient);
            Assert.Equal(expectedRemainder, remainder);
        }
    }
}