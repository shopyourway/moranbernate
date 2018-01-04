using System;
using System.Collections.Generic;
using System.Linq;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Utils
{
	internal static class InsertHelper
	{
		public static IEnumerable<Property> GetColumns<T>(ClassMap<T> map) where T : class
		{
			var columns = map
				.Identifiers
				.Concat(map.Properties ?? new Property[0])
				.Where(x => x.Insert && !x.ReadOnly)
				.ToArray();

			return columns;
		}

		private static string GetQuery(string tableName, IEnumerable<string> columns, string valuesText)
		{
			return $"INSERT INTO {tableName}({string.Join(", ", columns)}) VALUES {valuesText};";
		}

		public static string GetInsertSql<T>(ClassMap<T> map, Func<string[], string> valuesTextCreator) where T : class
		{
			var columns = GetColumns(map)
				.Select(x => x.ColumnName)
				.ToArray();

			var valuesText = valuesTextCreator.Invoke(columns);

			return GetQuery(map.TableName, columns, valuesText);
		}

		private static string GetUpsertQuery(string tableName, IEnumerable<string> columns, string valuesText)
		{
			return $"REPLACE INTO {tableName}({string.Join(", ", columns)}) VALUES {valuesText};";
		}

		public static string GetUpsertSql<T>(ClassMap<T> map, Func<string[], string> valuesTextCreator) where T : class
		{
			var columns = GetColumns(map)
				.Select(x => x.ColumnName)
				.ToArray();

			var valuesText = valuesTextCreator.Invoke(columns);

			return GetUpsertQuery(map.TableName, columns, valuesText);
		}
	}

}