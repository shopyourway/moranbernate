using System.Collections.Generic;
using System.Data;

namespace OhioBox.Moranbernate.Utils
{
	internal static class CommandExt
	{
		public static IDbCommand AttachPositionalParameters(this IDbCommand command, IList<object> parameters)
		{
			var i = 0;
			foreach (var parameter in parameters)
			{
				var prm = command.CreateParameter();
				prm.ParameterName = "p" + i++;
				prm.Value = parameter;
				command.Parameters.Add(prm);
			}
			return command;
		}
	}
}