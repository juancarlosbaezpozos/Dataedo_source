using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class ProcedureRow : BasicRow
{
	public ObjectIdName[] Modules { get; set; }

	public string DescriptionPlain { get; set; }

	public new SharedObjectTypeEnum.ObjectType ObjectType { get; set; }

	public ProcedureRow(int id, string title, ObjectIdName[] modules, string description, string descriptionPlain)
	{
		base.Id = id;
		base.Title = title;
		Modules = modules;
		base.Description = description;
		DescriptionPlain = descriptionPlain;
	}
}
