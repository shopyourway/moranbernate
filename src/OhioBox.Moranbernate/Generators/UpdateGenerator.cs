using System.Collections.Generic;
using System.Linq;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Generators
{
	internal class UpdateGenerator<T> : ISqlCommandGenerator
		where T : class
	{
		private readonly ClassMap<T> _map;
		private readonly IList<Property> _properties;

		public UpdateGenerator()
		{
			_map = MappingRepo<T>.GetMap();
			_properties = _map.Properties;
		}

		public UpdateGenerator(IList<Property> properties)
		{
			_map = MappingRepo<T>.GetMap();
			_properties = properties;
		}

		public string GetSql()
		{
			int[] i = { 0 };

			var columns = _properties
				.Where(x => !x.ReadOnly)
				.Select(x => x.ColumnName + " = " + _map.CreateParameter("p" + i[0]++));

			var ids = _map
				.Identifiers
				.Select(x => x.ColumnName + " = " + _map.CreateParameter("p" + i[0]++));

			return string.Format("UPDATE {0} SET {1} WHERE {2};", _map.TableName, string.Join(", ", columns), string.Join(" AND ", ids));
		}

		public IEnumerable<Property> GetColumns()
		{
			return _properties
				.Where(x => !x.ReadOnly)
				.Concat(_map.Identifiers);
		}
	}
}