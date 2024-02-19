# My approach to solving this problem
To understand the my approach to finding a solution, it's probably a good idea to state the problem in detail first.
## Problem statement
An point ("target") is moving in a vacuum (no air resistance) with any number of initial position derivatives (position, velocity, acceleration, jerk, etc.). A shooter (also a point for simplicity) is also moving with its own unlimited initial position derivatives. The goal is to shoot a projectile (again, a point) such that it hits the object with the least effort possible; that is, we aim to minimize the 2-norm (magnitude) of the initial velocity vector (equivalent to minimizing the squared magnitude).
The position derivatives of of the shooter are transferred to the projectile during launch.

Also, please note that I use the term "position derivatives" interchangeably with "movement vectors".

## Reasoning
This problem when the initial positions vectors are limited to constant acceleration is studied in high school, so let's review that first. First, we can take the shooter as the reference point to simplify the problem.
Let $x(t)$ be the position vector of some object. Define $x(0)=p$, $\dot x(0)=v$, $\ddot x(0)=a$ where the dot denotes the derivative with respect to time. The acceleration (2nd derivative) is constant here, which means the higher order derivatives will be $0$.

Notice that this system can be explicitly solved quite simply. If we integrate both sides of each equation, we will find that
$$x(t)=p+vt+\frac 1 2 at^2$$So we have found an explicit formulation for the system. In fact, this formulation can be extended for any amount of derivatives, leading us to our model of the system with respect to time as a Taylor polynomial:
$$x(t)=x(0)+t\dot x(0)+\frac{t^2}{2}\ddot x(0)+\frac{t^3}{6}\ddot x(0)+...$$
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
 $$v=\frac{x_{target}(T)-x_{shooter}(T)}{T} \tag{1}$$
 Notice that the magnitude $\lVert v \rVert$ is totally formulated in terms of a single unknown $T$; we know all the other information. So we just need to minimize this function $\lVert v(T) \rVert ^2$.

 To recap, we have reduced our problem into a minimization of a rational function. I have gone down the rabbit hole of this problem when trying to find some methods to tackle it such as [[1](https://mathweb.ucsd.edu/~njw/PUBLICPAPERS/sosgcd.pdf)] and [[2](https://www.researchgate.net/publication/226393980_Global_Optimization_of_Rational_Functions_A_Semidefinite_Programming_Approach)], but ultimately I think that it is simplest to use the traditional single-variable calculus approach.

 Therefore, in order to minimize this function, we will take the derivative and set it to zero to find the critical points (the only other critical point here would be $T=0$, but we are not interesting in that case).
 $$\frac{d}{dT} \lVert v(T) \rVert ^2=\frac{d}{dT} v(T)\cdot v(T)$$$$=2v(T)\cdot\frac{dv(T)}{dT} = 0$$ $$\iff v(T)\cdot\frac{dv(T)}{dT} = 0$$
 To simplify the following algebra we will define $x(T)=x_{target}(T)-x_{shooter}(T)$ as the relative position from the shooter to the target.
 From equation $(1)$ we obtain
 $$\frac{x(T)}{T}\cdot \frac{d}{dT} \left( \frac{x(T)}{T} \right)=0$$
 $$\implies \frac{x(T)}{T}\cdot \frac{x(T)-T \frac{dx}{dT}(T)}{T^2} = 0$$
 Since we said we ignored $T=0$ this is equivalent to
 $$x(T)\cdot \left( x(T)-T \frac{dx}{dT}(T)\right) = 0$$
 Expainding the dot product is not so useful and only results in more computation, so we will keep it in this form. Since the vector $x(T)$ is modeled by a polynomial in $T$, then believe it or not, this whole dot product is a polynomial in $T$! So we have reduced this problem once again into finding all the positive roots/zeroes of a polynomial, then testing them one-by-one into the function $\lVert v(T) \rVert^2$ in order to find the minimum. For some approaches on finding these roots this, details can be found in the `polynomial-rootfinder` folder.