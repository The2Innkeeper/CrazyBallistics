namespace NonstandardPhysicsSolver.Polynomials.Tests.PolynomialDoubleTests;

using NonstandardPhysicsSolver.Intervals;
using NonstandardPhysicsSolver.Polynomials;
using NonstandardPhysicsSolver.Polynomials.Tests.TestUtils;

public class RefineIntervalITPTests
{
    [Theory]
    // (x+1)(x+3) = x^2 + 4x + 3 has roots at x=1, x=3
    [InlineData(0, 2, 1.0f)] // Test case for root at x=1
    [InlineData(2, 4, 3.0f)] // Test case for root at x=3
    [InlineData(1, 1, 1.0f)] // Equal bounds
    public void RefineIntervalITP_KnownRoots_ReturnsExpectedRoot(double leftBound, double rightBound, double expectedRoot)
    {
        PolynomialDouble polynomial = new([3, -4, 1]);
        double tol = 0.0005f;

        double actualRoot = IntervalDouble.RefineRootIntervalITP(polynomial.EvaluatePolynomialAccurate, leftBound, rightBound, tol);

        // Assert that the actual root is within tolerance of the expected root
        AssertExtensionsDouble.DoublesApproximatelyEqual(expectedRoot, actualRoot, tol);
    }
}
