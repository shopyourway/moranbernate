using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Tests.Domain;
using NUnit.Framework;

namespace OhioBox.Moranbernate.Tests.MappingTests
{
	public class CustomTypeMappingTests
	{
		[Test]
		public void Sanity()
		{
			var customTypeObject = new CustomTypeObject();
			var property = MappingRepo<CustomTypeObject>.GetMap().Properties[0];

			property.SetValue(customTypeObject, "5,3");
			CollectionAssert.AreEqual(customTypeObject.SomeIntArray, new[] { 5, 3 });
			Assert.AreEqual(property.ValueAccessor(customTypeObject), "5,3");
		}

		[Test]
		public void Sanity2()
		{
			MappingRepoDictionary.InitializeAssemblies(GetType().Assembly);
			MappingRepoDictionary.InitializeAssemblies(GetType().Assembly);
			
			var customTypeObject = new CustomTypeObject();
			var property = MappingRepo<CustomTypeObject>.GetMap().Properties[0];

			property.SetValue(customTypeObject, "5,3");
			CollectionAssert.AreEqual(customTypeObject.SomeIntArray, new[] { 5, 3 });
			Assert.AreEqual(property.ValueAccessor(customTypeObject), "5,3");
		}
	}
}