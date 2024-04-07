namespace NonstandardPhysicsSolver.Polynomials;

public static class PolynomialUtils
{
    public static float[] NormalizedCoefficients(PolynomialFloat polynomial)
    {
        // Clone the coefficients array properly and cast to float[] if necessary
        var coefficients = polynomial.Coefficients.Clone() as float[];
        if (coefficients == null || coefficients.Length == 0) return []; // Ensure there's at least one coefficient to avoid division by zero

        float scalingFactor = coefficients[^1]; // Use the last coefficient as the scaling factor
        // Normalize coefficients and convert the result back to an array
        return coefficients.Select(c => c / scalingFactor).ToArray();
    }
}