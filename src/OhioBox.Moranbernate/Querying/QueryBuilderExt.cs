using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Querying.Restrictions;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Moranbernate.Querying
{
	public static class QueryBuilderExt
	{
		public static IQueryBuilder<T> OrderBy<T>(this IQueryBuilder<T> query, params Expression<Func<T, object>>[] fields)
		{
			foreach (var expression in fields)
				query.OrderBy(expression, true);

			return query;
		}

		public static IQueryBuilder<T> OrderByDescending<T>(this IQueryBuilder<T> query, params Expression<Func<T, object>>[] fields)
		{
			foreach (var expression in fields)
				query.OrderBy(expression, false);

			return query;
		}

		public static IRestrictable<T> Or<T>(this IRestrictable<T> restrictable, params Func<IRestrictable<T>, IRestrictable<T>>[] restrictions)
		{
			return restrictable.AddRestriction(new OrRestriction<T>(restrictions));
		}

		public static IRestrictable<T> Equal<T, TValue>(this IRestrictable<T> restrictable, Expression<Func<T, TValue>> member, TValue value)
		{
			return restrictable.AddRestriction(new OperatorRestriction<TValue>(ExpressionProcessor<T>.GetPropertyFromCache(member), value, "="));
		}

		public static IRestrictable<T> NotEqual<T, TValue>(this IRestrictable<T> restrictable, Expression<Func<T, TValue>> member, TValue value)
		{
			return restrictable.AddRestriction(new OperatorRestriction<TValue>(ExpressionProcessor<T>.GetPropertyFromCache(member), value, "<>"));
		}

		public static IRestrictable<T> In<T, TValue>(this IRestrictable<T> restrictable, Expression<Func<T, TValue>> member, ICollection<TValue> value)
		{
			return restrictable.AddRestriction(new InRestriction<TValue>(ExpressionProcessor<T>.GetPropertyFromCache(member), value));
		}

		public static IRestrictable<T> NotIn<T, TValue>(this IRestrictable<T> restrictable, Expression<Func<T, TValue>> member, ICollection<TValue> value)
		{
			return restrictable.AddRestriction(new InRestriction<TValue>(ExpressionProcessor<T>.GetPropertyFromCache(member), value, not:true));
		}

		public static IRestrictable<T> GreaterOrEqual<T, TValue>(this IRestrictable<T> restrictable, Expression<Func<T, TValue>> member, TValue value)
		{
			return restrictable.AddRestriction(new OperatorRestriction<TValue>(ExpressionProcessor<T>.GetPropertyFromCache(member), value, ">="));
		}

		public static IRestrictable<T> GreaterThan<T, TValue>(this IRestrictable<T> restrictable, Expression<Func<T, TValue>> member, TValue value)
		{
			return restrictable.AddRestriction(new OperatorRestriction<TValue>(ExpressionProcessor<T>.GetPropertyFromCache(member), value, ">"));
		}

		public static IRestrictable<T> LessOrEqual<T, TValue>(this IRestrictable<T> restrictable, Expression<Func<T, TValue>> member, TValue value)
		{
			return restrictable.AddRestriction(new OperatorRestriction<TValue>(ExpressionProcessor<T>.GetPropertyFromCache(member), value, "<="));
		}

		public static IRestrictable<T> LessThan<T, TValue>(this IRestrictable<T> restrictable, Expression<Func<T, TValue>> member, TValue value)
		{
			return restrictable.AddRestriction(new OperatorRestriction<TValue>(ExpressionProcessor<T>.GetPropertyFromCache(member), value, "<"));
		}

		public static IRestrictable<T> IsNull<T, TValue>(this IRestrictable<T> restrictable, Expression<Func<T, TValue>> member)
		{
			return restrictable.AddRestriction(new IsNullRestriction(ExpressionProcessor<T>.GetPropertyFromCache(member)));
		}

		public static IRestrictable<T> IsNotNull<T, TValue>(this IRestrictable<T> restrictable, Expression<Func<T, TValue>> member)
		{
			return restrictable.AddRestriction(new IsNullRestriction(ExpressionProcessor<T>.GetPropertyFromCache(member), not:true));
		}

		public static IRestrictable<T> Contains<T>(this IRestrictable<T> restrictable, Expression<Func<T, object>> member, string value)
		{
			return LikeActions(restrictable, member, "%" + value + "%");
		}

		public static IRestrictable<T> StartWith<T>(this IRestrictable<T> restrictable, Expression<Func<T, object>> member, string value)
		{
			return LikeActions(restrictable, member, value + "%");
		}

		public static IRestrictable<T> EndWith<T>(this IRestrictable<T> restrictable, Expression<Func<T, object>> member, string value)
		{
			return LikeActions(restrictable, member, "%" + value);
		}

		public static IRestrictable<T> NotLike<T>(this IRestrictable<T> restrictable, Expression<Func<T, object>> member, string value)
		{
			return restrictable.AddRestriction(new OperatorRestriction<string>(ExpressionProcessor<T>.GetPropertyFromCache(member), "%" + value + "%", "NOT LIKE"));
		}

		public static IRestrictable<T> RegexMatch<T>(this IRestrictable<T> restrictable, Expression<Func<T, object>> member, string pattern)
		{
			return restrictable.AddRestriction(new RegexRestriction<string>(ExpressionProcessor<T>.GetPropertyFromCache(member), pattern));
		}

		private static IRestrictable<T> LikeActions<T>(IRestrictable<T> restrictable, Expression<Func<T, object>> member, string value)
		{
			return restrictable.AddRestriction(new OperatorRestriction<string>(ExpressionProcessor<T>.GetPropertyFromCache(member), value , "LIKE"));
		}
	}
}