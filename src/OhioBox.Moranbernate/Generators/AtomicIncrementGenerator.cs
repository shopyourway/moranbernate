using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Moranbernate.Generators
{
	internal class AtomicIncrementGenerator<T>
		where T : class
	{
		private readonly ClassMap<T> _map;

		public AtomicIncrementGenerator()
		{
			_map = MappingRepo<T>.GetMap();
		}

		public string GetSql(Action<IRestrictable<T>> restriction, Expression<Func<T, object>> accessor, int amount, List<object> parameters)
		{
			var restrictable = new Restrictable<T>();
			restriction(restrictable);
			var where = restrictable.BuildRestrictionsIncludeWhere(parameters, _map.Dialect);

			var property = ExpressionProcessor<T>.GetPropertyFromCache(accessor);

			if (property.Update)
				throw new MoranbernateMappingException("Can't use property " + property.Name + " for atomic increment as it is not defined as Readonly");

			return string.Format("UPDATE {0} SET {1} = {1} + {2}{3};", _map.TableName, property.Name, amount, where);
		}

		public IEnumerable<Property> GetColumns()
		{
			return _map.Identifiers;
		}
	}
}