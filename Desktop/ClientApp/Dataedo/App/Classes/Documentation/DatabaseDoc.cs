using System.ComponentModel;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;
using Dataedo.App.Documentation.Tools;
using Dataedo.App.Enums;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Common.CustomFields;
using Dataedo.Model.Data.Documentations;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class DatabaseDoc : ObjectDoc
{
	public class LayoutModel
	{
		public ComplexFooterElementModel Left { get; set; } = new ComplexFooterElementModel();


		public ComplexFooterElementModel Center { get; set; } = new ComplexFooterElementModel();


		public ComplexFooterElementModel Right { get; set; } = new ComplexFooterElementModel();

	}

	public RootDoc RootDoc { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType? Type { get; set; }

	public string Title { get; set; }

	public BindingList<ModuleDoc> Modules { get; set; }

	public TitlePage TitlePage { get; set; }

	public string Database { get; set; }

	public string DatabaseLower => Database?.ToLower();

	public string Server { get; set; }

	public string ServerLower => Server?.ToLower();

	public bool? HasMultipleSchemas { get; set; }

	public new bool? ShowSchema { get; set; }

	public new bool? ShowSchemaOverride { get; set; }

	public new bool ShowSchemaEffective => DatabaseRow.GetShowSchema(ShowSchema, ShowSchemaOverride);

	public bool HasModules { get; set; }

	public string NumberingPrefix
	{
		get
		{
			if ((base.ObjectsOnSameLevelCount ?? 1) != 1)
			{
				return $"{base.ObjectOrder}.";
			}
			return string.Empty;
		}
	}

	public new bool UseSchema => HasMultipleSchemas.GetValueOrDefault();

	public override string CustomFieldsStringValuesSeparator => "<br/>";

	public DatabaseDoc(RootDoc rootDoc, int databaseId, DocGeneratingOptions docGeneratingOptions = null, int objectOrder = 1, int? objectsOnSameLevelCount = null, Form owner = null)
		: base(docGeneratingOptions, DB.Database.GetDataById(databaseId), SharedObjectTypeEnum.ObjectType.Database, objectOrder, objectsOnSameLevelCount)
	{
		RootDoc = rootDoc;
		DocumentationObject dataById = DB.Database.GetDataById(databaseId);
		base.Id = PrepareValue.ToInt(dataById.DatabaseId).Value;
		Title = PrepareValue.ToString(dataById.Title);
		Type = DatabaseTypeEnum.StringToType(dataById.Type);
		Server = PrepareValue.ToString(dataById.Host);
		Database = PrepareValue.ToString(dataById.Name);
		HasMultipleSchemas = PrepareValue.ToBool(dataById.MultipleSchemas);
		ShowSchema = dataById.ShowSchema;
		ShowSchemaOverride = dataById.ShowSchemaOverride;
		base.DatabaseShowSchema = dataById.ShowSchema;
		base.DatabaseShowSchemaOverride = dataById.ShowSchemaOverride;
		base.IdString = PdfLinksSupport.CreateIdString(dataById.DatabaseId);
		string dbTitle = null;
		string dbDescription = null;
		bool flag = true;
		if ((docGeneratingOptions == null && DB.Module.CountModulesScalarQuery(databaseId) == 0) || (docGeneratingOptions != null && docGeneratingOptions.DBModulesId(databaseId).Count == 0 && docGeneratingOptions.AppendObjectsWithoutModule(databaseId)))
		{
			dbTitle = Title;
			dbDescription = base.Description;
			flag = false;
		}
		TitlePage = new TitlePage(databaseId)
		{
			HideDescription = (string.IsNullOrEmpty(base.Description) || !flag)
		};
		HasModules = flag;
		Modules = ModuleDoc.GetModules(docGeneratingOptions, this, Type, SharedObjectTypeEnum.StringToType(dataById.Type), dbTitle, dbDescription, 1, owner);
	}

	protected override void RetrieveDescription(CustomFieldsData row, bool retrieveCustomFields = true)
	{
		RetrieveCustomFieldsAsHtmlWithDescription(row, retrieveCustomFields);
	}
}
