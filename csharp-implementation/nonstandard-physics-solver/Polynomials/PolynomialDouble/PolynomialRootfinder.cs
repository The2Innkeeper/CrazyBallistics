using NonstandardPhysicsSolver.Intervals;

namespace NonstandardPhysicsSolver.Polynomials;

public partial struct PolynomialDouble
{
    public List<double> FindAllRoots(double precision = 1e-5f)
    {
        List<double> roots = [];
        PolynomialDouble squarefreePolynomial = this.MakeSquarefree();
        List<IntervalDouble> isolatedRootIntervals = squarefreePolynomial.IsolatePositiveRootIntervalsBisection();
        //List<IntervalDouble> isolatedRootIntervals = squarefreePolynomial.IsolatePositiveRootIntervalsContinuedFractions();

        foreach (IntervalDouble interval in isolatedRootIntervals)
        {
            double root = IntervalDouble.RefineRootIntervalBisection(squarefreePolynomial.EvaluatePolynomialAccurate, interval, precision);
            //double root = IntervalDouble.RefineRootIntervalITP(squarefreePolynomial.EvaluatePolynomialAccurate, interval, precision);
            roots.Add(root);
        }

        return roots;
    }
}
