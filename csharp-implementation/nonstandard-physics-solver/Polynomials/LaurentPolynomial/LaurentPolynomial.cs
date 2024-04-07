namespace NonstandardPhysicsSolver.Polynomials.LaurentPolynomial;

public partial struct LaurentPolynomial
{
    /// <summary>
    /// Coefficients for negative exponents, in increasing order of degree
    /// From minExponent(L) to -1
    /// </summary>
    public float[] NegativeDegreeCoefficients;
    /// <summary>
    /// Coefficients for non-negative exponents, in increasing order of degree
    /// From 0 to maxExponent(L)
    /// </summary>
    public float[] PositiveDegreeCoefficients;

    /// <summary>
    /// Create a Laurent polynomial, which is a polynomial with any integer exponents (including negative)
    /// </summary>
    /// <param name="positiveDegreeCoefficients">Coefficients for non-negative exponents, in increasing order of degree
    /// From 0 to maxExponent(P)</param>
    /// <param name="negativeDegreeCoefficients">Coefficients for negative exponents, in increasing order of degree
    /// From minExponent(P) to -1</param>
    public LaurentPolynomial(float[] negativeDegreeCoefficients, float[] positiveDegreeCoefficients)
    {
        NegativeDegreeCoefficients = negativeDegreeCoefficients;
        PositiveDegreeCoefficients = positiveDegreeCoefficients;
    }

    public LaurentPolynomial(PolynomialFloat polynomial)
    {
        NegativeDegreeCoefficients = [];
        PositiveDegreeCoefficients = polynomial.Coefficients;
    }

    public LaurentPolynomial MultiplyByXPower(int exponent)
    {
        // 3 cases: m=0, m>0, m<0

        // m = 0
        // No change in the polynomial, return a copy of the current instance
        if (exponent == 0) { return this; }

        // m > 0
        if (exponent > 0) { return MultiplyByPositiveXPower(this, exponent); }

        // m < 0
        return MultiplyByNegativeXPower(this, -exponent);


        static LaurentPolynomial MultiplyByPositiveXPower(LaurentPolynomial laurentPolynomial, int shiftAmount)
        {
            float[] posDegCoeffs = laurentPolynomial.PositiveDegreeCoefficients;
            float[] negDegCoeffs = laurentPolynomial.NegativeDegreeCoefficients;
            int newPosDegreeLength = posDegCoeffs.Length + shiftAmount;
            int newNegDegreeLength = Math.Max(0, negDegCoeffs.Length - shiftAmount);
            float[] newPosDegreeCoeffs = new float[newPosDegreeLength];
            float[] newNegDegreeCoeffs = new float[newNegDegreeLength];

            // Case 1: no padding
            if (shiftAmount <= negDegCoeffs.Length)
            {
                // Left
                Array.Copy(negDegCoeffs, newNegDegreeCoeffs, newNegDegreeLength);
                // Middle
                Array.Copy(negDegCoeffs, newNegDegreeLength, newPosDegreeCoeffs, 0, shiftAmount);
                // Right
                Array.Copy(posDegCoeffs, 0, newPosDegreeCoeffs, shiftAmount, posDegCoeffs.Length);
            }
            else
            // Case 2: m > len(n) so pad zeroes
            {
                // Array.Fill(newPosDegreeCoeffs, 0, 0, shiftAmount); // Array is initialized with 0s so unnecessary
                Array.Copy(posDegCoeffs, 0, newPosDegreeCoeffs, shiftAmount, posDegCoeffs.Length);
                Array.Copy(negDegCoeffs, 0, newPosDegreeCoeffs, shiftAmount - negDegCoeffs.Length, negDegCoeffs.Length);
            }

            return new LaurentPolynomial(newNegDegreeCoeffs, newPosDegreeCoeffs);
        }

        static LaurentPolynomial MultiplyByNegativeXPower(LaurentPolynomial laurentPolynomial, int shiftAmount)
        {
            float[] posDegCoeffs = laurentPolynomial.PositiveDegreeCoefficients;
            float[] negDegCoeffs = laurentPolynomial.NegativeDegreeCoefficients;
            int newPosDegreeLength = Math.Max(0, posDegCoeffs.Length - shiftAmount);
            int newNegDegreeLength = negDegCoeffs.Length + shiftAmount;
            float[] newPosDegreeCoeffs = new float[newPosDegreeLength];
            float[] newNegDegreeCoeffs = new float[newNegDegreeLength];

            // Case 1: no padding
            if (shiftAmount <= posDegCoeffs.Length)
            {
                // Left
                Array.Copy(negDegCoeffs, newNegDegreeCoeffs, negDegCoeffs.Length);
                // Middle
                Array.Copy(posDegCoeffs, 0, newNegDegreeCoeffs, negDegCoeffs.Length, shiftAmount);
                // Right
                Array.Copy(posDegCoeffs, shiftAmount, newPosDegreeCoeffs, 0, newPosDegreeLength);
            }
            else // Case 2: |m| > len(p) so pad zeroes
            {
                // Array.Fill(newNegDegreeCoeffs, 0, 0, shiftAmount); // Array is initialized with 0s so unnecessary
                Array.Copy(negDegCoeffs, 0, newNegDegreeCoeffs, 0, negDegCoeffs.Length);
                Array.Copy(posDegCoeffs, 0, newNegDegreeCoeffs, newNegDegreeLength - shiftAmount, posDegCoeffs.Length);
            }

            return new LaurentPolynomial(newNegDegreeCoeffs, newPosDegreeCoeffs);
        }
    }

    /*
    /// <summary>
    /// Multiplies the Laurent polynomial by x^m where m is an integer and returns the result as a new Laurent polynomial.
    /// </summary>
    /// <param name="exponent">The integer power of x by which to multiply the polynomial.</param>
    /// <returns>A new Laurent polynomial representing the original polynomial multiplied by x^m.</returns>
    public LaurentPolynomial MultiplyByXPower(int exponent)
    {
        // Calculate the effective shift for negative and positive coefficients
        int shiftForNegative = Math.Max(exponent, 0); // Shifting negative coefficients to more negative if exponent is positive
        int shiftForPositive = Math.Max(-exponent, 0); // Shifting positive coefficients to more positive if exponent is negative

        // Calculate the new sizes for the coefficient arrays after the shift
        int newNegativeSize = NegativeDegreeCoefficients.Length + shiftForNegative - shiftForPositive;
        int newPositiveSize = PositiveDegreeCoefficients.Length + shiftForPositive - shiftForNegative;

        // Initialize the new coefficient arrays
        float[] newNegativeCoefficients = new float[Math.Max(newNegativeSize, 0)];
        float[] newPositiveCoefficients = new float[Math.Max(newPositiveSize, 0)];

        // Populate the new negative coefficients
        for (int i = 0; i < NegativeDegreeCoefficients.Length; i++)
        {
            int newIndex = i + shiftForNegative;
            if (newIndex >= 0 && newIndex < newNegativeSize)
            {
                newNegativeCoefficients[newIndex] = NegativeDegreeCoefficients[i];
            }
        }

        // Populate the new positive coefficients
        for (int i = 0; i < PositiveDegreeCoefficients.Length; i++)
        {
            int newIndex = i + shiftForPositive;
            if (newIndex >= 0 && newIndex < newPositiveSize)
            {
                newPositiveCoefficients[newIndex] = PositiveDegreeCoefficients[i];
            }
        }

        // Fill in zeros for any new leading/trailing coefficients created by the shift
        for (int i = 0; i < shiftForNegative && i < newNegativeSize; i++)
        {
            newNegativeCoefficients[i] = 0;
        }
        for (int i = PositiveDegreeCoefficients.Length + shiftForPositive; i < newPositiveSize; i++)
        {
            newPositiveCoefficients[i] = 0;
        }

        return new LaurentPolynomial(newPositiveCoefficients, newNegativeCoefficients);
    }
    */

    /// <summary>
    /// Multiply by a power so that all exponents are nonnegative
    /// For example L(x):= 2x^-1 + 1 becomes P(x):= 2 + x
    /// </summary>
    /// <returns>A standard polynomial that has shifted exponents compared to the Laurent one.</returns>
    /// <remarks>The transformed polynomial has the same roots except at 0 where L(0) is undefined
    /// It can be interpreted as the numerator when converting the Laurent polynomial into a rational function in fraction form</remarks>
    public PolynomialFloat ConvertToNumeratorPolynomial()
    {
        // Calculate the total length of the new coefficients array
        int totalLength = PositiveDegreeCoefficients.Length + NegativeDegreeCoefficients.Length;
        float[] newCoefficients = new float[totalLength];

        // Shift negative coefficients to the beginning of the new array
        for (int i = 0; i < NegativeDegreeCoefficients.Length; i++)
        {
            newCoefficients[i] = NegativeDegreeCoefficients[i];
        }

        // Copy positive coefficients to the new array, starting after the shifted negative coefficients
        // You could use the indexing operator for a very clean solution, but I will stick to a basic one.
        for (int i = 0; i < PositiveDegreeCoefficients.Length; i++)
        {
            newCoefficients[i + NegativeDegreeCoefficients.Length] = PositiveDegreeCoefficients[i];
        }

        // Create and return the new PolynomialFloat object
        return new PolynomialFloat(newCoefficients);
    }

    /// <summary>
    /// Calculate the derivative of the Laurent polynomial.
    /// </summary>
    /// <returns>A new Laurent polynomial representing the derivative.</returns>
    public LaurentPolynomial Derivative()
    {
        // Calculate the derivative of the positive coefficients
        float[] positiveDegreeDerivative = new float[PositiveDegreeCoefficients.Length > 0 ? PositiveDegreeCoefficients.Length - 1 : 0];
        // d/dx x^+n = nx^{n-1} and d/dx constant = 0
        for (int i = 1; i < PositiveDegreeCoefficients.Length; i++)
        {
            positiveDegreeDerivative[i - 1] = i * PositiveDegreeCoefficients[i];
        }

        // For negative coefficients, the differentiation process effectively shifts the exponents towards zero,
        // which means we need to extend the length of the negative coefficients array by 1.
        float[] negativeDegreeDerivative = new float[NegativeDegreeCoefficients.Length + 1];
        for (int i = 0; i < NegativeDegreeCoefficients.Length; i++)
        {
            // d/dx x^-n = -nx^{-(n+1)}
            // The exponent for the negative part is -(i+1) due to 0-indexing, so the derivative is -(i+1) * coefficient.
            // x^-1 at index 0 becomes 0, everything other index is shifted by +1.
            // We start filling the array from index 1
            negativeDegreeDerivative[i + 1] = -(i + 1) * NegativeDegreeCoefficients[i];
        }

        // Return the new Laurent polynomial representing the derivative
        return new LaurentPolynomial(positiveDegreeDerivative, negativeDegreeDerivative);
    }

    /// <summary>
    /// Evaluate the Laurent polynomial at a given point x using Horner's scheme
    /// </summary>
    /// <param name="x">Input value to evaluate L(x) at</param>
    /// <returns></returns>
    public float EvaluateAt(float x)
    {
        float result = 0;

        // Evaluate positive exponents using Horner's scheme
        if (PositiveDegreeCoefficients.Length > 0)
        {
            result = PositiveDegreeCoefficients[^1];
            for (int i = PositiveDegreeCoefficients.Length - 2; i >= 0; i--)
            {
                result = result * x + PositiveDegreeCoefficients[i];
            }
        }

        // Evaluate negative exponents
        // For negative exponents, we apply a similar idea but with 1/x
        if (NegativeDegreeCoefficients.Length > 0)
        {
            float negativeResult = NegativeDegreeCoefficients[0];
            // Start at lowest degree, stop at highest degree, incrementing
            for (int i = 0; i <= NegativeDegreeCoefficients.Length - 1; i++)
            {
                negativeResult = negativeResult / x + NegativeDegreeCoefficients[i];
            }
            result += negativeResult / x; // Divide once more for the correct power (b/c ends at x^-1)
        }

        return result;
    }
}
