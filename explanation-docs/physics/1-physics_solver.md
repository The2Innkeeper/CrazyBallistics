## NOTE: This file is outdated, it contains my previous incomplete reasoning which turned out not to be too useful.


# Physics solver
This file contains a high-level description of the algorithm for the physics solver.

## Problem statement
The problem is about finding the optimal initial velocity vector for a projectile to hit a moving target with the least effort. The target and the shooter (which launches the projectile) are both moving with any number of initial position derivatives (position, velocity, acceleration, jerk, etc.). The goal is to minimize the 2-norm (magnitude) of the initial velocity vector of the projectile.

## Notation overview
Note that we will represent polynomials as a list of their coefficients in increasing order of degree since we care most about position, and it often simplifies implementation going from left to right instead of right to left. That is, a polynomial $p(t)=a_0+a_1 t^1+...+a_n t^n$ is represented by the list $\text{polynomial\_coefficients}=[a_0,a_1,...,a_n]$ with $n+1$ elements, and $p[i]$ will denote the $i$-th degree's coefficient $a_i$.

An important point to consider is that our position $x(T)$ is a vector while $T$ is a scalar. We should write it as $x(T)=x(0)+T \dot x(0)+\frac{T^2}{2} \ddot x(0)+...+\frac{T^n}{n!}x^{(n)}(0)$ where $x^{(n)}$ denotes the $n$-th derivative. So in reality, even though I call the initial position derivatives "coefficients", remember that they are in fact vectors and only disappear into a scalar after the dot product is taken.

## Mathematical description

The derivative of a polynomial function can be computed using the power rule of differentiation. Given a polynomial function

$$p(T) \coloneqq a_0 + T a_1 + T^2 a_2  + ... + T^n a_n$$

where $a_i$ are the "coefficient" vectors, the derivative $dp/dT$ is given by:

$$\frac{dp(T)}{dT} = a_1 + 2T a_2 + 3T^2 a_3 + ... + n T^{n-1} a_n$$

To compute $T dp/dT$, you simply multiply the derivative by $T$:

$$T \frac{dp(T)}{dT} = T a_1 + 2T^2 a_2  + 3 T^3 a_3 + ... + n T^n a_n$$

Compared to the original polynomial $p(T)$, the coefficients of $T \frac{dp(T)}{dT}$ are each multiplied by the index of the current term. That is, $$\left( T \frac{dp}{dT} \right)\left[ i\right] = i\cdot p[i]$$ for $i=0,...,n$.

We wish to find all the positive roots of the last polynomial derived in the `solution_approach` file and test them one-by-one into the function for the squared magnitude of $v(T)$ to find the minimum. The values to test are the values of $T$ that satisfy $$\text{Critical}(T):=x(T)\cdot \left(T \frac{dx}{dT}(T)-x(T)\right) = 0$$

We can parhaps break the expression for some intuition and see if there is room for optimization. Also, reducing the intermediate computation steps like the dot product can help with reducing precision errors.
$$\text{Critical}(T)\coloneqq$$
$$\left( \sum_{j=0}^n \left( \frac{T^j}{j!}\right) x^{(j)}(0) \right) \cdot \left( \sum_{i=0}^n \left( \frac{i\cdot T^i}{i!}\right) x^{(i)}(0)-\sum_{i=0}^n \left( \frac{T^i}{i!} \right) x^{(i)}(0)\right)$$
$$=\left( \sum_{j=0}^n \left( \frac{T^j}{j!}\right) x^{(j)}(0) \right) \cdot \left( \sum_{i=0}^n \left( \frac{(i-1)T^i}{i!}\right) x^{(i)}(0) \right)$$
$$=\left( \sum_{j=0}^n \left( \frac{x^{(j)}(0)}{j!}\right) T^j \right) \cdot \left( \sum_{i=0}^n \left( \frac{(i-1)x^{(i)}(0)}{i!}\right) T^i \right)$$
At this point, the expression is a product of 2 sums which in general can only be expressed as a double sum. I have come to the realization that in order for us to be able to compute an upper bound on the roots of the polynomial, this must be fully expanded to isolate each power's coefficient, which has quadratic time complexity. This will be quite computationally expensive as the degree grows large.

To find the coefficient for each term in the expanded polynomial, we need to multiply out the two sums and collect like terms. Let's break it down step by step.

1) First, let's simplify the notation a bit. Let $$a_j = \frac{x^{(j)}(0)}{j!}$$ and $$b_i = \frac{(i-1)x^{(i)}(0)}{i!}$$. Then our expression becomes:

   $$\text{Critical}(T) = \left(\sum_{j=0}^n a_j T^j\right) \cdot \left(\sum_{i=0}^n b_i T^i\right)$$

2) Now, let's multiply out these sums. Each term in the first sum will multiply with each term in the second sum, giving us a double sum:

   $$\text{Critical}(T) = \sum_{j=0}^n \sum_{i=0}^n a_j b_i T^{j+i}$$

3) Now, let's look at the coefficient of $T^k$ in this expanded sum. It will come from all the terms where $j+i=k\iff i=k-j$. So, the coefficient of $T^k$ is:

   $$\text{coeff}(T^k) = \sum_{j=0}^k a_j b_{k-j}$$

4) Substituting back in the original expressions for $a_j$ and $b_i$:

   $$\text{coeff}(T^k) = \sum_{j=0}^k \frac{x^{(j)}(0)}{j!} \frac{((k-j)-1)x^{(k-j)}(0)}{(k-j)!}$$

5) Simplifying:

   $$\text{coeff}(T^k) = \sum_{j=0}^k \frac{x^{(j)}(0)x^{(k-j)}(0)(k-j-1)}{j!(k-j)!}$$

Remember, since $x$ is a vector, $x^{(j)}(0)x^{(k-j)}(0)$ is a dot product.

So, in summary, to find the coefficient of $T^k$ in the expanded polynomial, you need to compute the sum:

$$\sum_{j=0}^k \frac{x^{(j)}(0)x^{(k-j)}(0)(1-k+j)}{j!(k-j)!}$$

where $x^{(j)}(0)\cdot x^{(k-j)}(0)$ is a dot product.
This should allow us to use the common operations of the polynomial based on its coefficients, which many root-finding algorithms rely on.

Perhaps if one could use an upper bound that did not rely on its coefficients but a blackbox evaluation function, we wouldn't need to expand it. Alternatively, you could fix it to a fairly large constant value since in practice, a simulation cannot stay consistent for very long times unless you give it a lot of numerical precision, which slows down computation significantly anyways. 

In fact, in his paper [Good News for Polynomial Root-finding](https://arxiv.org/abs/1805.12042) (Pan, 2024), Victor Y. Pan claims that "Furthermore, our root-finders can be applied to a black box polynomial, defined by an oracle (that is, black box subroutine) for its evaluation rather than by its coefficients".  This would then allow one to compute all the roots $\text{Critical}(T)$ through direct blackbox evaluation with the dot product form, allowing one to avoid quadratic time complexity. This would scale better for very high degree polynomials.

I will not be implementing this in the near future since it is more involved than the simple root-finding methods that I have discussed, and it requires much more preparation to understand. If someone is bored they could real through all 87 pages of the paper and implement the real and near real-rootfinding methods described involving complex number arithmetic.