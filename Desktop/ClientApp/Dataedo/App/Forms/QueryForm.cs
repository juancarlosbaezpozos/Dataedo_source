using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.CustomControls;
using DevExpress.XtraEditors;

namespace Dataedo.App.Forms;

public class QueryForm : BaseXtraForm
{
	private IContainer components;

	private MemoEdit memoEdit;

	public QueryForm(string title, string query)
	{
		InitializeComponent();
		Text = title;
		memoEdit.Text = query;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.QueryForm));
		this.memoEdit = new DevExpress.XtraEditors.MemoEdit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit.Properties).BeginInit();
		base.SuspendLayout();
		this.memoEdit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.memoEdit.Location = new System.Drawing.Point(0, 0);
		this.memoEdit.Name = "memoEdit";
		this.memoEdit.Properties.ReadOnly = true;
		this.memoEdit.Properties.UseReadOnlyAppearance = false;
		this.memoEdit.Size = new System.Drawing.Size(290, 268);
		this.memoEdit.TabIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(290, 268);
		base.Controls.Add(this.memoEdit);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("QueryForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "QueryForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "QueryForm";
		base.TopMost = true;
		((System.ComponentModel.ISupportInitialize)this.memoEdit.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
