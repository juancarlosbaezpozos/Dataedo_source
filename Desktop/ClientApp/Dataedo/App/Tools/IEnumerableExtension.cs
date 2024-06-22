using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Dataedo.App.Tools;

public static class IEnumerableExtension
{
	public static DataTable ToDataTable<T>(this IEnumerable<T> data)
	{
		DataTable dataTable = new DataTable(typeof(T).Name);
		PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			dataTable.Columns.Add(propertyInfo.Name, BaseType(propertyInfo.PropertyType));
		}
		foreach (T datum in data)
		{
			object[] array2 = new object[properties.Length];
			for (int j = 0; j < properties.Length; j++)
			{
				array2[j] = properties[j].GetValue(datum, null);
			}
			dataTable.Rows.Add(array2);
		}
		return dataTable;
	}

	public static Type BaseType(Type type)
	{
		if (type != null && type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
		{
			return Nullable.GetUnderlyingType(type);
		}
		return type;
	}
}
