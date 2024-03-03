namespace NonstandardPhysicsSolver.Intervals;

using NonstandardPhysicsSolver.Polynomials;

public partial struct Interval
{
    public float LeftBound { get; }
    public float RightBound { get; }

    public Interval(float leftBound, float rightBound)
    {
        if (float.IsNaN(leftBound) || float.IsNaN(rightBound))
        {
            throw new ArgumentException("Bounds cannot be NaN.");
        }

        if (leftBound > rightBound)
        {
            this.LeftBound = rightBound;
            this.RightBound = leftBound;
        }
        else
        {
            this.LeftBound = leftBound;
            this.RightBound = rightBound;
        }
    }

    // Indexer to access bounds like an array
    public float this[int index] => index switch
    {
        0 => LeftBound,
        1 => RightBound,
        _ => throw new IndexOutOfRangeException("Valid indexes are 0 for LeftBound and 1 for RightBound."),
    };

    public bool ContainsValue(float value)
    {
        if (float.IsNaN(value))
        {
            throw new ArgumentException("Value cannot be NaN.");
        }
        return value >= LeftBound && value <= RightBound;
    }

    /// <summary>
    /// Checks if the interval contains at least one root of the polynomial.
    /// </summary>
    /// <param name="polynomial">The polynomial to check against.</param>
    /// <returns>True if the interval contains at least one root, otherwise false.</returns>
    public bool ContainsRoot(Polynomial polynomial)
    {
        // Checking sign change as a necessary condition for a root in the interval
        float valueAtLeft = polynomial.EvaluatePolynomialAccurate(LeftBound);
        float valueAtRight = polynomial.EvaluatePolynomialAccurate(RightBound);

        // If the signs are different, there is at least one root in the interval
        return MathF.Sign(valueAtLeft) != MathF.Sign(valueAtRight);
    }
}
