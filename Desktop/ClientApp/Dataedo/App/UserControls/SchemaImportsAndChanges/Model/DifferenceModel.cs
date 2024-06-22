namespace Dataedo.App.UserControls.SchemaImportsAndChanges.Model;

public class DifferenceModel
{
	public string Name { get; set; }

	public object Before { get; set; }

	public object After { get; set; }

	public bool IsAdded
	{
		get
		{
			if (Before == null)
			{
				return After != null;
			}
			return false;
		}
	}

	public bool IsRemoved
	{
		get
		{
			if (Before != null)
			{
				return After == null;
			}
			return false;
		}
	}

	public bool IsChanged
	{
		get
		{
			if (Before != null || After == null)
			{
				object before = Before;
				if (before == null)
				{
					return false;
				}
				return !before.Equals(After);
			}
			return true;
		}
	}

	public DifferenceModel(string name, object before, object after)
	{
		Name = name;
		Before = before;
		After = after;
	}

	public DifferenceModel(string name, bool? before, bool? after)
	{
		Name = name;
		Before = ((!before.HasValue) ? null : (before.Value ? "Yes" : "No"));
		After = ((!after.HasValue) ? null : (after.Value ? "Yes" : "No"));
	}

	public DifferenceModel(string name, object before, object after, bool swapBeforeAndAfter)
		: this(name, before, after)
	{
		if (swapBeforeAndAfter)
		{
			object before2 = Before;
			Before = After;
			After = before2;
		}
	}

	public DifferenceModel(string name, bool? before, bool? after, bool swapBeforeAndAfter)
		: this(name, before, after)
	{
		if (swapBeforeAndAfter)
		{
			object before2 = Before;
			Before = After;
			After = before2;
		}
	}
}
