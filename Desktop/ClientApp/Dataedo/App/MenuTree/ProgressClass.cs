namespace Dataedo.App.MenuTree;

public class ProgressClass
{
	public int Points { get; set; }

	public int TotalPoints { get; set; }

	public ProgressClass(int points, int totalPoints)
	{
		Points = points;
		TotalPoints = totalPoints;
	}

	public ProgressClass()
	{
		Points = 0;
		TotalPoints = 0;
	}
}
