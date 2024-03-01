namespace NonstandardPhysicsSolver.Polynomials;

public struct MobiusTransformation
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

        this.NumeratorCoefficient = numeratorCoefficient;
        this.NumeratorConstant = numeratorConstant;
        this.DenominatorCoefficient = denominatorCoefficient;
        this.DenominatorConstant = denominatorConstant;
    }

    /// <summary>
    /// Constructor to copy a Mobius transformation
    /// </summary>
    /// <param name="mobius"></param>
    public MobiusTransformation(MobiusTransformation mobius)
    {
        this.NumeratorCoefficient = mobius.NumeratorCoefficient;
        this.NumeratorConstant = mobius.NumeratorConstant;
        this.DenominatorCoefficient = mobius.DenominatorCoefficient;
        this.DenominatorConstant = mobius.DenominatorConstant;
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

    public readonly float EvaluateAt(float x)
    {
        return (NumeratorCoefficient * x + NumeratorConstant) / (DenominatorCoefficient * x + DenominatorConstant);
    }

    public readonly (float leftBound, float rightBound) PositiveDomainImage()
    {
        float leftBound = MathF.Min(NumeratorCoefficient / DenominatorCoefficient, NumeratorConstant / DenominatorConstant);
        float rightBound = MathF.Max(NumeratorCoefficient / DenominatorCoefficient, NumeratorConstant / DenominatorConstant);
        return (leftBound, rightBound);
    }

    public readonly MobiusTransformation TaylorShift(float shift)
    {
        return new MobiusTransformation(
            NumeratorCoefficient + shift * DenominatorCoefficient,
            NumeratorConstant + shift * DenominatorConstant,
            DenominatorCoefficient,
            DenominatorConstant);
    }

    public MobiusTransformation Invert()
    {
        return new MobiusTransformation(
            DenominatorCoefficient,
            DenominatorConstant,
            NumeratorCoefficient,
            NumeratorConstant);
    }

    public readonly MobiusTransformation Scale(float factor)
    {
        return new MobiusTransformation(
            NumeratorCoefficient * factor,
            NumeratorConstant * factor,
            DenominatorCoefficient,
            DenominatorConstant);
    }
}
