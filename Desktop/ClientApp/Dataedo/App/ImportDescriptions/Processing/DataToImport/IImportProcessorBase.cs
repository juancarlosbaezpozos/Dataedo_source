using System.Collections.Generic;
using System.ComponentModel;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace Dataedo.App.ImportDescriptions.Processing.DataToImport;

public interface IImportProcessorBase
{
	Dictionary<string, string> WarningsDuringImport { get; }

	List<ImportDataModel> ModelsGeneral { get; }

	bool IsChanged { get; }

	void PrepareColumns();

	void AddRows(string data, BackgroundWorker worker, DoWorkEventArgs e);

	void RemoveSelectedRows();

	void RemoveAllRows();

	void CheckRows();

	void CheckValueChanged(object row, string fieldName);

	string GetTooltipString(ImportDataModel row, string fieldName);

	bool CheckIfDataValid();

	void UnselectField(FieldDefinition fieldDefinition);

	void SetLabels(LabelControl errorTextLabelControl, LayoutControlItem errorInformationLayoutControlItem, LabelControl warningTextLabelControl, LayoutControlItem warningInformationLayoutControlItem, LabelControl valuesTextLabelControl, LayoutControlItem valuesInformationLayoutControlItem);
}
