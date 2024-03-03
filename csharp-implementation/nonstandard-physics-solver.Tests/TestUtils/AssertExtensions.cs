using NonstandardPhysicsSolver.Intervals;

namespace NonstandardPhysicsSolver.Polynomials.Tests.TestUtils
{
    public static class AssertExtensions
    {
        // Generic method to handle assertion logic
        private static void AssertWithMessage(Action assertionAction)
        {
            try
            {
                assertionAction();
            }
            catch (ArgumentException ex)
            {
                Assert.True(false, ex.Message);
            }
        }

        public static void IntervalsEqual(Interval expected, Interval actual, float tolerance = 0)
        {
            AssertWithMessage(() => ApproximateComparers.IntervalsEqual(expected, actual, tolerance));
        }

        public static void AssertIntervalsContainRoots(List<Interval> intervals, List<float> expectedRoots, float tolerance = 0f)
        {
            // Always print intervals for diagnostic purposes, regardless of the assertion outcome
            string intervalsStr = IntervalsToString(intervals);

            // Check if the number of intervals matches the number of expected roots and print intervals in the message
            Assert.True(expectedRoots.Count == intervals.Count, $"Expected {expectedRoots.Count} roots but found {intervals.Count} intervals.\nIntervals: {intervalsStr}");

            foreach (var root in expectedRoots)
            {
                bool containsRoot = intervals.Any(interval => interval.LeftBound <= root + tolerance && interval.RightBound >= root - tolerance);
                Assert.True(containsRoot, $"Expected to find a root at {root} within the intervals, but it was not found.\nIntervals: {intervalsStr}");
            }
        }

        private static string IntervalsToString(List<Interval> intervals)
        {
            return string.Join(", ", intervals.Select(interval => $"[{interval.LeftBound}, {interval.RightBound}]"));
        }

        public static void ListsEqual(List<float> expected, List<float> actual)
        {
            AssertWithMessage(() => ApproximateComparers.ListsEqual(expected, actual));
        }


        public static void ListsApproximatelyEqual(List<float> expected, List<float> actual)
        {
            AssertWithMessage(() => ApproximateComparers.ListsApproximatelyEqual(expected, actual));
        }

        public static void ArraysEqual(float[] expected, float[] actual)
        {
            AssertWithMessage(() => ApproximateComparers.ArraysEqual(expected, actual));
        }

        public static void ArraysApproximatelyEqual(float[] expected, float[] actual, float tolerance = 1e-5f)
        {
            AssertWithMessage(() => ApproximateComparers.ArraysApproximatelyEqual(expected, actual, tolerance));
        }

        public static void FloatsApproximatelyEqual(float expected, float actual, float tolerance = 1e-5f)
        {
            AssertWithMessage(() => ApproximateComparers.FloatsApproximatelyEqual(expected, actual, tolerance));
        }
    }
}