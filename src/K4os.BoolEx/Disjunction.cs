using System;
using System.Collections.Generic;
using System.Linq;

namespace K4os.BoolEx
{
	public class Disjunction: Expression
	{
		public IEnumerable<Expression> Inner { get; }
		private Disjunction(IEnumerable<Expression> inner) => Inner = inner;

		public override string ToString() =>
			$"({string.Join(" | ", Inner.Select(x => x.ToString()))})";

		public static Expression Create(Expression a, Expression b) => Create(new[] { a, b });
		public static Expression Create(params Expression[] e) => Create(e.AsEnumerable());

		public static Expression Create(IEnumerable<Expression> expressions)
		{
			var inner = expressions.SelectMany(Flatten).Distinct().ToArray();

			var anyTrue = inner.Any(Constant.IsTrue);
			if (anyTrue) return Constant.True;
			
			var allFalse = inner.All(Constant.IsFalse);
			if (allFalse) return Constant.False;

			return
				inner.Length <= 0 ? throw new ArgumentException("Empty disjunction is not allowed") :
				inner.Length == 1 ? inner[0] :
				new Disjunction(inner.Where(e => !(e is Constant)));
		}

		public static IEnumerable<Expression> Flatten(Expression e) =>
			e is Disjunction d ? d.Inner.SelectMany(Flatten) : new[] { e };
	}
}
