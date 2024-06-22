using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Import.DataLake;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using DevExpress.DataProcessing;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.UserControls;

public class InputObjectTypeUserControl : BaseUserControl
{
	private OverlayWindowOptions overlayWindowOptions;

	private IOverlaySplashScreenHandle overlaySplashScreenHandle;

	private IEnumerable<CheckButton> buttons;

	private IContainer components;

	private NonCustomizableLayoutControl mainNonCustomizableLayoutControl;

	private LayoutControlGroup rootMainLayoutControlGroup;

	private TableLayoutPanel tableLayoutPanel;

	private LayoutControlItem tableLayoutPanelLayoutControlItem;

	private CheckButton jsonCheckButton;

	private CheckButton csvCheckButton;

	private CheckButton xmlCheckButton;

	public bool IsAnyChecked => buttons.Any((CheckButton x) => x.Checked);

	public DataLakeTypeEnum.DataLakeType? DataLakeType
	{
		get
		{
			object obj = buttons.SingleOrDefault((CheckButton x) => x.Checked)?.Tag;
			if (obj != null)
			{
				return (DataLakeTypeEnum.DataLakeType?)Enum.Parse(typeof(DataLakeTypeEnum.DataLakeType), obj.ToString());
			}
			return null;
		}
		set
		{
			CheckButton button = ((!value.HasValue) ? null : buttons.SingleOrDefault((CheckButton x) => x.Tag.ToString() == Enum.GetName(typeof(DataLakeTypeEnum.DataLakeType), value)));
			if (button != null)
			{
				button.Checked = true;
			}
			buttons.Where((CheckButton x) => x != button).ForEach(delegate(CheckButton x)
			{
				x.Checked = false;
			});
		}
	}

	public event EventHandler CheckedChanged;

	public InputObjectTypeUserControl()
	{
		InitializeComponent();
		buttons = new CheckButton[3] { jsonCheckButton, csvCheckButton, xmlCheckButton };
	}

	private void InputObjectTypeUserControl_Load(object sender, EventArgs e)
	{
		overlayWindowOptions = new OverlayWindowOptions(backColor: base.ParentForm.BackColor, opacity: 0.7, fadeIn: true, fadeOut: true);
	}

	private void checkButton_CheckedChanged(object sender, EventArgs e)
	{
		CheckButton checkButton = sender as CheckButton;
		if (checkButton == null)
		{
			return;
		}
		if (checkButton.Checked)
		{
			buttons.Where((CheckButton x) => x != checkButton).ForEach(delegate(CheckButton x)
			{
				x.Checked = false;
			});
		}
		this.CheckedChanged?.Invoke(this, e);
	}

	public void ShowLoader()
	{
		HideLoader();
		overlaySplashScreenHandle = SplashScreenManager.ShowOverlayForm(this, overlayWindowOptions);
	}

	public void HideLoader()
	{
		if (overlaySplashScreenHandle != null)
		{
			SplashScreenManager.CloseOverlayForm(overlaySplashScreenHandle);
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
		this.mainNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
		this.jsonCheckButton = new DevExpress.XtraEditors.CheckButton();
		this.csvCheckButton = new DevExpress.XtraEditors.CheckButton();
		this.xmlCheckButton = new DevExpress.XtraEditors.CheckButton();
		this.rootMainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.tableLayoutPanelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).BeginInit();
		this.mainNonCustomizableLayoutControl.SuspendLayout();
		this.tableLayoutPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rootMainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableLayoutPanelLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainNonCustomizableLayoutControl.AllowCustomization = false;
		this.mainNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainNonCustomizableLayoutControl.Controls.Add(this.tableLayoutPanel);
		this.mainNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainNonCustomizableLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.mainNonCustomizableLayoutControl.Name = "mainNonCustomizableLayoutControl";
		this.mainNonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3659, 310, 650, 543);
		this.mainNonCustomizableLayoutControl.Root = this.rootMainLayoutControlGroup;
		this.mainNonCustomizableLayoutControl.Size = new System.Drawing.Size(696, 200);
		this.mainNonCustomizableLayoutControl.TabIndex = 0;
		this.mainNonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.tableLayoutPanel.ColumnCount = 5;
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10f));
		this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333f));
		this.tableLayoutPanel.Controls.Add(this.jsonCheckButton, 0, 0);
		this.tableLayoutPanel.Controls.Add(this.csvCheckButton, 2, 0);
		this.tableLayoutPanel.Controls.Add(this.xmlCheckButton, 4, 0);
		this.tableLayoutPanel.Location = new System.Drawing.Point(0, 10);
		this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
		this.tableLayoutPanel.Name = "tableLayoutPanel";
		this.tableLayoutPanel.RowCount = 1;
		this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100f));
		this.tableLayoutPanel.Size = new System.Drawing.Size(696, 180);
		this.tableLayoutPanel.TabIndex = 4;
		this.jsonCheckButton.Dock = System.Windows.Forms.DockStyle.Fill;
		this.jsonCheckButton.Location = new System.Drawing.Point(0, 0);
		this.jsonCheckButton.Margin = new System.Windows.Forms.Padding(0);
		this.jsonCheckButton.Name = "jsonCheckButton";
		this.jsonCheckButton.Size = new System.Drawing.Size(225, 180);
		this.jsonCheckButton.TabIndex = 0;
		this.jsonCheckButton.Tag = "JSON";
		this.jsonCheckButton.Text = "JSON";
		this.jsonCheckButton.CheckedChanged += new System.EventHandler(checkButton_CheckedChanged);
		this.csvCheckButton.Dock = System.Windows.Forms.DockStyle.Fill;
		this.csvCheckButton.Location = new System.Drawing.Point(235, 0);
		this.csvCheckButton.Margin = new System.Windows.Forms.Padding(0);
		this.csvCheckButton.Name = "csvCheckButton";
		this.csvCheckButton.Size = new System.Drawing.Size(225, 180);
		this.csvCheckButton.TabIndex = 1;
		this.csvCheckButton.Tag = "CSV";
		this.csvCheckButton.Text = "CSV";
		this.csvCheckButton.CheckedChanged += new System.EventHandler(checkButton_CheckedChanged);
		this.xmlCheckButton.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xmlCheckButton.Location = new System.Drawing.Point(470, 0);
		this.xmlCheckButton.Margin = new System.Windows.Forms.Padding(0);
		this.xmlCheckButton.Name = "xmlCheckButton";
		this.xmlCheckButton.Size = new System.Drawing.Size(226, 180);
		this.xmlCheckButton.StyleController = this.mainNonCustomizableLayoutControl;
		this.xmlCheckButton.TabIndex = 7;
		this.xmlCheckButton.Tag = "XML";
		this.xmlCheckButton.Text = "XML";
		this.xmlCheckButton.CheckedChanged += new System.EventHandler(checkButton_CheckedChanged);
		this.rootMainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.rootMainLayoutControlGroup.GroupBordersVisible = false;
		this.rootMainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.tableLayoutPanelLayoutControlItem });
		this.rootMainLayoutControlGroup.Name = "Root";
		this.rootMainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 10, 10);
		this.rootMainLayoutControlGroup.Size = new System.Drawing.Size(696, 200);
		this.rootMainLayoutControlGroup.TextVisible = false;
		this.tableLayoutPanelLayoutControlItem.Control = this.tableLayoutPanel;
		this.tableLayoutPanelLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutPanelLayoutControlItem.Name = "tableLayoutPanelLayoutControlItem";
		this.tableLayoutPanelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.tableLayoutPanelLayoutControlItem.Size = new System.Drawing.Size(696, 180);
		this.tableLayoutPanelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.tableLayoutPanelLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainNonCustomizableLayoutControl);
		base.Margin = new System.Windows.Forms.Padding(0);
		base.Name = "InputObjectTypeUserControl";
		base.Size = new System.Drawing.Size(696, 200);
		base.Load += new System.EventHandler(InputObjectTypeUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).EndInit();
		this.mainNonCustomizableLayoutControl.ResumeLayout(false);
		this.tableLayoutPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.rootMainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableLayoutPanelLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
