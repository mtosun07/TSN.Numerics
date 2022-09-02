using System;
using System.Numerics;

namespace TSN.Numerics
{
	partial struct Numeric
	{
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
		public static Numeric Min(Numeric left, Numeric right) => left < right ? left : right;
		public static Numeric Max(Numeric left, Numeric right) => left > right ? left : right;
		public static Numeric Pow(Numeric value, Numeric exponent)
		{
			if (exponent._i.HasValue)
			{
				if (value._i.HasValue)
					return BigInteger.Pow(value._i.Value, (int)exponent._i.Value);
				else if (value._d.HasValue)
					return Math.Pow(value._d.Value, (double)exponent._i.Value);
				else if (value._m.HasValue)
					return Math.Pow((double)value._m.Value, (double)exponent._i.Value);
				else if (value._c.HasValue)
					return Complex.Pow(value._c.Value, (double)exponent._i.Value);
			}
			else if (exponent._d.HasValue)
			{
				if (value._i.HasValue)
					return Math.Pow((double)value._i.Value, exponent._d.Value);
				else if (value._d.HasValue)
					return Math.Pow(value._d.Value, exponent._d.Value);
				else if (value._m.HasValue)
					return Math.Pow((double)value._m.Value, exponent._d.Value);
				else if (value._c.HasValue)
					return Complex.Pow(value._c.Value, exponent._d.Value);
			}
			else if (exponent._m.HasValue)
			{
				if (value._i.HasValue)
					return Math.Pow((double)value._i.Value, (double)exponent._m.Value);
				else if (value._d.HasValue)
					return Math.Pow(value._d.Value, (double)exponent._m.Value);
				else if (value._m.HasValue)
					return Math.Pow((double)value._m.Value, (double)exponent._m.Value);
				else if (value._c.HasValue)
					return Complex.Pow(value._c.Value, (double)exponent._m.Value);
			}
			else if (exponent._c.HasValue)
			{
				if (value._c.HasValue)
					return Complex.Pow(value._c.Value, exponent._c.Value);
				else if (exponent._c.Value.Imaginary != 0)
					throw new ArithmeticException();
				else if (value._i.HasValue)
					return Math.Pow((double)value._i.Value, exponent._c.Value.Real);
				else if (value._d.HasValue)
					return Math.Pow(value._d.Value, exponent._c.Value.Real);
				else if (value._m.HasValue)
					return Math.Pow((double)value._m.Value, exponent._c.Value.Real);
			}
			return _empty;
		}
		public static Numeric ModPow(Numeric value, Numeric exponent, Numeric modulus) => value._i.HasValue && exponent._i.HasValue && modulus._i.HasValue ? BigInteger.ModPow(value._i.Value, exponent._i.Value, modulus._i.Value) : (Pow(value, exponent) % modulus);
		public static Numeric Sqrt(Numeric value) => value._c.HasValue ? Complex.Sqrt(value._c.Value) : (value._i.HasValue ? Math.Sqrt((double)value._i.Value) : (value._d.HasValue ? Math.Sqrt(value._d.Value) : (value._m.HasValue ? Math.Sqrt((double)value._m.Value) : _empty)));
		public static Numeric Log(Numeric value) => value._i.HasValue ? BigInteger.Log(value._i.Value) : (value._c.HasValue ? Complex.Log(value._c.Value) : (value._d.HasValue ? Math.Log(value._d.Value) : (value._m.HasValue ? Math.Log((double)value._m.Value) : _empty)));
		public static Numeric Log(Numeric value, Numeric baseValue) => value._i.HasValue ? BigInteger.Log(value._i.Value, (double)baseValue) : (value._c.HasValue ? Complex.Log(value._c.Value, (double)baseValue) : (value._d.HasValue ? Math.Log(value._d.Value, (double)baseValue) : (value._m.HasValue ? Math.Log((double)value._m.Value, (double)baseValue) : _empty)));
		public static Numeric Log10(Numeric value) => value._i.HasValue ? BigInteger.Log10(value._i.Value) : (value._c.HasValue ? Complex.Log10(value._c.Value) : (value._d.HasValue ? Math.Log10(value._d.Value) : (value._m.HasValue ? Math.Log10((double)value._m.Value) : _empty)));
		public static Numeric Exp(Numeric value) => value._i.HasValue ? Math.Exp((double)value._i.Value) : (value._c.HasValue ? Complex.Exp(value._c.Value) : (value._d.HasValue ? Math.Exp(value._d.Value) : (value._m.HasValue ? Math.Exp((double)value._m.Value) : _empty)));

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
				if (divisor._i.HasValue)
					return new Numeric(dividend._i.Value / divisor._i.Value);
				else if (divisor._d.HasValue)
					return new Numeric((double)dividend._i.Value / divisor._d.Value);
				else if (divisor._m.HasValue)
					return new Numeric((decimal)dividend._i.Value / divisor._m.Value);
				else if (divisor._c.HasValue)
					return new Numeric((Complex)dividend._i.Value / divisor._c.Value);
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