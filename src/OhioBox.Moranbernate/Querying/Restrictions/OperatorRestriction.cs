using System.Collections.Generic;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Querying.Restrictions
{
	internal class OperatorRestriction<T> : IRestriction
	{
		private readonly Property _property;
		private readonly T _value;
		private readonly string _operator;

		public OperatorRestriction(Property property, T value, string @operator)
		{
			_property = property;
			_value = value;
			_operator = @operator;
		}

		public string Apply(List<object> parameters, IDialect dialect)
		{
			var sql = string.Format("{0} {1} {2}", _property.ColumnName, _operator, dialect.CreateParameter("p" + parameters.Count));
			parameters.Add(_property.ToParameter(_value));
			return sql;
		}
	}
}