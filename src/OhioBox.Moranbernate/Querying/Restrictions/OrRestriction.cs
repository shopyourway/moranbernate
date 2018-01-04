using System;
using System.Collections.Generic;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Querying.Restrictions
{
	internal class OrRestriction<T> : IRestriction
	{
		private readonly Func<IRestrictable<T>, IRestrictable<T>>[] _restrictions;

		public OrRestriction(Func<IRestrictable<T>, IRestrictable<T>>[] restrictions)
		{
			_restrictions = restrictions;
		}

		public string Apply(List<object> parameters, IDialect dialect)
		{
			var disjunction = new List<string>();

			foreach (var restriction in _restrictions)
			{
				var restrictable = new Restrictable<T>();
				restriction(restrictable);
				disjunction.Add(restrictable.BuildRestrictions(parameters, dialect));
			}
			
			return "(" + string.Join(" OR ", disjunction) + ")";
		}
	}
}