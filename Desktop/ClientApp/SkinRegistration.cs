using System.ComponentModel;
using DevExpress.Skins;
using DevExpress.UserSkins;

public class SkinRegistration : Component
{
    public SkinRegistration()
    {
        SkinManager.Default.RegisterAssembly(typeof(Customized).Assembly);
        SkinManager.Default.RegisterAssembly(typeof(ToggleSwitch).Assembly);
    }
}
