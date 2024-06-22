using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Classes.Documentation;

public class DataLinkDoc : ObjectDoc
{
	public string DataType { get; set; }

	public Bitmap Icon { get; set; }

	public Bitmap DocumentationIcon { get; set; }

	public string DocumentationIdString { get; private set; }

	public override string NewLineSeparator => ObjectDoc.HtmlNewLineSeparator;

	public override string CustomFieldsStringValuesSeparator => NewLineSeparator;

	public DataLinkDoc(DocGeneratingOptions docGeneratingOptions, DataLinkObjectExtended row)
		: base(docGeneratingOptions, row, SharedObjectTypeEnum.ObjectType.DataLink)
	{
		base.NameHtml = ((row.ElementName == null) ? row.ObjectNameWithSchemaAndTitle : (row.ObjectName + (string.IsNullOrEmpty(row.ElementPath) ? "." : null) + ColumnNames.GetFullNameFormattedForHtml(string.IsNullOrEmpty(row.ElementPath) ? null : ("." + row.ElementPath), PrepareValue.CreateNameDisplayed(row.ElementName, row.ElementTitle))));
		base.DocumentationTitle = row.ObjectDocumentationTitle;
		Icon = IconsSupport.GetObjectIcon(row.TypeForIcon, row.SubtypeForIcon, row.SourceForIcon);
		DocumentationIcon = IconsSupport.GetObjectIcon(SharedObjectTypeEnum.ObjectType.Database);
		base.IdString = PdfLinksSupport.CreateIdString(row.ObjectDocumentationHost, row.ObjectDocumentationName, row.ObjectSchema, row.ObjectName);
		DocumentationIdString = PdfLinksSupport.CreateIdString(row.ObjectDocumentationId);
	}

	public static BindingList<DataLinkDoc> GetDataLinks(DocGeneratingOptions docGeneratingOptions, int termId, Form owner = null)
	{
		try
		{
			return new BindingList<DataLinkDoc>((from x in DB.BusinessGlossary.GetDataLinks<DataLinkObjectExtended>(termId)
				select new DataLinkDoc(docGeneratingOptions, x)).ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting data links.", owner);
			return null;
		}
	}
}
