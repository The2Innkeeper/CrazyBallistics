namespace NonstandardPhysicsSolver.Tests.PhysicsSolverTests;

using NonstandardPhysicsSolver.PhysicsSolver;

public class ScaleTaylorVectorCoefficientsTests
{
    [Fact]
    public void ScaleTaylorVectorCoefficientsTests_WithValidInput_ReturnsCorrectPolynomialCoefficients()
    {
        // Arrange
        Vector2[] scaledRelativeVectors = { new Vector2(2, 3), new Vector2(5, 13), new Vector2(17, 23), new Vector2(31, 47) };
        Vector2[] expectedCoefficients = { new Vector2(2, 3), new Vector2(5f, 13f), new Vector2(17f / 2f, 23f / 2f), new Vector2(5.166667f, 7.8333335f) };

        // Act
        Vector2[] resultPolynomial = VelocityMinimizer<Vector2>.ScaleTaylorVectorCoefficients(scaledRelativeVectors);

        // Assert
        Assert.Equal(expectedCoefficients, resultPolynomial);
    }

    // Additional tests can be added here for edge cases and invalid inputs
}
