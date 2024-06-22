using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class RelatedTermDoc : ObjectDoc
{
	public string DataType { get; set; }

	public Bitmap Icon { get; set; }

	public string RelationshipTitle { get; set; }

	public string RelatedTermTitle { get; set; }

	public override string NewLineSeparator => ObjectDoc.HtmlNewLineSeparator;

	public override string CustomFieldsStringValuesSeparator => NewLineSeparator;

	public RelatedTermDoc(DocGeneratingOptions docGeneratingOptions, TermRelationshipObjectSimple row)
		: base(docGeneratingOptions, row as IBasicIdentification, SharedObjectTypeEnum.ObjectType.TermRelationship)
	{
		RelationshipTitle = row.RelationshipTitle;
		RelatedTermTitle = row.RelatedTermTitle;
		Icon = BusinessGlossarySupport.GetTermIcon(row.RelatedTermTypeIconId);
		base.IdString = PdfLinksSupport.CreateIdString(row.RelatedTermDatabaseId, row.RelatedTermId);
	}

	public static BindingList<RelatedTermDoc> GetRelatedTerms(DocGeneratingOptions docGeneratingOptions, int termId, Form owner = null)
	{
		try
		{
			List<TermRelationshipObjectExtended> termRelationships = DB.BusinessGlossary.GetTermRelationships(termId);
			List<TermRelationshipObjectSimple> termParentChildRelationships = DB.BusinessGlossary.GetTermParentChildRelationships(termId);
			foreach (TermRelationshipObjectSimple item in termParentChildRelationships)
			{
				if (item.RelationshipTitle == "CHILD")
				{
					item.RelationshipTitle = "Is parent of";
				}
				else if (item.RelationshipTitle == "PARENT")
				{
					item.RelationshipTitle = "Is child of";
				}
			}
			return new BindingList<RelatedTermDoc>((from x in termRelationships.Union(termParentChildRelationships)
				orderby x.RelatedTermTitle, x.RelatedTermId, x.RelationshipTitle
				select new RelatedTermDoc(docGeneratingOptions, x)).ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting related terms.", owner);
			return null;
		}
	}
}
