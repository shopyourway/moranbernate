namespace OhioBox.Moranbernate.Logging
{
	public class QueryPerforanceLog
	{
		private readonly string _query;
		private readonly double _miliseconds;
		private readonly string[] _params;

		public QueryPerforanceLog(string query, double miliseconds, string[] @params)
		{
			_miliseconds = miliseconds;
			_params = @params;
			_query = query;
		}
	}
}