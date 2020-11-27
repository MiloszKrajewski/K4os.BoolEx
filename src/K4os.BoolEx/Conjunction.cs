using System;
using System.Collections.Generic;
using System.Linq;
using K4os.BoolEx.Internal;

namespace K4os.BoolEx
{
	public class Conjunction: Composite, IEquatable<Conjunction>
	{
		private Conjunction(IEnumerable<Expression> expressions): 
			base(expressions.ToArray()) { }

		public static Expression Create(Expression a, Expression b) => Create(new[] { a, b });
		public static Expression Create(params Expression[] e) => Create(e.AsEnumerable());
		public static Expression Create(IEnumerable<Expression> expressions) =>
			Combine(expressions, false, x => new Conjunction(x));
		
		public static IEnumerable<Expression> Flatten(Expression e) =>
			Composite.Flatten<Conjunction>(e);

		public override string ToString() =>
			$"({string.Join(" & ", Inner.Select(x => x.ToString()))})";

		public bool Equals(Conjunction other) =>
			!ReferenceEquals(null, other) && (
				ReferenceEquals(this, other) ||
				GetHashCode() == other.GetHashCode() && Inner.EqualSets(other.Inner)
			);

		public override bool Equals(object obj) => this.EqualsForEquatable(obj);

		public override int GetHashCode() => HashCode;
	}
}
