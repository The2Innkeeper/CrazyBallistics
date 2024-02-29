﻿namespace NonstandardPhysicsSolver.Polynomials.Tests;

using NonstandardPhysicsSolver.Polynomials;
using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;

public class PolynomialRootRefinerTests
{
    [Theory]
    // (x+1)(x+3) = x^2 + 4x + 3 has roots at x=1, x=3
    [InlineData(0, 2, 1.0)] // Test case for root at x=1
    [InlineData(2, 4, 3.0)] // Test case for root at x=3
    public void RefineIntervalITP_KnownRoots_ReturnsExpectedRoot(float leftBound, float rightBound, float expectedRoot)
    {
        Polynomial polynomial = new([3, -4, 1]);
        float tol = 0.0005f;

        float actualRoot = polynomial.RefineIntervalITP(leftBound, rightBound, tol);

        // Assert that the actual root is within tolerance of the expected root
        AssertExtensions.FloatsApproximatelyEqual(expectedRoot, actualRoot, tol);
    }
}
