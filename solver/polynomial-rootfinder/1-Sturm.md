Here is explanation of a basic algorithm for finding all the positive real roots of a polynomial.

First, there exists a theorem called [Sturm's theorem](https://en.wikipedia.org/wiki/Sturm%27s_theorem), which states allows you to count the number of real roots of a squarefree polynomial inside an open-closed interval $]a,b]$. For our purposes, a squarefree polynomial is one with no repeated roots (all zeroes have exponent 1 in the factored form). However, this is not a problem since we can reduce the polynomial to a squarefree one with the same roots (we are not interested in the multiplicity). If you do the algebra, you will notice that for a given polynomial $p(x)$, you can simply divide it by its greatest common divisor with its derivative, and it will give a squarefree polynomial with the same roots. So $$p_{reduced}(x) = \frac{p(x)}{\gcd(p(x), p'(x))}$$ will have the same roots but multiplicity 1.

Back to Sturm's theorem: it allows us to find the number of real roots, as mentioned. This motivated me to use the bisection algorithm (like binary search) to split up the real number line until we reached intervals containing only 1 root each, so we could apply a root bracketing algorithm such as bisection. Of course, the real number line is infinite, so bisecting the entire thing is not possible. Luckily, I was delighted to find that there are well known bounds on polynomial roots: examples can be found at [[1](https://en.wikipedia.org/wiki/Geometrical_properties_of_polynomial_roots#Bounds_of_real_roots)] and [[2](https://www.journals.vu.lt/nonlinear-analysis/article/view/14557/13475)]. Therefore this algorithm will be possible to use. 

## Polynomial real root-finding algorithm
A recursive formulation of a basic algorithm for finding all positive real roots of a polynomial (can be applied to any interval on the real number line).

### Plain text formulation
- Initialize the bound on the roots. This creates an initial interval with all the roots contained inside.
- Execute the bisection algorithm: divide the interval in half. This gives us 2 new intervals. Then, apply Sturm's theorem to find the number of the roots in the interval.
  - If there are no roots in the interval, we discard it as it is not useful anymore.
  - If there is 1 root, we apply the root-bracketing algorithm to isolate the root to a desired precision.
  - If there is more than 1 root, then we pass it back into the algorithm to be bisected.

### Mathematical formulation
- The input of the algorithm is a polynomial function $f(x)$ of degree $n$, and an initial interval $[a, b]$ that contains all the real roots of $f(x)$. The output is a list of real roots of $f(x)$, isolated to a desired precision $\epsilon > 0$.
- The algorithm initializes an empty list of roots $R$, and a stack of intervals $S$ that contains the initial interval $[a, b]$. Then, it repeats the following steps until $S$ is empty:
  - Pop an interval $[c, d]$ from $S$.
  - Apply Sturm's theorem to find the number of real roots of $f(x)$ in $[c, d]$, denoted by $k$.
  - If $k = 0$, do nothing.
  - If $k = 1$, apply the root-bracketing algorithm (such as bisection, secant, or Newton's method) to isolate the root of $f(x)$ in $[c, d]$ to within $\epsilon$, denoted by $r$. Append $r$ to $R$.
  - If $k > 1$, bisect the interval $[c, d]$ into two subintervals $[c, m]$ and $[m, d]$, where $m = (c + d) / 2$. Push $[c, m]$ and $[m, d]$ to $S$.
- Return the list of roots $R$.

### Pseudo-code
```
function find_roots(f, a, b, tol):
  # f is the polynomial function
  # a and b are the lower and upper bounds of the initial interval
  # tol is the desired precision for the roots
  # returns a list of roots

  # initialize an empty list of roots
  roots = []

  # initialize a stack of intervals to process
  stack = [(a, b)]

  # loop until the stack is empty
  while stack is not empty:
    # pop an interval from the stack
    (c, d) = stack.pop()

    # apply Sturm's theorem to find the number of roots in the interval
    n = sturm(f, c, d)

    # if there are no roots, do nothing
    if n == 0:
      continue

    # if there is one root, apply the root-bracketing algorithm to isolate it
    if n == 1:
      root = bracket(f, c, d, tol)
      # append the root to the list of roots
      roots.append(root)

    # if there is more than one root, bisect the interval and push the subintervals to the stack
    if n > 1:
      m = (c + d) / 2
      stack.push((c, m))
      stack.push((m, d))

  # return the list of roots
  return roots
```