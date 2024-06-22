namespace Dataedo.App.Tools.Export.Universal.Storage;

public interface IWriteStorage
{
	void DeleteFile(string path);

	void DeleteDir(string path);

	void MoveFile(string oldPath, string newPath);

	void MoveDir(string oldPath, string newPath);

	void SaveFile(string path, byte[] content);

	void SaveFile(string path, string content);

	void AppendFile(string path, string content);
}
