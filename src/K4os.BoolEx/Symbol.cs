using System;
using K4os.BoolEx.Internal;

namespace K4os.BoolEx
{
	public class Symbol: Expression, IEquatable<Symbol>
	{
		public object Value { get; }
		
		private Symbol(object value) => 
			Value = value.Required(nameof(value));

		public static Symbol Create(object value) => new Symbol(value);

		public bool Equals(Symbol other) =>
			!ReferenceEquals(null, other) &&
			(ReferenceEquals(this, other) || Equals(Value, other.Value));

		public override bool Equals(object obj) => this.EqualsForEquatable(obj);
		public override int GetHashCode() => Value.GetHashCode();
		public override string ToString() => Value.ToString();
	}
}
