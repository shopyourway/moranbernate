using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Utils
{
	public interface IUpdateStatementBuilder<T>
	{
		IUpdateStatementBuilder<T> Set<TValue>(Expression<Func<T, TValue>> expression, TValue value);
		IUpdateStatementBuilder<T> Increment<TValue>(Expression<Func<T, TValue>> expression, TValue value);
		IUpdateStatementBuilder<T> Decrement<TValue>(Expression<Func<T, TValue>> expression, TValue value);

		bool HasNoStatements();
	}

	internal class UpdateStatementBuilder<T> : IUpdateStatementBuilder<T>
	{
		private readonly List<IPropertyToUpdate> _statements = new List<IPropertyToUpdate>();
		public IList<IPropertyToUpdate> GetEnumerable() => _statements;

		public IUpdateStatementBuilder<T> Set<TValue>(Expression<Func<T, TValue>> expression, TValue value)
		{
			var settableProperty = new PropertyToSet(ExpressionProcessor<T>.GetPropertyFromCache(expression), value);
			_statements.Add(settableProperty);
			return this;
		}

		public IUpdateStatementBuilder<T> Increment<TValue>(Expression<Func<T, TValue>> expression, TValue value)
		{
			var incrementableProperty = new PropertyToIncrement(ExpressionProcessor<T>.GetPropertyFromCache(expression), value);
			_statements.Add(incrementableProperty);

			return this;
		}

		public IUpdateStatementBuilder<T> Decrement<TValue>(Expression<Func<T, TValue>> expression, TValue value)
		{
			var incrementableProperty = new PropertyToDecrement(ExpressionProcessor<T>.GetPropertyFromCache(expression), value);
			_statements.Add(incrementableProperty);

			return this;
		}

		public bool HasNoStatements()
		{
			return _statements.Count == 0;
		}
	}

	internal interface IPropertyToUpdate
	{
		Property Property { get; }
		string GetSql<T>(ClassMap<T> map, List<object> parameters) where T:class;
	}

	internal class PropertyToSet : IPropertyToUpdate
	{
		public Property Property { get; }
		private readonly object _value;

		public PropertyToSet(Property property, object value)
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

	internal class PropertyToIncrement : IPropertyToUpdate
	{
		public Property Property { get; }
		private readonly object _value;

		public PropertyToIncrement(Property property, object value)
		{
			Property = property;
			_value = value;
		}

		public string GetSql<T>(ClassMap<T> map, List<object> parameters) where T : class
		{
			var sql = $"{Property.ColumnName} = IFNULL({Property.ColumnName},0) + {map.CreateParameter("p" + parameters.Count)}";
			var value = Property.ConvertValue(_value);
			parameters.Add(value);
			return sql;
		}
	}

	internal class PropertyToDecrement : IPropertyToUpdate
	{
		public Property Property { get; }
		private readonly object _value;

		public PropertyToDecrement(Property property, object value)
		{
			Property = property;
			_value = value;
		}

		public string GetSql<T>(ClassMap<T> map, List<object> parameters) where T : class
		{
			var sql = $"{Property.ColumnName} = IFNULL({Property.ColumnName},0) - {map.CreateParameter("p" + parameters.Count)}";
			var value = Property.ConvertValue(_value);
			parameters.Add(value);
			return sql;
		}
	}
}