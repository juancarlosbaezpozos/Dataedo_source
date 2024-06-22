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
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class TriggerDoc : ObjectDoc
{
	public string When { get; set; }

	public Bitmap Icon { get; set; }

	public string Script { get; set; }

	public override string NewLineSeparator => ObjectDoc.HtmlNewLineSeparator;

	public override string CustomFieldsStringValuesSeparator => NewLineSeparator;

	public TriggerDoc()
	{
	}

	public TriggerDoc(DocGeneratingOptions docGeneratingOptions, DatabaseDoc database, TableViewDoc table, TriggerObject row)
		: base(docGeneratingOptions, row, SharedObjectTypeEnum.ObjectType.Trigger)
	{
		base.ObjectSubtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Trigger, PrepareValue.ToString(row.Type));
		When = TriggerRow.GetWhenRunDetails(row);
		string text = SharedObjectSubtypeEnum.TypeToString(base.ObjectType, base.ObjectSubtype).ToLower();
		string text2 = (row.Disabled ? "_disabled" : "_active");
		string text3 = "_16";
		Icon = Resources.ResourceManager.GetObject(text + text2 + text3) as Bitmap;
		if (Icon == null)
		{
			text = SharedObjectSubtypeEnum.TypeToString(base.ObjectType, null).ToLower();
			Icon = Resources.ResourceManager.GetObject(text + text2 + text3) as Bitmap;
		}
		base.IdString = PdfLinksSupport.CreateIdString(database, table, row);
		if (!docGeneratingOptions.ExcludedObjects.Any((ObjectTypeHierarchy x) => x.ParentObjectType == table.ObjectType && x.IsType(SharedObjectTypeEnum.ObjectType.Trigger, SharedObjectTypeEnum.ObjectType.Script)))
		{
			Script = docGeneratingOptions.ColorizeSyntax(PrepareValue.ToString(row.Definition), "SQL", 7);
		}
	}

	public static BindingList<TriggerDoc> GetTriggers(DocGeneratingOptions docGeneratingOptions, DatabaseDoc database, TableViewDoc table, Form owner = null)
	{
		try
		{
			return new BindingList<TriggerDoc>(new List<TriggerDoc>(from trigger in DB.Trigger.GetDataByTable(table.Id.Value, notDeletedOnly: true)
				select new TriggerDoc(docGeneratingOptions, database, table, trigger)));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting triggers from the table.", owner);
			return null;
		}
	}
}
