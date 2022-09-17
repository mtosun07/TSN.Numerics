﻿using System;
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
            if (value.Sign == 0)
            {
                _i = null;
                _d = 0D;
            }
            else
            {
                _i = value;
                _d = null;
            }
            _m = null;
            _c = null;
        }
        public Numeric(float value) : this((double)value) { }
        public Numeric(double value)
        {
            _m = null;
            _c = null;
            var x = Math.Truncate(value);
            if (value == x)
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
        public static bool TryGetSign(Numeric value, out int sign)
        {
            if (!value.IsEmpty() && !value.IsNaN())
            {
                sign = value._i?.Sign ?? value.CompareTo(_zero);
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

        public static bool operator ==(Numeric left, Numeric right) => left.CompareTo(right) == 0;
        public static bool operator !=(Numeric left, Numeric right) => left.CompareTo(right) != 0;
        public static bool operator <(Numeric left, Numeric right) => left.CompareTo(right) < 0;
        public static bool operator >(Numeric left, Numeric right) => left.CompareTo(right) > 0;
        public static bool operator <=(Numeric left, Numeric right) => left.CompareTo(right) <= 0;
        public static bool operator >=(Numeric left, Numeric right) => left.CompareTo(right) >= 0;
    }
}