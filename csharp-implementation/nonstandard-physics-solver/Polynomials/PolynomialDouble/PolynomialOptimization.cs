namespace NonstandardPhysicsSolver.Polynomials;

public partial struct PolynomialDouble
{
    public (double globalMinimumInput, double globalMinimum) FindGlobalMinimum()
    {
        double globalMinimumInput = double.NaN;
        double globalMinimum = double.PositiveInfinity;
        PolynomialDouble derivative = this.PolynomialDerivative();
        List<double> derivativeRoots = derivative.FindAllRoots();

        foreach (double root in derivativeRoots)
        {
            double evaluationAtRoot = this.EvaluatePolynomialAccurate(root);
            if (evaluationAtRoot < globalMinimum)
            {
                globalMinimumInput = root;
                globalMinimum = evaluationAtRoot;
            }
        }

        return (globalMinimumInput, globalMinimum);
    }
}
