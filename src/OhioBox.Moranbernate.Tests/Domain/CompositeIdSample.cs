using System;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Tests.Domain
{
	public class CompositeIdSample
	{
		public long Id1 { get; set; }
		public long Id2 { get; set; }
		
		public string SomeValue { get; set; }
		public DateTime SomeDate { get; set; }
	}

	public class CompositeIdSampleMap : ClassMap<CompositeIdSample>
	{
		public CompositeIdSampleMap()
		{
			Table("composite_id_table");

			CompositeId()
				.KeyProperty(x => x.Id1)
				.KeyProperty(x => x.Id2);

			Map(x => x.SomeValue);
			Map(x => x.SomeDate);
		}
	}
}