using System;
using System.Collections.Generic;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public interface ICommentsExportExceptionHanlder
{
	void HandleExceptions(Action action, List<DBDescription> objectsFailureList, DBDescription description);
}
