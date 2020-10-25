using System;

// ReSharper disable InconsistentNaming

namespace K4os.BoolEx
{
	public static class ExpressionExtensions
	{
		public static Expression NNF(this Expression e) => Operations.NNF(e);
		public static Expression CNF(this Expression e) => Operations.CNF(e);
		public static Expression DNF(this Expression e) => Operations.DNF(e);
	}
}
