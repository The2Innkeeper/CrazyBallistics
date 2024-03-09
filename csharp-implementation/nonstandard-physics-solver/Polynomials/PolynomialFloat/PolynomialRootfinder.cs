using NonstandardPhysicsSolver.Intervals;

namespace NonstandardPhysicsSolver.Polynomials;

public partial struct PolynomialFloat
{
    public List<float> FindAllRoots(float precision = 1e-5f)
    {
        List<float> roots = [];
        PolynomialFloat squarefreePolynomial = this.MakeSquarefree();
        //List<Interval> isolatedRootIntervals = squarefreePolynomial.IsolatePositiveRootIntervalsBisection();
        List<Interval> isolatedRootIntervals = squarefreePolynomial.IsolatePositiveRootIntervalsContinuedFractions();

        foreach (Interval interval in isolatedRootIntervals)
        {
            float root = Interval.RefineRootIntervalBisection(squarefreePolynomial.EvaluatePolynomialAccurate, interval, precision);
            //float root = Interval.RefineRootIntervalITP(squarefreePolynomial.EvaluatePolynomialAccurate, interval, precision);
            roots.Add(root);
        }

        return roots;
    }
}
