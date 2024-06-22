using System.Diagnostics;

namespace Dataedo.App.ImportDescriptions.Tools;

[DebuggerDisplay("{CurrentValue} => {OverwriteValue}; {IsSelectedText}")]
public class DefaultFieldData : FieldData
{
	public bool IsImported { get; set; }

	public override bool IsChanged
	{
		get
		{
			if (IsImported)
			{
				return base.IsChanged;
			}
			return false;
		}
	}

	public override bool IsChangedIgnoringEmptyOverwrite
	{
		get
		{
			if (IsImported)
			{
				return base.IsChangedIgnoringEmptyOverwrite;
			}
			return false;
		}
	}

	public override bool IsSelectedAndChanged
	{
		get
		{
			if (IsImported)
			{
				return base.IsSelectedAndChanged;
			}
			return false;
		}
	}
}
