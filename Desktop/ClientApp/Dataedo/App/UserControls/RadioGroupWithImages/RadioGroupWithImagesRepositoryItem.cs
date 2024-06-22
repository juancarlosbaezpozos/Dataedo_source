using System.Drawing;
using System.Reflection;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;

namespace Dataedo.App.UserControls.RadioGroupWithImages;

[UserRepositoryItem("RegisterCustomEdit")]
public class RadioGroupWithImagesRepositoryItem : RepositoryItemRadioGroup
{
	public const string CustomEditName = "RadioGroupWithImages";

	public override string EditorTypeName => "RadioGroupWithImages";

	static RadioGroupWithImagesRepositoryItem()
	{
		RegisterCustomEdit();
	}

	public static void RegisterCustomEdit()
	{
		Image image = null;
		try
		{
			image = (Bitmap)Image.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.CustomEditors.CustomEdit.bmp"));
		}
		catch
		{
		}
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("RadioGroupWithImages", typeof(RadioGroupWithImages), typeof(RadioGroupWithImagesRepositoryItem), typeof(RadioGroupWithImagesViewInfo), new RadioGroupPainter(), designTimeVisible: true, image));
	}
}
