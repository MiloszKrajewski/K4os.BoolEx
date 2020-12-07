using System;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace K4os.BoolEx
{
	public static class Canonical
	{
		public static Result<T> DNF<T>(
			Expression expression, Func<bool, object?, T> adapter)
		{
			T AsSymbol(Expression e, bool invert = false) =>
				e is Symbol s ? adapter(invert, s.Value) : throw SymbolExpected(e);

			T AsNegation(Expression e) =>
				e is Negation n ? AsSymbol(n.Inner, true) : AsSymbol(e);

			T[] AsConjunction(Expression e) =>
				e is Conjunction c
					? c.Inner.Select(AsNegation).ToArray()
					: new[] { AsNegation(e) };

			T[][] AsDisjunction(Expression e) =>
				e is Disjunction d
					? d.Inner.Select(AsConjunction).ToArray()
					: new[] { AsConjunction(e) };

			return Translate(expression.DNF(), AsDisjunction);
		}

		public static Result<T> CNF<T>(
			Expression expression, Func<bool, object?, T> adapter)
		{
			T AsSymbol(Expression e, bool invert = false) =>
				e is Symbol s ? adapter(invert, s.Value) : throw SymbolExpected(e);

			T AsNegation(Expression e) =>
				e is Negation n ? AsSymbol(n.Inner, true) : AsSymbol(e);

			T[] AsDisjunction(Expression e) =>
				e is Disjunction c
					? c.Inner.Select(AsNegation).ToArray()
					: new[] { AsNegation(e) };

			T[][] AsConjunction(Expression e) =>
				e is Conjunction c
					? c.Inner.Select(AsDisjunction).ToArray()
					: new[] { AsDisjunction(e) };

			return Translate(expression.CNF(), AsConjunction);
		}

		public class Result<T>
		{
			public bool? Constant { get; }
			public T[][]? Expression { get; }

			private Result(bool? constant, T[][]? expression)
			{
				Constant = constant;
				Expression = expression;
			}

			public Result(T[][] expression): this(null, expression) { }

			public Result(bool constant): this(constant, null) { }
		}

		private static Result<TData> Translate<TData>(
			Expression e, Func<Expression, TData[][]> parser) =>
			e is Constant c ? new Result<TData>(c.Value) : new Result<TData>(parser(e));

		private static Exception SymbolExpected(Expression? e) =>
			new ArgumentException(
				$"{nameof(Symbol)} expected but {(e?.GetType().Name ?? "<null>")} found");
	}
}
