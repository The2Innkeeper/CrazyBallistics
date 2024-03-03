namespace NonstandardPhysicsSolver.Polynomials;

using NonstandardPhysicsSolver.Intervals;

public partial struct Polynomial
{
    /// <summary>
    /// Take a polynomial and find a list of intervals each containing a single root.
    /// </summary>
    /// <returns>A list of (float leftBound, float rightBound) tuples as intervals each containing a single root.</returns>
    public List<Interval> IsolateRootIntervalsContinuedFractions()
    {
        var tasks = new Queue<(Polynomial polynomial, MobiusTransformation mobius)>();
        tasks.Enqueue((this.MakeSquareFree(), new MobiusTransformation(1, 0, 0, 1)));
        var isolatedRootIntervals = new List<Interval>();

        int coefficientCount = Coefficients.Length;
        // Validate input
        if (Coefficients == null || coefficientCount == 0)
        {
            throw new ArgumentException("The coefficients list cannot be null or empty.");
        }
        if (!Coefficients.Any(coeff => coeff <= 0)) // If all coefficients are strictly positive, there will be no positive roots
        {
            return [];
        }

        while (tasks.Count > 0)
        {
            (Polynomial currentPolynomial, MobiusTransformation currentMobius) = tasks.Dequeue();

            // Empty coefficients
            if (currentPolynomial.Coefficients.Length == 0) { continue; }
            // Constant function only has zeroes if it is the zero function
            if (currentPolynomial.Coefficients.Length == 1 && currentPolynomial.Coefficients[0] == 0)
            {
                AddIntervalWithoutDuplicates(ref isolatedRootIntervals, 0, float.PositiveInfinity);
                continue;
            }

            // Check for NaN in coefficients before proceeding
            for (int i = 0; i < currentPolynomial.Coefficients.Length; i++)
            {
                if (float.IsNaN(currentPolynomial.Coefficients[i]))
                {
                    // Log the detection of NaN
                    Console.WriteLine($"NaN detected in polynomial coefficients at index {i}.");
                    // Throw an exception or handle it as necessary
                    throw new ArgumentException($"NaN detected in polynomial coefficients at index {i}." +
                        $"\nPolynomial coefficients: {string.Join(", ", currentPolynomial.Coefficients)}");
                }
            }

            // Divide by x if needed (input polynomial is squarefree)
            if (currentPolynomial.Coefficients[0] == 0)
            {
                currentPolynomial = currentPolynomial.ShiftCoefficientsBy1();
                AddIntervalWithoutDuplicates(ref isolatedRootIntervals, 0f, 0f);
            }


            // Main algorithm
            int signVariationCount = currentPolynomial.CountSignVariations();
            if (signVariationCount == 0) continue; // No roots, exit
            if (signVariationCount == 1)
            {
                Interval mobiusImage = currentMobius.PositiveDomainImage();
                if (mobiusImage.RightBound == float.PositiveInfinity)
                {
                    float updatedRightBound = currentPolynomial.LMQPositiveUpperBound();
                    AddIntervalWithoutDuplicates(ref isolatedRootIntervals, mobiusImage.LeftBound, updatedRightBound); // Exactly one root, add interval
                    continue;
                }

                AddIntervalWithoutDuplicates(ref isolatedRootIntervals, mobiusImage.LeftBound, mobiusImage.RightBound); // Exactly one root, add interval
                continue; // Exit after adding interval
            }

            float lowerBound = currentPolynomial.LMQPositiveLowerBound(); // Implement this based on your strategy
            if (lowerBound >= 1)
            {
                currentPolynomial = currentPolynomial.TaylorShift(lowerBound);
                currentMobius = currentMobius.TaylorShift(lowerBound);
            }

            Polynomial polynomial1ToInf = currentPolynomial.TaylorShift(1);
            MobiusTransformation mobius1ToInf = currentMobius.TaylorShift(1);

            //polynomial1ToInf = currentPolynomial.TransformedForLowerInteval(lowerBound);
            //mobius1ToInf = currentMobius.TransformedForLowerInterval(lowerBound);

            if (polynomial1ToInf.Coefficients[0] < float.Epsilon) // Check for a root at x = 0 after the shift
            {
                // This code is causing duplicates, I will try removing it
                //float root = mobius1ToInf.EvaluateAt(0);
                //AddIntervalWithoutDuplicates(ref isolatedRootIntervals, root, root);
                polynomial1ToInf = polynomial1ToInf.ShiftCoefficientsBy1(); // Implement method to divide by x and reduce degree
            }

            // Search for roots in ] M(1), M(+inf) [, which we practically reduce to ] M(1), M(upperBound) [ if needed
            tasks.Enqueue((polynomial1ToInf, mobius1ToInf));

            int signVariationCountChange = signVariationCount - polynomial1ToInf.CountSignVariations();
            if (signVariationCountChange == 0) { continue; }
            if (signVariationCountChange == 1)
            {
                AddIntervalWithoutDuplicates(ref isolatedRootIntervals, currentMobius.EvaluateAt(0), currentMobius.EvaluateAt(lowerBound));
                continue;
            }

            if (!(signVariationCountChange > 1)) continue;
            // Roots are possibly in the interval before the shift ] M(0), M(1) [
            // x := 1/(1+x)
            Polynomial polynomial0To1 = currentPolynomial.TransformedForLowerInteval(1);
            MobiusTransformation mobius0To1 = currentMobius.TransformedForLowerInterval(1);
            if (polynomial0To1.Coefficients[0] < float.Epsilon) polynomial0To1 = polynomial0To1.ShiftCoefficientsBy1();
            // x := b/(1+x)
            //Polynomial polynomial0To1 = currentPolynomial.TransformedForLowerInteval(lowerBound);
            //MobiusTransformation mobius0To1 = currentMobius.TransformedForLowerInterval(lowerBound);
            tasks.Enqueue((polynomial0To1, mobius0To1));
        }

        return isolatedRootIntervals;

        // New method to check and add intervals without duplicates
        static void AddIntervalWithoutDuplicates(ref List<Interval> intervals, float leftBound, float rightBound, float tolerance = 1e-16f)
        {
            bool isDuplicate = intervals.Any(interval => MathF.Abs(interval.LeftBound - leftBound) < tolerance &&
                                                          MathF.Abs(interval.RightBound - rightBound) < tolerance);
            if (!isDuplicate)
            {
                intervals.Add(new Interval(leftBound, rightBound));
            }
        }
    }

    /*
    /// <summary>
    /// Take a polynomial and find a list of intervals each containing a single root.
    /// </summary>
    /// <returns>A list of (float leftBound, float rightBound) tuples as intervals each containing a single root.</returns>
    public List<Interval> IsolateRootIntervalsContinuedFractions()
    {
        // Initiate the input polynomial to be squarefree
        // Initiate Möbius transformation setup as the identity function: M(x) = x = (1x + 0) / (0x + 1)
        var identityFunction = new MobiusTransformation(1, 0, 0, 1);
        List<Interval> rootIntervals = [];
        ContinuedFractions(this.MakeSquareFree(), identityFunction, ref rootIntervals);
        return rootIntervals;

        // Takes in a squarefree polynomial and a Mobius transformation
        static void ContinuedFractions(Polynomial polynomial, MobiusTransformation mobius, ref List<Interval> rootIntervals)
        {
            // Empty coefficients
            if (polynomial.Coefficients.Length == 0) { return; }
            // Constant function only has zeroes if it is the zero function
            if (polynomial.Coefficients.Length == 1 && polynomial.Coefficients[0] == 0)
            {
                AddIntervalWithoutDuplicates(ref rootIntervals, new Interval(0, float.PositiveInfinity));
                return;
            }

            // Check for NaN in coefficients before proceeding
            for (int i = 0; i < polynomial.Coefficients.Length; i++)
            {
                if (float.IsNaN(polynomial.Coefficients[i]))
                {
                    // Log the detection of NaN
                    Console.WriteLine($"NaN detected in polynomial coefficients at index {i}.");
                    // Throw an exception or handle it as necessary
                    throw new ArgumentException($"NaN detected in polynomial coefficients at index {i}." +
                        $"\nPolynomial coefficients: {string.Join(", ", polynomial.Coefficients)}");
                }
            }

            // Main algorithm
            // If constant term is zero
            if (MathF.Abs(polynomial.Coefficients[0]) < float.Epsilon)
            {
                // Add M(0) as root
                AddIntervalWithoutDuplicates(ref rootIntervals, new Interval(mobius.EvaluateAt(0), mobius.EvaluateAt(0)));
                // Divide by x
                polynomial = polynomial.ShiftCoefficientsBy1();
                // 
                ContinuedFractions(polynomial, mobius, ref rootIntervals);
                return; // Early return after handling root at 0
            }

            int signVariations = polynomial.CountSignVariations();
            if (signVariations == 0) return; // No roots, exit recursion
            if (signVariations == 1)
            {
                Interval mobius_image = mobius.PositiveDomainImage();
                if (mobius_image.RightBound == float.PositiveInfinity)
                {
                    mobius_image = new Interval(mobius_image.LeftBound, polynomial.LMQPositiveUpperBound());
                }

                AddIntervalWithoutDuplicates(ref rootIntervals, mobius_image); // Exactly one root, add interval
                return; // Exit recursion after adding interval
            }

            // Proceed with further isolation only if necessary
            float calculatedPositiveLowerBound = polynomial.LMQPositiveLowerBound();
            if (calculatedPositiveLowerBound >= 1)
            {
                polynomial = polynomial.TaylorShift(calculatedPositiveLowerBound);
                mobius = mobius.TaylorShift(calculatedPositiveLowerBound);
            }

            // Split the interval and proceed with recursion on each part
            // Right interval ]1, +inf[
            var polynomial1ToInf = polynomial.TaylorShift(1);
            var mobius1ToInf = mobius.TaylorShift(1);
            ContinuedFractions(polynomial1ToInf, mobius1ToInf, ref rootIntervals);

            // Left interval ]0, 1]
            if (!(polynomial1ToInf.CountSignVariations() < signVariations)) { return; } // Only proceed if sign variations changed

            var polynomial0To1 = polynomial1ToInf.Reversed();
            var mobius0To1 = mobius1ToInf.Invert();
            if (MathF.Abs(polynomial0To1.Coefficients[0]) < float.Epsilon)
            {
                polynomial0To1 = polynomial0To1.ShiftCoefficientsBy1();
            }
            ContinuedFractions(polynomial0To1, mobius0To1, ref rootIntervals);

            // New method to check and add intervals without duplicates
            static void AddIntervalWithoutDuplicates(ref List<Interval> intervals, Interval newInterval, float tolerance = 1e-8f)
            {
                bool isDuplicate = intervals.Any(interval => MathF.Abs(interval.LeftBound - newInterval.LeftBound) < tolerance &&
                                                              MathF.Abs(interval.RightBound - newInterval.RightBound) < tolerance);
                if (!isDuplicate)
                {
                    intervals.Add(newInterval);
                }
            }
        }
    }
    */

    /// <summary>
    /// Counts the number of sign variations in the polynomial's coefficients.
    /// </summary>
    /// <returns>The number of sign variations in the polynomial's coefficients.</returns>
    public readonly int CountSignVariations()
    {
        int signVariations = 0;
        int previousSign = 0; // 0 indicates no sign determined yet

        for (int i = 0; i < Coefficients.Length; i++) // Use for loop to have an index
        {
            float coefficient = Coefficients[i];

            // Skip if coefficient is zero
            if (coefficient == 0) continue;

            if (float.IsNaN(coefficient))
            {
                throw new ArgumentException($"NaN detected in coefficient at index {i}." +
                    $"\nPolynomial coefficients: {string.Join(", ", Coefficients)}");
            }

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