using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Tools.XmlExportTools.Model;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.XmlExportTools.Tools;

internal class ObjectExporter
{
	public static bool IsInModule(ModuleExportData module, int? objectId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (module != null && module.ExportData != null)
		{
			if (objectType == SharedObjectTypeEnum.ObjectType.Table && module.ExportData.TablesSource != null)
			{
				return IsObjectIn(module.ExportData.TablesSource, objectId);
			}
			if (objectType == SharedObjectTypeEnum.ObjectType.View && module.ExportData.ViewsSource != null)
			{
				return IsObjectIn(module.ExportData.ViewsSource, objectId);
			}
			if (objectType == SharedObjectTypeEnum.ObjectType.Procedure && module.ExportData.ProceduresSource != null)
			{
				return IsObjectIn(module.ExportData.ProceduresSource, objectId);
			}
			if (objectType == SharedObjectTypeEnum.ObjectType.Function && module.ExportData.FunctionsSource != null)
			{
				return IsObjectIn(module.ExportData.FunctionsSource, objectId);
			}
		}
		return false;
	}

	public static bool IsObjectForExport(List<ModuleExportData> allDocumentationsData, ModuleExportData firstDocumentationDataToCheck, SharedObjectTypeEnum.ObjectType objectType, int? objectId, out ModuleExportData documentationContainingObject)
	{
		bool flag = firstDocumentationDataToCheck.IsObjectForExport(objectType, objectId);
		if (flag)
		{
			documentationContainingObject = firstDocumentationDataToCheck;
			return flag;
		}
		foreach (ModuleExportData allDocumentationsDatum in allDocumentationsData)
		{
			if (allDocumentationsDatum != firstDocumentationDataToCheck)
			{
				flag = allDocumentationsDatum.IsObjectForExport(objectType, objectId);
				if (flag)
				{
					documentationContainingObject = allDocumentationsDatum;
					return flag;
				}
			}
		}
		documentationContainingObject = null;
		return flag;
	}

	public static bool IsObjectForExport(List<ModuleExportData> allDocumentationsData, ModuleExportData firstDocumentationDataToCheck, Dataedo.App.Data.MetadataServer.Model.DependencyRow node, out ModuleExportData documentationContainingObject)
	{
		bool flag = firstDocumentationDataToCheck.IsObjectForExport(node);
		if (flag)
		{
			documentationContainingObject = firstDocumentationDataToCheck;
			return flag;
		}
		foreach (ModuleExportData allDocumentationsDatum in allDocumentationsData)
		{
			if (allDocumentationsDatum != firstDocumentationDataToCheck)
			{
				flag = allDocumentationsDatum.IsObjectForExport(node);
				if (flag)
				{
					documentationContainingObject = allDocumentationsDatum;
					return flag;
				}
			}
		}
		documentationContainingObject = null;
		return flag;
	}

	private static bool IsObjectIn(List<ObjectRow> list, int? objectId)
	{
		if (list != null && objectId.HasValue)
		{
			return list.Any((ObjectRow t) => t.ObjectId == objectId);
		}
		return false;
	}
}
