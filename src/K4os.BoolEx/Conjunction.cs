using System;
using System.Collections.Generic;
using System.Linq;

namespace K4os.BoolEx
{
	public class Conjunction: Expression
	{
		public IEnumerable<Expression> Inner { get; }
		private Conjunction(IEnumerable<Expression> e) => Inner = e;
		public override string ToString() => $"({string.Join(" & ", Inner.Select(x => x.ToString()))})";

		public static Expression Create(Expression a, Expression b) => Create(new[] { a, b });
		public static Expression Create(params Expression[] e) => Create(e.AsEnumerable());

		public static Expression Create(IEnumerable<Expression> expressions)
		{
			var inner = expressions.SelectMany(Flatten).Distinct().ToArray();
			
			var anyFalse = inner.Any(Constant.IsFalse);
			if (anyFalse) return Constant.False;
			
			var allTrue = inner.All(Constant.IsTrue);
			if (allTrue) return Constant.True;
			
			return
				inner.Length <= 0 ? throw new ArgumentException("Empty conjunction is not allowed") :
				inner.Length == 1 ? inner[0] :
				new Conjunction(inner.Where(e => !(e is Constant)));
		}

		public static IEnumerable<Expression> Flatten(Expression e) =>
			e is Conjunction c ? c.Inner.SelectMany(Flatten) : new[] { e };
	}
}
