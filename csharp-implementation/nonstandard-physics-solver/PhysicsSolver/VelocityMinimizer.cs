namespace NonstandardPhysicsSolver.PhysicsSolver;
using System;

public static class VelocityMinimizer<T> where T : struct, IVector<T>
{
    /* TODO:
     * CalculateRelativeVectors(T[] targetVectors, T[] shooterVectors)
     * EvaluateVectorTaylorExpansion(T[] vectorCoefficients, float time)
     * CalculateInitialVelocity(float timeToTarget)
     * Func<float, float> VelocitySquareMagnitude(T[] relativeVectors, float timeToTarget) // To minimize
     * Polynomial VelocityDerivativePolynomial(T[] relativeVectors, float timeToTarget) // Set to 0 (find all roots)
     * CalculateMinimizedInitialVelocity()
     * 
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
}