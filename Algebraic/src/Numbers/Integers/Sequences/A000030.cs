using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Algebraic.Numbers.Integers;

// ReSharper disable once CheckNamespace
namespace Algebraic.Numbers
{
    public static partial class IntegerSequence
    {
        private static class A000030
        {
            [Pure]
            public static BigInteger GetElement(BigInteger idx)
            {
                return idx / BigInteger.Pow(10, (int)BigInteger.Log10(idx));
            }

            [Pure]
            public static IEnumerable<BigInteger> Sequence()
            {
                yield return 0;
                for (BigInteger ln = 1; ; ln.Multiply(10))
                {
                    for (var ans = 1; ans < 10; ans++)
                        for (BigInteger cnt = 0; cnt < ln; cnt.Increment())
                            yield return ans;
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}