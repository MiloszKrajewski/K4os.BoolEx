using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace K4os.BoolEx.Internal
{
	public static class Extensions
	{
		public static T Required<T>(this T value, string name = null) =>
			value ?? throw new ArgumentNullException(name ?? "<expression>");

		public static bool EqualsForEquatable<T>(this T subject, object other)
			where T: IEquatable<T> =>
			!ReferenceEquals(null, other) && (
				ReferenceEquals(subject, other) ||
				other.GetType() == subject.GetType() && subject.Equals((T) other)
			);

		public static bool EqualSets<T>(
			this IReadOnlyCollection<T> sequenceA, IReadOnlyCollection<T> sequenceB) =>
			sequenceA.Count == sequenceB.Count && (
				sequenceA is ISet<T> setA && setA.SetEquals(sequenceB) ||
				sequenceB is ISet<T> setB && setB.SetEquals(sequenceA) ||
				sequenceA.ToSet().SetEquals(sequenceB)
			);

		public static bool IsEmpty<T>(this IEnumerable<T> sequence) =>
			sequence is ICollection collection ? collection.Count <= 0 : !sequence.Any();

		public static T[] AsArray<T>(this IEnumerable<T> sequence, bool allowNull = false) =>
			sequence is null ? allowNull ? null : Array.Empty<T>() :
			sequence is T[] array ? array :
			sequence.ToArray();

		public static HashSet<T> ToSet<T>(this IEnumerable<T> sequence) =>
			new HashSet<T>(sequence);
	}
}
