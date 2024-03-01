
namespace NonstandardPhysicsSolver.Polynomials;

public partial struct Polynomial
{
    /// <summary>
    /// Take a squarefree polynomial and find a list of intervals each containing a single root.
    /// </summary>
    /// <returns>A list of (float leftBound, float rightBound) tuples as intervals each containing a single root.</returns>
    public List<(float leftBound, float rightBound)> IsolateRootIntervalsContinuedFractions()
    {
        // Initial Möbius transformation setup as the identity function: M(x) = x = (1x + 0) / (0x + 1)
        var identityFunction = new MobiusTransformation(1, 0, 0, 1);
        List<(float leftBound, float rightBound)> rootIntervals = [];
        ContinuedFractions(this.MakeSquareFree(), identityFunction, ref rootIntervals);
        return rootIntervals;

        // Takes in a squarefree polynomial and a Mobius transformation
        static void ContinuedFractions(Polynomial polynomial, MobiusTransformation mobius, ref List<(float, float)> rootIntervals)
        {
            if (MathF.Abs(polynomial.Coefficients[0]) < float.Epsilon)
            {
                rootIntervals.Add((mobius.EvaluateAt(0), mobius.EvaluateAt(0)));
                polynomial = polynomial.ShiftCoefficientsBy1(); // Properly handle the root at 0
                ContinuedFractions(polynomial, mobius, ref rootIntervals);
                return; // Early return after handling root at 0
            }

            int signVariations = polynomial.CountSignVariations();
            if (signVariations == 0) return; // No roots, exit recursion
            if (signVariations == 1)
            {
                rootIntervals.Add(mobius.PositiveDomainImage()); // Exactly one root, add interval
                return; // Exit recursion after adding interval
            }

            // Proceed with further isolation only if necessary
            float calculatedPositiveLowerBound = polynomial.LMQPositiveLowerBound();
            if (calculatedPositiveLowerBound >= 1)
            {
                polynomial = polynomial.TaylorShiftQuadratic(calculatedPositiveLowerBound);
                mobius = mobius.TaylorShift(calculatedPositiveLowerBound);
            }

            // Split the interval and proceed with recursion on each part
            // Right interval ]1, +inf[
            var polynomial1ToInf = polynomial.TaylorShiftQuadratic(1);
            var mobius1ToInf = mobius.TaylorShift(1);
            ContinuedFractions(polynomial1ToInf, mobius1ToInf, ref rootIntervals);

            // Left interval ]0, 1]
            if (polynomial1ToInf.CountSignVariations() < signVariations)
            {
                var polynomial0To1 = polynomial1ToInf.Reversed();
                var mobius0To1 = mobius1ToInf.Invert();
                if (MathF.Abs(polynomial0To1.Coefficients[0]) < float.Epsilon)
                {
                    polynomial0To1 = polynomial0To1.ShiftCoefficientsBy1();
                }
                ContinuedFractions(polynomial0To1, mobius0To1, ref rootIntervals);
            }
        }
    }

    /// <summary>
    /// Counts the number of sign variations in the polynomial's coefficients.
    /// </summary>
    /// <returns>The number of sign variations in the polynomial's coefficients.</returns>
    public readonly int CountSignVariations()
    {
        int signVariations = 0;
        int previousSign = 0; // 0 indicates no sign determined yet

        foreach (var coefficient in Coefficients)
        {
            // Skip if coefficient is zero
            if (coefficient == 0) continue;

            int currentSign = MathF.Sign(coefficient);

            // If previousSign has been set and currentSign differs, increase count
            if (previousSign != 0 && currentSign != previousSign)
            {
                signVariations++;
            }

            previousSign = currentSign;
        }

        return signVariations;
    }
}