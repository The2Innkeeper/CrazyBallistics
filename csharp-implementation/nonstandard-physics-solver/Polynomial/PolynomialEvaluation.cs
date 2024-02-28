namespace NonstandardPhysicsSolver.Polynomials;

public partial class Polynomial
{
    /// <summary>
    /// Evaluates a polynomial at a specified point using compensated Horner's method. It takes on average around 2, up to 3 times as many calculations as Horner's method.
    /// </summary>
    /// <param name="x">The point at which to evaluate the polynomial.</param>
    /// <returns>The value of the polynomial at point x.</returns>
    /// <exception cref="ArgumentException">Thrown when the coefficients list is null or empty.</exception>
    public float EvaluatePolynomialAccurate(float x)
    {
        if (Coefficients == null || Coefficients.Count == 0)
            throw new ArgumentException("Coefficients list cannot be null or empty.");

        float sum = Coefficients[^1]; // Start with the last coefficient
        float comp = 0.0f; // Compensation for lost low-order bits

        for (int coeff_i = Coefficients.Count - 2; coeff_i >= 0; coeff_i--)
        {
            float y = Coefficients[coeff_i] + comp; // Add the compensation
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
    /// <returns>The result of the polynomial evaluation.</returns>
    public float EvaluatePolynomialHorner(float inputValue)
    {
        int degree = Coefficients.Count - 1;
        float hornerResult = Coefficients[degree];
        for (int coeff_i = degree - 1; coeff_i >= 0; coeff_i--)
        {
            hornerResult = hornerResult * inputValue + Coefficients[coeff_i];
        }
        return hornerResult;
    }
}