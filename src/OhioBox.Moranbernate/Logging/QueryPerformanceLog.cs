namespace OhioBox.Moranbernate.Logging
{
	public class QueryPerformanceLog
	{
		private readonly string _query;
		private readonly string[] _parameters;
		private readonly double _miliseconds;

		public QueryPerformanceLog(string query, string[] parameters,double miliseconds)
		{
			_query = query;
			_parameters = parameters;
			_miliseconds = miliseconds;
		}
	}
}