using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.DataRepository;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export.Universal.ProgressTrackers;
using Dataedo.App.Tools.Export.Universal.Storage;
using Dataedo.App.Tools.Export.Universal.Transformers.Json;
using Dataedo.CustomMessageBox;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers;

internal class JsonTransformer : ITemplateTransformer
{
	public string DataFileName = "data.json.js.part";

	public string DataDirName = "data";

	public bool DistributedData;

	private int AsyncThreshold = 5;

	public int Threshold = 1500;

	private IEnumerable<IMenuTreeItem> CachedMenuTree;

	public bool ForceReloadData = true;

	private int MaxConcurrentSessions { get; } = 8;


	private ParallelOptions ParallelOptions => new ParallelOptions
	{
		MaxDegreeOfParallelism = MaxConcurrentSessions
	};

	private string InstanceName
	{
		get
		{
			if (!DistributedData)
			{
				return "local";
			}
			return "web";
		}
	}

	public event EventHandler ThresholdReached;

	public void Export(ITemplate template, IStorage destination, IProgressTracker progress = null, object options = null)
	{
		WizardOptions wizardOptions = options as WizardOptions;
		progress?.Started();
		progress.Log("Initializing...");
		IEnumerable<IMenuTreeItem> enumerable = CachedMenuTree;
		if (enumerable == null || ForceReloadData)
		{
			IRepository repository = DataFactory.Make(wizardOptions);
			List<IModel> list = new List<IModel>();
			if (IsHandlingBG(template, repository))
			{
				list.AddRange(from x in repository.GetBusinessGlossaries()
					orderby x.Title
					select x);
			}
			list.AddRange(from x in repository.GetDocumentations()
				orderby x.Title
				select x);
			enumerable = DataConverter.ToMenuTree(list);
		}
		progress?.OnProgress(2);
		progress.ThrowIfCanceled();
		progress.Log("Exporting metadata...");
		ExportHeader(wizardOptions.Title, destination);
		progress?.OnProgress(4);
		progress?.Log("Preparing database objects (" + InstanceName + ")...");
		ExportMenuTree(enumerable, destination, new PartProgressTracker(progress, 4, 60));
		progress?.Log("Exporting objects (" + InstanceName + ")...");
		ExportObjects(enumerable, destination, new PartProgressTracker(progress, 60, 99), wizardOptions);
		progress.Log("Ending exporting...");
		ExportFooter(destination);
		CachedMenuTree = enumerable;
		progress?.Done();
	}

	protected bool IsHandlingBG(ITemplate template, IRepository repository)
	{
		IList<IBusinessGlossary> businessGlossaries = repository.GetBusinessGlossaries();
		if (businessGlossaries == null || businessGlossaries.Count <= 0)
		{
			return false;
		}
		if (template.Version == null)
		{
			if (Environment.GetCommandLineArgs().Contains("/dataedocmd"))
			{
				return true;
			}
			return CustomMessageBoxForm.Show("Your template not supports exporting Business Glossaries. It's probably caused because you choose an outdated custom template.\r\n\r\nDo you want to skip exporting BG?", "Unsupported data detected", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No;
		}
		return true;
	}

	private string JsonEscape(string text)
	{
		return text.Replace("\"", "\\\"");
	}

	private string GetLicenseType()
	{
		return "null";
	}

	private string GetLicenseExpireDateJson()
	{
		if (!StaticData.License.ValidTillUtcDateTime.HasValue)
		{
			return "null";
		}
		return "\"" + StaticData.License.ValidTillUtcDateTime.Value.ToLocalTime().ToString("yyyy-MM-dd") + "\"";
	}

	private void ExportHeader(string title, IStorage destination)
	{
		StringBuilder stringBuilder = new StringBuilder("window.repository = {");
		stringBuilder.Append("\"title\": \"" + JsonEscape(title) + "\",");
		stringBuilder.Append("\"license\": \"" + JsonEscape(GetLicenseType()) + "\",");
		stringBuilder.Append("\"license_expires_at\": " + GetLicenseExpireDateJson() + ",");
		stringBuilder.Append("\"exported_at\": \"" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "\",");
		destination.SaveFile(DataFileName, stringBuilder.ToString());
	}

	private void ExportMenuTree(IEnumerable<IMenuTreeItem> menuTree, IStorage destination, IProgressTracker progress)
	{
		destination.AppendFile(DataFileName, "\"structure\": [");
		progress.SubLog("Repository");
		ExportMenuTreeItems(menuTree, destination, progress);
		destination.AppendFile(DataFileName, "],");
	}

	private void ExportMenuTreeItems(IEnumerable<IMenuTreeItem> items, IStorage destination, IProgressTracker progress)
	{
		if (items.Count() < AsyncThreshold || SomeHasNestedObjects(items))
		{
			ExportMenuTreeItemsSync(items, destination, progress);
		}
		else
		{
			ExportMenuTreeItemsAsync(items, destination, progress);
		}
	}

	private bool SomeHasNestedObjects(IEnumerable<IMenuTreeItem> items)
	{
		foreach (IMenuTreeItem item in items)
		{
			IEnumerable<IMenuTreeItem> menuChildren = item.MenuChildren;
			if (menuChildren != null && menuChildren.Count() > 0)
			{
				return true;
			}
		}
		return false;
	}

	private void ExportMenuTreeItemsSync(IEnumerable<IMenuTreeItem> items, IStorage destination, IProgressTracker progress)
	{
		int num = 0;
		int num2 = items.Count();
		foreach (IMenuTreeItem item in items)
		{
			PartProgressTracker progress2 = new PartProgressTracker(progress, num * 100 / num2, (num + 1) * 100 / num2);
			ExportMenuTreeItem(item, destination, progress2);
			num++;
			progress.OnProgress(num * 100 / num2);
		}
	}

	private StringBuilder MenuItemTreeToJsonPart(IMenuTreeItem item)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("{");
		stringBuilder.Append("\"id\": " + JsonConvert.SerializeObject(item.Id) + ",");
		stringBuilder.Append("\"object_id\": " + JsonConvert.SerializeObject(item.ObjectId) + ",");
		stringBuilder.Append("\"type\": " + JsonConvert.SerializeObject(item.MenuType) + ",");
		stringBuilder.Append("\"name\": " + JsonConvert.SerializeObject(item.MenuName) + ",");
		stringBuilder.Append("\"subtype\": " + JsonConvert.SerializeObject(item.MenuSubtype) + ",");
		stringBuilder.Append("\"is_user_defined\": " + JsonConvert.SerializeObject(item.IsUserDefined) + ",");
		IEnumerable<string> columnNames = item.ColumnNames;
		if (columnNames != null && columnNames.Count() > 0)
		{
			stringBuilder.Append("\"columns\": " + JsonConvert.SerializeObject(item.ColumnNames) + ",");
		}
		return stringBuilder;
	}

	private void ExportMenuTreeItemsAsync(IEnumerable<IMenuTreeItem> items, IStorage destination, IProgressTracker progress)
	{
		int done = 0;
		int total = items.Count();
		string[] jsons = new string[total];
		if (!Parallel.ForEach(items, ParallelOptions, delegate(IMenuTreeItem item, ParallelLoopState state, long index)
		{
			StringBuilder stringBuilder2 = MenuItemTreeToJsonPart(item);
			IEnumerable<IMenuTreeItem> menuChildren = item.MenuChildren;
			if (menuChildren != null && menuChildren.Count() > 0)
			{
				throw new NotSupportedException("Asynchronous menu tree export supports only items without children.");
			}
			stringBuilder2.Append("},");
			jsons[index] = stringBuilder2.ToString();
			lock (destination)
			{
				try
				{
					done++;
					progress.SubLog(item.MenuName);
					progress.OnProgress(done * 100 / total);
				}
				catch (OperationCanceledException)
				{
					state.Break();
				}
			}
		}).IsCompleted)
		{
			throw new OperationCanceledException();
		}
		StringBuilder stringBuilder = new StringBuilder();
		string[] array = jsons;
		foreach (string value in array)
		{
			stringBuilder.Append(value);
		}
		destination.AppendFile(DataFileName, stringBuilder.ToString());
	}

	private void ExportMenuTreeItem(IMenuTreeItem item, IStorage destination, IProgressTracker progress)
	{
		progress.SubLog(item.MenuName);
		StringBuilder stringBuilder = MenuItemTreeToJsonPart(item);
		destination.AppendFile(DataFileName, stringBuilder.ToString());
		IEnumerable<IMenuTreeItem> menuChildren = item.MenuChildren;
		if (menuChildren != null && menuChildren.Count() > 0)
		{
			destination.AppendFile(DataFileName, "\"children\": [");
			ExportMenuTreeItems(item.MenuChildren, destination, progress);
			destination.AppendFile(DataFileName, "]");
		}
		destination.AppendFile(DataFileName, "},");
	}

	private void ExportObjects(IEnumerable<IMenuTreeItem> menuTree, IStorage destination, IProgressTracker progress, WizardOptions options)
	{
		destination.AppendFile(DataFileName, "\"objects\": {");
		IEnumerable<IMenuTreeItem> source = DataConverter.FlattenMenuTree(menuTree);
		if (source.Count() > Threshold && this.ThresholdReached != null)
		{
			this.ThresholdReached(this, EventArgs.Empty);
		}
		int done = 0;
		int total = source.Count();
		if (!Parallel.ForEach(source, ParallelOptions, delegate(IMenuTreeItem item, ParallelLoopState state)
		{
			IObject @object = DataConverter.ToObject(item);
			string objectJson = JsonConvert.SerializeObject(@object);
			lock (destination)
			{
				try
				{
					progress?.Log($"Exporting object {done + 1} of {total} ({InstanceName})...");
					progress.SubLog(item.MenuName);
					SerializeObject(destination, @object, objectJson, DistributedData);
					done++;
					progress.OnProgress(done * 100 / total);
				}
				catch (OperationCanceledException)
				{
					state.Break();
				}
			}
		}).IsCompleted)
		{
			throw new OperationCanceledException();
		}
		destination.AppendFile(DataFileName, "}");
	}

	private void SerializeObject(IStorage destination, IObject obj, string objectJson = null, bool distributedData = false)
	{
		if (obj != null)
		{
			if (objectJson == null)
			{
				JsonSerializerSettings settings = new JsonSerializerSettings();
				objectJson = JsonConvert.SerializeObject(obj, settings);
			}
			if (distributedData)
			{
				string text = Path.Combine(DataDirName, obj.ObjectId + ".json.js").Replace("\\", "/");
				destination.AppendFile(DataFileName, "\"" + JsonEscape(obj.ObjectId) + "\": { _ref: \"" + JsonEscape(text) + "\" },");
				destination.SaveFile(text, "window.repositoryObject = " + objectJson + ";");
			}
			else
			{
				destination.AppendFile(DataFileName, "\"" + JsonEscape(obj.ObjectId) + "\": " + objectJson + ",");
			}
		}
	}

	private void ExportFooter(IStorage destination)
	{
		destination.AppendFile(DataFileName, "};");
	}
}
