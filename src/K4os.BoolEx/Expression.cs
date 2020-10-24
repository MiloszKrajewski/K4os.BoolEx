﻿namespace K4os.BoolEx
{
	public abstract class Expression
	{
		public static Expression operator ~(Expression e) => Negation.Create(e);
		public static Expression operator |(Expression a, Expression b) => Disjunction.Create(a, b);
		public static Expression operator &(Expression a, Expression b) => Conjunction.Create(a, b);
	}
}
