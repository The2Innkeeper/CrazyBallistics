namespace NonstandardPhysicsSolver.PhysicsSolver;

public struct Vector2 : IEquatable<Vector2>, IVector<Vector2>
{
    public float X { get; set; }
    public float Y { get; set; }

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Vector2()
    {
        this = Zero;
    }

    public static Vector2 Zero => new Vector2(0, 0);
    public static Vector2 One => new Vector2(1, 1);
    public static Vector2 UnitX => new Vector2(1, 0);
    public static Vector2 UnitY => new Vector2(0, 1);

    public float Magnitude => MathF.Sqrt(X * X + Y * Y);
    public float SquareMagnitude => X * X + Y * Y;
    public Vector2 Normalized
    {
        get
        {
            float magnitude = Magnitude;
            return magnitude > 0 ? new Vector2(X / magnitude, Y / magnitude) : Zero;
        }
    }

    public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
    public static Vector2 operator -(Vector2 v) => new Vector2(-v.X, -v.Y);
    public static Vector2 operator *(Vector2 v, float scalar) => new Vector2(v.X * scalar, v.Y * scalar);
    public static Vector2 operator *(float scalar, Vector2 v) => new Vector2(v.X * scalar, v.Y * scalar);
    public static Vector2 operator /(Vector2 v, float scalar) => new Vector2(v.X / scalar, v.Y / scalar);

    public Vector2 Add(Vector2 other) => this + other;
    public Vector2 Subtract(Vector2 other) => this - other;
    public Vector2 Scale(float scalar) => this * scalar;
    public float Dot(Vector2 other) => Dot(this, other);
    public float MagnitudeSquared() => SquareMagnitude;
    public Vector2 ZeroVector() => Zero;

    public static float Dot(Vector2 a, Vector2 b) => a.X * b.X + a.Y * b.Y;
    public static float Cross(Vector2 a, Vector2 b) => a.X * b.Y - a.Y * b.X;

    public static float Distance(Vector2 a, Vector2 b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return (float)Math.Sqrt(dx * dx + dy * dy);
    }

    public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => a + (b - a) * t;

    public static Vector2 Reflect(Vector2 v, Vector2 normal) => v - 2 * Dot(v, normal) * normal;

    public static Vector2 Rotate(Vector2 v, float angle)
    {
        float cos = (float)Math.Cos(angle);
        float sin = (float)Math.Sin(angle);
        return new Vector2(v.X * cos - v.Y * sin, v.X * sin + v.Y * cos);
    }

    public override string ToString() => $"({X}, {Y})";

    public bool Equals(Vector2 other)
    {
        return MathF.Abs(X - other.X) < float.Epsilon && MathF.Abs(Y - other.Y) < float.Epsilon;
    }

    public override bool Equals(object obj)
    {
        if (obj is Vector2 other)
            return Equals(other);

        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Vector2 left, Vector2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2 left, Vector2 right)
    {
        return !left.Equals(right);
    }
}