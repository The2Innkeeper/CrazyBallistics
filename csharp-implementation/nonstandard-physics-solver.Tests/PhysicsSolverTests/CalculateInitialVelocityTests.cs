using NonstandardPhysicsSolver.PhysicsSolver;

namespace NonstandardPhysicsSolver.Tests.PhysicsSolverTests;

public class CalculateInitialVelocityTests
{
    [Fact]
    public void CalculateInitialVelocity_WithValidInput_ReturnsCorrectInitialVelocityVector()
    {
        // Arrange
        Vector2[] relativeVectors = { new Vector2(2, 3), new Vector2(5, 13) };
        float timeToTarget = 7f;
        Vector2 expectedInitialVelocity = new Vector2(5.2857146f, 94f / 7f);

        // Act
        Vector2 actualInitialVelocity = VelocityMinimizer<Vector2>.CalculateInitialVelocity(relativeVectors, timeToTarget);

        // Assert
        Assert.Equal(expectedInitialVelocity, actualInitialVelocity);
    }
}
