using System;

namespace K4os.BoolEx
{
	public class Symbol: Expression, IEquatable<Symbol>
	{
		public object? Value { get; }
		
		private Symbol(object? value) => Value = value;

		public static Symbol Create(object? value) => new Symbol(value);

		public bool Equals(Symbol? other) =>
			!ReferenceEquals(null, other) &&
			(ReferenceEquals(this, other) || Equals(Value, other.Value));

		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;

			return Equals((Symbol) obj);
		}

		public override int GetHashCode()
		{
			return (Value != null ? Value.GetHashCode() : 0);
		}

		public override string ToString() => Value?.ToString() ?? "<null>";
	}
}
