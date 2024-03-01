﻿namespace NonstandardPhysicsSolver.Polynomials.Tests;

using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;

public class LMQUpperBoundCalculatorTests
{
    [Fact]
    public void LMQUpperBound_WithValidCoefficients_ReturnsCorrectBound()
    {
        // Arrange
        var coeffs = new float[] { 1, -2, -1, 2, 3 }; // 3x^4 + 2x^3 - x^2 - 2x + 1
        var polynomial = new Polynomial(coeffs);
        /* 
         * To calculate this bound by hand, start from the highest degree negative coefficient,
         * then check all higher degree coefficients from high to low for positive terms, incrementing a scaling power of 2.
         * After computing the radicand, you take the root of the difference of degree (j-i).
         * In this case, it is -1 * x^2. First, we have 3x^4, so we take (-2^1 * -1 / 3)^(1/2) = sqrt(2/3) = 0.816496581
         * Then we lower the degree to 2x^3: we take (-2^2 * -1 / 2)^(1/1) = 2/3 = 0.666666666
         * We take the minimum of both these values, which is 2/3 = 0.66666...
         * Repeat the process with other negative coefficients, in this case only -2x:
         * (-2^1 * -2 / 3)^(1/3) = cbrt(4/3) = 1.10064242
         * (-2^2 * -2 / 2)^(1/2) = 2
         * min(4/3, 4) = cbrt(4/3)
         * Then we take the maximum of these minimums.
         * max(2/3, cbrt(4/3)) = cbrt(4/3) = 1.10064242
        */
        float expectedBound = MathF.Pow(4.0f / 3.0f, 1.0f / 3.0f);

        // Act
        var bound = polynomial.LMQPositiveUpperBound();

        // Assert
        AssertExtensions.FloatsApproximatelyEqual(expectedBound, bound, 1e-4f);
    }
}