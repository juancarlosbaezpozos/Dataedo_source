using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.Common.CustomFieldsBase;
using DevExpress.DataProcessing;

namespace Dataedo.App.ImportDescriptions.Processing.Saving;

public abstract class SaveProcessorBase<T> : ISaveProcessorBase where T : ImportDataModel
{
	public List<T> Models { get; protected set; }

	protected CustomFieldsSupport CustomFieldsSupport { get; set; }

	protected SaveProcessorBase(CustomFieldsSupport customFieldsSupport)
	{
		CustomFieldsSupport = customFieldsSupport;
	}

	public abstract bool ProcessSaving(int databaseId, Form owner);

	protected void SetCustomFields(BaseWithCustomFields parameter, ImportDataModel model)
	{
		Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> dictionary = new Dictionary<string, BaseWithCustomFields.CustomFieldWithValue>();
		foreach (FieldData item in model.Fields.Where((FieldData x) => x?.IsSelected ?? false))
		{
			dictionary.Add(CustomFieldsDataBase<string>.GetFieldName(model.Fields.IndexOf(item)) ?? "", new BaseWithCustomFields.CustomFieldWithValue(null, item.OverwriteValue));
		}
		parameter.SetCustomFields(dictionary);
	}

	protected void ShowSummaryMessage(Form owner)
	{
		ShowSummaryMessage(Models.Sum((T x) => x.CountAllValues(ChangeEnum.Change.New)), Models.Sum((T x) => x.CountSelectedValues(ChangeEnum.Change.New)), Models.Sum((T x) => x.CountAllValues(ChangeEnum.Change.Update)), Models.Sum((T x) => x.CountSelectedValues(ChangeEnum.Change.Update)), Models.Sum((T x) => x.CountAllValues(ChangeEnum.Change.Erase)), Models.Sum((T x) => x.CountSelectedValues(ChangeEnum.Change.Erase)), Models.Sum((T x) => x.CountAllValues(ChangeEnum.Change.NoChange)), owner);
	}

	private void ShowSummaryMessage(int allNew, int allSelectedNew, int allUpdated, int allSelectedUpdated, int allErased, int allSelectedErased, int allUnchaged, Form owner)
	{
		Color textColor = ChangeEnum.GetTextColor(ChangeEnum.Change.New, isSelected: true);
		string text = "<b>" + ((allSelectedNew == allNew) ? "All" : ((allSelectedNew == 0) ? "None" : allSelectedNew.ToString())) + "</b>" + $" out of {allNew} " + $"<color={textColor.R}, {textColor.G}, {textColor.B}>new</color>" + " values were added.";
		textColor = ChangeEnum.GetTextColor(ChangeEnum.Change.Update, isSelected: true);
		string text2 = "<b>" + ((allSelectedUpdated == allUpdated) ? "All" : ((allSelectedUpdated == 0) ? "None" : allSelectedUpdated.ToString())) + "</b>" + $" out of {allUpdated} " + $"<color={textColor.R}, {textColor.G}, {textColor.B}>changed</color>" + " values were overwritten.";
		textColor = ChangeEnum.GetTextColor(ChangeEnum.Change.Erase, isSelected: true);
		string text3 = "<b>" + ((allSelectedErased == allErased) ? "All" : ((allSelectedErased == 0) ? "None" : allSelectedErased.ToString())) + "</b>" + $" out of {allErased} " + $"<color={textColor.R}, {textColor.G}, {textColor.B}>removed</color>" + " values were erased.";
		string text4 = $"{allSelectedNew + allSelectedUpdated + allSelectedErased} " + "fields in total were updated.";
		textColor = ChangeEnum.GetTextColor(ChangeEnum.Change.NoChange, isSelected: true);
		string text5 = $"{allUnchaged} " + $"<color={textColor.R}, {textColor.G}, {textColor.B}>unchaged</color>" + " fields were ignored.";
		GeneralMessageBoxesHandling.Show(text + Environment.NewLine + text2 + Environment.NewLine + text3 + Environment.NewLine + text4 + Environment.NewLine + text5, "Data saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, owner);
	}
}
