namespace NonstandardPhysicsSolver.Polynomials;
using System;

public partial struct Polynomial
{
    /// <summary>
    /// List of coefficients in increasing order of degree
    /// </summary>
    public float[] Coefficients { get; }

    public Polynomial(float[] coefficients)
    {
        if (coefficients == null || coefficients.Length == 0)
            throw new ArgumentException("Coefficients list cannot be null or empty.", nameof(coefficients));

        for (int i = 0; i < coefficients.Length; i++) // Use for loop to have an index
        {
            float coefficient = coefficients[i];
            if (float.IsNaN(coefficient))
            {
                throw new ArgumentException($"NaN detected in coefficient at index {i}." +
                    $"\nPolynomial coefficients: {string.Join(", ", coefficients)}");
            }
        }

        Coefficients = coefficients;
    }

    public static Polynomial FromDoubleArray(double[] doubleCoefficients)
    {
        if (doubleCoefficients == null || doubleCoefficients.Length == 0)
        {
            throw new ArgumentException("Coefficients list cannot be null or empty.", nameof(doubleCoefficients));
        }

        // Initialize the float array with the same length as the input double array
        float[] floatCoefficients = new float[doubleCoefficients.Length];

        // Convert each double coefficient to float
        for (int i = 0; i < doubleCoefficients.Length; i++)
        {
            double coefficient = doubleCoefficients[i];
            if (double.IsNaN(coefficient))
            {
                throw new ArgumentException($"NaN detected in coefficient at index {i}." +
                    $"\nPolynomial coefficients: {string.Join(", ", doubleCoefficients)}");
            }

            // Explicitly cast the double to float and store it
            floatCoefficients[i] = (float)coefficient;
        }

        // Assign the converted float array to the Coefficients property
        return new Polynomial(floatCoefficients);
    }

    /// <summary>
    /// Updates the coefficient at a specified index within the Coefficients list.
    /// </summary>
    /// <param name="index">The zero-based index where the coefficient is to be updated.</param>
    /// <param name="newValue">The new value of the coefficient at the specified index.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the bounds of the Coefficients list.</exception>
    public void UpdateCoefficient(int index, float newValue)
    {
        // Validate the index before attempting to update
        if (index < 0 || index >= Coefficients.Length)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        // Update the coefficient at the given index
        Coefficients[index] = newValue;
    }
}
