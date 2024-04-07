using NonstandardPhysicsSolver.Intervals;

namespace NonstandardPhysicsSolver.Tests.TestUtils.TestUtilsDouble
{
    public static class AssertExtensionsDouble
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

        public static void IntervalsEqual(IntervalDouble expected, IntervalDouble actual, double tolerance = 0)
        {
            AssertWithMessage(() => ApproximateComparersDouble.IntervalsEqual(expected, actual, tolerance));
        }

        public static void AssertIntervalsContainRoots(List<IntervalDouble> intervals, List<double> expectedRoots, double tolerance = 0f)
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

        private static string IntervalsToString(List<IntervalDouble> intervals)
        {
            return string.Join(", ", intervals.Select(interval => $"[{interval.LeftBound}, {interval.RightBound}]"));
        }

        public static void ListsEqual(List<double> expected, List<double> actual)
        {
            AssertWithMessage(() => ApproximateComparersDouble.ListsEqual(expected, actual));
        }


        public static void ListsApproximatelyEqual(List<double> expected, List<double> actual, double tolerance = 1e-5f)
        {
            AssertWithMessage(() => ApproximateComparersDouble.ListsApproximatelyEqual(expected, actual, tolerance));
        }

        public static void ArraysEqual(double[] expected, double[] actual)
        {
            AssertWithMessage(() => ApproximateComparersDouble.ArraysEqual(expected, actual));
        }

        public static void ArraysApproximatelyEqual(double[] expected, double[] actual, double tolerance = 1e-5f)
        {
            AssertWithMessage(() => ApproximateComparersDouble.ArraysApproximatelyEqual(expected, actual, tolerance));
        }

        public static void DoublesApproximatelyEqual(double expected, double actual, double tolerance = 1e-5f)
        {
            AssertWithMessage(() => ApproximateComparersDouble.DoublesApproximatelyEqual(expected, actual, tolerance));
        }
    }
}