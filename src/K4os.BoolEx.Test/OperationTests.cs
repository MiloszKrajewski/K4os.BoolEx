using System;
using System.Linq;
using Xunit;

// ReSharper disable InconsistentNaming

namespace K4os.BoolEx.Test
{
	public class OperationTests
	{
		private static readonly Expression A = Symbol.Create("A");
		private static readonly Expression B = Symbol.Create("B");
		private static readonly Expression C = Symbol.Create("C");
		private static readonly Expression D = Symbol.Create("D");
		private static readonly Expression E = Symbol.Create("E");
		private static readonly Expression F = Symbol.Create("F");
		private static readonly Expression TRUE = Constant.True;
		private static readonly Expression FALSE = Constant.False;

		private static string Explain(Expression e)
		{
			switch (e)
			{
				case Constant v: return $"{v.Value}";
				case Symbol v: return $"{v.Value}";
				case Negation n: return $"~{Explain(n.Inner)}";
				case Disjunction d: return $"({string.Join("|", d.Inner.Select(Explain))})";
				case Conjunction c: return $"({string.Join("&", c.Inner.Select(Explain))})";
				default: throw new ArgumentException("Unexpected expression type");
			}
		}

		private static string ExplainDNF(string[][] c) => // magic :-)
			$"({string.Join("|", c.Select(d => $"({string.Join("&", d)})"))})";

		private static string ExplainDNF(Canonical.Result<string> c) => // magic :-)
			c.Expression != null
				? ExplainDNF(c.Expression)
				: $"({c.Constant ?? throw new ArgumentException()})";

		[Fact]
		public void WhenNestedDisjunctionIsCreated_ThenExpressionsAreFlattened()
		{
			var x = A | C;
			var y = B | E | D;
			Assert.Equal("(A|C|B|E|D)", Explain(x | y));
		}

		[Fact]
		public void WhenNestedConjunctionIsCreated_ThenExpressionsAreFlattened()
		{
			var x = A & C;
			var y = B;
			var z = E & D;
			Assert.Equal("(A&C&B&E&D)", Explain(x & y & z));
		}

		[Fact]
		public void WhenNestingConjunctionInsideDisjunction_ThenNestingIsPreserved()
		{
			var x = A & C;
			var y = B;
			var z = E & D;
			Assert.Equal("((A&C)|B|(E&D))", Explain(x | y | z));
		}

		[Fact]
		public void WhenNestingDisjunctionInsideConjunction_ThenNestingIsPreserved()
		{
			var x = A | C;
			var y = B | E | D;
			var z = x & y;
			Assert.Equal("((A|C)&(B|E|D))", Explain(z));
		}

		[Fact]
		public void WhenUsingDNF_ThenSimpleConjunctionConvertsToDisjunction()
		{
			var x = (A | B) & C;
			Assert.Equal("((A&C)|(B&C))", Explain(x.DNF()));
		}

		[Fact]
		public void WhenUsingDNF_ThenSimpleDisjunctionStaysDisjunction()
		{
			var x = A | (B & C);
			Assert.Equal("(A|(B&C))", Explain(x.DNF()));
		}

		[Fact]
		public void WhenUsingDNF_ThenNestedExpressionsAreFlattened()
		{
			var x = (A | B) & (C | (D & E));
			Assert.Equal("((A&C)|(A&D&E)|(B&C)|(B&D&E))", Explain(x.DNF()));
		}

		[Fact]
		public void WhenUsingDNF_ThenConjunctionConvertsToDisjunction()
		{
			var x = A | C;
			var y = B | ~E | D;
			var z = x & y;
			Assert.Equal("((A&B)|(A&~E)|(A&D)|(C&B)|(C&~E)|(C&D))", Explain(z.DNF()));
		}

		[Fact]
		public void WhenUsingDNF_ThenConjunctionWithNestedNegationConvertsToDisjunction()
		{
			var x = A | C;
			var y = ~(B & ~E & D);
			var z = x & y;
			Assert.Equal("((A&~B)|(A&E)|(A&~D)|(C&~B)|(C&E)|(C&~D))", Explain(z.DNF()));
		}

		[Fact]
		public void WhenUsingDNF_ThenDoubleNestedExpressionsAreFlattened()
		{
			var x = (A | B) & (C | (D & (E | F)));
			Assert.Equal("((A&C)|(A&D&E)|(A&D&F)|(B&C)|(B&D&E)|(B&D&F))", Explain(x.DNF()));
		}
		
		[Fact]
		public void WhenUsingConstants_ThenTheyGetOptimized()
		{
			void Test(Expression expected, Expression input) =>
				Assert.Equal(Explain(expected), Explain(input.DNF()));

			Test(FALSE, FALSE | FALSE);
			Test(TRUE, FALSE | TRUE);
			Test(TRUE, TRUE | FALSE);
			Test(TRUE, TRUE | TRUE);

			Test(FALSE, FALSE & FALSE);
			Test(FALSE, FALSE & TRUE);
			Test(FALSE, TRUE & FALSE);
			Test(TRUE, TRUE & TRUE);

			Test(TRUE, FALSE | FALSE | TRUE | FALSE);
			Test(FALSE, TRUE & TRUE & FALSE & TRUE);
		}

		[Fact]
		public void WhenUsingConstants_ThenTheyMakeOtherExpressionsRedundant()
		{
			void Test(Expression expected, Expression input) =>
				Assert.Equal(Explain(expected), Explain(input.DNF()));

			Test(A, A & (TRUE | B));
			Test(A | B, A | (TRUE & B));
			Test(FALSE, A & (FALSE & B));
			Test(A | B, A | (B & TRUE) | (C & (FALSE | FALSE | FALSE | (D & FALSE))));
		}

		[Fact]
		public void WhenUsingDNF_ThenFalseBubblesUp()
		{
			var x = A | (B & FALSE);
			Assert.Equal("A", Explain(x.DNF()));
		}

		[Fact]
		public void WhenUsingDNF_ThenDoubleNestedExpressionsWithNegationAreFlattened()
		{
			var x = (A | B) & (C | (D & ~(~E & ~F)));
			Assert.Equal("((A|B)&(C|(D&~(~E&~F))))", Explain(x));
			Assert.Equal("((A&C)|(A&D&E)|(A&D&F)|(B&C)|(B&D&E)|(B&D&F))", Explain(x.DNF()));
		}

		[Fact]
		public void WhenUsingDNF_ThenCanonicalDNFProducesSameResult()
		{
			var x = (A | B) & (C | (D & ~(~E & ~F)));
			var y = ExplainDNF(Canonical.DNF(x, (i, v) => $"{(i ? "~" : "")}{v}"));
			Assert.Equal("((A&C)|(A&D&E)|(A&D&F)|(B&C)|(B&D&E)|(B&D&F))", y);
		}

		[Fact]
		public void WhenComparingSymbols_ThenNegationIsHandledProperly()
		{
			bool Opposite(Expression a, Expression b) => Equals(a, ~b);
			Assert.True(Opposite(A, ~A));
			Assert.True(Opposite(~A, A));
			Assert.False(Opposite(A, A));
			Assert.False(Opposite(A, B));
			Assert.False(Opposite(~A, ~B));
		}
		
		[Fact]
		public void WhenConstantIsNegated_ThenResultIsStillConstant()
		{
			var t = Constant.Create(true);
			var f = Constant.False;

			Assert.Equal(~t, f);
			Assert.Equal(~f, t);
		}
		
		[Fact]
		public void WhenCombiningIdenticalSubexpressions_ThenTheyGetCollapsed()
		{
			var x = A & B & C;
			var y = C & A & B;
			
			Assert.Equal(x & y, A & B & C);
			Assert.Equal(x & y, C & B & A);
			Assert.Equal(x | y, B & A & C);
			Assert.Equal((x | y) & (x & y), B & A & C);
			Assert.Equal((x | y) | (x | y), B & A & C);
		}
	}
}
