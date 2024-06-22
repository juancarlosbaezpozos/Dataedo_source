using System.Collections.Generic;

namespace Dataedo.App.Tools.Export.Universal.Storage;

public interface IReadStorage
{
	bool IsOpenable { get; }

	IEnumerable<string> ListFiles(string searchDir = "");

	bool HasFile(string path);

	byte[] ReadFile(string path);

	string ReadTextFile(string path);

	void Open();
}
