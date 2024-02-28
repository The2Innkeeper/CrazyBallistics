namespace NonstandardPhysicsSolver.Polynomials;

public partial struct Polynomial
{
    /// <summary>
    /// Finds a root of the polynomial within the specified interval using an iterative refinement process.
    /// The method combines interpolation, truncation, and projection (ITP) steps to converge towards a single root,
    /// offering superlinear convergence on average and linear convergence in the worst case.
    /// </summary>
    /// <param name="leftBound">The left boundary of the interval to search for a root.</param>
    /// <param name="rightBound">The right boundary of the interval to search for a root.</param>
    /// <param name="tol">The tolerance for convergence. The method aims to find a root within an interval of size less than or equal to 2 * tol. Default is 0.0005f.</param>
    /// <param name="k1">A coefficient used in the calculation of the truncation step. Default if unchanged is 0.2f / (rightBound - leftBound).</param>
    /// <param name="k2">A coefficient used in the calculation of the delta for the truncation step, affecting the interpolation robustness. Default is 2f.</param>
    /// <param name="n0">The initial offset for the maximum number of iterations, contributing to the calculation of the dynamic range for the interpolation step. Default is 1.</param>
    /// <param name="maxIterations">The maximum number of iterations to perform. This prevents the method from running indefinitely. Default is 50.</param>
    /// <returns>The approximate position of the root within the specified interval, determined to be within the specified tolerance.</returns>
    /// <remarks>
    /// This method employs an iterative technique that refines the interval containing the root by evaluating the polynomial's sign changes.
    /// It dynamically adjusts the interval based on the polynomial's behavior, using interpolation, truncation, and projection steps
    /// to efficiently converge towards the root. The method is designed to work with polynomials where a single root exists within the given interval.
    /// </remarks>
    public float RefineIntervalITP(float leftBound, float rightBound, float tol = 1e-5f, int maxIterations = 50, float k1 = float.NaN, float k2 = 2f, int n0 = 1)
    {
        // Preprocessing
        if (float.IsNaN(k1))
        {
            k1 = 0.2f / (rightBound - leftBound); // Value determined experimentally
        }
        if (Coefficients.Count > 10)
        {
            return (float)RefineIntervalITP((double)leftBound, (double)rightBound, (double)tol, maxIterations, (double)k1, (double)k2, n0);
        }
        int nHalf = (int)Math.Ceiling(Math.Log((rightBound - leftBound) / (2 * tol), 2));
        int nMax = nHalf + n0;
        bool increasing = EvaluatePolynomialAccurate(leftBound) < EvaluatePolynomialAccurate(rightBound);

        // Iterate until convergence or maximum number of iterations is reached
        for (int iterations = 0; iterations < maxIterations && rightBound - leftBound > 2 * tol; iterations++)
        {
            // Calculating Parameters
            float xHalf = (leftBound + rightBound) / 2;
            float r = tol * (float)Math.Pow(2, nMax - iterations) - (rightBound - leftBound) / 2;
            float delta = k1 * (float)Math.Pow(rightBound - leftBound, k2);

            // Interpolation
            float xF = (rightBound * EvaluatePolynomialAccurate(leftBound) - leftBound * EvaluatePolynomialAccurate(rightBound)) / (EvaluatePolynomialAccurate(leftBound) - EvaluatePolynomialAccurate(rightBound));

            // Truncation
            float sigma = Math.Sign(xHalf - xF);
            float xT = Math.Abs(xHalf - xF) >= delta ? xF + sigma * delta : xHalf;

            // Projection
            float xITP = Math.Abs(xT - xHalf) <= r ? xT : xHalf - sigma * r;

            // Updating Interval
            if (increasing)
            {
                if (EvaluatePolynomialAccurate(xITP) > 0)
                {
                    rightBound = xITP;
                }
                else if (EvaluatePolynomialAccurate(xITP) < 0)
                {
                    leftBound = xITP;
                }
                else
                {
                    leftBound = rightBound = xITP;
                }
            }
            else
            {
                if (EvaluatePolynomialAccurate(xITP) < 0)
                {
                    rightBound = xITP;
                }
                else if (EvaluatePolynomialAccurate(xITP) > 0)
                {
                    leftBound = xITP;
                }
                else
                {
                    leftBound = rightBound = xITP;
                }
            }
        }

        return (float)(leftBound + rightBound) / 2f;
    }

    /// <summary>
    /// Same as RefineIntervalITP but uses double precision internally, recommended for higher degree (10+). 
    /// Finds a root of the polynomial within the specified interval using an iterative refinement process.
    /// The method combines interpolation, truncation, and projection (ITP) steps to converge towards a single root,
    /// offering superlinear convergence on average and linear convergence in the worst case.
    /// </summary>
    /// <param name="leftBound">The left boundary of the interval to search for a root.</param>
    /// <param name="rightBound">The right boundary of the interval to search for a root.</param>
    /// <param name="tol">The tolerance for convergence. The method aims to find a root within an interval of size less than or equal to 2 * tol. Default is 0.0005f.</param>
    /// <param name="k1">A coefficient used in the calculation of the truncation step. Default if unchanged is 0.2f / (rightBound - leftBound).</param>
    /// <param name="k2">A coefficient used in the calculation of the delta for the truncation step, affecting the interpolation robustness. Default is 2f.</param>
    /// <param name="n0">The initial offset for the maximum number of iterations, contributing to the calculation of the dynamic range for the interpolation step. Default is 1.</param>
    /// <param name="maxIterations">The maximum number of iterations to perform. This prevents the method from running indefinitely. Default is 50.</param>
    /// <returns>The approximate position of the root within the specified interval, determined to be within the specified tolerance.</returns>
    /// <remarks>
    /// This method employs an iterative technique that refines the interval containing the root by evaluating the polynomial's sign changes.
    /// It dynamically adjusts the interval based on the polynomial's behavior, using interpolation, truncation, and projection steps
    /// to efficiently converge towards the root. The method is designed to work with polynomials where a single root exists within the given interval.
    /// </remarks>
    public double RefineIntervalITP(double leftBound, double rightBound, double tol = 1e-5f, int maxIterations = 50, double k1 = double.NaN, double k2 = 2f, int n0 = 1)
    {
        // Preprocessing
        if (double.IsNaN(k1))
        {
            k1 = 0.2 / (rightBound - leftBound); // Value determined experimentally
        }
        int nHalf = (int)Math.Ceiling(Math.Log((rightBound - leftBound) / (2 * tol), 2));
        int nMax = nHalf + n0;
        bool increasing = EvaluatePolynomialAccurate(leftBound) < EvaluatePolynomialAccurate(rightBound);

        // Iterate until convergence or maximum number of iterations is reached
        for (int iterations = 0; iterations < maxIterations && rightBound - leftBound > 2 * tol; iterations++)
        {
            // Calculating Parameters
            double xHalf = (leftBound + rightBound) / 2;
            double r = tol * Math.Pow(2, nMax - iterations) - (rightBound - leftBound) / 2;
            double delta = k1 * Math.Pow(rightBound - leftBound, k2);

            // Interpolation
            double xF = (rightBound * EvaluatePolynomialAccurate(leftBound) - leftBound * EvaluatePolynomialAccurate(rightBound)) / (EvaluatePolynomialAccurate(leftBound) - EvaluatePolynomialAccurate(rightBound));

            // Truncation
            double sigma = Math.Sign(xHalf - xF);
            double xT = Math.Abs(xHalf - xF) >= delta ? xF + sigma * delta : xHalf;

            // Projection
            double xITP = Math.Abs(xT - xHalf) <= r ? xT : xHalf - sigma * r;

            // Updating Interval
            if (increasing)
            {
                if (EvaluatePolynomialAccurate(xITP) > 0)
                {
                    rightBound = xITP;
                }
                else if (EvaluatePolynomialAccurate(xITP) < 0)
                {
                    leftBound = xITP;
                }
                else
                {
                    leftBound = rightBound = xITP;
                }
            }
            else
            {
                if (EvaluatePolynomialAccurate(xITP) < 0)
                {
                    rightBound = xITP;
                }
                else if (EvaluatePolynomialAccurate(xITP) > 0)
                {
                    leftBound = xITP;
                }
                else
                {
                    leftBound = rightBound = xITP;
                }
            }
        }

        return (leftBound + rightBound) / 2.0;
    }
}
