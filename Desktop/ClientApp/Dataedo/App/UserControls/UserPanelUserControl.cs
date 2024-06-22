using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.API.Enums;
using Dataedo.App.API.Models;
using Dataedo.App.Tools;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class UserPanelUserControl : BaseUserControl
{
    private IContainer components;

    private NonCustomizableLayoutControl mainLayoutControl;

    private LayoutControlGroup mainLayoutControlGroup;

    private LabelControl manageAccountLabelControl;

    private LabelControl signWithDifferentAccountLabelControl;

    private LabelControl checkAvailableLicensesLabelControl;

    private EmptySpaceItem emptySpaceItem;

    private LayoutControlItem checkAvailableLicensesLabelControlLayoutControlItem;

    private LayoutControlItem signWithDifferentAccountLabelControlLayoutControlItem;

    private LayoutControlItem manageAccountLabelControlLayoutControlItem;

    private LabelControl userNameLabelControl;

    private LayoutControlItem userNameLabelControlLayoutControlItem;

    private LabelControl userEmailLabelControl;

    private LayoutControlItem userEmailLabelControlLayoutControlItem;

    private XtraScrollableControl xtraScrollableControl;

    private LabelControl licenseLabelControl;

    private LayoutControlItem layoutControlItem1;

    private LabelControl detailsLabelControl;

    private LayoutControlItem detailsLabelControlLayoutControlItem;

    private LabelControl signOutLabelControl;

    private LayoutControlItem signOutLabelControlLayoutControlItem;

    public event EventHandler OnSignWithDifferentAccount;

    public event EventHandler OnChangeLicense;

    public event EventHandler OnLicenseDetails;

    public event EventHandler OnSignOut;

    public UserPanelUserControl()
    {
        InitializeComponent();
    }

    public void SetParameters()
    {
        userNameLabelControl.Text = UserDataService.GetUsernameText();
        using (Graphics e = CreateGraphics())
        {
            userNameLabelControl.Font = new Font(userNameLabelControl.Font.FontFamily, GetFontSize(userNameLabelControl.Font.Size, e));
        }
        userEmailLabelControl.Text = ((string.IsNullOrEmpty(StaticData.Profile?.FullName) || StaticData.License.IsFileLicense) ? string.Empty : StaticData.Profile.Email);
        string baseLicenseText = UserDataService.GetBaseLicenseText();
        licenseLabelControl.Text = baseLicenseText;
        LayoutControlItem layoutControlItem = manageAccountLabelControlLayoutControlItem;
        List<AccountRoleResult> accountRoles = StaticData.License.AccountRoles;
        layoutControlItem.Visibility = ((accountRoles == null || !accountRoles.Any((AccountRoleResult x) => x.RoleValue == AccountRoleEnum.AccountRole.Administrator || x.RoleValue == AccountRoleEnum.AccountRole.Owner)) ? LayoutVisibility.Never : LayoutVisibility.Always);
    }

    private float GetFontSize(float fontSize, Graphics e)
    {
        if (e.MeasureString(userNameLabelControl.Text, new Font(userNameLabelControl.Font.FontFamily, fontSize)).Width > (float)userNameLabelControl.Width)
        {
            fontSize -= 1f;
            return GetFontSize(fontSize, e);
        }
        if (fontSize <= 10f)
        {
            return 10f;
        }
        return fontSize;
    }

    private void SignWithDifferentAccountLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        this.OnSignWithDifferentAccount?.Invoke(this, EventArgs.Empty);
    }

    private void CheckAvailableLicensesLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        this.OnChangeLicense?.Invoke(this, EventArgs.Empty);
    }

    private void ManageAccountLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        Links.OpenLink(Links.ManageAccounts);
    }

    private void UserPanelUserControl_Load(object sender, EventArgs e)
    {
        if (!base.DesignMode)
        {
            if (base.ParentForm != null)
            {
                BackColor = base.ParentForm.BackColor;
            }
            xtraScrollableControl.BackColor = BackColor;
        }
    }

    private void detailsLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        this.OnLicenseDetails?.Invoke(this, EventArgs.Empty);
    }

    private void signOutLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
    {
        this.OnSignOut?.Invoke(this, EventArgs.Empty);
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
            this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
            this.xtraScrollableControl = new DevExpress.XtraEditors.XtraScrollableControl();
            this.licenseLabelControl = new DevExpress.XtraEditors.LabelControl();
            this.userEmailLabelControl = new DevExpress.XtraEditors.LabelControl();
            this.userNameLabelControl = new DevExpress.XtraEditors.LabelControl();
            this.manageAccountLabelControl = new DevExpress.XtraEditors.LabelControl();
            this.signWithDifferentAccountLabelControl = new DevExpress.XtraEditors.LabelControl();
            this.checkAvailableLicensesLabelControl = new DevExpress.XtraEditors.LabelControl();
            this.detailsLabelControl = new DevExpress.XtraEditors.LabelControl();
            this.signOutLabelControl = new DevExpress.XtraEditors.LabelControl();
            this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
            this.emptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
            this.checkAvailableLicensesLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            this.signWithDifferentAccountLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            this.manageAccountLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            this.userNameLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            this.userEmailLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
            this.detailsLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            this.signOutLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
            ((System.ComponentModel.ISupportInitialize)(this.mainLayoutControl)).BeginInit();
            this.mainLayoutControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainLayoutControlGroup)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkAvailableLicensesLabelControlLayoutControlItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.signWithDifferentAccountLabelControlLayoutControlItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.manageAccountLabelControlLayoutControlItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userNameLabelControlLayoutControlItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.userEmailLabelControlLayoutControlItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailsLabelControlLayoutControlItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.signOutLabelControlLayoutControlItem)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayoutControl
            // 
            this.mainLayoutControl.AllowCustomization = false;
            this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
            this.mainLayoutControl.Controls.Add(this.xtraScrollableControl);
            this.mainLayoutControl.Controls.Add(this.userEmailLabelControl);
            this.mainLayoutControl.Controls.Add(this.userNameLabelControl);
            this.mainLayoutControl.Controls.Add(this.manageAccountLabelControl);
            this.mainLayoutControl.Controls.Add(this.signWithDifferentAccountLabelControl);
            this.mainLayoutControl.Controls.Add(this.checkAvailableLicensesLabelControl);
            this.mainLayoutControl.Controls.Add(this.detailsLabelControl);
            this.mainLayoutControl.Controls.Add(this.signOutLabelControl);
            this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
            this.mainLayoutControl.Name = "mainLayoutControl";
            this.mainLayoutControl.Root = this.mainLayoutControlGroup;
            this.mainLayoutControl.Size = new System.Drawing.Size(520, 317);
            this.mainLayoutControl.TabIndex = 0;
            this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
            // 
            // xtraScrollableControl
            // 
            this.xtraScrollableControl.Controls.Add(this.licenseLabelControl);
            this.xtraScrollableControl.Location = new System.Drawing.Point(12, 78);
            this.xtraScrollableControl.Name = "xtraScrollableControl";
            this.xtraScrollableControl.Size = new System.Drawing.Size(496, 83);
            this.xtraScrollableControl.TabIndex = 13;
            // 
            // licenseLabelControl
            // 
            this.licenseLabelControl.AllowHtmlString = true;
            this.licenseLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.licenseLabelControl.Appearance.Options.UseFont = true;
            this.licenseLabelControl.Appearance.Options.UseTextOptions = true;
            this.licenseLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.licenseLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.licenseLabelControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.licenseLabelControl.Location = new System.Drawing.Point(0, 0);
            this.licenseLabelControl.Name = "licenseLabelControl";
            this.licenseLabelControl.Size = new System.Drawing.Size(496, 80);
            this.licenseLabelControl.StyleController = this.mainLayoutControl;
            this.licenseLabelControl.TabIndex = 12;
            this.licenseLabelControl.Text = "<b>Line 1<b>\r\n<b>Line 2<b>\r\n<b>Line 3<b>\r\n<b>Line 4<b>\r\n<b>Line 5<b>";
            // 
            // userEmailLabelControl
            // 
            this.userEmailLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.userEmailLabelControl.Appearance.Options.UseFont = true;
            this.userEmailLabelControl.Appearance.Options.UseTextOptions = true;
            this.userEmailLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.userEmailLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.userEmailLabelControl.Location = new System.Drawing.Point(12, 45);
            this.userEmailLabelControl.Name = "userEmailLabelControl";
            this.userEmailLabelControl.Size = new System.Drawing.Size(496, 16);
            this.userEmailLabelControl.StyleController = this.mainLayoutControl;
            this.userEmailLabelControl.TabIndex = 12;
            this.userEmailLabelControl.Text = "email@dataedo.com";
            // 
            // userNameLabelControl
            // 
            this.userNameLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 18F);
            this.userNameLabelControl.Appearance.Options.UseFont = true;
            this.userNameLabelControl.AutoEllipsis = true;
            this.userNameLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.userNameLabelControl.Location = new System.Drawing.Point(12, 12);
            this.userNameLabelControl.Name = "userNameLabelControl";
            this.userNameLabelControl.Size = new System.Drawing.Size(496, 29);
            this.userNameLabelControl.StyleController = this.mainLayoutControl;
            this.userNameLabelControl.TabIndex = 9;
            this.userNameLabelControl.Text = "User";
            // 
            // manageAccountLabelControl
            // 
            this.manageAccountLabelControl.AllowHtmlString = true;
            this.manageAccountLabelControl.Location = new System.Drawing.Point(12, 284);
            this.manageAccountLabelControl.Name = "manageAccountLabelControl";
            this.manageAccountLabelControl.Size = new System.Drawing.Size(79, 13);
            this.manageAccountLabelControl.StyleController = this.mainLayoutControl;
            this.manageAccountLabelControl.TabIndex = 6;
            this.manageAccountLabelControl.Text = "<href>Manage account<href>";
            this.manageAccountLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(this.ManageAccountLabelControl_HyperlinkClick);
            // 
            // signWithDifferentAccountLabelControl
            // 
            this.signWithDifferentAccountLabelControl.AllowHtmlString = true;
            this.signWithDifferentAccountLabelControl.Location = new System.Drawing.Point(12, 209);
            this.signWithDifferentAccountLabelControl.Name = "signWithDifferentAccountLabelControl";
            this.signWithDifferentAccountLabelControl.Size = new System.Drawing.Size(129, 13);
            this.signWithDifferentAccountLabelControl.StyleController = this.mainLayoutControl;
            this.signWithDifferentAccountLabelControl.TabIndex = 5;
            this.signWithDifferentAccountLabelControl.Text = "<href>Sign with different account<href>";
            this.signWithDifferentAccountLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(this.SignWithDifferentAccountLabelControl_HyperlinkClick);
            // 
            // checkAvailableLicensesLabelControl
            // 
            this.checkAvailableLicensesLabelControl.AllowHtmlString = true;
            this.checkAvailableLicensesLabelControl.Location = new System.Drawing.Point(12, 234);
            this.checkAvailableLicensesLabelControl.Name = "checkAvailableLicensesLabelControl";
            this.checkAvailableLicensesLabelControl.Size = new System.Drawing.Size(114, 13);
            this.checkAvailableLicensesLabelControl.StyleController = this.mainLayoutControl;
            this.checkAvailableLicensesLabelControl.TabIndex = 4;
            this.checkAvailableLicensesLabelControl.Text = "<href>Check available licenses<href>";
            this.checkAvailableLicensesLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(this.CheckAvailableLicensesLabelControl_HyperlinkClick);
            // 
            // detailsLabelControl
            // 
            this.detailsLabelControl.AllowHtmlString = true;
            this.detailsLabelControl.Location = new System.Drawing.Point(12, 259);
            this.detailsLabelControl.Name = "detailsLabelControl";
            this.detailsLabelControl.Size = new System.Drawing.Size(69, 13);
            this.detailsLabelControl.StyleController = this.mainLayoutControl;
            this.detailsLabelControl.TabIndex = 4;
            this.detailsLabelControl.Text = "<href>License details<href>";
            this.detailsLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(this.detailsLabelControl_HyperlinkClick);
            // 
            // signOutLabelControl
            // 
            this.signOutLabelControl.AllowHtmlString = true;
            this.signOutLabelControl.Location = new System.Drawing.Point(12, 184);
            this.signOutLabelControl.Name = "signOutLabelControl";
            this.signOutLabelControl.Size = new System.Drawing.Size(39, 13);
            this.signOutLabelControl.StyleController = this.mainLayoutControl;
            this.signOutLabelControl.TabIndex = 5;
            this.signOutLabelControl.Text = "<href>Sign out<href>";
            this.signOutLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(this.signOutLabelControl_HyperlinkClick);
            // 
            // mainLayoutControlGroup
            // 
            this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
            this.mainLayoutControlGroup.GroupBordersVisible = false;
            this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.emptySpaceItem,
            this.checkAvailableLicensesLabelControlLayoutControlItem,
            this.signWithDifferentAccountLabelControlLayoutControlItem,
            this.manageAccountLabelControlLayoutControlItem,
            this.userNameLabelControlLayoutControlItem,
            this.userEmailLabelControlLayoutControlItem,
            this.layoutControlItem1,
            this.detailsLabelControlLayoutControlItem,
            this.signOutLabelControlLayoutControlItem});
            this.mainLayoutControlGroup.Name = "Root";
            this.mainLayoutControlGroup.Size = new System.Drawing.Size(520, 317);
            this.mainLayoutControlGroup.TextVisible = false;
            // 
            // emptySpaceItem
            // 
            this.emptySpaceItem.AllowHotTrack = false;
            this.emptySpaceItem.Location = new System.Drawing.Point(0, 153);
            this.emptySpaceItem.Name = "emptySpaceItem";
            this.emptySpaceItem.Size = new System.Drawing.Size(500, 19);
            this.emptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
            // 
            // checkAvailableLicensesLabelControlLayoutControlItem
            // 
            this.checkAvailableLicensesLabelControlLayoutControlItem.Control = this.checkAvailableLicensesLabelControl;
            this.checkAvailableLicensesLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 222);
            this.checkAvailableLicensesLabelControlLayoutControlItem.Name = "checkAvailableLicensesLabelControlLayoutControlItem";
            this.checkAvailableLicensesLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
            this.checkAvailableLicensesLabelControlLayoutControlItem.Size = new System.Drawing.Size(500, 25);
            this.checkAvailableLicensesLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
            this.checkAvailableLicensesLabelControlLayoutControlItem.TextVisible = false;
            this.checkAvailableLicensesLabelControlLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // signWithDifferentAccountLabelControlLayoutControlItem
            // 
            this.signWithDifferentAccountLabelControlLayoutControlItem.Control = this.signWithDifferentAccountLabelControl;
            this.signWithDifferentAccountLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 197);
            this.signWithDifferentAccountLabelControlLayoutControlItem.Name = "signWithDifferentAccountLabelControlLayoutControlItem";
            this.signWithDifferentAccountLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
            this.signWithDifferentAccountLabelControlLayoutControlItem.Size = new System.Drawing.Size(500, 25);
            this.signWithDifferentAccountLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
            this.signWithDifferentAccountLabelControlLayoutControlItem.TextVisible = false;
            this.signWithDifferentAccountLabelControlLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // manageAccountLabelControlLayoutControlItem
            // 
            this.manageAccountLabelControlLayoutControlItem.Control = this.manageAccountLabelControl;
            this.manageAccountLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 272);
            this.manageAccountLabelControlLayoutControlItem.Name = "manageAccountLabelControlLayoutControlItem";
            this.manageAccountLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
            this.manageAccountLabelControlLayoutControlItem.Size = new System.Drawing.Size(500, 25);
            this.manageAccountLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
            this.manageAccountLabelControlLayoutControlItem.TextVisible = false;
            this.manageAccountLabelControlLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // userNameLabelControlLayoutControlItem
            // 
            this.userNameLabelControlLayoutControlItem.Control = this.userNameLabelControl;
            this.userNameLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
            this.userNameLabelControlLayoutControlItem.Name = "userNameLabelControlLayoutControlItem";
            this.userNameLabelControlLayoutControlItem.Size = new System.Drawing.Size(500, 33);
            this.userNameLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
            this.userNameLabelControlLayoutControlItem.TextVisible = false;
            // 
            // userEmailLabelControlLayoutControlItem
            // 
            this.userEmailLabelControlLayoutControlItem.Control = this.userEmailLabelControl;
            this.userEmailLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 33);
            this.userEmailLabelControlLayoutControlItem.Name = "userEmailLabelControlLayoutControlItem";
            this.userEmailLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 15);
            this.userEmailLabelControlLayoutControlItem.Size = new System.Drawing.Size(500, 33);
            this.userEmailLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
            this.userEmailLabelControlLayoutControlItem.TextVisible = false;
            // 
            // layoutControlItem1
            // 
            this.layoutControlItem1.Control = this.xtraScrollableControl;
            this.layoutControlItem1.Location = new System.Drawing.Point(0, 66);
            this.layoutControlItem1.Name = "layoutControlItem1";
            this.layoutControlItem1.Size = new System.Drawing.Size(500, 87);
            this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
            this.layoutControlItem1.TextVisible = false;
            // 
            // detailsLabelControlLayoutControlItem
            // 
            this.detailsLabelControlLayoutControlItem.Control = this.detailsLabelControl;
            this.detailsLabelControlLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.detailsLabelControlLayoutControlItem.CustomizationFormText = "checkAvailableLicensesLabelControlLayoutControlItem";
            this.detailsLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 247);
            this.detailsLabelControlLayoutControlItem.Name = "detailsLabelControlLayoutControlItem";
            this.detailsLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
            this.detailsLabelControlLayoutControlItem.Size = new System.Drawing.Size(500, 25);
            this.detailsLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
            this.detailsLabelControlLayoutControlItem.TextVisible = false;
            // 
            // signOutLabelControlLayoutControlItem
            // 
            this.signOutLabelControlLayoutControlItem.Control = this.signOutLabelControl;
            this.signOutLabelControlLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
            this.signOutLabelControlLayoutControlItem.CustomizationFormText = "signOutLabelControlLayoutControlItem";
            this.signOutLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 172);
            this.signOutLabelControlLayoutControlItem.Name = "signOutLabelControlLayoutControlItem";
            this.signOutLabelControlLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.signOutLabelControlLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
            this.signOutLabelControlLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.signOutLabelControlLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
            this.signOutLabelControlLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.signOutLabelControlLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
            this.signOutLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
            this.signOutLabelControlLayoutControlItem.Size = new System.Drawing.Size(500, 25);
            this.signOutLabelControlLayoutControlItem.Text = "Sign out";
            this.signOutLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
            this.signOutLabelControlLayoutControlItem.TextVisible = false;
            this.signOutLabelControlLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
            // 
            // UserPanelUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainLayoutControl);
            this.Name = "UserPanelUserControl";
            this.Size = new System.Drawing.Size(520, 317);
            this.Load += new System.EventHandler(this.UserPanelUserControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.mainLayoutControl)).EndInit();
            this.mainLayoutControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainLayoutControlGroup)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.checkAvailableLicensesLabelControlLayoutControlItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.signWithDifferentAccountLabelControlLayoutControlItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.manageAccountLabelControlLayoutControlItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userNameLabelControlLayoutControlItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.userEmailLabelControlLayoutControlItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.detailsLabelControlLayoutControlItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.signOutLabelControlLayoutControlItem)).EndInit();
            this.ResumeLayout(false);

    }
}
