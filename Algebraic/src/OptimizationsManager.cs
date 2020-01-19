using System;
using System.Collections.Generic;
using System.Text;

namespace Algebraic
{
    /// <summary>
    /// Manages optimization parameters. Should be used only if it is neccesery to change something.
    /// </summary>
    public static class OptimizationsManager
    {
        public static readonly bool IsLittleEndianSystem = unchecked((byte)261) == 5;
        #region Collections BigInteger
        /// <summary>
        /// Default internal size of Big Integer allocation. Do not reduce (can cause runtime errors in some constructors)
        /// </summary>
        public static int BigIntegerDefaultAllocation = 4;
        public static int BigIntegerReallocationCoefficient = 2;

        #endregion
    }
}
