using System.Collections.Generic;
using System.Linq;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying.Restrictions;

namespace OhioBox.Moranbernate.Querying
{
	public interface IRestrictable<T>
	{
		IRestrictable<T> AddRestriction(IRestriction restriction);
	}

	internal class Restrictable<T> : IRestrictable<T>
	{
		private IList<IRestriction> Restrictions { get; set; }

		public IRestrictable<T> AddRestriction(IRestriction restriction)
		{
			if (Restrictions == null)
				Restrictions = new List<IRestriction>();

			Restrictions.Add(restriction);
			return this;
		}

		internal string BuildRestrictions(List<object> parameters, IDialect map)
		{
			if (Restrictions == null || Restrictions.Count == 0)
				return null;

			var junction = Restrictions.Select(r => r.Apply(parameters, map));

			return "(" + string.Join(" AND ", junction) + ")";
		}

		internal string BuildRestrictionsIncludeWhere(List<object> parameters, IDialect map)
		{
			var where = BuildRestrictions(parameters, map);
			return where != null ? " WHERE " + where : "";
		}
	}
}