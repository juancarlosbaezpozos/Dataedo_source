namespace Dataedo.App.Tools.TemplateEditor;

public class FileSourceAndDestination
{
	public string Source { get; private set; }

	public string Destination { get; private set; }

	public FileSourceAndDestination(string source, string destination)
	{
		Source = source;
		Destination = destination;
	}
}
