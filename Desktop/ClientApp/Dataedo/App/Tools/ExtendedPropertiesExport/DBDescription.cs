using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.ExtendedProperties;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public class DBDescription
{
	public string Name { get; set; }

	public SharedObjectTypeEnum.ObjectType? MajorObjectType { get; set; }

	public SharedObjectTypeEnum.ObjectType? MinorObjectType { get; set; }

	public string Command { get; set; }

	public DBDescription(DbDescriptionObject dbDescriptionObject)
	{
		Name = PrepareValue.ToString(dbDescriptionObject.Name);
		MajorObjectType = SharedObjectTypeEnum.StringToType(PrepareValue.ToString(dbDescriptionObject.MajorObjectType));
		MinorObjectType = SharedObjectTypeEnum.StringToType(PrepareValue.ToString(dbDescriptionObject.MinorObjectType));
		Command = PrepareValue.ToString(dbDescriptionObject.SqlCommand);
	}
}
