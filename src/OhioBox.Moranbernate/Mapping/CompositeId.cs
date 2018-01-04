using System;
using System.Linq.Expressions;

namespace OhioBox.Moranbernate.Mapping
{
	public class CompositeId<T>
		where T : class
	{
		private ClassMap<T> ClassMap { get; set; }

		public CompositeId(ClassMap<T> classMap)
		{
			ClassMap = classMap;
		}

		public CompositeId<T> KeyProperty<TOut>(Expression<Func<T, TOut>> expression, string columnName = null)
		{
			ClassMap.Id(expression, columnName).GeneratedBy.Assigned();
			return this;
		}
	}
}