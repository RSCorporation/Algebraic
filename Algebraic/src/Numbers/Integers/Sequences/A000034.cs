using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Algebraic.Numbers.Integers;

// ReSharper disable once CheckNamespace
namespace Algebraic.Numbers
{
    public static partial class IntegerSequence
    {
        private static class A000034
        {
            [Pure]
            public static BigInteger GetElement(BigInteger idx)
            {
                return idx.IsEven ? 1 : 2;
            }

            [Pure]
            public static IEnumerable<BigInteger> Sequence()
            {
                while (true)
                {
                    yield return 1;
                    yield return 2;
                }
                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}