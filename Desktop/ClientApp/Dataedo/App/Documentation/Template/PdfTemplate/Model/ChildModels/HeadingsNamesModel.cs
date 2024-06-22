using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;

public class HeadingsNamesModel : Element
{
	[XmlElement(IsNullable = true)]
	public string TableOfContents { get; set; } = "Table of contents";


	[XmlElement(IsNullable = true)]
	public string Legend { get; set; } = "Legend";


	[XmlElement(IsNullable = true)]
	public string Tables { get; set; } = "Tables";


	[XmlElement(IsNullable = true)]
	public string Views { get; set; } = "Views";


	[XmlElement(IsNullable = true)]
	public string Procedures { get; set; } = "Procedures";


	[XmlElement(IsNullable = true)]
	public string Functions { get; set; } = "Functions";


	[XmlElement(IsNullable = true)]
	public string Structures { get; set; } = "Structures";


	[XmlElement(IsNullable = true)]
	public string Table { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.Table);


	[XmlElement(IsNullable = true)]
	public string ExternalTable { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.ExternalTable);


	[XmlElement(IsNullable = true)]
	public string ForeignTable { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.ForeignTable);


	[XmlElement(IsNullable = true)]
	public string FileTable { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.FileTable);


	[XmlElement(IsNullable = true)]
	public string GraphTable { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.GraphTable);


	[XmlElement(IsNullable = true)]
	public string GraphNodeTable { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.GraphNodeTable);


	[XmlElement(IsNullable = true)]
	public string GraphEdgeTable { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.GraphEdgeTable);


	[XmlElement(IsNullable = true)]
	public string SystemVersionedTable { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.SystemVersionedTable);


	[XmlElement(IsNullable = true)]
	public string HistoryTable { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.HistoryTable);


	[XmlElement(IsNullable = true)]
	public string SearchIndex { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.SearchIndex);


	[XmlElement(IsNullable = true)]
	public string Collection { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.Collection);


	[XmlElement(IsNullable = true)]
	public string Entity { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.Entity);


	[XmlElement(IsNullable = true)]
	public string Cube { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.Cube);


	[XmlElement(IsNullable = true)]
	public string Dimension { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.Dimension);


	[XmlElement(IsNullable = true)]
	public string FlatFile { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.FlatFile);


	[XmlElement(IsNullable = true)]
	public string Object { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.Object);


	[XmlElement(IsNullable = true)]
	public string StandardObject { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.StandardObject);


	[XmlElement(IsNullable = true)]
	public string CustomObject { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.CustomObject);


	[XmlElement(IsNullable = true)]
	public string ExternalObject { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Table, SharedObjectSubtypeEnum.ObjectSubtype.ExternalObject);


	[XmlElement(IsNullable = true)]
	public string View { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.View, SharedObjectSubtypeEnum.ObjectSubtype.View);


	[XmlElement(IsNullable = true)]
	public string MaterializedView { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.View, SharedObjectSubtypeEnum.ObjectSubtype.MaterializedView);


	[XmlElement(IsNullable = true)]
	public string EditioningView { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.View, SharedObjectSubtypeEnum.ObjectSubtype.EditioningView);


	[XmlElement(IsNullable = true)]
	public string IndexedView { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.View, SharedObjectSubtypeEnum.ObjectSubtype.IndexedView);


	[XmlElement(IsNullable = true)]
	public string NamedQuery { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.View, SharedObjectSubtypeEnum.ObjectSubtype.NamedQuery);


	[XmlElement(IsNullable = true)]
	public string Projection { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.View, SharedObjectSubtypeEnum.ObjectSubtype.Projection);


	[XmlElement(IsNullable = true)]
	public string Procedure { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectSubtypeEnum.ObjectSubtype.Procedure);


	[XmlElement(IsNullable = true)]
	public string Package { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectSubtypeEnum.ObjectSubtype.Package);


	[XmlElement(IsNullable = true)]
	public string CLRProcedure { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectSubtypeEnum.ObjectSubtype.CLRProcedure);


	[XmlElement(IsNullable = true)]
	public string ExtendedProcedure { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectSubtypeEnum.ObjectSubtype.ExtendedProcedure);


	[XmlElement(IsNullable = true)]
	public string Function { get; set; } = "Function";


	[XmlElement(IsNullable = true)]
	public string CLRFunction { get; set; } = SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Function, SharedObjectSubtypeEnum.ObjectSubtype.CLRFunction);


	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, "Table of contents", TableOfContents, delegate(object x)
		{
			TableOfContents = x?.ToString();
		});
		Add(ref id, "Legend", Legend, delegate(object x)
		{
			Legend = x?.ToString();
		});
		Add(ref id, "Tables", Tables, delegate(object x)
		{
			Tables = x?.ToString();
		});
		Add(ref id, "Views", Views, delegate(object x)
		{
			Views = x?.ToString();
		});
		Add(ref id, "Procedures", Procedures, delegate(object x)
		{
			Procedures = x?.ToString();
		});
		Add(ref id, "Functions", Functions, delegate(object x)
		{
			Functions = x?.ToString();
		});
		Add(ref id, "Table", Table, delegate(object x)
		{
			Table = x?.ToString();
		});
		Add(ref id, "View", View, delegate(object x)
		{
			View = x?.ToString();
		});
		Add(ref id, "Procedure", Procedure, delegate(object x)
		{
			Procedure = x?.ToString();
		});
		Add(ref id, "Function", Function, delegate(object x)
		{
			Function = x?.ToString();
		});
	}

	public string GetObjectSubtypeTypeName(SharedObjectTypeEnum.ObjectType mainType, SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype)
	{
		switch (mainType)
		{
		case SharedObjectTypeEnum.ObjectType.Table:
			switch (objectSubtype)
			{
			case SharedObjectSubtypeEnum.ObjectSubtype.Table:
				return Table;
			case SharedObjectSubtypeEnum.ObjectSubtype.ExternalTable:
				return ExternalTable;
			case SharedObjectSubtypeEnum.ObjectSubtype.ForeignTable:
				return ForeignTable;
			case SharedObjectSubtypeEnum.ObjectSubtype.FileTable:
				return FileTable;
			case SharedObjectSubtypeEnum.ObjectSubtype.GraphTable:
				return GraphTable;
			case SharedObjectSubtypeEnum.ObjectSubtype.GraphNodeTable:
				return GraphNodeTable;
			case SharedObjectSubtypeEnum.ObjectSubtype.GraphEdgeTable:
				return GraphEdgeTable;
			case SharedObjectSubtypeEnum.ObjectSubtype.SystemVersionedTable:
				return SystemVersionedTable;
			case SharedObjectSubtypeEnum.ObjectSubtype.HistoryTable:
				return HistoryTable;
			case SharedObjectSubtypeEnum.ObjectSubtype.SearchIndex:
				return SearchIndex;
			case SharedObjectSubtypeEnum.ObjectSubtype.Collection:
				return Collection;
			case SharedObjectSubtypeEnum.ObjectSubtype.Entity:
				return Entity;
			case SharedObjectSubtypeEnum.ObjectSubtype.Cube:
				return Cube;
			case SharedObjectSubtypeEnum.ObjectSubtype.Dimension:
				return Dimension;
			case SharedObjectSubtypeEnum.ObjectSubtype.FlatFile:
				return FlatFile;
			case SharedObjectSubtypeEnum.ObjectSubtype.Object:
				return Object;
			case SharedObjectSubtypeEnum.ObjectSubtype.StandardObject:
				return StandardObject;
			case SharedObjectSubtypeEnum.ObjectSubtype.CustomObject:
				return CustomObject;
			case SharedObjectSubtypeEnum.ObjectSubtype.ExternalObject:
				return ExternalObject;
			}
			break;
		case SharedObjectTypeEnum.ObjectType.View:
			switch (objectSubtype)
			{
			case SharedObjectSubtypeEnum.ObjectSubtype.View:
				return View;
			case SharedObjectSubtypeEnum.ObjectSubtype.MaterializedView:
				return MaterializedView;
			case SharedObjectSubtypeEnum.ObjectSubtype.EditioningView:
				return EditioningView;
			case SharedObjectSubtypeEnum.ObjectSubtype.IndexedView:
				return IndexedView;
			case SharedObjectSubtypeEnum.ObjectSubtype.NamedQuery:
				return NamedQuery;
			case SharedObjectSubtypeEnum.ObjectSubtype.Projection:
				return Projection;
			}
			break;
		case SharedObjectTypeEnum.ObjectType.Procedure:
			switch (objectSubtype)
			{
			case SharedObjectSubtypeEnum.ObjectSubtype.Procedure:
				return Procedure;
			case SharedObjectSubtypeEnum.ObjectSubtype.Package:
				return Package;
			case SharedObjectSubtypeEnum.ObjectSubtype.CLRProcedure:
				return CLRProcedure;
			case SharedObjectSubtypeEnum.ObjectSubtype.ExtendedProcedure:
				return ExtendedProcedure;
			}
			break;
		case SharedObjectTypeEnum.ObjectType.Function:
			switch (objectSubtype)
			{
			case SharedObjectSubtypeEnum.ObjectSubtype.Function:
				return Function;
			case SharedObjectSubtypeEnum.ObjectSubtype.CLRFunction:
				return CLRFunction;
			}
			break;
		}
		return SharedObjectTypeEnum.TypeToStringForSingle(mainType);
	}
}
