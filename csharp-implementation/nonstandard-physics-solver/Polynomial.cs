namespace Polynomial;

using System;

/// <summary>
/// Provides methods for polynomials.
/// </summary>
public static class Polynomial
{
    /// <summary>
    /// Evaluates a polynomial at a specified point using compensated Horner's method. It takes on average around 2, up to 3 times as many calculations as Horner's method.
    /// </summary>
    /// <param name="coefficients">The coefficients of the polynomial in increasing order.</param>
    /// <param name="x">The point at which to evaluate the polynomial.</param>
    /// <returns>The value of the polynomial at point x.</returns>
    /// <exception cref="ArgumentException">Thrown when the coefficients list is null or empty.</exception>
    public static float EvaluatePolynomialAccurate(List<float> coefficients, float x)
    {
        if (coefficients == null || coefficients.Count == 0)
            throw new ArgumentException("Coefficients list cannot be null or empty.");

        float sum = coefficients[^1]; // Start with the last coefficient
        float comp = 0.0f; // Compensation for lost low-order bits

        for (int coeff_i = coefficients.Count - 2; coeff_i >= 0; coeff_i--)
        {
            float y = coefficients[coeff_i] + comp; // Add the compensation
            float t = sum * x + y; // Horner's method step
            comp = t - sum * x - y; // Update the compensation
            sum = t; // Update the sum
        }

        return sum;
    }

    /// <summary>
    /// Evaluates a polynomial at a given point using Horner's method.
    /// </summary>
    /// <param name="inputValue">The input value at which to evaluate the polynomial.</param>
    /// <param name="polynomialCoefficients">The coefficients of the polynomial in increasing order of degree.</param>
    /// <returns>The result of the polynomial evaluation.</returns>
    public static float EvaluatePolynomialHorner(float inputValue, float[] polynomialCoefficients)
    {
        int degree = polynomialCoefficients.Length - 1;
        float hornerResult = polynomialCoefficients[degree];
        for (int coeff_i = degree - 1; coeff_i >= 0; coeff_i--)
        {
            hornerResult = hornerResult * inputValue + polynomialCoefficients[coeff_i];
        }
        return hornerResult;
    }

    /// <summary>
    /// Generates a square-free polynomial from the input list of polynomial coefficients.
    /// </summary>
    /// <param name="polyCoeffs">A list of coefficients of the polynomial, from lowest to highest degree.</param>
    /// <returns>A list of coefficients of the square-free part of the polynomial.</returns>
    public static List<float> MakeSquareFree(List<float> polyCoeffs)
    {
        var derivativeCoeffs = PolynomialDerivativeCoefficients(polyCoeffs);
        var gcdCoeffs = GcdPolynomials(polyCoeffs, derivativeCoeffs);
        if (gcdCoeffs.Count == 1) return polyCoeffs;
        var (squareFreeCoeffs, _) = PolynomialDivision(polyCoeffs, gcdCoeffs);

        return squareFreeCoeffs;
    }

    /// <summary>
    /// Calculates the coefficients of the derivative of a polynomial.
    /// </summary>
    /// <param name="coeffs">A list of coefficients of the polynomial, from lowest to highest degree.</param>
    /// <returns>A list of coefficients of the derivative of the polynomial.</returns>
    public static List<float> PolynomialDerivativeCoefficients(List<float> coeffs)
    {
        var derivativeCoeffs = new List<float>();
        for (int power = 0; power < coeffs.Count; power++)
        {
            if (power > 0)  // Skip the constant term
            {
                derivativeCoeffs.Add(coeffs[power] * power);
            }
        }
        return derivativeCoeffs;
    }

    /// <summary>
    /// Performs polynomial division of a dividend by a divisor.
    /// </summary>
    /// <param name="dividend">A list of coefficients of the dividend polynomial, from lowest to highest degree.</param>
    /// <param name="divisor">A list of coefficients of the divisor polynomial, from lowest to highest degree.</param>
    /// <returns>
    /// A tuple where the first item is a list of coefficients of the quotient polynomial,
    /// and the second item is a list of coefficients of the remainder polynomial. Both are returned from lowest to highest degree.
    /// </returns>
    /// <exception cref="DivideByZeroException">Thrown when the divisor is a zero polynomial.</exception>
    /// <remarks>
    /// This method uses negative indexing for lists to simplify access to polynomial coefficients from the end.
    /// Example usage:
    /// var result = PolynomialDivision(new List<float> {1, -3, 0, 2}, new List<float> {1, -1});
    /// Console.WriteLine($"Quotient: {string.Join(", ", result.Quotient)}");
    /// Console.WriteLine($"Remainder: {string.Join(", ", result.Remainder)}");
    /// </remarks>
    public static (List<float> Quotient, List<float> Remainder) PolynomialDivision(List<float> dividend, List<float> divisor)
    {
        if (divisor.All(coefficient => coefficient == 0))
        {
            throw new DivideByZeroException();
        }

        int len_diff = dividend.Count - divisor.Count;
        if (len_diff < 0)
        {
            return ([0f], dividend);
        }

        var quotient = new List<double>();
        var remainder = dividend.Select(x => (double)x).ToList();
        double normalizer = divisor.Last();

        for (int i = 0; i <= len_diff; i++)
        {
            // Calculate the scale factor for the divisor to subtract from the dividend
            double scale = remainder[^1] / normalizer;
            quotient.Insert(0, scale); // Insert at the beginning to maintain the order from highest to lowest degree

            // Subtract the scaled divisor from the remainder
            for (int j = 0; j < divisor.Count; j++)
            {
                // Subtract the scaled value from the corresponding remainder coefficient
                remainder[remainder.Count - divisor.Count + j] -= scale * divisor[j];
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

        return (finalQuotient, trimmedRemainder);
    }

    /// <summary>
    /// Calculates the greatest common divisor (GCD) of two polynomials.
    /// </summary>
    /// <param name="a">A list of coefficients of the first polynomial, from lowest to highest degree.</param>
    /// <param name="b">A list of coefficients of the second polynomial, from lowest to highest degree.</param>
    /// <returns>A list of coefficients representing the GCD of the two polynomials, normalized to have a positive leading coefficient.</returns>
    /// <remarks>
    /// This method implements the Euclidean algorithm for polynomials. It handles edge cases such as zero polynomials and ensures
    /// the leading coefficient of the GCD is positive. If either polynomial is the zero polynomial, the other is returned as the GCD.
    /// </remarks>
    public static List<float> GcdPolynomials(List<float> a, List<float> b)
    {
        // Handle zero polynomial cases
        if (!a.Any(x => x != 0)) return normalize(b);
        if (!b.Any(x => x != 0)) return normalize(a);

        while (b.Any(x => x != 0))
        {
            var (_, remainder) = PolynomialDivision(a, b);
            a = b;
            b = remainder;
        }

        return normalize(a); // Ensure the GCD has 1 as its leading coefficient

        // Normalize to ensure leading coefficient is 1
        static List<float> normalize(List<float> poly)
        {
            if (poly.Last() == 1) return poly;
            return poly.Select(x => x / poly.Last()).ToList(); // Scale leading coefficient to 1
        }
    }
}