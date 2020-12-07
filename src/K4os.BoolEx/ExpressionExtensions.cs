using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace K4os.BoolEx
{
	public static class ExpressionExtensions
	{
		public static Expression NNF(this Expression e) => Operations.NNF(e);
		public static Expression CNF(this Expression e) => Operations.CNF(e);
		public static Expression DNF(this Expression e) => Operations.DNF(e);

		public static bool Evaluate(this Expression e, Func<object?, bool> symbolValue) =>
			e switch {
				Constant constant => constant.Value,
				Negation negation => !Evaluate(negation.Inner, symbolValue),
				Symbol symbol => symbolValue(symbol.Value),
				Conjunction conjunction => conjunction.Inner
					.Select(i => Evaluate(i, symbolValue))
					.Aggregate((a, b) => a & b),
				Disjunction disjunction => disjunction.Inner
					.Select(i => Evaluate(i, symbolValue))
					.Aggregate((a, b) => a | b),
				_ => throw new NotSupportedException(
					$"Evaluating '{e?.GetType().Name}' is not supported")
			};
	}
}
