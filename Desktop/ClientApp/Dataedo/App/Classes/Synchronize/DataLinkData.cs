using System.Drawing;
using Dataedo.App.Properties;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Tables.Columns;

namespace Dataedo.App.Classes.Synchronize;

public class DataLinkData
{
	public int? TermId { get; set; }

	public string TermType { get; set; }

	public int? TermDocumentationId { get; set; }

	public string TermDocumentationTitle { get; set; }

	public string TermTitle { get; set; }

	public string TermDescriptionPlain { get; set; }

	public bool HasData => TermId.HasValue;

	public string FullTermName => TermDocumentationTitle + "." + TermTitle;

	public Image ObjectImage => Resources.term_16;

	public string TermTitleWithType => TermTypeObject.GetTitleAsSuffixWord(TermType) + " " + TermTitle;

	public string DescriptionPlainWithoutNewLines => TermDescriptionPlain?.Replace("\r\n", " ")?.Replace("\r", " ")?.Replace("\n", " ");

	public string ShortDescriptionFormatted => DataLinkObject.CreateShortDescriptionFormatted(TermType, TermTitle, DescriptionPlainWithoutNewLines);

	public DataLinkData(ColumnWithReferenceObject row)
	{
		TermId = row.DataLinkTermId;
		TermType = row.DataLinkTermType;
		TermDocumentationId = row.DataLinkTermDocumentationId;
		TermDocumentationTitle = row.DataLinkTermDocumentationTitle;
		TermTitle = row.DataLinkTermTitle;
		TermDescriptionPlain = row.DataLinkTermDescriptionPlain;
	}
}
