using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Dataedo.App.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.4.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

	public static Settings Default => defaultInstance;

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("")]
	public string DocumentationSaveDirectoryPath
	{
		get
		{
			return (string)this["DocumentationSaveDirectoryPath"];
		}
		set
		{
			this["DocumentationSaveDirectoryPath"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("0, 0")]
	public Point WindowLocation
	{
		get
		{
			return (Point)this["WindowLocation"];
		}
		set
		{
			this["WindowLocation"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("1024, 680")]
	public Size WindowSize
	{
		get
		{
			return (Size)this["WindowSize"];
		}
		set
		{
			this["WindowSize"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool IsMaximized
	{
		get
		{
			return (bool)this["IsMaximized"];
		}
		set
		{
			this["IsMaximized"] = value;
		}
	}

	private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
	{
	}

	private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
	{
	}
}
