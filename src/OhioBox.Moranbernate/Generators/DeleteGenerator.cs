using System.Collections.Generic;
using System.Linq;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Generators
{
	interface ISqlCommandGenerator
	{
		string GetSql();
		IEnumerable<Property> GetColumns();
	}

	internal class DeleteGenerator<T> : ISqlCommandGenerator
		where T : class
	{
		private readonly ClassMap<T> _map;

		public DeleteGenerator()
		{
			_map = MappingRepo<T>.GetMap();
		}

		public string GetSql()
		{
			var ids = _map
				.Identifiers
				.Select((x, i) => x.ColumnName + " = " + _map.CreateParameter("p" + i));

			return string.Format("DELETE FROM {0} WHERE {1};", _map.TableName, string.Join(" AND ", ids));
		}

		public IEnumerable<Property> GetColumns()
		{
			return _map.Identifiers;
		}
	}
}