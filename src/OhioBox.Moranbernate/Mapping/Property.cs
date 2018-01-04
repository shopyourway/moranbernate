using System;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Moranbernate.Mapping
{
	public interface IProperty
	{
	}

	public static class PropertyExt
	{
		public static IProperty Column(this IProperty property, string columnName)
		{
			((Property) property).ColumnName = columnName;
			return property;
		}

		public static IProperty Insert(this IProperty property)
		{
			((Property)property).Insert = true;
			return property;
		}

		public static IProperty NotInsert(this IProperty property)
		{
			((Property)property).Insert = false;
			return property;
		}

		public static IProperty Update(this IProperty property)
		{
			((Property)property).Update = true;
			return property;
		}

		public static IProperty NotUpdate(this IProperty property)
		{
			((Property)property).Update = false;
			return property;
		}

		public static IProperty ReadOnly(this IProperty property)
		{
			((Property)property).Insert = false;
			((Property)property).Update = false;
			return property;
		}

		public static IProperty CustomType<TOut>(this IProperty property, ICustomTypeMapper<TOut> customTypeMapper)
		{
			var p = ((IProperty<TOut>) property);
			if (p.CustomTypeMapper != null)
				return property;
			
			p.CustomTypeMapper = customTypeMapper;
			p.FromDbConvertor = customTypeMapper.FromDb;
			p.ToParameter = i => customTypeMapper.ToParameter((TOut)i);

			var valueAccessor = p.ValueAccessor;
			p.ValueAccessor = i => customTypeMapper.ToParameter((TOut)valueAccessor(i));
			return property;
		}
	}

	public interface ICustomTypeMapper<TOut>
	{
		TOut FromDb(object input);
		object ToParameter(TOut input);
	}

	internal abstract class Property : IProperty
	{
		protected Property()
		{
			Insert = true;
			Update = true;
			ToParameter = i => i;
		}

		public string Name { get; protected set; }
		public Type Type { get; protected set; }
		public Func<object, object> ValueAccessor { get; set; }

		public Func<object, object> ToParameter { get; set; }

		public string ColumnName { get; internal set; }

		public bool ReadOnly { get { return !Insert && !Update; } }
		public bool Insert { get; set; }
		public bool Update { get; set; }

		public abstract void SetValue(object t, object v);
		public abstract object ConvertValue(object v);
	}

	internal class TypedProperty<T, TOut> : Property, IProperty<TOut>
	{
		public Func<object, TOut> FromDbConvertor { get; set; }
		public ICustomTypeMapper<TOut> CustomTypeMapper { get; set; }

		private TypedProperty()
		{
			FromDbConvertor = BetterConvert<TOut>.ConvertType;
		}

		public static Property Create(string name, Type type, Func<T, TOut> accessor, Action<T, TOut> setter)
		{
			return new TypedProperty<T, TOut>
			{
				ValueAccessor = o => accessor((T)o),
				Name = name,
				Type = type,
				Setter = setter
			};
		}

		private Action<T, TOut> Setter { get; set; }

		public override void SetValue(object t, object v)
		{
			if (v is DBNull)
				return;

			var value = FromDbConvertor(v);
			Setter((T)t, value);
		}

		public override object ConvertValue(object v)
		{
			if (CustomTypeMapper == null)
				return v;

			return CustomTypeMapper.ToParameter((TOut) v);
		}
	}

	internal interface IProperty<T>
	{
		Func<object, T> FromDbConvertor { get; set; }
		Func<object, object> ValueAccessor { get; set; }
		ICustomTypeMapper<T> CustomTypeMapper { get; set; }
		Func<object, object> ToParameter { get; set; }
	}
}