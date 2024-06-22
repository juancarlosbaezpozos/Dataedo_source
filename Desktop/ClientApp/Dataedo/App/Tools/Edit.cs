using Dataedo.App.Classes;
using DevExpress.XtraEditors;

namespace Dataedo.App.Tools;

public class Edit : BasicEdit, IEdit
{
	private TextEdit labelControl;

	private bool _isEdited;

	public override bool IsEdited
	{
		get
		{
			return _isEdited;
		}
		set
		{
			_isEdited = value;
			CommonFunctionsPanels.SetLabelTitle(_isEdited, labelControl);
		}
	}

	public Edit(TextEdit labelControl)
	{
		this.labelControl = labelControl;
	}
}
