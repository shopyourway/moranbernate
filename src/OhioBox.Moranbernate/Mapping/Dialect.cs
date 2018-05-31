using System;

namespace OhioBox.Moranbernate.Mapping
{
	public interface IDialect
	{
		string EscapeColumn(string name);
		string EscapeTable(string name);
		string CreateParameter(string name);
		string RegexMatch(string expr, string pattern);
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

		public string RegexMatch(string expr, string pattern)
		{
			return $"{expr} REGEX {pattern}";
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

		public string RegexMatch(string expr, string pattern)
		{
			throw new NotImplementedException("No (normal) regex matching in MsSql");
		}
	}
}