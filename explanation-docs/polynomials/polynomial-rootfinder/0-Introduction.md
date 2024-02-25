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

Researchers focus the most on upper bounds of these polynomial roots because we can obtain lower bounds through algebraic transformations. For example, you can find a lower bound of the negative roots of a polynomial $P(x)$ by instead finding an upper bound for the positive roots of $P(-x)$. Similarly, for the case we are interested in, we wish to find a lower bound for the positive real roots of the polynomial: in that case, we can find the upper bound for the positive real roots of $x^nP(\frac 1 x)$ since the transformation $x \coloneqq \frac{1}{x}$ will essentially flip the ordering of any 2 numbers in $\mathbb{R}^+$. With respect to the coefficients, this is equivalent to reading from right to left, in the sense that $P(x)=a_n x^n+...+a_0$ becomes $x^nP(\frac 1 x)=a_0 x^n+...+a_n$.

For an effective upper bound, we will take the Local-Max-Quadratic (LMQ) bound, which is a quadratic complexity bound with respect to polynomial degree on the values of the positive roots of polynomials, derived from Theorem 5 [in this paper](https://www.jucs.org/jucs_15_3/linear_and_quadratic_complexity/jucs_15_03_0523_0537_akritas.pdf). It is based on the idea of pairing each negative coefficient of the polynomial with each one of the preceding positive coefficients divided by $2^t$, where $t$ is the number of times the positive coefficient has been used, and taking the minimum over all such pairs. The maximum of all those minimums is taken as the estimate of the bound.

To implement the LMQ bound, we need to do the following steps:

- Given a polynomial $p(x) = a_n x^n + a_{n-1} x^{n-1} + ... + a_0$, find all the negative coefficients and their indices, i.e., $a_i < 0$ for some $i < n$.
- For each negative coefficient $a_i$, loop over all the preceding positive coefficients $a_j$, where $j > i$, and compute $\sqrt[j-i]{\frac{-2^t_ja_i}{a_j}}$, where $t_j$ is initially set to 1 and is incremented each time $a_j$ is used in a pair. Then, take the minimum over all $j$.
- Finally, take the maximum of all the minimum radicals obtained in the previous step. This is the LMQ bound.

Here is a possible pseudocode implementation of the LMQ bound:

```python
# Input: "coeffs": list of coefficients of a polynomial p(x) in decreasing order of degree
# Output: an upper bound on the values of the positive roots of p(x)[^1^][1][^2^][2]
def LMQ_bound(coeffs):
  # Initialize the bound to a negative (invalid) number
  upper_bound = float('-inf')
  # Initialize an array to store the powers of 2 for each positive coefficient
  powers = [1] * len(coeffs)
  # Iterate through all elements in array of coefficients
  for neg_i in range(len(coeffs)):
    # Check if ai < 0
    if not coeffs[neg_i] < 0:
      continue
    # Iterate through all preceding positive coefficients
    for pos_i in range(neg_i+1, len(coeffs)):
      # Check if aj > 0
      if not coeffs[pos_i] > 0:
        continue
      # Calculate the (j-i)-th root of (-2^t_j*a_i/a_j)
      value = (-2**powers[pos_i] * coeffs[neg_i] / coeffs[pos_i]) ** (1 / (pos_i - neg_i))
      # Update the minimum value for this pair
      minValue = min(value, coeffs[pos_i])
      # Update the maximum value for all pairs
      upper_bound = max(upper_bound, minValue)
      # Increment the power of 2 for this positive coefficient
      powers[pos_i] += 1
  # Return the upper bound
  return upper_bound
```

## Square-free polynomials
A recurring concept when looking at this type of algorithm is the "squarefree polynomial". For our purposes, a squarefree polynomial is one with no repeated roots (all zeroes have exponent 1 in the factored form). Indeed, many such algorithms require that the input is a squarefree polynomial. In theory, this is not a problem since we can reduce the polynomial to a squarefree one with the same roots (we are not interested in the multiplicity). If you do the algebra, you will notice that for a given polynomial $p(x)$, you can simply divide it by its greatest common divisor with its derivative, and it will give a squarefree polynomial with the same roots. So $$p_{reduced}(x) = \frac{p(x)}{\gcd(p(x), p'(x))}$$ will have the same roots but multiplicity $1$. However, in practice, this procedure is not always numerically stable and can result in great errors, even missing roots completely. For our problem, this should hopefully not occur too many times to be problematic since in most cases, it will be squarefree from the start anyways. Relevant work I found about this is from [(Yun,1976)](https://dl.acm.org/doi/10.1145/800205.806320).

#### Pseudocode
```python
def polynomial_derivative_coefficients(poly_coeffs):
    # Calculate the coefficients of the derivative of the polynomial
    derivative_coeffs = []
    for power, coeff in enumerate(poly_coeffs):
        if power > 0:  # Skip the constant term
            derivative_coeffs.append(coeff * power)
    return derivative_coeffs

def polynomial_division(dividend, divisor):
    # Perform polynomial division of dividend by divisor
    # The dividend and divisor are lists of coefficients, from lowest to highest degree
    quotient = []
    remainder = list(dividend)  # Start with the dividend as the remainder

    # Degree of the dividend and divisor polynomials
    deg_dividend = len(dividend) - 1
    deg_divisor = len(divisor) - 1

    while deg_dividend >= deg_divisor and any(remainder):
        # Leading coefficients and their degree
        lead_dividend = remainder[-1]
        lead_divisor = divisor[-1]
        deg_lead_dividend = len(remainder) - 1

        # Calculate the next coefficient of the quotient
        coeff_quotient = lead_dividend / lead_divisor
        deg_diff = deg_lead_dividend - deg_divisor
        quotient_term = [0] * deg_diff + [coeff_quotient]

        # Subtract the current divisor term from the remainder
        divisor_term = [coeff * coeff_quotient for coeff in divisor] + [0] * deg_diff
        remainder = [a - b for a, b in zip(remainder + [0] * len(divisor_term), divisor_term)]

        # Remove trailing zeros from the remainder
        while remainder and remainder[-1] == 0:
            remainder.pop()

        # Update the degree of the remainder
        deg_dividend = len(remainder) - 1

        # Add the current quotient term to the quotient
        quotient = [a + b for a, b in zip(quotient + [0] * len(quotient_term), quotient_term)]

    # Remove trailing zeros from the quotient
    while quotient and quotient[-1] == 0:
        quotient.pop()

    return quotient, remainder

def gcd_polynomials(a, b):
    # Calculate the GCD of two polynomials a and b
    while any(b):  # While b is not the zero polynomial
        _, remainder = polynomial_division(a, b)
        a, b = b, remainder
    return a

def make_square_free(poly_coeffs):
    # Calculate the derivative of the polynomial
    derivative_coeffs = polynomial_derivative_coefficients(poly_coeffs)
    
    # Calculate the GCD of the polynomial and its derivative
    gcd_coeffs = gcd_polynomials(poly_coeffs, derivative_coeffs)
    
    # Divide the original polynomial by the GCD
    square_free_coeffs, _ = polynomial_division(poly_coeffs, gcd_coeffs)
    
    return square_free_coeffs
```