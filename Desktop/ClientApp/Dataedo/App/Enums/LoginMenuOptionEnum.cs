using System.ComponentModel;

namespace Dataedo.App.Enums;

public enum LoginMenuOptionEnum
{
	[Description("File")]
	File = 0,
	[Description("Repository")]
	Reopsitory = 1,
	[Description("Open file")]
	OpenProject = 2,
	[Description("Connect to repository")]
	ConnectToRepository = 3,
	[Description("Create file")]
	NewProject = 4,
	[Description("Create repository")]
	CreateRepository = 5
}
