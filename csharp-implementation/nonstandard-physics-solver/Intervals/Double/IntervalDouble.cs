namespace NonstandardPhysicsSolver.Intervals;

using NonstandardPhysicsSolver.Polynomials;

/// <summary>
/// Open-closed interval ]a,b] due to Budan's theorem's statements
/// </summary>
public partial struct IntervalDouble
{
    public double LeftBound { get; }
    public double RightBound { get; }
    public double Length { get; }

    public IntervalDouble(double leftBound, double rightBound)
    {
        if (double.IsNaN(leftBound) || double.IsNaN(rightBound))
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
        this.Length = this.RightBound - this.LeftBound;
    }

    // Indexer to access bounds like an array
    public double this[int index] => index switch
    {
        0 => LeftBound,
        1 => RightBound,
        _ => throw new IndexOutOfRangeException("Valid indexes are 0 for LeftBound and 1 for RightBound."),
    };

    public bool ContainsValue(double value)
    {
        if (double.IsNaN(value))
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
    public bool ContainsSingleRoot(PolynomialDouble polynomial)
    {
        // Checking sign change as a necessary condition for a root in the interval
        double valueAtLeft = polynomial.EvaluatePolynomialAccurate(LeftBound);
        double valueAtRight = polynomial.EvaluatePolynomialAccurate(RightBound);

        // If the signs are different, there is at least one root in the interval
        return Math.Sign(valueAtLeft) != Math.Sign(valueAtRight);
    }
}
