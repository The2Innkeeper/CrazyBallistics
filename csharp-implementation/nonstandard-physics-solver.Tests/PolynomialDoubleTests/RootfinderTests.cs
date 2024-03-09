namespace NonstandardPhysicsSolver.Polynomials.Tests.PolynomialDoubleTests;

using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;

public class RootfinderTests
{
    // This one does not work, the numbers blow up to infinity ;'(
    [Theory]
    //(x−0.02)(x−0.2346)(x−0.5)(x−3)(x−11) = -0.077418 + 4.38858 x - 26.752 x^2 + 43.6964 x^3 - 14.7546 x^4 + x^5
    // Hard one: failed (probably first few roots are too close, so numerical errors add up in floats.
    // You would likely need very high precision or even arbitrary precision for exact arithmetic.
    // Update: It turns out that it was a bug with the upper bound LMQ calculator, it was giving too low of a value and therefore missing roots.
    // Investigation needed
    //[InlineData(new double[] { -0.077418f, 4.38858f, -26.752f, 43.6964f, 14.7546f, 1f }, new double[] { 0.02f, 0.2346f, 0.5f, 3f, 11f })]
    // This one should be reasonable.
    // 84.9837 - 74.5816 x + 21.3423 x^2 - 2 x^3
    // Roots at x≈2.64986, x≈3.78754, x≈4.23375
    [InlineData(new double[] { 84.9837f, -74.5816f, 21.3423f, -2 }, new double[] { 2.64986f, 3.78754f, 4.23375f })]
    public void Rootfinder_WithCloseAndSpacedRoots_FindsAllRootsWithinTolerance(double[] coeffs, double[] expectedRoots)
    {
        // Arrange
        PolynomialDouble polynomial = new PolynomialDouble(coeffs);
        double precision = 1e-4f;
        var expectedRootsList = new List<double>(expectedRoots);

        // Act
        var actualRoots = polynomial.FindAllRoots(precision);
        actualRoots.Sort();

        // Assert
        AssertExtensionsDouble.ListsApproximatelyEqual(expectedRootsList, actualRoots, precision);
    }

    [Fact]
    // Test with (x-1)^2*(x-2) = -2 + 5 x - 4 x^2 + x^3
    public void Rootfinder_WithNonSquarefreePolynomial_FindsAllRootsWithinTolerance()
    {
        // Arrange
        double[] coeffs = [-2, 5, -4, 1];
        PolynomialDouble polynomial = new PolynomialDouble(coeffs);
        double precision = 1e-4f;
        List<double> expectedRoots = [1f, 2f];

        // Act
        var actualRoots = polynomial.FindAllRoots(precision);
        actualRoots.Sort();

        // Assert
        AssertExtensionsDouble.ListsApproximatelyEqual(expectedRoots, actualRoots, precision);
    }

    [Fact]
    // Test with -3x^4-x^2+40x-3
    // Roots at x≈0.075144, x≈2.2978
    public void Rootfinder_WithNonNormalizedCoefficients_FindsAllRootsWithinTolerance()
    {
        // Arrange
        double[] coeffs = [-3, 40, -1, 0, -3];
        PolynomialDouble polynomial = new PolynomialDouble(coeffs);
        double precision = 1e-4f;
        List<double> expectedRoots = [0.075144f, 2.2978f];

        // Act
        var actualRoots = polynomial.FindAllRoots(precision);
        actualRoots.Sort();

        // Assert
        AssertExtensionsDouble.ListsApproximatelyEqual(expectedRoots, actualRoots, precision);
    }
}
