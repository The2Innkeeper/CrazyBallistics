namespace NonstandardPhysicsSolver.Intervals;

public readonly struct MobiusTransformation
{
    public readonly float NumeratorCoefficient, NumeratorConstant, DenominatorCoefficient, DenominatorConstant;

    /// <summary>
    /// Applies the transformation Mobius(x) := (ax+b)/(cx+d).
    /// </summary>
    /// <param name="numeratorCoefficient"></param>
    /// <param name="numeratorConstant"></param>
    /// <param name="denominatorCoefficient"></param>
    /// <param name="denominatorConstant"></param>
    /// <exception cref="ArgumentException"></exception>

    public MobiusTransformation(float numeratorCoefficient, float numeratorConstant, float denominatorCoefficient, float denominatorConstant)
    {
        if (numeratorCoefficient * denominatorConstant == numeratorConstant * denominatorCoefficient)
        {
            throw new ArgumentException("Invalid Möbius transformation parameters: ad should not equal bc.");
        }

        NumeratorCoefficient = numeratorCoefficient;
        NumeratorConstant = numeratorConstant;
        DenominatorCoefficient = denominatorCoefficient;
        DenominatorConstant = denominatorConstant;
    }

    /// <summary>
    /// Constructor to copy a Mobius transformation
    /// </summary>
    /// <param name="mobius"></param>
    public MobiusTransformation(MobiusTransformation mobius)
    {
        NumeratorCoefficient = mobius.NumeratorCoefficient;
        NumeratorConstant = mobius.NumeratorConstant;
        DenominatorCoefficient = mobius.DenominatorCoefficient;
        DenominatorConstant = mobius.DenominatorConstant;
    }

    /// <summary>
    /// Apply the transformation x := 1 /(x + 1)
    /// </summary>
    public readonly MobiusTransformation ProcessUnitInterval()
    {
        return new MobiusTransformation(
            NumeratorConstant,
            NumeratorConstant + NumeratorCoefficient,
            DenominatorConstant,
            DenominatorConstant + DenominatorCoefficient);
    }

    /// <summary>
    /// Transforms the Möbius transformation for the lower interval based on a shift value 's'.
    /// Specifically, applies x := s / (x + 1)
    /// </summary>
    /// <param name="shift">The shift amount used for the transformation.</param>
    /// <returns>A new MobiusTransformation adjusted for the lower interval.</returns>
    public readonly MobiusTransformation TransformedForLowerInterval(float shift)
    {
        float newA = NumeratorConstant; // a becomes b
        float newB = NumeratorConstant + shift * NumeratorCoefficient; // b becomes as + b
        float newC = DenominatorConstant; // c becomes d
        float newD = DenominatorConstant + shift * DenominatorCoefficient; // d becomes cs + d

        return new MobiusTransformation(newA, newB, newC, newD);
    }

    public readonly float EvaluateAt(float x)
    {
        float denominator = DenominatorCoefficient * x + DenominatorConstant;
        if (MathF.Abs(denominator) < float.Epsilon)
        {
            // Handle division by zero as per your application's context
            // For example, return float.PositiveInfinity or float.NegativeInfinity based on the sign of the numerator
            float numerator = NumeratorCoefficient * x + NumeratorConstant;
            if (numerator > 0) return float.PositiveInfinity;
            if (numerator < 0) return float.NegativeInfinity;
        }
        return (NumeratorCoefficient * x + NumeratorConstant) / denominator;
    }

    public readonly Interval PositiveDomainImage()
    {
        float bound1, bound2;
        bound1 = NumeratorCoefficient / DenominatorCoefficient;
        bound2 = NumeratorConstant / DenominatorConstant;

        return new Interval(MathF.Min(bound1, bound2), MathF.Max(bound1, bound2));
    }

    public readonly MobiusTransformation TaylorShift(float shift)
    {
        // (a(x+s)+b)/(c(x+s)+d) = (ax + b+as) / (cx + d+cs)
        return new MobiusTransformation(
            NumeratorCoefficient,
            NumeratorConstant + shift * NumeratorCoefficient,
            DenominatorCoefficient,
            DenominatorConstant + shift * DenominatorCoefficient);
    }

    /// <summary>
    /// Apply M(x):=M(1/x)
    /// </summary>
    /// <returns></returns>
    public MobiusTransformation ReciprocalInput()
    {
        // M(x) := (ax+b)/(cx+d)
        // M(1/x) = (a(1/x)+b)/(c(1/x)+d) = (bx+a)/(dx+c)
        return new MobiusTransformation(
            NumeratorConstant,
            NumeratorCoefficient,
            DenominatorConstant,
            DenominatorCoefficient);
    }

    public readonly MobiusTransformation Scale(float factor)
    {
        return new MobiusTransformation(
            NumeratorCoefficient * factor,
            NumeratorConstant,
            DenominatorCoefficient * factor,
            DenominatorConstant);
    }
}
