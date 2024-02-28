namespace NonstandardPhysicsSolver.Polynomials;

public partial struct Polynomial
{
    /// <summary>
    /// Calculates the Local-Max-Quadratic (LMQ) bound for the positive roots of a polynomial.
    /// This method provides an upper bound estimate based on the polynomial's coefficients.
    /// </summary>
    /// <returns>The LMQ bound as a float, representing an upper bound on the polynomial's positive roots.</returns>
    /// <exception cref="ArgumentException">Thrown when the coefficients list is null or empty.</exception>
    public readonly float LMQPositiveUpperBound()
    {
        // Validate input
        if (Coefficients == null || Coefficients.Count == 0)
        {
            throw new ArgumentException("The coefficients list cannot be null or empty.");
        }

        double upperBound = double.MinValue;

        // Find all negative coefficients and their indices, from high to low degree
        for (int neg_i = Coefficients.Count - 1; neg_i >= 0; neg_i--)
        {
            if (!(Coefficients[neg_i] < 0)) continue;

            double minRadical = double.MaxValue;
            int power = 1; // Reset t_j for each negative coefficient

            // Loop over all preceding positive coefficients, from high to low degree
            for (int pos_i = Coefficients.Count - 1; pos_i > neg_i; pos_i--)
            {
                if (!(Coefficients[pos_i] > 0)) continue;

                // Compute radical
                double radical = Math.Pow(-Math.Pow(2, power) * Coefficients[neg_i] / Coefficients[pos_i], 1.0 / (pos_i - neg_i)); // $\sqrt[j-i]{\frac{-2^{t_j} a_i}{a_j}}$
                minRadical = Math.Min(minRadical, radical);  // Update minimum

                // Increment t_j for each usage
                power++;
            }

            upperBound = Math.Max(upperBound, minRadical);
        }

        if (upperBound == double.MinValue)
        {
            // This means there were no negative coefficients, hence no bound could be calculated
            return 0f;
        }

        return (float)upperBound;
    }

    /// <summary>
    /// Calculates the Local-Max-Quadratic (LMQ) lower bound for the positive roots of a polynomial
    /// by transforming the polynomial P(x) -> x^n*P(1/x) and then computing the upper bound of the transformed polynomial.
    /// </summary>
    /// <returns>The LMQ lower bound as a float, representing a lower bound on the polynomial's positive roots.</returns>
    /// <exception cref="ArgumentException">Thrown when the coefficients list is null or empty.</exception>
    /// <remarks>The reason why process in reverse order is because we apply LMQ to the transformed polynomial x^n*P(1/x).
    /// Given P(x) = a_0 + a_1x + ... + a_nx^n, we obtain x^nP(1/x) = a_n + a_{n-1}x + ... + a_0x^n
    /// which is equivalent to reversing the order of coefficients.</remarks>
    public float LMQPositiveLowerBound()
    {
        // Validate input
        if (Coefficients == null || Coefficients.Count == 0)
        {
            throw new ArgumentException("The coefficients list cannot be null or empty.");
        }

        // Reverse the coefficients for the transformation P(x) -> x^n*P(1/x)
        var reversedCoefficients = Coefficients.AsEnumerable().Reverse().ToList();
        Coefficients = reversedCoefficients;

        // Reuse the logic from LMQUpperBound to calculate the upper bound of the transformed polynomial
        float upperBoundOfTransformed = LMQPositiveUpperBound();

        // Restore the original coefficients to maintain the object's integrity
        Coefficients = reversedCoefficients.AsEnumerable().Reverse().ToList();

        // The lower bound of the original polynomial is the inverse of the upper bound of the transformed polynomial
        if (upperBoundOfTransformed == 0)
        {
            // If the upper bound is 0, it implies all coefficients were positive, and thus, there's no positive lower bound
            return float.MaxValue;
        }

        // Return the upper bound for the transformed polynomial as the lower bound for the original polynomial
        return upperBoundOfTransformed;
    }
}
