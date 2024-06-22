namespace Dataedo.App.Helpers.CloudStorage.AzureStorage.Nodes;

public abstract class AzureStorageNode : FileLikeNode
{
	protected string _fullName;

	protected string _name;

	protected string _errorMessage;

	public string FullName => _fullName;

	public override string Name
	{
		get
		{
			if (!string.IsNullOrEmpty(_errorMessage))
			{
				return _name + " (" + _errorMessage + ")";
			}
			return _name;
		}
	}

	public void SetError(string message)
	{
		_errorMessage = message;
	}
}
