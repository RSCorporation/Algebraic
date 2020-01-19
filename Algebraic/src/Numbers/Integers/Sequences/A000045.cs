using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Algebraic.LinearAlgebra;
using Algebraic.Numbers.Integers;

// ReSharper disable once CheckNamespace
namespace Algebraic.Numbers
{
    public static partial class IntegerSequence
    {
        private static class A000045
        {
            [Pure]
            private static Matrix2By2 CustomMatrixMultiply(Matrix2By2 lop, Matrix2By2 rop)
            {
                var a1111 = lop[0, 0] * rop[0, 0];
                var a1212 = lop[0, 1] * rop[0, 1];
                var a1112 = lop[0, 0] * rop[0, 1];
                var a1222 = lop[0, 1] * rop[1, 1];
                var a2222 = lop[1, 1] * rop[1, 1];
                a1112.Add(a1222);
                a1111.Add(a1212);
                a1212.Add(a2222);
                return new Matrix2By2(
                    a1111,
                    a1112,
                    a1112,
                    a1212);
            }

            [Pure]
            private static Matrix2By2 CustomPow(Matrix2By2 a, BigInteger power)
            {
                if (power == 1)
                    return a;

                var tmp = CustomPow(a, power / 2);
                return power % 2 == 0
                    ? CustomMatrixMultiply(tmp, tmp)
                    : CustomMatrixMultiply(tmp, CustomMatrixMultiply(tmp, a));
            }

            [Pure]
            public static BigInteger GetElement(BigInteger idx)
            {
                if (idx == 0)
                    return 0;
                var matrix = new Matrix2By2(1, 1, 1, 0);
                return CustomPow(matrix, idx)[0, 1];
            }

            [Pure]
            public static IEnumerable<BigInteger> Sequence()
            {
                BigInteger curr = 0, next = 1;
                while (true)
                {
                    yield return curr;
                    (curr, next) = (next, curr + next);
                }

                // ReSharper disable once IteratorNeverReturns
            }
        }
    }
}