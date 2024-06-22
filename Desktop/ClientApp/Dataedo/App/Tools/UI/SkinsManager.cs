using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools.UI.Skins;
using Dataedo.App.Tools.UI.Skins.Base;
using Dataedo.CustomMessageBox;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.XtraEditors;

namespace Dataedo.App.Tools.UI;

internal class SkinsManager
{
	private class SkinPalettePair
	{
		public string SkinName { get; private set; }

		public string Palette { get; private set; }

		public SkinPalettePair(string skinName, string palette)
		{
			SkinName = skinName;
			Palette = palette;
		}
	}

	private static string skinName;

	private static string paletteName;

	private static bool? isFormattedFieldsLightSkinEnabled;

	private static string ApplicationSkinName
	{
		get
		{
			return LastConnectionInfo.LOGIN_INFO.SkinName;
		}
		set
		{
			LastConnectionInfo.LOGIN_INFO.SkinName = value;
		}
	}

	private static string ApplicationPalette
	{
		get
		{
			return LastConnectionInfo.LOGIN_INFO.SkinPalette;
		}
		set
		{
			LastConnectionInfo.LOGIN_INFO.SkinPalette = value;
		}
	}

	public static bool IsFormattedFieldsLightSkinEnabled
	{
		get
		{
			if (!isFormattedFieldsLightSkinEnabled.HasValue)
			{
				isFormattedFieldsLightSkinEnabled = LastConnectionInfo.LOGIN_INFO?.IsFormattedFieldsLightSkinEnabled ?? (!CurrentSkin.IsDarkTheme);
			}
			return isFormattedFieldsLightSkinEnabled.Value;
		}
		set
		{
			isFormattedFieldsLightSkinEnabled = value;
		}
	}

	public static BaseSkin DefaultSkin => new OfficeWhite();

	public static BaseSkin DarkSkin => new VsDark();

	private static List<BaseSkin> AvailableSkins => new List<BaseSkin> { DefaultSkin, DarkSkin };

	public static BaseSkin CurrentSkin { get; private set; } = DefaultSkin;


	public static bool IsCurrentSkinDark
	{
		get
		{
			if (CurrentSkin.SkinName == VsDark.SkinNameValue)
			{
				return CurrentSkin.Palette == VsDark.PaletteValue;
			}
			return false;
		}
	}

	public static bool IsConfigSkinAsCurrent
	{
		get
		{
			if (CurrentSkin.Palette == ApplicationPalette)
			{
				return CurrentSkin.SkinName == ApplicationSkinName;
			}
			return false;
		}
	}

	public static bool IsConfigSkinDark
	{
		get
		{
			if (ApplicationSkinName == VsDark.SkinNameValue)
			{
				return ApplicationPalette == VsDark.PaletteValue;
			}
			return false;
		}
	}

	public static void SwitchTheme(bool showMessage = true)
	{
		BaseSkin currentSkin = CurrentSkin;
		if (currentSkin.SkinName == VsDark.SkinNameValue && currentSkin.Palette == VsDark.PaletteValue)
		{
			SaveSkinSettings(OfficeWhite.SkinNameValue, OfficeWhite.PaletteValue, showMessage);
		}
		else
		{
			SaveSkinSettings(VsDark.SkinNameValue, VsDark.PaletteValue, showMessage);
		}
	}

	public static void SetSkin()
	{
		try
		{
			skinName = (string.IsNullOrEmpty(ApplicationSkinName) ? DefaultSkin.SkinName : ApplicationSkinName);
			paletteName = (string.IsNullOrEmpty(ApplicationPalette) ? DefaultSkin.Palette : ApplicationPalette);
			if (!string.IsNullOrEmpty(skinName) && AvailableSkins.Any((BaseSkin x) => x.SkinName == skinName && x.Palette == paletteName))
			{
				UserLookAndFeel.Default.SetSkinStyle(skinName, paletteName);
			}
			else
			{
				UserLookAndFeel.Default.SetSkinStyle(DefaultSkin.SkinName, DefaultSkin.Palette);
				skinName = DefaultSkin.SkinName;
				paletteName = DefaultSkin.Palette;
			}
		}
		catch
		{
			UserLookAndFeel.Default.SetSkinStyle(DefaultSkin.SkinName, DefaultSkin.Palette);
			skinName = DefaultSkin.SkinName;
			paletteName = DefaultSkin.Palette;
		}
		SetCurrentSkinCustomization();
		if (IsActiveSkinSet())
		{
			SkinColors.SetStoredColors();
		}
	}

	public static bool IsActiveSkinSet()
	{
		UserLookAndFeel activeLookAndFeel = UserLookAndFeel.Default.ActiveLookAndFeel;
		if (activeLookAndFeel.SkinName == skinName)
		{
			return activeLookAndFeel.ActiveSvgPaletteName == paletteName;
		}
		return false;
	}

	public static void SetPalette(UserLookAndFeel lookAndFeel)
	{
		try
		{
			if (SkinCollectionHelper.GetSkinCategory(UserLookAndFeel.Default.SkinName) == SkinCategory.SVG)
			{
				lookAndFeel.SetSkinStyle(lookAndFeel.ActiveSkinName, ApplicationPalette);
			}
			else
			{
				lookAndFeel.SetSkinStyle(ApplicationSkinName, ApplicationPalette);
			}
		}
		catch
		{
		}
	}

	public static bool SaveSkinSettings(string skinName, string palette, bool showMessage)
	{
		try
		{
			ApplicationSkinName = skinName;
			ApplicationPalette = palette;
			LastConnectionInfo.LOGIN_INFO.IsFormattedFieldsLightSkinEnabled = !(skinName == VsDark.SkinNameValue) || !(palette == VsDark.PaletteValue);
			LastConnectionInfo.Save();
			if (showMessage)
			{
				CustomMessageBoxForm.Show("New theme will be applied on restart.", "Changing theme", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
			}
			return true;
		}
		catch (Exception ex)
		{
			CustomMessageBoxForm.Show("Error while changing theme.", "Changing theme", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.ToString());
			return false;
		}
	}

	private static void SetCurrentSkinCustomization()
	{
		BaseSkin baseSkin = AvailableSkins.FirstOrDefault((BaseSkin x) => x.SkinName == UserLookAndFeel.Default.SkinName && x.Palette == UserLookAndFeel.Default.ActiveSvgPaletteName);
		if (baseSkin == null)
		{
			baseSkin = DefaultSkin;
		}
		CurrentSkin = baseSkin;
		SetResources();
	}

	public static void SetResources()
	{
		SetResources(CurrentSkin.ResourceManager);
	}

	public static void SetResources(ResourceManager resourcesManager)
	{
		typeof(Resources).GetField("resourceMan", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, resourcesManager);
	}

	public static void SetToggleSwitchTheme(ToggleSwitch toggleSwitch)
	{
		if (toggleSwitch != null)
		{
			if (IsCurrentSkinDark)
			{
				toggleSwitch.LookAndFeel.UseDefaultLookAndFeel = false;
				toggleSwitch.LookAndFeel.SkinName = "My Basic Dark";
			}
			else
			{
				toggleSwitch.LookAndFeel.UseDefaultLookAndFeel = false;
				toggleSwitch.LookAndFeel.SkinName = "My Basic";
			}
		}
	}
}
