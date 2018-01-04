using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Moranbernate.Generators
{
	internal class PartialUpdateGenerator<T> : UpdateGenerator<T>
		where T : class
	{
		public PartialUpdateGenerator(Expression<Func<T, object>>[] properties)
			: base(GetProperties(properties))
		{
		}

		private static IList<Property> GetProperties(IEnumerable<Expression<Func<T, object>>> properties)
		{
			MappingRepo<T>.GetMap();
			return properties.Select(ExpressionProcessor<T>.GetPropertyFromCache).ToArray();
		}
	}
}