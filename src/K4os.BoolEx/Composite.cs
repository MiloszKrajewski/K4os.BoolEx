using System;
using System.Collections.Generic;
using System.Linq;
using K4os.BoolEx.Internal;

namespace K4os.BoolEx
{
	public abstract class Composite: Expression
	{
		public IReadOnlyCollection<Expression> Inner { get; }
		protected int HashCode { get; }

		protected Composite(Expression[] expressions)
		{
			Inner = expressions.Required(nameof(expressions));
			HashCode = Inner.Aggregate(GetType().GetHashCode(), (h, e) => h ^ e.GetHashCode());
		}

		protected static Expression Combine<T>(
			IEnumerable<Expression> expressions,
			Func<IEnumerable<Expression>, T> materialize,
			bool shortCircuit)
			where T: Composite
		{
			var candidates = expressions.SelectMany(Flatten<T>);
			var processed = new HashSet<Expression>();
			var result = new List<Expression>();
			var constants = 0;
			foreach (var e in candidates)
			{
				if (e is Constant c)
				{
					if (c.Value == shortCircuit)
						return Constant.Create(shortCircuit);

					constants++;
				}
				else if (processed.Add(e))
				{
					result.Add(e);
				}
			}

			return
				result.Count > 1 ? materialize(result) :
				result.Count > 0 ? result[0] :
				constants > 0 ? Constant.Create(!shortCircuit) :
				throw new ArgumentException("Given argument leads to empty expression");
		}

		protected static IEnumerable<Expression> Flatten<T>(Expression e) where T: Composite =>
			e is T c ? c.Inner.SelectMany(Flatten<T>) : new[] { e };
	}
}
