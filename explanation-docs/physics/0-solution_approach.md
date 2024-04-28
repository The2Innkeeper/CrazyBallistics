# My approach to solving this problem
To understand any approach to finding a solution, it's probably a good idea to state the problem in detail first.

## Problem statement
An point ("target") is moving in a vacuum (no air resistance) with any number of initial position derivatives (position, velocity, acceleration, jerk, etc.). A shooter (also a point for simplicity) is also moving with its own unlimited initial position derivatives. The goal is to shoot a projectile (again, a point) such that it hits the object with the least effort possible; that is, we aim to minimize the 2-norm (magnitude) of the initial velocity vector (equivalent to minimizing the squared magnitude).
The position derivatives of the shooter are transferred to the projectile during launch.

Also, please note that I use the term "position derivatives" interchangeably with "movement vectors".

## Reasoning
This problem when the initial position derivative vectors are limited to constant acceleration is studied in high school, so let's review that first. First, we can take the shooter as the reference point to simplify the problem.
Let $x(t)$ be the position vector of some object. Define $x(0)=p$, $\dot x(0)=v$, $\ddot x(0)=a$ where the dot denotes the derivative with respect to time. The acceleration (2nd derivative) is constant here, which means the higher order derivatives will be $0$.

Notice that this system can be explicitly solved quite simply. If we integrate both sides of each equation, we will find that
$$x(t)=p+vt+\frac 1 2 at^2$$So we have found an explicit formulation for the system. In fact, this formulation can be extended for any amount of derivatives, leading us to our model of the system with respect to time as a Taylor polynomial:
$$x(t)=x(0)+t\dot x(0)+\frac{t^2}{2}\ddot x(0)+\frac{t^3}{6} x^{(3)}(0)+...$$
$$=\sum_{k=0}^{n} \frac{t^k}{k!}x^{(k)}(0)$$ where $n$ is the highest order non-zero derivative and $x^{(k)}(0)$ denotes the initial value of the $k$-th position derivative.

Great, now that we have a way to model the system, we can work on solving the problem.

First, what does it mean for the projectile to hit the target? Well, in order to intersect, they must have be at the same position. In equation form,
$$x_{projectile}(T)=x_{target}(T)$$
$$\iff 0=x_{target}(T)-x_{projectile}(T)$$
At the critical time of intersection $T$.
Okay, we know how to model $x_{target}(t)$ based on its initial position derivatives. How about $x_{projectile}$? Since we know the shooter's initial movement vectors, let's break this down in terms of the shooter's position and the relative position between the projectile and the shooter to study how the projectile itself moves. We will consider a form $x_{projectile}\colonequals x_{shooter}+\Delta x_{sp}$ where $\Delta x_{sp}(t)$ is the displacement from the shooter to the projectile.
However, we only want to minimize a single initial condition in order to have a unique solution to the problem. Since we are minimizing the initial velocity vector $\dot {\Delta x_{sp}}(0) \colonequals v$, we find that $\Delta x_{sp}(t)=tv$.

Referring back to the original problem statement, we wanted to minimize the magnitude of the initial velocity vector $v$. We are in luck! It is possible to isolate the term $v$ in the intersection equation
$$0=x_{target}(T)-x_{projectile}(T)$$
$$\iff 0=x_{target}(T)-(x_{shooter}(T)+\Delta x_{sp}(T))$$
$$\iff 0=x_{target}(T)-x_{shooter}(T)-Tv$$
Therefore, we get that
$$
\begin{equation}
v=\frac{x_{target}(T)-x_{shooter}(T)}{T} \tag{velocity}
\end{equation}
$$
Notice that the magnitude $\lVert v \rVert$ can be totally formulated in terms of a single unknown $T$; we know all the other information. So we just need to minimize this function $\lVert v(T) \rVert ^2$ by finding the ideal time of intersection $T$.



To recap, we have reduced our problem into a minimization of a rational function, more specifically a Laurent polynomial. I have gone down the rabbit hole of this problem when trying to find some methods to tackle it such as [[1](https://mathweb.ucsd.edu/~njw/PUBLICPAPERS/sosgcd.pdf)] and [[2](https://www.researchgate.net/publication/226393980_Global_Optimization_of_Rational_Functions_A_Semidefinite_Programming_Approach)], but ultimately I think that it is simplest to use the traditional single-variable calculus approach.

Therefore, in order to minimize this function, we will take the derivative and set it to zero to find the critical points (the only other critical point here would be $T=0$, but we are not interested in that case).

NOTE: Okay, after some thought, the velocity function should probably be expanded into a Laurent polynomial since the scalar coefficients will not change for each target. This simplifies the derivative calculation so much, so that you don't have to use the quotient rule to expand everything out and waste many calculations. Indeed it is necessary to find the coefficients of the actual polynomial for the root-finding algorithms anyways (this is done by shifted the exponents of the Laurent polynomial), and it also boosts performance. Expanding requires us to compute a double sum of length $n$, degree of the Taylor polynomial, to find each coefficient, so it is on the order of $n$ vector scalings for vector coefficients $\vec {c_k}:= \frac 1 {k!} \vec{a_k}$ and $\frac {(n+1)(n+2)}{2}$ unique dot products for $\vec {c_i} \cdot \vec{c_j}$ for $i,j,k$ from $0$ to $n$. 

The dot products are around is $d\frac {(n+1)(n+2)}{2}$ scalar multiplications for $d$-dimensional vectors. Subsequently, each evaluation of this polynomial is only $2n$ scalar multiplications and additions using Horner's method, which will speed things up tremendously. Not expanding as a double sum and caching the values instead results in having to re-do the same calculations each time, so it is beneficial in that regard.

Also, the expansion into scalar coefficients will allow us to rapidly find the derivative's coefficients without having to expand as a vector dot product, using the standard derivative algorithm for scalar coefficient Laurent polynomials.

Since the vector $x(T)$ is modeled by a polynomial in $T$, then believe it or not, the following dot product is a polynomial in $T$! So we have reduced this problem once again into finding all the positive roots/zeroes of the derivative, then testing them one-by-one into the function $$\lVert v(T) \rVert^2 \coloneqq \frac {x(T)\cdot x(T)}{T^2}$$ in order to find the minimum.

# Deprecated

There exists the second derivative test to make sure that the derivative's root will result in a local minimum, which will reduce the number of candidates to choose from; however, for our purposes, the polynomials should be fairly low degree anyways since if there are very high order derivatives of position, the objects will quickly fly off to infinity. Based on this, I judged that it would not be too useful. For some approaches on finding roots of polynomials, details can be found in the `polynomials` folder.

As a quick side note, for actual computational purposes, it is very useful that
$$\Delta x_{ts}(T) = x_{target}(T)-x_{shooter}(T)=\sum_{k=0}^{n} \frac{T^k}{k!}x^{(k)}_{target}(0) -\sum_{k=0}^{n} \frac{T^k}{k!}x^{(k)}_{shooter}(0)$$ is equivalent to 
$$
(x_{target}-x_{shooter})(T) = \sum_{k=0}^{n} \frac{T^k}{k!}\left(x^{(k)}_{target}(0)-x^{(k)}_{shooter}(0)\right)
$$ Mathematically, the 2 formulations are totally equivalent, but the second one is cheaper computationally with only $n$ vector subtractions, $n$ vector scalings and $n$ vector additions, compared to the first one which requires $2n$ vector scalings, $2n$ vector additions and $1$ vector subtraction. 

Let's now expand the derivative of the objective minimized function to see what we obtain.
$$\frac{d}{dT} \lVert v(T) \rVert ^2=\frac{d}{dT} v(T)\cdot v(T)$$$$=2v(T)\cdot\frac{dv(T)}{dT} = 0$$ $$\iff v(T)\cdot\frac{dv(T)}{dT} = 0$$
To simplify the following algebra we will define $x(T)=x_{target}(T)-x_{shooter}(T)$ as the relative position from the shooter to the target.
From equation $(\text{velocity})$ we obtain $v(T)=\frac{x(T)}{T}$ so
$$\frac{x(T)}{T}\cdot \frac{d}{dT} \left( \frac{x(T)}{T} \right)=0$$
$$\implies \frac{x(T)}{T}\cdot \frac{x(T)-T \frac{dx}{dT}(T)}{T^2} = 0$$
Since we said we ignored $T=0$ this is equivalent to
$$x(T)\cdot \left(T \frac{dx}{dT}(T)-x(T)\right) = 0$$
Expainding the dot product is not so useful and only results in more computation, so we will keep it in this form.

Once we have determined the ideal value of $T$ to give the minimum velocity magnitude, then we can plug it back into the equation $(\text{velocity})$:
$$
\begin{equation}
v(T)=\frac{x_{target}(T)-x_{shooter}(T)}{T} \tag{velocity}
\end{equation}
$$

In fact, this problem can be generalized to any single unknown position derivative to be minimized in magnitude, simply solving for the unknown within the equation 
$$0=x_{target}(T)-(x_{shooter}(T)+\Delta x_{sp}(T))$$
where $\Delta x_{sp}(T) \coloneqq \frac{t^k}{k!}x^{(k)}(T)$ for some $k$-th position derivative.

Also, because the spatial dimensions are not specified (they are implicit in the dot products), this also works for any number of dimensions, $1D$, $2D$, $3D$, $4D$, $\infty D$!
