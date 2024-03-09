namespace NonstandardPhysicsSolver.PhysicsSolver;

public interface IVector<T> where T : IVector<T>
{
    T Add(T other);
    T Subtract(T other);
    T Scale(float scalar);
    float Dot(T other);
    float MagnitudeSquared();
    T ZeroVector();
}