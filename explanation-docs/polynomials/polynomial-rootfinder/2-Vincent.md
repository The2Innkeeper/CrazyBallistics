This algorithm is a continued fraction algorithm for isolating the real roots of a square-free input polynomial, based on Vincent's theorem. It is more efficient than the method of bisection using Sturm's theorem, allowing us to minimize more polynomials in a shorter amount of time and therefore calculate the path for more projectiles.

Relevant links:

https://en.wikipedia.org/wiki/Vincent%27s_theorem
https://www.sciencedirect.com/science/article/pii/S0304397508006476
https://en.wikipedia.org/wiki/Real-root_isolation#Continued_fraction_method

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
Given this relation, the procedure $CF(A, M)$ returns a list of isolating intervals for the roots of $A_{in}(X)$ in $I_M$ . To isolate all the positive roots of $A_{in}(X)$, initiate $CF(A, M)$ with $A(X) \coloneqq A_{in}(X)$ and $M(X) \coloneqq X$; to isolate the negative roots of $A_{in}(X)$, initiate $CF(A, M)$ on $A_{in}(X) \coloneqq A_{in}(−X)$ and $M(X) \coloneqq X$, and swap the endpoints of the intervals returned while simultaneously changing their sign.
The procedure $CF(A, M)$ is as follows:
"

$$
\begin{aligned}
& \textbf{Finding all positive real roots of an input polynomial }A_{in}(X) \\
& \textbf{Initiate } A(X):=A_{in}(X), M(X):=X \\
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

```python
def CF_iterative(A, M):
    stack = [(A, M)]
    while stack:
        A, M = stack.pop()
        if A(0) == 0:
            print("Output Interval (M(0),M(0))")
            continue
        V = Var(A, X)
        if V == 0:
            continue
        if V == 1:
            print("Output Interval (M(0),M(∞))")
            continue
        b = PLB(A)  # PLB ≡ PositiveLowerBound
        if b > 1:
            A = A(b + X)
            M = M(b + X)
        A₁:=A(x+1), M₁:=M(x+1)
        stack.append((A₁, M₁))  # Looking for real roots in (1,+∞)
        A₂ = A(1/(x+1)), M₂ = M(1/(x+1))
        stack.append((A₂, M₂))  # Looking for real roots in (0,1)
```

I believe a Mobius transformation should be implementable using a series of Taylor shifts $x\coloneqq x+b$, inversions $x \coloneqq 1/x$ and scalings $x \coloneqq ax$. Let's test this theory.

I think a good way to proceed is to try to break down the expression by performing long division, so let's try:
$$\frac{ax+b}{cx+d} = \frac{a}{c} + \frac{b - \frac{ad}{c}}{cx+d}$$
It seems promising. We can first apply the scaling by $c$ and Taylor shift by $d$ to obtain $x\coloneqq cx+d$, then invert to get $x\coloneqq \frac{1}{cx+d}$. Afterwards we simply scale by $b-\frac{ad}{c}$ and shift by $\frac{a}{c}$ to obtain $x\coloneqq \frac{b - \frac{ad}{c}}{cx+d}=\frac{ax+b}{cx+d}$. I will see if I should hard-code the composed transformations or break them down into compositions of these simple transformations. The accumulated errors from repeated operations is likely to be problematic for small numbers.

An important transformation is $M(x)\coloneqq \frac{ax+b}{x+1}$, which maps the interval $]a,b[$ onto $]0,+\infty[$. As before, let's break it down into compositions of simple transformations, then trace our steps back (I might hard-code a transformation later in one step but for now I have suffered too many errors).
$$
M(x)\coloneqq \frac{ax+b}{x+1}=\frac{a}{1}+\frac{b-\frac{a\cdot1}{1}}{1x+1}=a+\frac{b-a}{x+1}
$$
Based on that, let's find the order of composition from last to first by following order of operations:
$$
\begin{aligned}
&1. \space a+\frac{b-a}{x+1} && \mapsto a+\frac{b-a}{x} &&& (x+1=:x) \\
&2. \space a+(b-a)\frac 1 x && \mapsto a+(b-a)x &&& (\frac 1 x=:x) \\
&3. \space a+(b-a)x && \mapsto a+x &&& ((b-a)x=:x) \\
&4. \space a+x && \mapsto x &&& (a+x=:x)
\end{aligned}
$$
Therefore, the order of composition of simple transformations will be 
$$\begin{aligned}
&1.\space x\coloneqq x+a \\
&2.\space x\coloneqq (b-a)x \\
&3.\space x\coloneqq \frac 1 x \\
&4.\space x\coloneqq x+1 \\
\end{aligned}$$

Then, in order to hard-code the transformation $x\coloneqq \frac 1{x+1}$, let's see what we can do: $$M\left(\frac1{x+1}\right)=\frac{a\frac{1}{x+1}+b}{c\frac{1}{x+1}+d}=\frac{a+b(x+1)}{c+d(x+1)}=\frac{(b)x+(a+b)}{(d)x+(c+d)}$$
In other words: $a\coloneqq b,b\coloneqq a+b,c\coloneqq d,d\coloneqq c+d$.

When expanding out the Taylor shifted polynomials to find the coefficients, a naive implementation has a time complexity of $O(degree(p)^2)$. Luckily, there are better methods than this, such as the [convolution method (section F)](https://dl.acm.org/doi/10.1145/258726.258745). For our case it is probably overkill but a fun project to implement, surely.

As of newer developments, it seems like [Wikipedia](https://en.wikipedia.org/wiki/Real-root_isolation#Continued_fraction_method) has a non-recursive implementation of the algorithm. I will try it as the recursive version is not working after many attempts.

$$
\begin{aligned}
& \textbf{function: continued fraction} \\
& \quad \textbf{input:} \, P(x), \text{ a square-free polynomial,} \\
& \quad \textbf{output:} \, \text{a list of pairs of rational numbers defining isolating intervals} \\
& \text{ /* Initialization */} \\
& \quad L := [(P(x), x), (P(–x), –x)] \quad \text{/* two starting intervals */} \\
& \quad Isol := [ ] \\
& \text{ /* Computation */} \\
& \quad \textbf{while} \, L \neq [ ] \, \textbf{do} \\
& \quad \quad \text{Choose} \, (A(x), M(x)) \, \text{in} \, L, \text{ and remove it from} \, L \\
& \quad \quad v := \text{var}(A) \\
& \quad \quad \textbf{if} \, v = 0 \, \textbf{then exit} \quad \text{/* no root in the interval */} \\
& \quad \quad \textbf{if} \, v = 1 \, \textbf{then} \quad \text{/* isolating interval found */} \\
& \quad \quad \quad \text{add} \, (M(0), M(\infty)) \, \text{to} \, Isol \\
& \quad \quad \quad \textbf{exit} \\
& \quad \quad b := \text{some positive integer} \\
& \quad \quad B(x) = A(x + b) \\
& \quad \quad w := v – \text{var}(B) \\
& \quad \quad \textbf{if} \, B(0) = 0 \, \textbf{then} \quad \text{/* rational root found */} \\
& \quad \quad \quad \text{add} \, (M(b), M(b)) \, \text{to} \, Isol \\
& \quad \quad \quad B(x) := B(x)/x \\
& \quad \quad \text{add} \, (B(x),  M(b + x)) \, \text{to} \, L \quad \text{/* roots in (M(b), M(+∞)) */} \\
& \quad \quad \textbf{if} \, w = 0 \, \textbf{then exit} \quad \text{/* Budan's theorem */} \\
& \quad \quad \textbf{if} \, w = 1 \, \textbf{then} \quad \text{/* Budan's theorem again */} \\
& \quad \quad \quad \text{add} \, (M(0), M(b)) \, \text{to} \, Isol \\
& \quad \quad \textbf{if} \, w > 1 \, \textbf{then} \\
& \quad \quad \quad \text{add} \, ( A(b/(1 + x)),  M(b/(1 + x)) ) \, \text{to} \, L \quad \text{/* roots in (M(0), M(b)) */}
\end{aligned}
$$

As you can see we need a transformation of the kind $M(s/(1+x))$. Let's see how it affects the numbers:
$$M\left(\frac s{1+x}\right) = \frac{a(\frac{s}{1+x})+b}{c(\frac{s}{1+x})+d}=\frac{as+b(1+x)}{cs+d(1+x)}=\frac{(b)x+(as+b)}{(d)x+(cs+d)}$$
Therefore $$a:=b \\ b:=b+as \\ c:=d \\ d:=d+cs$$

Now for the polynomial $(x+1)^n P(\frac{s}{x+1})$... it is probably too complicated to do in 1 step, so we will use the composition of transformations discussed earlier. We will break it down into a composition by making substitution and backtracking our steps.
$$
\begin{aligned}
&1. \space \frac s {x+1} && \mapsto \frac s x &&& (x+1=:x) \\
&2. \space \frac s x && \mapsto sx &&& (\frac 1 x=:x) \\
&3. \space sx && \mapsto x &&& (sx=:x)
\end{aligned}
$$

Therefore we will perform the transformations in the order
$$
\begin{aligned}
&1. \space x:=sx \\
&2. \space x:=\frac 1 x \\
&3. \space x:=x+1
\end{aligned}
$$

Still not working after a long while, I think [logcf (arXiv:1209.3555)](https://arxiv.org/pdf/1209.3555.pdf) should be good:

$$
\begin{aligned}
& \textbf{Algorithm 4. cf} \\
& \textbf{Input: } \text{A squarefree polynomial } F \in Z[x] \setminus \{0\}. \\
& \textbf{Output: } \text{roots, a list of isolating intervals of positive roots of } F. \\
& 1 \quad \text{roots} = \emptyset; \quad s = V (F); \\
& 2 \quad \text{intstack} = \emptyset; \quad \text{intstack.add}(\{1, 0, 0, 1, F, s\}); \\
& 3 \quad \text{while intstack} \neq \emptyset \text{ do} \\
& 4 \quad \quad \{a, b, c, d, P, s\} = \text{intstack.pop()}; \quad \text{/* pop the first element */} \\
& 5 \quad \quad \alpha = \text{loglb}(P); \\
& 6 \quad \quad \text{if } \alpha \geq 1 \text{ then} \\
& 7 \quad \quad \quad \{a, c, P\} = \{\alpha a, \alpha c, H_\alpha(P)\}; \quad \{b, d, P\} = \{a + b, c + d, T(P)\}; \\
& 8 \quad \quad \quad \text{if } P(0) == 0 \text{ then} \\
& 9 \quad \quad \quad \quad \text{roots.add}([ \frac{b}{d}, \frac{b}{d} ]) ; \quad P = \frac{P}{x} ; \\
& 10 \quad \quad \quad \quad s = V (P); \\
& 11 \quad \quad \quad \quad \text{if } s == 0 \text{ then} \\
& 12 \quad \quad \quad \quad \quad \text{continue}; \\
& 13 \quad \quad \quad \quad \text{else if } s == 1 \text{ then} \\
& 14 \quad \quad \quad \quad \quad \text{roots.add(intvl(a, b, c, d)); continue}; \\
& 15 \quad \quad \quad \quad \{P1, a1, b1, c1, d1, r\} = \{T(P), a, a + b, c, c + d, 0\} \\
& 16 \quad \quad \quad \quad \text{if } P1(0) == 0 \text{ then} \\
& 17 \quad \quad \quad \quad \quad \text{roots.add}([ \frac{b1}{d1}, \frac{b1}{d1} ]); \quad P1 = \frac{P1}{x} ; \quad r = 1; \\
& 18 \quad \quad \quad \quad s1 = V (P1); \quad \{s2, a2, b2, c2, d2\} = \{s - s1 - r, b, a + b, d, c + d\}; \\
& 19 \quad \quad \quad \quad \text{if } s2 > 1 \text{ then} \\
& 20 \quad \quad \quad \quad \quad P2 = (x + 1)^{\text{deg}(P)}T(P); \\
& 21 \quad \quad \quad \quad \quad \text{if } P2(0) == 0 \text{ then} \\
& 22 \quad \quad \quad \quad \quad \quad P2 = \frac{P2}{x} ; \quad s2 = V (P2); \\
& 23 \quad \quad \quad \quad \quad \text{if } s1 == 1 \text{ then} \\
& 24 \quad \quad \quad \quad \quad \quad \text{roots.add(intvl(a1, b1, c1, d1))}; \\
& 25 \quad \quad \quad \quad \quad \text{else if } s1 > 1 \text{ then} \\
& 26 \quad \quad \quad \quad \quad \quad \text{intstack.add}(\{a1, b1, c1, d1, P1, s1\}); \\
& 27 \quad \quad \quad \quad \quad \text{if } s2 == 1 \text{ then} \\
& 28 \quad \quad \quad \quad \quad \quad \text{roots.add(intvl(a2, b2, c2, d2))}; \\
& 29 \quad \quad \quad \quad \quad \text{else if } s2 > 1 \text{ then} \\
& 30 \quad \quad \quad \quad \quad \quad \text{intstack.add}(\{a2, b2, c2, d2, P2, s2\});
\end{aligned}
$$
