using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Algebraic.Numbers.Integers;

// ReSharper disable once CheckNamespace
namespace Algebraic.Numbers
{
    public static partial class IntegerSequence
    {
        private static class A000012
        {
            [Pure]
            public static BigInteger GetElement(BigInteger idx)
            {
                return 1;
            }

            [Pure]
            public static IEnumerable<BigInteger> Sequence()
            {
                while (true)
                {
                    yield return 1;
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}