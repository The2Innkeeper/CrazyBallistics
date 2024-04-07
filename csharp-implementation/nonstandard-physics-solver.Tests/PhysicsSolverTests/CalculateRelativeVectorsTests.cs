namespace NonstandardPhysicsSolver.Tests.PhysicsSolverTests;

using NonstandardPhysicsSolver.PhysicsSolver;

public class CalculateRelativeVectorsTests
{
    [Fact]
    public void CalculateRelativeVectors_WithValidInput_ReturnsCorrectRelativeVectors()
    {
        // Arrange
        Vector2[] targetVectors = { new Vector2(3, 4), new Vector2(1, 2) };
        Vector2[] shooterVectors = { new Vector2(1, 1), new Vector2(2, 2) };
        Vector2[] expected = { new Vector2(2, 3), new Vector2(-1, 0) };

        // Act
        var result = VelocityMinimizer<Vector2>.CalculateRelativeVectors(targetVectors, shooterVectors);

        // Assert
        Assert.Equal(expected, result);
    }
}
