using System.Collections.Generic;
using OhioBox.Moranbernate.Generators;
using OhioBox.Moranbernate.Tests.Domain;
using NUnit.Framework;
using OhioBox.Moranbernate.Querying;

namespace OhioBox.Moranbernate.Tests.GeneratorTests
{
	public class UpdateByQueryTests
	{
		[Test]
		public void Update_WhenSingleColumnSpecified_GeneratesSQL()
		{
			var parameters = new List<object>();
			var sql = new UpdateByQuery<SimpleObject>()
				.GetSql(
					b => b.Set(x => x.Long, 3),
					q => q.In(x => x.Id, new[] { 1L, 2, 3 }),
					parameters
				);

			Assert.AreEqual("UPDATE `table_name` SET `LongColumnName` = ?p0 WHERE (`Id` IN (?p1,?p2,?p3));", sql);
			Assert.AreEqual(parameters[0], 3);
		}

		[Test]
		public void Update_CustomMapping_WhenSingleColumnSpecified_GeneratesSQL()
		{
			var parameters = new List<object>();
			var sql = new UpdateByQuery<CustomTypeObject>()
				.GetSql(
					b => b.Set(x => x.SomeIntArray, new[] { 1, 2, 3 }),
					q => q.Equal(x => x.Id, 1),
					parameters
				);

			Assert.AreEqual("UPDATE `table_name` SET `SomeIntArray` = ?p0 WHERE (`Id` = ?p1);", sql);
			Assert.AreEqual(parameters[0], "1,2,3");
		}


		[Test]
		public void Update_WhenColumnIsReadyOnly_Throws()
		{
			Assert.Throws<UpdateByQueryException>(() => new UpdateByQuery<SimpleObject>()
				.GetSql(
					b => b.Set(x => x.Id, 3),
					q => q.In(x => x.Id, new[] {1L, 2, 3}),
					new List<object>()
				)
			);
		}

		[Test]
		public void Update_WhenUpdateMultipleFields_GenerateUpdateQueryWithMutlipleSetStatements()
		{
			var parameters = new List<object>();
			var sql = new UpdateByQuery<SimpleObject>()
				.GetSql(
					b => b
						.Set(x => x.Long, 3)
						.Set(x => x.SomeString, "there are no strings on me"),
					q => q.In(x => x.Id, new[] { 1L, 2, 3 }),
					parameters
				);

			Assert.That(sql, Is.EqualTo("UPDATE `table_name` SET `LongColumnName` = ?p0, `SomeString` = ?p1 WHERE (`Id` IN (?p2,?p3,?p4));"));
			Assert.That(parameters[0], Is.EqualTo(3));
			Assert.That(parameters[1], Is.EqualTo("there are no strings on me"));
		}

		[Test]
		public void Update_WhenUseIncrement_GenerateIncrementQuery()
		{
			var parameters = new List<object>();
			var sql = new UpdateByQuery<SimpleObject>()
				.GetSql(
					b => b
						.Increment(x => x.Long, 3),
					q => q.In(x => x.Id, new[] { 1L, 2, 3 }),
					parameters
				);

			Assert.That(sql, Is.EqualTo("UPDATE `table_name` SET `LongColumnName` = IFNULL(`LongColumnName`,0) + ?p0 WHERE (`Id` IN (?p1,?p2,?p3));"));
			Assert.That(parameters, Is.EqualTo(new object[] { 3, 1, 2, 3 }));
		}

		[Test]
		public void Update_WhenUseIncrementWithSet_GenerateUpdateQueryAccordingly()
		{
			var parameters = new List<object>();
			var sql = new UpdateByQuery<SimpleObject>()
				.GetSql(
					b => b
						.Set(x => x.SomeString, "there are no strings on me")
						.Increment(x => x.Long, 3)
						.Set(x => x.NullableLong, 4),
					q => q.In(x => x.Id, new[] { 1L, 2, 3 }),
					parameters
				);

			Assert.That(sql, Is.EqualTo("UPDATE `table_name` SET `SomeString` = ?p0, `LongColumnName` = IFNULL(`LongColumnName`,0) + ?p1, `NullableLong` = ?p2 WHERE (`Id` IN (?p3,?p4,?p5));"));
			Assert.That(parameters, Is.EqualTo(new object[] { "there are no strings on me", 3, 4, 1, 2, 3 }));
		}

		[Test]
		public void Update_WhenDecrementingField_DecrementFieldValue()
		{
			var parameters = new List<object>();
			var sql = new UpdateByQuery<SimpleObject>()
				.GetSql(
					b => b
						.Decrement(x => x.Long, 3),
					q => q.In(x => x.Id, new[] { 1L, 2, 3 }),
					parameters
				);

			Assert.That(sql, Is.EqualTo("UPDATE `table_name` SET `LongColumnName` = IFNULL(`LongColumnName`,0) - ?p0 WHERE (`Id` IN (?p1,?p2,?p3));"));
			Assert.That(parameters[0], Is.EqualTo(3));
		}
	}
}