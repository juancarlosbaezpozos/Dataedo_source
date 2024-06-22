using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Amazon;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Helpers.CloudStorage;
using Dataedo.App.Helpers.CloudStorage.AmazonS3;
using Dataedo.App.Helpers.Files;
using Dataedo.App.Import.CloudStorage;
using Dataedo.App.Import.DataLake;
using Dataedo.App.UserControls.Interfaces;

namespace Dataedo.App.UserControls.CloudStorageBrowserUserControls;

public class AmazonS3BrowserUserControl : CloudStorageBrowserUserControl, ICloudStorageBrowserUserControl<AmazonS3Connection>, ICloudStorageBrowserUserControl, IDisposable
{
	private AmazonS3Connection _client;

	private IContainer components;

	public bool IsSelectedItem => GetCheckedObjects().Any();

	public AmazonS3BrowserUserControl()
	{
		InitializeComponent();
		base.SquishFirstLevelIfOneItem = true;
	}

	private IEnumerable<S3Node> GetCheckedObjects()
	{
		return GetCheckedObjects<S3Node>();
	}

	public ICloudStorageResultObject GetResult()
	{
		AmazonS3CloudStorageResultObject amazonS3CloudStorageResultObject = new AmazonS3CloudStorageResultObject
		{
			CloudResult = GetCheckedObjects().ToList(),
			AmazonClient = _client
		};
		long num = 0L;
		foreach (S3Node item in amazonS3CloudStorageResultObject.CloudResult)
		{
			if (!(DataLakeTypeDeterminer.GetDataLakeByPathExtension(item?.S3Object?.Key) is IStreamableDataLakeImport))
			{
				num += item.Size.GetValueOrDefault();
			}
		}
		if (num > 104857600)
		{
			amazonS3CloudStorageResultObject.WarningMessage = "Total size of files that have to be downloaded is about " + FilesHelper.ToReadableSize(num) + ". Reading them might take a while. Do you want to continue?";
		}
		return amazonS3CloudStorageResultObject;
	}

	public void Init(AmazonS3Connection storageConnection, string arnString)
	{
		if (storageConnection == null)
		{
			throw new ArgumentNullException("storageConnection");
		}
		ClearNodes();
		_client = storageConnection;
		Arn arn = Arn.Parse(arnString);
		List<S3Node> objectsStructure = storageConnection.GetObjectsStructure(arn);
		InitTreeView(objectsStructure, dynamicLoading: false);
	}

	public void Init(AmazonS3Connection storageConnection, DatabaseRow databaseRow, CloudStorageBrowserInfo browserInfo)
	{
		_browserInfo = browserInfo;
		Init(storageConnection, databaseRow.Host);
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
