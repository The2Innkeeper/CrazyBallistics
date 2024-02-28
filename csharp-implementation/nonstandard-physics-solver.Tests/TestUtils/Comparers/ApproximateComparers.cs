namespace NonstandardPhysicsSolver.Polynomial.Tests.TestUtils.Comparers;
public static class ApproximateComparers
{
    public static void AssertListApproximatelyEquals(List<float> a, List<float> b, float tolerance = 1e-5f)
    {
        if (a == null || b == null)
            throw new ArgumentNullException("One of the lists is null.");

        if (a.Count != b.Count)
            throw new ArgumentException($"List counts do not match. Expected count: {a.Count}, Actual count: {b.Count}.");

        for (int i = 0; i < a.Count; i++)
        {
            if (Math.Abs(a[i] - b[i]) > tolerance)
            {
                throw new ArgumentException($"Lists differ at index {i}. Expected: {a[i]}, Actual: {b[i]}, Tolerance: {tolerance}.");
            }
        }
    }
}