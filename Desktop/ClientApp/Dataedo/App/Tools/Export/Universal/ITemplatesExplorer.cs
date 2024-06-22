using System.Collections.Generic;

namespace Dataedo.App.Tools.Export.Universal;

public interface ITemplatesExplorer
{
	IEnumerable<ITemplate> List();
}
