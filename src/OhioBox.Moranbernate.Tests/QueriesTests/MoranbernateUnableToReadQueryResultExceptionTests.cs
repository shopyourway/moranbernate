using System;
using System.Collections.Generic;
using System.Configuration;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Moranbernate.Tests.QueriesTests
{
	[TestFixture]
	[Category("integration")]
	public class MoranbernateUnableToReadQueryResultExceptionTests
	{
		[TestFixtureSetUp]
		public void FixtureSetUp()
		{
			using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
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

			MappingRepoDictionary.InitializeAssemblies(GetType().Assembly);
		}

		[Test]
		public void Ctor_WhenCalled_ExceptionWithMessageThatIncludesAllRelevantDataFromTable()
		{
			string message;

			using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySql"].ConnectionString))
			{
				conn.Open();

				var builder = new QueryBuilder<LocationDto>();
				var parameters = new List<object>();
				var sql = builder.Build(parameters);

				using (var command = conn.CreateCommand())
				{
					command.CommandText = sql;
					command.AttachPositionalParameters(parameters);

					using (var reader = command.ExecuteReader())
					{
						message = new MoranbernateUnableToReadQueryResultException<LocationDto>(sql, parameters, reader, conn,new Exception()).Message;
					}
				}
			}

			Assert.That(message, Is.StringStarting("\r\nT: LocationDto,\r\n T-Maps: Type: LocationDto,\r\nTable Name: `location`,\r\n Dialect Table Name: ``location``, \r\nIdentifiers: Zip, \r\nProperties: City,Longitude,Latitude,Extra,AnnoyingInterface,\r\n SQL: SELECT `Zip`, `City`, `Longitude`, `Latitude`, `Extra`, `AnnoyingInterface` FROM `location`;,\r\n Parameters: ,\r\n Mapping: CheckInDto,CompositeIdSample,CustomTypeObject,LocationDto,NotUpdateObject,SimpleObject,StamObject,\r\n Schema: Table name: location, Schema Name: moranbernate, ColumnsNames: Zip + type: System.String,City + type: System.String,Longitude + type: System.Double,Latitude + type: System.Double,Extra + type: System.String,AnnoyingInterface + type: System.String \r\n ConnectionString: server=localhost;port=3306;database=moranbernate;"));
		}
	}
}