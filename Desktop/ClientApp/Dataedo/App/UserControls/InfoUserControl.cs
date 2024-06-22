using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Common.Interfaces;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class InfoUserControl : BaseUserControl
{
    private bool shouldLoadColorsAfterLoad = true;

    private IContainer components;

    private LabelControl infoLabel;

    private PanelControl infoPanelControl;

    private NonCustomizableLayoutControl infoLayoutControl;

    private LayoutControlGroup infoLayoutControlGroup;

    private LayoutControlItem infoLayoutControlItem;

    private string description { get; set; }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Bindable(true)]
    public string Description
    {
        get
        {
            return description;
        }
        set
        {
            description = value;
            infoLabel.Text = description;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Bindable(true)]
    public Image Image
    {
        get
        {
            return infoLayoutControlItem.Image;
        }
        set
        {
            infoLayoutControlItem.Image = value;
            infoLayoutControlItem.TextSize = new Size(5 + infoLayoutControlItem.Image.Width, infoLayoutControlItem.TextSize.Height);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Always)]
    [Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Bindable(true)]
    public Color BackgroundColor
    {
        get
        {
            return infoPanelControl.BackColor;
        }
        set
        {
            infoPanelControl.BackColor = value;
        }
    }

    public bool SaveFilter { get; internal set; }

    public InfoUserControl()
    {
        InitializeComponent();
        BackgroundColor = SkinsManager.CurrentSkin.DisabledFunctionalitiesBackColor;
        ForeColor = SkinsManager.CurrentSkin.DisabledFunctionalitiesForeColor;
    }

    private void InfoUserControl_Load(object sender, EventArgs e)
    {
        if (shouldLoadColorsAfterLoad)
        {
            BackgroundColor = SkinsManager.CurrentSkin.DisabledFunctionalitiesBackColor;
            ForeColor = SkinsManager.CurrentSkin.DisabledFunctionalitiesForeColor;
        }
    }

    public void CheckAndSetDeletedObjectProperties(IStatus row)
    {
        if (SynchronizeStateEnum.DBStringToState(row.Status) == SynchronizeStateEnum.SynchronizeState.Deleted)
        {
            BackgroundColor = SkinsManager.CurrentSkin.DeletedObjectPanelBackColor;
            ForeColor = SkinsManager.CurrentSkin.DeletedObjectPanelForeColor;
            base.Visible = true;
        }
        else
        {
            base.Visible = false;
        }
    }

    public void SetDeletedObjectProperties(bool isControlVisible = true)
    {
        BackgroundColor = SkinsManager.CurrentSkin.DeletedObjectPanelBackColor;
        ForeColor = SkinsManager.CurrentSkin.DeletedObjectPanelForeColor;
        base.Visible = isControlVisible;
    }

    public new void Hide()
    {
        base.Visible = false;
    }

    private void InfoLabel_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        if (e.MouseArgs.Button == MouseButtons.Left)
        {
            Links.OpenLink(e.Link);
        }
    }

    public void SetShouldLoadColorsAfterLoad(bool shouldLoadColorsAfterLoad)
    {
        this.shouldLoadColorsAfterLoad = shouldLoadColorsAfterLoad;
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
        this.infoPanelControl = new DevExpress.XtraEditors.PanelControl();
        this.infoLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
        this.infoLabel = new DevExpress.XtraEditors.LabelControl();
        this.infoLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
        this.infoLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
        ((System.ComponentModel.ISupportInitialize)this.infoPanelControl).BeginInit();
        this.infoPanelControl.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.infoLayoutControl).BeginInit();
        this.infoLayoutControl.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.infoLayoutControlGroup).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.infoLayoutControlItem).BeginInit();
        base.SuspendLayout();
        this.infoPanelControl.Appearance.Options.UseBackColor = true;
        this.infoPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
        this.infoPanelControl.Controls.Add(this.infoLayoutControl);
        this.infoPanelControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.infoPanelControl.Location = new System.Drawing.Point(0, 0);
        this.infoPanelControl.Name = "infoPanelControl";
        this.infoPanelControl.Size = new System.Drawing.Size(729, 48);
        this.infoPanelControl.TabIndex = 1;
        this.infoLayoutControl.AllowCustomization = false;
        this.infoLayoutControl.AutoScroll = false;
        this.infoLayoutControl.BackColor = System.Drawing.Color.Transparent;
        this.infoLayoutControl.Controls.Add(this.infoLabel);
        this.infoLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.infoLayoutControl.Location = new System.Drawing.Point(0, 0);
        this.infoLayoutControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
        this.infoLayoutControl.Name = "infoLayoutControl";
        this.infoLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(0, 365, 250, 350);
        this.infoLayoutControl.Root = this.infoLayoutControlGroup;
        this.infoLayoutControl.Size = new System.Drawing.Size(729, 48);
        this.infoLayoutControl.TabIndex = 5;
        this.infoLayoutControl.Text = "layoutControl1";
        this.infoLabel.AllowHtmlString = true;
        this.infoLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
        this.infoLabel.Location = new System.Drawing.Point(51, 7);
        this.infoLabel.Margin = new System.Windows.Forms.Padding(0);
        this.infoLabel.Name = "infoLabel";
        this.infoLabel.Size = new System.Drawing.Size(671, 20);
        this.infoLabel.StyleController = this.infoLayoutControl;
        this.infoLabel.TabIndex = 4;
        this.infoLabel.Text = "Test";
        this.infoLabel.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(InfoLabel_HyperlinkClick);
        this.infoLayoutControlGroup.AllowCustomizeChildren = false;
        this.infoLayoutControlGroup.CustomizationFormText = "infoLayoutControlGroup";
        this.infoLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
        this.infoLayoutControlGroup.GroupBordersVisible = false;
        this.infoLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.infoLayoutControlItem });
        this.infoLayoutControlGroup.Name = "Root";
        this.infoLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
        this.infoLayoutControlGroup.Size = new System.Drawing.Size(729, 48);
        this.infoLayoutControlGroup.TextVisible = false;
        this.infoLayoutControlItem.Control = this.infoLabel;
        this.infoLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
        this.infoLayoutControlItem.CustomizationFormText = " ";
        this.infoLayoutControlItem.ImageOptions.Image = Dataedo.App.Properties.Resources.about_16;
        this.infoLayoutControlItem.ImageOptions.ImageToTextDistance = 0;
        this.infoLayoutControlItem.Location = new System.Drawing.Point(0, 0);
        this.infoLayoutControlItem.Name = "infoLayoutControlItem";
        this.infoLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 2, 2, 2);
        this.infoLayoutControlItem.Size = new System.Drawing.Size(719, 38);
        this.infoLayoutControlItem.Text = " ";
        this.infoLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
        this.infoLayoutControlItem.TextSize = new System.Drawing.Size(37, 20);
        this.infoLayoutControlItem.TextToControlDistance = 5;
        base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.Controls.Add(this.infoPanelControl);
        base.Name = "InfoUserControl";
        base.Size = new System.Drawing.Size(729, 48);
        base.Load += new System.EventHandler(InfoUserControl_Load);
        ((System.ComponentModel.ISupportInitialize)this.infoPanelControl).EndInit();
        this.infoPanelControl.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.infoLayoutControl).EndInit();
        this.infoLayoutControl.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.infoLayoutControlGroup).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.infoLayoutControlItem).EndInit();
        base.ResumeLayout(false);
    }
}
