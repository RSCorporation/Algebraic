using Algebraic.Numbers.Integers;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

// ReSharper disable once CheckNamespace
namespace Algebraic.Numbers
{
    public static partial class IntegerSequence
    {
        private static class A000004
        {
            [Pure]
            public static BigInteger GetElement(BigInteger idx)
            {
                return 0;
            }

            [Pure]
            public static IEnumerable<BigInteger> Sequence()
            {
                while (true)
                {
                    yield return 0;
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}