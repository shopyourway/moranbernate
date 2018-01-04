using System;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Tests.Domain;
using OhioBox.Moranbernate.Utils;
using NUnit.Framework;

namespace OhioBox.Moranbernate.Tests.MappingTests
{
	public class MappingRepoTests
	{
		[Test]
		public void InitializeAssemblies_BuildsMaps()
		{
			MappingRepoDictionary.InitializeAssemblies(GetType().Assembly);
			var map = MappingRepoDictionary.GetMap<SimpleObject>();
			Assert.IsNotNull(map);
		}
	}
}