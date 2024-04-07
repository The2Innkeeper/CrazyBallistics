namespace NonstandardPhysicsSolver.Tests.PolynomialDoubleTests;

using NonstandardPhysicsSolver.Tests.TestUtils.TestUtilsDouble;

public class GcdPolynomialsTest
{
    private static void AssertPolynomialGCD(PolynomialDouble p, PolynomialDouble q, PolynomialDouble expected)
    {
        PolynomialDouble actual = PolynomialDouble.PolynomialGCD(p, q);
        // Make sure they are both normalized or there can be mistakes
        actual = actual.Normalized();
        expected = expected.Normalized();
        AssertExtensionsDouble.ArraysApproximatelyEqual(actual.Coefficients, expected.Coefficients);
    }

    [Fact]
    public void TestGcdPolynomialsWithGCD1()
    {
        var a = new PolynomialDouble([0, 0, 1]); // x^2
        var b = new PolynomialDouble([1, 1]); // x + 1
        var expected = new PolynomialDouble([1]); // GCD 1

        AssertPolynomialGCD(a, b, expected);
    }

    [Fact]
    public void TestGcdPolynomialsWithGCDNot1()
    {
        var a = new PolynomialDouble([-1, 0, 1]); // x^2 - 1
        var b = new PolynomialDouble([1, 1]); // x + 1
        var expected = new PolynomialDouble([1, 1]); // GCD x + 1

        AssertPolynomialGCD(a, b, expected);
    }

    [Fact]
    public void TestGcdPolynomialsWithHigherDegree()
    {
        var a = new PolynomialDouble([0, 0, 3, -6, 3]); // 3x^4 - 6x^3 + 3x^2
        var b = new PolynomialDouble([0, 6, -12, 6]); // 6x^3 - 12x^2 + 6x
        var expected = new PolynomialDouble([0, 1, -2, 1]); // Normalized GCD: x^3 - 2x^2 + x

        AssertPolynomialGCD(a, b, expected);
    }
}