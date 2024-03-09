Finding the global minimum of a polynomial involves a few steps, which often require calculus and numerical methods. Here's a step-by-step guide to finding the global minimum of a polynomial function:

### 1. Understand the Polynomial

First, ensure the polynomial is well-defined and its degree (the highest power of the variable) is known. A polynomial of degree \(n\) has the form:
\[ P(x) = a_nx^n + a_{n-1}x^{n-1} + \cdots + a_1x + a_0 \]
where \(a_n, a_{n-1}, \ldots, a_1, a_0\) are constants, and \(a_n \neq 0\).

### 2. Calculate the Derivative

Find the first derivative of the polynomial, \(P'(x)\). This derivative represents the rate of change of the function and is used to identify critical points where the slope of the function is zero or undefined. For a polynomial, the derivative is also a polynomial and is defined as:
\[ P'(x) = na_nx^{n-1} + (n-1)a_{n-1}x^{n-2} + \cdots + a_1 \]

### 3. Find Critical Points

Solve \(P'(x) = 0\) to find the critical points. By definition, these are the \(x\) values where the function's slope (derivative) is zero or is undefined, which means the function could have a local minimum, local maximum, or neither at these points.

Note that polynomials are differentiable everywhere, which means that the derivative exists at every single point. Therefore, there cannot be critical points with undefined slope; and so all that remains are points with zero derivative.

### 4. Testing Critical Points
After we find all the x-values of the critical points, we proceed to test them one-by-one and take the minimum of all the y-values. We can do this iteratively:
```py
min_x = 0
min_y = float('inf')
for x in critical_points:
    y = polynomial.evaluate_at(x)
    if y < min_y:
        min_x = x
        min_y = y
```

Finally, this gives us the x value which will return the global minimum y.