using System;
using OhioBox.Moranbernate.Generators;
using OhioBox.Moranbernate.Tests.Domain;
using NUnit.Framework;

namespace OhioBox.Moranbernate.Tests.GeneratorTests
{
	public class ReadOnlyTests
	{
		[Test]
		public void Insert_Simple()
		{
			var generator = new InsertGenerator<NotUpdateObject>();
			Console.WriteLine(generator.GetSql());
			Assert.AreEqual("INSERT INTO (`Id`, `Long`) VALUES (?p0, ?p1);", generator.GetSql());
		} 
	}
}