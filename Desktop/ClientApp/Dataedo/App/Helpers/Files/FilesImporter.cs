using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Helpers.FileImport;
using Dataedo.App.Import.DataLake;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.DataLake.Processing;
using Dataedo.App.Import.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.Shared.Enums;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.Helpers.Files;

public static class FilesImporter
{
	public static ObjectModel GetInvalidObjectModel(ImportItem importItem, DataLakeTypeEnum.DataLakeType? dataLakeType, SharedObjectTypeEnum.ObjectType objectType)
	{
		return new ObjectModel(importItem.Name, importItem.Location, dataLakeType, objectType);
	}

	public static ObjectModel GetInvalidObjectModel(string path, DataLakeTypeEnum.DataLakeType? dataLakeType, SharedObjectTypeEnum.ObjectType objectType)
	{
		string name = null;
		try
		{
			name = Path.GetFileName(path);
		}
		catch (Exception)
		{
		}
		return new ObjectModel(name, path, dataLakeType, objectType);
	}

	public static ObjectModel GetInvalidObjectModel(ImportItem importItem, DataLakeTypeEnum.DataLakeType? dataLakeType, SharedObjectTypeEnum.ObjectType objectType, Exception exception)
	{
		ObjectModel invalidObjectModel = GetInvalidObjectModel(importItem, dataLakeType, objectType);
		invalidObjectModel.InitializationDetails = exception?.Message;
		return invalidObjectModel;
	}

	public static ImportResult ImportItems(List<ImportItem> importedItems, DataLakeTypeEnum.DataLakeType? dataLakeType, SharedObjectTypeEnum.ObjectType objectType, SplashScreenManager splashScreenManager, Form owner)
	{
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			if (importedItems.Count == 1)
			{
				return ImportSingleItem(importedItems[0], dataLakeType, splashScreenManager, owner, objectType);
			}
			return ImportMultipleItems(importedItems, dataLakeType, splashScreenManager, owner, objectType);
		}
		catch (InvalidDataProvidedException ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, ex?.InnerException?.Message, 1, owner);
			return new ImportResult
			{
				Success = false
			};
		}
		catch (IOException ex2)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show("Unable to open file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, ex2?.Message, 1, owner);
			return new ImportResult
			{
				Success = false
			};
		}
		catch (FileFormatException ex3)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show("Unable to open file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, ex3?.Message, 1, owner);
			return new ImportResult
			{
				Success = false
			};
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private static bool CheckIfImportFilesExists(IEnumerable<ImportItem> importedItems, SplashScreenManager splashScreenManager, Form owner)
	{
		foreach (ImportItem importedItem in importedItems)
		{
			if (!importedItem.IsStream && !File.Exists(importedItem.Path))
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
				GeneralMessageBoxesHandling.Show("File \"" + importedItem.Path + "\" not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
				return false;
			}
		}
		return true;
	}

	private static bool CheckIfImportItemExists(ImportItem importedItem, DataLakeTypeEnum.DataLakeType? dataLakeType, SplashScreenManager splashScreenManager, Form owner)
	{
		if (importedItem.IsStream)
		{
			return true;
		}
		if (dataLakeType == DataLakeTypeEnum.DataLakeType.DELTALAKE)
		{
			if (!Directory.Exists(importedItem.Path))
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
				GeneralMessageBoxesHandling.Show("Delta Lake directory \"" + importedItem.Path + "\" not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
				return false;
			}
		}
		else if (!File.Exists(importedItem.Path))
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show("File \"" + importedItem.Path + "\" not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
			return false;
		}
		return true;
	}

	public static string GetDeltaLakePath(string folderPath, SplashScreenManager splashScreenManager, Form owner)
	{
		if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
		{
			return null;
		}
		try
		{
			List<FileInfo> list = new List<FileInfo>();
			list.AddRange(new DirectoryInfo(folderPath).GetFiles("*.parquet")?.Where((FileInfo f) => !f.DirectoryName.Contains("\\_delta_log")));
			IEnumerable<DirectoryInfo> enumerable = new DirectoryInfo(folderPath).GetDirectories()?.Where((DirectoryInfo f) => f.Name != "_delta_log");
			if (enumerable != null && enumerable.Count() > 0)
			{
				foreach (DirectoryInfo item in enumerable)
				{
					list.AddRange(item.GetFiles("*.parquet")?.Where((FileInfo f) => !f.DirectoryName.Contains("\\_delta_log")));
				}
			}
			if (list.Count() == 0)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
				GeneralMessageBoxesHandling.Show("There are no .parquet files in the selected folder.\r\n\r\nPlease make sure the selected folder is a valid Delta Lake directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
				return null;
			}
			return list.OrderByDescending((FileInfo f) => f.CreationTime).FirstOrDefault().FullName;
		}
		catch (UnauthorizedAccessException ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show(ex.Message, "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
			return null;
		}
		catch (Exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			return null;
		}
	}

	private static List<ObjectModel> GetObjectModels(ImportItem importedItem, DataLakeTypeEnum.DataLakeType? dataLakeType, SharedObjectTypeEnum.ObjectType objectType)
	{
		try
		{
			IDataLakeImport dataLakeImport = DataLakeImportFactory.GetDataLakeImport(objectType, dataLakeType.Value);
			List<ObjectModel> list;
			if (dataLakeImport is DeltaLakeImport deltaLakeImport)
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(importedItem.GetFile().FullName);
				list = deltaLakeImport.GetObjectsFromFile(importedItem.GetFile().FullName, directoryInfo.Name).ToList();
			}
			else
			{
				list = ((importedItem.IsStream && dataLakeImport is AvroImport avroImport) ? avroImport.GetObjectsFromAvroOrAvscStream(importedItem.CreateStream(), importedItem.Name).ToList() : ((!importedItem.IsStream || !(dataLakeImport is IStreamableDataLakeImport streamableDataLakeImport)) ? dataLakeImport.GetObjectsFromFile(importedItem.GetFile().FullName).ToList() : streamableDataLakeImport.GetObjectsFromStream(importedItem.CreateStream()).ToList()));
			}
			ImportFileTrackingHelper.TrackImportFileReadSuccess(list.SelectMany((ObjectModel o) => o.Fields).Count(), importedItem.Size.ToString(), dataLakeType);
			foreach (ObjectModel item in list)
			{
				importedItem.CorrectObjectModelAfterImport(item);
			}
			return list;
		}
		catch
		{
			ImportFileTrackingHelper.TrackImportFileReadFailed(importedItem.Size.ToString(), dataLakeType);
			throw;
		}
	}

	private static List<ObjectModel> GetObjectModelsForMultipleItems(SharedObjectTypeEnum.ObjectType objectType, ImportItem importedItem, DataLakeTypeEnum.DataLakeType? defaultDataLakeType)
	{
		List<ObjectModel> list = new List<ObjectModel>();
		DataLakeTypeEnum.DataLakeType? dataLakeType = defaultDataLakeType;
		try
		{
			dataLakeType = DataLakeTypeDeterminer.DetermineType(importedItem, out var _);
			if (!dataLakeType.HasValue)
			{
				ObjectModel invalidObjectModel = GetInvalidObjectModel(importedItem, dataLakeType, objectType, null);
				invalidObjectModel.ImportItem = importedItem;
				list.Add(invalidObjectModel);
				return list;
			}
			IDataLakeImport dataLakeImport = DataLakeImportFactory.GetDataLakeImport(objectType, dataLakeType.Value);
			IEnumerable<ObjectModel> enumerable = ((importedItem.IsStream && dataLakeImport is AvroImport avroImport) ? avroImport.GetObjectsFromAvroOrAvscStream(importedItem.CreateStream(), importedItem.Name).ToList() : ((!importedItem.IsStream || !(dataLakeImport is IStreamableDataLakeImport streamableDataLakeImport)) ? dataLakeImport.GetObjectsFromFile(importedItem.GetFile().FullName).ToList() : streamableDataLakeImport.GetObjectsFromStream(importedItem.CreateStream()).ToList()));
			list.AddRange(enumerable);
			ImportFileTrackingHelper.TrackImportFileReadSuccess(enumerable.SelectMany((ObjectModel m) => m.Fields).Count(), importedItem.Size.ToString(), dataLakeType);
			foreach (ObjectModel item in enumerable)
			{
				importedItem.CorrectObjectModelAfterImport(item);
			}
			return list;
		}
		catch (Exception exception2)
		{
			ImportFileTrackingHelper.TrackImportFileReadFailed(importedItem.Size.ToString(), dataLakeType);
			ObjectModel invalidObjectModel2 = GetInvalidObjectModel(importedItem, dataLakeType, objectType, exception2);
			invalidObjectModel2.ImportItem = importedItem;
			list.Add(invalidObjectModel2);
			return list;
		}
	}

	private static ImportResult ImportMultipleItems(List<ImportItem> importedItems, DataLakeTypeEnum.DataLakeType? dataLakeType, SplashScreenManager splashScreenManager, Form owner, SharedObjectTypeEnum.ObjectType objectType)
	{
		ImportResult importResult = new ImportResult();
		if (!CheckIfImportFilesExists(importedItems, splashScreenManager, owner))
		{
			importResult.Success = false;
			return importResult;
		}
		foreach (ImportItem importedItem in importedItems)
		{
			List<ObjectModel> objectModelsForMultipleItems = GetObjectModelsForMultipleItems(objectType, importedItem, dataLakeType);
			importResult.ObjectModels.AddRange(objectModelsForMultipleItems);
		}
		return importResult;
	}

	private static ImportResult ImportSingleItem(ImportItem importedItem, DataLakeTypeEnum.DataLakeType? dataLakeType, SplashScreenManager splashScreenManager, Form owner, SharedObjectTypeEnum.ObjectType objectType)
	{
		ImportResult importResult = new ImportResult();
		if (!CheckIfImportItemExists(importedItem, dataLakeType, splashScreenManager, owner))
		{
			importResult.Success = false;
			return importResult;
		}
		if (!dataLakeType.HasValue)
		{
			ObjectModel invalidObjectModel = GetInvalidObjectModel(importedItem, dataLakeType, objectType, null);
			invalidObjectModel.ImportItem = importedItem;
			importResult.ObjectModels.Add(invalidObjectModel);
		}
		else
		{
			importedItem = ((dataLakeType == DataLakeTypeEnum.DataLakeType.DELTALAKE) ? GetDeltaLakeImportItem(importedItem, splashScreenManager, owner) : importedItem);
			if (importedItem == null || (string.IsNullOrEmpty(importedItem.Path) && !importedItem.IsStream))
			{
				importResult.Success = false;
				importResult.CancelEvent = true;
				return importResult;
			}
			importResult.ObjectModels = GetObjectModels(importedItem, dataLakeType, objectType);
		}
		return importResult;
	}

	private static ImportItem GetDeltaLakeImportItem(ImportItem importedItem, SplashScreenManager splashScreenManager, Form owner)
	{
		if (importedItem is LocalFolderImportItem localFolderImportItem)
		{
			string deltaLakePath = GetDeltaLakePath(localFolderImportItem.Path, splashScreenManager, owner);
			if (string.IsNullOrEmpty(deltaLakePath))
			{
				return null;
			}
			return new LocalFileImportItem(deltaLakePath);
		}
		ImportItem result = importedItem.FindDeltaLakeItem();
		importedItem.DeleteTemporaryFiles();
		return result;
	}
}
