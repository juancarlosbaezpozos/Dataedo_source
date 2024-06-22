using Dataedo.App.Classes;
using Dataedo.Shared.Enums;

namespace Dataedo.App.UserControls.Interfaces;

public interface ISelectImportControl
{
	DBMSGridModel DBMSGridModel { get; }

	SharedDatabaseTypeEnum.DatabaseType? SelectedDatabaseType { get; }

	bool IsDBAdded { get; }

	DBMSGridModel GetFocusedDBMSGridModel();
}
