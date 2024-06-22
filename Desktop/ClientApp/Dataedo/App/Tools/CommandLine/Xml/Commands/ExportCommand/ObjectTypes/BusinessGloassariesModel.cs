using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes;

public class BusinessGloassariesModel : ExportModelBase
{
	[XmlElement]
	public ExportModelBase Terms { get; set; }

	[XmlElement]
	public ExportModelBase Categories { get; set; }

	[XmlElement]
	public ExportModelBase Rules { get; set; }

	[XmlElement]
	public ExportModelBase Policies { get; set; }

	[XmlElement]
	public ExportModelBase Other { get; set; }

	public BusinessGloassariesModel()
	{
		Terms = new ExportModelBase(export: true);
		Categories = new ExportModelBase(export: true);
		Rules = new ExportModelBase(export: true);
		Policies = new ExportModelBase(export: true);
		Other = new ExportModelBase(export: true);
	}

	public BusinessGloassariesModel(bool export)
		: base(export)
	{
		Terms = new ExportModelBase(export);
		Categories = new ExportModelBase(export);
		Rules = new ExportModelBase(export);
		Policies = new ExportModelBase(export);
		Other = new ExportModelBase(export);
	}
}
