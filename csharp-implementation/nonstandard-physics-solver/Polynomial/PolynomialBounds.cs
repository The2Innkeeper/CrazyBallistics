namespace NonstandardPhysicsSolver.Polynomials;

public partial class Polynomial
{
    /// <summary>
    /// Calculates the Local-Max-Quadratic (LMQ) bound for the positive roots of a polynomial.
    /// This method provides an upper bound estimate based on the polynomial's coefficients.
    /// </summary>
    /// <returns>The LMQ bound as a float, representing an upper bound on the polynomial's positive roots.</returns>
    /// <exception cref="ArgumentException">Thrown when the coefficients list is null or empty.</exception>
    public float LMQUpperBound()
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
}
