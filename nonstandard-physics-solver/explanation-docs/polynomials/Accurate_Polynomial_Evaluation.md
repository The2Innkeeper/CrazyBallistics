# Accurate polynomial evaluation
Polynomial evaluation with floating point precision can introduce errors during repeated computational operations.

## Horner's method
Horner's method, also known as Horner's scheme, is a clever algorithm for evaluating polynomials efficiently. It's named after William George Horner, although the method predates him, being used by mathematicians across various cultures for centuries. The essence of Horner's method lies in its ability to reduce the computational complexity of evaluating a polynomial by minimizing the number of multiplications and additions required.

#### Plain Language Explanation

Suppose you have a polynomial like $$5x^3 + 3x^2 + 2x + 1$$ and you want to find its value at $x = 2$. The straightforward approach would involve calculating each term separately and then adding them all up, which is quite repetitive and inefficient, especially for polynomials of high degree.

Horner's method simplifies this process by reorganizing the polynomial into a nested form, allowing us to evaluate it using fewer operations. Instead of computing each power of $x$ separately, Horner's method takes advantage of the fact that the polynomial can be rewritten as $$(((5x + 3)x + 2)x + 1)$$.

This nested form means that we start from the coefficient of the highest degree term, multiply it by $x$, and then add the next coefficient, repeating this process until we reach the constant term. This approach significantly reduces the number of multiplications and additions needed to evaluate the polynomial.

#### Pseudocode

Here's how you could implement Horner's method in Python-like pseudocode:

```python
def horner_method(coefficients, x):
    """
    Evaluates a polynomial at a given point x using Horner's method.
    
    Parameters:
    - coefficients: List of coefficients [a_n, ..., a_1, a_0]
    - x: The point at which to evaluate the polynomial
    
    Returns:
    - The value of the polynomial at x
    """
    result = coefficients[0]  # Start with the leading coefficient
    for i in range(1, len(coefficients)):
        result = result * x + coefficients[i]  # Multiply by x and add the next coefficient
    return result
```

Horner's method is not only efficient for polynomial evaluation but also for polynomial division and finding roots when combined with other algorithms like Newton-Raphson. It's a fundamental technique in numerical analysis and computer science due to its simplicity and efficiency.

## The problem
Horner's method is applicable for many standard problems. However, in our case, we often encounter very high degree polynomials with very small coefficients and roots that are very small and close together. This is problematic as it is very prone to precision errors, in which case it can lead to the projectile passing right by the target without actually intersecting it. Therefore, I searched some solutions to this.

## Sutin's algorithm
Brian M. Sutin's paper [Accurate Evaluation of Polynomials](https://arxiv.org/abs/0805.3194) presents an algorithm for evaluating polynomials more accurately than Horner's method. It involves several key steps, including finding a nearby polynomial, rearranging and evaluating the original polynomial in terms of the nearby polynomial and a correction term, computing constants, and finally evaluating the polynomial with increased accuracy. The specifics of these steps are crucial for understanding and implementing the algorithm. Here is a detailed breakdown and Python-like pseudocode implementation based on the paper's content.

## Algorithm Overview

1. **Finding a Nearby Polynomial**: The algorithm computes a nearby polynomial and evaluation point that can be evaluated exactly, even with finite decimal place arithmetic. This involves adjusting the coefficients and the evaluation point slightly.

2. **Rearranging and Evaluating**: The original polynomial is rearranged in terms of the nearby polynomial and a correction term. This rearrangement simplifies the evaluation by allowing the division of the correction term by the difference between the original and adjusted evaluation points.

3. **Computing Constants**: Constants for the polynomial are computed, which are crucial for the accurate evaluation at the original point. This step involves exact computation for part of the constants and an approximation for the small terms.

4. **Evaluation**: Finally, the polynomial is evaluated using the computed constants and the nearby polynomial, yielding a more accurate result.

## Python-like Pseudocode Implementation

The pseudocode below reflects the algorithm's steps as described in the paper. The specifics of computing the nearby polynomial, the correction term, and the constants are crucial for the method's increased accuracy.

```python
def evaluate_polynomial(coefficients, x):
    # Step 1: Compute a nearby polynomial and evaluation point
    x_adj, coeffs_adj = compute_nearby_polynomial(coefficients, x)
    
    # Step 2: Rearrange the original polynomial in terms of the nearby polynomial
    # and compute the correction term
    correction_term = compute_correction_term(coefficients, x, x_adj)
    
    # Step 3: Compute the constants for the accurate evaluation
    constants = compute_constants(coeffs_adj, x_adj)
    
    # Step 4: Evaluate the polynomial using the nearby polynomial and correction term
    result = evaluate_nearby_polynomial(x_adj, coeffs_adj) + correction_term
    
    return result

def compute_nearby_polynomial(coefficients, x):
    # Implementation details based on the paper
    x_adj = truncate_to_half_precision(x)
    coeffs_adj = []
    H = 0
    for i in reversed(range(len(coefficients))):
        S = truncate_to_half_precision(H * x_adj)
        H = truncate_to_half_precision(S + coefficients[i])
        coeffs_adj.append(H - S)
    coeffs_adj.reverse()
    return x_adj, coeffs_adj

def compute_correction_term(coefficients, x, x_adj):
    # Implementation details based on the paper
    correction_term = 0
    for i in range(1, len(coefficients)):
        term = coefficients[i] * (x**i - x_adj**i)
        correction_term += term / (x - x_adj)
    return correction_term

def compute_constants(coeffs_adj, x_adj):
    # Implementation details based on the paper
    constants = []
    for j in range(len(coeffs_adj) - 1):
        Cj = sum(coeffs_adj[j+1+i] * x_adj**i for i in range(len(coeffs_adj)-1-j))
        constants.append(Cj)
    return constants

def evaluate_nearby_polynomial(x_adj, coeffs_adj):
    # Horner's method for evaluating the nearby polynomial
    result = coeffs_adj[0]
    for coeff in coeffs_adj[1:]:
        result = result * x_adj + coeff
    return result

def truncate_to_half_precision(value):
    # Placeholder function for truncating a value to half its precision
    # Implementation details would depend on the floating-point representation
    return value
```

This pseudocode provides a detailed implementation of the algorithm described in Sutin's paper. It includes the computation of a nearby polynomial, the rearrangement and evaluation of the original polynomial, the computation of constants, and the final evaluation using the nearby polynomial and correction term. The `truncate_to_half_precision` function is a placeholder for the actual implementation of truncating a value to half its precision, which would depend on the specific floating-point representation used.