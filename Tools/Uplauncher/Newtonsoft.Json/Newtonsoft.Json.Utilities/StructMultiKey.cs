using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	internal readonly struct StructMultiKey< T1,  T2> : IEquatable<StructMultiKey<T1, T2>>
	{
		public readonly T1 Value1;

		public readonly T2 Value2;

		public StructMultiKey(T1 v1, T2 v2)
		{
			Value1 = v1;
			Value2 = v2;
		}

		public override int GetHashCode()
		{
			return (Value1?.GetHashCode() ?? 0) ^ (Value2?.GetHashCode() ?? 0);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is StructMultiKey<T1, T2> other))
			{
				return false;
			}
			return Equals(other);
		}

		public bool Equals(/*[Nullable(new byte[] { 0, 1, 1 })]*/ StructMultiKey<T1, T2> other)
		{
			if (Equals(Value1, other.Value1))
			{
				return Equals(Value2, other.Value2);
			}
			return false;
		}
	}
}
