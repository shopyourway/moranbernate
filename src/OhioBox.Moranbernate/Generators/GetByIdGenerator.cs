using System.Collections.Generic;
using System.Linq;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Generators
{
	internal class GetByIdGenerator<T> : ISqlCommandGenerator
		where T : class
	{
		private readonly ClassMap<T> _map;

		public GetByIdGenerator()
		{
			_map = MappingRepo<T>.GetMap();
		}

		public string GetSql()
		{
			return string.Format("SELECT {0} FROM {1} WHERE {2} = {3};",
				string.Join(",", GetColumns().Select(x => x.ColumnName)),
				_map.TableName,
				_map.Identifiers[0].ColumnName,
				_map.CreateParameter("p0")
			);
		}

		public IEnumerable<Property> GetColumns()
		{
			return _map
				.Identifiers
				.Concat(_map.Properties);
		}
	}
}