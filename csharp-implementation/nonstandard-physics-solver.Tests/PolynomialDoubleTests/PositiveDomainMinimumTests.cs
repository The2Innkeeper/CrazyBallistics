namespace NonstandardPhysicsSolver.Tests.PolynomialDoubleTests;

using NonstandardPhysicsSolver.Tests.TestUtils.TestUtilsDouble;

public class PositiveDomainMinimumTests
{
    // (x-2)^2 = 4 -4x +x^2
    // Global minimum at x=2
    [Fact]
    public void GlobalMinimum_WithSingleMinimumQuadratic()
    {
        // Arrange
        double[] coefficients = [4f, -4f, 1f];
        double expectedMinX = 2f;
        double expectedMinValue = 0f;

        PolynomialDouble polynomial = new PolynomialDouble(coefficients);

        // Act
        var (actualMinX, actualMinValue) = polynomial.FindGlobalMinimum();

        // Assert
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedMinX, actualMinX);
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedMinValue, actualMinValue);
    }

    // 22 - 10 x - 4 x^2 + 2 x^3
    // Minimum at x≈2.11963f, value≈1.87865482
    [Fact]
    public void GlobalMinimum_WithSingleMinimumInPositiveDomainCubic()
    {
        // Arrange
        double[] coefficients = [22f, -10f, -4f, 2];
        double expectedMinX = 2.11963f;
        double expectedMinValue = 1.87865f;

        PolynomialDouble polynomial = new PolynomialDouble(coefficients);

        // Act
        var (actualMinX, actualMinValue) = polynomial.FindGlobalMinimum();

        // Assert
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedMinX, actualMinX);
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedMinValue, actualMinValue);
    }

    // 174 - 215 x + 91 x^2 - 16 x^3 + x^4
    // Global minimum of R in negatives, irrelevant: min{174 - 215 x + 91 x^2 - 16 x^3 + x^4}≈-2.69471 at x≈5.52854
    // What we want: min{174 - 215 x + 91 x^2 - 16 x^3 + x^4}≈-5.85539 at x≈2.37105
    [Fact]
    public void GlobalMinimum_WithMultipleLocalMinimaQuartic()
    {
        // Arrange
        double[] coefficients = [174f, -215f, 91f, -16f, 1f];
        double expectedMinX = 2.37105f;
        double expectedMinValue = -5.85539f;

        PolynomialDouble polynomial = new PolynomialDouble(coefficients);

        // Act
        var (actualMinX, actualMinValue) = polynomial.FindGlobalMinimum();

        // Assert
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedMinX, actualMinX);
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedMinValue, actualMinValue, tolerance: 1e-4f);
    }


    // 2 x^4 - 10 x^3 + 18 x^2 - 14 x + 5
    // min{2 (x - 1)^3 (x - 2) + 1} = 0.789063 at x = 1.75
    [Fact]
    public void GlobalMinimum_WithNonSquarefreeQuartic()
    {
        // Arrange
        double[] coefficients = [5f, -14f, 18f, -10f, 2f];
        double expectedMinX = 1.75f;
        double expectedMinValue = 0.789063f;

        PolynomialDouble polynomial = new PolynomialDouble(coefficients);

        // Act
        var (actualMinX, actualMinValue) = polynomial.FindGlobalMinimum();

        // Assert
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedMinX, actualMinX);
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedMinValue, actualMinValue);
    }

    // (x-2)^2 + 1 = 5 -4x +x^2
    // global minimum 1 at x = 2
    [Fact]
    public void GlobalMinimum_WithPositiveRootFreeQuadratic()
    {
        // Arrange
        double[] coefficients = [5f, -4f, 1f];
        double expectedMinX = 2f;
        double expectedMinValue = 1f;

        PolynomialDouble polynomial = new PolynomialDouble(coefficients);

        // Act
        var (actualMinX, actualMinValue) = polynomial.FindGlobalMinimum();

        // Assert
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedMinX, actualMinX);
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedMinValue, actualMinValue);
    }
}
