using nonstandard_physics_solver;

namespace nonstandard_physics_solver.Tests
{
    public class PolynomialEvaluatorTests
    {
        /// <summary>
        /// Test method to evaluate a polynomial using Horner's method.
        /// This test checks if the evaluation of the polynomial at a given positive input value
        /// returns the correct result.
        /// </summary>
        [Fact]
        public void EvaluatePolynomialHorner_ZeroInputValue_ReturnsCorrectEvaluation()
        {
            // Arrange
            // Set the input value for which the polynomial is to be evaluated
            float inputValue = 0f;

            // Define the coefficients of the polynomial in decreasing powers
            // For this test, the polynomial is 11 -32x +2x^2
            float[] polynomialCoefficients = { 11, -32, 2 };

            // Set the expected result after evaluating the polynomial at the inputValue using Horner's method
            float expected = 11f; // The polynomial evaluated at x=0 is equal to the constant term
                                  // Act
                                  // Evaluate the polynomial at the given inputValue using Horner's method
            float result = Polynomial.EvaluatePolynomialHorner(inputValue, polynomialCoefficients);

            // Assert
            // Verify that the result is equal to the expected value with a precision of 4 decimal places
            Assert.Equal((float)expected, (float)result, (float)0.002f);
        }

        // A test for evaluating the polynomial using Horner's method with a negative input value and verifying the correct evaluation.
        [Fact]
        public void EvaluatePolynomialHorner_NegativeInputValue_ReturnsCorrectEvaluation()
        {
            // Arrange
            float inputValue = -1.5f;
            float[] polynomialCoefficients = { 2, 3, 5 };
            float expected = 8.75f; // Replace with the expected result after calculation

            // Act
            float result = Polynomial.EvaluatePolynomialHorner(inputValue, polynomialCoefficients);

            // Assert
            Assert.Equal((float)expected, (float)result, (float)4); // Adjust the precision as needed
        }

        [Fact]
        public void TestMakeSquareFree()
        {
            // Example polynomial: x^2 - 2x + 1, which is (x-1)^2 and should be reduced to (x-1)
            var coefficients = new List<double> { 1, -2, 1 };
            var expected = new List<double> { 1, -1 }; // Expected result after making squarefree

            var result = Polynomial.MakeSquareFree(coefficients);

            Assert.Equal(expected, result);
        }
    }
}
