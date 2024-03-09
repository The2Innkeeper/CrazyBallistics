namespace NonstandardPhysicsSolver.Polynomials;

public partial struct PolynomialDouble
{
    /// <summary>
    /// Evaluates a polynomial at a specified point using compensated Horner's method. It takes on average around 2, up to 3 times as many calculations as Horner's method.
    /// </summary>
    /// <param name="evaluationPoint">The point at which to evaluate the polynomial.</param>
    /// <returns>The value of the polynomial at the specified evaluation point.</returns>
    /// <exception cref="ArgumentException">Thrown when the coefficients list is null or empty.</exception>
    public double EvaluatePolynomialAccurate(double evaluationPoint)
    {
        if (Coefficients == null || Coefficients.Length == 0)
            throw new ArgumentException("Coefficients list cannot be null or empty.");

        double polynomialSum = Coefficients[^1]; // Start with the last coefficient
        double compensation = 0.0f; // Compensation for lost low-order bits

        for (int coefficientIndex = Coefficients.Length - 2; coefficientIndex >= 0; coefficientIndex--)
        {
            double coefficientWithCompensation = Coefficients[coefficientIndex] + compensation; // Add the compensation
            double partialResult = polynomialSum * evaluationPoint + coefficientWithCompensation; // Horner's method step
            compensation = partialResult - polynomialSum * evaluationPoint - coefficientWithCompensation; // Update the compensation
            polynomialSum = partialResult; // Update the polynomial sum
        }

        return polynomialSum;
    }

    /// <summary>
    /// Evaluates a polynomial at a given point using Horner's method.
    /// </summary>
    /// <param name="inputValue">The input value at which to evaluate the polynomial.</param>
    /// <returns>The result of the polynomial evaluation.</returns>
    public double EvaluatePolynomialHorner(double inputValue)
    {
        int degree = Coefficients.Length - 1;
        double hornerResult = Coefficients[degree];
        for (int coeff_i = degree - 1; coeff_i >= 0; coeff_i--)
        {
            hornerResult = hornerResult * inputValue + Coefficients[coeff_i];
        }
        return hornerResult;
    }
}