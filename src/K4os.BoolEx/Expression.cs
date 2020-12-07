using System;
using System.Collections.Generic;

namespace K4os.BoolEx
{
	public abstract class Expression
	{
		public static Expression operator ~(Expression e) => Negation.Create(e);
		public static Expression operator |(Expression a, Expression b) => Disjunction.Create(a, b);
		public static Expression operator &(Expression a, Expression b) => Conjunction.Create(a, b);
		
		public static Expression Not(Expression expression) => 
			Negation.Create(expression);
		public static Expression And(IEnumerable<Expression> sequence) => 
			Conjunction.Create(sequence);
		public static Expression And(params Expression[] sequence) => 
			Conjunction.Create(sequence);
		public static Expression Or(IEnumerable<Expression> sequence) => 
			Disjunction.Create(sequence);
		public static Expression Or(params Expression[] sequence) => 
			Disjunction.Create(sequence);
		
		public static Expression True => Constant.True;
		public static Expression False => Constant.False;
		public static Expression Const(bool value) => Constant.Create(value);
		public static Expression Ident<T>(T value) => Symbol.Create(value);

	}
}
