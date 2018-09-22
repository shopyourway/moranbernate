using System.Reflection;
using OhioBox.Moranbernate.Logging;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Utils
{
	public class MoranbernateInitializer
	{
		public static void Initialize(Assembly[] assemblies, IQueryLogger[] queryLoggers)
		{
			MappingRepoDictionary.InitializeAssemblies(assemblies);
			QueryLoggingRepo.InitializeLogger(queryLoggers);
		}
	}
}