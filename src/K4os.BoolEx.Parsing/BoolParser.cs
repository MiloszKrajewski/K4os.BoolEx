using System;
using System.Text.RegularExpressions;
using System.Threading;
using Sprache;

namespace K4os.BoolEx.Parsing
{
	public class ExpressionParser
	{
		public static readonly ExpressionParser Default = new ExpressionParser();
		private static readonly Regex DefaultIdentRegex = new Regex(@"(\w|\d|_)+");

		private readonly Lazy<Parser<Expression>> _anyParser;

		public ExpressionParser()
		{
			_anyParser = new Lazy<Parser<Expression>>(
				BuildParser, LazyThreadSafetyMode.ExecutionAndPublication);
		}

		private Parser<Expression> BuildParser()
		{
			Parser<Expression> anyParserPromise = null!;

			// ReSharper disable once AccessToModifiedClosure
			Parser<Expression> anyParser = Parse.Ref(() => anyParserPromise);

			var trueParser = Parse.String("true").Token().Return(Expression.True);
			var falseParser = Parse.String("false").Token().Return(Expression.False);
			var constantParser = trueParser.Or(falseParser);
			var identParser = Parse.Regex(IdentRegex).Token().Select(Expression.Ident);
			var valueParser = constantParser.Or(identParser);

			var parenParser = (
				from lp in Parse.Char('(').Token()
				from e in anyParser
				from rp in Parse.Char(')').Token()
				select e
			).Or(valueParser);

			var notParser = (
				from n in Parse.Char('~').Token()
				from e in parenParser
				select ~e
			).Or(parenParser);

			var andParser = (
				from a in notParser
				from op in Parse.Char('&').Token()
				from b in anyParser
				select a & b
			).Or(notParser);

			var orParser = (
				from a in andParser
				from op in Parse.Char('|').Token()
				from b in anyParser
				select a | b
			).Or(andParser);

			anyParserPromise = orParser;

			return anyParser;
		}

		public virtual Regex IdentRegex => DefaultIdentRegex;

		public Expression FromString(string text) => _anyParser.Value.Parse(text);
	}
}
