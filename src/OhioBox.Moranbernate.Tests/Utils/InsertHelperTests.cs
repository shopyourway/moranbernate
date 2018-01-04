using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OhioBox.Moranbernate.Tests.Domain;
using OhioBox.Moranbernate.Utils;
using NUnit.Framework;

namespace OhioBox.Moranbernate.Tests.Utils
{
	[TestFixture]
	public class InsertHelperTests
	{
		[Test]
		public void GetColumns_SimpleObject_ShouldContainSimpleObjectsMappedColumns()
		{
			var simpleObjectMap = new SimpleObjectMap();

			var columns = InsertHelper.GetColumns(simpleObjectMap);

			var columneNames = columns.Select(p => p.ColumnName);

			Assert.That(columneNames, Contains.Item("`LongColumnName`"));
			Assert.That(columneNames, Contains.Item("`SomeString`"));
			Assert.That(columneNames, Contains.Item("`NullableLong`"));
			Assert.That(columneNames, Contains.Item("`Guid`"));
			Assert.That(columneNames, Contains.Item("`NullableGuid`"));
			Assert.That(columneNames, Contains.Item("`IntBasedEnum`"));
			Assert.That(columneNames, Contains.Item("`NullableIntBasedEnum`"));
		}
	}
}