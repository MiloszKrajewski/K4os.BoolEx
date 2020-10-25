using System;
using K4os.BoolEx.Internal;

namespace K4os.BoolEx
{
	public class Constant: Expression, IEquatable<Constant>
	{
		public bool Value { get; }

		public static readonly Constant True = new Constant(true);
		public static readonly Constant False = new Constant(false);

		private Constant(bool value) { Value = value; }

		public static bool IsTrue(Expression e) => e is Constant c && c.Value;
		public static bool IsFalse(Expression e) => e is Constant c && !c.Value;
		
		public static Expression Negate(Constant constant) => constant.Value ? False : True;

		public static Expression Create(bool value) => value ? True : False;

		public bool Equals(Constant other) =>
			!ReferenceEquals(null, other) &&
			(ReferenceEquals(this, other) || Value == other.Value);

		public override bool Equals(object obj) => this.EqualsForEquatable(obj);

		public override int GetHashCode() => Value.GetHashCode();
		
		public override string ToString() => Value.ToString();
	}
}
