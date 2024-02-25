namespace nonstandard_physics_solver;

using System;
using System.Numerics;

/// <summary>
/// Provides methods for evaluating a polynomial using a nearby polynomial and correction term.
/// </summary>
public static class PolynomialEvaluator
{
    public static float EvaluatePolynomial(float[] coefficients, float x)
    {
        // Step 1: Compute a nearby polynomial and evaluation point
        (float x_adj, float[] coeffs_adj) = ComputeNearbyPolynomial(coefficients, x);
        
        // Step 2: Rearrange the original polynomial in terms of the nearby polynomial
        // and compute the correction term
        float correction_term = ComputeCorrectionTerm(coefficients, x, x_adj);
        
        // Step 3: Compute the constants for the accurate evaluation
        float[] constants = ComputeConstants(coeffs_adj, x_adj);
        
        // Step 4: Evaluate the polynomial using the nearby polynomial and correction term
        float result = EvaluatePolynomialHorner(x_adj, coeffs_adj) + correction_term;
        
        return result;

        /// <summary>
        /// Computes a nearby polynomial using the given coefficients and x value.
        /// </summary>
        /// <param name="coefficients">The coefficients of the polynomial.</param>
        /// <param name="x">The x value at which to compute the nearby polynomial.</param>
        /// <returns>A tuple containing the adjusted x value and adjusted coefficients.</returns>
        (float, float[]) ComputeNearbyPolynomial(float[] coefficients, float x)
        {
            float x_adj = TruncateToHalfPrecision(x);
            float[] coeffs_adj = new float[coefficients.Length];
            float H = 0;
            for (int i = coefficients.Length - 1; i >= 0; i--)
            {
                float S = TruncateToHalfPrecision(H * x_adj);
                H = TruncateToHalfPrecision(S + coefficients[i]);
                coeffs_adj[i] = H - S;
            }
            return (x_adj, coeffs_adj);
        }

        /// <summary>
        /// Computes the correction term based on the given coefficients, x, and x_adj parameters.
        /// </summary>
        /// <param name="coefficients">The coefficients of the polynomial.</param>
        /// <param name="x">The x value at which to compute the correction term.</param>
        /// <param name="x_adj">The adjusted x value.</param>
        /// <returns>The correction term.</returns>
        float ComputeCorrectionTerm(float[] coefficients, float x, float x_adj)
        {
            float correction_term = 0;
            for (int i = 1; i < coefficients.Length; i++)
            {
                float term = coefficients[i] * (MathF.Pow(x, i) - MathF.Pow(x_adj, i));
                correction_term += term / (x - x_adj);
            }
            return correction_term;
        }

        /// <summary>
        /// Computes the constants for the given coefficients and x value.
        /// </summary>
        /// <param name="coeffs_adj">The adjusted coefficients of the polynomial.</param>
        /// <param name="x_adj">The adjusted x value.</param>
        /// <returns>The constants for the polynomial evaluation.</returns>
        float[] ComputeConstants(float[] coeffs_adj, float x_adj)
        {
            float[] constants = new float[coeffs_adj.Length - 1];
            for (int j = 0; j < coeffs_adj.Length - 1; j++)
            {
                float Cj = 0;
                for (int i = 0; i < coeffs_adj.Length - 1 - j; i++)
                {
                    Cj += coeffs_adj[j + 1 + i] * MathF.Pow(x_adj, i);
                }
                constants[j] = Cj;
            }
            return constants;
        }

        /// <summary>
        /// Truncates a float to half precision.
        /// </summary>
        /// <param name="value">The float value to truncate.</param>
        /// <returns>The truncated float value.</returns>
        float TruncateToHalfPrecision(float value)
        {
            Half halfValue = (Half)value;
            return (float)halfValue;
        }
        /// <summary>
        /// Evaluates a polynomial using a nearby polynomial and correction term.
        /// </summary>
        /// <param name="coefficients">The coefficients of the polynomial.</param>
        /// <param name="x">The x value at which to evaluate the polynomial.</param>
        /// <returns>The result of the polynomial evaluation.</returns>
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
        for (int i = degree - 1; i >= 0; i--)
        {
            hornerResult = hornerResult * inputValue + polynomialCoefficients[i];
        }
        return hornerResult;
    }
}