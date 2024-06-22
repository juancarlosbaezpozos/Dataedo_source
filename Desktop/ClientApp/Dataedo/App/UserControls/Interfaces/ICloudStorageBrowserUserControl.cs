using System;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Helpers.CloudStorage;
using Dataedo.App.Import.CloudStorage;

namespace Dataedo.App.UserControls.Interfaces;

public interface ICloudStorageBrowserUserControl : IDisposable
{
	bool IsSelectedItem { get; }

	event EventHandler SelectedItemsChanged;

	void ClearNodes();

	ICloudStorageResultObject GetResult();
}
public interface ICloudStorageBrowserUserControl<TConnection> : ICloudStorageBrowserUserControl, IDisposable
{
	void Init(TConnection storageConnection, DatabaseRow databaseRow, CloudStorageBrowserInfo browserInfo);
}
