using System;
using System.Collections.Generic;

namespace K4os.BoolEx
{
	public abstract class Symbol: Expression
	{
		public object Name => GetName();
		protected abstract object GetName();
		public override string ToString() => $"{Name}";

		public static Symbol<T> Create<T>(T name) => Symbol<T>.Create(name);

		public static (Symbol<T1>, Symbol<T2>) Create<T1, T2>(T1 nameA, T2 nameB) =>
			(Create(nameA), Create(nameB));

		public static (Symbol<T1>, Symbol<T2>, Symbol<T3>) Create<T1, T2, T3>(
			T1 nameA, T2 nameB, T3 nameC) =>
			(Create(nameA), Create(nameB), Create(nameC));

		public static (Symbol<T1>, Symbol<T2>, Symbol<T3>, Symbol<T4>) Create<T1, T2, T3, T4>(
			T1 nameA, T2 nameB, T3 nameC, T4 nameD) =>
			(Create(nameA), Create(nameB), Create(nameC), Create(nameD));

		public static (Symbol<T1>, Symbol<T2>, Symbol<T3>, Symbol<T4>, Symbol<T5>)
			Create<T1, T2, T3, T4, T5>(T1 nameA, T2 nameB, T3 nameC, T4 nameD, T5 nameE) =>
			(Create(nameA), Create(nameB), Create(nameC), Create(nameD), Create(nameE));

		public static (Symbol<T1>, Symbol<T2>, Symbol<T3>, Symbol<T4>, Symbol<T5>, Symbol<T6>)
			Create<T1, T2, T3, T4, T5, T6>(T1 nameA, T2 nameB, T3 nameC, T4 nameD, T5 nameE, T6 nameF) =>
			(Create(nameA), Create(nameB), Create(nameC), Create(nameD), Create(nameE), Create(nameF));
	}
	
	public class Symbol<T>: Symbol, IEquatable<Symbol<T>>
	{
		public new T Name { get; }
		private Symbol(T name) => Name = name;
		protected override object GetName() => Name;

		public static Symbol<T> Create(T name) => new Symbol<T>(name);

		// ReSharper disable once PossibleNullReferenceException
		public bool Equals(Symbol<T> other) => 
			this.MaybeEqual(other) ?? Name.Equals(other.Name);

		public override bool Equals(object other) => this.EqualsForEquatable(other);

		public override int GetHashCode() => EqualityComparer<T>.Default.GetHashCode(Name);
	}
}
