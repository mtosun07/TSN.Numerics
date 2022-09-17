using System.Numerics;

namespace TSN.Numerics
{
    partial struct Numeric
    {
        static Numeric()
        {
            _empty = new Numeric();
            _NaN = 0D / 0D;
            _minusOne = BigInteger.MinusOne;
            _zero = BigInteger.Zero;
            _one = BigInteger.One;
            _negativeInfinity = -1D / 0D;
            _positiveInfinity = 1D / 0D;
            _epsilon = 4.94065645841247E-324;
            _E = 2.7182818284590451;
            _PI = 3.1415926535897931;
            _tau = 6.2831853071795862;
        }


        private const double _d_DecimalMin = (double)decimal.MinValue;
        private const double _d_DecimalMax = (double)decimal.MaxValue;
        
        private static readonly Numeric _empty;
        private static readonly Numeric _minusOne;
        private static readonly Numeric _zero;
        private static readonly Numeric _one;
        private static readonly Numeric _NaN;
        private static readonly Numeric _negativeInfinity;
        private static readonly Numeric _positiveInfinity;
        private static readonly Numeric _epsilon;
        private static readonly Numeric _E;
        private static readonly Numeric _PI;
        private static readonly Numeric _tau;

        public static Numeric Empty => _empty;
        public static Numeric MinusOne => _minusOne;
        public static Numeric Zero => _zero;
        public static Numeric One => _one;
        public static Numeric NaN => _NaN;
        public static Numeric NegativeInfinity => _negativeInfinity;
        public static Numeric PositiveInfinity => _positiveInfinity;
        public static Numeric Epsilon => _epsilon;
        public static Numeric E => _E;
        public static Numeric PI => _PI;
        public static Numeric Tau => _tau;
    }
}