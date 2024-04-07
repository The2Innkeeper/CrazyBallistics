namespace NonstandardPhysicsSolver.PhysicsSolver;

using NonstandardPhysicsSolver.Polynomials;
using NonstandardPhysicsSolver.Polynomials.LaurentPolynomial;
using System;

public static class VelocityMinimizer<T> where T : struct, IVector<T>
{
    /* TODO:
     * CalculateRelativeVectors(T[] targetVectors, T[] shooterVectors)
     * ExpandVelocityNumeratorPolynomial(T[] vectorCoefficients, float time)
     * ExpandVelocityLaurentPolynomial(T[] vectorCoefficients, float time) OR
     * Func<float, float> VelocitySquareMagnitude(T[] relativeVectors, float timeToTarget) // To minimize
     * Polynomial VelocityDerivativePolynomial(T[] relativeVectors) // Set to 0 (find all roots)
     * CalculateInitialVelocity(float timeToTarget)
     * CalculateMinimizedInitialVelocity()
     */

    /// <summary>
    /// Calculates the relative vectors between the target vectors and shooter vectors.
    /// R[i] = T[i] - S[i]
    /// </summary>
    /// <param name="targetVectors">The array of target vectors.</param>
    /// <param name="shooterVectors">The array of shooter vectors.</param>
    /// <returns>An array of relative vectors.</returns>
    public static T[] CalculateRelativeVectors(T[] targetVectors, T[] shooterVectors)
    {
        int maxDegree = Math.Max(targetVectors.Length, shooterVectors.Length);
        T[] relativeVectors = new T[maxDegree];

        for (int degree = 0; degree < maxDegree; degree++)
        {
            if (degree < targetVectors.Length && degree < shooterVectors.Length)
            {
                // If both target and shooter vectors exist at the current degree,
                // subtract the shooter vector from the target vector.
                relativeVectors[degree] = targetVectors[degree].Subtract(shooterVectors[degree]);
            }
            else if (degree < targetVectors.Length)
            {
                // If only the target vector exists at the current degree,
                // use the target vector as the relative vector.
                relativeVectors[degree] = targetVectors[degree];
            }
            else if (degree < shooterVectors.Length)
            {
                // If only the shooter vector exists at the current degree,
                // negate the shooter vector to get the relative vector.
                relativeVectors[degree] = shooterVectors[degree].Scale(-1);
            }
            else
            {
                // If neither target nor shooter vectors exist at the current degree,
                // use a zero vector as the relative vector.
                // This should only happen if target and shooter vectors are empty.
                relativeVectors[degree] = default(T)!.ZeroVector();
            }
        }

        return relativeVectors;
    }

    /// <summary>
    /// Expands the velocity numerator polynomial by calculating the coefficients of the polynomial
    /// that represents the square of the velocity's magnitude over time, with optimized factorial computation.
    /// </summary>
    /// <param name="scaledRelativeVectors">The array of scaled Taylor vector coefficients representing initial position, velocity, acceleration, etc.</param>
    /// <returns>A list of floats representing the coefficients of the expanded polynomial.</returns>
    public static PolynomialFloat ExpandVelocityNumeratorPolynomial(T[] scaledRelativeVectors)
    {
        int vectorCoefficientCount = scaledRelativeVectors.Length;
        int expandedPolynomialCoefficientCount = 2 * vectorCoefficientCount - 1;
        float[] expandedPolynomialCoefficients = new float[expandedPolynomialCoefficientCount];

        // Initialize scaled coefficients array.
        // T[] scaledVectorCoefficients = ScaleCoefficients(scaledVectorCoefficients);

        // Compute the coefficients of the expanded polynomial using a static local function.
        ComputeExpandedPolynomialCoefficients(scaledRelativeVectors, expandedPolynomialCoefficients);

        return new PolynomialFloat(expandedPolynomialCoefficients);


        // Static local function to compute the coefficients of the expanded polynomial.
        static void ComputeExpandedPolynomialCoefficients(T[] scaledVectorCoefficients, float[] expandedCoefficients)
        {
            int initialPolynomialDegree = scaledVectorCoefficients.Length - 1;
            for (int k = 0; k < expandedCoefficients.Length; k++)
            {
                float coefficientSum = 0f;

                // Handle the middle term separately for even k to avoid doubling it.
                if (k % 2 == 0) // k is even
                {
                    int middleIndex = k / 2;
                    for (int j = 0;
                        //int j = k < initialPolynomialDegree ? 0 : k - initialPolynomialDegree;
                        j < middleIndex;
                        j++) // Stop before the middle to leverage symmetry
                    {
                        if (j > initialPolynomialDegree || (k - j) > initialPolynomialDegree) continue;
                        // Compute the sum of dot products for the symmetric terms.
                        coefficientSum += scaledVectorCoefficients[j].Dot(scaledVectorCoefficients[k - j]);
                    }

                    // Double the sum of symmetric terms (not including the middle term for even k).
                    coefficientSum *= 2;

                    coefficientSum += scaledVectorCoefficients[middleIndex].Dot(scaledVectorCoefficients[middleIndex]); // c_{k/2} * c_{k/2}
                }
                else // k odd
                {
                    // Sum from 0 to (k-1)/2
                    int middleIndex = (k - 1) / 2;
                    for (int j = 0;
                        // int j = k < initialPolynomialDegree ? 0 : k - initialPolynomialDegree;
                        j <= middleIndex;
                        j++)
                    {
                        if (j > initialPolynomialDegree || (k - j) > initialPolynomialDegree) continue;
                        coefficientSum += scaledVectorCoefficients[j].Dot(scaledVectorCoefficients[k - j]);
                    }

                    // Double the sum of symmetric terms
                    coefficientSum *= 2;
                }

                expandedCoefficients[k] = coefficientSum;
            }
        }
    }


    /// <summary>
    /// Function to compute the scaled vector coefficients of the Taylor polynomial in T, dividing by the index factorial.
    /// </summary>
    public static T[] ScaleTaylorVectorCoefficients(T[] coefficients)
    {
        T[] scaled = new T[coefficients.Length];
        float currentFactorial = 1f; // Initialize at 1! = 1

        // Copy the first two coefficients without scaling as their scale factor is 1.
        if (coefficients.Length > 0) scaled[0] = coefficients[0];
        if (coefficients.Length > 1) scaled[1] = coefficients[1];

        for (int index = 2; index < coefficients.Length; index++)
        {
            currentFactorial *= index; // Calculate the current factorial.
            scaled[index] = coefficients[index].Scale(1f / currentFactorial); // Scale the coefficient.
        }

        return scaled;
    }

    public static Func<float, float> VelocitySquareMagnitude(T[] relativeVectors) // To minimize
    {
        return CalculateVelocitySquareMagnitude;

        float CalculateVelocitySquareMagnitude(float timeToTarget)
        {
            return ExpandVelocityNumeratorPolynomial(relativeVectors).EvaluatePolynomialHorner(timeToTarget) / MathF.Pow(timeToTarget, 2);
        }
    }

    /// <summary>
    /// Calculates the initial velocity vector based on the time to reach the target.
    /// Uses Horner's method.
    /// </summary>
    /// <param name="timeToTarget">The .</param>
    /// <returns>The result of the polynomial evaluation.</returns>
    public static T CalculateInitialVelocity(T[] scaledRelativeVectors, float timeToTarget)
    {
        int degree = scaledRelativeVectors.Length - 1;
        T hornerResult = scaledRelativeVectors[degree];
        for (int coeff_i = degree - 1; coeff_i >= 0; coeff_i--)
        {
            hornerResult = hornerResult.Scale(timeToTarget).Add(scaledRelativeVectors[coeff_i]);
        }

        T initialVelocity = hornerResult.Scale(1f / timeToTarget);
        return initialVelocity;
    }

    public static float TestPointsForMinimum(Func<float, float> functionToMinimize, List<float> inputValues)
    {
        float minimumInput = float.NaN;
        float minimumOutput = float.PositiveInfinity;

        for (int i = 0; i < inputValues.Count; i++)
        {
            float currentInput = inputValues[i];
            float currentOutput = functionToMinimize(currentInput);

            if (currentOutput < minimumOutput)
            {
                minimumInput = currentInput;
                minimumOutput = currentOutput;
            }
        }

        return minimumInput == float.PositiveInfinity ? float.NaN : minimumInput;
    }

    public static T CalculateMinimizedInitialVelocity(T[] targetVectors, T[] shooterVectors)
    {
        // Calculate relative movement vectors
        T[] relativeVectors = CalculateRelativeVectors(targetVectors, shooterVectors);

        // Find the numerator polynomial of the velocity function
        PolynomialFloat velocityNumeratorPolynomial = ExpandVelocityNumeratorPolynomial(relativeVectors);
        LaurentPolynomial velocityLaurentPolynomial = new LaurentPolynomial(velocityNumeratorPolynomial).MultiplyByXPower(-2);

        // Find the numerator of the derivative of the velocity function
        PolynomialFloat derivativeNumeratorPolynomial = velocityLaurentPolynomial.Derivative().ConvertToNumeratorPolynomial();

        // Find all roots of the derivative polynomial
        List<float> criticalTimes = derivativeNumeratorPolynomial.FindAllRoots();

        // Test all these critical points into the velocity function to find the minimum critical time
        Func<float, float> velocitySquareMagnitude = VelocitySquareMagnitude(relativeVectors);
        float optimalTimeToTarget = TestPointsForMinimum(velocitySquareMagnitude, criticalTimes);

        // Calculate the initial velocity based on the minimum critical time
        T minimizedInitialVelocity = CalculateInitialVelocity(relativeVectors, optimalTimeToTarget);

        // Return the minimum initial velocity
        return minimizedInitialVelocity;
    }



    /* DEPRECATED

    /// <summary>
    /// Evaluates a vector-valued Taylor polynomial at a specified scalar point using Horner's method.
    /// </summary>
    /// <param name="time">The scalar point at which to evaluate the polynomial.</param>
    /// <returns>The vector value of the polynomial at point x.</returns>
    /// <exception cref="ArgumentException">Thrown when the coefficients list is null or empty.</exception>
    /// <remarks>
    /// x(t)=x(0) + t*x'(0)+(t^2/2)x''(0)+(t^3/6) x'''(0)+...
    /// $$=\sum_{k=0}^{n} \frac{t^k}{ k!}x ^{ (k)} (0)$$
    /// where $n$ is the highest order non-zero derivative and 
    /// $x^{(k)}(0)$ denotes the initial value of the $k$-th position derivative.
    /// 
    /// Note: I decided against using compensated Horner's method for more precision since
    /// vector operations are more costly and it takes about 3 times more operations,
    /// which will be a significant performance drop. Usually Horner's works well for these
    /// physics applications, so it should be fine.
    /// </remarks>
    public static T EvaluateVectorTaylorExpansion(T[] vectorCoefficients, float time)
    {
        if (vectorCoefficients == null || vectorCoefficients.Length == 0)
            throw new ArgumentException("Vector coefficients list cannot be null or empty.");

        T result = vectorCoefficients[0]; // Start with the first term

        float timePower = 1; // Start with t^0 = 1
        int factorial = 1; // Start with 0! = 1

        for (int coeffIndex = 1; coeffIndex < vectorCoefficients.Length; coeffIndex++)
        {
            timePower *= time; // t^k
            factorial *= coeffIndex; // k!

            T term = vectorCoefficients[coeffIndex].Scale(timePower / factorial);
            result.Add(term);
        }

        return result;
    }
    
    /// <summary>
    /// Calculate the initial velocity based on v(T) = (T(T) - S(T)) / T
    /// </summary>
    /// <param name="relativeVectors">R(T) = T(T) - S(T)</param>
    /// <param name="timeToTarget">The time it takes to reach the target.</param>
    /// <returns>The initial velocity based on the relative movement vectors and the time to hit the target.</returns>
    public static T CalculateInitialVelocity(T[] relativeVectors, float timeToTarget)
    {
        return EvaluateVectorTaylorExpansion(relativeVectors, timeToTarget).Scale(1f / timeToTarget);
    }

    /// <summary>
    /// This is the function that we seek to minimize.
    /// Optimized version of the squared magnitude of the initial velocity vector
    /// based on relative movement vectors and the time to reach the target.
    /// </summary>
    /// <param name="relativeVectors">R(T) = T(T) - S(T)</param>
    /// <param name="timeToTarget">The time it takes to reach the target.</param>
    /// <returns>The function to minimize, calculates the squared magnitude of the initial velocity vector based on the time to hit the target.</returns>
    public static Func<float, float> VelocitySquareMagnitude(T[] relativeVectors, float timeToTarget)
    {
        return CalculateVelocitySquareMagnitude;

        // Calculate the value v(T) = x(T)/T
        float CalculateVelocitySquareMagnitude(float time)
        {
            if (relativeVectors == null || relativeVectors.Length == 0)
                throw new ArgumentException("Vector coefficients list cannot be null or empty.");

            T result = relativeVectors[0]; // Start with the first term

            float timePower = 1; // Start with t^0 = 1
            int factorial = 1; // Start with 0! = 1

            for (int coeffIndex = 1; coeffIndex < relativeVectors.Length; coeffIndex++)
            {
                timePower *= time; // t^k
                factorial *= coeffIndex; // k!

                T term = relativeVectors[coeffIndex].Scale(timePower / factorial);
                result.Add(term);
            }

            return result.MagnitudeSquared() / MathF.Pow(time, 2f);
        }
    }

    /// <summary>
    /// This is the function that we set to 0 to minimize the initial velocity vector's magnitude.
    /// The polynomial part (numerator) of the VelocitySquareMagnitude's derivative.
    /// </summary>
    /// <param name="relativeVectors">R(T) = T(T) - S(T)</param>
    /// <param name="timeToTarget">The time it takes to reach the target.</param>
    /// <returns>
    /// A <see cref="Func{float,float}"/> float -> float, the numerator of a derivative of the squared magnitude of 
    /// the initial velocity vector based on the time to hit the target.
    /// </returns>
    public static Func<float, float> VelocitySquareMagnitudeDerivativePolynomial(T[] relativeVectors, float timeToTarget)
    {
        return VelocitySquareMagnitudeDerivativePolynomial;

        // Calculate the value x(T) and T*dx/dt (T) in one pass
        // Then compute x(T).(x(T)-T*dx/dt(T))
        float VelocitySquareMagnitudeDerivativePolynomial(float time)
        {
            if (relativeVectors == null || relativeVectors.Length == 0)
                throw new ArgumentException("Vector coefficients list cannot be null or empty.");

            T x_T = relativeVectors[0]; // Start with the first term x(0)
            T TdxdT_T = default(T).ZeroVector(); // Start as zero vector

            float timePower = 1; // Start with t^0 = 1
            int derivativeFactorialCoefficient = 1; // Start with 0! = 1

            for (int coeffIndex = 1; coeffIndex < relativeVectors.Length; coeffIndex++)
            {
                timePower *= time; // t^k
                derivativeFactorialCoefficient *= coeffIndex; // k!

                // Calculate the term for x(T)
                T termForX_T = relativeVectors[coeffIndex].Scale(timePower / derivativeFactorialCoefficient);
                x_T = x_T.Add(termForX_T);

                // Calculate the term for T*dx/dT(T)
                // Note that T*dx/dT [i] = i * x[i]
                T termForTdxdt_T = termForX_T.Scale(coeffIndex);
                TdxdT_T = TdxdT_T.Add(termForTdxdt_T);
            }

            return x_T.Dot(x_T.Subtract(TdxdT_T));
        }
    }
    */
}