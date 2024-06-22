using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.DataRepository.Models.Aggregates;
using Dataedo.App.Tools.ERD;
using Dataedo.App.Tools.ERD.Canvas;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Export;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Module : IModule, IModel, ICloneable, ICustomFieldsContainer
{
	private DbRepository repository;

	private Lazy<string> erd;

	private Lazy<IDocumentation> documentation;

	private Lazy<IList<ICustomField>> customFields;

	private Lazy<IList<ITable>> tables;

	private Lazy<IList<IView>> views;

	private Lazy<IList<IProcedure>> procedures;

	private Lazy<IList<IFunction>> functions;

	private Lazy<IList<IStructure>> structures;

	public int Id { get; private set; }

	public int DocumentationId { get; private set; }

	public string Title { get; private set; }

	public string Description { get; private set; }

	public string Erd => erd.Value;

	public IDocumentation Documentation => documentation.Value;

	public IList<ICustomField> CustomFields => customFields.Value;

	public IList<ITable> Tables => tables.Value;

	public IList<IView> Views => views.Value;

	public IList<IProcedure> Procedures => procedures.Value;

	public IList<IFunction> Functions => functions.Value;

	public IList<IStructure> Structures => structures.Value;

	public Module(DbRepository repository, ModuleObject row, bool loadErd)
	{
		Module module = this;
		this.repository = repository;
		Id = row.ModuleId;
		DocumentationId = row.DatabaseId;
		Title = row.Title;
		Description = (this.repository.OtherFields.IsSelected(OtherFieldEnum.OtherField.Description) ? row.Description : null);
		customFields = new Lazy<IList<ICustomField>>(() => ((IEnumerable<ICustomField>)(from x in module.repository.GetCustomFields()
			where x.ModuleVisibility
			select new CustomField(module.repository, row, x))).ToList());
		erd = new Lazy<string>(() => (!loadErd) ? null : module.GetDiagram(row));
		documentation = new Lazy<IDocumentation>(() => module.repository.GetDocumentation(module.DocumentationId));
		tables = new Lazy<IList<ITable>>(() => module.repository.GetModuleTables(module.Id));
		views = new Lazy<IList<IView>>(() => module.repository.GetModuleViews(module.Id));
		procedures = new Lazy<IList<IProcedure>>(() => module.repository.GetModuleProcedures(module.Id));
		functions = new Lazy<IList<IFunction>>(() => module.repository.GetModuleFunctions(module.Id));
		structures = new Lazy<IList<IStructure>>(() => module.repository.GetModuleStructures(module.Id));
	}

	private string GetDiagram(ModuleObject row)
	{
		ModuleRow moduleRow = new ModuleRow(row, repository.CustomFields, forXml: true);
		Diagram diagram = new Diagram();
		if (new DiagramManager(DocumentationId, Id, moduleRow?.ErdShowTypes, LinkStyleEnum.LinkStyle.Straight, moduleRow.DisplayDocumentationNameMode, diagram, null, null, repository.NotDeletedOnly, setLinks: true, formatted: true, forHtml: true, repository.OtherFields, moduleRow?.ErdShowNullable).diagram.Elements.HasAnyNodes)
		{
			return diagram.ToSvg(CanvasObject.Output.Image).OuterXml;
		}
		return null;
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
