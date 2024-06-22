using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Helpers.CloudStorage;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;
using Dataedo.App.Helpers.Files;
using Dataedo.App.Import.CloudStorage;
using Dataedo.App.Import.DataLake;
using Dataedo.App.UserControls.Interfaces;

namespace Dataedo.App.UserControls;

public class AzureStorageBrowserUserControl : CloudStorageBrowserUserControl, ICloudStorageBrowserUserControl<AzureStorageConnection>, ICloudStorageBrowserUserControl, IDisposable
{
	private AzureStorageConnection _client;

	private IContainer components;

	public bool IsSelectedItem => GetCheckedObjects().Any();

	public AzureStorageBrowserUserControl()
	{
		InitializeComponent();
		base.SquishFirstLevelIfOneItem = true;
	}

	private IEnumerable<FileLikeNode> GetCheckedObjects()
	{
		return GetCheckedObjects<FileLikeNode>();
	}

	public ICloudStorageResultObject GetResult()
	{
		AzureStorageCloudStorageResultObject azureStorageCloudStorageResultObject = new AzureStorageCloudStorageResultObject(GetCheckedObjects())
		{
			AzureStorageConnection = _client
		};
		long num = 0L;
		foreach (FileLikeNode item in azureStorageCloudStorageResultObject.CloudResult)
		{
			if (!(DataLakeTypeDeterminer.GetDataLakeByPathExtension(item.Name) is IStreamableDataLakeImport))
			{
				num += item.Size.GetValueOrDefault();
			}
		}
		if (num > 104857600)
		{
			azureStorageCloudStorageResultObject.WarningMessage = "Total size of files that have to be downloaded is about " + FilesHelper.ToReadableSize(num) + ". Reading them might take a while. Do you want to continue?";
		}
		return azureStorageCloudStorageResultObject;
	}

	public void Init(AzureStorageConnection storageConnection, DatabaseRow databaseRow, CloudStorageBrowserInfo browserInfo)
	{
		_browserInfo = browserInfo;
		Init(storageConnection);
	}

	public void Init(AzureStorageConnection storageConnection)
	{
		_client = storageConnection ?? throw new ArgumentNullException("storageConnection");
		ClearNodes();
		bool dynamicLoading = true;
		List<FileLikeNode> objectsStructure = storageConnection.GetObjectsStructure(dynamicLoading);
		InitTreeView(objectsStructure, dynamicLoading);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
	}
}
