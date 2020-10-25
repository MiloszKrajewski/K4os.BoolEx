using System;
using System.Collections.Generic;
using System.Linq;
using K4os.BoolEx.Internal;

// ReSharper disable InconsistentNaming

namespace K4os.BoolEx
{
	public class Operations
	{
		public static Expression NNF(Expression e) =>
			e switch {
				Constant _ => e, 
				Symbol _ => e,
				Negation n when n.Inner is Symbol => e,
				Negation n when n.Inner is Constant c => Negate(c),
				Negation n when n.Inner is Negation nn => NNF(nn.Inner),
				Negation n when n.Inner is Conjunction c => 
					Disjunction.Create(c.Inner.Select(x => NNF(Negation.Create(x)))),
				Negation n when n.Inner is Disjunction d => 
					Conjunction.Create(d.Inner.Select(x => NNF(Negation.Create(x)))),
				Conjunction c => Conjunction.Create(c.Inner.Select(NNF)),
				Disjunction d => Conjunction.Create(d.Inner.Select(NNF)),
				_ => throw new ArgumentException($"{e.GetType().Name} is not supported")
			};

		private static Expression Negate(Negation n) => n.Inner;

		private static Expression Negate(Constant c) => Constant.Negate(c);

		private static Expression Negate(Disjunction d) => 
			Conjunction.Create(d.Inner.Select(Negation.Create));

		private static Expression Negate(Conjunction c) => 
			Disjunction.Create(c.Inner.Select(Negation.Create));
		
		private static Expression Flatten(Conjunction c) => 
			Conjunction.Create(c.Inner.Select(CNF).SelectMany(Conjunction.Flatten));
		
		private static Expression Flatten(Disjunction d) => 
			Disjunction.Create(d.Inner.Select(DNF).SelectMany(Disjunction.Flatten));

		private static Expression ToConjunction(Disjunction d) =>
			Conjunction.Create(
				d.Inner.Select(CNF).Select(Conjunction.Flatten)
					.Aggregate((pn, qn) => Dot(pn, qn, Disjunction.Create)));
		
		private static Expression ToDisjunction(Conjunction c) =>
			Disjunction.Create(
				c.Inner.Select(DNF).Select(Disjunction.Flatten)
					.Aggregate((pn, qn) => Dot(pn, qn, Conjunction.Create)));
		
		private static IEnumerable<Expression> Dot(
			IEnumerable<Expression> pn, IEnumerable<Expression> qn,
			Func<Expression, Expression, Expression> combine)
		{
			var qna = qn.AsArray(); // avoid multiple iterations
			foreach (var p in pn)
			foreach (var q in qna)
				yield return combine(p, q);
		}


		public static Expression CNF(Expression e) =>
			e switch {
				Constant _ => e,
				Symbol _ => e,
				Negation n when n.Inner is Symbol => e,
				Negation n when n.Inner is Constant c => Negate(c),
				Negation n when n.Inner is Negation nn => CNF(Negate(nn)),
				Negation n when n.Inner is Conjunction c => CNF(Negate(c)),
				Negation n when n.Inner is Disjunction d => CNF(Negate(d)),
				Conjunction c => Flatten(c),
				Disjunction d => ToConjunction(d),
				_ => throw new ArgumentException($"{e.GetType().Name} is not supported")
			};

		public static Expression DNF(Expression e) =>
			e switch {
				Constant _ => e,
				Symbol _ => e,
				Negation n when n.Inner is Symbol => e,
				Negation n when n.Inner is Constant c => Negate(c),
				Negation n when n.Inner is Negation nn => DNF(Negate(nn)),
				Negation n when n.Inner is Conjunction c => DNF(Negate(c)),
				Negation n when n.Inner is Disjunction d => DNF(Negate(d)),
				Disjunction d => Flatten(d),
				Conjunction c => ToDisjunction(c),
				_ => throw new ArgumentException($"{e.GetType().Name} is not supported")
			};
	}
}
