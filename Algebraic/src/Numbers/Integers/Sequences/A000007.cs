using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Algebraic.Numbers.Integers;

// ReSharper disable once CheckNamespace
namespace Algebraic.Numbers
{
    public static partial class IntegerSequence
    {
        private static class A000007
        {
            [Pure]
            public static BigInteger GetElement(BigInteger idx)
            {
                return idx.IsZero ? 1 : 0;
            }

            [Pure]
            public static IEnumerable<BigInteger> Sequence()
            {
                yield return 1;
                while (true)
                {
                    yield return 0;
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}