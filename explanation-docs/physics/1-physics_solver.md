# Physics solver
This file contains a high-level description of the algorithm for the physics solver.

## Problem statement
The problem is about finding the optimal initial velocity vector for a projectile to hit a moving target with the least effort. The target and the shooter (which launches the projectile) are both moving with any number of initial position derivatives (position, velocity, acceleration, jerk, etc.). The goal is to minimize the 2-norm (magnitude) of the initial velocity vector of the projectile.

## Notation overview
Note that we will represent polynomials as a list of their coefficients in increasing order of degree since we care most about position, and it often simplifies implementation going from left to right instead of right to left. That is, a polynomial $p(t)=a_0+a_1 t^1+...+a_n t^n$ is represented by the list $\text{polynomial\_coefficients}=[a_0,a_1,...,a_n]$ with $n+1$ elements, and $p[i]$ will denote the $i$-th degree's coefficient $a_i$.

An important point to consider is that our position $x(T)$ is a vector while $T$ is a scalar. We should write it as $x(T)=x(0)+T \dot x(0)+\frac{T^2}{2} \ddot x(0)+...+\frac{T^n}{n!}x^{(n)}(0)$ where $x^{(n)}$ denotes the $n$-th derivative. So in reality, even though I call the initial position derivatives "coefficients", remember that they are in fact vectors and only disappear into a scalar after the dot product is taken.

## Mathematical description

The derivative of a polynomial function can be computed using the power rule of differentiation. Given a polynomial function $$p(T) \coloneqq a_0 + T a_1 + T^2 a_2  + ... + T^n a_n$$, where $a_i$ are the "coefficient" vectors, the derivative $dp/dT$ is given by:

$$\frac{dp(T)}{dT} = a_1 + 2T a_2 + 3T^2 a_3 + ... + n T^{n-1} a_n$$

To compute $T dp/dT$, you simply multiply the derivative by $T$:

$$T \frac{dp(T)}{dT} = T a_1 + 2T^2 a_2  + 3 T^3 a_3 + ... + n T^n a_n$$

Compared to the original polynomial $p(T)$, the coefficients of $T \frac{dp(T)}{dT}$ are each multiplied by the index of the current term. That is, $$\left( T \frac{dp}{dT} \right)\left[ i\right] = i\cdot p[i]$$ for $i=0,...,n$.

We wish to find all the positive roots of the last polynomial derived in the `solution_approach` file and test them one-by-one into the function for the squared magnitude of $v(T)$ to find the minimum. The values to test are the values of $T$ that satisfy $$\text{Critical}(T):=x(T)\cdot \left( x(T)-T \frac{dx}{dT}(T)\right) = 0$$

We can parhaps break the expression for some intuition and see if there is room for optimization. Also, reducing the intermediate computation steps like the dot product can help with reducing precision errors.
$$\text{Critical}(T)=\left( \sum_{j=0}^n \left( \frac{T^j}{j!}\right) x^{(j)}(0) \right) \cdot \left( \sum_{i=0}^n \left( \frac{T^i}{i!} \right) x^{(i)}(0)-\sum_{i=0}^n \left( \frac{i\cdot T^i}{i!}\right) x^{(i)}(0) \right)$$
$$=\left( \sum_{j=0}^n \left( \frac{T^j}{j!}\right) x^{(j)}(0) \right) \cdot \left( \sum_{i=0}^n \left( \frac{(1-i)T^i}{i!}\right) x^{(i)}(0) \right)$$
At this point, the expression is a product of 2 sums which in general can only be expressed as a double sum. Expanding only increases the number of computations, so it is not worth it. So we will keep the unreduced expression in the actual implementation because we can cache $x(T)$:
$$\text{Critical}(T):=x(T)\cdot \left( x(T)-T \frac{dx}{dT}(T)\right)$$
where 
$$x(T)\coloneqq \sum_{j=0}^n \left( \frac{T^j}{j!}\right) x^{(j)}(0)$$$$T\frac{dx}{dT}(T)=\sum_{i=0}^n \left( \frac{i\cdot T^i}{i!}\right) x^{(i)}(0)$$


#### Pseudocode:
```pseudocode
function calculate(T, rel_pos_coeffs):
    # Calculate the value of x(T)
    x_T = getValueAt(T, rel_pos_coeffs)

    # Calculate the derivative dx/dT at T
    rel_pos_coeffs_derivative = find_derivative_coeffs(rel_pos_coeffs)
    dx_dT = getValueAt(T, rel_pos_coeffs_derivative)

    # Calculate the value of the expression
    result = dotProduct(x_T,(x_T - T * dx_dT))

    # Check if the result is zero
    if result == 0:
        return True
    else:
        return False
```
```python
def get_rel_pos(target_derivatives: list, shooter_derivatives: list):
    # Calculate the relative position from the shooter to the target
    max_coeff_count = max(len(target_derivatives), len(shooter_derivatives))
    rel_pos = [0] * max_coeff_count

    for coeff_i in range(max_coeff_count):
        target_value = target_derivatives[coeff_i] if coeff_i < len(target_derivatives) else 0
        shooter_value = shooter_derivatives[coeff_i] if coeff_i < len(shooter_derivatives) else 0
        rel_pos[coeff_i] = target_value - shooter_value
```
```python
def polynomial_derivative_coeffs(polynomial_coeffs):
        """
    This function calculates the coefficients of the derivative of a polynomial represented by polynomial_coeffs.

    Parameters:
    polynomial_coeffs (list of int/float): A list of coefficients in increasing order of term degree representing a polynomial. The coefficient at index i represents the coefficient of the term with degree i.

    Returns:
    derivative_coeffs (list of int/float): A list of coefficients representing the derivative of the polynomial (also in increasing order of degree). The coefficient at index i represents the coefficient of the term with degree i in the derivative.

    The function uses the power rule of differentiation, which states that the derivative of x^n is n*x^(n-1). It iterates over each coefficient in rel_pos_coeffs, starting from the second coefficient (at index 1), and applies the power rule.

    Note: This function does not handle the case where rel_pos_coeffs is an empty list. You may want to add error checking to handle this case in your actual implementation.
    """
    derivative_coeff_count = len(polynomial_coeffs) - 1
    derivative_coeffs = [0] * derivative_coeff_count

    # Iterate over each coefficient in the vector
    for i in range(1, len(polynomial_coeffs)):
        # Apply the power rule: derivative of x^n is n*x^(n-1)
        derivative_coeffs[i-1] = i * polynomial_coeffs[i]

    return derivative_coeffs
```
```
    # Define the function to be minimized
    f = norm(v)^2

    # Take the derivative of f
    df = derivative(f)

    # Set the derivative to zero and solve for T
    T_values = solve(polynomial=df,left_bound=0)

    # Find the positive roots of the polynomial
    time_values = [time for time in times_values if time > 0]

    # Test the T_values one-by-one into the function f
    minimum = find_minimum(f, T_values)

    return minimum
```
This pseudocode assumes that we have a function `find_minimum()` that finds the minima of a polynomial, and functions `norm()`, `derivative()`, and `solve()` that calculate the norm of a vector, the derivative of a function, and the solutions of an equation, respectively. The variables `target_derivatives` and `shooter_derivatives` represent the initial position derivatives of the target and the shooter, respectively. The variable `T` represents the time of intersection. The variable `v` represents the initial velocity vector of the projectile. The variable `f` represents the function to be minimized. The variable `df` represents the derivative of `f`. The variable `T_values` represents the possible values of `T` that make `df` equal to zero. The variable `minimum` represents the minimum of `f`. The pseudocode returns the minimum of `f`. Note that the actual implementation of this algorithm would require numerical methods for solving the equations and finding the minimum.