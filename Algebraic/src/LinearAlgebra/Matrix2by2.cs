using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Algebraic.Numbers.Integers;

namespace Algebraic.LinearAlgebra
{
    [DebuggerDisplay("[[{m11}, {m12}], [{m21}, {m22}]]")]
    internal sealed class Matrix2By2 : Matrix<BigInteger>
    {
        private BigInteger m11;
        private BigInteger m12;
        private BigInteger m21;
        private BigInteger m22;

        public Matrix2By2(BigInteger m11, BigInteger m12, BigInteger m21, BigInteger m22)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m21 = m21;
            this.m22 = m22;
        }

        public override int Rows => 2;
        public override int Cols => 2;

        public override BigInteger this[int col, int row]
        {
            get
            {
                Contract.Requires(col >= 0);
                Contract.Requires(col < 2);
                Contract.Requires(row >= 0);
                Contract.Requires(row < 2);

                return col == 0 ? (row == 0 ? m11 : m21) : (row == 0 ? m12 : m22);
            }
            set
            {
                Contract.Requires(col >= 0);
                Contract.Requires(col < 2);
                Contract.Requires(row >= 0);
                Contract.Requires(row < 2);

                if (col == 0)
                    if (row == 0)
                        m11 = value;
                    else
                        m21 = value;
                else if (row == 0)
                    m12 = value;
                else
                    m22 = value;
            }
        }

        public static Matrix2By2 operator *(Matrix2By2 lop, Matrix2By2 rop) => new Matrix2By2(
            lop.m11 * rop.m11 + lop.m12 * rop.m21,
            lop.m11 * rop.m12 + lop.m12 * rop.m22,
            lop.m21 * rop.m11 + lop.m22 * rop.m21,
            lop.m21 * rop.m12 + lop.m22* rop.m22);

        public override Matrix<BigInteger> Multiply(Matrix<BigInteger> other)
        {
            Contract.Requires(other.Rows == 2);
            if (other is Matrix2By2 otherTyped)
                return this * otherTyped;

            throw new NotImplementedException();
        }
    }
}