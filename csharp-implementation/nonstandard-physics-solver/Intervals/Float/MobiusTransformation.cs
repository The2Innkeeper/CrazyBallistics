namespace NonstandardPhysicsSolver.Intervals.Float;

public struct MobiusTransformation
{
    public float NumeratorCoefficient, NumeratorConstant, DenominatorCoefficient, DenominatorConstant;

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
        //if (numeratorCoefficient * denominatorConstant == numeratorConstant * denominatorCoefficient)
        //{
        //    throw new ArgumentException("Invalid Möbius transformation parameters: ad should not equal bc.");
        //}

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
    /// M(x):=x=(1x+0)/(0x+1)
    /// </summary>
    /// <returns>The identity Mobius transformation M(x) := x</returns>
    public static MobiusTransformation Identity()
    {
        return new MobiusTransformation(
            1f,
            0f,
            0f,
            1f);
    }

    /// <summary>
    /// Apply the transformation x := 1 /(x + 1)
    /// </summary>
    public MobiusTransformation ProcessUnitInterval()
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
    public MobiusTransformation TransformedForLowerInterval(float shift)
    {
        float newA = NumeratorConstant; // a becomes b
        float newB = NumeratorConstant + shift * NumeratorCoefficient; // b becomes as + b
        float newC = DenominatorConstant; // c becomes d
        float newD = DenominatorConstant + shift * DenominatorCoefficient; // d becomes cs + d

        return new MobiusTransformation(newA, newB, newC, newD);
    }

    public float EvaluateAt(float x)
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

    public Interval PositiveDomainImage()
    {
        if (DenominatorCoefficient == 0 && DenominatorConstant == 0) return new Interval(0f, float.PositiveInfinity);
        float bound1, bound2;
        bound1 = NumeratorConstant / DenominatorConstant;
        bound2 = NumeratorCoefficient / DenominatorCoefficient;

        return new Interval(MathF.Min(bound1, bound2), MathF.Max(bound1, bound2));
    }

    /// <summary>
    /// Maps the unit interval back to its original form through the Mobius transformation.
    /// a = Min(M(0),M(1))
    /// b = Max(M(0),M(1))
    /// </summary>
    /// <returns>The interval with bounds M(0) and M(1), in left to right (increasing) order.</returns>
    public Interval UnitIntervalImage()
    {
        if (DenominatorCoefficient == 0 && DenominatorConstant == 0) return new Interval(0f, float.PositiveInfinity);

        float bound1, bound2;
        // M(0) = b/d
        bound1 = NumeratorConstant / DenominatorConstant;
        // M(1) = (a+b)/(c+d)
        bound2 = (NumeratorCoefficient + NumeratorConstant) / (DenominatorCoefficient + DenominatorConstant);

        return new Interval(MathF.Min(bound1, bound2), MathF.Max(bound1, bound2));
    }

    public MobiusTransformation TaylorShiftBy1()
    {
        // (a(x+s)+b)/(c(x+s)+d) = (ax + b+as) / (cx + d+cs)
        // Since s = 1 then
        // = (ax + a+b) / (cx + c+d)
        return new MobiusTransformation(
            NumeratorCoefficient,
            NumeratorConstant + NumeratorCoefficient,
            DenominatorCoefficient,
            DenominatorConstant + DenominatorCoefficient);
    }

    public MobiusTransformation TaylorShift(float shift)
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

    public MobiusTransformation ScaleInput(float factor)
    {
        return new MobiusTransformation(
            NumeratorCoefficient * factor,
            NumeratorConstant,
            DenominatorCoefficient * factor,
            DenominatorConstant);
    }
}
