namespace NonstandardPhysicsSolver.Polynomials;

using NonstandardPhysicsSolver.Intervals;

public partial struct Polynomial
{
    /// <summary>
    /// Take a polynomial and find a list of intervals each containing a single root.
    /// </summary>
    /// <returns>A list of (float leftBound, float rightBound) tuples as intervals each containing a single root.</returns>
    public List<Interval> IsolatePositiveRootIntervalsContinuedFractions()
    {
        // Validate input
        CheckEmptyCoefficients(Coefficients);
        if (!HasPositiveRoots(Coefficients)) return [];

        /* Initialize:
         * P(x) = SquareFree(P(x))
         * M(x) = x = (1x+0)/(0x+1)
         * varCount = Var(P)
         * rootIntervals = { (P(x), M(x), varCount) }
         */
        int initialSignVariationCount = this.CountSignVariations();
        var tasks = new Queue<(Polynomial polynomial, MobiusTransformation mobius, int signVariationCount)>();
        tasks.Enqueue((this.MakeSquareFree(), MobiusTransformation.Identity(), initialSignVariationCount));
        var isolatedRootIntervals = new List<Interval>();


        while (tasks.Count > 0)
        {
            (Polynomial currentPolynomial, MobiusTransformation currentMobius, int variationCount0ToInf) = tasks.Dequeue();

            // Handle edge cases
            // Empty coefficients
            if (currentPolynomial.Coefficients.Length == 0) { continue; }
            // Constant function only has zeroes if it is the zero function
            if (HandleZeroFunction(ref isolatedRootIntervals, currentPolynomial)) { break; }
            // Check for NaN in coefficients before proceeding
            ValidateCoefficientsForNaN(currentPolynomial.Coefficients);


            // Main algorithm
            float lowerBound = currentPolynomial.LMQPositiveLowerBound(); // Implement this based on your strategy
            AdjustForLowerBound(ref currentPolynomial, ref currentMobius, lowerBound);

            // Divide by x if needed (input polynomial is squarefree so only once is OK)
            CheckAndHandleRootAtZero(ref isolatedRootIntervals, ref currentPolynomial, currentMobius);

            if (variationCount0ToInf == 0) continue; // No roots, exit
            if (variationCount0ToInf == 1) // 1 root, add to isolated and exit
            {
                AddMobiusIntervalAdjusted(ref isolatedRootIntervals, currentMobius, this);
                continue;
            }
            // If var(P) > 1 then proceed to splitting into ]0,1[, [1,1] and ]1,+inf[

            // Starting with ]1,+inf[ because it only requires a Taylor shift, compared to
            // ]0,1[ where it needs a Taylor shift + reversion
            // ]1,+inf[
            // x:= x+1
            Polynomial polynomial1ToInf = currentPolynomial.TaylorShift(1);
            MobiusTransformation mobius1ToInf = currentMobius.TaylorShift(1);

            // [1,1]
            int foundRootAt1 = 0;
            foundRootAt1 += CheckAndHandleRootAtZero(ref isolatedRootIntervals, ref polynomial1ToInf, mobius1ToInf) ? 1 : 0;

            int variationCount1ToInf = polynomial1ToInf.CountSignVariations();
            if (variationCount1ToInf == 1)
            {
                AddMobiusIntervalAdjusted(ref isolatedRootIntervals, mobius1ToInf, this);
            }
            else if (variationCount1ToInf > 1)
            {
                tasks.Enqueue((polynomial1ToInf, mobius1ToInf, variationCount1ToInf));
            }

            // ]0, 1[
            int variationCount0To1 = variationCount0ToInf - variationCount1ToInf - foundRootAt1;
            if (variationCount0To1 == 0) { continue; } // No roots in this interval, avoid extra computation
            // x := 1/(1+x)
            Polynomial polynomial0To1 = currentPolynomial.TransformedForLowerInteval(1);
            MobiusTransformation mobius0To1 = currentMobius.TransformedForLowerInterval(1);
            if (variationCount0To1 == 1)
            {
                AddMobiusIntervalAdjusted(ref isolatedRootIntervals, mobius0To1, this);
                continue;
            }

            if (polynomial0To1.Coefficients[0] < float.Epsilon) polynomial0To1 = polynomial0To1.ShiftCoefficientsBy1();
            tasks.Enqueue((polynomial0To1, mobius0To1, variationCount0To1));
        }

        return isolatedRootIntervals;

        static void CheckEmptyCoefficients(float[] coefficients)
        {
            // Validate input
            if (coefficients == null || coefficients.Length == 0)
            {
                throw new ArgumentException("The coefficients list cannot be null or empty.");
            }
        }

        static bool HasPositiveRoots(float[] coefficients)
        {
            if (!coefficients.Any(coeff => coeff <= 0)) // If all coefficients are strictly positive, there will be no positive roots
            {
                return false;
            }
            return true;
        }

        static void ValidateCoefficientsForNaN(float[] coefficients)
        {
            for (int i = 0; i < coefficients.Length; i++)
            {
                if (float.IsNaN(coefficients[i]))
                {
                    // Log the detection of NaN
                    Console.WriteLine($"NaN detected in polynomial coefficients at index {i}.");
                    // Throw an exception or handle it as necessary
                    throw new ArgumentException($"NaN detected in polynomial coefficients at index {i}." +
                        $"\nPolynomial coefficients: {string.Join(", ", coefficients)}");
                }
            }
        }

        static bool HandleZeroFunction(ref List<Interval> isolatedRootIntervals, Polynomial polynomial)
        {
            if (!(polynomial.Coefficients.Length == 1 && polynomial.Coefficients[0] == 0)) { return false; }

            AddIntervalWithoutDuplicates(ref isolatedRootIntervals, new Interval(0, float.PositiveInfinity));
            return true;
        }

        static void AdjustForLowerBound(ref Polynomial polynomial, ref MobiusTransformation mobius, float lowerBound)
        {
            if (!(lowerBound >= 1)) { return; }

            Polynomial transformedPolynomial = polynomial.ScaleInput(lowerBound).TaylorShift(1);
            MobiusTransformation transformedMobius = mobius.ScaleInput(lowerBound).TaylorShift(1);
            polynomial = transformedPolynomial;
            mobius = transformedMobius;
        }

        static bool CheckAndHandleRootAtZero(ref List<Interval> intervals, ref Polynomial polynomial, MobiusTransformation mobius)
        {
            if (polynomial.Coefficients[0] != 0) return false;

            float root = mobius.EvaluateAt(0f);
            AddIntervalWithoutDuplicates(ref intervals, new Interval(root, root));
            polynomial = polynomial.ShiftCoefficientsBy1();
            return true;
        }

        static void AddMobiusIntervalAdjusted(ref List<Interval> isolatedRootIntervals, MobiusTransformation mobius, Polynomial initialPolynomial)
        {
            Interval mobiusImage = mobius.PositiveDomainImage();
            if (mobiusImage.RightBound == float.PositiveInfinity)
            {
                float updatedRightBound = initialPolynomial.LMQPositiveUpperBound();
                AddIntervalWithoutDuplicates(ref isolatedRootIntervals, new Interval(mobiusImage.LeftBound, updatedRightBound)); // Exactly one root, add interval
                return;
            }

            AddIntervalWithoutDuplicates(ref isolatedRootIntervals, mobiusImage); // Exactly one root, add interval
        }

        // Enhanced method to check and add intervals without duplicates or subintervals
        static void AddIntervalWithoutDuplicates(ref List<Interval> intervals, Interval newInterval)
        {
            for (int i = 0; i < intervals.Count; i++)
            {
                Interval existingInterval = intervals[i];
                bool isDuplicate = existingInterval.LeftBound == newInterval.LeftBound &&
                                    existingInterval.RightBound == newInterval.RightBound;
                if (isDuplicate)
                {
                    return;
                }
                // Subinterval: left bound of must be to the right (larger) and right bound must be to the left (smaller)
                // Note that Budan's theorem works for open-closed intervals ]a,b]
                bool isExistingSubintervalOfNew = existingInterval.LeftBound > newInterval.LeftBound &&
                                                   existingInterval.RightBound <= newInterval.RightBound;
                bool isNewSubintervalOfExisting = newInterval.LeftBound > existingInterval.LeftBound &&
                                                   newInterval.RightBound <= existingInterval.RightBound;

                if (isExistingSubintervalOfNew)
                {
                    // If the existing interval is a subinterval of the new one, no need to add the new interval.
                    return;
                }
                else if (isNewSubintervalOfExisting)
                {
                    // If the new interval is a subinterval of the existing one, replace it as the new one is tighter.
                    intervals.RemoveAt(i);
                    i--; // Adjust the index to reflect the removal
                }
            }

            // If the new interval is not a subinterval nor contains subintervals, add it.
            intervals.Add(newInterval);
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
    public int CountSignVariations()
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