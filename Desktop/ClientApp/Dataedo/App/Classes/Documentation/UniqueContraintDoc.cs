using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Classes.Documentation;

public class UniqueContraintDoc : ObjectDoc
{
	public int UniqueContraintId { get; set; }

	public Bitmap Icon { get; set; }

	public string Columns { get; set; }

	public string RowColumns { get; set; }

	public int IsNotEmpty { get; set; }

	public override string NewLineSeparator => ObjectDoc.HtmlNewLineSeparator;

	public override string CustomFieldsStringValuesSeparator => NewLineSeparator;

	public UniqueContraintDoc()
	{
	}

	public UniqueContraintDoc(DocGeneratingOptions docGeneratingOptions, UniqueConstraintWithColumnObject row)
		: base(docGeneratingOptions, row, SharedObjectTypeEnum.ObjectType.Key)
	{
		string text = (row.PrimaryKey ? "primary_key" : "unique_key");
		string text2 = ((row.Disabled == true) ? "_disabled" : string.Empty);
		string text3 = "_64";
		if (row.Source.Equals("DBMS"))
		{
			Icon = Resources.ResourceManager.GetObject(text + text2 + text3) as Bitmap;
		}
		else
		{
			Icon = Resources.ResourceManager.GetObject(text + "_user" + text3) as Bitmap;
		}
		UniqueContraintId = row.UniqueConstraintId;
		RowColumns = ColumnNames.GetFullNameFormattedForHtml(row.ColumnPath, row.ColumnName);
		IsNotEmpty = 1;
	}

	public static BindingList<UniqueContraintDoc> GetUniqueContraints(DocGeneratingOptions docGeneratingOptions, int tableId, Form owner = null)
	{
		try
		{
			return new BindingList<UniqueContraintDoc>(GroupUniqueConstraints(new List<UniqueContraintDoc>(from constraint in DB.Constraint.GetDataWithColumnsByTableDoc(tableId, notDeletedOnly: true)
				select new UniqueContraintDoc(docGeneratingOptions, constraint))).ToList());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting unique constraints from the table.", owner);
			return null;
		}
	}

	public static IEnumerable<UniqueContraintDoc> GroupUniqueConstraints(IEnumerable<UniqueContraintDoc> uniqueConstraints)
	{
		return (from x in uniqueConstraints
			group x by x.UniqueContraintId).Select(delegate(IGrouping<int, UniqueContraintDoc> x)
		{
			UniqueContraintDoc uniqueContraintDoc = x.First();
			if (x.Count() == 1)
			{
				uniqueContraintDoc.Columns = uniqueContraintDoc.RowColumns;
				return uniqueContraintDoc;
			}
			uniqueContraintDoc.Columns = string.Join(", ", x.Select((UniqueContraintDoc y) => y.RowColumns));
			return uniqueContraintDoc;
		}).ToList();
	}
}
