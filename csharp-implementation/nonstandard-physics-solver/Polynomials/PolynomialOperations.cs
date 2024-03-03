namespace NonstandardPhysicsSolver.Polynomials;

/// <summary>
/// Represents a polynomial and provides methods for various polynomial operations.
/// </summary>
public partial struct Polynomial
{
    /// <summary>
    /// Create a normalized version of the polynomial (leading coefficient is 1).
    /// </summary>
    /// <returns>A normalized version of the polynomial (leading coefficient is 1).</returns>
    public Polynomial Normalized()
    {
        if (Coefficients.Length == 0 || Coefficients[^1] == 0)
        {
            return new Polynomial([0]);
        }

        float scalingFactor = Coefficients[^1];
        var normalizedCoefficients = Coefficients.Select(c => c / scalingFactor).ToArray();
        return new Polynomial(normalizedCoefficients);
    }

    /// <summary>
    /// Calculates the derivative of the polynomial.
    /// </summary>
    /// <returns>A new Polynomial instance representing the derivative of the original polynomial.</returns>
    public Polynomial PolynomialDerivative()
    {
        if (Coefficients.Length <= 1) return new Polynomial([0]);

        var derivativeCoeffs = new float[Coefficients.Length - 1];
        for (int i = 1; i < Coefficients.Length; i++)
        {
            derivativeCoeffs[i - 1] = Coefficients[i] * i;
        }
        return new Polynomial(derivativeCoeffs);
    }

    /// <summary>
    /// Shifts the polynomial coefficients by one degree lower, effectively removing the constant term.
    /// This operation creates a new polynomial where each coefficient's degree is lowered by one,
    /// and the constant term is removed from the array.
    /// For example, if the polynomial was 3 + 2X + X^2, it becomes 2 + X after the shift.
    /// </summary>
    /// <returns>A new <see cref="Polynomial"/> instance with coefficients shifted by one degree lower.</returns>
    public Polynomial ShiftCoefficientsBy1()
    {
        if (Coefficients.Length <= 1)
        {
            // If there's only a constant term (or none), return a polynomial that represents 0.
            return new Polynomial([0]);
        }

        // Create a new array for coefficients with one less element.
        float[] shiftedCoefficients = new float[Coefficients.Length - 1];

        // Copy the coefficients starting from index 1 (skip the constant term) to the new array.
        Array.Copy(Coefficients, 1, shiftedCoefficients, 0, Coefficients.Length - 1);

        // Return a new Polynomial instance with the shifted coefficients.
        return new Polynomial(shiftedCoefficients);
    }

    /// <summary>
    /// Generates a square-free version of the polynomial by removing any repeated roots.
    /// </summary>
    /// <returns>A new Polynomial instance that is square-free.</returns>
    public Polynomial MakeSquareFree()
    {
        var derivative = this.PolynomialDerivative();
        var gcd = PolynomialGCD(this, derivative);

        // If the GCD is a constant, the original polynomial is already square-free.
        if (gcd.Coefficients.Length == 1 && MathF.Abs(gcd.Coefficients[0] - 1) < float.Epsilon)
        {
            return this;
        }

        var (squareFree, _) = PolynomialDivision(this, gcd);
        return squareFree;
    }

    /// <summary>
    /// Divides one polynomial by another, returning the quotient and remainder.
    /// </summary>
    /// <param name="dividend">The polynomial to be divided.</param>
    /// <param name="divisor">The polynomial to divide by.</param>
    /// <returns>A tuple containing the quotient and remainder polynomials.</returns>
    /// <exception cref="DivideByZeroException">Thrown when attempting to divide by a zero polynomial.</exception>
    public static (Polynomial Quotient, Polynomial Remainder) PolynomialDivision(Polynomial dividend, Polynomial divisor)
    {
        var dividendCoeffs = dividend.Coefficients;
        var divisorCoeffs = divisor.Coefficients;

        if (divisorCoeffs.All(coefficient => MathF.Abs(coefficient) < float.Epsilon))
        {
            throw new DivideByZeroException("Attempted to divide by a zero polynomial.");
        }

        int len_diff = dividendCoeffs.Length - divisorCoeffs.Length;
        if (len_diff < 0)
        {
            // When dividend's degree is less than divisor's, quotient is 0, and remainder is the dividend.
            return (new Polynomial([0]), dividend);
        }

        var quotientCoeffs = new List<float>();
        var remainderCoeffs = new List<float>(dividendCoeffs);
        float normalizer = divisorCoeffs.Last();

        for (int i = 0; i <= len_diff; i++)
        {
            // Calculate the scale factor for the divisor to subtract from the dividend
            float scale = remainderCoeffs[^1] / normalizer;
            quotientCoeffs.Insert(0, scale); // Insert at the beginning to maintain the order from highest to lowest degree

            // Subtract the scaled divisor from the remainder
            for (int j = 0; j < divisorCoeffs.Length; j++)
            {
                int remainderIndex = remainderCoeffs.Count - divisorCoeffs.Length + j;
                if (remainderIndex < remainderCoeffs.Count)
                {
                    remainderCoeffs[remainderIndex] -= scale * divisorCoeffs[j];
                }
            }

            // Remove the last element of the remainder as it's been fully processed
            remainderCoeffs.RemoveAt(remainderCoeffs.Count - 1);
        }

        // Trim leading zeros from the remainder
        while (remainderCoeffs.Count > 1 && MathF.Abs(remainderCoeffs.Last()) < float.Epsilon)
        {
            remainderCoeffs.RemoveAt(remainderCoeffs.Count - 1);
        }

        // Ensure at least one zero remains if the remainder is fully divided
        if (remainderCoeffs.Count == 0)
        {
            remainderCoeffs.Add(0f);
        }

        return (new Polynomial([.. quotientCoeffs]), new Polynomial([.. remainderCoeffs]));
    }

    /// <summary>
    /// Calculates the greatest common divisor (GCD) of two polynomials.
    /// </summary>
    /// <param name="a">The first polynomial.</param>
    /// <param name="b">The second polynomial.</param>
    /// <returns>The GCD of the two polynomials.</returns>
    /// <remarks>
    /// Implements the Euclidean algorithm tailored for polynomials.
    /// </remarks>
    public static Polynomial PolynomialGCD(Polynomial a, Polynomial b)
    {
        // Continuously apply the Euclidean algorithm until a remainder of zero is found.
        while (b.Coefficients.Any(c => Math.Abs(c) > float.Epsilon))
        {
            var (_, remainder) = PolynomialDivision(a, b);
            a = b;
            b = remainder;
        }

        // Normalize the leading coefficient to 1 for the GCD.
        return a.Normalized();
    }
}