using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.API.Models;
using Dataedo.App.Tools;
using Dataedo.CustomControls;
using Dataedo.Shared.Licenses.Models;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace Dataedo.App.UserControls;

public class LicenseDetailsUserControl : UserControl
{
    private IContainer components;

    private NonCustomizableLayoutControl detailsNonCustomizableLayoutControl;

    private LayoutControlGroup Root;

    private LabelControl detailsLabelControl;

    private LayoutControlItem layoutControlItem1;

    private EmptySpaceItem emptySpaceItem1;

    public LicenseDetailsUserControl()
    {
        InitializeComponent();
    }

    public void SetParameters(AppLicense appLicense)
    {
        Dictionary<string, IList<ModuleDataResult>> groupedModules = GetGroupedModules(appLicense);
        detailsLabelControl.Text = UserDataService.GetBaseLicenseText();
        detailsLabelControl.Text += Environment.NewLine;
        foreach (KeyValuePair<string, IList<ModuleDataResult>> item in groupedModules)
        {
            if (!string.IsNullOrEmpty(detailsLabelControl.Text))
            {
                detailsLabelControl.Text += Environment.NewLine;
            }
            LabelControl labelControl = detailsLabelControl;
            labelControl.Text = labelControl.Text + "<b>" + item.Key + "</b>";
            foreach (ModuleDataResult item2 in item.Value.OrderBy((ModuleDataResult x) => x.Sort))
            {
                detailsLabelControl.Text += Environment.NewLine;
                LabelControl labelControl2 = detailsLabelControl;
                labelControl2.Text = labelControl2.Text + "  " + item2.Name;
                if (item2.Count.HasValue && item2.Count > 0)
                {
                    detailsLabelControl.Text += $" - {item2.Count}";
                }
            }
        }
    }

    private static Dictionary<string, IList<ModuleDataResult>> GetGroupedModules(AppLicense appLicense)
    {
        Dictionary<string, IList<ModuleDataResult>> dictionary = new Dictionary<string, IList<ModuleDataResult>>();
        Dictionary<string, int> sortValues = new Dictionary<string, int>();
        foreach (ModuleDataResult module in appLicense.Modules)
        {
            sortValues[module.GroupName] = module.GroupSort;
            if (!dictionary.ContainsKey(module.GroupName))
            {
                dictionary[module.GroupName] = new List<ModuleDataResult>();
            }
            dictionary[module.GroupName].Add(module);
        }
        return dictionary.OrderBy((KeyValuePair<string, IList<ModuleDataResult>> x) => sortValues[x.Key]).ToDictionary((KeyValuePair<string, IList<ModuleDataResult>> x) => x.Key, (KeyValuePair<string, IList<ModuleDataResult>> x) => x.Value);
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
        this.detailsNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
        this.detailsLabelControl = new DevExpress.XtraEditors.LabelControl();
        this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
        this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
        this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
        ((System.ComponentModel.ISupportInitialize)this.detailsNonCustomizableLayoutControl).BeginInit();
        this.detailsNonCustomizableLayoutControl.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
        base.SuspendLayout();
        this.detailsNonCustomizableLayoutControl.AllowCustomization = false;
        this.detailsNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
        this.detailsNonCustomizableLayoutControl.Controls.Add(this.detailsLabelControl);
        this.detailsNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
        this.detailsNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
        this.detailsNonCustomizableLayoutControl.Name = "detailsNonCustomizableLayoutControl";
        this.detailsNonCustomizableLayoutControl.Root = this.Root;
        this.detailsNonCustomizableLayoutControl.Size = new System.Drawing.Size(261, 228);
        this.detailsNonCustomizableLayoutControl.TabIndex = 0;
        this.detailsNonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
        this.detailsLabelControl.AllowHtmlString = true;
        this.detailsLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
        this.detailsLabelControl.Appearance.Options.UseFont = true;
        this.detailsLabelControl.Appearance.Options.UseTextOptions = true;
        this.detailsLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
        this.detailsLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
        this.detailsLabelControl.Dock = System.Windows.Forms.DockStyle.Top;
        this.detailsLabelControl.Location = new System.Drawing.Point(12, 12);
        this.detailsLabelControl.Name = "detailsLabelControl";
        this.detailsLabelControl.Size = new System.Drawing.Size(57, 13);
        this.detailsLabelControl.StyleController = this.detailsNonCustomizableLayoutControl;
        this.detailsLabelControl.TabIndex = 4;
        this.detailsLabelControl.Text = "labelControl";
        this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
        this.Root.GroupBordersVisible = false;
        this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layoutControlItem1, this.emptySpaceItem1 });
        this.Root.Name = "Root";
        this.Root.Size = new System.Drawing.Size(261, 228);
        this.Root.TextVisible = false;
        this.layoutControlItem1.Control = this.detailsLabelControl;
        this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
        this.layoutControlItem1.Name = "layoutControlItem1";
        this.layoutControlItem1.Size = new System.Drawing.Size(241, 17);
        this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
        this.layoutControlItem1.TextVisible = false;
        this.emptySpaceItem1.AllowHotTrack = false;
        this.emptySpaceItem1.Location = new System.Drawing.Point(0, 17);
        this.emptySpaceItem1.Name = "emptySpaceItem1";
        this.emptySpaceItem1.Size = new System.Drawing.Size(241, 191);
        this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
        base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.Controls.Add(this.detailsNonCustomizableLayoutControl);
        base.Name = "LicenseDetailsUserControl";
        base.Size = new System.Drawing.Size(261, 228);
        ((System.ComponentModel.ISupportInitialize)this.detailsNonCustomizableLayoutControl).EndInit();
        this.detailsNonCustomizableLayoutControl.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
        ((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
        base.ResumeLayout(false);
    }
}
