using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace TSN.Numerics
{
    [Serializable]
    public partial struct Numeric : ISerializable, ICloneable, IEquatable<Numeric>, IComparable, IComparable<Numeric>, IFormattable
    {
        private Numeric(SerializationInfo info, StreamingContext context)
        {
            _i = (BigInteger?)info.GetValue(nameof(_i), typeof(BigInteger?));
            _d = (double?)info.GetValue(nameof(_d), typeof(double?));
            _m = (decimal?)info.GetValue(nameof(_m), typeof(decimal?));
            _c = (Complex?)info.GetValue(nameof(_c), typeof(Complex?));
        }
        public Numeric(sbyte value)
        {
            _i = value;
            _d = null;
            _m = null;
            _c = null;
        }
        public Numeric(byte value)
        {
            _i = value;
            _d = null;
            _m = null;
            _c = null;
        }
        public Numeric(short value)
        {
            _i = value;
            _d = null;
            _m = null;
            _c = null;
        }
        public Numeric(ushort value)
        {
            _i = value;
            _d = null;
            _m = null;
            _c = null;
        }
        public Numeric(int value)
        {
            _i = value;
            _d = null;
            _m = null;
            _c = null;
        }
        public Numeric(uint value)
        {
            _i = value;
            _d = null;
            _m = null;
            _c = null;
        }
        public Numeric(long value)
        {
            _i = value;
            _d = null;
            _m = null;
            _c = null;
        }
        public Numeric(ulong value)
        {
            _i = value;
            _d = null;
            _m = null;
            _c = null;
        }
        public Numeric(BigInteger value)
        {
            _i = value;
            _d = null;
            _m = null;
            _c = null;
        }
        public Numeric(float value)
        {
            _i = null;
            _d = value;
            _m = null;
            _c = null;
        }
        public Numeric(double value)
        {
            _i = null;
            _d = value;
            _m = null;
            _c = null;
        }
        public Numeric(decimal value)
        {
            _i = null;
            _d = null;
            _m = value;
            _c = null;
        }
        public Numeric(Complex value)
        {
            _i = null;
            _d = null;
            _m = null;
            _c = value;
        }


        private readonly BigInteger? _i;
        private readonly double? _d;
        private readonly decimal? _m;
        private readonly Complex? _c;

        public bool IsInteger => _i.HasValue;
        public bool IsDouble => _d.HasValue;
        public bool IsDecimal => _m.HasValue;
        public bool IsComplex => _c.HasValue;



        public bool IsEmpty() => !_i.HasValue && !_d.HasValue && !_m.HasValue && !_c.HasValue;
        public bool IsFinite() => _i.HasValue || _m.HasValue || (_d.HasValue && IsFinite(_d.Value)) || (_c.HasValue && IsFinite(_c.Value));
        public bool IsInfinity() => (_d.HasValue && double.IsInfinity(_d.Value)) || (_c.HasValue && IsInfinity(_c.Value));
        public bool IsNaN() => _d.HasValue ? double.IsNaN(_d.Value) : (_c.HasValue ? IsNaN(_c.Value) : (!_i.HasValue && !_m.HasValue));

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

        public static bool operator ==(Numeric left, Numeric right) => left.CompareTo(right) == 0;
        public static bool operator !=(Numeric left, Numeric right) => left.CompareTo(right) != 0;
        public static bool operator <(Numeric left, Numeric right) => left.CompareTo(right) < 0;
        public static bool operator >(Numeric left, Numeric right) => left.CompareTo(right) > 0;
        public static bool operator <=(Numeric left, Numeric right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Numeric left, Numeric right) => left.CompareTo(right) >= 0;
    }
}