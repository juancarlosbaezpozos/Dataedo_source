namespace Dataedo.App.Classes.Synchronize;

public class DatabaseSyncRow : BasicRow
{
	public string DescriptionPlain { get; set; }

	public DatabaseSyncRow(int id, string name, string title, string description, string descriptionPlain)
	{
		base.Id = id;
		base.Name = name;
		base.Title = title;
		base.Description = description;
		DescriptionPlain = descriptionPlain;
	}
}
