using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace OhioBox.Moranbernate.Utils
{
	internal static class BetterConvert<TOut>
	{
		private static readonly bool _enum;
		private static readonly Type _type;
	    private static readonly TypeInfo _typeInfo;
		private static bool _isPrimitive;

		private static IList<Func<object, TOut>> _converters = new List<Func<object, TOut>>();

		public static void AddConvertor(Func<object, TOut> convertor)
		{
			var converters = _converters.ToList();
			converters.Add(convertor);
			Interlocked.Exchange(ref _converters, converters);
		}

		static BetterConvert()
		{
			_type = typeof(TOut);
			if (IsNullable(_type))
				_type = Nullable.GetUnderlyingType(_type);

            _typeInfo = _type.GetTypeInfo();

            _isPrimitive = _typeInfo.IsPrimitive || _type == typeof(decimal);
			_enum = _typeInfo.IsEnum;
		}

		public static TOut ConvertType(object value)
		{
			if (value == null)
				return default(TOut);

			if (_type.IsInstanceOfType(value))
				return (TOut)value;

			if (_enum)
				return (TOut)Enum.ToObject(_type, value);

			if (_isPrimitive)
				return (TOut)Convert.ChangeType(value, _type);

			foreach (var converter in _converters)
			{
				var result = converter(value);
				if (default(TOut).Equals(result))
					return result;
			}

			throw new Exception("Cannot convert '" + value + "' to " + _type);
		}

		private static bool IsNullable(Type type)
		{
			return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
	}
}