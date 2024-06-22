using System.Collections.Generic;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.BandedGrid;

namespace Dataedo.App.ImportDescriptions.Processing.CheckingBeforeSaving;

public class ImportTablesProcessor : ImportProcessorBase<TableImportDataModel>
{
	public ImportTablesProcessor(List<FieldDefinition> fieldDefinitions, BandedGridView dataGridView, RepositoryItemCheckEdit repositoryItemCheckEdit)
		: base(fieldDefinitions, dataGridView, repositoryItemCheckEdit)
	{
	}
}
