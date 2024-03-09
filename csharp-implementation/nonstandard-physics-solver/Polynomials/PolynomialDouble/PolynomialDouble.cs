namespace NonstandardPhysicsSolver.Polynomials;
using System;

public partial struct PolynomialDouble
{
    /// <summary>
    /// List of coefficients in increasing order of degree
    /// </summary>
    public double[] Coefficients { get; }

    public PolynomialDouble(double[] coefficients)
    {
        if (coefficients == null || coefficients.Length == 0)
            throw new ArgumentException("Coefficients list cannot be null or empty.", nameof(coefficients));

        for (int i = 0; i < coefficients.Length; i++) // Use for loop to have an index
        {
            double coefficient = coefficients[i];
            if (double.IsNaN(coefficient))
            {
                throw new ArgumentException($"NaN detected in coefficient at index {i}." +
                    $"\nPolynomial coefficients: {string.Join(", ", coefficients)}");
            }
        }

        Coefficients = coefficients;
    }

    public static PolynomialDouble FromDoubleArray(double[] doubleCoefficients)
    {
        if (doubleCoefficients == null || doubleCoefficients.Length == 0)
        {
            throw new ArgumentException("Coefficients list cannot be null or empty.", nameof(doubleCoefficients));
        }

        // Initialize the double array with the same length as the input double array
        double[] floatCoefficients = new double[doubleCoefficients.Length];

        // Convert each double coefficient to double
        for (int i = 0; i < doubleCoefficients.Length; i++)
        {
            double coefficient = doubleCoefficients[i];
            if (double.IsNaN(coefficient))
            {
                throw new ArgumentException($"NaN detected in coefficient at index {i}." +
                    $"\nPolynomial coefficients: {string.Join(", ", doubleCoefficients)}");
            }

            // Explicitly cast the double to double and store it
            floatCoefficients[i] = (double)coefficient;
        }

        // Assign the converted double array to the Coefficients property
        return new PolynomialDouble(floatCoefficients);
    }

    /// <summary>
    /// Updates the coefficient at a specified index within the Coefficients list.
    /// </summary>
    /// <param name="index">The zero-based index where the coefficient is to be updated.</param>
    /// <param name="newValue">The new value of the coefficient at the specified index.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the bounds of the Coefficients list.</exception>
    public void UpdateCoefficient(int index, double newValue)
    {
        // Validate the index before attempting to update
        if (index < 0 || index >= Coefficients.Length)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        // Update the coefficient at the given index
        Coefficients[index] = newValue;
    }
}
