using System;
using System.Collections.Generic;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Tests.Domain;
using NUnit.Framework;
using System.Linq;

namespace OhioBox.Moranbernate.Tests.MappingTests
{
	[TestFixture]
	public class MappingTests
	{
		[Test]
		public void TableName_ReflectedProperly()
		{
			var map = new SimpleObjectMap();
			Assert.That(map.TableName, Is.EqualTo("`table_name`"));
		}

		[Test]
		public void CompositeIds_TypesAndNamesSet()
		{
			var map = new CompositeIdSampleMap();

			var id1 = map.Identifiers.First(x => x.Name == "Id1");
			Assert.False(id1.ReadOnly);
			Assert.True(id1.Insert);

			var id2 = map.Identifiers.First(x => x.Name == "Id2");
			Assert.False(id2.ReadOnly);
			Assert.True(id2.Insert);
		}

		[Test]
		public void Id_TypesAndNamesSet()
		{
			var map = new SimpleObjectMap();
			AssertTypeOfPropertyIs(map.Identifiers, "Id", typeof(Int64));
			var id = map.Identifiers.First(x => x.Name == "Id");
			Assert.True(id.ReadOnly);
			Assert.False(id.Insert);
		}

		[Test]
		public void Properties_TypesAndNamesSet()
		{
			var map = new SimpleObjectMap();

			AssertTypeOfPropertyIs(map.Properties, "Long", typeof(Int64));
			AssertTypeOfPropertyIs(map.Properties, "SomeString", typeof(string));
			AssertTypeOfPropertyIs(map.Properties, "NullableLong", typeof(long?));
			AssertTypeOfPropertyIs(map.Properties, "Guid", typeof(Guid));
			AssertTypeOfPropertyIs(map.Properties, "NullableGuid", typeof(Guid?));
			AssertTypeOfPropertyIs(map.Properties, "IntBasedEnum", typeof(IntBasedEnum));
			AssertTypeOfPropertyIs(map.Properties, "NullableIntBasedEnum", typeof(IntBasedEnum?));
		}

		private static void AssertTypeOfPropertyIs(IList<Property> properties, string name, Type type)
		{
			var property = properties.First(x => x.Name == name);
			Assert.That(property.Type, Is.EqualTo(type));
		}
	}
}