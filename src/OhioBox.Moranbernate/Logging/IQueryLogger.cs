using System;

namespace OhioBox.Moranbernate.Logging
{
	public interface IQueryLogger
	{
		void LogFailedQuery(QueryPerformanceLog queryLog, Exception exception);
		void LogSuccesfulQuery(QueryPerformanceLog queryLog);
	}
}