using System;
using System.Collections.Generic;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Querying.Restrictions
{
	internal class IdRestriction<T> : IRestriction where T : class
	{
		private readonly Property _property;
		private readonly object _id;

		public IdRestriction(object id)
		{
			var identifiers = MappingRepo<T>.GetMap().Identifiers;
			if (identifiers.Count > 1)
				throw new Exception("Id restriction can be used only when the object Id is composed of a single field");

			_property = identifiers[0];
			_id = id;
		}

		public string Apply(List<object> parameters, IDialect dialect)
		{
			var sql = string.Format("{0} = {1}", _property.ColumnName, dialect.CreateParameter("p" + parameters.Count));
			parameters.Add(_id);
			return sql;
		}
	}
}