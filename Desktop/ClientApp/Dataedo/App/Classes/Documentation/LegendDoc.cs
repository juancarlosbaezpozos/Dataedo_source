using System.Collections.Generic;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;
using Dataedo.App.Properties;

namespace Dataedo.App.Classes.Documentation;

public class LegendDoc
{
	public static List<LegendDocRow> Load(LegendNamesModel legendNamesModel)
	{
		return new List<LegendDocRow>
		{
			new LegendDocRow(Resources.primary_key_64, legendNamesModel.PrimaryKey),
			new LegendDocRow(Resources.primary_key_disabled_64, legendNamesModel.PrimaryKeyDisabled),
			new LegendDocRow(Resources.primary_key_user_64, legendNamesModel.UserDefinedPrimaryKey),
			new LegendDocRow(Resources.unique_key_64, legendNamesModel.UniqueKey),
			new LegendDocRow(Resources.unique_key_disabled_64, legendNamesModel.UniqueKeyDisabled),
			new LegendDocRow(Resources.unique_key_user_64, legendNamesModel.UserDefinedUniqueKey),
			new LegendDocRow(Resources.trigger_active_64, legendNamesModel.ActiveTrigger),
			new LegendDocRow(Resources.trigger_disabled_64, legendNamesModel.DisabledTrigger),
			new LegendDocRow(Resources.relation_mx_1x_24, legendNamesModel.ManyToOneRelation),
			new LegendDocRow(Resources.relation_mx_1x_user_24, legendNamesModel.UserDefinedManyToOneRelation),
			new LegendDocRow(Resources.relation_1x_mx_24, legendNamesModel.OneToManyRelation),
			new LegendDocRow(Resources.relation_1x_mx_user_24, legendNamesModel.UserDefinedOneToManyRelation),
			new LegendDocRow(Resources.relation_mx_mx_24, legendNamesModel.ManyToManyRelation),
			new LegendDocRow(Resources.relation_mx_mx_user_24, legendNamesModel.UserDefinedManyToManyRelation),
			new LegendDocRow(Resources.relation_1x_1x_24, legendNamesModel.OneToOneRelation),
			new LegendDocRow(Resources.relation_1x_1x_user_24, legendNamesModel.UserDefinedOneToOneRelation),
			new LegendDocRow(Resources.parameter_in_64, legendNamesModel.Input),
			new LegendDocRow(Resources.parameter_out_64, legendNamesModel.Output),
			new LegendDocRow(Resources.parameter_in_out_64, legendNamesModel.InputOutput),
			new LegendDocRow(Resources.uses_view_16, legendNamesModel.Uses),
			new LegendDocRow(Resources.uses_user_view_16, legendNamesModel.UserDefinedUses),
			new LegendDocRow(Resources.used_by_view_16, legendNamesModel.UsedBy),
			new LegendDocRow(Resources.used_by_user_view_16, legendNamesModel.UserDefinedUsedBy)
		};
	}
}
