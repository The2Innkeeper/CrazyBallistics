namespace NonstandardPhysicsSolver.Polynomials;

using NonstandardPhysicsSolver.Intervals;
using NonstandardPhysicsSolver.Intervals.Float;

public partial struct PolynomialFloat
{
    /// <summary>
    /// Take a polynomial and find a list of intervals each containing a single real root using Bisection and Descartes' Rule of Signs.
    /// </summary>
    /// <returns>A list of (float leftBound, float rightBound) tuples as intervals each containing a single real root.</returns>
    public List<Interval> IsolatePositiveRootIntervalsBisection(int maxIterations = 50)
    {
        // Validate input
        CheckEmptyCoefficients(Coefficients);
        ValidateCoefficientsForNaN(Coefficients);

        // Process polynomial
        PolynomialFloat squarefreePolynomial = this.MakeSquarefree();
        float positiveUpperBound = squarefreePolynomial.LMQPositiveUpperBound();
        PolynomialFloat horizontallyShrunkenPolynomial = squarefreePolynomial.ScaleInput(positiveUpperBound);
        MobiusTransformation initialMobius = new MobiusTransformation(positiveUpperBound, 0f, 0f, 1f);
        int initialSignVariationCount = horizontallyShrunkenPolynomial.CountSignVariations();

        var tasks = new Queue<(PolynomialFloat polynomial, MobiusTransformation mobius, int signVariationCount)>();
        var isolatingIntervals = new List<Interval>();

        tasks.Enqueue((horizontallyShrunkenPolynomial, initialMobius, initialSignVariationCount));

        for (int iteration = 0; iteration < maxIterations && tasks.Count > 0; iteration++)
        {
            if (iteration == maxIterations) return isolatingIntervals.Count == 0 ? new List<Interval>() : isolatingIntervals;

            var (currentPolynomial, currentMobius, currentSignVariationCount) = tasks.Dequeue();

            switch (currentSignVariationCount)
            {
                case 0:
                    // No roots in this interval
                    break;
                case 1:
                    // One root in this interval, add to the list
                    AddIsolatingInterval(ref isolatingIntervals, currentMobius, squarefreePolynomial);
                    break;
                default:
                    // More than one root, split the interval and add both halves to the queue
                    // Check midpoint for root
                    float originalMidpoint = currentMobius.EvaluateAt(0.5f);
                    int foundRootAtMidpoint = 0;
                    if (horizontallyShrunkenPolynomial.EvaluatePolynomialAccurate(originalMidpoint) == 0)
                    {
                        AddIntervalWithoutDuplicates(ref isolatingIntervals, new Interval(originalMidpoint, originalMidpoint));
                        foundRootAtMidpoint = 1;
                    }

                    // Left half
                    PolynomialFloat leftHalfPolynomial = currentPolynomial.ScaleInputInReverseOrder(2f);
                    int leftHalfVariations = leftHalfPolynomial.CountSignVariationsInUnitInterval();
                    MobiusTransformation leftMobius = currentMobius.ScaleInput(0.5f);
                    if (leftHalfVariations == 1)
                    {
                        AddIsolatingInterval(ref isolatingIntervals, leftMobius, squarefreePolynomial);
                    }
                    else if (leftHalfVariations > 1)
                    {
                        tasks.Enqueue((leftHalfPolynomial, leftMobius, leftHalfVariations));
                    }

                    // Right half
                    int rightHalfVariations = currentSignVariationCount - foundRootAtMidpoint - leftHalfVariations;
                    if (rightHalfVariations == 0)
                    {
                        break;
                    }
                    PolynomialFloat rightHalfPolynomial = leftHalfPolynomial.TaylorShiftBy1();
                    MobiusTransformation rightMobius = leftMobius.TaylorShiftBy1();
                    if (rightHalfVariations == 1)
                    {
                        AddIsolatingInterval(ref isolatingIntervals, rightMobius, squarefreePolynomial);
                    }
                    else if (rightHalfVariations > 1)
                    {
                        tasks.Enqueue((rightHalfPolynomial, rightMobius, rightHalfVariations));
                    }

                    break;
            }
        }

        return isolatingIntervals;

        static void AddIsolatingInterval(ref List<Interval> isolatedRootIntervals, MobiusTransformation mobius, PolynomialFloat initialPolynomial)
        {
            Interval mobiusImage = mobius.UnitIntervalImage();
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

    /// <summary>
    /// Take a polynomial and find a list of intervals each containing a single root.
    /// </summary>
    /// <returns>A list of (float leftBound, float rightBound) tuples as intervals each containing a single root.</returns>
    public List<Interval> IsolatePositiveRootIntervalsContinuedFractions(int maxIterations = 50)
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
        PolynomialFloat squarefreePolynomial = this.MakeSquarefree();
        int initialSignVariationCount = squarefreePolynomial.CountSignVariations();
        var tasks = new Queue<(PolynomialFloat polynomial, MobiusTransformation mobius, int signVariationCount)>();
        tasks.Enqueue((squarefreePolynomial, MobiusTransformation.Identity(), initialSignVariationCount));
        var isolatedRootIntervals = new List<Interval>();


        for (int iteration = 0; iteration < maxIterations && tasks.Count > 0; iteration++)
        {
            if (iteration == maxIterations) return isolatedRootIntervals.Count == 0 ? new List<Interval>() : isolatedRootIntervals;

            (PolynomialFloat currentPolynomial, MobiusTransformation currentMobius, int variationCount0ToInf) = tasks.Dequeue();

            // Handle edge cases
            // Empty coefficients
            CheckEmptyCoefficients(currentPolynomial.Coefficients);
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
            PolynomialFloat polynomial1ToInf = currentPolynomial.TaylorShiftBy1();
            MobiusTransformation mobius1ToInf = currentMobius.TaylorShiftBy1();

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
            PolynomialFloat polynomial0To1 = currentPolynomial.TransformedForLowerInteval(1);
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

        static bool HasPositiveRoots(float[] coefficients)
        {
            if (!coefficients.Any(coeff => coeff <= 0)) // If all coefficients are strictly positive, there will be no positive roots
            {
                return false;
            }
            return true;
        }

        static bool HandleZeroFunction(ref List<Interval> isolatedRootIntervals, PolynomialFloat polynomial)
        {
            if (!(polynomial.Coefficients.Length == 1 && polynomial.Coefficients[0] == 0)) { return false; }

            AddIntervalWithoutDuplicates(ref isolatedRootIntervals, new Interval(0, float.PositiveInfinity));
            return true;
        }

        static void AdjustForLowerBound(ref PolynomialFloat polynomial, ref MobiusTransformation mobius, float lowerBound)
        {
            if (!(lowerBound >= 1)) { return; }

            PolynomialFloat transformedPolynomial = polynomial.ScaleInput(lowerBound).TaylorShiftBy1();
            MobiusTransformation transformedMobius = mobius.ScaleInput(lowerBound).TaylorShiftBy1();
            polynomial = transformedPolynomial;
            mobius = transformedMobius;
        }

        static bool CheckAndHandleRootAtZero(ref List<Interval> intervals, ref PolynomialFloat polynomial, MobiusTransformation mobius)
        {
            if (polynomial.Coefficients[0] != 0) return false;

            float root = mobius.EvaluateAt(0f);
            AddIntervalWithoutDuplicates(ref intervals, new Interval(root, root));
            polynomial = polynomial.ShiftCoefficientsBy1();
            return true;
        }

        static void AddMobiusIntervalAdjusted(ref List<Interval> isolatedRootIntervals, MobiusTransformation mobius, PolynomialFloat initialPolynomial)
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

    private static void CheckEmptyCoefficients(float[] coefficients)
    {
        // Validate input
        if (coefficients == null || coefficients.Length == 0)
        {
            throw new ArgumentException("The coefficients list cannot be null or empty.");
        }
    }

    private static void ValidateCoefficientsForNaN(float[] coefficients)
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

    /// <summary>
    /// Compute Descartes's rule of signs number with a transformed polynomial's coefficients
    /// whose subdomain ]0,1[ has been mapped to ]0,+inf[.
    /// </summary>
    /// <returns>Descarte's rules of signs number for the unit interval ]0,1[.</returns>
    public int CountSignVariationsInUnitInterval()
    {
        return this.MapUnitIntervalToPositiveReals().CountSignVariations();
    }

    /// <summary>
    /// Compute Descartes's rule of signs number with a transformed polynomial's coefficients
    /// whose subdomain ]a,b[ has been mapped to ]0,+inf[.
    /// </summary>
    /// <param name="interval"></param>
    /// <returns>Descartes's rule of signs number for the interval ]a,b[.</returns>
    public int CountSignVariationsInInterval(Interval interval)
    {
        return this.MapIntervalToPositiveReals(interval).CountSignVariations();
    }

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