namespace OhioBox.Moranbernate.Logging
{
	public class QueryPerformanceLog
	{
		public readonly string Query;
		public readonly string[] Parameters;
		public readonly double MiliSeconds;

		public QueryPerformanceLog(string query, string[] parameters,double miliSeconds)
		{
			Query = query;
			Parameters = parameters;
			MiliSeconds = miliSeconds;
		}
	}
}