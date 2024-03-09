Context: I am creating a special ballistics simulation with nonstandard physics, and I have reached a step where I need to find the global minimum of a rational function very fast to be able to run the simulation interactively in real-time.

Goal: Find the global minimum of a univariate, nonnegative rational function of the form
$$\left\| v(T)\right\|^2:= \left\| \frac{x(T)}{T} \right\|^2=\frac{x(T)\cdot x(T)}{T^2}$$

for some Taylor polynomial with vector "coefficients" $$x(T):=\sum_{k=0}^{n} \frac{T^k}{k!}\vec{a_k}$$ with $x(T)$ and therefore $a_k$ of arbitrary dimension.

I am interested in the domain of $\mathbb{R}_{>0}$. As you notice, this function is strictly nonnegative on this interval.

My first thought, which is a standard approach for optimization, is to use calculus to take the derivative and set it to $0$ to find critical points and test for the minimum (the function is differentiable on $\mathbb{R}_{>0}$ so the critical points must be where the derivative is $0$). So I obtained:
$$\frac{x(T)}{T}\cdot \frac{x(T)-T \frac{dx}{dT}(T)}{T^2} = 0$$
Based on that, I defined a new function $\text{Critical}$ whose roots constitute critical points of the original function:
$$\text{Critical}(T):=x(T)\cdot \left(T \frac{dx}{dT}(T)-x(T)\right) = 0$$
This is a polynomial in $T$. However, I quickly encountered a problem: how do I find all the positive real roots of the derivative polynomial reliably? I searched for algorithms and found some.

However, I then rapidly hit another obstacle: many of these root-finding algorithms rely on operations on the polynomial's coefficients. This meant that I was forced to expand the derivative to find its coefficients. Obtaining that the coefficient of $T^k$ is
$$\sum_{j=0}^k \frac{\vec a_j \cdot \vec a_{k-j}(1-k+j)}{j!(k-j)!}$$

where $\vec a_j \cdot \vec a_{k-j}$ is a dot product for $k=0,1,...,n$, or in other words, for all the vectors in defining the Taylor polynomial of $x(T)$.

As you can notice, the time complexity of this algorithm is $O(n^2)$ dot products, which is quite expensive computationally. On top of that, I need to apply a root-finding algorithm on this new polynomial to find all its roots then test each critical point in the original function, which is not cheap either, resulting in a lot of latency.

Upon investigation, I have found a technique called [Semi-definite optimization](https://ocw.mit.edu/courses/6-972-algebraic-techniques-and-semidefinite-optimization-spring-2006/a4337243b7dfbd98173565dfca7d6ce9_lecture_10.pdf) (also relevant: [here](https://www.sfu.ca/~tstephen/Seminar/BB/030326_slides.pdf)) that could be used to minimize rational functions directly through representation of a nonnegative polynomial as a sum of squares. Nevertheless, this also seems quite involved, having to compute a matrix describing an affine space on a monomial basis, then making sure that each term is nonnegative. I am not familiar with this, and if anyone could suggest if this is potentially a better solution to this problem, then please feel free to explain.

I am looking for suggestions on optimization in terms of performance. If anyone could offer ideas to be able to minimize the original function fast, it would be greatly appreciated.

As a side note, following this problem, I would like generalize this problem to any function of the form $$x^{(k)}(T):=\frac{x(T)\cdot x(T)}{T^{2k}}$$

I am guessing that if the original problem is solved efficiently, it should not be too difficult to extend the logic to this one, but this comes after solving the first problem in an efficient manner.