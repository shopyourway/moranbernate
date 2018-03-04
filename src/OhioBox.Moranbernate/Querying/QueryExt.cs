using System;
using System.Collections.Generic;
using System.Data;
using OhioBox.Moranbernate.Generators;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Utils;
using System.Linq;

namespace OhioBox.Moranbernate.Querying
{
	public static class QueryExt
	{
		public static T GetById<T>(this IDbConnection connection, object id)
			where T : class, new()
		{
			var generator = CrudOperator<T>.GetById;
			var parameters = new List<object> { id };
			var sql = generator.GetSql();
			return Run<T>(connection, sql, parameters, generator.GetColumns()).FirstOrDefault();
		}

		public static long Count<T>(this IDbConnection connection, Action<IRestrictable<T>> restrictions = null)
			where T : class, new()
		{
			var countByQuery = new CountByQuery<T>();

			var parameters = new List<object>();
			var sql = countByQuery.GetSql(restrictions, parameters);

			using (var command = connection.CreateCommand())
			{
				command.CommandText = sql;
				command.AttachPositionalParameters(parameters);
				return (long)command.ExecuteScalar();
			}
		}

		public static IEnumerable<T> Query<T>(this IDbConnection connection, Action<IQueryBuilder<T>> query = null)
			where T : class, new()
		{
			var builder = new QueryBuilder<T>();
			if (query != null)
				query(builder);

			var parameters = new List<object>();
			var sql = builder.Build(parameters);
			return Run<T>(connection, sql, parameters, builder.Properties);
		}

		public static IEnumerable<QueryResult<T>> QueryAggregated<T>(this IDbConnection connection, Action<IQueryBuilder<T>> query = null)
			where T : class, new()
		{
			var builder = new QueryBuilder<T>();
			if (query != null)
				query(builder);

			builder.RowCount();

			var parameters = new List<object>();
			var sql = builder.Build(parameters);

			using (var command = connection.CreateCommand())
			{
				command.CommandText = sql;
				command.AttachPositionalParameters(parameters);
				var properties = builder.Properties.ToArray();

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						var t = new T();
						var i = 0;

						foreach (var property in properties)
							property.SetValue(t, reader.GetValue(i++));

						var rowCount = reader.GetInt32(i);

						yield return new QueryResult<T> { Item = t, RowCount = rowCount };
					}
				}
			}
		}

		private static IEnumerable<T> Run<T>(IDbConnection connection, string sql, List<object> parameters, IEnumerable<Property> properties)
			where T : class, new()
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = sql;
				command.AttachPositionalParameters(parameters);
				properties = properties.ToArray();

				IDataReader reader;
				try
				{
					reader = command.ExecuteReader();
				}
				catch (Exception ex)
				{
					throw new MoranbernateQueryException(sql, parameters, ex);
				}

				using (reader)
				{
					while (reader.Read())
					{
						var t = new T();
						var i = 0;

						foreach (var property in properties)
						{
							try
							{
								property.SetValue(t, reader.GetValue(i++));
							}
							catch (Exception ex)
							{
								HandleSetValueException<T>(sql, parameters, reader, connection, ex);
							}
						}

						yield return t;
					}
				}
			}
		}

		private static void HandleSetValueException<T>(string sql, List<object> parameters, IDataReader reader, IDbConnection connection, Exception ex)
			where T : class, new()
		{
			Exception e;
			try
			{
				e = new MoranbernateUnableToReadQueryResultException<T>(sql, parameters, reader,connection, ex);
			}
			catch (Exception)
			{
				throw new Exception("Cannot Create MoranbernateUnableToReadQueryResultException", ex);
			}
			throw e;
		}
	}

	public class QueryResult<T>
	{
		public T Item { get; set; }
		public int RowCount { get; set; }
	}

	public class MoranbernateQueryException : Exception
	{
		public MoranbernateQueryException(string sql, List<object> parameters, Exception inner) : base(CreateMessage(sql, parameters), inner)
		{
			SQL = sql;
			Parameters = parameters;
		}

		protected List<object> Parameters { get; set; }
		protected string SQL { get; set; }

		private static string CreateMessage(string sql, List<object> parameters)
		{
			return "Unable to run query:\r\n" + sql + "\r\nParameters:\r\n" + string.Join("\r\n", parameters);
		}
	}

	public class MoranbernateUnableToReadQueryResultException<T> : Exception where T:class,new()
	{
		public MoranbernateUnableToReadQueryResultException(string sql, IList<object> parameters, IDataReader reader, IDbConnection connection, Exception ex) : base (CreateMessage(sql, parameters, reader, connection), ex)
		{}

		private static string CreateMessage(string sql, IList<object> parameters, IDataReader reader, IDbConnection connection)
		{
            var mapping = MappingRepoDictionary.GetMappedTypes();
            var schema = reader.GetSchemaTable();
            var rows = schema?.Rows;
            var tableName = string.Empty;
            var schemaName = string.Empty;

            var columnNameIndex = 0;
            var schemaIndex = 9;
            var tableNameIndex = 10;
            var typeIndex = 11;


            var columns = new List<string>();
            if (rows != null && schema.Columns.Count > 11)
            {
                foreach (DataRow row in rows)
                {
                    columns.Add($"{row.ItemArray[columnNameIndex]} + type: {row.ItemArray[typeIndex]}");
                }
                tableName = rows[0][tableNameIndex].ToString();
                schemaName = rows[0][schemaIndex].ToString();
            }

            var mappedNames = mapping.Select(x => x.Name).OrderBy(x => x).ToArray();
            var mapsMessage = MappingRepoDictionary.GetMap<T>().ToString();
            var mappingMessage = string.Join(",", mappedNames);
            var schemaMessage = $"Table name: {tableName}, Schema Name: {schemaName}";
            if (columns.Any())
            {
                schemaMessage += $", ColumnsNames: {string.Join(",", columns)}";
            }

            return $"\r\nT: {typeof(T).Name},\r\n T-Maps: {mapsMessage},\r\n SQL: {sql},\r\n Parameters: {string.Join(",", parameters)},\r\n Mapping: {mappingMessage},\r\n Schema: {schemaMessage} \r\n ConnectionString: {connection?.ConnectionString}";
        }
    }

}