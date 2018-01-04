using System.Collections.Generic;
using System.Linq;
using System.Text;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Querying.Restrictions
{
	internal class InRestriction<TValue> : IRestriction
	{
		private readonly Property _property;
		private readonly ICollection<TValue> _values;
		private readonly bool _not;

		public InRestriction(Property property, ICollection<TValue> values, bool not = false)
		{
			_property = property;
			_values = values;
			_not = not;
		}

		public string Apply(List<object> parameters, IDialect dialect)
		{
			var commaDelimited = string.Join(",", _values
				.Select(x => {
					var index = parameters.Count;
					parameters.Add(_property.ToParameter(x));
					return dialect.CreateParameter("p" + index);
				}));

			return new StringBuilder()
				.Append(_property.ColumnName)
				.Append(_not ? " NOT " : "")
				.Append(" IN (")
				.Append(commaDelimited)
				.Append(")")
				.ToString();
		}
	}
}