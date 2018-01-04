using System;
using System.Collections.Generic;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying;

namespace OhioBox.Moranbernate.Generators
{
	internal class DeleteByQuery<T>
		where T : class
	{
		public string GetSql(Action<IRestrictable<T>> restriction, List<object> parameters)
		{
			var map = MappingRepo<T>.GetMap();

			var restrictable = new Restrictable<T>();
			restriction(restrictable);

			var where = restrictable.BuildRestrictionsIncludeWhere(parameters, map.Dialect);
			return "DELETE FROM " + map.TableName + where + ";";
		}
	}

	internal class CountByQuery<T>
		where T : class
	{
		public string GetSql(Action<IRestrictable<T>> restriction, List<object> parameters)
		{
			var map = MappingRepo<T>.GetMap();
			var baseSql = "SELECT COUNT(*) FROM " + map.TableName;

			if (restriction == null)
				return baseSql + ";";

			var restrictable = new Restrictable<T>();
			restriction(restrictable);

			var where = restrictable.BuildRestrictionsIncludeWhere(parameters, map.Dialect);
			return baseSql + where + ";";
		}
	}
}