using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;

public class ObjectTypesModel
{
	[XmlElement]
	public Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.DocumentationModel Documentation { get; set; }

	[XmlElement]
	public TablesModel Tables { get; set; }

	[XmlElement]
	public ViewsModel Views { get; set; }

	[XmlElement]
	public ProceduresModel Procedures { get; set; }

	[XmlElement]
	public FunctionsModel Functions { get; set; }

	[XmlElement]
	public StructuresModel Structures { get; set; }

	[XmlElement]
	public ERDsModel ERDs { get; set; }

	[XmlElement]
	public BusinessGloassariesModel BusinessGloassaries { get; set; }

	public ObjectTypesModel()
	{
		Documentation = new Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.DocumentationModel();
		Tables = new TablesModel();
		Views = new ViewsModel();
		Procedures = new ProceduresModel();
		Functions = new FunctionsModel();
		Structures = new StructuresModel();
		ERDs = new ERDsModel();
		BusinessGloassaries = new BusinessGloassariesModel(export: false);
	}

	public static ObjectTypesModel GetExportAllModel()
	{
		return new ObjectTypesModel
		{
			Documentation = new Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.DocumentationModel(export: true)
			{
				DatabaseName = new ExportModelBase(export: true),
				HostName = new ExportModelBase(export: true)
			},
			Tables = new TablesModel(export: true)
			{
				Relations = new ExportModelBase(export: true),
				Keys = new ExportModelBase(export: true),
				Triggers = new ExportModelWithScriptBase(export: true)
				{
					Script = new ExportModelBase(export: true)
				},
				Dependencies = new ExportModelBase(export: true)
			},
			Views = new ViewsModel(export: true)
			{
				Relations = new ExportModelBase(export: true),
				Keys = new ExportModelBase(export: true),
				Dependencies = new ExportModelBase(export: true)
			},
			Procedures = new ProceduresModel(export: true)
			{
				Dependencies = new ExportModelBase(export: true)
			},
			Functions = new FunctionsModel(export: true)
			{
				Dependencies = new ExportModelBase(export: true)
			},
			Structures = new StructuresModel(export: true)
			{
				Relations = new ExportModelBase(export: true),
				Keys = new ExportModelBase(export: true),
				Dependencies = new ExportModelBase(export: true)
			},
			ERDs = new ERDsModel(export: true),
			BusinessGloassaries = new BusinessGloassariesModel(export: true)
		};
	}
}
