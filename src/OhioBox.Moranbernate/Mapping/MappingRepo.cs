using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace OhioBox.Moranbernate.Mapping
{
	public static class MappingRepoDictionary
	{
		private static readonly ConcurrentDictionary<RuntimeTypeHandle, ClassMapBase> _mapDictionary = new ConcurrentDictionary<RuntimeTypeHandle, ClassMapBase>();

		public static void InitializeAssemblies(params Assembly[] assemblies)
		{
			var classMapBase = typeof(ClassMapBase).GetTypeInfo();

			var types = assemblies
				.SelectMany(x => x.ExportedTypes)
                .Select(x => x.GetTypeInfo())
				.Where(x => classMapBase.IsAssignableFrom(x.BaseType?.GetTypeInfo()));

			foreach (var type in types)
			{
				var local = type;
				var key = local.BaseType.GetTypeInfo().GenericTypeArguments[0].TypeHandle;
				_mapDictionary.GetOrAdd(key, k => (ClassMapBase) Activator.CreateInstance(local.AsType()));
			}
		}

		internal static ClassMap<T> GetMap<T>()
			where T : class
		{
			ClassMapBase map;
			if (_mapDictionary.TryGetValue(typeof(T).TypeHandle, out map))
				return (ClassMap<T>) map;

			return null;
		}

		public static Type[] GetMappedTypes()
		{
			if (_mapDictionary == null)
				throw new MoranbernateMappingException("You haven't initialized the MappingRepoDictionary");

			return _mapDictionary.Values.Select(x => x.Type).ToArray();
		}
	}

	public class MoranbernateMappingException : Exception
	{
		public MoranbernateMappingException(string message) : base(message)
		{
		}
	}

	internal static class MappingRepo<T>
		where T : class
	{
		private static readonly ClassMap<T> Map;

		static MappingRepo()
		{
			var map = MappingRepoDictionary.GetMap<T>();
			if (map != null)
			{
				Map = map;
				return;
			}

			var classMapBase = typeof(ClassMap<T>).GetTypeInfo();
			Map = typeof (T)
                .GetTypeInfo()
				.Assembly
				.ExportedTypes
				.Where(x => classMapBase.IsAssignableFrom(x.GetTypeInfo().BaseType?.GetTypeInfo()))
				.Select(x => (ClassMap<T>) Activator.CreateInstance(x))
				.FirstOrDefault();
		}

		public static ClassMap<T> GetMap()
		{
			return Map;
		}
	}
}