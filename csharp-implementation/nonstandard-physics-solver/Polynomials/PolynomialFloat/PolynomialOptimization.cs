namespace NonstandardPhysicsSolver.Polynomials;

public partial struct PolynomialFloat
{
    public (float globalMinimumInput, float globalMinimum) FindGlobalMinimum()
    {
        float globalMinimumInput = float.NaN;
        float globalMinimum = float.PositiveInfinity;
        PolynomialFloat derivative = this.PolynomialDerivative();
        List<float> derivativeRoots = derivative.FindAllRoots();

        foreach (float root in derivativeRoots)
        {
            float evaluationAtRoot = this.EvaluatePolynomialAccurate(root);
            if (evaluationAtRoot < globalMinimum)
            {
                globalMinimumInput = root;
                globalMinimum = evaluationAtRoot;
            }
        }

        return (globalMinimumInput, globalMinimum);
    }
}
