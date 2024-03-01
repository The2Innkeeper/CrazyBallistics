namespace NonstandardPhysicsSolver.Polynomials;
using System;

public partial struct Polynomial
{
    /// <summary>
    /// List of coefficients in increasing order of degree
    /// </summary>
    public readonly float[] Coefficients { get; }

    public Polynomial(float[] coefficients)
    {
        if (coefficients == null || coefficients.Length == 0)
            throw new ArgumentException("Coefficients list cannot be null or empty.", nameof(coefficients));
        Coefficients = coefficients;
    }

    /// <summary>
    /// Updates the coefficient at a specified index within the Coefficients list.
    /// </summary>
    /// <param name="index">The zero-based index where the coefficient is to be updated.</param>
    /// <param name="newValue">The new value of the coefficient at the specified index.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is outside the bounds of the Coefficients list.</exception>
    public readonly void UpdateCoefficient(int index, float newValue)
    {
        // Validate the index before attempting to update
        if (index < 0 || index >= Coefficients.Length)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        // Update the coefficient at the given index
        Coefficients[index] = newValue;
    }
}
