namespace OhioBox.Moranbernate.Logging
{
	public static class QueryLoggingRepo
	{
		private static IQueryLogger[] _queryLoggers;
		
		public static void InitializeLogger(IQueryLogger[] queryLoggers)
		{
			_queryLoggers = queryLoggers;
		}

		public static void LogQuery(QueryPerformanceLog queryLog)
		{
			if (_queryLoggers == null || _queryLoggers.Length == 0)
				return;

			foreach (var log in _queryLoggers)
			{

			}
		}
	}
}