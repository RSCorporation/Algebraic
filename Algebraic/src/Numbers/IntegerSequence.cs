using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Algebraic.Numbers.Integers;

namespace Algebraic.Numbers
{
    public static partial class IntegerSequence
    {
        private const int SequencesCount = 1000000;

        public const int ZeroSequence = 4;
        public const int CharasteristicFunctionOfZero = 7;
        public const int AllOnesSequence = 12;
        public const int PositiveIntegers = 27;
        public const int InitialDigit = 30;
        public const int LucasNumbers = 32;
        // ReSharper disable InconsistentNaming
        public const int Period2_12 = 34;
        public const int Period2_01 = 35;
        // ReSharper restore InconsistentNaming
        public const int Nonsquares = 37;
        public const int TwiceCharasteristicFunctionOfZero = 38;
        public const int FibonacciNumbers = 45;

        private static readonly Func<IEnumerable<BigInteger>>[] sequences =
            new Func<IEnumerable<BigInteger>>[SequencesCount];

        private static readonly Func<BigInteger, BigInteger>[] elementAt =
            new Func<BigInteger, BigInteger>[SequencesCount];

        static IntegerSequence()
        {
            sequences[4] = A000004.Sequence;
            sequences[7] = A000007.Sequence;
            sequences[12] = A000012.Sequence;
            sequences[27] = A000027.Sequence;
            sequences[30] = A000030.Sequence;
            sequences[32] = A000032.Sequence;
            sequences[34] = A000034.Sequence;
            sequences[35] = A000035.Sequence;
            sequences[37] = A000037.Sequence;
            sequences[38] = A000038.Sequence;
            sequences[45] = A000045.Sequence;

            elementAt[4] = A000004.GetElement;
            elementAt[7] = A000007.GetElement;
            elementAt[12] = A000012.GetElement;
            elementAt[27] = A000027.GetElement;
            elementAt[30] = A000030.GetElement;
            elementAt[32] = A000032.GetElement;
            elementAt[34] = A000034.GetElement;
            elementAt[35] = A000035.GetElement;
            elementAt[37] = A000037.GetElement;
            elementAt[38] = A000038.GetElement;
            elementAt[45] = A000045.GetElement;
        }

        [ContractArgumentValidator]
        // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
        private static void ValidateSequence(int sequence)
        {
            if (sequence >= SequencesCount || sequence < 0 || sequences[sequence] == null ||
                sequences[sequence].GetInvocationList().Length != 1)
                throw new ArgumentException("Sequence doesn't exist");
            Contract.EndContractBlock();
        }

        [Pure]
        public static IEnumerable<BigInteger> Enumerate(int sequence)
        {
            ValidateSequence(sequence);
            return sequences[sequence]();
        }

        [Pure]
        public static BigInteger GetElement(int sequence, BigInteger idx)
        {
            ValidateSequence(sequence);
            Contract.Requires(idx >= 0);
            return elementAt[sequence](idx);
        }
    }
}