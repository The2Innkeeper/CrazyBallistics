using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;

namespace NonstandardPhysicsSolver.Polynomials.Tests.PolynomialTests;
public class PositiveDomainMinimumTests
{
    // (x-2)^2 = 4 -4x +x^2
    // Global minimum at x=2
    [Fact]
    public void GlobalMinimum_WithSingleMinimumQuadratic()
    {
        // Arrange
        float[] coefficients = [4f, -4f, 1f];
        float expectedMinX = 2f;
        float expectedMinValue = 0f;

        Polynomial polynomial = new Polynomial(coefficients);

        // Act
        var (actualMinX, actualMinValue) = polynomial.FindGlobalMinimum();

        // Assert
        AssertExtensions.FloatsApproximatelyEqual(expectedMinX, actualMinX);
        AssertExtensions.FloatsApproximatelyEqual(expectedMinValue, actualMinValue);
    }

    // 22 - 10 x - 4 x^2 + 2 x^3
    // Minimum at x≈2.11963f, value≈1.87865482
    [Fact]
    public void GlobalMinimum_WithSingleMinimumInPositiveDomainCubic()
    {
        // Arrange
        float[] coefficients = [22f, -10f, -4f, 2];
        float expectedMinX = 2.11963f;
        float expectedMinValue = 1.87865f;

        Polynomial polynomial = new Polynomial(coefficients);

        // Act
        var (actualMinX, actualMinValue) = polynomial.FindGlobalMinimum();

        // Assert
        AssertExtensions.FloatsApproximatelyEqual(expectedMinX, actualMinX);
        AssertExtensions.FloatsApproximatelyEqual(expectedMinValue, actualMinValue);
    }

    // 174 - 215 x + 91 x^2 - 16 x^3 + x^4
    // Global minimum of R in negatives, irrelevant: min{174 - 215 x + 91 x^2 - 16 x^3 + x^4}≈-2.69471 at x≈5.52854
    // What we want: min{174 - 215 x + 91 x^2 - 16 x^3 + x^4}≈-5.85539 at x≈2.37105
    [Fact]
    public void GlobalMinimum_WithMultipleLocalMinimaQuartic()
    {
        // Arrange
        float[] coefficients = [174f, -215f, 91f, -16f, 1f];
        float expectedMinX = 2.37105f;
        float expectedMinValue = -5.85539f;

        Polynomial polynomial = new Polynomial(coefficients);

        // Act
        var (actualMinX, actualMinValue) = polynomial.FindGlobalMinimum();

        // Assert
        AssertExtensions.FloatsApproximatelyEqual(expectedMinX, actualMinX);
        AssertExtensions.FloatsApproximatelyEqual(expectedMinValue, actualMinValue, tolerance: 1e-4f);
    }


    // 2 x^4 - 10 x^3 + 18 x^2 - 14 x + 5
    // min{2 (x - 1)^3 (x - 2) + 1} = 0.789063 at x = 1.75
    [Fact]
    public void GlobalMinimum_WithNonSquarefreeQuartic()
    {
        // Arrange
        float[] coefficients = [5f, -14f, 18f, -10f, 2f];
        float expectedMinX = 1.75f;
        float expectedMinValue = 0.789063f;

        Polynomial polynomial = new Polynomial(coefficients);

        // Act
        var (actualMinX, actualMinValue) = polynomial.FindGlobalMinimum();

        // Assert
        AssertExtensions.FloatsApproximatelyEqual(expectedMinX, actualMinX);
        AssertExtensions.FloatsApproximatelyEqual(expectedMinValue, actualMinValue);
    }

    // (x-2)^2 + 1 = 5 -4x +x^2
    // global minimum 1 at x = 2
    [Fact]
    public void GlobalMinimum_WithPositiveRootFreeQuadratic()
    {
        // Arrange
        float[] coefficients = [5f, -4f, 1f];
        float expectedMinX = 2f;
        float expectedMinValue = 1f;

        Polynomial polynomial = new Polynomial(coefficients);

        // Act
        var (actualMinX, actualMinValue) = polynomial.FindGlobalMinimum();

        // Assert
        AssertExtensions.FloatsApproximatelyEqual(expectedMinX, actualMinX);
        AssertExtensions.FloatsApproximatelyEqual(expectedMinValue, actualMinValue);
    }
}
