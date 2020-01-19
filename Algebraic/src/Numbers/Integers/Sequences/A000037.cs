using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Algebraic.Numbers.Integers;

// ReSharper disable once CheckNamespace
namespace Algebraic.Numbers
{
    public static partial class IntegerSequence
    {
        private static class A000037
        {
            [Pure]
            public static BigInteger GetElement(BigInteger idx)
            {
                idx.Increment();
                return idx + BigInteger.IntSqrt(BigInteger.IntSqrt(idx) + idx);
            }

            [Pure]
            public static IEnumerable<BigInteger> Sequence()
            {
                BigInteger candidate = 1, sqrt = 1, sqr = 1;
                while (true)
                {
                    if (candidate != sqr)
                        yield return candidate;
                    else
                    {
                        sqrt.Increment();
                        sqr = sqrt * sqrt;
                    }

                    candidate.Increment();
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}