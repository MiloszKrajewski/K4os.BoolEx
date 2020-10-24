using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace K4os.BoolEx
{
	public class Operations
	{
		public static Expression NNF(Expression e)
		{
			switch (e)
			{
				case Constant _: return e;
				case Symbol _: return e;
				case Negation n when n.Inner is Negation nn: return NNF(nn.Inner);
				case Negation n when n.Inner is Symbol _: return e;
				case Negation n when n.Inner is Conjunction c:
					return Disjunction.Create(c.Inner.Select(x => NNF(Negation.Create(x))));
				case Negation n when n.Inner is Disjunction d:
					return Conjunction.Create(d.Inner.Select(x => NNF(Negation.Create(x))));
				case Conjunction c: return Conjunction.Create(c.Inner.Select(NNF));
				case Disjunction d: return Conjunction.Create(d.Inner.Select(NNF));
				default: throw new ArgumentException($"{e.GetType().Name} is not supported");
			}
		}

		public static Expression CNF(Expression e)
		{
			switch (e)
			{
				case Constant _: return e;
				case Symbol _: return e;
				case Negation n when n.Inner is Negation nn: return CNF(nn.Inner);
				case Negation n when n.Inner is Symbol _: return e;
				case Negation n when n.Inner is Conjunction c:
					return CNF(Disjunction.Create(c.Inner.Select(Negation.Create)));
				case Negation n when n.Inner is Disjunction d:
					return CNF(Conjunction.Create(d.Inner.Select(Negation.Create)));
				case Conjunction c:
					return Conjunction.Create(c.Inner.Select(CNF).SelectMany(Conjunction.Flatten));
				case Disjunction d:
					return Conjunction.Create(
						d.Inner.Select(CNF).Select(Conjunction.Flatten)
							.Aggregate((pn, qn) => Multiply(pn, qn, Disjunction.Create)));
				default: throw new ArgumentException($"{e.GetType().Name} is not supported");
			}
		}

		public static Expression DNF(Expression e)
		{
			switch (e)
			{
				case Constant _: return e;
				case Symbol _: return e;
				case Negation n when n.Inner is Negation nn: return DNF(nn.Inner);
				case Negation n when n.Inner is Symbol _: return e;
				case Negation n when n.Inner is Conjunction c:
					return DNF(Disjunction.Create(c.Inner.Select(Negation.Create)));
				case Negation n when n.Inner is Disjunction d:
					return DNF(Conjunction.Create(d.Inner.Select(Negation.Create)));
				case Disjunction d:
					return Disjunction.Create(d.Inner.Select(DNF).SelectMany(Disjunction.Flatten));
				case Conjunction c:
					return Disjunction.Create(
						c.Inner.Select(DNF).Select(Disjunction.Flatten)
							.Aggregate((pn, qn) => Multiply(pn, qn, Conjunction.Create)));
				default: throw new ArgumentException($"{e.GetType().Name} is not supported");
			}
		}

		private static IEnumerable<Expression> Multiply(
			IEnumerable<Expression> pn,
			IEnumerable<Expression> qn,
			Func<Expression, Expression, Expression> combine)
		{
			var pnc = pn.ToArray();
			var qnc = qn.ToArray();
			foreach (var p in pnc)
			foreach (var q in qnc)
				yield return combine(p, q);
		}

		public static CanonicalResult<R> CanonicalDNF<T, R>(Expression e, Func<bool, T, R> adapter)
		{
			R AsSymbol(Expression ns, bool invert = false) =>
				ns is Symbol s ? adapter(invert, (T) s.Name) : throw new Exception();

			R AsNegation(Expression ns) =>
				ns is Negation n ? AsSymbol(n.Inner, true) : AsSymbol(ns);

			R[] AsConjunction(Expression cnv) =>
				cnv is Conjunction c ? c.Inner.Select(AsNegation).ToArray()
					: new[] { AsNegation(cnv) };

			R[][] AsDisjunction(Expression dcnv) =>
				dcnv is Disjunction d ? d.Inner.Select(AsConjunction).ToArray()
					: new[] { AsConjunction(dcnv) };

			var result = e.DNF();
			return
				Constant.IsTrue(result) ? CanonicalResult.Create<R>(true) :
				Constant.IsFalse(result) ? CanonicalResult.Create<R>(false) :
				CanonicalResult.Create(AsDisjunction(result));
		}

		public static CanonicalResult<R> CanonicalCNF<T, R>(Expression e, Func<bool, T, R> adapter)
		{
			R AsSymbol(Expression ns, bool invert = false) =>
				ns is Symbol s ? adapter(invert, (T) s.Name) : throw new Exception();

			R AsNegation(Expression nv) =>
				nv is Negation n ? AsSymbol(n.Inner, true) : AsSymbol(nv);

			R[] AsDisjunction(Expression dnv) =>
				dnv is Disjunction c 
					? c.Inner.Select(AsNegation).ToArray() 
					: new[] { AsNegation(dnv) };

			R[][] AsConjunction(Expression cdnv) =>
				cdnv is Conjunction c 
					? c.Inner.Select(AsDisjunction).ToArray()
					: new[] { AsDisjunction(cdnv) };

			var result = e.CNF();
			return
				Constant.IsTrue(result) ? CanonicalResult.Create<R>(true) :
				Constant.IsFalse(result) ? CanonicalResult.Create<R>(false) :
				CanonicalResult.Create(AsConjunction(result));
		}
	}
}
