namespace NonstandardPhysicsSolver.Tests.TestUtils;

using NonstandardPhysicsSolver.Intervals;

public static class ApproximateComparers
{
    public static void IntervalsEqual(Interval expected, Interval actual, float tolerance = 1e-5f)
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

    public static void ListsEqual(List<float> expected, List<float> actual)
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

    public static void ListsApproximatelyEqual(List<float> expected, List<float> actual, float tolerance = 1e-5f)
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

    public static void ArraysEqual(float[] expected, float[] actual)
    {
        if (expected == null || actual == null)
        {
            throw new ArgumentNullException($"One of the arrays is null. Expected: {FormatArray(expected)}, Actual: {FormatArray(actual)}");
        }

        int expectedLength = expected.Length;
        int actualLength = actual.Length;

        if (expectedLength != actualLength)
            throw new ArgumentException($"Array counts do not match. Expected count: {expectedLength}, Actual count: {actualLength}. Expected: {FormatArray(expected)}, Actual: {FormatArray(actual)}");

        for (int i = 0; i < expectedLength; i++)
        {
            if (expected[i] != actual[i])
            {
                throw new ArgumentException($"Arrays differ at index {i}. Expected: {expected[i]}, Actual: {actual[i]}." +
                    $"\nFull arrays - Expected: {FormatArray(expected)}, Actual: {FormatArray(actual)}");
            }
        }
    }

    public static void ArraysApproximatelyEqual(float[] expected, float[] actual, float tolerance = 1e-5f)
    {
        if (expected == null || actual == null)
        {
            throw new ArgumentNullException($"One of the arrays is null. Expected: {FormatArray(expected)}, Actual: {FormatArray(actual)}");
        }

        int expectedLength = expected.Length;
        int actualLength = actual.Length;

        if (expectedLength != actualLength)
            throw new ArgumentException($"Array counts do not match. Expected count: {expectedLength}, Actual count: {actualLength}. Expected: {FormatArray(expected)}, Actual: {FormatArray(actual)}");

        for (int i = 0; i < expectedLength; i++)
        {
            if (Math.Abs(expected[i] - actual[i]) > tolerance)
            {
                throw new ArgumentException($"Arrays differ at index {i}. Expected: {expected[i]}, Actual: {actual[i]}, Tolerance: {tolerance}." +
                    $"\nFull arrays - Expected: {FormatArray(expected)}, Actual: {FormatArray(actual)}");
            }
        }
    }

    // Helper method to format an array as a string
    private static string FormatArray(float[] array)
    {
        if (array == null) return "null";
        return "[" + string.Join(", ", array.Select(x => x.ToString())) + "]";
    }

    public static void FloatsApproximatelyEqual(float expected, float actual, float tolerance = 1e-5f)
    {
        if (Math.Abs(expected - actual) > tolerance)
        {
            throw new ArgumentException($"Floats are not approximately equal. Expected: {expected}, Actual: {actual}, Tolerance: {tolerance}.");
        }
    }
}