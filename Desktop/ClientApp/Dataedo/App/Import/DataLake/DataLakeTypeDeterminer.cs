using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Dataedo.App.Helpers.FileImport;

namespace Dataedo.App.Import.DataLake;

public static class DataLakeTypeDeterminer
{
	public static DataLakeTypeEnum.DataLakeType? DetermineType(ImportItem importItem, out Exception exception)
	{
		string text = null;
		exception = null;
		try
		{
			text = ((!importItem.IsStream) ? Path.GetExtension(importItem.Path) : Path.GetExtension(importItem.Name));
		}
		catch (Exception ex)
		{
			Exception ex2 = (exception = ex);
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		DataLakeTypeEnum.DataLakeType[] array = (DataLakeTypeEnum.DataLakeType[])Enum.GetValues(typeof(DataLakeTypeEnum.DataLakeType));
		for (int i = 0; i < array.Length; i++)
		{
			IDataLakeImport dataLakeImport = DataLakeImportFactory.GetDataLakeImport(array[i]);
			if (dataLakeImport.Extensions.Contains(text.ToLower()))
			{
				return dataLakeImport.DataLakeType;
			}
		}
		return null;
	}

	public static DataLakeTypeEnum.DataLakeType? DetermineTypeByFileExtension(string path, CancellationToken? cancellationToken, out Exception exception)
	{
		string text = null;
		exception = null;
		try
		{
			text = Path.GetExtension(path);
		}
		catch (Exception ex)
		{
			Exception ex2 = (exception = ex);
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		DataLakeTypeEnum.DataLakeType[] array = (DataLakeTypeEnum.DataLakeType[])Enum.GetValues(typeof(DataLakeTypeEnum.DataLakeType));
		foreach (DataLakeTypeEnum.DataLakeType dataLakeType in array)
		{
			if (cancellationToken.HasValue && cancellationToken.GetValueOrDefault().IsCancellationRequested)
			{
				return null;
			}
			IDataLakeImport dataLakeImport = DataLakeImportFactory.GetDataLakeImport(dataLakeType);
			if (dataLakeImport.DefaultExtension == text.ToLower())
			{
				return dataLakeImport.DataLakeType;
			}
		}
		return null;
	}

	public static DataLakeTypeEnum.DataLakeType? DetermineTypeByFile(string path, DataLakeTypeEnum.DataLakeType? supposedDataLakeType, CancellationToken? cancellationToken, out Exception exception)
	{
		string text = null;
		exception = null;
		try
		{
			text = Path.GetExtension(path);
		}
		catch (Exception ex)
		{
			Exception ex2 = (exception = ex);
		}
		List<IDataLakeImport> list = new List<IDataLakeImport>();
		DataLakeTypeEnum.DataLakeType[] source = (DataLakeTypeEnum.DataLakeType[])Enum.GetValues(typeof(DataLakeTypeEnum.DataLakeType));
		List<DataLakeTypeEnum.DataLakeType> list2 = new List<DataLakeTypeEnum.DataLakeType>();
		list2.AddRange(source.Where((DataLakeTypeEnum.DataLakeType x) => x != DataLakeTypeEnum.DataLakeType.CSV));
		list2.Add(DataLakeTypeEnum.DataLakeType.CSV);
		foreach (DataLakeTypeEnum.DataLakeType item in list2)
		{
			if (cancellationToken.HasValue && cancellationToken.GetValueOrDefault().IsCancellationRequested)
			{
				return null;
			}
			IDataLakeImport dataLakeImport = DataLakeImportFactory.GetDataLakeImport(item);
			if (dataLakeImport.DetermineByExtensionPriority && (!string.IsNullOrEmpty(text) || item == supposedDataLakeType) && dataLakeImport.DefaultExtension == text)
			{
				try
				{
					if (dataLakeImport.IsValidFile(path))
					{
						return item;
					}
				}
				catch (Exception ex3)
				{
					Exception ex4 = (exception = ex3);
				}
			}
			else
			{
				list.Add(dataLakeImport);
			}
		}
		foreach (IDataLakeImport item2 in list)
		{
			if (cancellationToken.HasValue && cancellationToken.GetValueOrDefault().IsCancellationRequested)
			{
				return null;
			}
			try
			{
				if (item2.IsValidFile(path))
				{
					return item2.DataLakeType;
				}
			}
			catch (Exception ex5)
			{
				Exception ex6 = (exception = ex5);
			}
		}
		return null;
	}

	public static DataLakeTypeEnum.DataLakeType? DetermineTypeByData(string data, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(data))
		{
			return null;
		}
		new List<IDataLakeImport>();
		DataLakeTypeEnum.DataLakeType[] array = (DataLakeTypeEnum.DataLakeType[])Enum.GetValues(typeof(DataLakeTypeEnum.DataLakeType));
		foreach (DataLakeTypeEnum.DataLakeType dataLakeType in array)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return null;
			}
			IDataLakeImport dataLakeImport = DataLakeImportFactory.GetDataLakeImport(dataLakeType);
			if (dataLakeImport.IsValidData(data))
			{
				return dataLakeImport.DataLakeType;
			}
		}
		return null;
	}

	public static IDataLakeImport GetDataLakeByPathExtension(string path)
	{
		string text = null;
		try
		{
			text = Path.GetExtension(path);
		}
		catch
		{
		}
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		DataLakeTypeEnum.DataLakeType[] array = (DataLakeTypeEnum.DataLakeType[])Enum.GetValues(typeof(DataLakeTypeEnum.DataLakeType));
		for (int i = 0; i < array.Length; i++)
		{
			IDataLakeImport dataLakeImport = DataLakeImportFactory.GetDataLakeImport(array[i]);
			if (dataLakeImport.DefaultExtension == text.ToLower())
			{
				return dataLakeImport;
			}
		}
		return null;
	}
}
