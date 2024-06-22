using System;
using System.Drawing;
using Dataedo.Model.Data.Interfaces;
using DevExpress.XtraEditors;

namespace Dataedo.App.UserControls;

internal class CustomComboBoxEdit : ComboBoxEdit
{
	public CustomComboBoxEdit()
	{
		base.SelectedValueChanged += CustomComboBoxEdit_SelectedValueChanged;
		base.DrawItem += CustomComboBoxEdit_DrawItem;
	}

	private void CustomComboBoxEdit_DrawItem(object sender, ListBoxDrawItemEventArgs e)
	{
		IEnabledable obj = e.Item as IEnabledable;
		if (obj != null && !obj.IsEnabled)
		{
			e.Cache.DrawString(e.Item.ToString(), e.Appearance.Font, new SolidBrush(Color.Gray), new Rectangle(e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height), e.Appearance.GetStringFormat());
			e.Handled = true;
		}
	}

	private void CustomComboBoxEdit_SelectedValueChanged(object sender, EventArgs e)
	{
		IEnabledable obj = SelectedItem as IEnabledable;
		if (obj != null && !obj.IsEnabled)
		{
			SelectedItem = null;
		}
	}

	protected override int FindItem(string text, bool autoComplete, int startIndex)
	{
		int num = -1;
		IEnabledable obj;
		do
		{
			num = base.FindItem(text, autoComplete, num + 1);
			if (num == -1)
			{
				break;
			}
			obj = base.Properties.Items[num] as IEnabledable;
		}
		while (obj != null && !obj.IsEnabled);
		return num;
	}
}
