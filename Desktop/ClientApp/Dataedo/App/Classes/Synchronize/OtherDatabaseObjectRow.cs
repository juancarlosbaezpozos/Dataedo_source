using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.Model.Data.Common.Objects;

namespace Dataedo.App.Classes.Synchronize;

public class OtherDatabaseObjectRow : ObjectRow
{
	public override string ObjectName
	{
		get
		{
			if (string.IsNullOrEmpty(base.Schema))
			{
				return base.Name;
			}
			return base.Schema + "." + base.Name;
		}
	}

	public override string ObjectIdString => Paths.EncodeInvalidPathCharacters((!string.IsNullOrEmpty(base.Schema)) ? $"{base.Schema}_{base.Name}_{base.ObjectId}" : $"{base.Name}_{base.ObjectId}");

	public OtherDatabaseObjectRow(ObjectByModuleObject row, CustomFieldsSupport customFieldsSupport, bool readDisplayedName = true, bool readDocumentationTitle = false)
		: base(row, customFieldsSupport, readDisplayedName, readDocumentationTitle)
	{
	}
}
