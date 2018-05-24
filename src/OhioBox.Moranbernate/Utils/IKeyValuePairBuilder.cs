using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Utils
{
	public interface IKeyValuePairBuilder<T>
	{
		IKeyValuePairBuilder<T> Set<TValue>(Expression<Func<T, TValue>> expression, TValue value);
		IKeyValuePairBuilder<T> Increment<TValue>(Expression<Func<T, TValue>> expression, TValue value);
	}

	internal class KeyValuePairBuilder<T> : IKeyValuePairBuilder<T>
	{
		private readonly List<IUpdatedProperty> _list = new List<IUpdatedProperty>();
		
		public IKeyValuePairBuilder<T> Set<TValue>(Expression<Func<T, TValue>> expression, TValue value)
		{
			var settableProperty = new ValueUpdatedProperty(ExpressionProcessor<T>.GetPropertyFromCache(expression), value);
			_list.Add(settableProperty);
			return this;
		}

		public IKeyValuePairBuilder<T> Increment<TValue>(Expression<Func<T, TValue>> expression, TValue value)
		{
			var incrementableProperty = new ValueIncrementedProperty(ExpressionProcessor<T>.GetPropertyFromCache(expression), value);
			_list.Add(incrementableProperty);

			return this;
		}

		public IList<IUpdatedProperty> GetEnumerable() => _list;
	}

	internal interface IUpdatedProperty
	{
		Property Property { get; }
		string GetSql<T>(ClassMap<T> map, List<object> parameters) where T:class;
	}

	internal class ValueUpdatedProperty : IUpdatedProperty
	{
		public Property Property { get; }
		private readonly object _value;

		public ValueUpdatedProperty(Property property, object value)
		{
			Property = property;
			_value = value;
		}

		public string GetSql<T>(ClassMap<T> map, List<object> parameters) where T : class
		{
			var sql = $"{Property.ColumnName} = {map.CreateParameter("p" + parameters.Count)}";
			var value = Property.ConvertValue(_value);
			parameters.Add(value);
			return sql;
		}
	}

	internal class ValueIncrementedProperty : IUpdatedProperty
	{
		public Property Property { get; }
		private readonly object _value;

		public ValueIncrementedProperty(Property property, object value)
		{
			Property = property;
			_value = value;
		}

		public string GetSql<T>(ClassMap<T> map, List<object> parameters) where T : class
		{
			var sql = $"{Property.ColumnName} = {Property.ColumnName} + {map.CreateParameter("p" + parameters.Count)}";
			var value = Property.ConvertValue(_value);
			parameters.Add(value);
			return sql;
		}
	}
}