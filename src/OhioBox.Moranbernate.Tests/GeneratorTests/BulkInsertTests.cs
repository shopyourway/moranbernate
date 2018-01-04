using System.Linq;
using OhioBox.Moranbernate.Generators;
using OhioBox.Moranbernate.Tests.Domain;
using NUnit.Framework;

namespace OhioBox.Moranbernate.Tests.GeneratorTests
{
	[TestFixture]
	public class BulkInsertTests
	{
		[Test]
		public void GetSql_WhenSingleColumnSpecified_GeneratesSQL()
		{
			var sql = new BulkInsert<SimpleObject>()
				.GetSql(1);

			Assert.AreEqual(
				"INSERT INTO `table_name`(`LongColumnName`, `SomeString`, `NullableLong`, `Guid`, `NullableGuid`, `IntBasedEnum`, `NullableIntBasedEnum`) VALUES (?p0, ?p1, ?p2, ?p3, ?p4, ?p5, ?p6);",
				sql);
		}

		[Test]
		public void GetSql_WhenTwoColumnsSpecified_GeneratesSQL()
		{
			var sql = new BulkInsert<SimpleObject>()
				.GetSql(2);

			Assert.AreEqual(
				"INSERT INTO `table_name`(`LongColumnName`, `SomeString`, `NullableLong`, `Guid`, `NullableGuid`, `IntBasedEnum`, `NullableIntBasedEnum`) VALUES (?p0, ?p1, ?p2, ?p3, ?p4, ?p5, ?p6), (?p7, ?p8, ?p9, ?p10, ?p11, ?p12, ?p13);",
				sql);
		}

		[Test]
		public void GetSql_ZeroEntries_GeneratesSQL()
		{
			var sql = new BulkInsert<SimpleObject>()
				.GetSql(0);

			Assert.AreEqual(
				"INSERT INTO `table_name`(`LongColumnName`, `SomeString`, `NullableLong`, `Guid`, `NullableGuid`, `IntBasedEnum`, `NullableIntBasedEnum`) VALUES ;",
				sql);
		}

		[Test]
		public void GetColumns_SimpleObject_ShouldContainSimpleObjectsMappedColumns()
		{
			var columns = new BulkInsert<SimpleObject>().GetColumns();

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