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

				logQuery(command, queryTime);
				return rowsAffected;
			}
			catch (Exception e)
			{
				stopWatch.Stop();
				logQuery(command, stopWatch.ElapsedMilliseconds, e);
				throw;
			}

		}

		private static void logQuery(IDbCommand command, long queryTime, Exception e = null)
		{
			var query = command.CommandText;
			var parameters = command.Parameters
				.Cast<IDbDataParameter>()
				.Select(p => p.Value.ToString())
				.ToArray();
			
			var queryLog = new QueryPerformanceLog(query, parameters, queryTime);
			
//			if (e != null)
//				QueryLoggingRepo.LogFailedQuery(queryLog, e);

			QueryLoggingRepo.LogQuery(queryLog);
		}
	}
}