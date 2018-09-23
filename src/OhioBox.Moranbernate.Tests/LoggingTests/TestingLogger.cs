using System;
using System.Collections.Generic;
using System.Linq;
using OhioBox.Moranbernate.Logging;

namespace OhioBox.Moranbernate.Tests.LoggingTests
{
	public class TestingLogger : IQueryLogger
	{
		public IList<string> Log = new List<string>();
		public void LogFailedQuery(QueryPerformanceLog queryLog, Exception exception)
		{
			Log.Add("Query: " + queryLog.Query);
			Log.Add("Values: " + string.Join(", ", queryLog.Parameters));
			Log.Add("Miliseconds: " + queryLog.MiliSeconds);
			Log.Add("Exception: " + exception);
		}

		public void LogSuccesfulQuery(QueryPerformanceLog queryLog)
		{
			Log.Add("Query: " + queryLog.Query);
			Log.Add("Values: " + string.Join(", ", queryLog.Parameters));
			Log.Add("Miliseconds: " + queryLog.MiliSeconds);
		}
	}
}