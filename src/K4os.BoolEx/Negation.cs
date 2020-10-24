using System;

namespace K4os.BoolEx
{
	public class Negation: Expression, IEquatable<Negation>
	{
		public Expression Inner { get; }
		private Negation(Expression e) => Inner = e;
		public override string ToString() => $"~{Inner}";

		public static Expression Create(Expression e) => e is Negation n ? n.Inner : new Negation(e);

		public bool Equals(Negation other) =>
			!(other is null) && (
				ReferenceEquals(this, other) || Equals(Inner, other.Inner));

		public override bool Equals(object other) =>
			!(other is null) && (
				ReferenceEquals(this, other) || other.GetType() == GetType() && Equals((Negation) other));

		public override int GetHashCode() => Inner != null ? Inner.GetHashCode() : 0;
	}
}
