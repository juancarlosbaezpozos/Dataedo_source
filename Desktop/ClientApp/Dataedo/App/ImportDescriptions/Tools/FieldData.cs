using System.Diagnostics;
using Dataedo.App.ImportDescriptions.Tools.Fields;

namespace Dataedo.App.ImportDescriptions.Tools;

[DebuggerDisplay("{CurrentValue} => {OverwriteValue}; {IsSelectedText}")]
public class FieldData
{
	private string overwriteValue;

	public bool IsSelected { get; set; }

	public string ChangeDescription => ChangeEnum.ToDisplayName(Change) + ((IsSelected || !IsChanged) ? string.Empty : "?");

	public string CurrentValue { get; set; }

	public string OverwriteValue
	{
		get
		{
			return overwriteValue;
		}
		set
		{
			ChangeEnum.Change change = Change;
			overwriteValue = value;
			if (change != ChangeEnum.Change.New && Change == ChangeEnum.Change.New)
			{
				IsSelected = true;
			}
		}
	}

	public bool IsCurrentValueEmpty => string.IsNullOrWhiteSpace(CurrentValue);

	public bool IsOverwriteValueEmpty => string.IsNullOrWhiteSpace(OverwriteValue);

	public virtual bool IsChanged => (CurrentValue?.Trim() ?? string.Empty) != (OverwriteValue?.Trim() ?? string.Empty);

	public virtual bool IsChangedIgnoringEmptyOverwrite
	{
		get
		{
			if (!IsOverwriteValueEmpty)
			{
				return IsChanged;
			}
			return false;
		}
	}

	public virtual bool IsSelectedAndChanged
	{
		get
		{
			if (IsSelected)
			{
				return IsChanged;
			}
			return false;
		}
	}

	public ChangeEnum.Change Change
	{
		get
		{
			if (string.IsNullOrWhiteSpace(CurrentValue) && !string.IsNullOrWhiteSpace(OverwriteValue))
			{
				return ChangeEnum.Change.New;
			}
			if (!string.IsNullOrWhiteSpace(CurrentValue) && string.IsNullOrWhiteSpace(OverwriteValue))
			{
				return ChangeEnum.Change.Erase;
			}
			if ((CurrentValue?.Trim() ?? string.Empty) == (OverwriteValue?.Trim() ?? string.Empty))
			{
				return ChangeEnum.Change.NoChange;
			}
			if (!string.IsNullOrWhiteSpace(CurrentValue) && !string.IsNullOrWhiteSpace(OverwriteValue))
			{
				return ChangeEnum.Change.Update;
			}
			return ChangeEnum.Change.NoChange;
		}
	}

	public void InitializeOverwriteValue(string value)
	{
		overwriteValue = value;
	}

	public void SelectIfNew()
	{
		if (Change == ChangeEnum.Change.New)
		{
			IsSelected = true;
		}
	}

	public void ClearCurrentData()
	{
		IsSelected = false;
		CurrentValue = null;
	}
}
