namespace nonstandard_physics_solver;

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
    /// <returns>A tuple where the first item is a list of coefficients of the quotient and the second item is a list of coefficients of the remainder.</returns>
    public static (List<float>, List<float>) PolynomialDivision(List<float> dividend, List<float> divisor)
    {
        int lenDividend = dividend.Count;
        int lenDivisor = divisor.Count;
        List<float> quotient = new(new float[lenDividend - lenDivisor + 1]);
        List<float> remainder = new(dividend);

        for (int i = 0; i < lenDividend - lenDivisor + 1; i++)
        {
            quotient[i] = remainder[i] / divisor[0];
            for (int j = 0; j < lenDivisor; j++)
            {
                remainder[i + j] -= quotient[i] * divisor[j];
            }
        }

        remainder.RemoveRange(0, lenDividend - lenDivisor + 1);
        return (quotient, remainder);
    }

    /// <summary>
    /// Calculates the greatest common divisor (GCD) of two polynomials.
    /// </summary>
    /// <param name="a">A list of coefficients of the first polynomial, from lowest to highest degree.</param>
    /// <param name="b">A list of coefficients of the second polynomial, from lowest to highest degree.</param>
    /// <returns>A list of coefficients of the GCD of the two polynomials.</returns>
    public static List<float> GcdPolynomials(List<float> a, List<float> b)
    {
        while (b.Any(x => x != 0))  // While b is not the zero polynomial
        {
            var (_, remainder) = PolynomialDivision(a, b);
            a = b;
            b = remainder;

            // If the GCD has become a constant polynomial, return it immediately
            if (b.Count == 1)
            {
                return b;
            }
        }
        return a;
    }
}