# Finding All Positive Real Roots of a Polynomial
## About polynomial roots
A polynomial is an expression of the form $$p(x) = a_nx^n + a_{n-1}x^{n-1} + \cdots + a_1x + a_0$$, where $$a_n, a_{n-1}, \ldots, a_1, a_0$$ are constants and $$n$$ is a non-negative integer. A root of a polynomial is a value of $$x$$ that makes $$p(x) = 0$$. A real root is a root that is a real number, and a positive root is a root that is greater than zero.

Finding all the positive real roots of a polynomial is an important problem in mathematics and computer science. In our case, it is needed to optimize our problem of finding the time it takes with the minimum initial projectile velocity required to hit the target. However, finding all the positive real roots of a polynomial is not a trivial task, as there is no general formula for solving polynomial equations of degree higher than four. Moreover, some polynomials may have no positive real roots, or have multiple positive real roots that are close to each other.



## Useful tools for locating intervals containing roots
In this section, we will explore some methods and algorithms for finding all the positive real roots of a polynomial, such as:
- **Descartes' rule of signs**, which gives an upper bound on the number of positive real roots of a polynomial by counting the sign changes in its coefficients.
- **Sturm's theorem**, which gives an exact count of the number of positive real roots of a polynomial by constructing a sequence of polynomials with certain properties.
- **Vincent’s theorem**, is a mathematical result that helps to find the real roots of a polynomial with rational coefficients.

#### Refinement methods to improve precision of a root
- The **bisection method**, which is a simple and robust numerical method that finds a positive real root of a polynomial by repeatedly dividing an interval that contains a root into two subintervals and choosing the one that contains a root.
- **Newton's method**, which is a fast and powerful numerical method that finds a positive real root of a polynomial by starting from an initial guess and iteratively improving it using the derivative of the polynomial.
- The [**ITP method**](https://en.wikipedia.org/wiki/ITP_method), short for Interpolate, Truncate, Project, is the first root-finding algorithm that achieves the superlinear convergence of the secant method while retaining the optimal worst-case performance of the bisection method.

## Finding a solution
Before we start tackling the problem of finding all positive real roots of a polynomial, let's establish a procedure for finding a solution. Since the real number line is an interval, I decided to take inspiration from techniques in computer science algorithms, namely binary search. There are methods for finding all complex roots of a polynomial at once, but we are only interested in real roots so we will try this simpler solution.

This motivated me to use the bisection algorithm (like binary search) to split up the real number line until we reached intervals containing only 1 root each, so we could apply a root bracketing algorithm such as bisection. Of course, the real number line is infinite, so bisecting the entire thing is not possible. Luckily, I was delighted to find that there are well known bounds on polynomial roots: examples can be found at [[1](https://en.wikipedia.org/wiki/Geometrical_properties_of_polynomial_roots#Bounds_of_real_roots)] and [[2](https://www.journals.vu.lt/nonlinear-analysis/article/view/14557/13475)]. Therefore this type of algorithm will be possible to use. 

The LMQ bound is a quadratic complexity bound with respect to polynomial degree on the values of the positive roots of polynomials, derived from Theorem 5 [in this paper](https://www.jucs.org/jucs_15_3/linear_and_quadratic_complexity/jucs_15_03_0523_0537_akritas.pdf). It is based on the idea of pairing each negative coefficient of the polynomial with each one of the preceding positive coefficients divided by $2^t$, where $t$ is the number of times the positive coefficient has been used, and taking the minimum over all such pairs. The maximum of all those minimums is taken as the estimate of the bound.

To implement the LMQ bound, we need to do the following steps:

- Given a polynomial p(x) = α_n x^n + α_(n-1) x^(n-1) + ... + α_0, where α_n > 0, find all the negative coefficients and their indices, i.e., α_i < 0 for some i < n.
- For each negative coefficient α_i, loop over all the preceding positive coefficients α_j, where j > i, and divide them by 2^t_j^, where t_j is initially set to 1 and is incremented each time α_j is used in a pair. Compute the radical (j-i)/(-α_i/α_j * 2^t_j^) and take the minimum over all j.
- Take the maximum of all the minimum radicals obtained in the previous step. This is the LMQ bound.

Here is a possible pseudocode implementation of the LMQ bound:

```python
# Input: a list of coefficients of a polynomial p(x) in decreasing order of degree
# Output: an upper bound on the values of the positive roots of p(x)
def LMQ_bound(p):
  # Initialize the bound to zero
  bound = 0
  # Initialize an array to store the powers of 2 for each positive coefficient
  powers = [1] * (n + 1)
  # Loop through the coefficients of p(x) from right to left
  for i in range(n, -1, -1):
    # If the coefficient is negative
    if a_i < 0:
      # Initialize the minimum to infinity
      minimum = float("inf")
      # Loop through the preceding positive coefficients
      for j in range(i + 1, n + 1):
        if a_j > 0:
          # Compute the radical from the pairing
          radical = (j - i) * (-a_i / (a_j * powers[j])) ** (1 / (j - i))
          # Update the minimum if needed
          minimum = min(minimum, radical)
          # Increment the power of 2 for the positive coefficient
          powers[j] *= 2
      # Update the bound if needed
      bound = max(bound, minimum)
  # Return the bound
  return bound
```

## Square-free polynomials
A recurring concept when looking at this type of algorithm is the "squarefree polynomial". For our purposes, a squarefree polynomial is one with no repeated roots (all zeroes have exponent 1 in the factored form). Indeed, many such algorithms require that the input is a squarefree polynomial. In theory, this is not a problem since we can reduce the polynomial to a squarefree one with the same roots (we are not interested in the multiplicity). If you do the algebra, you will notice that for a given polynomial $p(x)$, you can simply divide it by its greatest common divisor with its derivative, and it will give a squarefree polynomial with the same roots. So $$p_{reduced}(x) = \frac{p(x)}{\gcd(p(x), p'(x))}$$ will have the same roots but multiplicity 1. However, in practice, this procedure is not always numerically stable and can result in great errors, even missing roots completely. For our problem, this should hopefully not occur too many times to be problematic since in most cases, it will be squarefree from the start anyways. Relevant work I found about this is from [(Yun,1976)](https://dl.acm.org/doi/10.1145/800205.806320).