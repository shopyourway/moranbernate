using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using OhioBox.Moranbernate.Logging;

namespace OhioBox.Moranbernate.Querying
{
	public static class CommandRunner
	{
		public static int ExecuteCommandAndLog(IDbCommand command)
		{
			var stopWatch = new Stopwatch();
			try
			{
				stopWatch.Start();
				var rowsAffected = command.ExecuteNonQuery();
				stopWatch.Stop();
				var queryTime = stopWatch.ElapsedMilliseconds;

				LogQuery(command, queryTime);
				return rowsAffected;
			}
			catch (Exception e)
			{
				stopWatch.Stop();
				LogQuery(command, stopWatch.ElapsedMilliseconds, e);
				throw;
			}
		}

		private static void LogQuery(IDbCommand command, long queryTime, Exception e = null)
		{
			var query = command.CommandText;
			
			var parameters = ExtractParameterValues(command);

			var queryLog = new QueryPerformanceLog(query, parameters, queryTime);
			QueryLoggingRepo.LogQuery(queryLog, e);
		}

		private static string[] ExtractParameterValues(IDbCommand command)
		{
			var dbParameters = command.Parameters;

			if (dbParameters == null || dbParameters.Count == 0)
				return new string[0];

			var parameters = dbParameters
				.Cast<IDbDataParameter>()
				.Select(p => (p.Value != null) ? 
					p.Value.ToString() 
					:"null")
				.ToArray();
			
			return parameters;
		}
	}
}