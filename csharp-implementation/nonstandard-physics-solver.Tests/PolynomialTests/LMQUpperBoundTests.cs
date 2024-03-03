namespace NonstandardPhysicsSolver.Polynomials.Tests;

using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;

public class LMQUpperBoundCalculatorTests
{
    [Theory]
    [InlineData(new float[] { -1, -100, 100, 1 }, 2f)]
    [InlineData(new float[] { -6, 11, -6, 1 }, 12f)]
    // (x-1)(x-2)(x-3) = x^3 - 6x^2 + 11x - 6
    /*
     * To calculate this bound by hand, start from the highest degree negative coefficient,
     * then check all higher degree coefficients from high to low for positive terms, scaling a power of 2 incrementally each time the positive coefficient is used.
     * After computing the radicand, you take the root of the difference of degree (j-i).
     * Highest degree negative coefficient: -6 from -6x^2
     * Pair with 1 from 1x^3, t_3 = 1
     * Compute (-2^1 * -6 / 1)^(1/(3-2)) = 12
     * It is the only radical so Min({12}) = 12
     * Then consider -6
     * Pair with 1 from x^3, t_3 = 2 (second time being used)
     * Compute (-2^2 * -6 / 1)^(1/(3-0)) = 2.88449914
     * Pair with 11 from 11x^1, t_1 = 1
     * Compute (-2^1 * -6 / 11)^(1/(1-0)) = 1.09090909
     * Take the minimum Min(2.88449914, 1.09090909) = 1.09090909
     * 
     * After computing all minimums, take the max of all minimums
     * Max({minRadicals}) = Max(12, 1.09090909) = 12
     * So the computed upper bound is 12
     */
    [InlineData(new float[] { 1, -4, -1, 2, 3 }, 1.74716093f)]
    // 3x^4 + 2x^3 - x^2 - 4x + 1
    // For reference, it has real roots at x=0.24505, x=0.89790
    // So upper bound must be > 0.89790
    /* 
     * In this case, the highest degree negative coefficient is from -1x^2. 
     * First, we have 3x^4, so we take (-2^1 * -1 / 3)^(1/2) = sqrt(2/3) = 0.816496581
     * Note that we increment t_4 += 1 so t_4 = 2 now
     * Then we have 2x^3: we take (-2^1 * -1 / 2)^(1/1) = 1
     * t_3 = 2
     * We take the minimum of both these values, which is 0.816496
     * Repeat the process with other negative coefficients, in this case only -4x:
     * t_4 = 2
     * (-2^2 * -4 / 3)^(1/3) = cbrt(16/3) = 1.74716093
     * t_3 = 2
     * (-2^2 * -4 / 2)^(1/2) = sqrt(8) = 2.82842712
     * min(1.74716093, 2.82842712) = 1.74716093
     * Then we take the maximum of these minimums.
     * max(0.66666666, 1.74716093) = 1.74716093
     * So the upper bound is 1.74716093
    */
    public void LMQUpperBound_WithValidCoefficients_ReturnsCorrectBound(float[] coeffs, float expectedBound)
    {
        // Arrange
        var polynomial = new Polynomial(coeffs);

        // Act
        var bound = polynomial.LMQPositiveUpperBound();

        // Assert
        AssertExtensions.FloatsApproximatelyEqual(expectedBound, bound, 1e-4f);
    }
}
