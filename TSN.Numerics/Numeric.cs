using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.Serialization;

namespace TSN.Numerics
{
    [Serializable]
    public partial struct Numeric : ISerializable, ICloneable, IEquatable<Numeric>, IComparable, IComparable<Numeric>, IFormattable
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
        private Numeric(SerializationInfo info, StreamingContext context)
        {
            _i = (BigInteger?)info.GetValue(nameof(_i), typeof(BigInteger?));
            _d = (double?)info.GetValue(nameof(_d), typeof(double?));
            _m = (decimal?)info.GetValue(nameof(_m), typeof(decimal?));
            _c = (Complex?)info.GetValue(nameof(_c), typeof(Complex?));
        }
        public Numeric(sbyte value) : this((BigInteger)value) { }
        public Numeric(byte value) : this((BigInteger)value) { }
        public Numeric(short value) : this((BigInteger)value) { }
        public Numeric(ushort value) : this((BigInteger)value) { }
        public Numeric(int value) : this((BigInteger)value) { }
        public Numeric(uint value) : this((BigInteger)value) { }
        public Numeric(long value) : this((BigInteger)value) { }
        public Numeric(ulong value) : this((BigInteger)value) { }
        public Numeric(BigInteger value)
        {
            _i = value;
            _d = null;
            _m = null;
            _c = null;
        }
        public Numeric(float value) : this((double)value) { }
        public Numeric(double value)
        {
            double x;
            _m = null;
            _c = null;
            if (!double.IsNaN(value) && !double.IsInfinity(value) && value == (x = Math.Truncate(value)))
            {
                _i = (BigInteger)x;
                _d = null;
            }
            else
            {
                _i = null;
                _d = value;
            }
        }
        public Numeric(decimal value)
        {
            _d = null;
            _c = null;
            var x = Math.Truncate(value);
            if (value == x)
            {
                _i = (BigInteger)x;
                _m = null;
            }
            else
            {
                _i = null;
                _m = value;
            }
        }
        public Numeric(Complex value)
        {
            _m = null;
            if (value.Imaginary == 0D)
            {
                _c = null;
                var x = Math.Truncate(value.Real);
                if (value.Real == x)
                {
                    _i = (BigInteger)x;
                    _d = null;
                }
                else
                {
                    _i = null;
                    _d = value.Real;
                }
            }
            else
            {
                _i = null;
                _d = null;
                _c = value;
            }
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

        private readonly BigInteger? _i;
        private readonly double? _d;
        private readonly decimal? _m;
        private readonly Complex? _c;

        public bool IsInteger => _i.HasValue;
        public bool IsDouble => _d.HasValue;
        public bool IsDecimal => _m.HasValue;
        public bool IsComplex => _c.HasValue;

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



        public bool IsEmpty() => !_i.HasValue && !_d.HasValue && !_m.HasValue && !_c.HasValue;
        public bool IsFinite() => _i.HasValue || _m.HasValue || (_d.HasValue && IsFinite(_d.Value)) || (_c.HasValue && IsFinite(_c.Value));
        public bool IsInfinity() => (_d.HasValue && double.IsInfinity(_d.Value)) || (_c.HasValue && IsInfinity(_c.Value));
        public bool IsNaN() => _d.HasValue ? double.IsNaN(_d.Value) : (_c.HasValue ? IsNaN(_c.Value) : (!_i.HasValue && !_m.HasValue));
        public bool IsZero() => _i.HasValue && _i.Value.Sign == 0;
        public bool TryGetSign(out int sign)
        {
            if (!IsEmpty() && !IsNaN())
            {
                sign = _i?.Sign ?? CompareTo(_zero);
                return true;
            }
            sign = 0;
            return false;
        }

        public override string ToString() => _i?.ToString() ?? _d?.ToString() ?? _m?.ToString() ?? _c?.ToString();
        public override int GetHashCode() => _i?.GetHashCode() ?? _d?.GetHashCode() ?? _m?.GetHashCode() ?? _c?.GetHashCode() ?? 0;
        public override bool Equals(object obj) => obj is Numeric n && Equals(n);

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(_i), _i, typeof(BigInteger?));
            info.AddValue(nameof(_d), _d, typeof(double?));
            info.AddValue(nameof(_m), _m, typeof(decimal?));
            info.AddValue(nameof(_c), _c, typeof(Complex?));
        }
        public object Clone() => +this;
        public bool Equals(Numeric other) => CompareTo(other) == 0;

        public int CompareTo(object obj) => CompareTo(obj is Numeric n ? n : throw new ArgumentNullException(nameof(obj)));
        public int CompareTo(Numeric other)
        {
            if (_i.HasValue)
            {
                if (other._i.HasValue)
                    return _i.Value.CompareTo(other._i.Value);
                else if (other._d.HasValue)
                    return Compare(_i.Value, other._d.Value);
                else if (other._m.HasValue)
                    return Compare(_i.Value, other._m.Value);
                else if (other._c.HasValue)
                    return Compare(_i.Value, other._c.Value);
            }
            else if (_d.HasValue)
            {
                if (other._i.HasValue)
                    return Compare(_d.Value, other._i.Value);
                else if (other._d.HasValue)
                    return _d.Value.CompareTo(other._d.Value);
                else if (other._m.HasValue)
                    return Compare(_d.Value, other._m.Value);
                else if (other._c.HasValue)
                    return Compare(_d.Value, other._c.Value);
            }
            else if (_m.HasValue)
            {
                if (other._i.HasValue)
                    return Compare(_m.Value, other._i.Value);
                else if (other._d.HasValue)
                    return Compare(_m.Value, other._d.Value);
                else if (other._m.HasValue)
                    return _m.Value.CompareTo(other._m.Value);
                else if (other._c.HasValue)
                    return Compare(_m.Value, other._c.Value);
            }
            else if (_c.HasValue)
            {
                if (other._i.HasValue)
                    return Compare(_c.Value, other._i.Value);
                if (other._d.HasValue)
                    return Compare(_c.Value, other._d.Value);
                if (other._m.HasValue)
                    return Compare(_c.Value, other._m.Value);
                if (other._c.HasValue)
                    return Compare(_c.Value, other._c.Value);
            }
            else if (other.IsEmpty())
                return 0;
            throw new ArithmeticException();
        }
        public string ToString(string format, IFormatProvider formatProvider) => _i?.ToString(format, formatProvider) ?? _d?.ToString(format, formatProvider) ?? _m?.ToString(format, formatProvider) ?? _c?.ToString(format, formatProvider) ?? string.Empty;

        public bool TryConvertToInteger(out BigInteger value)
        {
            if (_i.HasValue)
            {
                value = _i.Value;
                return true;
            }
            value = default;
            return false;
        }
        public bool TryConvertToDouble(out double value)
        {
            if (_d.HasValue)
            {
                value = _d.Value;
                return true;
            }
            else if (_m.HasValue)
            {
                try
                {
                    value = checked((double)_m.Value);
                    return true;
                }
                catch (OverflowException) { }
            }
            value = default;
            return false;
        }
        public bool TryConvertToDecimal(out decimal value)
        {
            if (_m.HasValue)
            {
                value = _m.Value;
                return true;
            }
            else if (_d.HasValue)
            {
                try
                {
                    value = checked((decimal)_d.Value);
                    return true;
                }
                catch (OverflowException) { }
            }
            value = default;
            return false;
        }
        public bool TryConvertToComplex(out Complex value)
        {
            if (_c.HasValue)
            {
                value = _c.Value;
                return true;
            }
            try
            {
                value = new Complex(checked((double)this), 0D);
                return true;
            }
            catch (OverflowException)
            {
                value = default;
                return false;
            }
        }
        public static Numeric Parse(string value) => BigInteger.TryParse(value, out var i) ? new Numeric(i) : (double.TryParse(value, out var d) ? new Numeric(d) : decimal.TryParse(value, out var m) ? new Numeric(m) : throw new FormatException());
        public static Numeric Parse(string value, NumberStyles style) => BigInteger.TryParse(value, style, null, out var i) ? new Numeric(i) : (double.TryParse(value, style, null, out var d) ? new Numeric(d) : decimal.TryParse(value, style, null, out var m) ? new Numeric(m) : throw new FormatException());
        public static Numeric Parse(string value, IFormatProvider provider) => BigInteger.TryParse(value, NumberStyles.Integer, provider, out var i) ? new Numeric(i) : (double.TryParse(value, NumberStyles.Number, provider, out var d) ? new Numeric(d) : decimal.TryParse(value, NumberStyles.Number, provider, out var m) ? new Numeric(m) : throw new FormatException());
        public static Numeric Parse(string value, NumberStyles style, IFormatProvider provider) => BigInteger.TryParse(value, style, provider, out var i) ? new Numeric(i) : (double.TryParse(value, style, provider, out var d) ? new Numeric(d) : decimal.TryParse(value, style, provider, out var m) ? new Numeric(m) : throw new FormatException());
        public static bool TryParse(string value, out Numeric result)
        {
            if (BigInteger.TryParse(value, out var i))
            {
                result = new Numeric(i);
                return true;
            }
            if (double.TryParse(value, out var d))
            {
                result = new Numeric(d);
                return true;
            }
            if (decimal.TryParse(value, out var m))
            {
                result = new Numeric(m);
                return true;
            }
            result = _empty;
            return false;
        }
        public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out Numeric result)
        {
            if (BigInteger.TryParse(value, style, provider, out var i))
            {
                result = new Numeric(i);
                return true;
            }
            if (double.TryParse(value, style, provider, out var d))
            {
                result = new Numeric(d);
                return true;
            }
            if (decimal.TryParse(value, style, provider, out var m))
            {
                result = new Numeric(m);
                return true;
            }
            result = _empty;
            return false;
        }

        private static bool IsFinite(double d) => !double.IsNaN(d) && !double.IsInfinity(d);
        private static bool IsFinite(Complex c) => IsFinite(c.Real) && IsFinite(c.Imaginary);
        private static bool IsInfinity(Complex c) => double.IsInfinity(c.Real) || double.IsInfinity(c.Imaginary);
        private static bool IsNaN(Complex value) => !IsInfinity(value) && !IsFinite(value);
        private static int Compare(BigInteger left, double right)
        {
            int s1, s2, r;
            BigInteger tmp;
            return double.IsNaN(right) ? throw new ArithmeticException() : (double.IsNegativeInfinity(right) ? 1 : (double.IsPositiveInfinity(right) ? -1 : ((s1 = left.Sign) != (s2 = Math.Sign(right)) ? s1.CompareTo(s2) : ((double)(tmp = (BigInteger)right) == right ? s1.CompareTo(tmp) : (s1 > 0 ? ((r = left.CompareTo((BigInteger)Math.Ceiling(right))) == 0D ? 1 : r) : ((r = left.CompareTo((BigInteger)Math.Floor(right))) == 0D ? -1 : r))))));
        }
        private static int Compare(double left, BigInteger right) => -Compare(right, left);
        private static int Compare(BigInteger left, decimal right)
        {
            int s1, s2, r;
            BigInteger tmp;
            return (s1 = left.Sign) != (s2 = Math.Sign(right)) ? s1.CompareTo(s2) : ((decimal)(tmp = (BigInteger)right) == right ? s1.CompareTo(tmp) : (s1 > 0 ? ((r = left.CompareTo((BigInteger)Math.Ceiling(right))) == 0M ? 1 : r) : ((r = left.CompareTo((BigInteger)Math.Floor(right))) == 0M ? -1 : r)));
        }
        private static int Compare(decimal left, BigInteger right) => -Compare(right, left);
        private static int Compare(BigInteger left, Complex right) => right.Imaginary != 0D ? throw new ArithmeticException() : Compare(left, right.Real);
        private static int Compare(Complex left, BigInteger right) => -Compare(right, left);
        private static int Compare(double left, decimal right)
        {
            decimal m;
            double d;
            return double.IsNaN(left) ? throw new ArithmeticException() : (double.IsNegativeInfinity(left) ? -1 : (double.IsPositiveInfinity(left) ? 1 : ((double)(m = (decimal)left) == left ? m.CompareTo(right) : ((decimal)(d = (double)right) == right ? left.CompareTo(d) : (left < _d_DecimalMin ? -1 : (left > _d_DecimalMax ? 1 : m.CompareTo(right)))))));
        }
        private static int Compare(decimal left, double right) => -Compare(right, left);
        private static int Compare(double left, Complex right) => right.Imaginary != 0D ? throw new ArithmeticException() : left.CompareTo(right.Real);
        private static int Compare(Complex left, double right) => -Compare(right, left);
        private static int Compare(decimal left, Complex right) => right.Imaginary != 0D ? throw new ArithmeticException() : Compare(left, right.Real);
        private static int Compare(Complex left, decimal right) => -Compare(right, left);
        private static int Compare(Complex left, Complex right) => right.Imaginary != 0D || left.Imaginary != 0D ? throw new ArithmeticException() : left.Real.CompareTo(right.Real);

        public static Numeric Abs(Numeric value) => value._i.HasValue ? BigInteger.Abs(value._i.Value) : (value._d.HasValue ? Math.Abs(value._d.Value) : (value._m.HasValue ? Math.Abs(value._m.Value) : (value._c.HasValue ? Complex.Abs(value._c.Value) : _empty)));
        public static Numeric Negate(Numeric value) => -value;
        public static Numeric Inverse(Numeric value) => value._m.HasValue ? new Numeric(1M / value._m.Value) : (1D / value);
        public static Numeric Add(Numeric left, Numeric right) => left + right;
        public static Numeric Subtract(Numeric left, Numeric right) => left - right;
        public static Numeric Multiply(Numeric left, Numeric right) => left * right;
        public static Numeric Divide(Numeric dividend, Numeric divisor) => dividend / divisor;
        public static Numeric Remainder(Numeric dividend, Numeric divisor) => dividend % divisor;
        public static Numeric DivRem(Numeric dividend, Numeric divisor, out Numeric remainder)
        {
            if (dividend._i.HasValue && divisor._i.HasValue)
            {
                var division = BigInteger.DivRem(dividend._i.Value, divisor._i.Value, out var rem);
                remainder = rem;
                return division;
            }
            remainder = dividend % divisor;
            return dividend / divisor;
        }
        public static Numeric Pow(Numeric value, Numeric exponent)
        {
            if (value.IsEmpty() || value.IsNaN())
                return value;
            if (exponent._c.HasValue)
                return Complex.Pow((double)value, exponent._c.Value);
            else if (exponent.IsZero())
                return _one;
            else if (value.IsZero())
                return _zero;
            else if (exponent == _one)
                return value;
            else if (value._d.HasValue && double.IsInfinity(value._d.Value))
                return exponent._i.HasValue ? (exponent._i.Value.Sign < 0 ? _zero : double.PositiveInfinity) : Math.Pow(value._d.Value, (double)exponent);
            else if (exponent._d.HasValue && double.IsInfinity(exponent._d.Value))
                return value._i.HasValue ? (double.IsPositiveInfinity(exponent._d.Value) ? double.PositiveInfinity : _zero) : (value._c.HasValue ? Complex.Pow(value._c.Value, exponent._d.Value) : Math.Pow(value._d ?? ((double)value._m.Value), exponent._d.Value));
            else if (value == _one)
                return _one;
            else if (exponent._i.HasValue)
            {
                if (value._i.HasValue)
                    try
                    {
                        return exponent._i.Value.Sign < 0 ? Inverse(BigInteger.Pow(value._i.Value, checked((int)Abs(exponent)._i.Value))) : BigInteger.Pow(value._i.Value, checked((int)exponent._i.Value));
                    }
                    catch (OverflowException) { }
                return Math.Pow((double)value, (double)exponent);
            }
            else if (value < _zero)
            {
                if (exponent < _zero)
                    return Inverse(Pow(value, -exponent));
                if ((exponent % .5).IsZero())
                {   // The result has to be a Complex number.
                    /*
                     * In this case denominator of the exponent has to be an even number.
                     * So, sign of the result's imaginer part is determined by
                     * whether the exponent is even or not; which makes the imaginer
                     * part positive if the exponent is even.
                     * 
                     */

                    var powAbs = Pow(Abs(value), exponent);
                    return new Complex(0, (double)((Floor(exponent) % 2).IsZero() ? powAbs : -powAbs));
                }
                else
                {   // The result has to be a Real number.
                    /* 
                     * In this case denominator of the exponent has to be an odd number.
                     * So, resulting algorithm must be as demonstrated below:
                     * 
                     * if (numerator of the exponent is even) => result = powAbs
                     * else => result = -1 * powAbs
                     * 
                     * However, with C# and .NET, we can't exactly decide what
                     * neither the numerator nor the denominator is,
                     * for every exponent ∈ R.
                     * 
                     * Complex.Pow(base, exponent) results an 'approximation' and it most likely
                     * returns a Complex number for this case as well. Also, even for (-1)^(1/2),
                     * it's approximation does not have enough precise to be able to return "i".
                     */

                    // TODO: Discover or invent an appropriate algorithm!
                    return Complex.Pow((double)value, (double)exponent);
                }
            }
            return value._c.HasValue ? Complex.Pow(value._c.Value, exponent._d ?? (double)exponent._m.Value) : Math.Pow(value._i.HasValue ? ((double)value._i.Value) : (value._d ?? ((double)value._m.Value)), exponent._d ?? (double)exponent._m.Value);
        }
        public static Numeric ModPow(Numeric value, Numeric exponent, Numeric modulus) => Pow(value, exponent) % modulus;
        public static Numeric Sqrt(Numeric value) => Pow(value, .5);
        public static Numeric Log(Numeric value) => value.IsFinite() && value < _zero ? Complex.Log((Complex)value) : (value._i.HasValue ? BigInteger.Log(value._i.Value) : (value._c.HasValue ? Complex.Log(value._c.Value) : (value._d.HasValue ? Math.Log(value._d.Value) : (value._m.HasValue ? Math.Log((double)value._m.Value) : _empty))));
        public static Numeric Log(Numeric value, Numeric baseValue) => value.IsFinite() && value < _zero ? Complex.Log((Complex)value, (double)baseValue) : (value._i.HasValue ? BigInteger.Log(value._i.Value, (double)baseValue) : (value._c.HasValue ? Complex.Log(value._c.Value, (double)baseValue) : (value._d.HasValue ? Math.Log(value._d.Value, (double)baseValue) : (value._m.HasValue ? Math.Log((double)value._m.Value, (double)baseValue) : _empty))));
        public static Numeric Log10(Numeric value) => value.IsFinite() && value < _zero ? Complex.Log10((Complex)value) : (value._i.HasValue ? BigInteger.Log10(value._i.Value) : (value._c.HasValue ? Complex.Log10(value._c.Value) : (value._d.HasValue ? Math.Log10(value._d.Value) : (value._m.HasValue ? Math.Log10((double)value._m.Value) : _empty))));
        public static Numeric Exp(Numeric value) => value._i.HasValue ? Math.Exp((double)value._i.Value) : (value._c.HasValue ? Complex.Exp(value._c.Value) : (value._d.HasValue ? Math.Exp(value._d.Value) : (value._m.HasValue ? Math.Exp((double)value._m.Value) : _empty)));
        public static Numeric Min(Numeric left, Numeric right) => left < right ? left : right;
        public static Numeric Max(Numeric left, Numeric right) => left > right ? left : right;
        public static Numeric Round(Numeric value, int digits = 0, MidpointRounding mode = MidpointRounding.AwayFromZero) => value._i.HasValue || value._c.HasValue || !value.IsFinite() ? value : (value._d.HasValue ? new Numeric(Math.Round(value._d.Value, digits, mode)) : new Numeric(Math.Round(value._m.Value, digits, mode)));
        public static Numeric Ceiling(Numeric value) => value._i.HasValue || value._c.HasValue || !value.IsFinite() ? value : (value._d.HasValue ? new Numeric(Math.Ceiling(value._d.Value)) : new Numeric(Math.Ceiling(value._m.Value)));
        public static Numeric Floor(Numeric value) => value._i.HasValue || value._c.HasValue || !value.IsFinite() ? value : (value._d.HasValue ? new Numeric(Math.Floor(value._d.Value)) : new Numeric(Math.Floor(value._m.Value)));
        public static Numeric Truncate(Numeric value) => value._i.HasValue || value._c.HasValue || !value.IsFinite() ? value : (value._d.HasValue ? new Numeric(Math.Truncate(value._d.Value)) : new Numeric(Math.Truncate(value._m.Value)));



        public static explicit operator sbyte(Numeric n) => n._i.HasValue ? (sbyte)n._i.Value : (n._d.HasValue ? (sbyte)n._d.Value : (n._m.HasValue ? (sbyte)n._m.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (sbyte)n._c.Value.Real : throw new ArithmeticException()) : new sbyte())));
        public static explicit operator byte(Numeric n) => n._i.HasValue ? (byte)n._i.Value : (n._d.HasValue ? (byte)n._d.Value : (n._m.HasValue ? (byte)n._m.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (byte)n._c.Value.Real : throw new ArithmeticException()) : new byte())));
        public static explicit operator short(Numeric n) => n._i.HasValue ? (short)n._i.Value : (n._d.HasValue ? (short)n._d.Value : (n._m.HasValue ? (short)n._m.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (short)n._c.Value.Real : throw new ArithmeticException()) : new short())));
        public static explicit operator ushort(Numeric n) => n._i.HasValue ? (ushort)n._i.Value : (n._d.HasValue ? (ushort)n._d.Value : (n._m.HasValue ? (ushort)n._m.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (ushort)n._c.Value.Real : throw new ArithmeticException()) : new ushort())));
        public static explicit operator int(Numeric n) => n._i.HasValue ? (int)n._i.Value : (n._d.HasValue ? (int)n._d.Value : (n._m.HasValue ? (int)n._m.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (int)n._c.Value.Real : throw new ArithmeticException()) : 0)));
        public static explicit operator uint(Numeric n) => n._i.HasValue ? (uint)n._i.Value : (n._d.HasValue ? (uint)n._d.Value : (n._m.HasValue ? (uint)n._m.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (uint)n._c.Value.Real : throw new ArithmeticException()) : 0U)));
        public static explicit operator long(Numeric n) => n._i.HasValue ? (long)n._i.Value : (n._d.HasValue ? (long)n._d.Value : (n._m.HasValue ? (long)n._m.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (long)n._c.Value.Real : throw new ArithmeticException()) : 0L)));
        public static explicit operator ulong(Numeric n) => n._i.HasValue ? (ulong)n._i.Value : (n._d.HasValue ? (ulong)n._d.Value : (n._m.HasValue ? (ulong)n._m.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (ulong)n._c.Value.Real : throw new ArithmeticException()) : 0UL)));
        public static explicit operator BigInteger(Numeric n) => n._i ?? (n._d.HasValue ? (BigInteger)n._d.Value : (n._m.HasValue ? (BigInteger)n._m.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (BigInteger)n._c.Value.Real : throw new ArithmeticException()) : BigInteger.Zero)));
        public static explicit operator float(Numeric n) => n._d.HasValue ? (float)n._d.Value : (n._m.HasValue ? (float)n._m.Value : (n._i.HasValue ? (float)n._i.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (float)n._c.Value.Real : throw new ArithmeticException()) : 0F)));
        public static explicit operator double(Numeric n) => n._d ?? (n._m.HasValue ? (double)n._m.Value : (n._i.HasValue ? (double)n._i.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? n._c.Value.Real : throw new ArithmeticException()) : 0D)));
        public static explicit operator decimal(Numeric n) => n._m ?? (n._d.HasValue ? (decimal)n._d.Value : (n._i.HasValue ? (decimal)n._i.Value : (n._c.HasValue ? (n._c.Value.Imaginary == 0D ? (decimal)n._c.Value.Real : throw new ArithmeticException()) : 0M)));
        public static explicit operator Complex(Numeric n) => n._c ?? (n._d.HasValue ? new Complex(n._d.Value, 0D) : (n._m.HasValue ? new Complex((double)n._m.Value, 0D) : (n._i.HasValue ? new Complex((double)n._i.Value, 0D) : new Complex())));
        public static implicit operator Numeric(sbyte o) => new Numeric(o);
        public static implicit operator Numeric(byte o) => new Numeric(o);
        public static implicit operator Numeric(short o) => new Numeric(o);
        public static implicit operator Numeric(ushort o) => new Numeric(o);
        public static implicit operator Numeric(int o) => new Numeric(o);
        public static implicit operator Numeric(uint o) => new Numeric(o);
        public static implicit operator Numeric(long o) => new Numeric(o);
        public static implicit operator Numeric(ulong o) => new Numeric(o);
        public static implicit operator Numeric(BigInteger o) => new Numeric(o);
        public static implicit operator Numeric(float o) => new Numeric(o);
        public static implicit operator Numeric(double o) => new Numeric(o);
        public static implicit operator Numeric(decimal o) => new Numeric(o);
        public static implicit operator Numeric(Complex o) => new Numeric(o);

        public static bool operator ==(Numeric left, Numeric right) => left.CompareTo(right) == 0;
        public static bool operator !=(Numeric left, Numeric right) => left.CompareTo(right) != 0;
        public static bool operator <(Numeric left, Numeric right) => left.CompareTo(right) < 0;
        public static bool operator >(Numeric left, Numeric right) => left.CompareTo(right) > 0;
        public static bool operator <=(Numeric left, Numeric right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Numeric left, Numeric right) => left.CompareTo(right) >= 0;

        public static Numeric operator &(Numeric left, Numeric right) => (left._i.HasValue && right._i.HasValue) ? left._i.Value & right._i.Value : throw new ArithmeticException();
        public static Numeric operator |(Numeric left, Numeric right) => (left._i.HasValue && right._i.HasValue) ? left._i.Value | right._i.Value : throw new ArithmeticException();
        public static Numeric operator ^(Numeric left, Numeric right) => (left._i.HasValue && right._i.HasValue) ? left._i.Value ^ right._i.Value : throw new ArithmeticException();
        public static Numeric operator <<(Numeric value, int shift) => (value._i ?? throw new ArithmeticException()) << shift;
        public static Numeric operator >>(Numeric value, int shift) => (value._i ?? throw new ArithmeticException()) >> shift;
        public static Numeric operator ~(Numeric value) => value._i.HasValue ? (~value._i.Value) : throw new ArithmeticException();
        public static Numeric operator -(Numeric value) => value._i.HasValue ? new Numeric(-value._i.Value) : (value._d.HasValue ? new Numeric(-value._d.Value) : (value._m.HasValue ? new Numeric(-value._m.Value) : value._c.HasValue ? new Numeric(-value._c.Value) : _empty));
        public static Numeric operator +(Numeric value) => value._i.HasValue ? new Numeric(value._i.Value) : (value._d.HasValue ? new Numeric(value._d.Value) : (value._m.HasValue ? new Numeric(value._m.Value) : value._c.HasValue ? new Numeric(value._c.Value) : _empty));
        public static Numeric operator ++(Numeric value) => value._i.HasValue ? new Numeric(value._i.Value + BigInteger.One) : (value._d.HasValue ? new Numeric(value._d.Value + 1D) : (value._m.HasValue ? new Numeric(value._m.Value + 1M) : value._c.HasValue ? new Numeric(value._c.Value + Complex.One) : new Numeric(BigInteger.One)));
        public static Numeric operator --(Numeric value) => value._i.HasValue ? new Numeric(value._i.Value - BigInteger.One) : (value._d.HasValue ? new Numeric(value._d.Value - 1D) : (value._m.HasValue ? new Numeric(value._m.Value - 1M) : value._c.HasValue ? new Numeric(value._c.Value - Complex.One) : new Numeric(BigInteger.MinusOne)));
        public static Numeric operator +(Numeric left, Numeric right)
        {
            if (left._i.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._i.Value + right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric((double)left._i.Value + right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric((decimal)left._i.Value + right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric((Complex)left._i.Value + right._c.Value);
            }
            else if (left._d.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._d.Value + (double)right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric(left._d.Value + right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric((decimal)left._d.Value + right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric(left._d.Value + right._c.Value);
            }
            else if (left._m.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._m.Value + (decimal)right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric(left._m.Value + (decimal)right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric(left._m.Value + right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric((Complex)left._m.Value + right._c.Value);
            }
            else if (left._c.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._c.Value + (Complex)right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric(left._c.Value + right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric(left._c.Value + (Complex)right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric(left._c.Value + right._c.Value);
            }
            throw new ArithmeticException();
        }
        public static Numeric operator -(Numeric left, Numeric right)
        {
            if (left._i.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._i.Value - right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric((double)left._i.Value - right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric((decimal)left._i.Value - right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric((Complex)left._i.Value - right._c.Value);
            }
            else if (left._d.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._d.Value - (double)right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric(left._d.Value - right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric((decimal)left._d.Value - right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric(left._d.Value - right._c.Value);
            }
            else if (left._m.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._m.Value - (decimal)right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric(left._m.Value - (decimal)right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric(left._m.Value - right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric((Complex)left._m.Value - right._c.Value);
            }
            else if (left._c.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._c.Value - (Complex)right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric(left._c.Value - right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric(left._c.Value - (Complex)right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric(left._c.Value - right._c.Value);
            }
            throw new ArithmeticException();
        }
        public static Numeric operator *(Numeric left, Numeric right)
        {
            if (left._i.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._i.Value * right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric((double)left._i.Value * right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric((decimal)left._i.Value * right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric((Complex)left._i.Value * right._c.Value);
            }
            else if (left._d.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._d.Value * (double)right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric(left._d.Value * right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric((decimal)left._d.Value * right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric(left._d.Value * right._c.Value);
            }
            else if (left._m.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._m.Value * (decimal)right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric(left._m.Value * (decimal)right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric(left._m.Value * right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric((Complex)left._m.Value * right._c.Value);
            }
            else if (left._c.HasValue)
            {
                if (right._i.HasValue)
                    return new Numeric(left._c.Value * (Complex)right._i.Value);
                else if (right._d.HasValue)
                    return new Numeric(left._c.Value * right._d.Value);
                else if (right._m.HasValue)
                    return new Numeric(left._c.Value * (Complex)right._m.Value);
                else if (right._c.HasValue)
                    return new Numeric(left._c.Value * right._c.Value);
            }
            throw new ArithmeticException();
        }
        public static Numeric operator /(Numeric dividend, Numeric divisor)
        {
            if (dividend._i.HasValue)
            {
                if (divisor._d.HasValue || (divisor._i.HasValue && divisor._i.Value.Sign == 0))
                    return new Numeric((double)dividend._i.Value / (divisor._d ?? (double)divisor._i.Value));
                else if (divisor._i.HasValue)
                {
                    var div = BigInteger.DivRem(dividend._i.Value, divisor._i.Value, out var rem);
                    if (rem.Sign == 0)
                        return new Numeric(div);
                    try
                    {
                        var x = checked((double)dividend._i);
                        var y = checked((double)divisor._i);
                        return x / y;
                    }
                    catch (OverflowException)
                    {
                        return (decimal)dividend._i / (decimal)divisor._i;
                    }
                }
                else if (divisor._m.HasValue)
                    return new Numeric((decimal)dividend._i.Value / divisor._m.Value);
                else if (divisor._c.HasValue)
                    return new Numeric((double)dividend._i.Value / divisor._c.Value);
            }
            else if (dividend._d.HasValue)
            {
                if (divisor._i.HasValue)
                    return new Numeric(dividend._d.Value / (double)divisor._i.Value);
                else if (divisor._d.HasValue)
                    return new Numeric(dividend._d.Value / divisor._d.Value);
                else if (divisor._m.HasValue)
                    return new Numeric((decimal)dividend._d.Value / divisor._m.Value);
                else if (divisor._c.HasValue)
                    return new Numeric(dividend._d.Value / divisor._c.Value);
            }
            else if (dividend._m.HasValue)
            {
                if (divisor._i.HasValue)
                    return new Numeric(dividend._m.Value / (decimal)divisor._i.Value);
                else if (divisor._d.HasValue)
                    return new Numeric(dividend._m.Value / (decimal)divisor._d.Value);
                else if (divisor._m.HasValue)
                    return new Numeric(dividend._m.Value / divisor._m.Value);
                else if (divisor._c.HasValue)
                    return new Numeric((Complex)dividend._m.Value / divisor._c.Value);
            }
            else if (dividend._c.HasValue)
            {
                if (divisor._i.HasValue)
                    return new Numeric(dividend._c.Value / (Complex)divisor._i.Value);
                else if (divisor._d.HasValue)
                    return new Numeric(dividend._c.Value / divisor._d.Value);
                else if (divisor._m.HasValue)
                    return new Numeric(dividend._c.Value / (Complex)divisor._m.Value);
                else if (divisor._c.HasValue)
                    return new Numeric(dividend._c.Value / divisor._c.Value);
            }
            throw new ArithmeticException();
        }
        public static Numeric operator %(Numeric dividend, Numeric divisor)
        {
            if (dividend._i.HasValue)
            {
                if (divisor._i.HasValue)
                    return new Numeric(dividend._i.Value % divisor._i.Value);
                else if (divisor._d.HasValue)
                    return new Numeric((double)dividend._i.Value % divisor._d.Value);
                else if (divisor._m.HasValue)
                    return new Numeric((decimal)dividend._i.Value % divisor._m.Value);
                else if (divisor._c.HasValue && divisor._c.Value.Imaginary == 0D)
                    return new Numeric((double)dividend._i.Value % divisor._c.Value.Real);
            }
            else if (dividend._d.HasValue)
            {
                if (divisor._i.HasValue)
                    return new Numeric(dividend._d.Value % (double)divisor._i.Value);
                else if (divisor._d.HasValue)
                    return new Numeric(dividend._d.Value % divisor._d.Value);
                else if (divisor._m.HasValue)
                    return new Numeric((decimal)dividend._d.Value % divisor._m.Value);
                else if (divisor._c.HasValue && divisor._c.Value.Imaginary == 0D)
                    return new Numeric(dividend._d.Value % divisor._c.Value.Real);
            }
            else if (dividend._m.HasValue)
            {
                if (divisor._i.HasValue)
                    return new Numeric(dividend._m.Value % (decimal)divisor._i.Value);
                else if (divisor._d.HasValue)
                    return new Numeric(dividend._m.Value % (decimal)divisor._d.Value);
                else if (divisor._m.HasValue)
                    return new Numeric(dividend._m.Value % divisor._m.Value);
                else if (divisor._c.HasValue && divisor._c.Value.Imaginary == 0D)
                    return new Numeric(dividend._m.Value % (decimal)divisor._c.Value.Real);
            }
            else if (dividend._c.HasValue && dividend._c.Value.Imaginary == 0D)
            {
                if (divisor._i.HasValue)
                    return new Numeric(dividend._c.Value.Real % (double)divisor._i.Value);
                else if (divisor._d.HasValue)
                    return new Numeric(dividend._c.Value.Real % divisor._d.Value);
                else if (divisor._m.HasValue)
                    return new Numeric((decimal)dividend._c.Value.Real % divisor._m.Value);
                else if (divisor._c.HasValue && divisor._c.Value.Imaginary == 0D)
                    return new Numeric(dividend._c.Value.Real % divisor._c.Value.Real);
            }
            throw new ArithmeticException();
        }
    }
}