using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using DevExpress.DataProcessing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class LoginCaptionHorizontalListBoxControl : UserControl
{
	private IEnumerable<MenuOption> menuOptions;

	private IEnumerable<LayoutControlItem> menuItems;

	private bool headersVisible = true;

	private MouseEventHandler mouseClick;

	private MouseEventHandler mouseDoubleClick;

	private EventHandler selectedIndexChanged;

	private EventHandler selectedItemChanged;

	private KeyEventHandler keyDown;

	public bool LockProcessCmdKey;

	private IContainer components;

	private LabelControl captionLabelControl;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup;

	private LayoutControlItem captionLabelLayoutControlItem;

	private EmptySpaceItem emptySpaceItem;

	private ToolTipController toolTipController;

	private LabelControl subCaptionLabelControl;

	private LayoutControlItem subCaptionLabelLayoutControlItem;

	private NonCustomizableLayoutControl actionsLayoutControl;

	private LayoutControlGroup Root;

	private LayoutControlItem actionsLayoutControlLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public Size OptionSize { get; set; } = new Size(316, 316);


	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
	public VertAlignment OptionsLabelVerticalAlignment { get; set; } = VertAlignment.Top;


	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public bool HeadersVisible
	{
		get
		{
			return headersVisible;
		}
		set
		{
			if (headersVisible != value)
			{
				headersVisible = value;
				if (value)
				{
					captionLabelLayoutControlItem.Visibility = LayoutVisibility.Always;
					subCaptionLabelLayoutControlItem.Visibility = LayoutVisibility.Always;
					emptySpaceItem.Visibility = LayoutVisibility.Always;
					captionLabelControl.Visible = true;
				}
				else
				{
					captionLabelLayoutControlItem.Visibility = LayoutVisibility.Never;
					subCaptionLabelLayoutControlItem.Visibility = LayoutVisibility.Never;
					emptySpaceItem.Visibility = LayoutVisibility.Never;
					captionLabelControl.Visible = false;
				}
			}
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
	public string Caption
	{
		get
		{
			return captionLabelControl.Text;
		}
		set
		{
			captionLabelControl.Text = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public string SubCaption
	{
		get
		{
			return subCaptionLabelControl.Text;
		}
		set
		{
			if (!string.IsNullOrWhiteSpace(value))
			{
				subCaptionLabelLayoutControlItem.Visibility = LayoutVisibility.Always;
			}
			subCaptionLabelControl.Text = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public Image CaptionImage
	{
		get
		{
			return captionLabelLayoutControlItem.Image;
		}
		set
		{
			captionLabelLayoutControlItem.Image = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public string ToolTip
	{
		get
		{
			return captionLabelLayoutControlItem.OptionsToolTip.ToolTip;
		}
		set
		{
			captionLabelLayoutControlItem.OptionsToolTip.ToolTip = value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public new ContextMenuStrip ContextMenuStrip
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public int SelectedIndex { get; private set; } = -1;


	public LoginCaptionListBoxItemControl SelectedItem { get; private set; }

	public bool IsAnySelected => SelectedItem != null;

	[Browsable(true)]
	public new event MouseEventHandler MouseClick
	{
		add
		{
			mouseClick = (MouseEventHandler)Delegate.Combine(mouseClick, value);
		}
		remove
		{
			mouseClick = (MouseEventHandler)Delegate.Remove(mouseClick, value);
		}
	}

	[Browsable(true)]
	public new event MouseEventHandler MouseDoubleClick
	{
		add
		{
			mouseDoubleClick = (MouseEventHandler)Delegate.Combine(mouseDoubleClick, value);
		}
		remove
		{
			mouseDoubleClick = (MouseEventHandler)Delegate.Remove(mouseDoubleClick, value);
		}
	}

	[Browsable(true)]
	public new event KeyEventHandler KeyDown
	{
		add
		{
			keyDown = (KeyEventHandler)Delegate.Combine(keyDown, value);
		}
		remove
		{
			keyDown = (KeyEventHandler)Delegate.Remove(keyDown, value);
		}
	}

	[Browsable(true)]
	public event EventHandler SelectedItemChanged
	{
		add
		{
			selectedItemChanged = (EventHandler)Delegate.Combine(selectedItemChanged, value);
		}
		remove
		{
			selectedItemChanged = (EventHandler)Delegate.Remove(selectedItemChanged, value);
		}
	}

	public LoginCaptionHorizontalListBoxControl()
	{
		InitializeComponent();
		captionLabelControl.ForeColor = SkinsManager.CurrentSkin.LoginFormTitleForeColor;
	}

	public void SelectFirstIfAnyNotSelected()
	{
		if (!IsAnySelected)
		{
			SetSelected(0);
		}
	}

	public void SetDataSource(IEnumerable<MenuOption> menuOptions)
	{
		Size size3 = (actionsLayoutControl.MinimumSize = (actionsLayoutControl.MaximumSize = new Size(0, OptionSize.Height)));
		actionsLayoutControl.Size = new Size(actionsLayoutControl.Size.Width, OptionSize.Height);
		this.menuOptions = menuOptions;
		actionsLayoutControl.Root.Clear();
		List<LayoutControlItem> list = new List<LayoutControlItem>();
		LayoutControlGroup layoutControlGroup = new LayoutControlGroup();
		layoutControlGroup.GroupBordersVisible = false;
		LayoutControlItem layoutControlItem = null;
		MenuOption[] array = menuOptions.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			MenuOption menuOption = array[i];
			LayoutControlItem layoutControlItem2 = null;
			LoginCaptionListBoxItemControl loginCaptionListBoxItemControl = new LoginCaptionListBoxItemControl();
			if (menuOption.SvgImage != null)
			{
				loginCaptionListBoxItemControl.SvgImage = menuOption.SvgImage;
			}
			else
			{
				loginCaptionListBoxItemControl.Image = menuOption.Image;
			}
			loginCaptionListBoxItemControl.Label = menuOption.DisplayName;
			loginCaptionListBoxItemControl.LabelVerticalAlignment = OptionsLabelVerticalAlignment;
			loginCaptionListBoxItemControl.IsHelpIconVisible = menuOption.IsHelpIconVisible;
			loginCaptionListBoxItemControl.ToolTipHeader = menuOption.HelpIconToolTipHeader;
			loginCaptionListBoxItemControl.ToolTipText = menuOption.HelpIconToolTipText;
			Size size4 = (loginCaptionListBoxItemControl.Size = OptionSize);
			size3 = (loginCaptionListBoxItemControl.MinimumSize = (loginCaptionListBoxItemControl.MaximumSize = size4));
			loginCaptionListBoxItemControl.Click += Item_Click;
			loginCaptionListBoxItemControl.DoubleClick += Item_DoubleClick;
			loginCaptionListBoxItemControl.MouseClick += Item_MouseClick;
			loginCaptionListBoxItemControl.MouseDoubleClick += Item_MouseDoubleClick;
			if (layoutControlItem == null)
			{
				layoutControlItem2 = layoutControlGroup.AddItem();
				layoutControlItem2.Control = loginCaptionListBoxItemControl;
			}
			else
			{
				layoutControlItem2 = new LayoutControlItem(actionsLayoutControl, loginCaptionListBoxItemControl);
			}
			layoutControlItem2.TextVisible = false;
			layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			layoutControlItem2.SizeConstraintsType = SizeConstraintsType.Custom;
			LayoutControlItem layoutControlItem3 = layoutControlItem2;
			LayoutControlItem layoutControlItem4 = layoutControlItem2;
			size4 = (layoutControlItem2.Size = OptionSize);
			size3 = (layoutControlItem3.MinSize = (layoutControlItem4.MaxSize = size4));
			if (layoutControlItem != null)
			{
				layoutControlItem2.Move(layoutControlItem, InsertType.Right);
			}
			layoutControlItem = layoutControlItem2;
			list.Add(layoutControlItem2);
			if (i != array.Length - 1)
			{
				EmptySpaceItem obj = new EmptySpaceItem(layoutControlGroup);
				obj.Move(layoutControlItem, InsertType.Right);
				layoutControlItem = obj;
			}
		}
		actionsLayoutControl.Root.AddGroup(layoutControlGroup);
		menuItems = list;
	}

	public new bool Focus()
	{
		base.Focus();
		return true;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (!LockProcessCmdKey)
		{
			switch (keyData)
			{
			case Keys.Up:
			case Keys.Right:
				if (SelectedIndex < menuItems.Count() - 1)
				{
					SetSelected(SelectedIndex + 1);
				}
				return true;
			case Keys.Left:
			case Keys.Down:
				if (SelectedIndex >= 1)
				{
					SetSelected(SelectedIndex - 1);
				}
				return true;
			}
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void Item_Click(object sender, EventArgs e)
	{
		SetSelected((LoginCaptionListBoxItemControl)sender);
	}

	private void Item_DoubleClick(object sender, EventArgs e)
	{
		SetSelected((LoginCaptionListBoxItemControl)sender);
	}

	private void Item_MouseClick(object sender, MouseEventArgs e)
	{
		SetSelected((LoginCaptionListBoxItemControl)sender);
		base.ParentForm.BeginInvoke((Action)delegate
		{
			mouseClick(sender, e);
		});
	}

	private void Item_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		SetSelected((LoginCaptionListBoxItemControl)sender);
		base.ParentForm.BeginInvoke((Action)delegate
		{
			mouseDoubleClick(sender, e);
		});
	}

	private void SetSelected(LoginCaptionListBoxItemControl loginCaptionListBoxItemControl)
	{
		if (loginCaptionListBoxItemControl != SelectedItem)
		{
			LoginCaptionListBoxItemControl item = loginCaptionListBoxItemControl;
			SelectedItem = item;
			SelectedIndex = menuItems.IndexOf(menuItems.FirstOrDefault((LayoutControlItem x) => x.Control == item));
			menuItems.Where((LayoutControlItem x) => x.Control != item).ForEach(delegate(LayoutControlItem x)
			{
				(x.Control as LoginCaptionListBoxItemControl).SetNormal();
			});
			selectedItemChanged?.Invoke(this, null);
			item.SetSelected();
		}
	}

	private void SetSelected(int itemIndex)
	{
		SetSelected(menuItems.ElementAt(itemIndex).Control as LoginCaptionListBoxItemControl);
	}

	public void UnselectAll()
	{
		SelectedItem = null;
		SelectedIndex = -1;
		menuItems?.ForEach(delegate(LayoutControlItem x)
		{
			(x.Control as LoginCaptionListBoxItemControl).SetNormal();
		});
	}

	private void ImageListBoxControl_MouseClick(object sender, MouseEventArgs e)
	{
		base.ParentForm.BeginInvoke((Action)delegate
		{
			mouseClick(sender, e);
		});
	}

	private void ImageListBoxControl_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		base.ParentForm.BeginInvoke((Action)delegate
		{
			mouseDoubleClick(sender, e);
		});
	}

	private void listBoxControl_DrawItem(object sender, ListBoxDrawItemEventArgs e)
	{
		if (e.State != 0)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.ListSelectionBackColor;
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.actionsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.subCaptionLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.captionLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.captionLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.subCaptionLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.actionsLayoutControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.actionsLayoutControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.captionLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.subCaptionLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.actionsLayoutControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.Appearance.Options.UseTextOptions = true;
		this.toolTipController.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.toolTipController.AutoPopDelay = 32000;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.Controls.Add(this.actionsLayoutControl);
		this.layoutControl.Controls.Add(this.subCaptionLabelControl);
		this.layoutControl.Controls.Add(this.captionLabelControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(172, 583, 1038, 630);
		this.layoutControl.OptionsView.ShareLookAndFeelWithChildren = false;
		this.layoutControl.Root = this.layoutControlGroup;
		this.layoutControl.Size = new System.Drawing.Size(619, 476);
		this.layoutControl.TabIndex = 1;
		this.layoutControl.Text = "layoutControl1";
		this.layoutControl.ToolTipController = this.toolTipController;
		this.actionsLayoutControl.AllowCustomization = false;
		this.actionsLayoutControl.Location = new System.Drawing.Point(2, 110);
		this.actionsLayoutControl.Name = "actionsLayoutControl";
		this.actionsLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(496, 493, 653, 558);
		this.actionsLayoutControl.Root = this.Root;
		this.actionsLayoutControl.Size = new System.Drawing.Size(615, 332);
		this.actionsLayoutControl.TabIndex = 6;
		this.actionsLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(615, 332);
		this.Root.TextVisible = false;
		this.subCaptionLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 10f);
		this.subCaptionLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.subCaptionLabelControl.Appearance.Options.UseFont = true;
		this.subCaptionLabelControl.Appearance.Options.UseForeColor = true;
		this.subCaptionLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.subCaptionLabelControl.Location = new System.Drawing.Point(0, 72);
		this.subCaptionLabelControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.subCaptionLabelControl.Name = "subCaptionLabelControl";
		this.subCaptionLabelControl.Size = new System.Drawing.Size(619, 16);
		this.subCaptionLabelControl.TabIndex = 5;
		this.subCaptionLabelControl.Text = "Subcaption";
		this.captionLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 15.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.captionLabelControl.Appearance.ForeColor = System.Drawing.Color.LightGray;
		this.captionLabelControl.Appearance.Options.UseFont = true;
		this.captionLabelControl.Appearance.Options.UseForeColor = true;
		this.captionLabelControl.Appearance.Options.UseTextOptions = true;
		this.captionLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.captionLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.captionLabelControl.Location = new System.Drawing.Point(0, 0);
		this.captionLabelControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.captionLabelControl.LookAndFeel.UseDefaultLookAndFeel = false;
		this.captionLabelControl.Name = "captionLabelControl";
		this.captionLabelControl.Size = new System.Drawing.Size(619, 72);
		this.captionLabelControl.TabIndex = 0;
		this.captionLabelControl.Text = "Caption - line 1\r\nCaption - line 2";
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.captionLabelLayoutControlItem, this.emptySpaceItem, this.subCaptionLabelLayoutControlItem, this.emptySpaceItem1, this.actionsLayoutControlLayoutControlItem });
		this.layoutControlGroup.Name = "Root";
		this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup.Size = new System.Drawing.Size(619, 476);
		this.layoutControlGroup.TextVisible = false;
		this.captionLabelLayoutControlItem.Control = this.captionLabelControl;
		this.captionLabelLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
		this.captionLabelLayoutControlItem.CustomizationFormText = "captionLabelLayoutControlItem";
		this.captionLabelLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.captionLabelLayoutControlItem.MaxSize = new System.Drawing.Size(0, 72);
		this.captionLabelLayoutControlItem.MinSize = new System.Drawing.Size(1, 72);
		this.captionLabelLayoutControlItem.Name = "captionLabelLayoutControlItem";
		this.captionLabelLayoutControlItem.OptionsToolTip.AllowHtmlString = DevExpress.Utils.DefaultBoolean.True;
		this.captionLabelLayoutControlItem.OptionsToolTip.IconAllowHtmlString = DevExpress.Utils.DefaultBoolean.True;
		this.captionLabelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.captionLabelLayoutControlItem.Size = new System.Drawing.Size(619, 72);
		this.captionLabelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.captionLabelLayoutControlItem.Text = " ";
		this.captionLabelLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
		this.captionLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.captionLabelLayoutControlItem.TextToControlDistance = 0;
		this.captionLabelLayoutControlItem.TextVisible = false;
		this.emptySpaceItem.AllowHotTrack = false;
		this.emptySpaceItem.Location = new System.Drawing.Point(0, 88);
		this.emptySpaceItem.MaxSize = new System.Drawing.Size(0, 20);
		this.emptySpaceItem.MinSize = new System.Drawing.Size(104, 20);
		this.emptySpaceItem.Name = "emptySpaceItem";
		this.emptySpaceItem.Size = new System.Drawing.Size(619, 20);
		this.emptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.subCaptionLabelLayoutControlItem.Control = this.subCaptionLabelControl;
		this.subCaptionLabelLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.subCaptionLabelLayoutControlItem.MaxSize = new System.Drawing.Size(0, 16);
		this.subCaptionLabelLayoutControlItem.MinSize = new System.Drawing.Size(1, 16);
		this.subCaptionLabelLayoutControlItem.Name = "subCaptionLabelLayoutControlItem";
		this.subCaptionLabelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.subCaptionLabelLayoutControlItem.Size = new System.Drawing.Size(619, 16);
		this.subCaptionLabelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.subCaptionLabelLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
		this.subCaptionLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.subCaptionLabelLayoutControlItem.TextToControlDistance = 0;
		this.subCaptionLabelLayoutControlItem.TextVisible = false;
		this.subCaptionLabelLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 444);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(619, 32);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.actionsLayoutControlLayoutControlItem.Control = this.actionsLayoutControl;
		this.actionsLayoutControlLayoutControlItem.Location = new System.Drawing.Point(0, 108);
		this.actionsLayoutControlLayoutControlItem.MinSize = new System.Drawing.Size(50, 25);
		this.actionsLayoutControlLayoutControlItem.Name = "actionsLayoutControlLayoutControlItem";
		this.actionsLayoutControlLayoutControlItem.Size = new System.Drawing.Size(619, 336);
		this.actionsLayoutControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.actionsLayoutControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.actionsLayoutControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl);
		base.Name = "LoginCaptionHorizontalListBoxControl";
		base.Size = new System.Drawing.Size(619, 476);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.actionsLayoutControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.captionLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.subCaptionLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.actionsLayoutControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
