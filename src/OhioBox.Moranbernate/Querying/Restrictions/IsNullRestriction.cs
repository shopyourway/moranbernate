using System.Collections.Generic;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Querying.Restrictions
{
	internal class IsNullRestriction : IRestriction
	{
		private readonly Property _property;
		private readonly bool _not;

		public IsNullRestriction(Property property, bool not = false)
		{
			_property = property;
			_not = not;
		}

		public string Apply(List<object> parameters, IDialect dialect)
		{
			var sql = string.Format("{0} IS {1}NULL", _property.ColumnName, _not ? "NOT " : " ");
			return sql;
		}
	}
}