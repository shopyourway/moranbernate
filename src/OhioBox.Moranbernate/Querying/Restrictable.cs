using System.Collections.Generic;
using System.Linq;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying.Restrictions;

namespace OhioBox.Moranbernate.Querying
{
	public interface IRestrictable<T>
	{
		IRestrictable<T> AddRestriction(IRestriction restriction);
		bool NoRestrictions();
	}

	internal class Restrictable<T> : IRestrictable<T>
	{
		private readonly IList<IRestriction> _restrictions = new List<IRestriction>();

		public IRestrictable<T> AddRestriction(IRestriction restriction)
		{
			_restrictions.Add(restriction);
			return this;
		}

		public bool NoRestrictions()
		{
			return _restrictions == null || _restrictions.Count == 0;
		}

		internal string BuildRestrictions(List<object> parameters, IDialect map)
		{
			if (_restrictions == null || _restrictions.Count == 0)
				return null;

			var junction = _restrictions.Select(r => r.Apply(parameters, map));

			return "(" + string.Join(" AND ", junction) + ")";
		}

		internal string BuildRestrictionsIncludeWhere(List<object> parameters, IDialect map)
		{
			var where = BuildRestrictions(parameters, map);
			return where != null ? " WHERE " + where : "";
		}
	}
}