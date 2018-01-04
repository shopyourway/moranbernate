using System;
using System.Collections.Generic;
using System.Diagnostics;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying;
using OhioBox.Moranbernate.Tests.QueriesTests;
using System.Linq;

namespace OhioBox.Moranbernate.Tests
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var r = new[] { 1, 2 }.Skip(-5).ToArray();
			Console.WriteLine(r.Length);

			var map = MappingRepo<LocationDto>.GetMap();
			MeasureTime(TextBuildQuery);
		}

		private static void TextBuildQuery()
		{
			var q = new QueryBuilder<LocationDto>();
			q
				.Where(w => w.Equal(x => x.City, "LA").Equal(x => x.Zip, "LA"))
				.Select(x => x.City, x => x.Zip);

			q.RowCount();

			var sql = q.Build(new List<object>());
			//Console.WriteLine(sql);
		}

		private static void MeasureTime(Action action)
		{
			var sw = new Stopwatch();
			sw.Start();
			action();
			sw.Stop();
			Console.WriteLine("First time " + sw.ElapsedMilliseconds + " ms");
			Console.WriteLine();
			var stats = new List<TimeSpan>();

			for (var i = 0; i < 100000; i++)
			{
				sw.Restart();
				action();
				sw.Stop();
				stats.Add(sw.Elapsed);
			}

			stats.Sort();

			Console.WriteLine();
			Console.WriteLine(stats.Average(x => x.Ticks));

			const int bucket = 10;
			var size = stats.Count / bucket;
			for (var i = 1; i <= bucket; i++)
			{
				var time = stats[size*i - 1];
				Console.WriteLine(i + "\t->\t" + time.Ticks + "\t" + time.TotalMilliseconds);
			}
		}
	}
}