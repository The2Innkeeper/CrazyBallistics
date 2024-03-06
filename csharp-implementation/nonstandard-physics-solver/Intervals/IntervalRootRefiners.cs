namespace NonstandardPhysicsSolver.Intervals;

public partial struct Interval
{
    public static float RefineRootIntervalITP(
        Func<float, float> function,
        Interval interval,
        float tolerance = 1e-5f,
        float truncationFactor = float.NaN,
        float truncationExponent = 2f,
        int initialOffset = 0
        )
    {
        return RefineRootIntervalITP(function, interval.LeftBound, interval.RightBound, tolerance, truncationFactor, truncationExponent, initialOffset);
    }

    /// <summary>
    /// (Recommended) Finds a root of the polynomial within the specified interval using an iterative refinement process.
    /// The method combines interpolation, truncation, and projection (ITP) steps to converge towards a single root,
    /// offering superlinear convergence on average and linear convergence in the worst case.
    /// </summary>
    /// <param name="function">The function whose root interval we are trying to refine.</param>
    /// <param name="leftBound">The EXCLUDED left boundary of the interval</param>
    /// <param name="rightBound">The INCLUDED right boundary of the interval</param>
    /// <param name="tolerance">The tolerance for convergence. The method aims to find a root within an interval of size less than or equal to 2 * tol. Default is 0.0005f.</param>
    /// <param name="truncationFactor">A coefficient factor used in the calculation of the truncation step. Default if unchanged is 0.2f / (rightBound - leftBound).</param>
    /// <param name="truncationExponent">An exponent used in the calculation of the truncation step, affecting the interpolation robustness. Default is 2f.</param>
    /// <param name="initialOffset">The initial offset for the maximum number of iterations, contributing to the calculation of the dynamic range for the interpolation step. Default is 1.</param>
    /// <returns>The approximate position of the root within the specified interval, determined to be within the specified tolerance.</returns>
    /// <remarks>
    /// This method employs an iterative technique that refines the interval containing the root by evaluating the polynomial's sign changes.
    /// It dynamically adjusts the interval based on the polynomial's behavior, using interpolation, truncation, and projection steps
    /// to efficiently converge towards the root. The method is designed to work with polynomials where a single root exists within the given interval.
    /// </remarks>
    public static float RefineRootIntervalITP(
        Func<float, float> function,
        float leftBound,
        float rightBound,
        float tolerance = 1e-5f,
        float truncationFactor = float.NaN,
        float truncationExponent = 2f,
        int initialOffset = 0
        )
    {
        // Validate input
        if (leftBound > rightBound) throw new ArgumentException("Left bound must be less than right bound.");
        if (tolerance <= 0) throw new ArgumentException("Tolerance must be positive.");

        // Preprocessing
        float leftBoundValue = function(leftBound);
        if (leftBoundValue == 0)
        {
            leftBound += tolerance;
            leftBoundValue = function(leftBound);
        }
        float rightBoundValue = function(rightBound);
        // Edge cases
        if (rightBoundValue == 0) return rightBound;
        bool doesNotCrossZero = MathF.Sign(leftBoundValue) == MathF.Sign(rightBoundValue);
        if (doesNotCrossZero) throw new ArgumentException($"The initial interval ]{leftBound},{rightBound}] does not contain a single root.");

        // Set k1 = 0.2/(b-a) if not specified
        if (float.IsNaN(truncationFactor))
        {
            truncationFactor = 0.2f / (rightBound - leftBound); // Value determined experimentally
        }

        int nMaxBisections = (int)Math.Ceiling(Math.Log((rightBound - leftBound) / (2f * tolerance), 2));
        int nMaxIterations = nMaxBisections + initialOffset;

        // Main logic: iterate until convergence within tolerance
        for (int iteration = 0; rightBound - leftBound > 2f * tolerance; iteration++)
        {
            // Calculating Parameters
            var (xMidpoint, projectionRadius, truncationRange) = CalculateParameters(leftBound, rightBound, tolerance, truncationFactor, truncationExponent, nMaxIterations, iteration);

            // Interpolation
            float xRegulaFalsi = InterpolateRegulaFalsi(leftBound, rightBound, leftBoundValue, rightBoundValue);

            int perturbationSign = Math.Sign(xMidpoint - xRegulaFalsi); // Get direction for next steps

            // Truncation
            var xTruncated = Truncate(xMidpoint, projectionRadius, perturbationSign, truncationRange);

            // Projection
            float xITP = Project(xTruncated, xMidpoint, projectionRadius, perturbationSign);

            // Update bounds
            float xITPValue = function(xITP);
            UpdateBounds(xITP, xITPValue, ref leftBound, ref rightBound, ref leftBoundValue, ref rightBoundValue);

            // Return xITP if converged
            if ((rightBound - leftBound) / 2f < tolerance) return (rightBound + leftBound) / 2f;
        }

        static (float xMidpoint, float projectionRadius, float truncationRange) CalculateParameters(float leftBound, float rightBound, float tolerance, float truncationFactor, float truncationExponent, int maxIterations, int iteration)
        {
            float xMidpoint = (leftBound + rightBound) / 2;
            float projectionRadius = tolerance * (float)Math.Pow(2, maxIterations - iteration) - (rightBound - leftBound) / 2;
            float truncationRange = truncationFactor * (float)Math.Pow(rightBound - leftBound, truncationExponent);
            return (xMidpoint, projectionRadius, truncationRange);
        }

        static float InterpolateRegulaFalsi(float leftBound, float rightBound, float leftBoundValue, float rightBoundValue)
        {
            return (rightBound * leftBoundValue - leftBound * rightBoundValue) / (leftBoundValue - rightBoundValue);
        }

        static float Truncate(float xMidpoint, float xRegulaFalsi, int perturbationSign, float truncationRange)
        {
            return Math.Abs(xMidpoint - xRegulaFalsi) >= truncationRange ? xRegulaFalsi + perturbationSign * truncationRange : xMidpoint;
        }

        static float Project(float xTruncated, float xMidpoint, float projectionRadius, int perturbationSign)
        {
            return Math.Abs(xTruncated - xMidpoint) <= projectionRadius ? xTruncated : xMidpoint - perturbationSign * projectionRadius;
        }

        static void UpdateBounds(
            float xITP,
            float xITPValue,
            ref float leftBound,
            ref float rightBound,
            ref float leftBoundValue,
            ref float rightBoundValue
            )
        {
            // If xITPValue is exactly 0, we've found the root, update both bounds and return early
            if (xITPValue == 0)
            {
                leftBound = rightBound = xITP;
                return; // Early exit as we've found the root
            }

            // Determine if we should update the right or left bound based on the polynomial's behavior
            bool hasCrossedZero = MathF.Sign(xITPValue) != MathF.Sign(leftBoundValue);

            // If xITPValue has crossed zero, then it is safe to shrink the right side
            if (hasCrossedZero)
            {
                rightBound = xITP;
                rightBoundValue = xITPValue;
            }
            else // In this case, xITPValue has not crossed the x-axis so we should update the left bound
            {
                leftBound = xITP;
                leftBoundValue = xITPValue;
            }
        }

        return float.NaN;
    }


    public static float RefineRootIntervalBisection(Func<float, float> function, Interval interval, float tolerance = 1e-5f, int maxIterations = 100)
    {
        return RefineRootIntervalBisection(function, interval.LeftBound, interval.RightBound, tolerance, maxIterations);
    }
    /// <summary>
    /// Finds a root of the polynomial within the specified interval using the bisection method.
    /// </summary>
    /// <param name="function">The functions whose roots we are trying to refine.</param>
    /// <param name="leftBound">The left boundary of the interval to search for a root.</param>
    /// <param name="rightBound">The right boundary of the interval to search for a root.</param>
    /// <param name="tolerance">The tolerance for convergence. The method aims to find a root such that the size of the final interval is less than or equal to this value. Default is 0.0001f.</param>
    /// <param name="maxIterations">The maximum number of iterations to perform. This prevents the method from running indefinitely. Default is 100.</param>
    /// <returns>The approximate position of the root within the specified interval, determined to be within the specified tolerance, or null if the root cannot be found within the given number of iterations.</returns>
    /// <exception cref="ArgumentException">Thrown if the initial interval does not contain a root.</exception>
    public static float RefineRootIntervalBisection(Func<float, float> function, float leftBound, float rightBound, float tolerance = 1e-5f, int maxIterations = 100)
    {
        float fLeft = function(leftBound);
        float fRight = function(rightBound);

        if (fLeft == 0)
        {
            leftBound += tolerance;
            fLeft = function(leftBound);
        }
        if (fRight == 0) return rightBound;

        // Check if the initial interval is valid
        if (MathF.Sign(fLeft) == MathF.Sign(fRight))
        {
            throw new ArgumentException("The initial interval does not contain a single root.");
        }

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            float midpoint = (leftBound + rightBound) / 2f;
            float fMid = function(midpoint);

            if (fMid == 0 || (rightBound - leftBound) / 2f < tolerance)
            {
                return midpoint; // A root is found or the interval is sufficiently small
            }

            // If the points lie in the same region, the root is still on the right side of the midpoint and it is safe to shrink the left side
            if (MathF.Sign(fMid) == MathF.Sign(fLeft))
            {
                leftBound = midpoint; // The root lies in the right half
                fLeft = fMid; // Update the value at the left bound
            }
            else
            {
                rightBound = midpoint; // The root lies in the left half
                // fRight is implicitly updated as we do not use it after this
            }
        }

        // If the maximum number of iterations is reached without converging
        return float.NaN;
    }
}
