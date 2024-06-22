using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Dataedo.App.UserControls.RadioGroupWithImages;

public class RadioGroupWithImages : RadioGroup
{
	public delegate void CustomItemImageHandler(object sender, CustomItemImageEventArgs e);

	public override string EditorTypeName => "RadioGroupWithImages";

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RadioGroupWithImagesRepositoryItem Properties => base.Properties as RadioGroupWithImagesRepositoryItem;

	public event CustomItemImageHandler CustomItemImage;

	static RadioGroupWithImages()
	{
		RadioGroupWithImagesRepositoryItem.RegisterCustomEdit();
	}

	public Image GetItemImage(RadioGroupItem item)
	{
		return RaiseCustomItemImage(item);
	}

	protected Image RaiseCustomItemImage(RadioGroupItem item)
	{
		if (this.CustomItemImage != null)
		{
			CustomItemImageEventArgs customItemImageEventArgs = new CustomItemImageEventArgs
			{
				Image = null,
				Item = item
			};
			this.CustomItemImage(this, customItemImageEventArgs);
			return customItemImageEventArgs.Image;
		}
		return null;
	}
}
