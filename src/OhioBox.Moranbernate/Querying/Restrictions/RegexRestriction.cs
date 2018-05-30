using System.Collections.Generic;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Querying.Restrictions
{
	internal class RegexRestriction<T> : IRestriction
	{
		private readonly Property _property;
		private readonly T _value;

		public RegexRestriction(Property property, T value)
		{
			_property = property;
			_value = value;
		}

		public string Apply(List<object> parameters, IDialect dialect)
		{
			var sql = dialect.RegexMatch(_property.ColumnName, dialect.CreateParameter("p" + parameters.Count));
			parameters.Add(_property.ToParameter(_value));
			return sql;
		}
	}
}