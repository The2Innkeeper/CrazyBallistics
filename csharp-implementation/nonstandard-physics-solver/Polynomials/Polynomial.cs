namespace NonstandardPhysicsSolver.Polynomials;
using System;
using System.Collections.Generic;

public partial struct Polynomial
{
    /// <summary>
    /// List of coefficients in increasing order of degree
    /// </summary>
    public List<float> Coefficients { get; private set; }

    public Polynomial(List<float> coefficients)
    {
        if (coefficients == null || coefficients.Count == 0)
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
        if (index < 0 || index >= Coefficients.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Index is out of range.");

        // Update the coefficient at the given index
        Coefficients[index] = newValue;
    }

    /// <summary>
    /// Adds a new coefficient to the end of the Coefficients list, effectively increasing the degree.
    /// </summary>
    /// <param name="newCoefficient">The value of the new coefficient to be added.</param>
    public readonly void AddCoefficient(float newCoefficient)
    {
        // Add the new coefficient to the end of the list
        Coefficients.Add(newCoefficient);
    }
}
