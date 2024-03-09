namespace NonstandardPhysicsSolver.Intervals;

public struct MobiusDouble
{
    public double NumeratorCoefficient, NumeratorConstant, DenominatorCoefficient, DenominatorConstant;

    /// <summary>
    /// Applies the transformation Mobius(x) := (ax+b)/(cx+d).
    /// </summary>
    /// <param name="numeratorCoefficient"></param>
    /// <param name="numeratorConstant"></param>
    /// <param name="denominatorCoefficient"></param>
    /// <param name="denominatorConstant"></param>
    /// <exception cref="ArgumentException"></exception>

    public MobiusDouble(double numeratorCoefficient, double numeratorConstant, double denominatorCoefficient, double denominatorConstant)
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
    public MobiusDouble(MobiusDouble mobius)
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
    public static MobiusDouble Identity()
    {
        return new MobiusDouble(
            1f,
            0f,
            0f,
            1f);
    }

    /// <summary>
    /// Apply the transformation x := 1 /(x + 1)
    /// </summary>
    public MobiusDouble ProcessUnitInterval()
    {
        return new MobiusDouble(
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
    /// <returns>A new MobiusDouble adjusted for the lower interval.</returns>
    public MobiusDouble TransformedForLowerInterval(double shift)
    {
        double newA = NumeratorConstant; // a becomes b
        double newB = NumeratorConstant + shift * NumeratorCoefficient; // b becomes as + b
        double newC = DenominatorConstant; // c becomes d
        double newD = DenominatorConstant + shift * DenominatorCoefficient; // d becomes cs + d

        return new MobiusDouble(newA, newB, newC, newD);
    }

    public double EvaluateAt(double x)
    {
        double denominator = DenominatorCoefficient * x + DenominatorConstant;
        if (Math.Abs(denominator) < double.Epsilon)
        {
            // Handle division by zero as per your application's context
            // For example, return double.PositiveInfinity or double.NegativeInfinity based on the sign of the numerator
            double numerator = NumeratorCoefficient * x + NumeratorConstant;
            if (numerator > 0) return double.PositiveInfinity;
            if (numerator < 0) return double.NegativeInfinity;
        }
        return (NumeratorCoefficient * x + NumeratorConstant) / denominator;
    }

    public IntervalDouble PositiveDomainImage()
    {
        if (DenominatorCoefficient == 0 && DenominatorConstant == 0) return new IntervalDouble(0f, double.PositiveInfinity);
        double bound1, bound2;
        bound1 = NumeratorConstant / DenominatorConstant;
        bound2 = NumeratorCoefficient / DenominatorCoefficient;

        return new IntervalDouble(Math.Min(bound1, bound2), Math.Max(bound1, bound2));
    }

    /// <summary>
    /// Maps the unit interval back to its original form through the Mobius transformation.
    /// a = Min(M(0),M(1))
    /// b = Max(M(0),M(1))
    /// </summary>
    /// <returns>The interval with bounds M(0) and M(1), in left to right (increasing) order.</returns>
    public IntervalDouble UnitIntervalImage()
    {
        if (DenominatorCoefficient == 0 && DenominatorConstant == 0) return new IntervalDouble(0f, double.PositiveInfinity);

        double bound1, bound2;
        // M(0) = b/d
        bound1 = NumeratorConstant / DenominatorConstant;
        // M(1) = (a+b)/(c+d)
        bound2 = (NumeratorCoefficient + NumeratorConstant) / (DenominatorCoefficient + DenominatorConstant);

        return new IntervalDouble(Math.Min(bound1, bound2), Math.Max(bound1, bound2));
    }

    public MobiusDouble TaylorShiftBy1()
    {
        // (a(x+s)+b)/(c(x+s)+d) = (ax + b+as) / (cx + d+cs)
        // Since s = 1 then
        // = (ax + a+b) / (cx + c+d)
        return new MobiusDouble(
            NumeratorCoefficient,
            NumeratorConstant + NumeratorCoefficient,
            DenominatorCoefficient,
            DenominatorConstant + DenominatorCoefficient);
    }

    public MobiusDouble TaylorShift(double shift)
    {
        // (a(x+s)+b)/(c(x+s)+d) = (ax + b+as) / (cx + d+cs)
        return new MobiusDouble(
            NumeratorCoefficient,
            NumeratorConstant + shift * NumeratorCoefficient,
            DenominatorCoefficient,
            DenominatorConstant + shift * DenominatorCoefficient);
    }

    /// <summary>
    /// Apply M(x):=M(1/x)
    /// </summary>
    /// <returns></returns>
    public MobiusDouble ReciprocalInput()
    {
        // M(x) := (ax+b)/(cx+d)
        // M(1/x) = (a(1/x)+b)/(c(1/x)+d) = (bx+a)/(dx+c)
        return new MobiusDouble(
            NumeratorConstant,
            NumeratorCoefficient,
            DenominatorConstant,
            DenominatorCoefficient);
    }

    public MobiusDouble ScaleInput(double factor)
    {
        return new MobiusDouble(
            NumeratorCoefficient * factor,
            NumeratorConstant,
            DenominatorCoefficient * factor,
            DenominatorConstant);
    }
}
