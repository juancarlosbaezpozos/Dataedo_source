using System.Threading;
using Dataedo.App.Tools;

namespace Dataedo.App.LoginFormTools.Tools.CallingApi;

internal class SuccessfulLoginSupport
{
	private Thread successfullUrlThread;

	public void CallUrl()
	{
		successfullUrlThread = new Thread((ThreadStart)delegate
		{
			Links.MakeGetRequestWithoutResponse(Links.AfterLoggedToRepositoryUrl(ProgramVersion.VersionWithBuild));
		});
		successfullUrlThread.IsBackground = true;
		successfullUrlThread.Start();
		successfullUrlThread.Join();
	}
}
