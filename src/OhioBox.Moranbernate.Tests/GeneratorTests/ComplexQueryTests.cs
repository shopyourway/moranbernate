using System;
using System.Collections.Generic;
using OhioBox.Moranbernate.Querying;
using OhioBox.Moranbernate.Tests.Domain;
using NUnit.Framework;

namespace OhioBox.Moranbernate.Tests.GeneratorTests
{
	[TestFixture]
	public class ComplexQueryTests
	{
		[Test]
		public void BuildQuery()
		{
			var parameters = new List<object>();
			var query = new QueryBuilder<SimpleObject>();

			query
					.Where(w => w
						.Equal(x => x.Id, 5)
						.Or(
							q => q.Equal(x => x.Id, 5),
							q => q.Equal(x => x.Id, 5),
							q => q
								.Equal(x => x.Id, 5)
								.Equal(x => x.Long, 7),
							q => q.Equal(x => x.Id, 5)
						)
					)
					.Select(x => x.Id, x => x.Guid, x => x.NullableGuid, x => x.Long)
					.OrderBy(x => x.Id)
					.OrderByDescending(x => x.Guid)
					.Skip(40)
					.Take(20);

			var sql = query.Build(parameters);

			Assert.AreEqual("SELECT `Id`, `Guid`, `NullableGuid`, `LongColumnName` FROM `table_name` WHERE (`Id` = ?p0 AND ((`Id` = ?p1) OR (`Id` = ?p2) OR (`Id` = ?p3 AND `LongColumnName` = ?p4) OR (`Id` = ?p5))) ORDER BY `Id` ASC, `Guid` DESC LIMIT 40,20;", sql);
		}

		[Test]
		public void QueryBuilder_Build_Contains()
		{
			const string param = "Hello";
			var parameters = new List<object>();
			var query = new QueryBuilder<SimpleObject>();

			query.Where(w => w.Contains(x => x.SomeString, param));

			var sql = query.Build(parameters);

			Assert.AreEqual("SELECT `Id`, `LongColumnName`, `SomeString`, `NullableLong`, `Guid`, `NullableGuid`, `IntBasedEnum`, `NullableIntBasedEnum` FROM `table_name` WHERE (`SomeString` LIKE ?p0);", sql);
			Assert.That(parameters[0], Is.EqualTo("%" + param + "%"));
		}

		[Test]
		public void QueryBuilder_Build_StartWith()
		{
			const string param = "Hello";
			var parameters = new List<object>();
			var query = new QueryBuilder<SimpleObject>();

			query.Where(w => w.StartWith(x => x.SomeString, param));

			var sql = query.Build(parameters);

			Assert.AreEqual("SELECT `Id`, `LongColumnName`, `SomeString`, `NullableLong`, `Guid`, `NullableGuid`, `IntBasedEnum`, `NullableIntBasedEnum` FROM `table_name` WHERE (`SomeString` LIKE ?p0);", sql);
			Assert.That(parameters[0], Is.EqualTo(param + "%"));
		}

		[Test]
		public void QueryBuilder_Build_EndWith()
		{
			const string param = "Hello";
			var parameters = new List<object>();
			var query = new QueryBuilder<SimpleObject>();

			query.Where(w => w.EndWith(x => x.SomeString, param));

			var sql = query.Build(parameters);

			Assert.AreEqual("SELECT `Id`, `LongColumnName`, `SomeString`, `NullableLong`, `Guid`, `NullableGuid`, `IntBasedEnum`, `NullableIntBasedEnum` FROM `table_name` WHERE (`SomeString` LIKE ?p0);", sql);
			Assert.That(parameters[0], Is.EqualTo("%" + param));
		}

		[Test]
		public void TestPaging_FirstPage()
		{
			var parameters = new List<object>();
			var query = new QueryBuilder<SimpleObject>();

			query
					.OrderByDescending(x => x.Id)
					.Skip(0)
					.Take(20);

			var sql = query.Build(parameters);

			Assert.AreEqual("SELECT `Id`, `LongColumnName`, `SomeString`, `NullableLong`, `Guid`, `NullableGuid`, `IntBasedEnum`, `NullableIntBasedEnum` FROM `table_name` ORDER BY `Id` DESC LIMIT 0,20;", sql);
		}

		[Test]
		public void TestPaging_DeepPage()
		{
			var parameters = new List<object>();
			var query = new QueryBuilder<SimpleObject>();

			query
					.OrderByDescending(x => x.Id)
					.Skip(60)
					.Take(20);

			var sql = query.Build(parameters);

			Assert.AreEqual("SELECT `Id`, `LongColumnName`, `SomeString`, `NullableLong`, `Guid`, `NullableGuid`, `IntBasedEnum`, `NullableIntBasedEnum` FROM `table_name` ORDER BY `Id` DESC LIMIT 60,20;", sql);
		}

		[Test]
		public void Distinct()
		{
			var parameters = new List<object>();
			var query = new QueryBuilder<SimpleObject>();

			query
				.Where(w => w
					.In(c => c.Long, new[] { 1L })
				)
				.DistinctBy(x => x.SomeString);

			var sql = query.Build(parameters);
			Assert.AreEqual("SELECT DISTINCT `SomeString` FROM `table_name` WHERE (`LongColumnName` IN (?p0));", sql);
		}
	}

	public class LikeTestCasesSource
	{
		public Action<IRestrictable<SimpleObject>>[] LikeActions { get; private set; }

		public static string Value { get { return "Hello"; } }

		public LikeTestCasesSource()
		{
			LikeActions = new Action<IRestrictable<SimpleObject>>[4];
			LikeActions[0] = t => t.Contains(x => x.SomeString, Value);
		}
	}
}