using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Moranbernate.Utils
{
	internal static class ExpressionProcessor<T>
	{
		private static readonly IDictionary<string, Property> Cache = new ConcurrentDictionary<string, Property>();

		public static Property FindMemberExpression<TOut>(Expression<Func<T, TOut>> expression)
		{
			Property property;
			if (Cache.TryGetValue(GenerateCacheKey(expression.Body), out property))
				return property;

			property = GetProperty(expression);
			Cache.Add(GenerateCacheKey(expression.Body), property);
			return property;
		}

		public static Property GetPropertyFromCache(Expression<Func<T, object>> expression)
		{
			Property property;
			if (Cache.TryGetValue(GenerateCacheKey(expression.Body), out property))
				return property;

			throw new Exception("Can't find property based on expression: " + expression);
		}

		public static Property GetPropertyFromCache<TOut>(Expression<Func<T, TOut>> expression)
		{
			Property property;
			if (Cache.TryGetValue(GenerateCacheKey(expression.Body), out property))
				return property;

			throw new Exception("Can't find property based on expression: " + expression);
		}

		internal static string GenerateCacheKey(Expression expression)
		{
			var unaryExpression = expression as UnaryExpression;
			if (unaryExpression != null)
				expression = unaryExpression.Operand;

			var memberExpression = expression as MemberExpression;
			if (memberExpression == null)
				throw new Exception("Can't generate cache key based on expression: " + expression);

			return memberExpression.Member.Name;
		}

		private static Property GetProperty<TOut>(Expression<Func<T, TOut>> expression)
		{
			var memberExpression = expression.Body as MemberExpression;
			if (memberExpression == null)
				throw new Exception("Could not determine member from " + expression);
			
			var setter = GenerateSetterMethod<TOut>(memberExpression);
			return TypedProperty<T, TOut>.Create(memberExpression.Member.Name, memberExpression.Type, expression.Compile(), setter);
		}

		private static Action<T, TOut> GenerateSetterMethod<TOut>(MemberExpression memberExpression)
		{
			var target = Expression.Parameter(typeof(T), "x");
			var value = Expression.Parameter(memberExpression.Type, "v");

			var memberProperty = Expression.MakeMemberAccess(target, memberExpression.Member);
			var assignment = Expression.Assign(memberProperty, value);

			var lambda = Expression.Lambda<Action<T, TOut>>(assignment, target, value);
			return lambda.Compile();
		}
	}
}