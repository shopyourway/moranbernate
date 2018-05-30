using System.Collections.Generic;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying.Restrictions;
using OhioBox.Moranbernate.Tests.Domain;
using OhioBox.Moranbernate.Utils;
using NUnit.Framework;

namespace OhioBox.Moranbernate.Tests.GeneratorTests
{
	[TestFixture]
	public class RestrictionsTests
	{
		[Test]
		public void In()
		{
			var map = MappingRepo<SimpleObject>.GetMap();
			var property = ExpressionProcessor<SimpleObject>.GetPropertyFromCache(x => x.Long);

			var values = new[] {1L, 2, 3};
			var res = new InRestriction<long>(property, values);
			var parameters = new List<object>();
			var sql = res.Apply(parameters, map.Dialect);

			Assert.AreEqual("`LongColumnName` IN (?p0,?p1,?p2)", sql);
			CollectionAssert.AreEquivalent(values, parameters);
		}

		[Test]
		public void Equal()
		{
			var map = MappingRepo<SimpleObject>.GetMap();
			var property = ExpressionProcessor<SimpleObject>.GetPropertyFromCache(x => x.Long);

			var res = new OperatorRestriction<long>(property, 5L, "=");
			var parameters = new List<object>();
			var sql = res.Apply(parameters, map.Dialect);

			Assert.AreEqual("`LongColumnName` = ?p0", sql);
			Assert.AreEqual(5, parameters[0]);
		}

		[Test]
		public void Regex()
		{
			var map = MappingRepo<SimpleObject>.GetMap();
			var property = ExpressionProcessor<SimpleObject>.GetPropertyFromCache(x => x.Long);

			var res = new RegexRestriction<string>(property, "[a-zA-Z]+");
			var parameters = new List<object>();
			var sql = res.Apply(parameters, map.Dialect);

			Assert.AreEqual("`LongColumnName` REGEX ?p0", sql);
			Assert.AreEqual("[a-zA-Z]+", parameters[0]);
		}
	}
}