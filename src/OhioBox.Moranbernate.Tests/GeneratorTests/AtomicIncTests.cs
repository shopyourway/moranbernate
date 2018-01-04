using System.Collections.Generic;
using OhioBox.Moranbernate.Generators;
using OhioBox.Moranbernate.Mapping;
using NUnit.Framework;
using OhioBox.Moranbernate.Querying;

namespace OhioBox.Moranbernate.Tests.GeneratorTests
{
	public class AtomicIncTests
	{
		[Test]
		public void AtomicIncrement()
		{
			var generator = new AtomicIncrementGenerator<StamObject>();
			var parameters = new List<object>();
			var sql = generator.GetSql(q => q.In(x => x.Id, new[] { 5L }), x => x.Count, 5, parameters);

			Assert.AreEqual("UPDATE `stams` SET Count = Count + 5 WHERE (`Id` IN (?p0));", sql);
		}
	}

	public class StamObject
	{
		public long Id { get; set; }
		public long Count { get; set; }
	}

	public class CompositeIdSampleMap : ClassMap<StamObject>
	{
		public CompositeIdSampleMap()
		{
			Table("stams");

			Id(x => x.Id);
			Map(x => x.Count).ReadOnly();
		}
	}

}