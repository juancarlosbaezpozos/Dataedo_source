using System;
using DevExpress.XtraWizard;

namespace Dataedo.App.Helpers.Controls;

public class WizardPageInfo
{
	public bool SkipPage { get; set; }

	public BaseWizardPage PrevPage { get; set; }

	public BaseWizardPage NextPage { get; set; }

	public static WizardPageInfo GetOrCreate(BaseWizardPage wizardPage)
	{
		if (wizardPage == null)
		{
			throw new ArgumentNullException("wizardPage");
		}
		if (wizardPage.Tag is WizardPageInfo result)
		{
			return result;
		}
		return (WizardPageInfo)(wizardPage.Tag = new WizardPageInfo());
	}

	public static void InitWizardPages(WizardControl wizardControl)
	{
		foreach (BaseWizardPage page in wizardControl.Pages)
		{
			page.Tag = new WizardPageInfo();
		}
		for (int i = 1; i < wizardControl.Pages.Count; i++)
		{
			GetOrCreate(wizardControl.Pages[i]).PrevPage = wizardControl.Pages[i - 1];
			GetOrCreate(wizardControl.Pages[i - 1]).NextPage = wizardControl.Pages[i];
		}
	}
}
