namespace nonstandard_physics_solver;

using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This class provides methods for decomposing a polynomial into its square-free component.
/// </summary>
public static class PolynomialSquarefreeDecomposer
{
    /// <summary>
    /// Generates a square-free polynomial from the input list of polynomial coefficients.
    /// </summary>
    /// <param name="polyCoeffs">A list of coefficients of the polynomial, from lowest to highest degree.</param>
    /// <returns>A list of coefficients of the square-free part of the polynomial.</returns>
    public static List<double> MakeSquareFree(List<double> polyCoeffs)
    {
        var derivativeCoeffs = PolynomialDerivativeCoefficients(polyCoeffs);
        var gcdCoeffs = GcdPolynomials(polyCoeffs, derivativeCoeffs);
        var (squareFreeCoeffs, _) = PolynomialDivision(polyCoeffs, gcdCoeffs);
        
        return squareFreeCoeffs;

        /// <summary>
        /// Calculates the coefficients of the derivative of a polynomial.
        /// </summary>
        /// <param name="coeffs">A list of coefficients of the polynomial, from lowest to highest degree.</param>
        /// <returns>A list of coefficients of the derivative of the polynomial.</returns>
        List<double> PolynomialDerivativeCoefficients(List<double> coeffs)
        {
            var derivativeCoeffs = new List<double>();
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
        (List<double>, List<double>) PolynomialDivision(List<double> dividend, List<double> divisor)
        {
            var quotient = new List<double>();
            var remainder = new List<double>(dividend);  // Start with the dividend as the remainder

            int degDividend = dividend.Count - 1;
            int degDivisor = divisor.Count - 1;

            while (degDividend >= degDivisor && remainder.Any(x => x != 0))
            {
                double leadDividend = remainder.Last();
                double leadDivisor = divisor.Last();
                int degLeadDividend = remainder.Count - 1;

                double coeffQuotient = leadDividend / leadDivisor;
                int degDiff = degLeadDividend - degDivisor;
                var quotientTerm = new List<double>(new double[degDiff]);
                quotientTerm.Add(coeffQuotient);

                var divisorTerm = divisor.Select(coeff => coeff * coeffQuotient).ToList();
                divisorTerm.AddRange(new double[degDiff]);

                remainder = remainder.Zip(divisorTerm, (a, b) => a - b).ToList();

                while (remainder.Any() && remainder.Last() == 0)
                {
                    remainder.RemoveAt(remainder.Count - 1);
                }

                degDividend = remainder.Count - 1;

                quotient = quotient.Zip(quotientTerm, (a, b) => a + b).ToList();
            }

            while (quotient.Any() && quotient.Last() == 0)
            {
                quotient.RemoveAt(quotient.Count - 1);
            }

            return (quotient, remainder);
        }

        /// <summary>
        /// Calculates the greatest common divisor (GCD) of two polynomials.
        /// </summary>
        /// <param name="a">A list of coefficients of the first polynomial, from lowest to highest degree.</param>
        /// <param name="b">A list of coefficients of the second polynomial, from lowest to highest degree.</param>
        /// <returns>A list of coefficients of the GCD of the two polynomials.</returns>
        List<double> GcdPolynomials(List<double> a, List<double> b)
        {
            while (b.Any(x => x != 0))  // While b is not the zero polynomial
            {
                var (_, remainder) = PolynomialDivision(a, b);
                a = b;
                b = remainder;
            }
            return a;
        }
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

        /// <summary>
        /// Calculates the coefficients of the derivative of a polynomial.
        /// </summary>
        /// <param name="coeffs">A list of coefficients of the polynomial, from lowest to highest degree.</param>
        /// <returns>A list of coefficients of the derivative of the polynomial.</returns>
        List<float> PolynomialDerivativeCoefficients(List<float> coeffs)
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
        (List<float>, List<float>) PolynomialDivision(List<float> dividend, List<float> divisor)
        {
            var quotient = new List<float>();
            var remainder = new List<float>(dividend);  // Start with the dividend as the remainder

            int degDividend = dividend.Count - 1;
            int degDivisor = divisor.Count - 1;

            while (degDividend >= degDivisor && remainder.Any(x => x != 0))
            {
                float leadDividend = remainder.Last();
                float leadDivisor = divisor.Last();
                int degLeadDividend = remainder.Count - 1;

                float coeffQuotient = leadDividend / leadDivisor;
                int degDiff = degLeadDividend - degDivisor;
                var quotientTerm = new List<float>(new float[degDiff]);
                quotientTerm.Add(coeffQuotient);

                var divisorTerm = divisor.Select(coeff => coeff * coeffQuotient).ToList();
                divisorTerm.AddRange(new float[degDiff]);

                remainder = remainder.Zip(divisorTerm, (a, b) => a - b).ToList();

                while (remainder.Any() && remainder.Last() == 0)
                {
                    remainder.RemoveAt(remainder.Count - 1);
                }

                degDividend = remainder.Count - 1;

                quotient = quotient.Zip(quotientTerm, (a, b) => a + b).ToList();
            }

            while (quotient.Any() && quotient.Last() == 0)
            {
                quotient.RemoveAt(quotient.Count - 1);
            }

            return (quotient, remainder);
        }

        /// <summary>
        /// Calculates the greatest common divisor (GCD) of two polynomials.
        /// </summary>
        /// <param name="a">A list of coefficients of the first polynomial, from lowest to highest degree.</param>
        /// <param name="b">A list of coefficients of the second polynomial, from lowest to highest degree.</param>
        /// <returns>A list of coefficients of the GCD of the two polynomials.</returns>
        List<float> GcdPolynomials(List<float> a, List<float> b)
        {
            while (b.Any(x => x != 0))  // While b is not the zero polynomial
            {
                var (_, remainder) = PolynomialDivision(a, b);
                a = b;
                b = remainder;
            }
            return a;
        }
    }
}