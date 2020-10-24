using System;

// ReSharper disable InconsistentNaming

namespace K4os.BoolEx
{
	public static class Extensions
	{
		public static Expression NNF(this Expression e) => Operations.NNF(e);
		public static Expression CNF(this Expression e) => Operations.CNF(e);
		public static Expression DNF(this Expression e) => Operations.DNF(e);

		public static CanonicalResult<R> CanonicalDNF<T, R>(
			this Expression e, Func<bool, T, R> adapter) =>
			Operations.CanonicalDNF(e, adapter);

		public static CanonicalResult<R> CanonicalCNF<T, R>(
			this Expression e, Func<bool, T, R> adapter) =>
			Operations.CanonicalCNF(e, adapter);
		
		public static bool? MaybeEqual<T>(this T subject, T other) =>
			ReferenceEquals(other, null) ? false :
			ReferenceEquals(subject, other) ? true :
			default(bool?);

		public static bool EqualsForEquatable<T>(this T subject, object other) 
			where T: IEquatable<T> =>
			!ReferenceEquals(other, null) &&
			(ReferenceEquals(subject, other) || other is T otherAsT && subject.Equals(otherAsT));
	}
}
