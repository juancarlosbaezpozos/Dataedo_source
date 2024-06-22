using System.Drawing;
using DevExpress.XtraEditors.Controls;

namespace Dataedo.App.UserControls.RadioGroupWithImages;

public class CustomItemImageEventArgs
{
	private RadioGroupItem _Item;

	private Image _Image;

	public Image Image
	{
		get
		{
			return _Image;
		}
		set
		{
			_Image = value;
		}
	}

	public RadioGroupItem Item
	{
		get
		{
			return _Item;
		}
		set
		{
			_Item = value;
		}
	}
}
