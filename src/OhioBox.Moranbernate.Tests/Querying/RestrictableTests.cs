using System.Collections.Generic;
using NUnit.Framework;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying;
using OhioBox.Moranbernate.Querying.Restrictions;

namespace OhioBox.Moranbernate.Tests.Querying
{
	[TestFixture]
	public class RestrictableTests
	{
		private Restrictable<TestDto> _target;

		[SetUp]
		public void Setup()
		{
			_target = new Restrictable<TestDto>();
		}

		[Test]
		public void NoRestrictions_WhenNoRestrictionsAreAdded_ReturnsTrue()
		{
			var result = _target.NoRestrictions();

			Assert.That(result, Is.True);
		}

		[Test]
		public void NoRestrictions_WhenRestrictionsAreAdded_ReturnsFalse()
		{
			_target.AddRestriction(new TestRestriction());

			var result = _target.NoRestrictions();

			Assert.That(result, Is.False);
		}

		private class TestDto
		{
			public string Name { get; set; }
		}

		private class TestRestriction : IRestriction
		{
			public string Apply(List<object> parameters, IDialect dialect)
			{
				return "";
			}
		}
	}
}