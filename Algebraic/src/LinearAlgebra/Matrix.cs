using System.Diagnostics.Contracts;
using System.Numerics;

namespace Algebraic.LinearAlgebra
{
    public abstract class Matrix<T>
    {
        public abstract int Rows { get; }
        public abstract int Cols { get; }

        public abstract T this[int col, int row] { get; set; }

        public abstract Matrix<T> Multiply(Matrix<T> other);

        public Matrix<T> Pow(BigInteger power)
        {
            Contract.Requires(power.Sign > 0);
            if (power == 1)
                return this;

            var tmp = Pow(power / 2);
            return power % 2 == 0 ? tmp.Multiply(tmp) : tmp.Multiply(tmp).Multiply(this);
        }
    }
}