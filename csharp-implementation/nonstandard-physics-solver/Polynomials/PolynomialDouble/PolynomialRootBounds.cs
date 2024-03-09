namespace NonstandardPhysicsSolver.Polynomials;

public partial struct PolynomialDouble
{
    /// <summary>
    /// Calculates the Local-Max-Quadratic (LMQ) bound for the positive roots of a polynomial.
    /// This method provides an upper bound estimate based on the polynomial's coefficients.
    /// </summary>
    /// <returns>The LMQ bound as a double, representing an upper bound on the polynomial's positive roots.</returns>
    /// <exception cref="ArgumentException">Thrown when the coefficients list is null or empty.</exception>
    public double LMQPositiveUpperBound()
    {
        int coefficientCount = Coefficients.Length;
        double[] coefficients = new double[coefficientCount];
        if (Coefficients.Last() < 0)
        {
            for (int i = 0; i < coefficientCount; i++)
            {
                coefficients[i] = -Coefficients[i];
            }
        }
        else
        {
            coefficients = Coefficients;
        }

        // Validate input
        if (coefficients == null || coefficientCount == 0)
        {
            throw new ArgumentException("The coefficients list cannot be null or empty.");
        }
        if (!coefficients.Any(coeff => coeff <= 0)) // If all coefficients are strictly positive, there will be no positive roots
        {
            return double.NaN;
        }

        int degree = coefficientCount - 1;
        int[] usageCounts = Enumerable.Repeat(1, coefficientCount).ToArray();
        double upperBound = double.NegativeInfinity;

        for (int neg_i = degree - 1; neg_i >= 0; neg_i--)
        // Note: neg_i = i
        // max {a_i < 0}
        {
            if (!(coefficients[neg_i] < 0)) { continue; }

            double minRadical = double.PositiveInfinity;

            for (int pos_i = neg_i + 1; pos_i <= degree; pos_i++)
            // Note: pos_i = j
            // min {a_j > 0; j > i}
            {
                if (!(coefficients[pos_i] > 0)) { continue; }

                double radical = Math.Pow(-Math.Pow(2, usageCounts[pos_i]) * coefficients[neg_i] / coefficients[pos_i], 1.0 / (pos_i - neg_i));
                minRadical = Math.Min(minRadical, radical);

                usageCounts[pos_i]++;
            }

            if (minRadical != double.PositiveInfinity)
            {
                upperBound = Math.Max(upperBound, minRadical);
            }
        }

        return upperBound < 0 ? double.NaN : (double)upperBound;
    }

    /// <summary>
    /// Calculates the Local-Max-Quadratic (LMQ) lower bound for the positive roots of a polynomial
    /// by transforming the polynomial P(x) -> x^n*P(1/x) and then computing the upper bound of the transformed polynomial.
    /// </summary>
    /// <returns>The LMQ lower bound as a double, representing a lower bound on the polynomial's positive roots.</returns>
    /// <exception cref="ArgumentException">Thrown when the coefficients list is null or empty.</exception>
    /// <remarks>Note that our implementation of the polynomial coefficients are in increasing order of degree. 
    /// To find the LMQ lower bound, we process the transformed polynomial x^n*P(1/x), and calculate the reciprocal of its upper bound, 1 / ubLMQ.
    /// Given P(x) = a_0 + a_1x + ... + a_nx^n, we obtain x^nP(1/x) = a_n + a_{n-1}x + ... + a_0x^n
    /// which is equivalent to reversing the order of coefficients [a_0, ..., a_n] into [a_n, ..., a_0]
    /// But this means that we can simply process the algorithm with negative coefficients in incremental order,(a_0 to a_n), skipping a_0,
    /// then take the reciprocal at the end.
    /// Notice that when working that way, degree(neg_i) = n - neg_i and degree(pos_i) = n - pos_i
    /// And note: neg_i > pos_i
    /// </remarks>
    public double LMQPositiveLowerBound()
    {

        int coefficientCount = Coefficients.Length;
        double[] coefficients = new double[coefficientCount];
        if (Coefficients.First() < 0)
        {
            for (int i = 0; i < coefficientCount; i++)
            {
                coefficients[i] = -Coefficients[i];
            }
        }
        else
        {
            coefficients = Coefficients;
        }
        // Validate input
        if (coefficients == null || coefficientCount == 0)
        {
            throw new ArgumentException("The coefficients list cannot be null or empty.");
        }

        if (!coefficients.Any(coeff => coeff <= 0)) // If all coefficients are strictly positive, there will be no positive roots
        {
            return double.NaN;
        }

        int degree = coefficientCount - 1;
        int[] usageCounts = Enumerable.Repeat(1, coefficientCount).ToArray();
        double maxMinRadical = double.NegativeInfinity;

        // Go in incremental order: from a_0 to a_n, skipping a_0
        // Negative coefficients
        for (int neg_i = 1; neg_i <= degree; neg_i++)
        {
            if (!(coefficients[neg_i] < 0)) { continue; }

            double minRadical = double.PositiveInfinity;

            // Positive coefficients
            for (int pos_i = neg_i - 1; pos_i >= 0; pos_i--) // Process every higher degree positive coefficient, so pos_i < neg_i
            {
                if (!(coefficients[pos_i] > 0)) { continue; }
                // Compute (-2^t_j * a_i / a_j)^(1 / (j - i))
                /* Note that in our case, our indexes are reversed because we increment as we go from high to low degree
                 * That is,
                 * neg_i = ^i = coefficientCount - i
                 * pos_i = ^j = coefficientCount - j
                 * compared to the original algorithm.
                 * Hence: j - i = (coefficientCount - pos_i) - (coefficientCount - neg_i) = neg_i - pos_i
                 * So the exponent becomes 1.0 / (neg_i - pos_i)
                 * The rest is unchanged, since the indexes do not affect the coefficients themselves, only the order
                 * Honestly it's just the difference in degree
                */
                double radical = Math.Pow(-Math.Pow(2, usageCounts[pos_i]) * coefficients[neg_i] / coefficients[pos_i], 1.0 / (neg_i - pos_i));
                minRadical = Math.Min(minRadical, radical);

                usageCounts[pos_i]++; // Consistent as long as you use the same positions for the same terms, if you want to match degree you should use complementary index
            }

            if (minRadical != double.PositiveInfinity)
            {
                maxMinRadical = Math.Max(maxMinRadical, minRadical);
            }
        }

        // Return the reciprocal of upper bound for the transformed polynomial as the lower bound for the original polynomial
        double lowerBound = (double)(1.0 / maxMinRadical);
        return lowerBound < 0 ? double.NaN : lowerBound;
    }
}