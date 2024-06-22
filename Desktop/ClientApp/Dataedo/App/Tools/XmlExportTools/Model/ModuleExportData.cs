using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.XmlExportTools.Model;

public class ModuleExportData
{
	public ModuleExportData Documentation { get; set; }

	public DatabaseRowBase Module { get; set; }

	public ExportData ExportData { get; set; }

	public List<ModuleExportData> ModulesExportData { get; set; }

	public bool IsForExport { get; set; }

	public bool? IsAnyFromAnotherDocumentation { get; set; }

	public string IdString => $"{Module.Title}_{Module.IdValue}";

	public string IdStringForPath => Paths.RemoveInvalidFilePathCharacters(IdString.Replace(' ', '_'), "_");

	public string IdStringPath => "doc/" + IdStringForPath;

	public IEnumerable<ModuleExportData> ModulesExportDataForExport => ModulesExportData.Where((ModuleExportData m) => m.IsForExport && m.Module.Id != -1);

	public IEnumerable<ModuleExportData> OtherModuleExportDataForExport => ModulesExportData.Where((ModuleExportData m) => m.Module.Id == -1);

	public IEnumerable<ModuleExportData> ModulesExportDataForExportWithOther => ModulesExportData.Where((ModuleExportData m) => m.IsForExport);

	public bool IsObjectForExport(SharedObjectTypeEnum.ObjectType objectType, int? objectId)
	{
		if (objectId.HasValue)
		{
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
				return ModulesExportDataForExportWithOther.Any((ModuleExportData m) => m.ExportData.TablesSource != null && m.ExportData.TablesSource.Any((ObjectRow t) => t.ObjectId == objectId));
			case SharedObjectTypeEnum.ObjectType.View:
				return ModulesExportDataForExportWithOther.Any((ModuleExportData m) => m.ExportData.ViewsSource != null && m.ExportData.ViewsSource.Any((ObjectRow t) => t.ObjectId == objectId));
			case SharedObjectTypeEnum.ObjectType.Procedure:
				return ModulesExportDataForExportWithOther.Any((ModuleExportData m) => m.ExportData.ProceduresSource != null && m.ExportData.ProceduresSource.Any((ObjectRow t) => t.ObjectId == objectId));
			case SharedObjectTypeEnum.ObjectType.Function:
				return ModulesExportDataForExportWithOther.Any((ModuleExportData m) => m.ExportData.FunctionsSource != null && m.ExportData.FunctionsSource.Any((ObjectRow t) => t.ObjectId == objectId));
			}
		}
		return false;
	}

	public bool IsObjectForExport(Dataedo.App.Data.MetadataServer.Model.DependencyRow node)
	{
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(node.Type);
		if (objectType == SharedObjectTypeEnum.ObjectType.Trigger)
		{
			objectType = SharedObjectTypeEnum.ObjectType.Table;
		}
		if (objectType.HasValue && objectType != SharedObjectTypeEnum.ObjectType.UnresolvedEntity)
		{
			return IsObjectForExport(objectType.Value, node.DestinationObjectId);
		}
		return false;
	}
}
