This algorithm is a continued fraction algorithm for isolating the real roots of a square-free input polynomial, based on Vincent's theorem. It is more efficient than the method of bisection using Sturm's theorem, allowing us to minimize more polynomials in a shorter amount of time and therefore calculate the path for more projectiles.

Relevant links:

https://en.wikipedia.org/wiki/Vincent%27s_theorem
https://www.sciencedirect.com/science/article/pii/S0304397508006476

Here's a plain language description:

The continued fractions algorithm is a method for finding the real roots of a polynomial, i.e., the values of x that make the polynomial equal to zero. The algorithm works by transforming the polynomial into simpler polynomials that have fewer sign changes in their coefficients, using two operations: Taylor shift and inversion. A Taylor shift by $b$ is a change of variable $x \coloneqq x + b$, and an inversion is a change of variable $x \coloneqq \frac{1}{x + 1}$. The algorithm uses a lower bound on the smallest positive root of the polynomial to decide how much to shift by, and uses the Descartes’ rule of signs to determine when a polynomial has at most one positive root. The algorithm also keeps track of the intervals that contain the roots, using rational fractions to represent the endpoints. The algorithm stops when all the intervals have been output.

- This algorithm takes a polynomial and a Möbius transformation as inputs. (The Möbius transformation is a function that transforms the polynomial in a specific way, specifically it is a function of the form $M(x) = \frac{ax+b}{cx+d}$. See [Wikipedia](https://en.wikipedia.org/wiki/M%C3%B6bius_transformation) for more information)
- The algorithm then checks if the polynomial has any roots (values of $X$ for which the polynomial equals zero).
  - If a root is found, it is isolated and returned. If no roots are found, the algorithm checks the number of sign changes in the polynomial.
  - If there's only one sign change, the algorithm returns the interval of the transformation.
  - If there's more than one sign change, the algorithm calculates a lower bound on the smallest positive root of the polynomial. If this lower bound is greater than or equal to 1, the polynomial and the transformation are updated.
- The algorithm then recursively calls itself with the updated polynomial and transformation until all roots are found.

From [(Sharma, 2008)](https://www.sciencedirect.com/science/article/pii/S0304397508006476):
"The second crucial component is a procedure $PLB(A)$ that takes as input a polynomial $A(X)$ and returns a lower bound on the smallest positive root of $A(X)$, if such a root exists; our implementation of this procedure, however, will only guarantee a weaker inequality, namely a lower bound on the smallest real part amongst all the roots of $A(X)$ in the positive half plane.

Given these two components, the continued fraction algorithm for isolating the real roots of a square-free input polynomial $A_{in}(X)$ uses a recursive procedure $CF(A, M)$ that takes as input a polynomial $A(X)$ and a Möbius transformation $$M(X) = \frac{pX+q}{rX+s}$$, where $p,q,r,s \in \mathbb{N}$ and $ps − rq \neq 0$. The interval $I_M$ associated with the transformation $M(X)$ has endpoints $p/r$ and $q/s$. The relation among $A_{in}(X)$, $A(X)$ and $M(X)$ is the following:
$$A(X) = (rX + s)^n \cdot A_{in}(M(X)) \tag{3}$$
Given this relation, the procedure $CF(A, M)$ returns a list of isolating intervals for the roots of $A_{in}(X)$ in $I_M$ . To isolate all the positive roots of $A_{in}(X)$, initiate CF(A, M) with A(X) = $A_{in}(X)$ and M(X) = X; to isolate the negative roots of $A_{in}(X)$, initiate CF(A, M) on $A_{in}(X) \coloneqq A_{in}(−X)$ and $M(X) \coloneqq X$, and swap the endpoints of the intervals returned while simultaneously changing their sign.
The procedure CF(A, M) is as follows:
"

$$
\begin{aligned}
& \textbf{Procedure} \, \text{CF}(A, M) \\
& \quad \textbf{Input}: \text{A square-free polynomial } A(X) \text{ in } \mathbb{R}[X] \text{ and a Möbius transformation } M(X) \text{ satisfying (3)} \\
& \quad \textbf{Output}: \text{A list of isolating intervals for the roots of } A_{\text{in}}(X) \text{ in } I_M \\
\\
& \quad \textbf{If } A(0) = 0 \textbf{ then} \\
& \quad \quad \text{Output the interval } [M(0), M(0)] \\
& \quad \quad A(X):= A(X)/X \\
& \quad \quad \textbf{return} \, \text{CF}(A, M) \\
& \quad \textbf{If } \text{Var}(A) = 0 \textbf{ then return} \\
& \quad \textbf{If } \text{Var}(A) = 1 \textbf{ then} \\
& \quad \quad \text{Output the interval }I_M \\
& \quad \quad \textbf{return} \\
& \quad b := \text{PLB}(A) \\
& \quad \textbf{If } b \geq 1 \textbf{ then} \\
& \quad \quad A(X):= A(X + b) \\
& \quad \quad M(X):= M(X + b) \\
& \quad A_{R}(X):= A(1 + X) \\
& \quad M_{R}(X):= M(1 + X) \\
& \quad \text{CF}(A_{R}, M_{R}) \\
& \quad \textbf{If } \text{Var}(A_{R}) < \text{Var}(A) \textbf{ then} \\
& \quad \quad A_{L}(X):= (1 + X)^nA\left(\frac{1}{1+X}\right) \\
& \quad \quad M_{L}(X):= M\left(\frac{1}{1+X}\right) \\
& \quad \quad \textbf{If } A_{L}(0) = 0 \textbf{ then } A_{L}(X):=A_{L}(X)/X. \\
& \quad \quad \text{CF}(A_{L},M_{L})
\end{aligned}
$$


I believe a Mobius transformation should be implementable using a series of Taylor shifts $x\coloneqq x+b$, inversions $x \coloneqq 1/x$ and scalings $x \coloneqq ax$. Let's test this theory.

I think a good way to proceed is to try to break down the expression by performing long division, so let's try:
$$\frac{ax+b}{cx+d} = \frac{a}{c} + \frac{b - \frac{ad}{c}}{cx+d}$$
It seems promising. We can first apply the scaling by $c$ and Taylor shift by $d$ to obtain $x\coloneqq cx+d$, then invert to get $x\coloneqq \frac{1}{cx+d}$. Afterwards we simply scale by $b-\frac{ad}{c}$ and shift by $\frac{a}{c}$ to obtain $x\coloneqq \frac{b - \frac{ad}{c}}{cx+d}=\frac{ax+b}{cx+d}$. I will see if I should hard-code the composed transformations or break them down into compositions of these simple transformations.

In order to hard-code the transformation $x\coloneqq \frac 1{x+1}$, let's see what we can do: $$M\left(\frac1{x+1}\right)=\frac{a\frac{1}{x+1}+b}{c\frac{1}{x+1}+d}=\frac{a+b(x+1)}{c+d(x+1)}=\frac{(b)x+(a+b)}{(d)x+(c+d)}$$
In other words: $a\coloneqq b,b\coloneqq a+b,c\coloneqq d,d\coloneqq c+d$.

When expanding out the Taylor shifted polynomials to find the coefficients, a naive implementation has a time complexity of $O(degree(p)^2)$. Luckily, there are better methods than this, such as the [convolution method (section F)](https://dl.acm.org/doi/10.1145/258726.258745). For our case it is probably overkill but a fun project to implement, surely.