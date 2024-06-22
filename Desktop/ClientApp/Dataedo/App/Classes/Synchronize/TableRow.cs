using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class TableRow : BasicRow
{
	public int DatabaseId { get; set; }

	public string DatabaseTitle { get; set; }

	public string Schema { get; set; }

	public string Location { get; set; }

	public ObjectIdName[] Modules { get; set; }

	public string DescriptionPlain { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype Subtype { get; set; }

	public TableRow(int id, string title, string location, ObjectIdName[] modules, string description, string descriptionSearch)
	{
		base.Id = id;
		base.Title = title;
		Location = location;
		Modules = modules;
		base.Description = description;
		DescriptionPlain = descriptionSearch;
	}

	public TableRow(ObjectWithModulesObject row)
	{
		base.Id = row.Id;
		base.Name = row.Name;
		Schema = row.Schema;
		DatabaseId = row.DatabaseId;
		DatabaseTitle = row.DatabaseTitle;
		base.Title = row.Title;
		base.Source = UserTypeEnum.ObjectToType(PrepareValue.ToString(row.Source)) ?? UserTypeEnum.UserType.DBMS;
		base.ObjectType = SharedObjectTypeEnum.StringToType(row.ObjectType) ?? SharedObjectTypeEnum.ObjectType.UnresolvedEntity;
		Subtype = SharedObjectSubtypeEnum.StringToType(base.ObjectType, SharedObjectSubtypeEnum.DisplayStringToString(PrepareValue.ToString(row.Subtype)));
	}
}
