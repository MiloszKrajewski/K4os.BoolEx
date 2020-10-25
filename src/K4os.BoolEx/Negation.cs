using System;
using K4os.BoolEx.Internal;

namespace K4os.BoolEx
{
	public class Negation: Expression, IEquatable<Negation>
	{
		public Expression Inner { get; }
		
		private Negation(Expression expression) => 
			Inner = expression.Required(nameof(expression));

		public static Expression Create(Expression e) => 
			e switch {
				Negation n => n.Inner,
				Constant c => Constant.Negate(c),
				_ => new Negation(e)
			};

		public bool Equals(Negation other) =>
			!ReferenceEquals(null, other) && 
			(ReferenceEquals(this, other) || Equals(Inner, other.Inner));

		public override bool Equals(object obj) => this.EqualsForEquatable(obj);
		
		public override int GetHashCode() => -Inner.GetHashCode();
		
		public override string ToString() => $"~{Inner}";
	}
}
