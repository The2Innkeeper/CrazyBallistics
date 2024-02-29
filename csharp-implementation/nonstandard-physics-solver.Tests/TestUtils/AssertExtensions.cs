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

        public static void ListsEqual(List<float> expected, List<float> actual)
        {
            AssertWithMessage(() => ApproximateComparers.ListsEqual(expected, actual));
        }

        public static void ListsApproximatelyEqual(List<float> expected, List<float> actual, float tolerance = 1e-5f)
        {
            AssertWithMessage(() => ApproximateComparers.ListsApproximatelyEqual(expected, actual, tolerance));
        }

        public static void FloatsApproximatelyEqual(float expected, float actual, float tolerance = 1e-5f)
        {
            AssertWithMessage(() => ApproximateComparers.FloatsApproximatelyEqual(expected, actual, tolerance));
        }
    }
}