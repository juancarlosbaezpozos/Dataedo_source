using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.Model.Data.Common.Objects;

namespace Dataedo.App.Classes.Synchronize;

public class OracleObjectRow : ObjectRow
{
	public override string ObjectName
	{
		get
		{
			if (base.MultipleSchemasDatabase != true || string.IsNullOrEmpty(base.Schema))
			{
				return base.Name;
			}
			return base.Schema + "." + base.Name;
		}
	}

	public override string ObjectIdString => Paths.EncodeInvalidPathCharacters((base.MultipleSchemasDatabase == true && !string.IsNullOrEmpty(base.Schema)) ? $"{base.Schema}_{base.Name}_{base.ObjectId}" : $"{base.Name}_{base.ObjectId}");

	public OracleObjectRow(ObjectByModuleObject row, CustomFieldsSupport customFieldsSupport, bool readDisplayedName = true, bool readDocumentationData = false)
		: base(row, customFieldsSupport, readDisplayedName, readDocumentationData)
	{
	}
}
