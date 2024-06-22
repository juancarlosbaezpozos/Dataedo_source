using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Licences;
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

public class BannerControl : BaseUserControl
{
    private IContainer components;

    private NonCustomizableLayoutControl layoutControl;

    private LayoutControlGroup layoutControlGroup1;

    private LabelControl thirdLinkLabelControl;

    private LayoutControlItem pricingLayoutControlItem;

    private LabelControl trialLabelControlLine2;

    private LayoutControlItem trialLayoutControlItem;

    private LabelControl buySubscriptionLinkLabelControl1;

    private LabelControl trialLabelControlLine1;

    private LayoutControlItem layoutControlItem1;

    private TableLayoutPanel tableLayoutPanel1;

    private LayoutControlItem layoutControlItem2;

    private EmptySpaceItem emptySpaceItem2;

    private int TrialHeight => 135;

    public BannerControl()
    {
        InitializeComponent();
    }

    private void BannerControl_Load(object sender, EventArgs e)
    {
        SetTheme();
    }

    private void SetTheme()
    {
        buySubscriptionLinkLabelControl1.Appearance.Options.UseFont = true;
        BackColor = SkinsManager.CurrentSkin.ControlBackColor;
        ForeColor = SkinsManager.CurrentSkin.DisabledFunctionalitiesForeColor;
        trialLabelControlLine1.ForeColor = SkinsManager.CurrentSkin.DeletedObjectForeColor;
        trialLabelControlLine2.ForeColor = SkinsManager.CurrentSkin.DeletedObjectForeColor;
    }

    public void SetFunctionality()
    {
        SetTheme();
        if (!Licence.IsTrial)
        {
            base.Visible = false;
            return;
        }
        if (Licence.IsTrial)
        {
            base.Height = TrialHeight;
            trialLabelControlLine1.Enabled = true;
            trialLabelControlLine1.Text = StaticData.License.PackageName;
            if (StaticData.License.EndUtcDateTime.HasValue)
            {
                trialLabelControlLine2.Text = StaticData.License.GetEndDateSuffix(DateTime.UtcNow);
            }
            trialLayoutControlItem.Visibility = LayoutVisibility.Always;
            base.Visible = true;
        }
        else if (!StaticData.License.IsValid)
        {
            base.Height = TrialHeight;
            trialLabelControlLine2.Text = "<b>Your trial is invalid!</b>";
            trialLayoutControlItem.Visibility = LayoutVisibility.Always;
            base.Visible = true;
        }
        else
        {
            base.Visible = false;
        }
        pricingLayoutControlItem.Visibility = LayoutVisibility.Always;
    }

    private void ThirdLinkLabelControl_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            Links.OpenLink("https://account.dataedo.com", FindForm());
        }
    }

    private void BuySubscriptionLinkLabelControl1_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            Links.OpenLink(Links.DataedoPricingBuy, FindForm());
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
            this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buySubscriptionLinkLabelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.trialLabelControlLine1 = new DevExpress.XtraEditors.LabelControl();
            this.trialLabelControlLine2 = new DevExpress.XtraEditors.LabelControl();
            this.thirdLinkLabelControl = new DevExpress.XtraEditors.LabelControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.pricingLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            this.trialLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
            this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
            this.layoutControl.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pricingLayoutControlItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trialLayoutControlItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
            this.SuspendLayout();
            // 
            // layoutControl
            // 
            this.layoutControl.AllowCustomization = false;
            this.layoutControl.BackColor = System.Drawing.Color.Transparent;
            this.layoutControl.Controls.Add(this.tableLayoutPanel1);
            this.layoutControl.Controls.Add(this.trialLabelControlLine1);
            this.layoutControl.Controls.Add(this.trialLabelControlLine2);
            this.layoutControl.Controls.Add(this.thirdLinkLabelControl);
            this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutControl.Location = new System.Drawing.Point(0, 0);
            this.layoutControl.Name = "layoutControl";
            this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2272, 242, 250, 350);
            this.layoutControl.Root = this.layoutControlGroup1;
            this.layoutControl.Size = new System.Drawing.Size(325, 135);
            this.layoutControl.TabIndex = 2;
            this.layoutControl.Text = "layoutControl1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.buySubscriptionLinkLabelControl1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 47);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(321, 35);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // buySubscriptionLinkLabelControl1
            // 
            this.buySubscriptionLinkLabelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.buySubscriptionLinkLabelControl1.Appearance.BackColor = System.Drawing.Color.White;
            this.buySubscriptionLinkLabelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.buySubscriptionLinkLabelControl1.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(91)))), ((int)(((byte)(167)))));
            this.buySubscriptionLinkLabelControl1.Appearance.Options.UseBackColor = true;
            this.buySubscriptionLinkLabelControl1.Appearance.Options.UseFont = true;
            this.buySubscriptionLinkLabelControl1.Appearance.Options.UseForeColor = true;
            this.buySubscriptionLinkLabelControl1.Appearance.Options.UseTextOptions = true;
            this.buySubscriptionLinkLabelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.buySubscriptionLinkLabelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.buySubscriptionLinkLabelControl1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buySubscriptionLinkLabelControl1.Location = new System.Drawing.Point(95, 3);
            this.buySubscriptionLinkLabelControl1.MaximumSize = new System.Drawing.Size(130, 30);
            this.buySubscriptionLinkLabelControl1.MinimumSize = new System.Drawing.Size(130, 30);
            this.buySubscriptionLinkLabelControl1.Name = "buySubscriptionLinkLabelControl1";
            this.buySubscriptionLinkLabelControl1.Size = new System.Drawing.Size(130, 30);
            this.buySubscriptionLinkLabelControl1.TabIndex = 0;
            this.buySubscriptionLinkLabelControl1.Text = "Buy subscription";
            this.buySubscriptionLinkLabelControl1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BuySubscriptionLinkLabelControl1_MouseClick);
            // 
            // trialLabelControlLine1
            // 
            this.trialLabelControlLine1.Appearance.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.trialLabelControlLine1.Appearance.Options.UseFont = true;
            this.trialLabelControlLine1.Location = new System.Drawing.Point(95, 2);
            this.trialLabelControlLine1.Name = "trialLabelControlLine1";
            this.trialLabelControlLine1.Padding = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.trialLabelControlLine1.Size = new System.Drawing.Size(134, 20);
            this.trialLabelControlLine1.StyleController = this.layoutControl;
            this.trialLabelControlLine1.TabIndex = 8;
            this.trialLabelControlLine1.Text = "Trial (Data Dictionary)";
            // 
            // trialLabelControlLine2
            // 
            this.trialLabelControlLine2.AllowHtmlString = true;
            this.trialLabelControlLine2.Appearance.ForeColor = System.Drawing.Color.DimGray;
            this.trialLabelControlLine2.Appearance.Options.UseForeColor = true;
            this.trialLabelControlLine2.Appearance.Options.UseTextOptions = true;
            this.trialLabelControlLine2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.trialLabelControlLine2.Location = new System.Drawing.Point(7, 30);
            this.trialLabelControlLine2.Name = "trialLabelControlLine2";
            this.trialLabelControlLine2.Size = new System.Drawing.Size(311, 13);
            this.trialLabelControlLine2.StyleController = this.layoutControl;
            this.trialLabelControlLine2.TabIndex = 5;
            // 
            // thirdLinkLabelControl
            // 
            this.thirdLinkLabelControl.AllowHtmlString = true;
            this.thirdLinkLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.thirdLinkLabelControl.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(133)))), ((int)(((byte)(200)))));
            this.thirdLinkLabelControl.Appearance.Options.UseFont = true;
            this.thirdLinkLabelControl.Appearance.Options.UseForeColor = true;
            this.thirdLinkLabelControl.Appearance.Options.UseTextOptions = true;
            this.thirdLinkLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.thirdLinkLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
            this.thirdLinkLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.thirdLinkLabelControl.Location = new System.Drawing.Point(2, 90);
            this.thirdLinkLabelControl.Name = "thirdLinkLabelControl";
            this.thirdLinkLabelControl.Size = new System.Drawing.Size(321, 26);
            this.thirdLinkLabelControl.StyleController = this.layoutControl;
            this.thirdLinkLabelControl.TabIndex = 4;
            this.thirdLinkLabelControl.Text = "Log into Dataedo Account";
            this.thirdLinkLabelControl.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ThirdLinkLabelControl_MouseClick);
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.layoutControlGroup1.GroupBordersVisible = false;
            this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.pricingLayoutControlItem,
            this.trialLayoutControlItem,
            this.layoutControlItem1,
            this.layoutControlItem2,
            this.emptySpaceItem2});
            this.layoutControlGroup1.Name = "Root";
            this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
            this.layoutControlGroup1.Size = new System.Drawing.Size(325, 135);
            this.layoutControlGroup1.TextVisible = false;
            // 
            // pricingLayoutControlItem
            // 
            this.pricingLayoutControlItem.Control = this.thirdLinkLabelControl;
            this.pricingLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopCenter;
            this.pricingLayoutControlItem.Location = new System.Drawing.Point(0, 84);
            this.pricingLayoutControlItem.MinSize = new System.Drawing.Size(128, 25);
            this.pricingLayoutControlItem.Name = "pricingLayoutControlItem";
            this.pricingLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 6, 6);
            this.pricingLayoutControlItem.Size = new System.Drawing.Size(325, 38);
            this.pricingLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.pricingLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
            this.pricingLayoutControlItem.TextVisible = false;
            // 
            // trialLayoutControlItem
            // 
            this.trialLayoutControlItem.Control = this.trialLabelControlLine2;
            this.trialLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopCenter;
            this.trialLayoutControlItem.Location = new System.Drawing.Point(0, 24);
            this.trialLayoutControlItem.MaxSize = new System.Drawing.Size(315, 21);
            this.trialLayoutControlItem.MinSize = new System.Drawing.Size(315, 21);
            this.trialLayoutControlItem.Name = "trialLayoutControlItem";
            this.trialLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 6, 2);
            this.trialLayoutControlItem.Size = new System.Drawing.Size(325, 21);
            this.trialLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.trialLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
            this.trialLayoutControlItem.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.layoutControlItem1.Control = this.trialLabelControlLine1;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(325, 24);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // layoutControlItem2
            // 
            this.layoutControlItem2.Control = this.tableLayoutPanel1;
            this.layoutControlItem2.Location = new System.Drawing.Point(0, 45);
            this.layoutControlItem2.Name = "layoutControlItem2";
            this.layoutControlItem2.Size = new System.Drawing.Size(325, 39);
            this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem2.TextVisible = false;
            // 
            // emptySpaceItem2
            // 
            this.emptySpaceItem2.AllowHotTrack = false;
            this.emptySpaceItem2.Location = new System.Drawing.Point(0, 122);
            this.emptySpaceItem2.MaxSize = new System.Drawing.Size(0, 13);
            this.emptySpaceItem2.MinSize = new System.Drawing.Size(10, 13);
            this.emptySpaceItem2.Name = "emptySpaceItem2";
            this.emptySpaceItem2.Size = new System.Drawing.Size(325, 13);
            this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
            this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
            // 
            // BannerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(243)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.layoutControl);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "BannerControl";
            this.Size = new System.Drawing.Size(325, 135);
            this.Load += new System.EventHandler(this.BannerControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
            this.layoutControl.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pricingLayoutControlItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trialLayoutControlItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
            this.ResumeLayout(false);

    }
}
