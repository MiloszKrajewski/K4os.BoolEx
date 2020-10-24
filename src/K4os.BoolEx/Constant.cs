using System;

namespace K4os.BoolEx
{
	public class Constant: Expression, IEquatable<Constant>
	{
		public bool Value { get; }

		public static readonly Constant True = new Constant(true);
		public static readonly Constant False = new Constant(false);

		public static bool IsTrue(Expression e) => e is Constant c && c.Value;
		public static bool IsFalse(Expression e) => e is Constant c && !c.Value;

		private Constant(bool value) { Value = value; }

		public override string ToString() => Value.ToString();
		public override int GetHashCode() => Value.GetHashCode();

		// ReSharper disable once PossibleNullReferenceException
		public bool Equals(Constant other) =>
			this.MaybeEqual(other) ?? Value.Equals(other.Value);

		public override bool Equals(object other) => this.EqualsForEquatable(other);
	}
}
