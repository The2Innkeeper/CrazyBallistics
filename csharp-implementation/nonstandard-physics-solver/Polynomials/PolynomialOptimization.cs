namespace NonstandardPhysicsSolver.Polynomials;

public partial struct Polynomial
{
    public (float globalMinimumInput, float globalMinimum) FindGlobalMinimum()
    {
        float globalMinimumInput = float.NaN;
        float globalMinimum = float.PositiveInfinity;
        Polynomial derivative = this.PolynomialDerivative();
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
