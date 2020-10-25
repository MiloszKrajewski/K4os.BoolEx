using System;
using System.Collections.Generic;
using System.Linq;
using K4os.BoolEx.Internal;

namespace K4os.BoolEx
{
	public class Disjunction: Composite, IEquatable<Disjunction>
	{
		private Disjunction(IEnumerable<Expression> expressions):
			base(expressions.ToArray()) { }

		public static Expression Create(Expression a, Expression b) => Create(new[] { a, b });
		public static Expression Create(params Expression[] e) => Create(e.AsEnumerable());
		public static Expression Create(IEnumerable<Expression> expressions) =>
			Combine(expressions, x => new Disjunction(x), true);

		public static IEnumerable<Expression> Flatten(Expression e) =>
			Composite.Flatten<Disjunction>(e);

		public override string ToString() =>
			$"({string.Join(" | ", Inner.Select(x => x.ToString()))})";

		public bool Equals(Disjunction other) =>
			!ReferenceEquals(null, other) && (
				ReferenceEquals(this, other) ||
				GetHashCode() == other.GetHashCode() && Inner.EqualSets(other.Inner)
			);

		public override bool Equals(object obj) => this.EqualsForEquatable(obj);

		public override int GetHashCode() => HashCode;
	}
}
