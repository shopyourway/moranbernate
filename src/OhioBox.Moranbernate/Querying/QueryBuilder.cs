using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Moranbernate.Querying
{
	public interface IQueryBuilder<T>
	{
		IQueryBuilder<T> Where(Action<IRestrictable<T>> where);
		IQueryBuilder<T> Take(int limit);
		IQueryBuilder<T> Skip(int amount);
		IQueryBuilder<T> Select(params Expression<Func<T, object>>[] fields);
		IQueryBuilder<T> OrderBy(Expression<Func<T, object>> expression, bool ascending);
		IQueryBuilder<T> DistinctBy(params Expression<Func<T, object>>[] fields);
	}

	internal class QueryBuilder<T> : IQueryBuilder<T>
		where T : class
	{
		private readonly ClassMap<T> _map;
		private Restrictable<T> _restrictable;

		public QueryBuilder()
		{
			_map = MappingRepo<T>.GetMap();
		}

		public IQueryBuilder<T> Where(Action<IRestrictable<T>> where)
		{
			if (_restrictable == null)
				_restrictable = new Restrictable<T>();

			where(_restrictable);

			return this;
		}

		public IQueryBuilder<T> Take(int limit)
		{
			if (limit <= 0)
				throw new ArgumentException("Zero or negative number (" + limit + ") cannot be used for Take in a query", "limit");

			_maxResults = limit;
			return this;
		}

		public IQueryBuilder<T> Skip(int amount)
		{
			if (amount < 0)
				throw new ArgumentException("Negative number (" + amount + ") cannot be used for Skip in a query", "amount");

			_firstResult = amount;
			return this;
		}

		private IList<KeyValuePair<Property, bool>> _orderBy;

		public IQueryBuilder<T> OrderBy(Expression<Func<T, object>> expression, bool ascending)
		{
			if (_orderBy == null)
				_orderBy = new List<KeyValuePair<Property, bool>>();

			_orderBy.Add(new KeyValuePair<Property, bool>(
				ExpressionProcessor<T>.GetPropertyFromCache(expression),
				ascending
			));

			return this;
		}

		private int _maxResults;
		private int _firstResult;
		private bool _distinct;
		private bool _rowCount;

		public IQueryBuilder<T> DistinctBy(params Expression<Func<T, object>>[] fields)
		{
			_distinct = true;
			Select(fields);
			return this;
		}

		public IQueryBuilder<T> RowCount()
		{
			_rowCount = true;
			return this;
		}

		private List<Property> _projectedColumns;
		public IQueryBuilder<T> Select(params Expression<Func<T, object>>[] fields)
		{
			if (_projectedColumns == null)
				_projectedColumns = new List<Property>();

			_projectedColumns.AddRange(fields.Select(ExpressionProcessor<T>.GetPropertyFromCache));
			return this;
		}

		private List<Property> _properties;
		public Property[] Properties {  get { return _properties.ToArray(); } }

		public string Build(List<object> parameters)
		{
			_properties = _projectedColumns ?? _map.Identifiers.Concat(_map.Properties).ToList();
			var columns = _properties.Select(x => x.ColumnName);

			var sb = new StringBuilder();
			var sql = sb
				.Append("SELECT ");

			if (_distinct)
				sql.Append("DISTINCT ");

			var columnsSql = string.Join(", ", columns);
			sql.Append(columnsSql);

			if (_rowCount)
				sql.Append(", COUNT(*) `rowcount`");

			sql
				.Append(" FROM ")
				.Append(_map.TableName);

			sql.Append(_restrictable != null ? _restrictable.BuildRestrictionsIncludeWhere(parameters, _map.Dialect) : null);

			if (_rowCount)
				sql.Append(" GROUP BY ").Append(columnsSql);

			if (_orderBy != null)
				sql
					.Append(" ORDER BY ")
					.Append(string.Join(", ", _orderBy.Select(x => x.Key.ColumnName + (x.Value ? " ASC" : " DESC"))));
			else
				if (_rowCount)
					sql.Append(" ORDER BY `rowcount` DESC");

			if (_maxResults != 0)
				sql.Append(" LIMIT ").Append(_firstResult).Append(",").Append(_maxResults);

			return sql.Append(";").ToString();
		}
	}
}