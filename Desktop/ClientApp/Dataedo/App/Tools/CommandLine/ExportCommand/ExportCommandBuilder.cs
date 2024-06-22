using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Documentation;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.CustomFields;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Export;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.CommandLine.ExportCommand;

internal static class ExportCommandBuilder
{
	public static ExportVersion2 CreateExportCommandObject(DatabaseType repositoryType, LoginInfo loginInfo, DocFormatEnum.DocFormat format, string templatePath, string documentTitle, DocumentationModulesContainer documentationsModulesData, List<ObjectTypeHierarchy> objects, OtherFieldsSupport otherFieldsSupport, CustomFieldsSupport customFieldsSupport, string outputPathBase, string extension)
	{
		ExportVersion2 exportVersion = new ExportVersion2
		{
			IsEnabled = true,
			Overwrite = false
		};
		_ = string.Empty;
		switch (format)
		{
		case DocFormatEnum.DocFormat.PDF:
			exportVersion.Format = ExportBase.FormatEnum.PDF;
			break;
		case DocFormatEnum.DocFormat.HTML:
			exportVersion.Format = ExportBase.FormatEnum.HTML;
			break;
		case DocFormatEnum.DocFormat.Excel:
			exportVersion.Format = ExportBase.FormatEnum.Excel;
			break;
		}
		CommandBuilderBase.SetRepositoryConnection(exportVersion, repositoryType, loginInfo);
		exportVersion.TemplatePath = templatePath;
		exportVersion.DocumentTitle = documentTitle;
		exportVersion.Documentations = new DocumentationsModel();
		exportVersion.Documentations.Documentations.AddRange(documentationsModulesData.GetSelectedDocumentations().Select(delegate(DocumentationModules documentation)
		{
			DocumentationModelBase documentationModelBase = new DocumentationModelBase(documentation.Documentation.IdValue, export: true, documentation.Documentation.Title);
			documentationModelBase.Modules = new ModulesModel
			{
				ExportNotSpecified = false,
				Modules = new List<ModuleModelBase>()
			};
			documentationModelBase.Modules.Modules.AddRange((from x in documentation.Modules
				where x.IdValue != -1
				select new Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ModuleModel(x.IdValue, x.IsShown, x.Name)).ToList());
			documentationModelBase.Modules.Modules.Add(new ModuleOtherModel(documentation.Modules.Where((ModuleRow x) => x.IsShown).Any((ModuleRow x) => x.IdValue == -1)));
			return documentationModelBase;
		}));
		exportVersion.ObjectTypes = new ObjectTypesModel();
		exportVersion.ObjectTypes.Documentation.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(CustomExcludedTypeEnum.CustomExcludedType.Documentation));
		exportVersion.ObjectTypes.Documentation.DatabaseName.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(CustomExcludedTypeEnum.CustomExcludedType.DatabaseName));
		exportVersion.ObjectTypes.Documentation.HostName.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(CustomExcludedTypeEnum.CustomExcludedType.HostName));
		exportVersion.ObjectTypes.Tables.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Table));
		exportVersion.ObjectTypes.Tables.Relations.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Relation));
		exportVersion.ObjectTypes.Tables.Keys.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Key));
		exportVersion.ObjectTypes.Tables.Triggers.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Trigger));
		exportVersion.ObjectTypes.Tables.Triggers.Script.Export = objects.Any((ObjectTypeHierarchy x) => x.ParentObjectType == SharedObjectTypeEnum.ObjectType.Table && x.IsType(SharedObjectTypeEnum.ObjectType.Trigger, SharedObjectTypeEnum.ObjectType.Script));
		exportVersion.ObjectTypes.Tables.Dependencies.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Dependency));
		exportVersion.ObjectTypes.Views.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View));
		exportVersion.ObjectTypes.Views.Relations.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Relation));
		exportVersion.ObjectTypes.Views.Keys.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Key));
		exportVersion.ObjectTypes.Views.Triggers.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Trigger));
		exportVersion.ObjectTypes.Views.Triggers.Script.Export = objects.Any((ObjectTypeHierarchy x) => x.ParentObjectType == SharedObjectTypeEnum.ObjectType.View && x.IsType(SharedObjectTypeEnum.ObjectType.Trigger, SharedObjectTypeEnum.ObjectType.Script));
		exportVersion.ObjectTypes.Views.Dependencies.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Dependency));
		exportVersion.ObjectTypes.Views.Script.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Script));
		exportVersion.ObjectTypes.Procedures.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Procedure));
		exportVersion.ObjectTypes.Procedures.Dependencies.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Dependency));
		exportVersion.ObjectTypes.Procedures.Script.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Script));
		exportVersion.ObjectTypes.Functions.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Function));
		exportVersion.ObjectTypes.Functions.Dependencies.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Dependency));
		exportVersion.ObjectTypes.Functions.Script.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Script));
		exportVersion.ObjectTypes.Structures.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Structure));
		exportVersion.ObjectTypes.Structures.Relations.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Relation));
		exportVersion.ObjectTypes.Structures.Keys.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Key));
		exportVersion.ObjectTypes.Structures.Dependencies.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Dependency));
		exportVersion.ObjectTypes.Structures.Schema.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Schema));
		exportVersion.ObjectTypes.ERDs.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Erd));
		exportVersion.ObjectTypes.BusinessGloassaries.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.BusinessGlossary));
		exportVersion.ObjectTypes.BusinessGloassaries.Terms.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Term));
		exportVersion.ObjectTypes.BusinessGloassaries.Categories.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Category));
		exportVersion.ObjectTypes.BusinessGloassaries.Rules.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Rule));
		exportVersion.ObjectTypes.BusinessGloassaries.Policies.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Policy));
		exportVersion.ObjectTypes.BusinessGloassaries.Other.Export = objects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Other));
		if (format == DocFormatEnum.DocFormat.HTML)
		{
			exportVersion.Fields = new FieldsModel
			{
				Description = otherFieldsSupport.IsSelected(OtherFieldEnum.OtherField.Description),
				Title = otherFieldsSupport.IsSelected(OtherFieldEnum.OtherField.Title),
				DataType = otherFieldsSupport.IsSelected(OtherFieldEnum.OtherField.DataType),
				Nullable = otherFieldsSupport.IsSelected(OtherFieldEnum.OtherField.Nullable),
				Identity = otherFieldsSupport.IsSelected(OtherFieldEnum.OtherField.Identity),
				DefaultComputed = otherFieldsSupport.IsSelected(OtherFieldEnum.OtherField.DefaultComputed)
			};
		}
		exportVersion.CustomFields = new CustomFieldsModel();
		exportVersion.CustomFields.CustomFields = customFieldsSupport.Fields.Select((CustomFieldRowExtended x) => new CustomFieldModelBase(x.CustomFieldId, x.IsSelected, x.TitleVisibilityDescriptionString)).ToList();
		exportVersion.OutputPath = outputPathBase + " {DateTime:yyyy-MM-dd HHmm}" + extension;
		exportVersion.OutputPathAlternative = new XmlCommentObject("<OutputPath>" + outputPathBase + extension + "</OutputPath>");
		return exportVersion;
	}
}
