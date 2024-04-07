namespace NonstandardPhysicsSolver.Tests.PhysicsSolverTests;

using NonstandardPhysicsSolver.PhysicsSolver;
using Xunit;

public class ExpandVelocityNumeratorPolynomialTests
{
    [Fact]
    public void ExpandVelocityNumeratorPolynomial_WithValidInput_ReturnsCorrectPolynomialCoefficients()
    {
        // Arrange
        Vector2[] scaledRelativeVectors = { new Vector2(2, 3), new Vector2(7, 13) };
        float[] expectedCoefficients = { 13, 106, 218 };

        // Act
        var resultPolynomial = VelocityMinimizer<Vector2>.ExpandVelocityNumeratorPolynomial(scaledRelativeVectors);

        // Assert
        Assert.Equal(expectedCoefficients, resultPolynomial.Coefficients);
    }

    // Additional tests can be added here for edge cases and invalid inputs
}
