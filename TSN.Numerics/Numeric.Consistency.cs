using System.Numerics;

namespace TSN.Numerics
{
    partial struct Numeric
    {
        static Numeric()
        {
            _empty = new Numeric();
            _NaN = _d_NaN;
            _minusOne = BigInteger.MinusOne;
            _zero = BigInteger.Zero;
            _one = BigInteger.One;
            _negativeInfinity = _d_NegativeInfinity;
            _positiveInfinity = _d_PositiveInfinity;
            _epsilon = _d_Epsilon;
            _E = _d_E;
            _PI = _d_PI;
            _tau = _d_Tau;
        }


        private const double _d_DecimalMin = (double)decimal.MinValue;
        private const double _d_DecimalMax = (double)decimal.MaxValue;
        private const double _d_NegativeInfinity = -1D / 0D;
        private const double _d_PositiveInfinity = 1D / 0D;
        private const double _d_NaN = 0D / 0D;
        private const double _d_Epsilon = 4.94065645841247E-324;
        private const double _d_E = 2.7182818284590451;
        private const double _d_PI = 3.1415926535897931;
        private const double _d_Tau = 6.2831853071795862;
        
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