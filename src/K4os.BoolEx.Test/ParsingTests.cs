using K4os.BoolEx.Parsing;
using Xunit;

namespace K4os.BoolEx.Test
{
	public class ParsingTests
	{
		public Expression Parse(string expression) =>
			ExpressionParser.Default.FromString(expression);
		
		[Fact]
		public void ConstantsAreRecognized()
		{
			var expressionTrue = Parse("true");
			Assert.Equal(Constant.True, expressionTrue);
			
			var expressionFalse = Parse("false");
			Assert.Equal(Constant.False, expressionFalse);
		}
		
		[Theory]
		[InlineData("hello")]
		[InlineData("world123")]
		[InlineData("__special__")]
		[InlineData("0")]
		[InlineData("1")]
		public void IdentifiersAreRecognized(string name)
		{
			var expression = ExpressionParser.Default.FromString(name);
			Assert.Equal(Symbol.Create(name), expression);
		}
		
		[Theory]
		[InlineData("a|b", "(a | b)")]
		[InlineData("a&b", "(a & b)")]
		[InlineData("~a&b", "(~a & b)")]
		[InlineData("~(a&b)", "~(a & b)")]
		[InlineData("a|b&c", "(a | (b & c))")]
		[InlineData("(a|b)&c", "((a | b) & c)")]
		[InlineData("a|~b&c", "(a | (~b & c))")]
		[InlineData(" (     a | b ) &    c ", "((a | b) & c)")]
		[InlineData(" a | ~ b & c", "(a | (~b & c))")]
		[InlineData(" a|b&c|d", "(a | (b & c) | d)")]
		[InlineData(" a&b|c&d", "((a & b) | (c & d))")]
		[InlineData(" a|b&~c|d", "(a | (b & ~c) | d)")]
		[InlineData(" a&b|~c&d", "((a & b) | (~c & d))")]
		[InlineData(" a|~(b&c)|d", "(a | ~(b & c) | d)")]
		[InlineData(" a&~(b|c)&d", "(a & ~(b | c) & d)")]
		public void ExpressionRoundtrip(string expression, string expected)
		{
			var value = Parse(expression);
			Assert.Equal(expected, value.ToString());
		}
	}
}
