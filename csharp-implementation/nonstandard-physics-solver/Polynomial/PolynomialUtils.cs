using NonstandardPhysicsSolver.Polynomials;

namespace PolynomialUtils;

public static class PolynomialUtils
{
    public static List<float> NormalizedCoefficients(Polynomial polynomial)
    {
        var coefficients = new List<float>(polynomial.Coefficients);
        float scalingFactor = coefficients[^1];
        return coefficients.Select(c => c / scalingFactor).ToList();
    }
}