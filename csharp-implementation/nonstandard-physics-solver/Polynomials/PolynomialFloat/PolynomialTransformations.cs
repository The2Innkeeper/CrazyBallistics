using NonstandardPhysicsSolver.Intervals;

namespace NonstandardPhysicsSolver.Polynomials;

public partial struct PolynomialFloat
{
    /// <summary>
    /// Map ]a,b[ to ]0,+inf[ by P(x) := (x+1)^n * P((ax+b)/(x+1))
    /// </summary>
    /// <returns>The new polynomial whose roots are mapped from ]a,b[ to ]0,+inf[</returns>
    public PolynomialFloat MapIntervalToPositiveReals(Interval interval)
    {
        float[] transformedCoefficients = new float[Coefficients.Length];
        Array.Copy(Coefficients, transformedCoefficients, Coefficients.Length);
        float leftBound = interval.LeftBound;
        float rightBound = interval.RightBound;

        // Step 1: Apply x := x + a
        TaylorShift(ref transformedCoefficients, leftBound);

        // Step 2: Apply x := (b-a)x
        Scale(ref transformedCoefficients, rightBound - leftBound);

        // Step 3: Apply P(x) := x^degree(P) * P(1/x)
        Array.Reverse(transformedCoefficients);

        // Step 4: Apply x := x + 1
        TaylorShiftBy1(ref transformedCoefficients);

        return new PolynomialFloat(transformedCoefficients);

        static void TaylorShiftBy1(ref float[] coefficients)
        {
            for (int i = 1; i < coefficients.Length; i++)
            {
                for (int k = 0; k < i; k++)
                {
                    coefficients[k] += coefficients[i] * Binomial(i, k);
                }
            }

        }

        static void TaylorShift(ref float[] coefficients, float shift)
        {
            for (int i = 1; i < coefficients.Length; i++)
            {
                for (int k = 0; k < i; k++)
                {
                    coefficients[k] += coefficients[i] * Binomial(i, k) * MathF.Pow(shift, i - k);
                }
            }
        }

        static void Scale(ref float[] coefficients, float scaleFactor)
        {
            for (int i = 1; i < coefficients.Length; i++)
            {
                coefficients[i] = MathF.Pow(scaleFactor, i) * coefficients[i];
            }
        }

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

    /// <summary>
    /// Map ]0,1[ to ]0,+inf[ by P(x) := (x+1)^n * P(1/(x+1))
    /// </summary>
    /// <returns>The new polynomial whose roots are mapped from ]0,1[ to ]0,+inf[</returns>
    public PolynomialFloat MapUnitIntervalToPositiveReals()
    {
        float[] transformedCoefficients = new float[Coefficients.Length];
        Array.Copy(Coefficients, transformedCoefficients, Coefficients.Length);

        // Step 1: Apply x := 1/x
        Array.Reverse(transformedCoefficients);

        // Step 2: Apply x := x + 1
        TaylorShiftBy1(ref transformedCoefficients);

        return new PolynomialFloat(transformedCoefficients);


        static void TaylorShiftBy1(ref float[] coefficients)
        {
            for (int i = 1; i < coefficients.Length; i++)
            {
                for (int k = 0; k < i; k++)
                {
                    coefficients[k] += coefficients[i] * Binomial(i, k);
                }
            }
        }
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

    /// <summary>
    /// Transforms the polynomial according to the specified transformation: (x+1)^n * P(s / (x+1)).
    /// </summary>
    /// <returns>The new polynomial (x+1)^n*P(s/(1+x))</returns>
    public PolynomialFloat TransformedForLowerInteval(float scale)
    {
        float[] transformedCoefficients = new float[Coefficients.Length];
        Array.Copy(Coefficients, transformedCoefficients, Coefficients.Length);

        // Step 1: Apply x := sx
        Scale(ref transformedCoefficients, scale);

        // Step 2: Apply P(x) := x^degree(P)*P(1/x)
        Array.Reverse(transformedCoefficients);

        // Step 3: Apply x := x+1
        TaylorShift(ref transformedCoefficients, 1);

        return new PolynomialFloat(transformedCoefficients);


        static void TaylorShift(ref float[] coefficients, float shift)
        {
            for (int i = 1; i < coefficients.Length; i++)
            {
                for (int k = 0; k < i; k++)
                {
                    coefficients[k] += coefficients[i] * Binomial(i, k) * MathF.Pow(shift, i - k);
                }
            }

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

        static void Scale(ref float[] coefficients, float scaleFactor)
        {
            for (int i = 1; i < coefficients.Length; i++)
            {
                coefficients[i] = MathF.Pow(scaleFactor, i) * coefficients[i];
            }
        }
    }

    /// <summary>
    /// Applies the transformation p(x) := p(x + 1) in a quadratic complexity implementation.
    /// </summary>
    /// <returns>The shifted polynomial coefficients.</returns>
    public PolynomialFloat TaylorShiftBy1()
    {
        float[] newCoefficients = new float[Coefficients.Length];
        Array.Copy(Coefficients, newCoefficients, Coefficients.Length);
        for (int i = 1; i < Coefficients.Length; i++)
        {
            for (int k = 0; k < i; k++)
            {
                // s = 1 so s^k = 1
                newCoefficients[k] += Coefficients[i] * Binomial(i, k);
            }
        }

        return new PolynomialFloat(newCoefficients);

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

    /// <summary>
    /// Applies the transformation p(x) := p(x + shift) in a quadratic complexity implementation.
    /// </summary>
    /// <param name="shift">The value to shift by, x := x + shift.</param>
    /// <returns>The shifted polynomial coefficients.</returns>
    public PolynomialFloat TaylorShift(float shift)
    {
        float[] newCoefficients = new float[Coefficients.Length];
        Array.Copy(Coefficients, newCoefficients, Coefficients.Length);

        for (int i = 1; i < Coefficients.Length; i++)
        {
            for (int k = 0; k < i; k++)
            {
                newCoefficients[k] += Coefficients[i] * Binomial(i, k) * MathF.Pow(shift, i - k);
            }
        }

        return new PolynomialFloat(newCoefficients);

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
    public PolynomialFloat Reversed()
    {
        float[] reversedCoefficients = new float[Coefficients.Length];
        Array.Copy(Coefficients, reversedCoefficients, Coefficients.Length);
        Array.Reverse(reversedCoefficients);
        return new PolynomialFloat(reversedCoefficients);
    }

    /// <summary>
    /// Applies the transformation x := sx, equivalent to stretching the function horizontally
    /// or squishing the x-axis
    /// </summary>
    /// <param name="scaleFactor">The factor s by which you scale the input</param>
    /// <returns>A new Polynomial instasnce with scaled coefficients</returns>
    public PolynomialFloat ScaleInput(float scaleFactor)
    {
        float[] scaledCoefficients = new float[Coefficients.Length];
        for (int i = 0; i < scaledCoefficients.Length; i++)
        {
            scaledCoefficients[i] = MathF.Pow(scaleFactor, i) * Coefficients[i];
        }
        return new PolynomialFloat(scaledCoefficients);
    }

    /// <summary>
    /// Applies the transformation P(x) := s^n * P(x/s).
    /// p_k := s^{n-k} * p_k.
    /// </summary>
    /// <param name="scaleFactor"></param>
    /// <returns>A new Polynomial instasnce with scaled coefficients</returns>
    public PolynomialFloat ScaleInputInReverseOrder(float scaleFactor)
    {
        float[] scaledCoefficients = new float[Coefficients.Length];
        int degree = Coefficients.Length - 1;
        for (int i = 0; i < Coefficients.Length; i++)
        {
            scaledCoefficients[i] = MathF.Pow(scaleFactor, degree - i) * Coefficients[i];
        }
        return new PolynomialFloat(scaledCoefficients);
    }
}
