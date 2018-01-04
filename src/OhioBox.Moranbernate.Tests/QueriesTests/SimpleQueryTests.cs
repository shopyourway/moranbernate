using System;
using System.Collections.Generic;
using System.Configuration;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying;
using OhioBox.Moranbernate.Tests.Domain;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using System.Linq;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Moranbernate.Tests.QueriesTests
{
	public static class Exts
	{
		public static IEnumerable<IEnumerable<T>> Batch<T>(
		this IEnumerable<T> source, int size)
		{
			T[] bucket = null;
			var count = 0;

			foreach (var item in source)
			{
				if (bucket == null)
					bucket = new T[size];

				bucket[count++] = item;

				if (count != size)
					continue;

				yield return bucket.Select(x => x);

				bucket = null;
				count = 0;
			}

			// Return the last bucket with all remaining elements
			if (bucket != null && count > 0)
				yield return bucket.Take(count);
		}
	}

	[TestFixture]
	[Category("integration")]
	public class SimpleQueryTests
	{
		[SetUp]
		public void FixtureSetUp()
		{
			using(var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
			{
				conn.Open();
				var command = conn.CreateCommand();
				command.CommandText =
					@"DROP TABLE IF EXISTS `location`;
					CREATE TABLE `location` (
					  `zip` varchar(20) NOT NULL,
					  `city` varchar(50),
					  `latitude` double,
					  `longitude` double,
					  `Extra` varchar(50),
					  `AnnoyingInterface` varchar(50),
					  PRIMARY KEY (`zip`)
					) ENGINE=InnoDB DEFAULT CHARSET=utf8;";
				command.ExecuteNonQuery();
			}
		}

		[Test]
		public void Sanity()
		{
			using(var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
			{
				conn.Open();

				var city = "Los Hermanos";
				conn.Insert(new LocationDto {
					City = city, 
					Latitude = 1.0, 
					Longitude = 3.0, 
					Zip = "12320",
					Extra = new[] { 10, 12 },
					AnnoyingInterface = new AnnoyingInterface()
				});

				var dto = conn.Query<LocationDto>(
						q => q.Where(w => w.Equal(x => x.Extra, new[] { 10, 12 }))
					).FirstOrDefault();

				Assert.That(dto.City, Is.EqualTo(city));
			}
		}

		[Test]
		public void BulkInsert()
		{
			var range = 10;
			var dtos = Enumerable.Range(0, range).Select(i => new LocationDto
			{
				City = i.ToString(),
				Latitude = 1.0,
				Longitude = 3.0,
				Zip = i.ToString(),
				Extra = new[] { 10, 12 },
				AnnoyingInterface = new AnnoyingInterface()
			}).ToArray();

			using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
			{
				conn.Open();
				conn.BulkInsert(dtos);

				for (int i = 0; i < range; i++)
				{
					var dto = conn.Query<LocationDto>(
						q => q.Where(w => w.Equal(x => x.Zip, i.ToString()))
						).FirstOrDefault();
					Assert.That(dto.City, Is.EqualTo(i.ToString()));
				}
			}
		}

		[Test]
		public void BulkUpsert()
		{
			var range = 10;
			var dtos = Enumerable.Range(0, range).Select(i => new LocationDto
			{
				City = i.ToString(),
				Latitude = 1.0,
				Longitude = 3.0,
				Zip = i.ToString(),
				Extra = new[] { 10, 12 },
				AnnoyingInterface = new AnnoyingInterface()
			}).ToArray();

			using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
			{
				conn.Open();
				conn.BulkUpsert(dtos.Take(5).ToArray());
				conn.BulkUpsert(dtos);

				for (int i = 0; i < range; i++)
				{
					var dto = conn.Query<LocationDto>(
						q => q.Where(w => w.Equal(x => x.Zip, i.ToString()))
						).FirstOrDefault();
					Assert.That(dto.City, Is.EqualTo(i.ToString()));
				}
			}
		}

		[Test]
		public void BulkInsertZeroEntries()
		{
			var dtos = new LocationDto[0];

			using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
			{
				conn.Open();
				conn.BulkInsert(dtos);

				Assert.That(conn.Count<LocationDto>(), Is.EqualTo(0));
			}
		}

		[Test]
		public void Upsert()
		{
			using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
			{
				conn.Open();
				var checkInDto = new CheckInDto {
					Id = 13,
					UserId = 1,
					FourSquareId = "asdasd",
					Message = "asdasd",
					PictureId = 5,
					StoreId = 123123,
					Time = DateTime.Now
				};
				conn.Upsert(checkInDto);
				Console.WriteLine(checkInDto.Id);
			}
		}

		[Test]
		public void UpdateByQuery()
		{
			using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
			{
				conn.Open();
				var checkInDto = new CheckInDto {
					Id = 0,
					UserId = 1,
					FourSquareId = "asdasd",
					Message = "asdasd",
					PictureId = 5,
					StoreId = 1,
					Time = DateTime.Now
				};
				conn.Upsert(checkInDto);
				Assert.That(checkInDto.StoreId, Is.EqualTo(1));

				conn.UpdateByQuery<CheckInDto>(
					b => b
						.Set(x => x.FourSquareId, "Shalom")
						.Set(x => x.StoreId, 15),
					q => q.Equal(x => x.PictureId, 5)
				);

				var dto = conn.GetById<CheckInDto>(checkInDto.Id);
				Assert.That(dto.StoreId, Is.EqualTo(15));
			}
		}
	}

	public class LocationDto
	{
		public string Zip { get; set; }
		public string City { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public int[] Extra { get; set; }
		public IAnnoyingInterface AnnoyingInterface { get; set; }

		public override string ToString()
		{
			return string.Join(", ", new object[] { Zip, City, Latitude, Longitude, Extra, AnnoyingInterface });
		}
	}

	public class LocationDtoMap : ClassMap<LocationDto>
	{
		public LocationDtoMap()
		{
			Table("location");

			Id(x => x.Zip).GeneratedBy.Assigned();
			Map(x => x.City);
			Map(x => x.Longitude);
			Map(x => x.Latitude);
			Map(x => x.Extra).CustomType(new CsvPersister());
			Map(x => x.AnnoyingInterface).CustomType(new AnnoyingInterfaceMapper());
		}
	}

	public class CheckInDto
	{
		public long Id { get; set; }
		public long UserId { get; set; }
		public long? StoreId { get; set; }
		public string FourSquareId { get; set; }

		public DateTime Time { get; set; }
		public string Message { get; set; }
		public long? PictureId { get; set; }
		public string Json { get; set; }
		public bool Private { get; set; }
	}

	public class CheckInDtoMap : ClassMap<CheckInDto>
	{
		public CheckInDtoMap()
		{
			Table("checkins");

			Id(x => x.Id, "CheckInId").GeneratedBy.AutoGenerated();

			Map(x => x.StoreId);
			Map(x => x.UserId);
			Map(x => x.FourSquareId);

			Map(x => x.Time);
			Map(x => x.Message);
			Map(x => x.PictureId);
			Map(x => x.Private);
			Map(x => x.Json);
		}
	}
}