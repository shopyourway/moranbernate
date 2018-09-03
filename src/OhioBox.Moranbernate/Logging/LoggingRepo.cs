namespace OhioBox.Moranbernate.Logging
{
	public static class LoggingRepo
	{
		private static ILogger[] _loggers;
		
		public static void InitializeLogger(ILogger[] loggers)
		{
			_loggers = loggers;
		}

		public static void LogQuery()
		{
			if (_loggers == null || _loggers.Length == 0)
				return;

			foreach (var log in _loggers)
			{
				
			}
		}
	}
}