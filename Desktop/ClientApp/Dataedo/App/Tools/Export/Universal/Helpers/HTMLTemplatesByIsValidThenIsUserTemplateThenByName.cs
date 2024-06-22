using System.Collections.Generic;

namespace Dataedo.App.Tools.Export.Universal.Helpers;

internal class HTMLTemplatesByIsValidThenIsUserTemplateThenByName : IComparer<ITemplate>
{
	public int Compare(ITemplate x, ITemplate y)
	{
		if (x.Name == null)
		{
			return 1;
		}
		if (y.Name == null)
		{
			return -1;
		}
		return x.Name.CompareTo(y.Name);
	}
}
