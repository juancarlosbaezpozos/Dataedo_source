using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;

public class LegendNamesModel : Element
{
	[XmlElement(IsNullable = true)]
	public string PrimaryKey { get; set; } = "Primary key";


	[XmlElement(IsNullable = true)]
	public string PrimaryKeyDisabled { get; set; } = "Primary key disabled";


	[XmlElement(IsNullable = true)]
	public string UserDefinedPrimaryKey { get; set; } = "User-defined primary key";


	[XmlElement(IsNullable = true)]
	public string UniqueKey { get; set; } = "Unique key";


	[XmlElement(IsNullable = true)]
	public string UniqueKeyDisabled { get; set; } = "Unique key disabled";


	[XmlElement(IsNullable = true)]
	public string UserDefinedUniqueKey { get; set; } = "User-defined unique key";


	[XmlElement(IsNullable = true)]
	public string ActiveTrigger { get; set; } = "Active trigger";


	[XmlElement(IsNullable = true)]
	public string DisabledTrigger { get; set; } = "Disabled trigger";


	[XmlElement(IsNullable = true)]
	public string ManyToOneRelation { get; set; } = "Many to one relationship";


	[XmlElement(IsNullable = true)]
	public string UserDefinedManyToOneRelation { get; set; } = "User-defined many to one relationship";


	[XmlElement(IsNullable = true)]
	public string OneToManyRelation { get; set; } = "One to many relationship";


	[XmlElement(IsNullable = true)]
	public string UserDefinedOneToManyRelation { get; set; } = "User-defined one to many relationship";


	[XmlElement(IsNullable = true)]
	public string ManyToManyRelation { get; set; } = "Many to many relationship";


	[XmlElement(IsNullable = true)]
	public string UserDefinedManyToManyRelation { get; set; } = "User-defined many to many relationship";


	[XmlElement(IsNullable = true)]
	public string OneToOneRelation { get; set; } = "One to one relationship";


	[XmlElement(IsNullable = true)]
	public string UserDefinedOneToOneRelation { get; set; } = "User-defined one to one relationship";


	[XmlElement(IsNullable = true)]
	public string Input { get; set; } = "Input";


	[XmlElement(IsNullable = true)]
	public string Output { get; set; } = "Output";


	[XmlElement(IsNullable = true)]
	public string InputOutput { get; set; } = "Input/Output";


	[XmlElement(IsNullable = true)]
	public string Uses { get; set; } = "Uses dependency";


	[XmlElement(IsNullable = true)]
	public string UserDefinedUses { get; set; } = "User-defined uses dependency";


	[XmlElement(IsNullable = true)]
	public string UsedBy { get; set; } = "Used by dependency";


	[XmlElement(IsNullable = true)]
	public string UserDefinedUsedBy { get; set; } = "User-defined used by dependency";


	[XmlElement(IsNullable = true)]
	public string Nullable { get; set; } = "Nullable";


	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, "Primary key", PrimaryKey, delegate(object x)
		{
			PrimaryKey = x?.ToString();
		});
		Add(ref id, "Primary key disabled", PrimaryKeyDisabled, delegate(object x)
		{
			PrimaryKeyDisabled = x?.ToString();
		});
		Add(ref id, "User-defined primary key", UserDefinedPrimaryKey, delegate(object x)
		{
			UserDefinedPrimaryKey = x?.ToString();
		});
		Add(ref id, "Unique key", UniqueKey, delegate(object x)
		{
			UniqueKey = x?.ToString();
		});
		Add(ref id, "Unique key disabled", UniqueKeyDisabled, delegate(object x)
		{
			UniqueKeyDisabled = x?.ToString();
		});
		Add(ref id, "User-defined unique key", UserDefinedUniqueKey, delegate(object x)
		{
			UserDefinedUniqueKey = x?.ToString();
		});
		Add(ref id, "Active trigger", ActiveTrigger, delegate(object x)
		{
			ActiveTrigger = x?.ToString();
		});
		Add(ref id, "Disabled trigger", DisabledTrigger, delegate(object x)
		{
			DisabledTrigger = x?.ToString();
		});
		Add(ref id, "Many to one relationship", ManyToOneRelation, delegate(object x)
		{
			ManyToOneRelation = x?.ToString();
		});
		Add(ref id, "User-defined many to one relationship", UserDefinedManyToOneRelation, delegate(object x)
		{
			UserDefinedManyToOneRelation = x?.ToString();
		});
		Add(ref id, "One to many relationship", OneToManyRelation, delegate(object x)
		{
			OneToManyRelation = x?.ToString();
		});
		Add(ref id, "User-defined one to many relationship", UserDefinedOneToManyRelation, delegate(object x)
		{
			UserDefinedOneToManyRelation = x?.ToString();
		});
		Add(ref id, "One to one relationship", OneToOneRelation, delegate(object x)
		{
			OneToOneRelation = x?.ToString();
		});
		Add(ref id, "User-defined one to one relationship", UserDefinedOneToOneRelation, delegate(object x)
		{
			UserDefinedOneToOneRelation = x?.ToString();
		});
		Add(ref id, "Input", Input, delegate(object x)
		{
			Input = x?.ToString();
		});
		Add(ref id, "Output", Output, delegate(object x)
		{
			Output = x?.ToString();
		});
		Add(ref id, "Input/Output", InputOutput, delegate(object x)
		{
			InputOutput = x?.ToString();
		});
		Add(ref id, "Uses dependency", Uses, delegate(object x)
		{
			InputOutput = x?.ToString();
		});
		Add(ref id, "User-defined uses dependency", UserDefinedUses, delegate(object x)
		{
			InputOutput = x?.ToString();
		});
		Add(ref id, "Used by dependency", UsedBy, delegate(object x)
		{
			InputOutput = x?.ToString();
		});
		Add(ref id, "User-defined used by dependency", UserDefinedUsedBy, delegate(object x)
		{
			InputOutput = x?.ToString();
		});
	}
}
