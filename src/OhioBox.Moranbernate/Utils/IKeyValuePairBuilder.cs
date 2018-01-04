using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Utils
{
	public interface IKeyValuePairBuilder<T>
	{
		IKeyValuePairBuilder<T> Set<TValue>(Expression<Func<T, TValue>> expression, TValue value);
	}

	internal class KeyValuePairBuilder<T> : IKeyValuePairBuilder<T>
	{
		private readonly List<Tuple<Property, object>> _list = new List<Tuple<Property, object>>();

		public IKeyValuePairBuilder<T> Set<TValue>(Expression<Func<T, TValue>> expression, TValue value)
		{
			var tuple = Tuple.Create(ExpressionProcessor<T>.GetPropertyFromCache(expression), (object)value);
			_list.Add(tuple);
			return this;
		}

		public IEnumerable<Tuple<Property, object>> GetEnumerable()
		{
			return _list;
		} 
	}
}