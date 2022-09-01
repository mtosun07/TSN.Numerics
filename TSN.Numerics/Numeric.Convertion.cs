using System;
using System.Globalization;
using System.Numerics;

namespace TSN.Numerics
{
    partial struct Numeric
    {
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
    }
}