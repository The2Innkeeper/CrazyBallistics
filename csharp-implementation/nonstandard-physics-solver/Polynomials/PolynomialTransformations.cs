namespace NonstandardPhysicsSolver.Polynomials;

public partial struct Polynomial
{
    /// <summary>
    /// Applies the transformation p(x) := p(x + shift) in a quadratic complexity implementation with some caching.
    /// </summary>
    /// <param name="coefficients">List of coefficients of the polynomial.</param>
    /// <param name="shift">The value to shift by, x := x + shift.</param>
    /// <returns>The shifted polynomial coefficients.</returns>
    public readonly Polynomial TaylorShiftQuadratic(float shift)
    {
        float[] newCoefficients = new float[Coefficients.Length];
        for (int i = 0; i < Coefficients.Length; i++)
        {
            for (int k = 0; k <= i; k++)
            {
                newCoefficients[k] += Coefficients[i] * Binomial(i, k) * (float)Math.Pow(shift, i - k);
            }
        }
        return new Polynomial(newCoefficients);


        static float Binomial(int n, int k)
        {
            if (k < 0 || k > n) return 0;
            if (k == 0 || k == n) return 1; // n choose 0 = n choose n = 1
            if (k == 1 || k == n - 1) return n; // Handle n choose 1 and n choose n-1

            // Leverage symmetry: \binom{n}{k} = \binom{n}{n-k}
            if (k > n / 2) k = n - k;

            int[][] precomputed =
            [
            // Removed the cases for n = 0, 1, 2, 3 since they're already covered
            [6], // n = 4
            [10], // n = 5
            [15, 20],
            [21, 35],
            [28, 56, 70],
            [36, 84, 126],
            [45, 120, 210, 252], // n = 10
            ];

            if (n <= 10 && k < precomputed[n - 4].Length)
            {
                return precomputed[n - 4][k - 2]; // Adjusted index for n and k (i=0,n=4 and j=0,k=2)
            }

            if (binomialCache.TryGetValue((n, k), out float cachedValue))
            {
                return cachedValue;
            }

            float value = Binomial(n - 1, k - 1) + Binomial(n - 1, k);
            binomialCache[(n, k)] = value;
            return value;
        }
    }

    // Calculate binomial coefficients with caching
    private static Dictionary<(int, int), float> binomialCache = [];

    /// <summary>
    /// Applies the transformation p(x) := x^degree(p) * p(1/x), equivalent to flipping the coefficient list.
    /// </summary>
    /// <returns>A new Polynomial instance with coefficients in reversed order.</returns>
    public readonly Polynomial Reversed()
    {
        float[] reversedCoefficients = new float[Coefficients.Length];
        Array.Copy(Coefficients, reversedCoefficients, Coefficients.Length);
        Array.Reverse(reversedCoefficients);
        return new Polynomial(reversedCoefficients);
    }
}