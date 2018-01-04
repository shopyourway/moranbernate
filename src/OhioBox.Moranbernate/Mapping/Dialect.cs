namespace OhioBox.Moranbernate.Mapping
{
	public interface IDialect
	{
		string EscapeColumn(string name);
		string EscapeTable(string name);
		string CreateParameter(string name);
	}

	public static class Dialects
	{
		public static IDialect MySql = new MySqlDialect();
		public static IDialect MsSql = new MsSqlDialect();
	}

	internal class MySqlDialect : IDialect
	{
		public string EscapeColumn(string name)
		{
			return "`" + name + "`";
		}

		public string EscapeTable(string name)
		{
			return "`" + name + "`";
		}

		public string CreateParameter(string name)
		{
			return "?" + name;
		}
	}

	internal class MsSqlDialect : IDialect
	{
		public string EscapeColumn(string name)
		{
			return "[" + name + "]";
		}

		public string EscapeTable(string name)
		{
			return "[" + name + "]";
		}

		public string CreateParameter(string name)
		{
			return "@" + name;
		}
	}
}