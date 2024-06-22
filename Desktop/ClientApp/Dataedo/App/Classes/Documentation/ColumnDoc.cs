using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Classes.Synchronize.Tools;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Classes.Documentation;

public class ColumnDoc : ObjectDoc
{
	private static DataNamesModel dataNamesModel;

	public string DataType { get; set; }

	public bool IsNullable { get; set; }

	public bool IsIdentity { get; set; }

	public string DefaultValue { get; set; }

	public bool IsComputed { get; set; }

	public string ComputedFormula { get; set; }

	public string IsNullableString
	{
		get
		{
			if (!IsNullable)
			{
				return string.Empty;
			}
			return "Y";
		}
	}

	public UserTypeEnum.UserType? Source { get; set; }

	public Image SourceImage => IconsSupport.GetObjectIcon(SharedObjectTypeEnum.ObjectType.Column, base.ObjectSubtype, Source);

	public Image IsNullableImage
	{
		get
		{
			if (!IsNullable)
			{
				return null;
			}
			return Resources.nullable_64;
		}
	}

	public string Path { get; set; }

	public int Level { get; set; }

	public string ItemType { get; set; }

	public ColumnReferenceDataContainer ReferencesDataContainer { get; set; }

	public ColumnUniqueConstraintDataContainer UniqueConstraintsDataContainer { get; set; }

	public Image UniqueConstraintIcon => UniqueConstraintsDataContainer?.FirstItemIcon;

	public Image Icon => UniqueConstraintIcon;

	public override string NewLineSeparator => ObjectDoc.HtmlNewLineSeparator;

	public override string CustomFieldsStringValuesSeparator => NewLineSeparator;

	public ColumnDoc()
	{
	}

	public ColumnDoc(DocGeneratingOptions docGeneratingOptions, ColumnDocObject row, bool retrieveDescription = true)
		: base(docGeneratingOptions, row, SharedObjectTypeEnum.ObjectType.Column, withName: true, retrieveDescription)
	{
		base.Id = row.ColumnId;
		base.NameBase = PrepareValue.CreateNameDisplayed(base.NameBase, row.Title);
		base.NameHtml = ColumnNames.GetFullNameFormattedForHtml(row.Path, PrepareValue.CreateNameDisplayed(base.NameBase, null));
		DataType = row.DatatypeLen;
		IsNullable = row.Nullable;
		base.Description = GetDescriptionWithHtmlNewLines(row.Description);
		IsIdentity = row.IsIdentity;
		DefaultValue = row.DefaultValue;
		IsComputed = row.IsComputed;
		ComputedFormula = row.ComputedFormula;
		Source = UserTypeEnum.ObjectToType(row.Source);
		base.ObjectSubtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Column, row.ItemType);
		Path = row.Path;
		ItemType = row.ItemType;
		Level = row.Level;
		ReferencesDataContainer = new ColumnReferenceDataContainer();
		UniqueConstraintsDataContainer = new ColumnUniqueConstraintDataContainer(new ColumnUniqueConstraintData(row));
	}

	public ColumnDoc(DocGeneratingOptions docGeneratingOptions, DatabaseDoc databaseDoc, TableViewDoc objectDoc, ColumnDocObject row)
		: this(docGeneratingOptions, row, retrieveDescription: false)
	{
		ColumnReferenceData columnReferenceData = new ColumnReferenceData(databaseDoc.ShowSchemaEffective, new ObjectRow
		{
			ObjectId = (objectDoc.Id ?? (-1)),
			DatabaseId = databaseDoc.Id,
			DocumentationType = databaseDoc.Type,
			DocumentationTitle = databaseDoc.Title
		}, row);
		if (columnReferenceData.PkColumnId.HasValue)
		{
			ReferencesDataContainer.Data.Add(columnReferenceData);
		}
	}

	public static BindingList<ColumnDoc> GetColumns(DocGeneratingOptions docGeneratingOptions, DatabaseDoc databaseDoc, TableViewDoc objectDoc, DataNamesModel dataNamesModel, Form owner = null)
	{
		try
		{
			if (dataNamesModel != null)
			{
				ColumnDoc.dataNamesModel = dataNamesModel;
			}
			else
			{
				ColumnDoc.dataNamesModel = new DataNamesModel();
			}
			return new BindingList<ColumnDoc>((from column in DB.Column.GetDataByTableDoc(objectDoc.Id ?? (-1))
				select new ColumnDoc(docGeneratingOptions, databaseDoc, objectDoc, column.ColumnObject as ColumnDocObject) into x
				group x by x.Id).Select(delegate(IGrouping<int?, ColumnDoc> x)
			{
				ColumnDoc columnDoc = x.First();
				columnDoc.ReferencesDataContainer.Data = x.SelectMany((ColumnDoc y) => y.ReferencesDataContainer.Data).ToList();
				columnDoc.RetrieveDescription(columnDoc.Description, columnDoc.IsNullable, columnDoc.IsIdentity, columnDoc.DefaultValue, columnDoc.IsComputed, columnDoc.ComputedFormula, columnDoc.CustomFieldsString);
				return columnDoc;
			}).ToList());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting columns from the table.", owner);
			return null;
		}
	}

	private void RetrieveDescription(string description, bool isNullable, bool isIdentity, string defaultValue, bool isComputed, string computedFormula, string customFields)
	{
		string[] source = new string[5]
		{
			isNullable ? ("<b>" + dataNamesModel.Columns.Texts.Nullable + "</b>") : null,
			isIdentity ? ("<b>" + dataNamesModel.Columns.Texts.IdentityAutoIncrement + "</b>") : null,
			(!string.IsNullOrEmpty(defaultValue)) ? ("<b>" + dataNamesModel.Columns.Texts.Default + "</b>: " + defaultValue) : null,
			(isComputed && !string.IsNullOrEmpty(computedFormula)) ? ("<b>" + dataNamesModel.Columns.Texts.Computed + "</b>: " + computedFormula) : null,
			(!string.IsNullOrEmpty(ReferencesDataContainer.ReferencesStringCommaDelimited)) ? ("<b>" + dataNamesModel.Columns.Texts.References + "</b>: " + ReferencesDataContainer.ReferencesStringCommaDelimited) : null
		};
		string text = string.Join(NewLineSeparator, source.Where((string x) => x != null));
		base.Description = string.Join(NewLineSeparator, new string[3] { description, text, customFields }.Where((string x) => !string.IsNullOrEmpty(x)));
	}
}
