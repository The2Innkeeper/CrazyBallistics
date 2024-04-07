namespace NonstandardPhysicsSolver.Tests.TestUtils.TestUtilsDouble;

using NonstandardPhysicsSolver.Intervals;

public static class ApproximateComparersDouble
{
    public static void IntervalsEqual(IntervalDouble expected, IntervalDouble actual, double tolerance = 1e-5f)
    {
        const int N_ELEMENTS = 2;
        for (int i = 0; i < N_ELEMENTS; i++)
        {
            if (Math.Abs(expected[i] - actual[i]) > tolerance)
            {
                throw new ArgumentException($"Lists differ at index {i}. Expected: {expected[i]}, Actual: {actual[i]}, Tolerance: {tolerance}");
            }
        }
    }

    public static void ListsEqual(List<double> expected, List<double> actual)
    {
        if (expected == null || actual == null)
        {
            ArgumentNullException argumentNullException = new($"One of the lists is null. {expected}, {actual}");
            throw argumentNullException;
        }

        if (expected.Count != actual.Count)
            throw new ArgumentException($"List counts do not match. Expected count: {expected.Count}, Actual count: {actual.Count}.");

        for (int i = 0; i < expected.Count; i++)
        {
            if (expected[i] != actual[i])
            {
                throw new ArgumentException($"Lists differ at index {i}. Expected: {expected[i]}, Actual: {actual[i]}");
            }
        }
    }

    public static void ListsApproximatelyEqual(List<double> expected, List<double> actual, double tolerance = 1e-5f)
    {
        if (expected == null || actual == null)
        {
            ArgumentNullException argumentNullException = new($"One of the lists is null. {expected}, {actual}");
            throw argumentNullException;
        }

        if (expected.Count != actual.Count)
            throw new ArgumentException($"List counts do not match. Expected count: {expected.Count}, Actual count: {actual.Count}.");

        for (int i = 0; i < expected.Count; i++)
        {
            if (Math.Abs(expected[i] - actual[i]) > tolerance)
            {
                throw new ArgumentException($"Lists differ at index {i}. Expected: {expected[i]}, Actual: {actual[i]}, Tolerance: {tolerance}.");
            }
        }
    }


    public static void ArraysEqual(double[] expected, double[] actual)
    {
        if (expected == null || actual == null)
        {
            ArgumentNullException argumentNullException = new($"One of the arrays is null. {expected}, {actual}");
            throw argumentNullException;
        }

        int expectedLength = expected.Length;
        int actualLength = actual.Length;

        if (expectedLength != actualLength)
            throw new ArgumentException($"Array counts do not match. Expected count: {expectedLength}, Actual count: {actualLength}.");

        for (int i = 0; i < expectedLength; i++)
        {
            if (expected[i] != actual[i])
            {
                throw new ArgumentException($"Arrays differ at index {i}. Expected: {expected[i]}, Actual: {actual[i]}");
            }
        }
    }

    public static void ArraysApproximatelyEqual(double[] expected, double[] actual, double tolerance = 1e-5f)
    {
        if (expected == null || actual == null)
        {
            ArgumentNullException argumentNullException = new($"One of the arrays is null. {expected}, {actual}");
            throw argumentNullException;
        }

        int expectedLength = expected.Length;
        int actualLength = actual.Length;

        if (expectedLength != actualLength)
            throw new ArgumentException($"Array counts do not match. Expected count: {expectedLength}, Actual count: {actualLength}.");

        for (int i = 0; i < expectedLength; i++)
        {
            if (Math.Abs(expected[i] - actual[i]) > tolerance)
            {
                throw new ArgumentException($"Arrays differ at index {i}. Expected: {expected[i]}, Actual: {actual[i]}, Tolerance: {tolerance}.");
            }
        }
    }

    public static void DoublesApproximatelyEqual(double expected, double actual, double tolerance = 1e-5f)
    {
        if (Math.Abs(expected - actual) > tolerance)
        {
            throw new ArgumentException($"Doubles are not approximately equal. Expected: {expected}, Actual: {actual}, Tolerance: {tolerance}.");
        }
    }
}