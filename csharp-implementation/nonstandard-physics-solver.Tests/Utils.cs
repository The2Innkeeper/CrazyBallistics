namespace Utils;

public static class AssertExtensions
{
    private const float RelativeErrorThreshold = 1e-5f; // Adjust based on required precision

    public static void AssertEqualWithRelativeError(float expected, float actual, float threshold = RelativeErrorThreshold)
    {
        // Handle the special case where the expected value is zero
        if (expected == 0f)
        {
            Assert.True(Math.Abs(actual) <= threshold, $"Expected: {expected}, Actual: {actual}, Absolute Error: {Math.Abs(actual)}");
            return;
        }

        float relativeError = Math.Abs((expected - actual) / expected);
        Assert.True(relativeError <= threshold, $"Expected: {expected}, Actual: {actual}, Relative Error: {relativeError}");
    }

    public static void AssertEqualWithRelativeError(IEnumerable<float> expected, IEnumerable<float> actual, float threshold = RelativeErrorThreshold)
    {
        var expectedList = expected.ToList();
        var actualList = actual.ToList();

        Assert.Equal(expectedList.Count, actualList.Count); // Ensure they have the same number of elements

        for (int i = 0; i < expectedList.Count; i++)
        {
            AssertEqualWithRelativeError(expectedList[i], actualList[i], threshold);
        }
    }
}

