using Dataedo.App.Tools.CustomFields;
using Dataedo.Model.Data.Tables.Relations;

namespace Dataedo.App.Classes.Synchronize;

public class OracleRelationRow : RelationRow
{
	public OracleRelationRow(RelationWithColumnAndUniqueConstraint row, bool isForXml, CustomFieldsSupport customFieldsSupport)
		: base(row, isForXml, customFieldsSupport)
	{
	}
}
