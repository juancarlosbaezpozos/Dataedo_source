using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;

public class DataNamesModel : Element
{
	public class ColumnsModel : TableCaptionElementModel
	{
		public class ColumnsModelColumnsModel : Element
		{
			[XmlElement(IsNullable = true)]
			public string Name { get; set; } = "Name";


			[XmlElement(IsNullable = true)]
			public string DataType { get; set; } = "Data type";


			[XmlElement(IsNullable = true)]
			public string Nullable { get; set; } = "N";


			[XmlElement(IsNullable = true)]
			public string DescriptionAttributes { get; set; } = "Description / Attributes";


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, "Name", Name, delegate(object x)
				{
					Name = x?.ToString();
				});
				Add(ref id, "Data type", DataType, delegate(object x)
				{
					DataType = x?.ToString();
				});
				Add(ref id, "Nullable", Nullable, delegate(object x)
				{
					Nullable = x?.ToString();
				});
				Add(ref id, "Description / Attributes", DescriptionAttributes, delegate(object x)
				{
					DescriptionAttributes = x?.ToString();
				});
			}
		}

		public class ColumnsModelTextsModel : Element
		{
			[XmlElement(IsNullable = true)]
			public string Nullable { get; set; } = "Nullable";


			[XmlElement(IsNullable = true)]
			public string IdentityAutoIncrement { get; set; } = "Identity / Auto increment";


			[XmlElement(IsNullable = true)]
			public string Default { get; set; } = "Default";


			[XmlElement(IsNullable = true)]
			public string Computed { get; set; } = "Computed";


			[XmlElement(IsNullable = true)]
			public string References { get; set; } = "References";


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, "Nullable", Nullable, delegate(object x)
				{
					Nullable = x?.ToString();
				});
				Add(ref id, "Identity / Auto increment", IdentityAutoIncrement, delegate(object x)
				{
					IdentityAutoIncrement = x?.ToString();
				});
				Add(ref id, "Default", Default, delegate(object x)
				{
					Default = x?.ToString();
				});
				Add(ref id, "Computed", Computed, delegate(object x)
				{
					Computed = x?.ToString();
				});
				Add(ref id, "References", References, delegate(object x)
				{
					References = x?.ToString();
				});
			}
		}

		[XmlElement(IsNullable = true)]
		public ColumnsModelColumnsModel Columns { get; set; } = new ColumnsModelColumnsModel();


		[XmlElement(IsNullable = true)]
		public ColumnsModelTextsModel Texts { get; set; } = new ColumnsModelTextsModel();


		public ColumnsModel()
			: base("Columns")
		{
		}

		public ColumnsModel(string tableCaption)
			: base(tableCaption)
		{
		}

		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
			Add(ref id, Columns, "Columns");
			Add(ref id, Texts, "Texts");
		}
	}

	public class LinksModel : TableCaptionElementModel
	{
		public class LinksModelColumnsModel : Element
		{
			[XmlElement(IsNullable = true)]
			public string Table { get; set; } = "Table";


			[XmlElement(IsNullable = true)]
			public string Join { get; set; } = "Join";


			[XmlElement(IsNullable = true)]
			public string TitleNameDescription { get; set; } = "Title / Name / Description";


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, "Table", Table, delegate(object x)
				{
					Table = x?.ToString();
				});
				Add(ref id, "Join", Join, delegate(object x)
				{
					Join = x?.ToString();
				});
				Add(ref id, "Title / Name / Description", TitleNameDescription, delegate(object x)
				{
					TitleNameDescription = x?.ToString();
				});
			}
		}

		[XmlElement(IsNullable = true)]
		public LinksModelColumnsModel Columns { get; set; } = new LinksModelColumnsModel();


		public LinksModel()
			: base("Links")
		{
		}

		public LinksModel(string tableCaption)
			: base(tableCaption)
		{
		}

		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
			Add(ref id, Columns, "Columns");
		}
	}

	public class UniqueKeysModel : TableCaptionElementModel
	{
		public class UniqueKeysModelColumnsModel : Element
		{
			[XmlElement(IsNullable = true)]
			public string Columns { get; set; } = "Columns";


			[XmlElement(IsNullable = true)]
			public string NameDescription { get; set; } = "Name / Description";


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, "Columns", Columns, delegate(object x)
				{
					Columns = x?.ToString();
				});
				Add(ref id, "Name / Description", NameDescription, delegate(object x)
				{
					NameDescription = x?.ToString();
				});
			}
		}

		[XmlElement(IsNullable = true)]
		public UniqueKeysModelColumnsModel Columns { get; set; } = new UniqueKeysModelColumnsModel();


		public UniqueKeysModel()
			: base("Unique keys")
		{
		}

		public UniqueKeysModel(string tableCaption)
			: base(tableCaption)
		{
		}

		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
			Add(ref id, Columns, "Columns");
		}
	}

	public class TriggersModel : TableCaptionElementModel
	{
		public class TriggersModelColumnsModel : Element
		{
			[XmlElement(IsNullable = true)]
			public string Name { get; set; } = "Name";


			[XmlElement(IsNullable = true)]
			public string When { get; set; } = "When";


			[XmlElement(IsNullable = true)]
			public string Description { get; set; } = "Description";


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, "Name", Name, delegate(object x)
				{
					Name = x?.ToString();
				});
				Add(ref id, "When", When, delegate(object x)
				{
					When = x?.ToString();
				});
				Add(ref id, "Description", Description, delegate(object x)
				{
					Description = x?.ToString();
				});
			}
		}

		[XmlElement(IsNullable = true)]
		public TriggersModelColumnsModel Columns { get; set; } = new TriggersModelColumnsModel();


		public TriggersModel()
			: base("Triggers")
		{
		}

		public TriggersModel(string tableCaption)
			: base(tableCaption)
		{
		}

		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
			Add(ref id, Columns, "Columns");
		}
	}

	public class InputOutputModel : TableCaptionElementModel
	{
		public class InputOutputModelColumnsModel : Element
		{
			[XmlElement(IsNullable = true)]
			public string Name { get; set; } = "Name";


			[XmlElement(IsNullable = true)]
			public string DataType { get; set; } = "Data type";


			[XmlElement(IsNullable = true)]
			public string Description { get; set; } = "Description";


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, "Name", Name, delegate(object x)
				{
					Name = x?.ToString();
				});
				Add(ref id, "Data type", DataType, delegate(object x)
				{
					DataType = x?.ToString();
				});
				Add(ref id, "Description", Description, delegate(object x)
				{
					Description = x?.ToString();
				});
			}
		}

		[XmlElement(IsNullable = true)]
		public InputOutputModelColumnsModel Columns { get; set; } = new InputOutputModelColumnsModel();


		public InputOutputModel()
			: base("Input/Output")
		{
		}

		public InputOutputModel(string tableCaption)
			: base(tableCaption)
		{
		}

		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
			Add(ref id, Columns, "Columns");
		}
	}

	public class UseModel : TableCaptionElementModel
	{
		public class UseModelColumnsModel : Element
		{
			[XmlElement(IsNullable = true)]
			public string Name { get; set; } = "Name";


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, "Name", Name, delegate(object x)
				{
					Name = x?.ToString();
				});
			}
		}

		[XmlElement(IsNullable = true)]
		public UseModelColumnsModel Columns { get; set; } = new UseModelColumnsModel();


		public UseModel()
			: base("Use")
		{
		}

		public UseModel(string tableCaption)
			: base(tableCaption)
		{
		}

		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
			Add(ref id, Columns, "Columns");
		}
	}

	public class ScriptModel : TableCaptionElementModel
	{
		public ScriptModel()
			: base("Script")
		{
		}

		public ScriptModel(string tableCaption)
			: base(tableCaption)
		{
		}

		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
		}
	}

	public class RelatedTermsModel : TableCaptionElementModel
	{
		public class RelatedTermsColumnsModel : Element
		{
			[XmlElement(IsNullable = true)]
			public string Relationship { get; set; } = "Relationship";


			[XmlElement(IsNullable = true)]
			public string RelatedTerm { get; set; } = "Related term";


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, "Relationship", Relationship, delegate(object x)
				{
					Relationship = x?.ToString();
				});
				Add(ref id, "Related term", RelatedTerm, delegate(object x)
				{
					RelatedTerm = x?.ToString();
				});
			}
		}

		[XmlElement(IsNullable = true)]
		public RelatedTermsColumnsModel Columns { get; set; } = new RelatedTermsColumnsModel();


		public RelatedTermsModel()
			: base("Relationships")
		{
		}

		public RelatedTermsModel(string tableCaption)
			: base(tableCaption)
		{
		}

		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
			Add(ref id, Columns, "Columns");
		}
	}

	public class LinkedDataModel : TableCaptionElementModel
	{
		public class LinkedDataColumnsModel : Element
		{
			[XmlElement(IsNullable = true)]
			public string Object { get; set; } = "Object";


			[XmlElement(IsNullable = true)]
			public string Documentation { get; set; } = "Documentation";


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, "Object", Object, delegate(object x)
				{
					Object = x?.ToString();
				});
				Add(ref id, "Documentation", Documentation, delegate(object x)
				{
					Documentation = x?.ToString();
				});
			}
		}

		[XmlElement(IsNullable = true)]
		public LinkedDataColumnsModel Columns { get; set; } = new LinkedDataColumnsModel();


		public LinkedDataModel()
			: base("Data Links")
		{
		}

		public LinkedDataModel(string tableCaption)
			: base(tableCaption)
		{
		}

		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
			Add(ref id, Columns, "Columns");
		}
	}

	[XmlElement(IsNullable = true)]
	public ColumnsModel Columns { get; set; } = new ColumnsModel("Columns");


	[XmlElement(IsNullable = true)]
	public LinksModel LinksTo { get; set; } = new LinksModel("Links to");


	[XmlElement(IsNullable = true)]
	public LinksModel LinkedFrom { get; set; } = new LinksModel("Linked from");


	[XmlElement(IsNullable = true)]
	public UniqueKeysModel UniqueKeys { get; set; } = new UniqueKeysModel("Unique keys");


	[XmlElement(IsNullable = true)]
	public TriggersModel Triggers { get; set; } = new TriggersModel("Triggers");


	[XmlElement(IsNullable = true)]
	public InputOutputModel InputOutput { get; set; } = new InputOutputModel("Input/Output");


	[XmlElement(IsNullable = true)]
	public UseModel Uses { get; set; } = new UseModel("Uses");


	[XmlElement(IsNullable = true)]
	public UseModel UsedBy { get; set; } = new UseModel("Used By");


	[XmlElement(IsNullable = true)]
	public ScriptModel Script { get; set; } = new ScriptModel("Script");


	[XmlElement(IsNullable = true)]
	public ScriptModel Schema { get; set; } = new ScriptModel("Schema");


	[XmlElement(IsNullable = true)]
	public RelatedTermsModel RelatedTerms { get; set; } = new RelatedTermsModel("Relationships");


	[XmlElement(IsNullable = true)]
	public LinkedDataModel LinkedData { get; set; } = new LinkedDataModel("Data Links");


	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, Columns, "Columns table");
		Add(ref id, LinksTo, "Links to table");
		Add(ref id, LinkedFrom, "Linked from table");
		Add(ref id, UniqueKeys, "Unique keys table");
		Add(ref id, Triggers, "Triggers table");
		Add(ref id, InputOutput, "Input/Output table");
		Add(ref id, Uses, "Uses table");
		Add(ref id, UsedBy, "Used by table");
		Add(ref id, Script, "Script");
		Add(ref id, Schema, "Schema");
		Add(ref id, RelatedTerms, "Relationships");
		Add(ref id, LinkedData, "Data Links");
	}
}
