using System.Text;
using Dataedo.App.LoginFormTools.Tools.Recent;

namespace Dataedo.App.LoginFormTools.Tools.Repository;

internal class RepositoryOperationCompleted
{
	public string Title { get; set; }

	public string Description { get; set; }

	public StringBuilder Exceptions { get; set; }

	public RecentItemModel RecentItemModel { get; set; }
}
