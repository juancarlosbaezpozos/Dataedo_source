using System.Diagnostics;
using Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator.Data;

namespace Dataedo.App.LoginFormTools.Tools.ScriptsSupport;

[DebuggerDisplay("{Script}")]
public class Instruction
{
	public RepositoryVersion RepositoryVersion { get; set; }

	public string Script { get; set; }

	public Instruction(string script)
	{
		Script = script;
	}

	public Instruction(string script, RepositoryVersion repositoryVersion)
		: this(script)
	{
		RepositoryVersion = repositoryVersion;
	}
}
