using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Tools;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Classes.Documentation;

public class RelationsDoc : ObjectDoc
{
	public int RelationId { get; set; }

	public string Table { get; set; }

	public string TableTitle { get; set; }

	public string TableNameWithTitle
	{
		get
		{
			if (!string.IsNullOrEmpty(TableTitle))
			{
				return Table + " (" + TableTitle + ")";
			}
			return Table;
		}
	}

	public string Join { get; set; }

	public string RowJoin { get; set; }

	public Bitmap Icon { get; set; }

	public override string NewLineSeparator => ObjectDoc.HtmlNewLineSeparator;

	public override string CustomFieldsStringValuesSeparator => NewLineSeparator;

	public RelationsDoc()
	{
	}

	public RelationsDoc(DocGeneratingOptions docGeneratingOptions, DatabaseDoc database, int tableId, RelationKeyDataObject row, bool getPKRelations)
		: base(docGeneratingOptions, row, SharedObjectTypeEnum.ObjectType.Relation)
	{
		RelationId = row.TableRelationId;
		string title = row.Title;
		string descriptionWithHtmlNewLines = GetDescriptionWithHtmlNewLines(row.Description);
		base.NameBase = string.Join(NewLineSeparator, new string[4] { title, base.NameBase, descriptionWithHtmlNewLines, base.CustomFieldsString }.Where((string x) => !string.IsNullOrEmpty(x)));
		if (getPKRelations)
		{
			Table = ObjectNames.GetTableObjectName(database.ShowSchemaEffective, tableId, row.PkTableObject.DatabaseId, DatabaseRow.GetShowSchema(row.PkTableObject.DatabaseShowSchema, row.PkTableObject.DatabaseShowSchemaOverride), row.PkTableObject.DatabaseTitle, row.PkTableObject.Id, row.PkTableObject.Schema, row.PkTableObject.Name, row.FkTableObject.DatabaseId, useDatabaseName: true);
			TableTitle = row.PkTableObject.Title;
		}
		else
		{
			Table = ObjectNames.GetTableObjectName(database.ShowSchemaEffective, tableId, row.FkTableObject.DatabaseId, DatabaseRow.GetShowSchema(row.FkTableObject.DatabaseShowSchema, row.FkTableObject.DatabaseShowSchemaOverride), row.FkTableObject.DatabaseTitle, row.FkTableObject.Id, row.FkTableObject.Schema, row.FkTableObject.Name, row.PkTableObject.DatabaseId, useDatabaseName: true);
			TableTitle = row.FkTableObject.Title;
		}
		base.Id = (getPKRelations ? row.PkTableObject.Id : row.FkTableObject.Id);
		string name = "relation_" + (getPKRelations ? (new Cardinality(CardinalityTypeEnum.StringToType(row.FkType)).Id + "_" + new Cardinality(CardinalityTypeEnum.StringToType(row.PkType)).Id) : (new Cardinality(CardinalityTypeEnum.StringToType(row.PkType)).Id + "_" + new Cardinality(CardinalityTypeEnum.StringToType(row.FkType)).Id)) + (row.Source.Equals("DBMS") ? string.Empty : ("_" + row.Source.ToLower())) + "_24";
		Icon = Resources.ResourceManager.GetObject(name) as Bitmap;
		RowJoin = DB.Relation.GetJoinForPDF(database.ShowSchemaEffective, tableId, row, getPKRelations);
		if (getPKRelations)
		{
			base.IdString = PdfLinksSupport.CreateIdString(row.PkTableDatabaseHost, row.PkTableObject.DatabaseName, row.PkTableObject.Schema, row.PkTableObject.Name);
		}
		else
		{
			base.IdString = PdfLinksSupport.CreateIdString(row.FkTableDatabaseHost, row.FkTableObject.DatabaseName, row.FkTableObject.Schema, row.FkTableObject.Name);
		}
	}

	public static bool ShouldIncludeSchema(bool includeSchema, SharedDatabaseTypeEnum.DatabaseType? databaseType, string schema)
	{
		if (includeSchema)
		{
			if (!databaseType.HasValue)
			{
				return !string.IsNullOrEmpty(schema);
			}
			return true;
		}
		return false;
	}

	public static BindingList<RelationsDoc> GetRelations(DocGeneratingOptions docGeneratingOptions, DatabaseDoc database, int tableId, bool getPKRelations, Form owner = null)
	{
		try
		{
			return new BindingList<RelationsDoc>(GroupRelations(new List<RelationsDoc>(from relation in DB.Relation.GetKeyByTableDoc(tableId, getPKRelations, notDeletedOnly: true)
				select new RelationsDoc(docGeneratingOptions, database, tableId, relation, getPKRelations))).ToList());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting relationships from the table.", owner);
			return null;
		}
	}

	public static IEnumerable<RelationsDoc> GroupRelations(IEnumerable<RelationsDoc> relations)
	{
		return (from x in relations
			group x by x.RelationId).Select(delegate(IGrouping<int, RelationsDoc> x)
		{
			RelationsDoc relationsDoc = x.First();
			if (x.Count() == 1)
			{
				relationsDoc.Join = relationsDoc.RowJoin;
				return relationsDoc;
			}
			relationsDoc.Join = string.Join(",<br>", x.Select((RelationsDoc y) => y.RowJoin));
			return relationsDoc;
		}).ToList();
	}
}
