using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Utilities;

namespace Newtonsoft.Json.Serialization
{
	/*[NullableContext(2)]*/
	/*[Nullable(0)]*/
	public class JsonDictionaryContract : JsonContainerContract
	{
		private readonly Type _genericCollectionDefinitionType;

		private Type _genericWrapperType;

		/*[Nullable(new byte[] { 2, 1 })]*/
		private ObjectConstructor<object> _genericWrapperCreator;

		/*[Nullable(new byte[] { 2, 1 })]*/
		private Func<object> _genericTemporaryDictionaryCreator;

		private readonly ConstructorInfo _parameterizedConstructor;

		/*[Nullable(new byte[] { 2, 1 })]*/
		private ObjectConstructor<object> _overrideCreator;

		/*[Nullable(new byte[] { 2, 1 })]*/
		private ObjectConstructor<object> _parameterizedCreator;

		/*[Nullable(new byte[] { 2, 1, 1 })]*/
		/*[field: Nullable(new byte[] { 2, 1, 1 })]*/
		public Func<string, string> DictionaryKeyResolver
		{
			//[return: Nullable(new byte[] { 2, 1, 1 })]
			get;
			//[param: Nullable(new byte[] { 2, 1, 1 })]
			set;
		}

		public Type DictionaryKeyType { get; }

		public Type DictionaryValueType { get; }

		internal JsonContract KeyContract { get; set; }

		internal bool ShouldCreateWrapper { get; }

		/*[Nullable(new byte[] { 2, 1 })]*/
		internal ObjectConstructor<object> ParameterizedCreator
		{
			/*[return: Nullable(new byte[] { 2, 1 })]*/
			get
			{
				if (_parameterizedCreator == null && _parameterizedConstructor != null)
				{
					_parameterizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(_parameterizedConstructor);
				}
				return _parameterizedCreator;
			}
		}

		/*[Nullable(new byte[] { 2, 1 })]*/
		public ObjectConstructor<object> OverrideCreator
		{
			/*[return: Nullable(new byte[] { 2, 1 })]*/
			get
			{
				return _overrideCreator;
			}
			/*[param: Nullable(new byte[] { 2, 1 })]*/
			set
			{
				_overrideCreator = value;
			}
		}

		public bool HasParameterizedCreator { get; set; }

		internal bool HasParameterizedCreatorInternal
		{
			get
			{
				if (!HasParameterizedCreator && _parameterizedCreator == null)
				{
					return _parameterizedConstructor != null;
				}
				return true;
			}
		}

		/*[NullableContext(1)]*/
		public JsonDictionaryContract(Type underlyingType)
			: base(underlyingType)
		{
			ContractType = JsonContractType.Dictionary;
			Type keyType;
			Type valueType;
			if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IDictionary<, >), out _genericCollectionDefinitionType))
			{
				keyType = _genericCollectionDefinitionType.GetGenericArguments()[0];
				valueType = _genericCollectionDefinitionType.GetGenericArguments()[1];
				if (ReflectionUtils.IsGenericDefinition(UnderlyingType, typeof(IDictionary<, >)))
				{
					CreatedType = typeof(Dictionary<, >).MakeGenericType(keyType, valueType);
				}
				else if (underlyingType.IsGenericType() && underlyingType.GetGenericTypeDefinition().FullName == "System.Collections.Concurrent.ConcurrentDictionary`2")
				{
					ShouldCreateWrapper = true;
				}
				IsReadOnlyOrFixedSize = ReflectionUtils.InheritsGenericDefinition(underlyingType, typeof(ReadOnlyDictionary<, >));
			}
			else if (ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IReadOnlyDictionary<, >), out _genericCollectionDefinitionType))
			{
				keyType = _genericCollectionDefinitionType.GetGenericArguments()[0];
				valueType = _genericCollectionDefinitionType.GetGenericArguments()[1];
				if (ReflectionUtils.IsGenericDefinition(UnderlyingType, typeof(IReadOnlyDictionary<, >)))
				{
					CreatedType = typeof(ReadOnlyDictionary<, >).MakeGenericType(keyType, valueType);
				}
				IsReadOnlyOrFixedSize = true;
			}
			else
			{
				ReflectionUtils.GetDictionaryKeyValueTypes(UnderlyingType, out keyType, out valueType);
				if (UnderlyingType == typeof(IDictionary))
				{
					CreatedType = typeof(Dictionary<object, object>);
				}
			}
			if (keyType != null && valueType != null)
			{
				_parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(CreatedType, typeof(KeyValuePair<, >).MakeGenericType(keyType, valueType), typeof(IDictionary<, >).MakeGenericType(keyType, valueType));
				if (!HasParameterizedCreatorInternal && underlyingType.Name == "FSharpMap`2")
				{
					FSharpUtils.EnsureInitialized(underlyingType.Assembly());
					_parameterizedCreator = FSharpUtils.Instance.CreateMap(keyType, valueType);
				}
			}
			if (!typeof(IDictionary).IsAssignableFrom(CreatedType))
			{
				ShouldCreateWrapper = true;
			}
			DictionaryKeyType = keyType;
			DictionaryValueType = valueType;
			if (DictionaryKeyType != null && DictionaryValueType != null && ImmutableCollectionsUtils.TryBuildImmutableForDictionaryContract(underlyingType, DictionaryKeyType, DictionaryValueType, out var createdType, out var parameterizedCreator))
			{
				CreatedType = createdType;
				_parameterizedCreator = parameterizedCreator;
				IsReadOnlyOrFixedSize = true;
			}
		}

		/*[NullableContext(1)]*/
		internal IWrappedDictionary CreateWrapper(object dictionary)
		{
			if (_genericWrapperCreator == null)
			{
				_genericWrapperType = typeof(DictionaryWrapper<, >).MakeGenericType(DictionaryKeyType, DictionaryValueType);
				ConstructorInfo constructor = _genericWrapperType.GetConstructor(new Type[1] { _genericCollectionDefinitionType });
				_genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
			}
			return (IWrappedDictionary)_genericWrapperCreator(dictionary);
		}

		/*[NullableContext(1)]*/
		internal IDictionary CreateTemporaryDictionary()
		{
			if (_genericTemporaryDictionaryCreator == null)
			{
				Type type = typeof(Dictionary<, >).MakeGenericType(DictionaryKeyType ?? typeof(object), DictionaryValueType ?? typeof(object));
				_genericTemporaryDictionaryCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type);
			}
			return (IDictionary)_genericTemporaryDictionaryCreator();
		}
	}
}
