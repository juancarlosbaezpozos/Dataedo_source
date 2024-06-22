using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template.PdfTemplate.Model;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Licences;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.ERD;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Exceptions;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Common.CustomFields;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class ModuleDoc : ObjectDoc
{
	public static string objectsWithoutModuleName = "Other";

	private DatabaseDoc databaseDoc;

	private const float MAX_ERD_WIDTH = 727f;

	private const float MAX_ERD_HEIGHT = 999f;

	private const float ERD_SCALE = 1f;

	public Bitmap Diagram { get; set; }

	public object DiagramSize { get; set; }

	public BindingList<TableDoc> Tables { get; set; }

	public BindingList<ViewDoc> Views { get; set; }

	public BindingList<ProcedureDoc> Procedures { get; set; }

	public BindingList<FunctionDoc> Functions { get; set; }

	public BindingList<StructureDoc> Structures { get; set; }

	public BindingList<GlossaryEntriesDoc> GlossaryEntries { get; set; }

	public bool? IsAnyFromAnotherDocumentation { get; set; }

	public string TablesHeader { get; set; }

	public string ViewsHeader { get; set; }

	public string ProceduresHeader { get; set; }

	public string FunctionsHeader { get; set; }

	public string StructuresHeader { get; set; }

	public bool HasDatabaseModules => databaseDoc?.HasModules ?? false;

	public bool HasDiagramAnyNodes { get; private set; }

	public bool HasDescriptionOrCustomFieldOrErd
	{
		get
		{
			if (!base.HasDescriptionOrCustomFields)
			{
				return HasDiagramAnyNodes;
			}
			return true;
		}
	}

	public override string CustomFieldsStringValuesSeparator => "<br/>";

	public ModuleDoc(DocGeneratingOptions docGeneratingOptions, DatabaseDoc database, ModuleObject row, int moduleOrder, SharedDatabaseTypeEnum.DatabaseType? databaseType, int objectOrder = 1, int? objectsOnSameLevelCount = null, Form owner = null)
		: base(docGeneratingOptions, row, SharedObjectTypeEnum.ObjectType.Module, withName: false, retrieveDescription: true, retrieveCustomFields: true, objectOrder)
	{
		databaseDoc = database;
		int moduleId = row.ModuleId;
		int value = database.Id.Value;
		bool valueOrDefault = row.ErdShowTypes.GetValueOrDefault();
		DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode displayDocumentationNameMode = DisplayDocumentationNameModeEnum.ObjectToType(row.DisplayDocumentationNameMode);
		bool erdShowNullable = row.ErdShowNullable;
		DocHeaders docHeaders = new DocHeaders(moduleOrder, database.NumberingPrefix);
		base.NameBase = docHeaders.CreateModuleNameWithOrder(row.Title);
		Tables = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Table)).Any() ? new BindingList<TableDoc>() : TableDoc.GetTablesByModule(database, docGeneratingOptions, moduleId, docHeaders, databaseType, owner));
		Views = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View)).Any() ? new BindingList<ViewDoc>() : ViewDoc.GetViewsByModule(database, docGeneratingOptions, moduleId, docHeaders, databaseType, owner));
		Procedures = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Procedure)).Any() ? new BindingList<ProcedureDoc>() : ProcedureDoc.GetProceduresByModule(database, docGeneratingOptions, moduleId, docHeaders, databaseType, owner));
		Functions = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Function)).Any() ? new BindingList<FunctionDoc>() : FunctionDoc.GetFunctionByModule(database, docGeneratingOptions, moduleId, docHeaders, databaseType, owner));
		Structures = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Structure)).Any() ? new BindingList<StructureDoc>() : StructureDoc.GetStructuresByModule(database, docGeneratingOptions, moduleId, docHeaders, databaseType, owner));
		PrepareObjects();
		CreateListHeaders(docHeaders, docGeneratingOptions);
		if (!docGeneratingOptions.LoadErdDiagrams)
		{
			return;
		}
		Diagram diagram = new Diagram();
		new DiagramManager(value, moduleId, valueOrDefault, LinkStyleEnum.LinkStyle.Straight, displayDocumentationNameMode, diagram, null, null, notDeletedOnly: true, setLinks: false, formatted: false, forHtml: false, null, erdShowNullable);
		Diagram = new Bitmap(diagram.ToImage());
		HasDiagramAnyNodes = diagram.HasAnyNodes;
		if (HasDiagramAnyNodes)
		{
			float num = (float)Diagram.Width / 1f;
			float num2 = (float)Diagram.Height / 1f;
			if (num > 727f)
			{
				float num3 = (float)Diagram.Height / (float)Diagram.Width;
				num = 727f;
				num2 = 727f * num3;
			}
			if (num2 > 999f)
			{
				float num4 = (float)Diagram.Width / (float)Diagram.Height;
				num2 = 999f;
				num = 999f * num4;
			}
			DiagramSize = new SizeF(num, num2);
		}
		else
		{
			Diagram = null;
			DiagramSize = new SizeF(0f, 0f);
		}
	}

	public ModuleDoc(DocGeneratingOptions docGeneratingOptions, DatabaseDoc database, SharedDatabaseTypeEnum.DatabaseType? databaseType, SharedObjectTypeEnum.ObjectType? objectType, string dbTitle, int lastModuleOrder, string dbDescription, int objectOrder = 1, Form owner = null)
		: base(docGeneratingOptions, null, SharedObjectTypeEnum.ObjectType.Module, withName: false, retrieveDescription: false, retrieveCustomFields: false, objectOrder)
	{
		databaseDoc = database;
		string text;
		int moduleOrder;
		if (string.IsNullOrEmpty(dbTitle))
		{
			text = objectsWithoutModuleName;
			moduleOrder = lastModuleOrder;
		}
		else if (database.RootDoc.HaveDocumentationsNoModules)
		{
			text = database.Title;
			base.Description = dbDescription;
			moduleOrder = 1;
		}
		else
		{
			text = ((objectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary) ? "Business Glossary" : database.TitlePage.Subtitle);
			moduleOrder = 1;
		}
		DocHeaders docHeaders = new DocHeaders(moduleOrder, database.NumberingPrefix);
		base.NameBase = docHeaders.CreateModuleNameWithOrder(PrepareValue.ToString((!string.IsNullOrEmpty(text)) ? text : objectsWithoutModuleName), !database.RootDoc.HaveDocumentationsNoModules);
		if (objectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
		{
			GlossaryEntries = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.BusinessGlossary)).Any() ? new BindingList<GlossaryEntriesDoc>() : GlossaryEntriesDoc.GetGlossaryEntries(database, docGeneratingOptions, docHeaders, databaseType, docGeneratingOptions.ExcludedObjects, owner));
		}
		else
		{
			Tables = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Table)).Any() ? new BindingList<TableDoc>() : TableDoc.GetTablesWithoutModule(database, docGeneratingOptions, docHeaders, databaseType, owner));
			Views = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View)).Any() ? new BindingList<ViewDoc>() : ViewDoc.GetViewsWithoutModule(database, docGeneratingOptions, database.Id.Value, docHeaders, databaseType, owner));
			Procedures = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Procedure)).Any() ? new BindingList<ProcedureDoc>() : ProcedureDoc.GetProceduresWithoutModule(database, docGeneratingOptions, docHeaders, databaseType, owner));
			Functions = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Function)).Any() ? new BindingList<FunctionDoc>() : FunctionDoc.GetFunctionWithoutModule(database, docGeneratingOptions, database.Id.Value, docHeaders, databaseType, owner));
			Structures = (docGeneratingOptions.ExcludedObjects.Where((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Structure)).Any() ? new BindingList<StructureDoc>() : StructureDoc.GetStructuresWithoutModule(database, docGeneratingOptions, database.Id.Value, docHeaders, databaseType, owner));
		}
		CreateListHeaders(docHeaders, docGeneratingOptions);
	}

	private void CreateListHeaders(DocHeaders docHeaders, DocGeneratingOptions docGeneratingOptions)
	{
		HeadingsNamesModel headingsNamesModel = (docGeneratingOptions?.Template as PdfTemplateModel)?.Customization?.Localization?.Headings ?? new HeadingsNamesModel();
		BindingList<TableDoc> tables = Tables;
		TablesHeader = ((tables != null && tables.Count > 0) ? docHeaders.CreateListNameWithOrder(SharedObjectTypeEnum.ObjectType.Table, headingsNamesModel.Tables) : null);
		BindingList<ViewDoc> views = Views;
		ViewsHeader = ((views != null && views.Count > 0) ? docHeaders.CreateListNameWithOrder(SharedObjectTypeEnum.ObjectType.View, headingsNamesModel.Views) : null);
		BindingList<ProcedureDoc> procedures = Procedures;
		ProceduresHeader = ((procedures != null && procedures.Count > 0) ? docHeaders.CreateListNameWithOrder(SharedObjectTypeEnum.ObjectType.Procedure, headingsNamesModel.Procedures) : null);
		BindingList<FunctionDoc> functions = Functions;
		FunctionsHeader = ((functions != null && functions.Count > 0) ? docHeaders.CreateListNameWithOrder(SharedObjectTypeEnum.ObjectType.Function, headingsNamesModel.Functions) : null);
		BindingList<StructureDoc> structures = Structures;
		StructuresHeader = ((structures != null && structures.Count > 0) ? docHeaders.CreateListNameWithOrder(SharedObjectTypeEnum.ObjectType.Structure, headingsNamesModel.Structures) : null);
	}

	public static bool AreObjectsWithoutModule(int databaseId)
	{
		if (!DB.Table.AreObjectsWithoutModule(databaseId))
		{
			return DB.Procedure.AreObjectsWithoutModule(databaseId);
		}
		return true;
	}

	public static BindingList<ModuleDoc> GetModules(DocGeneratingOptions docGeneratingOptions, DatabaseDoc database, SharedDatabaseTypeEnum.DatabaseType? databaseType, SharedObjectTypeEnum.ObjectType? objectType, string dbTitle, string dbDescription, int moduleOrder, Form owner = null)
	{
		try
		{
			int objectOrder = 1;
			List<ModuleDoc> list = new List<ModuleDoc>();
			List<ModuleObject> dataByDatabase = DB.Module.GetDataByDatabase(database.Id.Value);
			if (docGeneratingOptions != null && docGeneratingOptions.ModulesId(database.Id).Count > 0)
			{
				List<int> modulesIdsFromDatabase = docGeneratingOptions.ModulesId(database.Id);
				dataByDatabase = dataByDatabase.Where((ModuleObject x) => modulesIdsFromDatabase.Contains(x.ModuleId)).ToList();
				list = new List<ModuleDoc>(dataByDatabase.Select(delegate(ModuleObject x)
				{
					DocGeneratingOptions docGeneratingOptions3 = docGeneratingOptions;
					DatabaseDoc database3 = database;
					int moduleOrder3 = moduleOrder++;
					SharedDatabaseTypeEnum.DatabaseType? databaseType3 = databaseType;
					int objectOrder3 = objectOrder++;
					Form owner3 = owner;
					return new ModuleDoc(docGeneratingOptions3, database3, x, moduleOrder3, databaseType3, objectOrder3, null, owner3);
				}));
				if (docGeneratingOptions.AppendObjectsWithoutModule(database.Id))
				{
					list.Add(new ModuleDoc(docGeneratingOptions, database, databaseType, objectType, dbTitle, moduleOrder, dbDescription, objectOrder++, owner));
				}
			}
			else if (docGeneratingOptions == null)
			{
				CustomFieldsSupport customFieldsSupport = new CustomFieldsSupport();
				customFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: false);
				docGeneratingOptions = new DocGeneratingOptions(customFieldsSupport);
				list = new List<ModuleDoc>(dataByDatabase.Cast<ModuleObject>().Select(delegate(ModuleObject module)
				{
					DocGeneratingOptions docGeneratingOptions2 = docGeneratingOptions;
					DatabaseDoc database2 = database;
					int moduleOrder2 = moduleOrder++;
					SharedDatabaseTypeEnum.DatabaseType? databaseType2 = databaseType;
					int objectOrder2 = objectOrder++;
					Form owner2 = owner;
					return new ModuleDoc(docGeneratingOptions2, database2, module, moduleOrder2, databaseType2, objectOrder2, null, owner2);
				}));
				if (AreObjectsWithoutModule(database.Id.Value))
				{
					list.Add(new ModuleDoc(docGeneratingOptions, database, databaseType, objectType, dbTitle, moduleOrder, dbDescription, objectOrder++, owner));
				}
			}
			return new BindingList<ModuleDoc>(list);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting modules from the database.", owner);
			return null;
		}
	}

	protected override void RetrieveDescription(CustomFieldsData row, bool retrieveCustomFields = true)
	{
		RetrieveDescriptionAsHtml(row, retrieveCustomFields);
	}

	private void PrepareObjects()
	{
		FixHeaders(Tables.Cast<ObjectDoc>());
		FixHeaders(Views.Cast<ObjectDoc>());
		FixHeaders(Procedures.Cast<ObjectDoc>());
		FixHeaders(Functions.Cast<ObjectDoc>());
		FixHeaders(Structures.Cast<ObjectDoc>());
	}

	private void FixHeaders(IEnumerable<ObjectDoc> objects)
	{
		foreach (ObjectDoc @object in objects)
		{
			@object.FixHeader();
		}
	}
}
