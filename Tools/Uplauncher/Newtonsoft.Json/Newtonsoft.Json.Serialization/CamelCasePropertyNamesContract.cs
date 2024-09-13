using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	/*[NullableContext(1)]*/
	/*[Nullable(0)]*/
	public class CamelCasePropertyNamesContractResolver : DefaultContractResolver
	{
		private static readonly object TypeContractCacheLock = new object();

		private static readonly DefaultJsonNameTable NameTable = new DefaultJsonNameTable();

		/*[Nullable(new byte[] { 2, 0, 1, 1, 1 })]*/
		private static Dictionary<StructMultiKey<Type, Type>, JsonContract> _contractCache;

		public CamelCasePropertyNamesContractResolver()
		{
			NamingStrategy = new CamelCaseNamingStrategy
			{
				ProcessDictionaryKeys = true,
				OverrideSpecifiedNames = true
			};
		}

		public override JsonContract ResolveContract(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			StructMultiKey<Type, Type> key = new StructMultiKey<Type, Type>(GetType(), type);
			Dictionary<StructMultiKey<Type, Type>, JsonContract> contractCache = _contractCache;
			if (contractCache == null || !contractCache.TryGetValue(key, out var value))
			{
				value = CreateContract(type);
				lock (TypeContractCacheLock)
				{
					contractCache = _contractCache;
					Dictionary<StructMultiKey<Type, Type>, JsonContract> obj = ((contractCache != null) ? new Dictionary<StructMultiKey<Type, Type>, JsonContract>(contractCache) : new Dictionary<StructMultiKey<Type, Type>, JsonContract>());
					obj[key] = value;
					_contractCache = obj;
					return value;
				}
			}
			return value;
		}

		internal override DefaultJsonNameTable GetNameTable()
		{
			return NameTable;
		}
	}
}
