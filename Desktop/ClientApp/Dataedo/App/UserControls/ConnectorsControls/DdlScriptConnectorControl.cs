using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.OverridenControls;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Helpers;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using Dataedo.SqlParser.Domain;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class DdlScriptConnectorControl : ConnectorControlBase
{
	private string script;

	private string originalFileScriptContent;

	private bool scriptTooLong;

	private readonly DXErrorProvider errorProvider;

	private IContainer components;

	private LabelControl labelControl1;

	private LayoutControlItem layoutControlItem1;

	private RichEditControlWithPasteAwareness scriptEditorRichTextControl;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private LayoutControlGroup Root;

	private LayoutControlItem layoutControlItem2;

	private SimpleButton browseSimpleButton;

	private TextEdit filePathTextEdit;

	private LayoutControlItem filePathLayoutControlItem;

	private LayoutControlItem layoutControlItem4;

	private OpenFileDialog openFileDialog;

	private LookUpEdit scriptLanguageLookUpEdit;

	private LayoutControlItem scriptLanguageLayoutControlItem;

	private PictureEdit scriptLanguagePictureEdit;

	private ToolTipController toolTipController;

	private LayoutControlItem scriptLanguageTooltipLayoutControlItem;

	private LabelControl pasteScriptLabelControl;

	private LabelControl scriptLanguageLabelControl;

	private LayoutControlItem scriptLanguageLabellayoutControlItem;

	private LayoutControlItem layoutControlItem5;

	private TextEdit defaultSchemaEdit;

	private LayoutControlItem SchemaInput;

	private EmptySpaceItem emptySpaceItem4;

	private EmptySpaceItem emptySpaceItem7;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem3;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.DdlScript;

	public DdlScriptConnectorControl()
	{
		InitializeComponent();
		scriptEditorRichTextControl.RefreshSkin();
		errorProvider = new DXErrorProvider();
		defaultSchemaEdit.EditValue = string.Empty;
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		SetScriptLanguageDataSource();
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
		filePathTextEdit.Enabled = false;
		RichEditUserControlHelper.SetLineNumbering(scriptEditorRichTextControl);
		filePathTextEdit.ReadOnly = false;
		filePathTextEdit.BackColor = SkinsManager.CurrentSkin.TextFieldBackColor;
		if (string.IsNullOrWhiteSpace(base.DatabaseRow?.Param1))
		{
			script = null;
			originalFileScriptContent = null;
			scriptEditorRichTextControl.Text = string.Empty;
			filePathTextEdit.Text = string.Empty;
		}
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, "DDLScriptDatabase", GetDocumentationTitle(), null, null, null, windows_authentication: false, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null, null);
		if (!string.IsNullOrWhiteSpace(filePathTextEdit.Text))
		{
			base.DatabaseRow.Param1 = filePathTextEdit.Text;
		}
		base.DatabaseRow.Param2 = SqlLanguageEnum.TypeToString((SqlLanguage)scriptLanguageLookUpEdit.EditValue);
		string input = (scriptTooLong ? script : scriptEditorRichTextControl.Document.GetText(scriptEditorRichTextControl.Document.Range).Trim());
		input = Regex.Replace(input, "\\u00A0", " ");
		base.DatabaseRow.Tag = new List<ScriptData>
		{
			new ScriptData
			{
				DdlScript = input
			}
		};
		base.DatabaseRow.Param3 = defaultSchemaEdit.EditValue.ToString();
	}

	protected override void ReadPanelValues()
	{
		filePathTextEdit.Text = base.DatabaseRow.Param1;
		LoadFileContent();
		scriptLanguageLookUpEdit.EditValue = SqlLanguageEnum.StringToType(base.DatabaseRow.Param2);
	}

	private void SetScriptLanguageDataSource()
	{
		Dictionary<SqlLanguage, string> dictionary = new Dictionary<SqlLanguage, string>();
		dictionary.Add(SqlLanguage.TSql, "SQL Server");
		dictionary.Add(SqlLanguage.PostgreSql, "PostgreSQL");
		scriptLanguageLookUpEdit.Properties.ValueMember = "Key";
		scriptLanguageLookUpEdit.Properties.DisplayMember = "Value";
		scriptLanguageLookUpEdit.Properties.DataSource = dictionary;
		scriptLanguageLookUpEdit.Properties.DropDownRows = dictionary.Count;
	}

	private void SetDefaultSchemaValue()
	{
		object editValue = scriptLanguageLookUpEdit.EditValue;
		if (editValue is SqlLanguage)
		{
			switch ((SqlLanguage)editValue)
			{
			case SqlLanguage.TSql:
				defaultSchemaEdit.EditValue = "dbo";
				break;
			case SqlLanguage.PostgreSql:
				defaultSchemaEdit.EditValue = "public";
				break;
			}
		}
	}

	protected override string GetPanelDocumentationTitle()
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(filePathTextEdit.Text))
			{
				return Path.GetFileName(filePathTextEdit.Text);
			}
		}
		catch
		{
		}
		return "DDLScriptDatabase (beta)";
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateScript() & ValidateDefaultSchema() & ValidateSqlLanguage();
	}

	private bool ValidateScript()
	{
		if (string.IsNullOrEmpty(scriptEditorRichTextControl.Text))
		{
			return false;
		}
		return true;
	}

	private void ScriptEditor_DocumentLoaded(object sender, EventArgs e)
	{
		RichEditUserControlHelper.SetLineNumbering(scriptEditorRichTextControl);
	}

	private void ScriptEditor_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Space)
		{
			FormatScriptEditorText();
		}
	}

	private void BrowseSimpleButton_Click(object sender, EventArgs e)
	{
		try
		{
			openFileDialog.FileName = filePathTextEdit.Text;
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				string text = filePathTextEdit.Text;
				filePathTextEdit.Text = openFileDialog.FileName;
				if (!LoadFileContent())
				{
					filePathTextEdit.Text = text;
				}
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, null, ex.Message, base.ParentForm);
		}
	}

	private void ScriptEditor_TextChanged(object sender, EventArgs e)
	{
		if (script == scriptEditorRichTextControl.Text || string.IsNullOrEmpty(scriptEditorRichTextControl.Text) || scriptTooLong)
		{
			return;
		}
		script = string.Copy(scriptEditorRichTextControl.Text);
		if (originalFileScriptContent != null)
		{
			string obj = Regex.Replace(originalFileScriptContent, "\\s+", " ");
			string text = Regex.Replace(script, "\\s+", " ");
			if (obj.Trim() != text.Trim())
			{
				filePathTextEdit.ReadOnly = true;
				filePathTextEdit.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			}
			else
			{
				filePathTextEdit.ReadOnly = false;
				filePathTextEdit.BackColor = SkinsManager.CurrentSkin.TextFieldBackColor;
			}
		}
	}

	private void FilePathTextEdit_EnabledChanged(object sender, EventArgs e)
	{
		filePathTextEdit.ForeColor = SkinsManager.CurrentSkin.ControlForeColor;
	}

	private void FormatScriptEditorText()
	{
		scriptEditorRichTextControl.BeginUpdate();
		scriptEditorRichTextControl.Document.BeginUpdate();
		DocumentPosition documentPosition = scriptEditorRichTextControl.Document?.CaretPosition;
		if (documentPosition != null)
		{
			scriptEditorRichTextControl.Document.CaretPosition = documentPosition;
		}
		scriptEditorRichTextControl.ScrollToCaret();
		scriptEditorRichTextControl.Document.EndUpdate();
		scriptEditorRichTextControl.EndUpdate();
	}

	private void scriptLanguageLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		SetDefaultSchemaValue();
	}

	private bool ValidateSqlLanguage()
	{
		return ValidateFields.ValidateEdit(scriptLanguageLookUpEdit, errorProvider, "sql language");
	}

	private bool ValidateDefaultSchema()
	{
		return ValidateFields.ValidateEdit(defaultSchemaEdit, errorProvider, string.Empty);
	}

	private bool LoadFileContent()
	{
		bool result = true;
		try
		{
			if (string.IsNullOrWhiteSpace(filePathTextEdit.Text))
			{
				filePathTextEdit.ReadOnly = false;
				filePathTextEdit.BackColor = SkinsManager.CurrentSkin.TextFieldBackColor;
				scriptEditorRichTextControl.Text = string.Empty;
				originalFileScriptContent = string.Empty;
				scriptEditorRichTextControl.Focus();
				return false;
			}
			if (!File.Exists(filePathTextEdit.Text))
			{
				GeneralMessageBoxesHandling.Show("Could not find \"" + filePathTextEdit.Text + "\" file." + Environment.NewLine + Environment.NewLine + "Please select a file to import.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
				return false;
			}
			if (!string.IsNullOrWhiteSpace(scriptEditorRichTextControl.Text) && GeneralMessageBoxesHandling.Show("Do you want to modify Script to import? All changes will be lost.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, null, 1, base.ParentForm).DialogResult != DialogResult.Yes)
			{
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			using (FileStream stream = File.Open(filePathTextEdit.Text, FileMode.Open))
			{
				using BufferedStream stream2 = new BufferedStream(stream);
				using StreamReader streamReader = new StreamReader(stream2);
				string value;
				while ((value = streamReader.ReadLine()) != null)
				{
					stringBuilder.AppendLine(value);
					num++;
				}
			}
			string text = stringBuilder.ToString();
			if (num <= 30000)
			{
				scriptEditorRichTextControl.Text = text;
				filePathTextEdit.ReadOnly = false;
				filePathTextEdit.BackColor = SkinsManager.CurrentSkin.TextFieldBackColor;
				originalFileScriptContent = text;
				FormatScriptEditorText();
				scriptEditorRichTextControl.Focus();
				return result;
			}
			script = text;
			scriptTooLong = true;
			scriptEditorRichTextControl.Text = "File too big to inspect...";
			filePathTextEdit.ReadOnly = false;
			filePathTextEdit.BackColor = SkinsManager.CurrentSkin.TextFieldBackColor;
			originalFileScriptContent = text;
			return result;
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, null, ex.Message, base.ParentForm);
			return false;
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
		this.components = new System.ComponentModel.Container();
		this.scriptEditorRichTextControl = new Dataedo.App.OverridenControls.RichEditControlWithPasteAwareness();
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.pasteScriptLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.scriptLanguageLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.scriptLanguageLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.browseSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.filePathTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.scriptLanguagePictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.defaultSchemaEdit = new DevExpress.XtraEditors.TextEdit();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.filePathLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.scriptLanguageLabellayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.scriptLanguageTooltipLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.scriptLanguageLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.SchemaInput = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.scriptLanguageLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.filePathTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.scriptLanguagePictureEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.defaultSchemaEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.filePathLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.scriptLanguageLabellayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.scriptLanguageTooltipLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.scriptLanguageLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SchemaInput).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		base.SuspendLayout();
		this.scriptEditorRichTextControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.scriptEditorRichTextControl.Appearance.Text.Font = new System.Drawing.Font("Courier New", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.scriptEditorRichTextControl.Appearance.Text.Options.UseFont = true;
		this.scriptEditorRichTextControl.IsHighlighted = false;
		this.scriptEditorRichTextControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.scriptEditorRichTextControl.Location = new System.Drawing.Point(0, 96);
		this.scriptEditorRichTextControl.Name = "scriptEditorRichTextControl";
		this.scriptEditorRichTextControl.OccurrencesCount = 0;
		this.scriptEditorRichTextControl.Options.Behavior.Drag = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
		this.scriptEditorRichTextControl.Options.Behavior.UseThemeFonts = false;
		this.scriptEditorRichTextControl.Options.DocumentCapabilities.Comments = DevExpress.XtraRichEdit.DocumentCapability.Disabled;
		this.scriptEditorRichTextControl.OriginalHtmlText = null;
		this.scriptEditorRichTextControl.Size = new System.Drawing.Size(522, 318);
		this.scriptEditorRichTextControl.TabIndex = 2;
		this.scriptEditorRichTextControl.Views.DraftView.AllowDisplayLineNumbers = true;
		this.scriptEditorRichTextControl.Views.SimpleView.AllowDisplayLineNumbers = true;
		this.scriptEditorRichTextControl.DocumentLoaded += new System.EventHandler(ScriptEditor_DocumentLoaded);
		this.scriptEditorRichTextControl.TextChanged += new System.EventHandler(ScriptEditor_TextChanged);
		this.scriptEditorRichTextControl.KeyDown += new System.Windows.Forms.KeyEventHandler(ScriptEditor_KeyDown);
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.labelControl1);
		this.nonCustomizableLayoutControl1.Controls.Add(this.pasteScriptLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.scriptLanguageLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.scriptLanguageLookUpEdit);
		this.nonCustomizableLayoutControl1.Controls.Add(this.browseSimpleButton);
		this.nonCustomizableLayoutControl1.Controls.Add(this.filePathTextEdit);
		this.nonCustomizableLayoutControl1.Controls.Add(this.scriptEditorRichTextControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.scriptLanguagePictureEdit);
		this.nonCustomizableLayoutControl1.Controls.Add(this.defaultSchemaEdit);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Margin = new System.Windows.Forms.Padding(0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(633, 149, 650, 400);
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(522, 414);
		this.nonCustomizableLayoutControl1.TabIndex = 3;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.Appearance.Options.UseTextOptions = true;
		this.labelControl1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.labelControl1.Location = new System.Drawing.Point(0, 50);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(101, 20);
		this.labelControl1.StyleController = this.nonCustomizableLayoutControl1;
		this.labelControl1.TabIndex = 16;
		this.labelControl1.Text = "Default Schema:";
		this.pasteScriptLabelControl.Appearance.Options.UseFont = true;
		this.pasteScriptLabelControl.Appearance.Options.UseTextOptions = true;
		this.pasteScriptLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.pasteScriptLabelControl.Location = new System.Drawing.Point(0, 74);
		this.pasteScriptLabelControl.Name = "pasteScriptLabelControl";
		this.pasteScriptLabelControl.Size = new System.Drawing.Size(520, 20);
		this.pasteScriptLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.pasteScriptLabelControl.TabIndex = 14;
		this.pasteScriptLabelControl.Text = "Paste script to import:";
		this.scriptLanguageLabelControl.Appearance.Options.UseFont = true;
		this.scriptLanguageLabelControl.Appearance.Options.UseTextOptions = true;
		this.scriptLanguageLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.scriptLanguageLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.scriptLanguageLabelControl.Location = new System.Drawing.Point(0, 26);
		this.scriptLanguageLabelControl.Name = "scriptLanguageLabelControl";
		this.scriptLanguageLabelControl.Size = new System.Drawing.Size(101, 20);
		this.scriptLanguageLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.scriptLanguageLabelControl.TabIndex = 13;
		this.scriptLanguageLabelControl.Text = "Script language:";
		this.scriptLanguageLookUpEdit.Location = new System.Drawing.Point(131, 25);
		this.scriptLanguageLookUpEdit.Name = "scriptLanguageLookUpEdit";
		this.scriptLanguageLookUpEdit.Properties.AutoHeight = false;
		this.scriptLanguageLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.scriptLanguageLookUpEdit.Properties.DisplayMember = "Value";
		this.scriptLanguageLookUpEdit.Properties.NullText = "";
		this.scriptLanguageLookUpEdit.Properties.ShowFooter = false;
		this.scriptLanguageLookUpEdit.Properties.ShowHeader = false;
		this.scriptLanguageLookUpEdit.Properties.ShowLines = false;
		this.scriptLanguageLookUpEdit.Properties.ValueMember = "Key";
		this.scriptLanguageLookUpEdit.Size = new System.Drawing.Size(287, 20);
		this.scriptLanguageLookUpEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.scriptLanguageLookUpEdit.TabIndex = 6;
		this.scriptLanguageLookUpEdit.EditValueChanged += new System.EventHandler(scriptLanguageLookUpEdit_EditValueChanged);
		this.browseSimpleButton.Location = new System.Drawing.Point(434, 0);
		this.browseSimpleButton.Name = "browseSimpleButton";
		this.browseSimpleButton.Size = new System.Drawing.Size(87, 24);
		this.browseSimpleButton.StyleController = this.nonCustomizableLayoutControl1;
		this.browseSimpleButton.TabIndex = 5;
		this.browseSimpleButton.Text = "Browse";
		this.browseSimpleButton.Click += new System.EventHandler(BrowseSimpleButton_Click);
		this.filePathTextEdit.Location = new System.Drawing.Point(0, 1);
		this.filePathTextEdit.Margin = new System.Windows.Forms.Padding(0);
		this.filePathTextEdit.Name = "filePathTextEdit";
		this.filePathTextEdit.Properties.AutoHeight = false;
		this.filePathTextEdit.Size = new System.Drawing.Size(418, 20);
		this.filePathTextEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.filePathTextEdit.TabIndex = 4;
		this.filePathTextEdit.EnabledChanged += new System.EventHandler(FilePathTextEdit_EnabledChanged);
		this.scriptLanguagePictureEdit.EditValue = Dataedo.App.Properties.Resources.question_16;
		this.scriptLanguagePictureEdit.Location = new System.Drawing.Point(103, 28);
		this.scriptLanguagePictureEdit.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
		this.scriptLanguagePictureEdit.Name = "scriptLanguagePictureEdit";
		this.scriptLanguagePictureEdit.Properties.AllowFocused = false;
		this.scriptLanguagePictureEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.scriptLanguagePictureEdit.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.scriptLanguagePictureEdit.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.scriptLanguagePictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.scriptLanguagePictureEdit.Properties.Appearance.Options.UseBackColor = true;
		this.scriptLanguagePictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.scriptLanguagePictureEdit.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Squeeze;
		this.scriptLanguagePictureEdit.Size = new System.Drawing.Size(26, 16);
		this.scriptLanguagePictureEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.scriptLanguagePictureEdit.TabIndex = 12;
		this.scriptLanguagePictureEdit.ToolTip = "If possible, please select a proper language of imported script.\r\nBe advised that in case of big scripts with unknown language\r\nparsing might take a while and consume large amount of RAM.";
		this.scriptLanguagePictureEdit.ToolTipController = this.toolTipController;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.defaultSchemaEdit.EditValue = "";
		this.defaultSchemaEdit.Location = new System.Drawing.Point(131, 49);
		this.defaultSchemaEdit.Margin = new System.Windows.Forms.Padding(0);
		this.defaultSchemaEdit.Name = "defaultSchemaEdit";
		this.defaultSchemaEdit.Properties.AutoHeight = false;
		this.defaultSchemaEdit.Size = new System.Drawing.Size(287, 20);
		this.defaultSchemaEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.defaultSchemaEdit.TabIndex = 15;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[13]
		{
			this.layoutControlItem2, this.filePathLayoutControlItem, this.emptySpaceItem4, this.emptySpaceItem7, this.scriptLanguageLabellayoutControlItem, this.scriptLanguageTooltipLayoutControlItem, this.scriptLanguageLayoutControlItem, this.layoutControlItem1, this.layoutControlItem5, this.layoutControlItem4,
			this.emptySpaceItem1, this.SchemaInput, this.emptySpaceItem3
		});
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(522, 414);
		this.Root.TextVisible = false;
		this.layoutControlItem2.Control = this.scriptEditorRichTextControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 96);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(104, 24);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem2.Size = new System.Drawing.Size(522, 318);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.filePathLayoutControlItem.Control = this.filePathTextEdit;
		this.filePathLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.filePathLayoutControlItem.MinSize = new System.Drawing.Size(50, 24);
		this.filePathLayoutControlItem.Name = "filePathLayoutControlItem";
		this.filePathLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 1, 3);
		this.filePathLayoutControlItem.Size = new System.Drawing.Size(418, 24);
		this.filePathLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.filePathLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.filePathLayoutControlItem.TextVisible = false;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(418, 24);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(0, 24);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(104, 24);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem7.AllowHotTrack = false;
		this.emptySpaceItem7.Location = new System.Drawing.Point(418, 48);
		this.emptySpaceItem7.Name = "emptySpaceItem7";
		this.emptySpaceItem7.Size = new System.Drawing.Size(104, 24);
		this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
		this.scriptLanguageLabellayoutControlItem.Control = this.scriptLanguageLabelControl;
		this.scriptLanguageLabellayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.scriptLanguageLabellayoutControlItem.MaxSize = new System.Drawing.Size(103, 0);
		this.scriptLanguageLabellayoutControlItem.MinSize = new System.Drawing.Size(103, 17);
		this.scriptLanguageLabellayoutControlItem.Name = "scriptLanguageLabellayoutControlItem";
		this.scriptLanguageLabellayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
		this.scriptLanguageLabellayoutControlItem.Size = new System.Drawing.Size(103, 24);
		this.scriptLanguageLabellayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.scriptLanguageLabellayoutControlItem.Text = "Script language";
		this.scriptLanguageLabellayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.scriptLanguageLabellayoutControlItem.TextVisible = false;
		this.scriptLanguageTooltipLayoutControlItem.Control = this.scriptLanguagePictureEdit;
		this.scriptLanguageTooltipLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.scriptLanguageTooltipLayoutControlItem.CustomizationFormText = "layoutControlItem9";
		this.scriptLanguageTooltipLayoutControlItem.Location = new System.Drawing.Point(103, 24);
		this.scriptLanguageTooltipLayoutControlItem.MaxSize = new System.Drawing.Size(28, 24);
		this.scriptLanguageTooltipLayoutControlItem.MinSize = new System.Drawing.Size(28, 24);
		this.scriptLanguageTooltipLayoutControlItem.Name = "scriptLanguageTooltipLayoutControlItem";
		this.scriptLanguageTooltipLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.scriptLanguageTooltipLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.scriptLanguageTooltipLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.scriptLanguageTooltipLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.scriptLanguageTooltipLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.scriptLanguageTooltipLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.scriptLanguageTooltipLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 4, 4);
		this.scriptLanguageTooltipLayoutControlItem.Size = new System.Drawing.Size(28, 24);
		this.scriptLanguageTooltipLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.scriptLanguageTooltipLayoutControlItem.Text = "layoutControlItem9";
		this.scriptLanguageTooltipLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.scriptLanguageTooltipLayoutControlItem.TextVisible = false;
		this.scriptLanguageLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.scriptLanguageLayoutControlItem.Control = this.scriptLanguageLookUpEdit;
		this.scriptLanguageLayoutControlItem.Location = new System.Drawing.Point(131, 24);
		this.scriptLanguageLayoutControlItem.MinSize = new System.Drawing.Size(50, 24);
		this.scriptLanguageLayoutControlItem.Name = "scriptLanguageLayoutControlItem";
		this.scriptLanguageLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 1, 3);
		this.scriptLanguageLayoutControlItem.Size = new System.Drawing.Size(287, 24);
		this.scriptLanguageLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.scriptLanguageLayoutControlItem.Text = "Script language";
		this.scriptLanguageLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
		this.scriptLanguageLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.scriptLanguageLayoutControlItem.TextToControlDistance = 0;
		this.scriptLanguageLayoutControlItem.TextVisible = false;
		this.layoutControlItem1.AppearanceItemCaption.Options.UseTextOptions = true;
		this.layoutControlItem1.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.layoutControlItem1.Control = this.labelControl1;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 48);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(13, 17);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
		this.layoutControlItem1.Size = new System.Drawing.Size(103, 24);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem5.Control = this.pasteScriptLabelControl;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 72);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 24);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(103, 24);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
		this.layoutControlItem5.Size = new System.Drawing.Size(522, 24);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layoutControlItem4.Control = this.browseSimpleButton;
		this.layoutControlItem4.Location = new System.Drawing.Point(434, 0);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(87, 24);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(87, 24);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem4.Size = new System.Drawing.Size(88, 24);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(418, 0);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(16, 24);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(16, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(16, 24);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.SchemaInput.Control = this.defaultSchemaEdit;
		this.SchemaInput.Location = new System.Drawing.Point(131, 48);
		this.SchemaInput.Name = "SchemaInput";
		this.SchemaInput.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 1, 3);
		this.SchemaInput.Size = new System.Drawing.Size(287, 24);
		this.SchemaInput.Text = "Default schema";
		this.SchemaInput.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.SchemaInput.TextSize = new System.Drawing.Size(0, 0);
		this.SchemaInput.TextToControlDistance = 0;
		this.SchemaInput.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(103, 48);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(28, 24);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(28, 24);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(28, 24);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.openFileDialog.FileName = "openFileDialog";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.Margin = new System.Windows.Forms.Padding(0);
		base.Name = "DdlScriptConnectorControl";
		base.Size = new System.Drawing.Size(522, 414);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.scriptLanguageLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.filePathTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.scriptLanguagePictureEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.defaultSchemaEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.filePathLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.scriptLanguageLabellayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.scriptLanguageTooltipLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.scriptLanguageLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SchemaInput).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		base.ResumeLayout(false);
	}
}
