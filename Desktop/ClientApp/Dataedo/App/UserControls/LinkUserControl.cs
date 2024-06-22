using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class LinkUserControl : BaseUserControl
{
    private const int PADDING = 14;

    private Action afterLinkClick;

    private IContainer components;

    private NonCustomizableLayoutControl layoutControl1;

    private LayoutControlGroup layoutControlGroup1;

    private LabelControl hyperLinkEdit;

    private LayoutControlItem layoutControlItem;

    private PanelControl linkPanelControl;

    [Browsable(true)]
    public event HyperlinkClickEventHandler HyperlinkClick
    {
        add
        {
            hyperLinkEdit.HyperlinkClick += value;
        }
        remove
        {
            hyperLinkEdit.HyperlinkClick -= value;
        }
    }

    public LinkUserControl()
    {
        InitializeComponent();
    }

    public void SetImage(Bitmap bitmap)
    {
        layoutControlItem.Image = bitmap;
    }

    public void SetParameters(bool enabled, string description, Bitmap bitmap, Action afterLinkClick = null)
    {
        if (enabled)
        {
            base.Visible = false;
        }
        else
        {
            BackColor = SkinsManager.CurrentSkin.DisabledFunctionalitiesBackColor;
            hyperLinkEdit.ForeColor = SkinsManager.CurrentSkin.DisabledFunctionalitiesForeColor;
            SetImage(bitmap);
            base.Visible = true;
            hyperLinkEdit.Text = description;
            int num = (int)CreateGraphics().MeasureString(hyperLinkEdit.Text, hyperLinkEdit.Font).Height;
            if (num > base.Height)
            {
                base.Height = num + 14;
            }
            MaximumSize = new Size(0, base.Height);
        }
        this.afterLinkClick = afterLinkClick;
    }

    private void hyperLinkEdit_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        if (e.MouseArgs.Button == MouseButtons.Left)
        {
            Links.OpenCTALink(e.Link, this, afterLogin: true, afterLinkClick, FindForm());
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
        this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
        this.hyperLinkEdit = new DevExpress.XtraEditors.LabelControl();
        this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
        this.layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        this.linkPanelControl = new DevExpress.XtraEditors.PanelControl();
        ((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
        this.layoutControl1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlItem).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.linkPanelControl).BeginInit();
        this.linkPanelControl.SuspendLayout();
        base.SuspendLayout();
        this.layoutControl1.AllowCustomization = false;
        this.layoutControl1.Controls.Add(this.hyperLinkEdit);
        this.layoutControl1.Cursor = System.Windows.Forms.Cursors.Default;
        this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.layoutControl1.Location = new System.Drawing.Point(0, 0);
        this.layoutControl1.Name = "layoutControl1";
        this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2072, 262, 250, 350);
        this.layoutControl1.Root = this.layoutControlGroup1;
        this.layoutControl1.Size = new System.Drawing.Size(729, 36);
        this.layoutControl1.TabIndex = 0;
        this.layoutControl1.Text = "layoutControl1";
        this.hyperLinkEdit.AllowHtmlString = true;
        this.hyperLinkEdit.Appearance.BackColor = System.Drawing.Color.Transparent;
        this.hyperLinkEdit.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.hyperLinkEdit.Appearance.ForeColor = System.Drawing.Color.FromArgb(31, 71, 163);
        this.hyperLinkEdit.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
        this.hyperLinkEdit.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        this.hyperLinkEdit.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
        this.hyperLinkEdit.Cursor = System.Windows.Forms.Cursors.Default;
        this.hyperLinkEdit.Location = new System.Drawing.Point(37, 2);
        this.hyperLinkEdit.Name = "hyperLinkEdit";
        this.hyperLinkEdit.Size = new System.Drawing.Size(690, 32);
        this.hyperLinkEdit.StyleController = this.layoutControl1;
        this.hyperLinkEdit.TabIndex = 4;
        this.hyperLinkEdit.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(hyperLinkEdit_HyperlinkClick);
        this.layoutControlGroup1.CaptionImageLocation = DevExpress.Utils.GroupElementLocation.BeforeText;
        this.layoutControlGroup1.ContentImageAlignment = System.Drawing.ContentAlignment.MiddleLeft;
        this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
        this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
        this.layoutControlGroup1.GroupBordersVisible = false;
        this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem });
        this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
        this.layoutControlGroup1.Name = "Root";
        this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
        this.layoutControlGroup1.Size = new System.Drawing.Size(729, 36);
        this.layoutControlGroup1.TextVisible = false;
        this.layoutControlItem.Control = this.hyperLinkEdit;
        this.layoutControlItem.Image = Dataedo.App.Properties.Resources.upgrade_24;
        this.layoutControlItem.Location = new System.Drawing.Point(0, 0);
        this.layoutControlItem.MinSize = new System.Drawing.Size(10, 13);
        this.layoutControlItem.Name = "layoutControlItem";
        this.layoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 2, 2, 2);
        this.layoutControlItem.Size = new System.Drawing.Size(729, 36);
        this.layoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
        this.layoutControlItem.Text = " ";
        this.layoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
        this.layoutControlItem.TextSize = new System.Drawing.Size(32, 24);
        this.layoutControlItem.TextToControlDistance = 0;
        this.linkPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.linkPanelControl.Controls.Add(this.layoutControl1);
        this.linkPanelControl.Cursor = System.Windows.Forms.Cursors.Default;
        this.linkPanelControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.linkPanelControl.Location = new System.Drawing.Point(0, 0);
        this.linkPanelControl.Name = "linkPanelControl";
        this.linkPanelControl.Size = new System.Drawing.Size(729, 36);
        this.linkPanelControl.TabIndex = 0;
        base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.Controls.Add(this.linkPanelControl);
        base.Name = "LinkUserControl";
        base.Size = new System.Drawing.Size(729, 36);
        ((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
        this.layoutControl1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlItem).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.linkPanelControl).EndInit();
        this.linkPanelControl.ResumeLayout(false);
        base.ResumeLayout(false);
    }
}
