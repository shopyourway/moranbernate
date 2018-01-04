using System.Collections.Generic;
using System.Linq;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Moranbernate.Generators
{
	internal class BulkUpsert<T>
		where T : class
	{
		private readonly ClassMap<T> _map;

		public BulkUpsert()
		{
			_map = MappingRepo<T>.GetMap();
		}

		public string GetSql(int numberOfRows)
		{
			return InsertHelper.GetUpsertSql(_map, columns => CreateValues(columns, numberOfRows));
		}

		private string CreateValues(string[] columns, int numberOfRows)
		{
			return string.Join(", ", Enumerable.Range(0, numberOfRows)
				.Select(i => CreateSingleValue(columns.Length, i)));
		}

		private string CreateSingleValue(int numberOfColumns, int index)
		{
			var values = Enumerable
				.Range(0, numberOfColumns)
				.Select(i => _map.CreateParameter("p" + (index * numberOfColumns + i)));
			return $"({string.Join(", ", values)})";
		}

		public IEnumerable<Property> GetColumns()
		{
			return InsertHelper.GetColumns(_map);
		}
	}
}