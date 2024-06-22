using System.Collections.Generic;
using Dataedo.App.Documentation;

namespace Dataedo.App.Tools.Export.PDF;

internal class PDFTemplatesByIsValidThenByIsUserTemplateThenByName : IComparer<DocTemplateFile>
{
	public int Compare(DocTemplateFile x, DocTemplateFile y)
	{
		if (x.Template == null)
		{
			return 1;
		}
		if (y.Template == null)
		{
			return -1;
		}
		if (x.Template.ExceptionValue != null && y.Template.ExceptionValue == null)
		{
			return 1;
		}
		if (x.Template.ExceptionValue == null && y.Template.ExceptionValue != null)
		{
			return -1;
		}
		if (x.Template.Name == null)
		{
			return 1;
		}
		if (y.Template.Name == null)
		{
			return -1;
		}
		if (x.Template.ExceptionValue != null && y.Template.ExceptionValue != null)
		{
			return x.Template.Name.CompareTo(y.Template.Name);
		}
		if (x.IsUserTemplate && y.IsUserTemplate)
		{
			return x.Template.Name.CompareTo(y.Template.Name);
		}
		if (x.IsUserTemplate && !y.IsUserTemplate)
		{
			return -1;
		}
		if (!x.IsUserTemplate && y.IsUserTemplate)
		{
			return 1;
		}
		if (!x.IsUserTemplate && !y.IsUserTemplate)
		{
			return x.Template.Name.CompareTo(y.Template.Name);
		}
		return 0;
	}
}
