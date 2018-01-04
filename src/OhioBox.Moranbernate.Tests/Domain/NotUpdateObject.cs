using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Tests.Domain
{
	public class NotUpdateObject
	{
		public long Id { get; set; }
		public long Long { get; set; }
	}

	public class NotUpdateObjectMap : ClassMap<NotUpdateObject>
	{
		public NotUpdateObjectMap()
		{
			Id(x => x.Id).GeneratedBy.Assigned();
			Map(x => x.Long).NotUpdate();
		}
	}
}