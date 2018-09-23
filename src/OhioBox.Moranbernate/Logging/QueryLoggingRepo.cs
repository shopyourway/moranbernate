using System;

namespace OhioBox.Moranbernate.Logging
{
	public static class QueryLoggingRepo
	{
		private static IQueryLogger[] _queryLoggers;
		
		public static void InitializeLogger(IQueryLogger[] queryLoggers)
		{
			_queryLoggers = queryLoggers;
		}

		public static void LogQuery(QueryPerformanceLog queryLog, Exception e = null)
		{
			if (_queryLoggers == null || _queryLoggers.Length == 0 || queryLog == null)
				return;

			foreach (var logger in _queryLoggers)
			{
				if (e != null)
					logger.LogFailedQuery(queryLog, e);
				
				else
					logger.LogSuccesfulQuery(queryLog);
			}
		}
	}
}