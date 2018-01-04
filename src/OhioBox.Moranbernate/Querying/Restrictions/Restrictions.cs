using System.Collections.Generic;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Querying.Restrictions
{
	public interface IRestriction
	{
		string Apply(List<object> parameters, IDialect dialect);
	}
}