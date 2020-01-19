using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
#if PLATFORM_X86 || PLATFORM_ANYCPU
using workType = System.UInt32;
#else
using workType = System.UInt64;
#endif

namespace Algebraic.Numbers.Integers
{
    /// <summary>
    ///     Structure to work with really big integers. Can be not as efficient as built-in for small integers.
    /// </summary>
    [SuppressMessage("ReSharper", "BuiltInTypeReferenceStyle")]
    public struct BigInteger : IComparable<BigInteger>, IComparable<int>, IEquatable<BigInteger>, IEquatable<int>, IFormattable
    {

        private int sign;
        /// <summary>
        ///     Little endian digits
        /// </summary>
        private workType[] bits;
        private int ptr;
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> structure using the values in a byte array.
        /// </summary>
        /// <param name="value">An array of byte values in little-endian order.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c></exception>
        public BigInteger(byte[] value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            bits = new workType[(value.Length + sizeof(workType) - 1) / sizeof(workType)];
            ptr = bits.Length;
            //copy data using the right order of bytes
            if (OptimizationsManager.IsLittleEndianSystem)
            {
                int preln = value.Length - value.Length % sizeof(workType);
                Buffer.BlockCopy(value, 0, bits, 0, preln);
                if (preln != value.Length)
                    for (var d = preln; d < value.Length; d++)
                    {
                        bits[preln / sizeof(workType)] <<= 8;
                        bits[preln / sizeof(workType)] |= value[d];
                    }
            }
            else
            {
                for (var d = 0; d < bits.Length; d++)
                    for (var b = d * sizeof(workType); b < value.Length && b < (d + 1) * sizeof(workType); b++)
                    {
                        bits[d] <<= 8;
                        bits[d] |= value[b];
                    }
            }
            //Process negative
            if ((value[^1] & 0b10000000) != 0)
            {
                sign = -1;
                for (var d = 0; d < bits.Length; d++)
                {
                    bits[d] = ~bits[d];
                }
                IncrementRaw();
            }
            else
            {
                sign = 1;
            }
            ShrinkPtr();
        }
        //TODO: Add constructor from decimal
        //TODO: Add constructor from double
        //TODO: Add comstructor from float
        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> structure using a 32-bit signed integer value.
        /// </summary>
        /// <param name="value">A 32-bit signed integer.</param>
        public BigInteger(int value)
        {
            bits = new workType[OptimizationsManager.BigIntegerDefaultAllocation];
            if (value == 0)
            {
                sign = 0;
                ptr = 0;
                return;
            }

            sign = Math.Sign(value);
            bits[0] = (workType)Math.Abs(value);
            ptr = 1;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="BigInteger"/> structure using a 64-bit signed integer value.
        /// </summary>
        /// <param name="value">A 64-bit signed integer.</param>
        public BigInteger(long value)
        {
            bits = new workType[OptimizationsManager.BigIntegerDefaultAllocation];
            if (value == 0)
            {
                sign = 0;
                ptr = 0;
                return;
            }

            sign = Math.Sign(value);
            var absval = Math.Abs(value);
#if PLATFORM_X86 || PLATFORM_ANYCPU
            bits[0] = (uint)(absval & 0xFFFFFFFF);
            bits[1] = (uint)(absval >> 32);
            ptr = bits[1] == 0 ? 1 : 2;
#else
            bits[0] = (ulong)absval;
            ptr = 1;
#endif
        }
        /// <summary>
        /// Initializes a new instance of the BigInteger structure using an unsigned 32-bit integer value.
        /// </summary>
        /// <param name="value">An unsigned 32-bit integer value.</param>
        public BigInteger(uint value)
        {
            bits = new workType[OptimizationsManager.BigIntegerDefaultAllocation];
            if (value == 0)
            {
                sign = 0;
                ptr = 0;
                return;
            }
            sign = 1;
            ptr = 1;
            bits[0] = value;
        }
        /// <summary>
        /// Initializes a new instance of the BigInteger structure with an unsigned 64-bit integer value.
        /// </summary>
        /// <param name="value">An unsigned 64-bit integer.</param>
        public BigInteger(ulong value)
        {
            {
                bits = new workType[OptimizationsManager.BigIntegerDefaultAllocation];
                if (value == 0)
                {
                    sign = 0;
                    ptr = 0;
                    return;
                }
                sign = 1;
#if PLATFORM_X86 || PLATFORM_ANYCPU
            bits[0] = (uint)(value & 0xFFFFFFFF);
            bits[1] = (uint)(value >> 32);
            ptr = bits[1] == 0 ? 1 : 2;
#else
                bits[0] = value;
                ptr = 1;
#endif
            }
        }
        public BigInteger(BigInteger other)
        {
            sign = other.sign;
            bits = new workType[other.bits.Length];
            Buffer.BlockCopy(other.bits, 0, bits, 0, bits.Length);
            ptr = other.ptr;
        }
        #endregion
        #region Resizes
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Expand()
        {
            ptr++;
            if (ptr <= bits.Length)
                return;

            var newBits = new workType[bits.Length * OptimizationsManager.BigIntegerReallocationCoefficient];
            Buffer.BlockCopy(bits, 0, newBits, 0, bits.Length);
            bits = newBits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Allocate(int sz)
        {
            if (bits.Length >= sz)
                return;

            var newSz = bits.Length;
            while (newSz < sz)
                newSz *= OptimizationsManager.BigIntegerReallocationCoefficient;

            var newBits = new workType[newSz];
            Buffer.BlockCopy(bits, 0, newBits, 0, bits.Length);
            bits = newBits;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShrinkPtr()
        {
            while (ptr > 0 && bits[ptr - 1] == 0)
                ptr--;
            if (ptr == 0)
                sign = 0;
        }
        #endregion
        #region Carries
#if PLATFORM_X86 || PLATFORM_ANYCPU
        private static uint MulCarry(ref uint u1, uint u2, uint uCarry)
        {
            // This is guaranteed not to overflow.
            var uuRes = (ulong)u1 * u2 + uCarry;
            u1 = (uint)uuRes;
            return (uint)(uuRes >> 32);
        }
#else
        private static ulong MulCarry(ref ulong u1, ulong u2, ulong uCarry)
        {
            ulong a = (u1 >> 32), b = (uint)u1, c = (u2 >> 32), d = (uint)u2;
            ulong f = a * c, s = a * d, t = b * d;
            var sn = s + b * c;
            if (sn < s)
                f++;
            s = sn;
            u1 = (s << 32) + b * d;
            return (s >> 32) + a * c;
        }
#endif
        #endregion
        #region InplaceOperations

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddAbs(BigInteger other)
        {
            var c = false;
            Allocate(other.ptr);
            ptr = Math.Max(ptr, other.ptr);

            for (var d = 0; d < other.ptr; d++)
            {
                var nb = bits[d] + other.bits[d];
                if (c)
                    nb++;
                if (nb < bits[d] || (c && nb == bits[d]))
                    c = true;
                else
                    c = false;
                bits[d] = nb;
            }

            if (!c)
                return;

            for (; ; Expand())
                if (++bits[ptr] != 0)
                    break;

            Expand();
        }

        /// <summary>
        /// Requires absolute value of this is greater than absolute value of other
        /// </summary>
        /// <param name="other"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SubstractAbs(BigInteger other)
        {
            var c = false;
            for (var d = 0; d < other.ptr; d++)
            {
                var nb = bits[d] - other.bits[d];
                if (c)
                    nb--;
                if (nb > bits[d] || (c && nb == bits[d]))
                    c = true;
                else
                    c = false;
                bits[d] = nb;
            }

            if (!c)
                return;

            for (var d = other.ptr; ; d++)
                if (--bits[d] != workType.MaxValue)
                    break;
            ShrinkPtr();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(BigInteger other)
        {
            if (IsZero)
            {
                this = new BigInteger(other);
                return;
            }

            if (sign == other.sign)
            {
                AddAbs(other);
            }
            else
            {
                var cmp = AbsoluteCompare(other);
                if (cmp == 0)
                    this = 0;
                else if (cmp > 0)
                    SubstractAbs(other);
                else
                {
                    var nxt = new BigInteger(other);
                    nxt.SubstractAbs(this);
                    this = nxt;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Substract(BigInteger other)
        {
            if (IsZero)
            {
                this = new BigInteger(other);
                sign = -sign;
                return;
            }

            if (sign != other.sign)
                AddAbs(other);
            else
            {
                var cmp = AbsoluteCompare(other);
                if (cmp == 0)
                    this = 0;
                else if (cmp > 0)
                    SubstractAbs(other);
                else
                {
                    var nxt = new BigInteger(other);
                    nxt.SubstractAbs(this);
                    this = nxt;
                    sign = -sign;
                }
            }
        }

        public void Multiply(workType uSmall)
        {
            if (uSmall == 0)
            {
                this = 0;
                return;
            }

            if (uSmall == 1)
                return;

            workType uCarry = 0;
            for (var d = 0; d < ptr; d++)
            {
                uCarry = MulCarry(ref bits[d], uSmall, uCarry);
            }

            if (uCarry == 0) return;
            Expand();
            bits[ptr - 1] = uCarry;

        }

        private void IncrementRaw()
        {
            for (var d = 0; d < ptr; d++)
                if (++bits[d] != 0)
                    return;
            Expand();
            bits[ptr - 1] = 1;
        }
        private void DecrementRaw()
        {
            for (var d = 0; d < ptr; d++)
                if (--bits[d] != workType.MaxValue)
                    break;
            ShrinkPtr();
        }
        public void Increment()
        {
            if (sign < 0)
            {
                DecrementRaw();
                if (ptr == 0)
                    sign = 0;
            }
            else
            {
                IncrementRaw();
                sign = 1;
            }
        }
        public void Decrement()
        {
            if (sign > 0)
            {
                DecrementRaw();
                if (ptr == 0)
                    sign = 0;
            }
            else
            {
                IncrementRaw();
                sign = -1;
            }
        }
        #endregion
        #region Operations
        public static BigInteger Pow(BigInteger value, int exponent)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
        public static double Log10(BigInteger value)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
        public static BigInteger IntSqrt(BigInteger value)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }

        public static BigInteger operator +(BigInteger l, BigInteger r)
        {
            var res = new BigInteger(l);
            res.Add(r);
            return res;
        }
        public static BigInteger operator -(BigInteger l, BigInteger r)
        {
            var res = new BigInteger(l);
            res.Substract(r);
            return res;
        }
        public static BigInteger operator *(BigInteger l, workType r)
        {
            var res = new BigInteger(l);
            res.Multiply(r);
            return res;
        }
        public static BigInteger operator *(BigInteger l, BigInteger r)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
        public static BigInteger operator /(BigInteger l, BigInteger r)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
        public static BigInteger operator /(BigInteger l, int r)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
        public static BigInteger operator %(BigInteger l, int r) =>
            //TODO: Implement
            throw new NotImplementedException();

        public static bool operator <(BigInteger l, BigInteger r) => l.CompareTo(r) < 0;
        public static bool operator >(BigInteger l, BigInteger r) => l.CompareTo(r) > 0;
        public static bool operator ==(BigInteger l, BigInteger r) => l.CompareTo(r) == 0;
        public static bool operator !=(BigInteger l, BigInteger r) => l.CompareTo(r) != 0;
        public static bool operator <(BigInteger l, int r) => l.CompareTo(r) < 0;
        public static bool operator >(BigInteger l, int r) => l.CompareTo(r) > 0;
        public static bool operator <=(BigInteger l, int r) => l.CompareTo(r) <= 0;
        public static bool operator >=(BigInteger l, int r) => l.CompareTo(r) >= 0;
        public static bool operator ==(BigInteger l, int r) => l.CompareTo(r) == 0;
        public static bool operator !=(BigInteger l, int r) => l.CompareTo(r) != 0;

        public static BigInteger operator ++(BigInteger op)
        {
            var res = new BigInteger(op);
            res.Increment();
            return res;
        }
        public static BigInteger operator --(BigInteger op)
        {
            var res = new BigInteger(op);
            res.Decrement();
            return res;
        }
        #endregion
        #region Comparisons

        public bool IsZero => sign == 0;
        public bool IsNegative => sign < 0;
        public bool IsPositive => sign > 0;
        public bool IsEven => ptr == 0 || (bits[0] & 1) == 0;

        private int AbsoluteCompare(BigInteger other)
        {
            if (ptr != other.ptr)
                return ptr - other.ptr;
            for (var cD = ptr; cD >= 0; cD--)
            {
                if (bits[cD] == other.bits[cD]) continue;
                if (bits[cD] < other.bits[cD])
                    return -1;
                return 1;
            }

            return 0;
        }
        public int CompareTo(BigInteger other)
        {
            if (sign != other.sign)
                return sign - other.sign;
            if (sign == 0)
                return 0;
            if (sign < 0)
                return -AbsoluteCompare(other);
            return AbsoluteCompare(other);
        }

        public int CompareTo(int other)
        {
            var os = Math.Sign(other);
            var o = (workType)Math.Abs(other);
            if (sign != os)
                return sign - os;
            if (sign == 0)
                return 0;
            if (ptr > 1)
            {
                if (sign > 0)
                    return 1;
                else
                    return -1;
            }
            else
            {
                if (bits[0] == 0)
                    return 0;
                if (sign > 0)
                {
                    if (bits[0] < o)
                        return -1;
                    else
                        return 1;
                }
                else
                {
                    if (bits[0] < o)
                        return 1;
                    else
                        return -1;
                }
            }
        }

        public bool Equals(BigInteger other) => CompareTo(other) == 0;

        public bool Equals(int other) => CompareTo(other) == 0;

        public string ToString(string format, IFormatProvider formatProvider)
        {
            //TODO: Implement
            throw new NotImplementedException();
        }

        public override bool Equals(object obj)
        {
            if (obj is BigInteger bigInteger)
                return Equals(bigInteger);
            else if (obj is Int32 int32)
                return Equals(int32);
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(sign, bits, ptr);
        }

        public override string ToString()
        {
            //TODO: Implement
            return base.ToString();
        }
        #endregion
        #region TypeTransformations
        public static implicit operator BigInteger(int obj) => new BigInteger(obj);
        #endregion

    }
}