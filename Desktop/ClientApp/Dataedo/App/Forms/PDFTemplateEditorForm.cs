using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Documentation;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.TemplateEditor;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraTreeList.Menu;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;

namespace Dataedo.App.Forms;

public class PDFTemplateEditorForm : BaseXtraForm
{
	private ElementsCollection xml;

	private string xmlFilePath;

	private bool isNewTemplate;

	private bool isNewTemplateSaved;

	private IContainer components;

	private TreeList treeList;

	private TreeListColumn colName;

	private TreeListColumn colValue;

	private BindingSource elementBindingSource;

	private RepositoryItemFontEdit repositoryItemFontEdit;

	private RepositoryItemColorPickEdit repositoryItemColorPickEdit;

	private RepositoryItemCheckEdit repositoryItemCheckEdit;

	private SimpleButton saveSimpleButton;

	private SimpleButton cancelSimpleButton;

	private NonCustomizableLayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem1;

	private RepositoryItemButtonEdit repositoryItemButtonEdit;

	private OpenFileDialog openImageFileDialog;

	private EmptySpaceItem emptySpaceItem2;

	private RepositoryItemSpinEdit fontSizeRepositoryItemSpinEdit;

	private TextEdit fileNameTextEdit;

	private LayoutControlItem layoutControlItem4;

	private TextEdit pathTextEdit;

	private LayoutControlItem layoutControlItem5;

	private ToolTipController treeListToolTipController;

	private RepositoryItemTextEdit linkRepositoryItemTextEdit;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	public string TemplateName { get; set; }

	public bool IsXmlDataLoaded
	{
		get
		{
			if (this == null)
			{
				return false;
			}
			return xml?.IsElementsDataLoaded == true;
		}
	}

	public PDFTemplateEditorForm(string path, bool newTemplateCreated = false, string baseTemplateName = null)
	{
		InitializeComponent();
		TemplateName = Path.GetFileName(path);
		xmlFilePath = Path.Combine(path, DocTemplateFile.MetaFileName);
		xml = new ElementsCollection(xmlFilePath, DocFormatEnum.DocFormat.PDF, this);
		if (xml.IsElementsDataLoaded)
		{
			if (newTemplateCreated)
			{
				xml.Description = DocTemplateFile.GetCustomizedTemplateDescription(baseTemplateName);
			}
			xml.InitializeModel(withoutCustomizationModel: false, this);
			fileNameTextEdit.Text = TemplateName;
			pathTextEdit.Text = Path.GetDirectoryName(path);
			isNewTemplate = newTemplateCreated;
			if (newTemplateCreated)
			{
				Text = "Create custom template from " + baseTemplateName;
			}
			else
			{
				Text = "Customize template " + TemplateName;
			}
			SetDataSource(TemplateName);
			if (isNewTemplate)
			{
				SetThumbnail();
			}
		}
	}

	private void SetThumbnail()
	{
		string destination = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(xmlFilePath)), TemplateName);
		OverwriteThumbnail(destination);
	}

	public void SetDataSource(string templateName)
	{
		treeList.DataSource = xml.GetCollection(this);
		if (isNewTemplate)
		{
			xml.Name = templateName;
		}
		xml.SaveXml(DocFormatEnum.DocFormat.PDF, this);
		treeList.ExpandToLevel(0);
	}

	private void treeList_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
	{
		if (e.Column == colValue)
		{
			switch ((treeList.GetDataRecordByNode(e.Node) as Element).ElementType)
			{
			case ElementTypeEnum.ElementType.Color:
				e.RepositoryItem = repositoryItemColorPickEdit;
				break;
			case ElementTypeEnum.ElementType.Font:
				e.RepositoryItem = repositoryItemFontEdit;
				break;
			case ElementTypeEnum.ElementType.Bool:
				e.RepositoryItem = repositoryItemCheckEdit;
				break;
			case ElementTypeEnum.ElementType.Decimal:
				e.RepositoryItem = fontSizeRepositoryItemSpinEdit;
				break;
			case ElementTypeEnum.ElementType.Path:
				e.RepositoryItem = repositoryItemButtonEdit;
				break;
			case ElementTypeEnum.ElementType.Link:
				e.RepositoryItem = linkRepositoryItemTextEdit;
				break;
			case ElementTypeEnum.ElementType.ReadOnly:
				break;
			}
		}
	}

	private void saveSimpleButton_Click(object sender, EventArgs e)
	{
		string newName = fileNameTextEdit.Text.Trim();
		if (SaveTemplate(newName))
		{
			isNewTemplateSaved = true;
			Close();
		}
	}

	private bool SaveTemplate(string newName)
	{
		string directoryName = Path.GetDirectoryName(xmlFilePath);
		string directoryName2 = Path.GetDirectoryName(directoryName);
		IEnumerable<char> invalidCharactersFromPath = Paths.GetInvalidCharactersFromPath(newName);
		if (invalidCharactersFromPath.Count() > 0)
		{
			GeneralMessageBoxesHandling.Show("Template name cannot contain " + string.Join(" ", invalidCharactersFromPath) + " characters.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			return false;
		}
		if (!Paths.IsValidName(newName, directoryName2, showMessage: false, this))
		{
			GeneralMessageBoxesHandling.Show("Template name is not valid", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			return false;
		}
		string text = Path.Combine(directoryName2, newName);
		bool flag = directoryName != text;
		string path = Path.Combine(DocTemplateFile.GetTemplatesPath(DocFormatEnum.DocFormat.PDF, this), newName + ".zip");
		if (flag && (Directory.Exists(text) || File.Exists(path)))
		{
			GeneralMessageBoxesHandling.Show("Template \"" + newName + "\" already exists. Please provide other name.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			return false;
		}
		xml.Name = newName;
		CopyImages();
		xml.SaveXml(DocFormatEnum.DocFormat.PDF, this);
		if (flag)
		{
			RenameTemplate(directoryName, text);
		}
		TemplateName = newName;
		return true;
	}

	private void CopyImages()
	{
		IEnumerator<Element> enumerator = xml.GetImages().Where(delegate(Element x)
		{
			string text2 = x.Value?.ToString();
			if (string.IsNullOrWhiteSpace(text2) || !File.Exists(text2))
			{
				return false;
			}
			string directoryName = Path.GetDirectoryName(text2);
			string directoryName2 = Path.GetDirectoryName(xmlFilePath);
			return directoryName != directoryName2;
		}).GetEnumerator();
		while (enumerator.MoveNext())
		{
			Element current = enumerator.Current;
			string sourceFullPath = current.Value?.ToString();
			string text = (string)(current.Value = CopyImageToLocal(sourceFullPath));
		}
	}

	private string CopyImageToLocal(string sourceFullPath)
	{
		string directoryName = Path.GetDirectoryName(xmlFilePath);
		string nextName = Paths.GetNextName(directoryName, "Image", Path.GetExtension(sourceFullPath), forFile: true, addExtenstion: true);
		string destFileName = Path.Combine(directoryName, nextName);
		File.Copy(sourceFullPath, destFileName, overwrite: false);
		return nextName;
	}

	private void RenameTemplate(string source, string destination)
	{
		try
		{
			if (source != destination && Directory.Exists(destination))
			{
				Directory.Delete(destination, recursive: true);
			}
			Directory.Move(source, destination);
		}
		catch (Exception)
		{
		}
	}

	private void OverwriteThumbnail(string destination)
	{
		File.Delete(Path.Combine(destination, DocTemplateFile.ThumbnailFileName + ".jpg"));
		File.Delete(Path.Combine(destination, DocTemplateFile.ThumbnailFileName + ".png"));
		string filename = Path.Combine(destination, DocTemplateFile.ThumbnailFileName + ".jpg");
		Resources.thumbnail_custom_pdf.Save(filename);
	}

	private void cancelSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void RemoveTemplate()
	{
		try
		{
			Directory.Delete(Path.GetDirectoryName(xmlFilePath), recursive: true);
		}
		catch (IOException ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to remove template.", "Unable to remove template", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, this);
		}
	}

	private Element GetRow(TreeListNode node)
	{
		return treeList.GetDataRecordByNode(node) as Element;
	}

	private void repositoryItemButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		if (openImageFileDialog.ShowDialog() == DialogResult.OK)
		{
			treeList.FocusedNode?.SetValue(colValue, openImageFileDialog.FileName);
		}
	}

	private void treeList_ShowingEditor(object sender, CancelEventArgs e)
	{
		Element row = GetRow(treeList.FocusedNode);
		if (row.ElementType == ElementTypeEnum.ElementType.ReadOnly || !row.IsLeaf)
		{
			e.Cancel = true;
		}
	}

	private void treeListToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		TreeListHitInfo treeListHitInfo = treeList.CalcHitInfo(e.ControlMousePosition);
		if (treeListHitInfo?.Column == colName && treeList.GetDataRecordByNode(treeListHitInfo.Node) is Element element)
		{
			string tooltip = element.Tooltip;
			if (!string.IsNullOrWhiteSpace(tooltip))
			{
				TreeListCellToolTipInfo @object = new TreeListCellToolTipInfo(treeListHitInfo.Node, treeListHitInfo.Column, null);
				e.Info = new ToolTipControlInfo(@object, tooltip);
			}
		}
	}

	private void repositoryItemFontEdit_KeyDown(object sender, KeyEventArgs e)
	{
		ClearEditorOnDelete(sender, e);
	}

	private void repositoryItemColorPickEdit_KeyDown(object sender, KeyEventArgs e)
	{
		ClearEditorOnDelete(sender, e);
	}

	private void ClearEditorOnDelete(object sender, KeyEventArgs e)
	{
		if (sender is PopupBaseEdit popupBaseEdit && e.KeyCode == Keys.Delete)
		{
			popupBaseEdit.EditValue = null;
		}
	}

	private void PDFTemplateEditor_FormClosed(object sender, FormClosedEventArgs e)
	{
		if (isNewTemplate && !isNewTemplateSaved)
		{
			RemoveTemplate();
		}
	}

	private void treeList_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
	{
		if (e.Menu.MenuType != TreeListMenuType.Column)
		{
			return;
		}
		for (int num = e.Menu.Items.Count - 1; num >= 0; num--)
		{
			object tag = e.Menu.Items[num].Tag;
			if (TreeListStringId.MenuColumnColumnCustomization.Equals(tag))
			{
				e.Menu.Items[num].Enabled = treeList.CustomizationForm == null;
			}
			if (TreeListStringId.MenuColumnFilterEditor.Equals(tag) || TreeListStringId.MenuColumnFindFilterShow.Equals(tag) || TreeListStringId.MenuColumnAutoFilterRowShow.Equals(tag))
			{
				e.Menu.Items.RemoveAt(num);
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.PDFTemplateEditorForm));
		this.treeList = new DevExpress.XtraTreeList.TreeList();
		this.colName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.colValue = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.elementBindingSource = new System.Windows.Forms.BindingSource(this.components);
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.repositoryItemFontEdit = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
		this.repositoryItemColorPickEdit = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
		this.repositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.repositoryItemButtonEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.fontSizeRepositoryItemSpinEdit = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
		this.linkRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.treeListToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.saveSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.pathTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.fileNameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.openImageFileDialog = new System.Windows.Forms.OpenFileDialog();
		((System.ComponentModel.ISupportInitialize)this.treeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elementBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFontEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fontSizeRepositoryItemSpinEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linkRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pathTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fileNameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		base.SuspendLayout();
		this.treeList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.treeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[2] { this.colName, this.colValue });
		this.treeList.DataSource = this.elementBindingSource;
		this.treeList.KeyFieldName = "Id";
		this.treeList.Location = new System.Drawing.Point(12, 12);
		this.treeList.MenuManager = this.barManager;
		this.treeList.Name = "treeList";
		this.treeList.OptionsCustomization.AllowFilter = false;
		this.treeList.OptionsView.BestFitNodes = DevExpress.XtraTreeList.TreeListBestFitNodes.Visible;
		this.treeList.OptionsView.ShowIndicator = false;
		this.treeList.ParentFieldName = "ParentId";
		this.treeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.repositoryItemFontEdit, this.repositoryItemColorPickEdit, this.repositoryItemCheckEdit, this.repositoryItemButtonEdit, this.fontSizeRepositoryItemSpinEdit, this.linkRepositoryItemTextEdit });
		this.treeList.Size = new System.Drawing.Size(623, 378);
		this.treeList.TabIndex = 0;
		this.treeList.ToolTipController = this.treeListToolTipController;
		this.treeList.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(treeList_CustomNodeCellEdit);
		this.treeList.ShowingEditor += new System.ComponentModel.CancelEventHandler(treeList_ShowingEditor);
		this.colName.FieldName = "ElementName";
		this.colName.MinWidth = 200;
		this.colName.Name = "colName";
		this.colName.OptionsColumn.AllowEdit = false;
		this.colName.OptionsColumn.ReadOnly = true;
		this.colName.Visible = true;
		this.colName.VisibleIndex = 0;
		this.colName.Width = 200;
		this.colValue.FieldName = "Value";
		this.colValue.Name = "colValue";
		this.colValue.Visible = true;
		this.colValue.VisibleIndex = 1;
		this.colValue.Width = 476;
		this.elementBindingSource.DataSource = typeof(Dataedo.App.Tools.TemplateEditor.Element);
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(647, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 452);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(647, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 452);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(647, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 452);
		this.repositoryItemFontEdit.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
		this.repositoryItemFontEdit.AutoHeight = false;
		this.repositoryItemFontEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemFontEdit.Name = "repositoryItemFontEdit";
		this.repositoryItemFontEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(repositoryItemFontEdit_KeyDown);
		this.repositoryItemColorPickEdit.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
		this.repositoryItemColorPickEdit.AutoHeight = false;
		this.repositoryItemColorPickEdit.AutomaticColor = System.Drawing.Color.Black;
		this.repositoryItemColorPickEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemColorPickEdit.Name = "repositoryItemColorPickEdit";
		this.repositoryItemColorPickEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(repositoryItemColorPickEdit_KeyDown);
		this.repositoryItemCheckEdit.AutoHeight = false;
		this.repositoryItemCheckEdit.Name = "repositoryItemCheckEdit";
		this.repositoryItemButtonEdit.AutoHeight = false;
		this.repositoryItemButtonEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repositoryItemButtonEdit.Name = "repositoryItemButtonEdit";
		this.repositoryItemButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(repositoryItemButtonEdit_ButtonClick);
		this.fontSizeRepositoryItemSpinEdit.AutoHeight = false;
		this.fontSizeRepositoryItemSpinEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.fontSizeRepositoryItemSpinEdit.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.fontSizeRepositoryItemSpinEdit.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.fontSizeRepositoryItemSpinEdit.MaxValue = new decimal(new int[4] { 128, 0, 0, 0 });
		this.fontSizeRepositoryItemSpinEdit.MinValue = new decimal(new int[4] { 1, 0, 0, 0 });
		this.fontSizeRepositoryItemSpinEdit.Name = "fontSizeRepositoryItemSpinEdit";
		this.linkRepositoryItemTextEdit.AutoHeight = false;
		this.linkRepositoryItemTextEdit.Name = "linkRepositoryItemTextEdit";
		this.treeListToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.treeListToolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(treeListToolTipController_GetActiveObjectInfo);
		this.saveSimpleButton.Location = new System.Drawing.Point(461, 418);
		this.saveSimpleButton.Name = "saveSimpleButton";
		this.saveSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.saveSimpleButton.StyleController = this.layoutControl1;
		this.saveSimpleButton.TabIndex = 1;
		this.saveSimpleButton.Text = "Save";
		this.saveSimpleButton.Click += new System.EventHandler(saveSimpleButton_Click);
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.Controls.Add(this.pathTextEdit);
		this.layoutControl1.Controls.Add(this.fileNameTextEdit);
		this.layoutControl1.Controls.Add(this.cancelSimpleButton);
		this.layoutControl1.Controls.Add(this.treeList);
		this.layoutControl1.Controls.Add(this.saveSimpleButton);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(834, 215, 250, 350);
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(647, 452);
		this.layoutControl1.TabIndex = 3;
		this.layoutControl1.Text = "layoutControl1";
		this.pathTextEdit.Location = new System.Drawing.Point(92, 394);
		this.pathTextEdit.MenuManager = this.barManager;
		this.pathTextEdit.Name = "pathTextEdit";
		this.pathTextEdit.Properties.ReadOnly = true;
		this.pathTextEdit.Size = new System.Drawing.Size(543, 20);
		this.pathTextEdit.StyleController = this.layoutControl1;
		this.pathTextEdit.TabIndex = 5;
		this.fileNameTextEdit.Location = new System.Drawing.Point(92, 418);
		this.fileNameTextEdit.MenuManager = this.barManager;
		this.fileNameTextEdit.Name = "fileNameTextEdit";
		this.fileNameTextEdit.Size = new System.Drawing.Size(355, 20);
		this.fileNameTextEdit.StyleController = this.layoutControl1;
		this.fileNameTextEdit.TabIndex = 4;
		this.cancelSimpleButton.Location = new System.Drawing.Point(555, 418);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl1;
		this.cancelSimpleButton.TabIndex = 2;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(cancelSimpleButton_Click);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.layoutControlItem1, this.layoutControlItem2, this.layoutControlItem3, this.emptySpaceItem2, this.emptySpaceItem1, this.layoutControlItem4, this.layoutControlItem5 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Size = new System.Drawing.Size(647, 452);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.treeList;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(627, 382);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.saveSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(449, 406);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.cancelSimpleButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(543, 406);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(533, 406);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(439, 406);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.Control = this.fileNameTextEdit;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 406);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(439, 26);
		this.layoutControlItem4.Text = "Template name:";
		this.layoutControlItem4.TextSize = new System.Drawing.Size(77, 13);
		this.layoutControlItem5.Control = this.pathTextEdit;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 382);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(627, 24);
		this.layoutControlItem5.Text = "Path";
		this.layoutControlItem5.TextSize = new System.Drawing.Size(77, 13);
		this.openImageFileDialog.DefaultExt = "jpg";
		this.openImageFileDialog.Filter = "Image Files |*.jpeg;*.png;*.jpg;*.gif;*.bmp";
		this.openImageFileDialog.Title = "Choose image file";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(647, 452);
		base.Controls.Add(this.layoutControl1);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("PDFTemplateEditorForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "PDFTemplateEditorForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Template editor";
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(PDFTemplateEditor_FormClosed);
		((System.ComponentModel.ISupportInitialize)this.treeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elementBindingSource).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemFontEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fontSizeRepositoryItemSpinEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linkRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pathTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fileNameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
