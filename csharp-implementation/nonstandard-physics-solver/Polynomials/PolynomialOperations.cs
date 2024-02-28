namespace NonstandardPhysicsSolver.Polynomials;

/// <summary>
/// Represents a polynomial and provides methods for various polynomial operations.
/// </summary>
public partial struct Polynomial
{
    /// <summary>
    /// Scale the coefficients such that the leading coefficient (highest degree) is 1.
    /// </summary>
    public readonly void Normalize()
    {
        var coefficients = new List<float>(Coefficients);
        float scalingFactor = coefficients[^1];
        coefficients = coefficients.Select(c => c / scalingFactor).ToList();
    }

    /// <summary>
    /// Create a normalized version of the polynomial (leading coefficient is 1).
    /// </summary>
    /// <returns>A normalized version of the polynomial (leading coefficient is 1).</returns>
    public readonly Polynomial Normalized()
    {
        var coefficients = new List<float>(Coefficients);
        float scalingFactor = coefficients[^1];
        coefficients = coefficients.Select(c => c / scalingFactor).ToList();
        return new Polynomial(coefficients);
    }

    /// <summary>
    /// Calculates the derivative of the polynomial.
    /// </summary>
    /// <returns>A new Polynomial instance representing the derivative of the original polynomial.</returns>
    public readonly Polynomial PolynomialDerivative()
    {
        if (Coefficients.Count == 1) return new Polynomial([0f]);
        var derivativeCoeffs = Coefficients.Select((coeff, index) => coeff * index).Skip(1).ToList();
        return new Polynomial(derivativeCoeffs);
    }

    /// <summary>
    /// Generates a square-free version of the polynomial by removing any repeated roots.
    /// </summary>
    /// <returns>A new Polynomial instance that is square-free.</returns>
    public Polynomial MakeSquareFree()
    {
        var derivative = this.PolynomialDerivative();
        var gcd = GcdPolynomials(this, derivative);

        // If the GCD is a constant, the original polynomial is already square-free.
        if (gcd.Coefficients.Count == 1 && gcd.Coefficients[0] == 1)
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

        if (divisorCoeffs.All(coefficient => coefficient == 0))
        {
            throw new DivideByZeroException();
        }

        int len_diff = dividendCoeffs.Count - divisorCoeffs.Count;
        if (len_diff < 0)
        {
            return (new Polynomial([0f]), dividend);
        }

        var quotient = new List<double>();
        var remainder = dividendCoeffs.Select(x => (double)x).ToList();
        double normalizer = divisorCoeffs.Last();

        for (int i = 0; i <= len_diff; i++)
        {
            // Calculate the scale factor for the divisor to subtract from the dividend
            double scale = remainder[^1] / normalizer;
            quotient.Insert(0, scale); // Insert at the beginning to maintain the order from highest to lowest degree

            // Subtract the scaled divisor from the remainder
            for (int j = 0; j < divisorCoeffs.Count; j++)
            {
                // Subtract the scaled value from the corresponding remainder coefficient
                remainder[remainder.Count - divisorCoeffs.Count + j] -= scale * divisorCoeffs[j];
            }

            // Remove the last element of the remainder as it's been fully processed
            remainder.RemoveAt(remainder.Count - 1);
        }

        // Convert the remainder back to float for the output, trimming leading zeros
        var trimmedRemainder = remainder.Select(x => (float)x).ToList();
        while (trimmedRemainder.Count > 0 && trimmedRemainder[^1] == 0)
        {
            trimmedRemainder.RemoveAt(trimmedRemainder.Count - 1);
        }

        // Convert the quotient to the required float format, though it's calculated as double
        var finalQuotient = quotient.Select(x => (float)x).ToList();

        // Ensure the remainder is properly formatted for output
        if (trimmedRemainder.Count == 0)
        {
            trimmedRemainder.Add(0f);
        }

        return (new Polynomial(finalQuotient), new Polynomial(trimmedRemainder));
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
    public static Polynomial GcdPolynomials(Polynomial a, Polynomial b)
    {
        // Continuously apply the Euclidean algorithm until a remainder of zero is found.
        while (b.Coefficients.Any(c => c != 0))
        {
            var (_, remainder) = PolynomialDivision(a, b);
            a = b;
            b = remainder;
        }

        // Normalize the leading coefficient to 1 for the GCD.
        var leadingCoefficient = a.Coefficients.Last();
        var normalizedCoefficients = a.Coefficients.Select(c => c / leadingCoefficient).ToList();
        return new Polynomial(normalizedCoefficients);
    }
}