using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Algebraic.Numbers.Integers;

// ReSharper disable once CheckNamespace
namespace Algebraic.Numbers
{
    public static partial class IntegerSequence
    {
        private static class A000027
        {
            [Pure]
            public static BigInteger GetElement(BigInteger idx)
            {
                return idx + 1;
            }

            [Pure]
            public static IEnumerable<BigInteger> Sequence()
            {
                BigInteger curr = 1;
                while (true)
                {
                    yield return curr;
                    curr.Increment();
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}