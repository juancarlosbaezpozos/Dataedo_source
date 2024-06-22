using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Tools;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.ERD;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.PanelControls.CommonHelpers;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class RelationForm : BaseXtraForm
{
	public const int MaxObjectsCountInPopupView = 25;

	public const int RowHeightDifferenceInPopupView = 2;

	private Link link;

	private readonly IEnumerable<Node> nodes;

	private RelationRow relationRow;

	private bool relationChanged;

	private string autoName = string.Empty;

	private BindingList<Cardinality> CardinalityTypes = new BindingList<Cardinality>
	{
		new Cardinality(CardinalityTypeEnum.CardinalityType.Many),
		new Cardinality(CardinalityTypeEnum.CardinalityType.One)
	};

	private IEnumerable<ColumnRow> sourceColumns;

	private bool isEditMode;

	private bool isLinkMode;

	private bool isColumnsGridAllowedToRefresh = true;

	private bool contextShowSchema;

	private int? pkDatabaseId;

	public bool? pkTableDatabaseMultipleSchemas;

	public bool pkTableDatabaseShowSchema;

	private string pkTableDatabaseName;

	private string pkTableSchema;

	private string pkTableName;

	private string pkTableTitle;

	private Cardinality pkCardinality;

	private int? fkDatabaseId;

	public bool? fkTableDatabaseMultipleSchemas;

	public bool fkTableDatabaseShowSchema;

	private int? fkTableId;

	private string fkTableDatabaseName;

	private string fkTableSchema;

	private string fkTableName;

	private string fkTableTitle;

	private Cardinality fkCardinality;

	private DXErrorProvider errorProvider;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl;

	private TextEdit nameTextEdit;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem1;

	private SimpleButton closeSimpleButton;

	private SimpleButton okSimpleButton;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private GridControl tableRelationsColumnsGrid;

	private LayoutControlItem layoutControlItem4;

	private GridColumn tableRelationFkColumnGridColumn;

	private GridColumn tableRelationPkColumnGridColumn;

	private TableLayoutPanel tableLayoutPanel1;

	private LabelControl fkTableLabelControl;

	private LabelControl pkTableLabelControl;

	private LayoutControlItem layoutControlItem5;

	private GridLookUpEdit pkTableLookUpEdit;

	private GridLookUpEdit fkTableLookUpEdit;

	private GridColumn pkTypeGridColumn;

	private GridColumn pkObjectNameGridColumn;

	private GridColumn fkTypeGridColumn;

	private GridColumn fkObjectNameGridColumn;

	private MemoEdit descriptionMemoEdit;

	private LayoutControlItem descriptionLayoutControlItem;

	private CheckEdit showTitleCheckEdit;

	private LayoutControlItem showLabelLayoutControlItem;

	private CheckEdit hideRelationCheckEdit;

	private LayoutControlItem hideRelationLayoutControlItem;

	private RadioGroup linkStyleRadioGroup;

	private LayoutControlItem linkStyleLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private TextEdit titleTextEdit;

	private LayoutControlItem titleLayoutControlItem;

	private CheckEdit showjoinConditionCheckEdit;

	private LayoutControlItem showJoinConditionLayoutControlItem;

	private TableLayoutPanel tableLayoutPanel2;

	private GridLookUpEdit pkDatabaseLookUpEdit;

	private GridColumn gridColumn1;

	private GridColumn gridColumn2;

	private LabelControl labelControl1;

	private LabelControl labelControl2;

	private GridLookUpEdit fkDatabaseLookUpEdit;

	private GridColumn gridColumn3;

	private GridColumn gridColumn4;

	private LayoutControlItem layoutControlItem6;

	private RepositoryItemGridLookUpEdit tablesRelationsPkColumnsRepositoryItemGridLookUpEdit;

	private GridColumn tablesRelationsPkColumnsNameGridColumn;

	private GridColumn tablesRelationsPkColumnsKeyGridColumn;

	private RepositoryItemPictureEdit tablesRelationsPkColumnsKeyRepositoryItemPictureEdit;

	private RepositoryItemGridLookUpEdit tablesRelationsFkColumnsRepositoryItemGridLookUpEdit;

	private GridColumn tablesRelationsFkColumnsNameGridColumn;

	private GridColumn tablesRelationsFkColumnsKeyGridColumn;

	private RepositoryItemPictureEdit tablesRelationsFkColumnsKeyRepositoryItemPictureEdit;

	private GridColumn tableRelationFkColumnKeyGridColumn;

	private GridColumn tableRelationPkColumnKeyGridColumn;

	private RepositoryItemPictureEdit tableRelationFkColumnKeyRepositoryItemPictureEdit;

	private RepositoryItemPictureEdit tableRelationPkColumnKeyRepositoryItemPictureEdit;

	private GridView pkTableLookUpEditView;

	private GridView fkTableLookUpEditView;

	private GridView pkDatabaseGridView;

	private GridView fkDatabaseGridView;

	private GridView tablesRelationsPkColumnsRepositoryItemGridLookUpEditView;

	private GridView tablesRelationsFkColumnsRepositoryItemGridLookUpEditView;

	private TableLayoutPanel tableLayoutPanel3;

	private GridLookUpEdit pkCardinalityLookUpEdit;

	private GridView customGridUserControl2;

	private GridColumn gridColumn7;

	private GridColumn gridColumn8;

	private GridLookUpEdit fkCardinalityLookUpEdit;

	private GridColumn gridColumn5;

	private GridColumn gridColumn6;

	private LabelControl pkCradinalityLabelControl;

	private LabelControl fkCradinalityLabelControl;

	private LayoutControlItem layoutControlItem7;

	private PictureEdit pictureEdit2;

	private GridColumn pkCardinalityTypeGridColumn;

	private GridColumn pkCardinalityDisplayNameGridColumn;

	private GridColumn fkCardinalityTypeGridColumn;

	private GridColumn fkCardinalityDisplayNameGridColumn;

	private CustomGridUserControl tableRelationsColumnsGridView;

	private CustomGridUserControl customGridUserControl1;

	private PopupMenu gridPopupMenu;

	private BarManager gridBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	private RepositoryItemCustomTextEdit fkFullNameFormattedrepositoryItemCustomTextEdit;

	private RepositoryItemCustomTextEdit fkColumnFullNameFormattedRepositoryItemCustomTextEdit;

	private RepositoryItemCustomTextEdit pkColumnFullNameFormattedRepositoryItemCustomTextEdit;

	public List<RelationColumnRow> Columns { get; set; }

	private bool isRelationEditable
	{
		get
		{
			if (isLinkMode)
			{
				Link obj = link;
				if (obj != null && obj.UserRelation)
				{
					return true;
				}
			}
			if (!isLinkMode)
			{
				return relationRow?.IsEditable ?? false;
			}
			return false;
		}
	}

	private bool isUserRelation
	{
		get
		{
			if (isLinkMode)
			{
				Link obj = link;
				if (obj != null && obj.UserRelation)
				{
					return true;
				}
			}
			if (!isLinkMode)
			{
				RelationRow obj2 = relationRow;
				if (obj2 == null)
				{
					return true;
				}
				return obj2.Source != UserTypeEnum.UserType.DBMS;
			}
			return false;
		}
	}

	private SharedDatabaseTypeEnum.DatabaseType? pkDatabaseType { get; set; }

	public int? PkTableId { get; set; }

	private SharedObjectTypeEnum.ObjectType? pkTableType { get; set; }

	private SharedObjectSubtypeEnum.ObjectSubtype? pkTableSubtype { get; set; }

	private UserTypeEnum.UserType? pkTableSource { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype PkSubtype { get; private set; }

	public UserTypeEnum.UserType? PkSource { get; private set; }

	public ObjectStatusEnum.ObjectStatus PKStatus { get; private set; }

	private SharedDatabaseTypeEnum.DatabaseType? fkDatabaseType { get; set; }

	private SharedObjectTypeEnum.ObjectType? fkTableType { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype FkSubtype { get; private set; }

	public UserTypeEnum.UserType? FkSource { get; private set; }

	public ObjectStatusEnum.ObjectStatus FKStatus { get; private set; }

	public RelationForm(Link link, int databaseId, bool contextShowSchema, bool editMode = true, IEnumerable<Node> nodes = null)
		: this(null, link.ToNode.DatabaseId, link.FromNode.DatabaseId, editMode)
	{
		isColumnsGridAllowedToRefresh = false;
		this.link = link;
		this.nodes = nodes;
		if (!editMode)
		{
			this.link.ShowTitle = true;
		}
		isLinkMode = true;
		nameTextEdit.Text = link.Name;
		autoName = nameTextEdit.Text;
		titleTextEdit.Text = link.Title;
		descriptionMemoEdit.EditValue = link.Description;
		showTitleCheckEdit.Checked = link.ShowTitle == true;
		showjoinConditionCheckEdit.Checked = link.ShowJoinCondition == true;
		hideRelationCheckEdit.Checked = link.Hidden == true;
		this.contextShowSchema = contextShowSchema;
		linkStyleRadioGroup.Properties.Items.AddRange(new RadioGroupItem[2]
		{
			new RadioGroupItem("STRAIGHT", LinkStyleEnum.StraightLinkStyleDisplayString),
			new RadioGroupItem("ORTHOGONAL", LinkStyleEnum.OrthogonalLinkStyleDisplayString)
		});
		linkStyleRadioGroup.EditValue = LinkStyleEnum.TypeToString(link.LinkStyle);
		fkDatabaseId = link.ToNode.DatabaseId;
		fkDatabaseType = link.ToNode.DatabaseType;
		fkDatabaseLookUpEdit.EditValue = fkDatabaseId;
		pkDatabaseId = link.FromNode.DatabaseId;
		pkTableDatabaseMultipleSchemas = link.FromNode.IsMultipleSchemasDatabase;
		pkTableDatabaseShowSchema = link.FromNode.ShowSchema;
		pkDatabaseType = link.ToNode.DatabaseType;
		pkDatabaseLookUpEdit.EditValue = pkDatabaseId;
		fkTableId = link.ToNode.TableId;
		fkTableDatabaseMultipleSchemas = link.ToNode.IsMultipleSchemasDatabase;
		fkTableDatabaseShowSchema = link.ToNode.ShowSchema;
		fkTableType = link.ToNode.ObjectType;
		fkTableLookUpEdit.EditValue = fkTableId;
		PkTableId = link.FromNode.TableId;
		pkTableLookUpEdit.EditValue = PkTableId;
		pkTableType = link.FromNode.ObjectType;
		pkTableSubtype = link.FromNode.TableSubtype;
		pkTableSource = link.FromNode.TableSource;
		pkCardinality.Type = link.FromNodeCardinality;
		fkCardinality.Type = link.ToNodeCardinality;
		fkCardinalityLookUpEdit.EditValue = this.link.ToNodeCardinality;
		pkCardinalityLookUpEdit.EditValue = this.link.FromNodeCardinality;
		if (!isRelationEditable)
		{
			nameTextEdit.ReadOnly = (pkDatabaseLookUpEdit.ReadOnly = (pkTableLookUpEdit.ReadOnly = true));
			pkDatabaseLookUpEdit.ReadOnly = (pkTableLookUpEdit.TabStop = false);
			descriptionMemoEdit.Select();
		}
		pkTableLookUpEdit.ReadOnly = (pkDatabaseLookUpEdit.ReadOnly = !isUserRelation);
		fkDatabaseLookUpEdit.ReadOnly = (fkTableLookUpEdit.ReadOnly = true);
		Columns = new List<RelationColumnRow>();
		foreach (RelationColumnRow column in link.Columns)
		{
			Columns.Add(new RelationColumnRow(column));
		}
		if (isUserRelation)
		{
			Text = (editMode ? ("User-defined relationship: " + link.Name) : "New user-defined relationship");
		}
		else
		{
			Text = (editMode ? ("Relationship: " + link.Name) : "New relationship");
		}
		isColumnsGridAllowedToRefresh = true;
	}

	public RelationForm(IEnumerable<ColumnRow> sourceColumns, RelationRow relation, int databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, int tableId, bool contextShowSchema, bool editMode = true)
		: this(sourceColumns, relation.FKTableDatabaseId, relation.PKTableDatabaseId, editMode)
	{
		InitializeData(sourceColumns, relation, databaseId, databaseType, tableId, contextShowSchema, editMode);
	}

	public RelationForm(IEnumerable<ColumnRow> sourceColumns, RelationRow relation, IEnumerable<ColumnRow> relationColumns, int databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, int tableId, bool contextShowSchema, bool editMode = true)
		: this(sourceColumns, relation.FKTableDatabaseId, relation.PKTableDatabaseId, editMode)
	{
		if (relationColumns != null)
		{
			int num = 1;
			relation.Columns = new BindingList<RelationColumnRow>();
			foreach (ColumnRow relationColumn in relationColumns)
			{
				relation.Columns.Add(new RelationColumnRow
				{
					TableFkId = tableId,
					ColumnFkId = relationColumn.Id,
					ColumnFkName = relationColumn.Name,
					ColumnFkTitle = relationColumn.Title,
					FkUniqueConstraintsDataContainer = relationColumn.UniqueConstraintsDataContainer,
					TablePkId = -1,
					ColumnPkId = -1,
					ColumnPkName = null,
					PkUniqueConstraintsDataContainer = null,
					OrdinalPosition = num++,
					RowState = ManagingRowsEnum.ManagingRows.ForAdding
				});
			}
		}
		InitializeData(sourceColumns, relation, databaseId, databaseType, tableId, contextShowSchema, editMode);
	}

	private RelationForm(IEnumerable<ColumnRow> sourceColumns, int fkTableDatabaseId, int pkTableDatabaseId, bool editMode = true)
	{
		InitializeComponent();
		base.FormClosing += RelationForm_Closing;
		errorProvider = new DXErrorProvider();
		this.sourceColumns = sourceColumns;
		isEditMode = editMode;
		fkCardinality = new Cardinality(isPk: false);
		pkCardinality = new Cardinality(isPk: true);
		LengthValidation.SetTitleOrNameLengthLimit(nameTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(titleTextEdit);
		List<DocumentationForMenuObject> dataWithoutBusinessGlossaryForMenu = DB.Database.GetDataWithoutBusinessGlossaryForMenu();
		fkDatabaseLookUpEdit.Properties.DataSource = (pkDatabaseLookUpEdit.Properties.DataSource = dataWithoutBusinessGlossaryForMenu);
		fkCardinalityLookUpEdit.Properties.DataSource = (pkCardinalityLookUpEdit.Properties.DataSource = CardinalityTypes);
		fkCardinalityLookUpEdit.Properties.DisplayMember = (pkCardinalityLookUpEdit.Properties.DisplayMember = "DisplayName");
		fkCardinalityLookUpEdit.Properties.ValueMember = (pkCardinalityLookUpEdit.Properties.ValueMember = "Type");
		fkDatabaseLookUpEdit.BindSettingPopupHeightMethod(25);
		pkDatabaseLookUpEdit.BindSettingPopupHeightMethod(25);
		fkTableLookUpEdit.BindSettingPopupHeightMethod(25);
		pkTableLookUpEdit.BindSettingPopupHeightMethod(25);
		pkCardinalityLookUpEdit.BindSettingPopupHeightMethod(25);
		fkCardinalityLookUpEdit.BindSettingPopupHeightMethod(25);
		tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.BindSettingPopupHeightMethod(25);
		tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.BindSettingPopupHeightMethod(25);
	}

	private void InitializeData(IEnumerable<ColumnRow> sourceColumns, RelationRow relation, int databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, int tableId, bool contextShowSchema, bool editMode = true)
	{
		isColumnsGridAllowedToRefresh = false;
		this.sourceColumns = sourceColumns;
		relationRow = relation;
		isLinkMode = false;
		Columns = new List<RelationColumnRow>();
		GridLookUpEdit gridLookUpEdit = pkTableLookUpEdit;
		bool readOnly = (pkDatabaseLookUpEdit.ReadOnly = !isUserRelation);
		gridLookUpEdit.ReadOnly = readOnly;
		GridLookUpEdit gridLookUpEdit2 = fkDatabaseLookUpEdit;
		readOnly = (fkTableLookUpEdit.ReadOnly = !isUserRelation);
		gridLookUpEdit2.ReadOnly = readOnly;
		pkCardinality.Type = relation.PKCardinality.Type;
		fkCardinality.Type = relation.FKCardinality.Type;
		fkCardinalityLookUpEdit.EditValue = fkCardinality.Type;
		pkCardinalityLookUpEdit.EditValue = pkCardinality.Type;
		this.contextShowSchema = contextShowSchema;
		if (editMode)
		{
			fkDatabaseId = relation.FKTableDatabaseId;
			fkTableDatabaseMultipleSchemas = relation.FKTableDatabaseMultipleSchemas;
			fkTableDatabaseShowSchema = relation.FKTableDatabaseShowSchema;
			fkDatabaseType = relation.FKTableDatabaseType;
			fkDatabaseLookUpEdit.EditValue = fkDatabaseId;
			fkTableId = relation.FKTableId;
			fkTableLookUpEdit.EditValue = fkTableId;
			fkTableType = relation.FKObjectType;
			fkTableDatabaseName = relation.FKTableDatabaseName;
			fkTableSchema = relation.FKTableSchema;
			fkTableName = relation.FKTableName;
			fkTableTitle = relation.FKTableTitle;
			pkDatabaseId = relation.PKTableDatabaseId;
			pkTableDatabaseMultipleSchemas = relation.PKTableDatabaseMultipleSchemas;
			pkTableDatabaseShowSchema = relation.PKTableDatabaseShowSchema;
			pkDatabaseType = relation.PKTableDatabaseType;
			pkDatabaseLookUpEdit.EditValue = pkDatabaseId;
			PkTableId = relation.PKTableId;
			pkTableLookUpEdit.EditValue = PkTableId;
			pkTableType = relation.PKObjectType;
			pkTableSubtype = relation.PKObjectSubtype;
			pkTableSource = relation.PKObjectSource;
			pkTableDatabaseName = relation.PKTableDatabaseName;
			pkTableSchema = relation.PKTableSchema;
			pkTableName = relation.PKTableName;
			pkTableTitle = relation.PKTableTitle;
			nameTextEdit.EditValue = relation.Name;
			titleTextEdit.EditValue = relation.Title;
			descriptionMemoEdit.EditValue = relation.Description;
			tableRelationsColumnsGridView.OptionsBehavior.Editable = relation.Source == UserTypeEnum.UserType.USER;
			nameTextEdit.ReadOnly = relation.Source == UserTypeEnum.UserType.DBMS;
			if (relation.Columns != null)
			{
				foreach (RelationColumnRow column in relation.Columns)
				{
					Columns.Add(new RelationColumnRow(column));
				}
			}
		}
		else
		{
			object obj3 = (fkDatabaseLookUpEdit.EditValue = (pkDatabaseLookUpEdit.EditValue = databaseId));
			fkTableLookUpEdit.EditValue = tableId;
			fkDatabaseId = (pkDatabaseId = databaseId);
			SharedDatabaseTypeEnum.DatabaseType? databaseType4 = (fkDatabaseType = (pkDatabaseType = databaseType));
			fkTableId = tableId;
			GridLookUpEdit gridLookUpEdit3 = pkDatabaseLookUpEdit;
			readOnly = (pkTableLookUpEdit.ReadOnly = false);
			gridLookUpEdit3.ReadOnly = readOnly;
			GridLookUpEdit gridLookUpEdit4 = pkDatabaseLookUpEdit;
			readOnly = (pkTableLookUpEdit.TabStop = true);
			gridLookUpEdit4.TabStop = readOnly;
			foreach (RelationColumnRow column2 in relation.Columns)
			{
				Columns.Add(new RelationColumnRow(column2));
			}
			nameTextEdit.Text = (autoName = RelationsRegexHelper.GetPartialRelationName(fkTableName));
			base.ActiveControl = pkTableLookUpEdit;
		}
		LayoutControlItem layoutControlItem = showLabelLayoutControlItem;
		LayoutControlItem layoutControlItem2 = showJoinConditionLayoutControlItem;
		LayoutControlItem layoutControlItem3 = hideRelationLayoutControlItem;
		LayoutVisibility layoutVisibility2 = (linkStyleLayoutControlItem.Visibility = LayoutVisibility.Never);
		LayoutVisibility layoutVisibility4 = (layoutControlItem3.Visibility = layoutVisibility2);
		LayoutVisibility layoutVisibility7 = (layoutControlItem.Visibility = (layoutControlItem2.Visibility = layoutVisibility4));
		int num = base.Size.Height - (showLabelLayoutControlItem.Height + showJoinConditionLayoutControlItem.Height + hideRelationLayoutControlItem.Height + linkStyleLayoutControlItem.Height - tableLayoutPanel3.Height * 2);
		base.Size = new Size(base.Size.Width, num);
		if (isUserRelation)
		{
			Text = (editMode ? ("User-defined relationship: " + relation.Name) : "New user-defined relationship");
		}
		else
		{
			Text = (editMode ? ("Relationship: " + relation.Name) : "New relationship");
		}
		isColumnsGridAllowedToRefresh = true;
	}

	private Bitmap SetIcon(bool pkRelations = false)
	{
		if (fkCardinality != null && pkCardinality != null)
		{
			return Icons.SetIconDocForRelation(isUserRelation, fkCardinality, pkCardinality, pkRelations);
		}
		return new Bitmap(24, 16);
	}

	private void ErdNewRelationForm_Load(object sender, EventArgs e)
	{
		if (isLinkMode)
		{
			LayoutVisibility layoutVisibility3 = (showLabelLayoutControlItem.Visibility = (showJoinConditionLayoutControlItem.Visibility = LayoutVisibility.Always));
		}
		RefeshColumnsGrid();
		if (isRelationEditable && isUserRelation)
		{
			AddRow();
		}
		else
		{
			for (int i = 0; i < tableRelationsColumnsGridView.VisibleColumns.Count; i++)
			{
				tableRelationsColumnsGridView.VisibleColumns[i].OptionsColumn.AllowEdit = false;
			}
		}
		if (Columns.Count == 1 && Columns.Any((RelationColumnRow x) => (x.IsPartiallyReady || x.IsEmpty) && pkTableLookUpEdit.ReadOnly))
		{
			base.ActiveControl = tableRelationsColumnsGrid;
			tableRelationsColumnsGridView.FocusedRowHandle = 0;
			tableRelationsColumnsGridView.FocusedColumn = tableRelationsColumnsGridView.VisibleColumns[1];
			tableRelationsColumnsGridView.ShowEditor();
		}
	}

	private void RefeshColumnsGrid()
	{
		if (!isColumnsGridAllowedToRefresh)
		{
			return;
		}
		TableViewHelpers.LoadColumnsWithoutEmptyFirst(tablesRelationsPkColumnsRepositoryItemGridLookUpEdit, null, PkTableId);
		TableViewHelpers.LoadColumnsWithoutEmptyFirst(tablesRelationsFkColumnsRepositoryItemGridLookUpEdit, null, fkTableId);
		if (Columns != null && Columns.Any((RelationColumnRow x) => x.IsPartiallyReady))
		{
			Columns = Columns.Where((RelationColumnRow x) => !x.IsEmpty).ToList();
		}
		tableRelationsColumnsGrid.DataSource = Columns;
		errorProvider.ClearErrors();
	}

	private void gridLookUpEditView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		try
		{
			if (!(sender is GridView gridView) || !(e?.Column?.FieldName == "ObjectType"))
			{
				return;
			}
			string text = e.CellValue?.ToString()?.ToUpper();
			Bitmap image = new Bitmap(24, 24);
			switch (text)
			{
			case "TABLE":
			case "STRUCTURE":
			{
				bool isUserSource = gridView.GetRowCellValue(e.RowHandle, "Source")?.ToString() == "USER";
				bool isDeleted = gridView.GetRowCellValue(e.RowHandle, "Status")?.ToString() == "D";
				string text2 = gridView.GetRowCellValue(e.RowHandle, "ObjectType")?.ToString()?.ToLower();
				string subtype = gridView.GetRowCellValue(e.RowHandle, "Subtype")?.ToString()?.ToLower();
				if (!string.IsNullOrWhiteSpace(text2))
				{
					image = Icons.SetObjectIcon(text2, subtype, isUserSource, isDeleted);
				}
				break;
			}
			case "VIEW":
				image = Resources.view_16;
				break;
			default:
				if (sender is GridView gridView2 && (gridView2 == pkDatabaseGridView || gridView2 == fkDatabaseGridView))
				{
					if ((sender as GridView).GetRow(e.RowHandle) is DocumentationForMenuObject documentationForMenuObject && !string.IsNullOrEmpty(documentationForMenuObject?.Type) && !string.IsNullOrWhiteSpace(documentationForMenuObject?.Class))
					{
						image = IconsSupport.GetDatabaseIconByName16(SharedDatabaseTypeEnum.StringToType(documentationForMenuObject?.Type), SharedObjectTypeEnum.StringToType(documentationForMenuObject?.Class));
					}
					break;
				}
				return;
			}
			Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, 16, 16);
			e.Cache.DrawImage(image, rect);
			e.Handled = true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, this);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			Save();
			break;
		case Keys.Escape:
			Close();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void AddRow()
	{
		if (link != null && link.FromNode.Columns.Any((NodeColumnDB x) => x.PrimaryKey || x.UniqueConstraintsDataContainer.IsUserDefinedPk || x.UniqueConstraintsDataContainer.IsPk) && Columns.Count == 0 && !isEditMode)
		{
			foreach (NodeColumnDB column in link.FromNode.Columns.Where((NodeColumnDB x) => x.PrimaryKey || x.UniqueConstraintsDataContainer.IsUserDefinedPk || x.UniqueConstraintsDataContainer.IsPk))
			{
				RelationColumnRow.AddRelationRow(Columns, column, link.ToNode.Columns.FirstOrDefault((NodeColumnDB x) => x.Name.Equals(column.Name)));
			}
			AddLastEmptyColumn();
		}
		else if (!Columns.Any((RelationColumnRow x) => x.IsEmpty))
		{
			RelationColumnRow.AddEmpty(Columns);
		}
	}

	private void okSimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void Save()
	{
		if (pkDatabaseLookUpEdit.EditValue == null)
		{
			errorProvider.SetError(pkDatabaseLookUpEdit, "PK Database name is required");
			base.DialogResult = DialogResult.None;
			return;
		}
		errorProvider.ClearErrors();
		if (pkTableLookUpEdit.EditValue == null)
		{
			base.DialogResult = DialogResult.None;
			errorProvider.SetError(pkTableLookUpEdit, "PK Table name is required");
			return;
		}
		errorProvider.ClearErrors();
		if (!ValidateAllColumns())
		{
			base.DialogResult = DialogResult.None;
		}
		else if (isLinkMode)
		{
			link.IsModifiedInRelationForm = true;
			link.Name = nameTextEdit.Text;
			link.Title = titleTextEdit.Text;
			link.Description = descriptionMemoEdit.Text;
			link.ShowTitle = showTitleCheckEdit.Checked;
			link.ShowJoinCondition = showjoinConditionCheckEdit.Checked;
			link.Hidden = hideRelationCheckEdit.Checked;
			link.LinkStyle = LinkStyleEnum.ObjectToType(linkStyleRadioGroup.EditValue);
			link.FromNodeCardinality = pkCardinality.Type;
			link.ToNodeCardinality = fkCardinality.Type;
			link.Columns = new List<RelationColumnRow>(Columns.Where((RelationColumnRow c) => !c.IsEmpty).ToList());
			link.RefreshUserRelationHint();
			RelationRow relationRow = new RelationRow(link);
			if (PkTableId != link.FromNode.TableId)
			{
				relationRow.PKTableId = PkTableId ?? (-1);
				link.SetFromNode(nodes.FirstOrDefault((Node x) => x.TableId == PkTableId));
			}
			if (relationRow.Id == -1)
			{
				DB.Relation.Insert(relationRow, this);
				link.RelationId = relationRow.Id;
				DB.ErdLink.InsertLink(new LinkDB(link), this);
			}
			else
			{
				DB.Relation.Update(relationRow, this);
				DB.ErdLink.UpdateLink(new LinkDB(link), this);
			}
			link.SetKey(relationRow.Id);
			WorkWithDataedoTrackingHelper.TrackFirstInSessionRelationSave();
			base.DialogResult = DialogResult.OK;
		}
		else if (PkTableId.HasValue && pkDatabaseId.HasValue && fkTableId.HasValue && fkDatabaseId.HasValue)
		{
			this.relationRow.Name = nameTextEdit.Text;
			this.relationRow.Title = titleTextEdit.Text;
			this.relationRow.Columns = new BindingList<RelationColumnRow>(Columns.Where((RelationColumnRow c) => !c.IsEmpty).ToList());
			this.relationRow.PKTableDatabaseId = pkDatabaseId.Value;
			this.relationRow.PKTableDatabaseMultipleSchemas = pkTableDatabaseMultipleSchemas;
			this.relationRow.PKTableDatabaseType = pkDatabaseType;
			this.relationRow.PKTableId = PkTableId.Value;
			this.relationRow.PKObjectType = pkTableType;
			this.relationRow.PKObjectSubtype = pkTableSubtype;
			this.relationRow.PKObjectSource = pkTableSource ?? UserTypeEnum.UserType.DBMS;
			this.relationRow.PKTableDatabaseName = pkTableDatabaseName;
			this.relationRow.PKTableSchema = pkTableSchema;
			this.relationRow.PKTableName = pkTableName;
			this.relationRow.PKTableTitle = pkTableTitle;
			this.relationRow.FKTableDatabaseId = fkDatabaseId.Value;
			this.relationRow.FKTableDatabaseMultipleSchemas = fkTableDatabaseMultipleSchemas;
			this.relationRow.FKTableDatabaseType = fkDatabaseType;
			this.relationRow.FKTableId = fkTableId.Value;
			this.relationRow.FKObjectType = fkTableType;
			this.relationRow.FKTableDatabaseName = fkTableDatabaseName;
			this.relationRow.FKTableSchema = fkTableSchema;
			this.relationRow.FKTableName = fkTableName;
			this.relationRow.FKTableTitle = fkTableTitle;
			this.relationRow.IsPKTableIdSet = true;
			this.relationRow.IsFKTableIdSet = true;
			this.relationRow.Source = ((this.relationRow.Source == UserTypeEnum.UserType.DBMS) ? UserTypeEnum.UserType.DBMS : UserTypeEnum.UserType.USER);
			this.relationRow.Description = descriptionMemoEdit.Text;
			this.relationRow.PKCardinality.Type = pkCardinality.Type;
			this.relationRow.FKCardinality.Type = fkCardinality.Type;
			if (relationChanged)
			{
				this.relationRow.SetModified();
			}
			if (this.relationRow.Id == 0)
			{
				DB.Relation.Insert(this.relationRow, this);
			}
			else
			{
				DB.Relation.Update(this.relationRow, this);
			}
			WorkWithDataedoTrackingHelper.TrackFirstInSessionRelationSave();
			base.DialogResult = DialogResult.OK;
		}
	}

	private void closeSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private bool ValidateAllColumns()
	{
		tableRelationsColumnsGridView.ClearColumnErrors();
		bool flag = true;
		StringBuilder errors = new StringBuilder();
		foreach (RelationColumnRow column in Columns)
		{
			if (column != null)
			{
				if (flag)
				{
					flag = ValidateColumn(column);
					flag &= CheckIfPkAndFkColumnsExists(column, ref errors);
				}
				else
				{
					ValidateColumn(column);
					CheckIfPkAndFkColumnsExists(column, ref errors);
				}
			}
		}
		if (errors.Length > 0)
		{
			errors.AppendLine();
			errors.AppendLine("Please refresh the data and try again.");
			GeneralMessageBoxesHandling.Show(errors.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		tableRelationsColumnsGridView.RefreshData();
		return flag;
	}

	private bool CheckIfPkAndFkColumnsExists(RelationColumnRow column, ref StringBuilder errors)
	{
		bool result = true;
		if (column.IsEmpty)
		{
			return result;
		}
		if (!DB.Column.CheckIfColumnWithIdExists(column.ColumnPkId))
		{
			result = false;
			errors.AppendLine($"Could not find the '{column.ColumnPkName}' column with id {column.ColumnPkId} in the database.");
		}
		if (!DB.Column.CheckIfColumnWithIdExists(column.ColumnFkId))
		{
			result = false;
			errors.AppendLine($"Could not find the '{column.ColumnFkName}' column with id {column.ColumnFkId} in the database.");
		}
		return result;
	}

	private bool ValidateColumn(RelationColumnRow column)
	{
		bool result = true;
		if (Columns.Count >= 1 && !column.IsReady)
		{
			if (tableRelationsColumnsGridView.FocusedRowHandle < 0)
			{
				tableRelationsColumnsGridView.FocusedRowHandle = tableRelationsColumnsGridView.RowCount - 1;
			}
			if (column.ColumnFkId == -1 || column.ColumnPkId == -1)
			{
				result = false;
			}
			column.IsValidated = true;
		}
		return result;
	}

	private void tablesRelationsFkColumnsRepositoryItemGridLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		HandleRelationColumnChange(sender, isPk: false);
	}

	private void tablesRelationsPkColumnsRepositoryItemGridLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		HandleRelationColumnChange(sender, isPk: true);
	}

	private void tableRelationsColumnsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		gridPopupMenu.ShowPopupMenu(sender, e, inRowCellOnly: false);
	}

	private void tableRelationsColumnsGridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (tableRelationsColumnsGridView.GetRow(e.RowHandle) is RelationColumnRow relationColumnRow)
		{
			e.Appearance.BackColor = relationColumnRow.RowColoring;
			e.Appearance.ForeColor = relationColumnRow.RowColoringForeColor;
		}
	}

	private void tableRelationsColumnsGrid_ProcessGridKey(object sender, KeyEventArgs e)
	{
		if (!(sender as GridControl).MainView.IsEditing && e.KeyCode == Keys.Delete && isUserRelation)
		{
			deleteColumnRow();
		}
	}

	private void deleteColumnRow()
	{
		tableRelationsColumnsGridView.CloseEditor();
		int[] selectedRows = tableRelationsColumnsGridView.GetSelectedRows();
		if (selectedRows.Count() > 0)
		{
			for (int num = selectedRows.Count() - 1; num >= 0; num--)
			{
				RelationColumnRow relationColumnRow = tableRelationsColumnsGridView.GetRow(selectedRows[num]) as RelationColumnRow;
				if (relationColumnRow.RowState != ManagingRowsEnum.ManagingRows.ForAdding)
				{
					Columns.Remove(relationColumnRow);
					relationChanged = true;
				}
				else
				{
					relationColumnRow.Clear();
				}
				RefreshRelationGrids();
			}
		}
		RefreshRelationGrids();
		AddRow();
	}

	private void HandleRelationColumnChange(object sender, bool isPk)
	{
		ColumnRow columnRow = ((GridLookUpEdit)sender).GetSelectedDataRow() as ColumnRow;
		if (tableRelationsColumnsGridView.GetFocusedRow() is RelationColumnRow relationColumnRow && columnRow != null)
		{
			if (isPk)
			{
				relationColumnRow.ColumnPkId = columnRow.Id;
				relationColumnRow.ColumnPkName = columnRow.Name;
				relationColumnRow.ColumnPkTitle = columnRow.Title;
				relationColumnRow.ColumnPkPath = columnRow.Path;
				relationColumnRow.PkUniqueConstraintsDataContainer = columnRow.UniqueConstraintsDataContainer;
			}
			else
			{
				relationColumnRow.ColumnFkId = columnRow.Id;
				relationColumnRow.ColumnFkName = columnRow.Name;
				relationColumnRow.ColumnFkTitle = columnRow.Title;
				relationColumnRow.ColumnFkPath = columnRow.Path;
				relationColumnRow.FkUniqueConstraintsDataContainer = columnRow.UniqueConstraintsDataContainer;
			}
			if (relationColumnRow != null)
			{
				AddRow();
			}
			relationChanged = true;
			RefreshRelationGrids();
		}
	}

	private void RefreshRelationGrids()
	{
		tableRelationsColumnsGridView.RefreshRow(tableRelationsColumnsGridView.FocusedRowHandle);
		RefeshColumnsGrid();
	}

	private void pkDatabaseLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		relationChanged = (sender as BaseEdit).IsModified;
		if (!string.IsNullOrWhiteSpace(pkDatabaseLookUpEdit.EditValue?.ToString()))
		{
			DocumentationForMenuObject documentationForMenuObject = pkDatabaseLookUpEdit.GetSelectedDataRow() as DocumentationForMenuObject;
			pkDatabaseId = documentationForMenuObject?.DatabaseId;
			pkTableDatabaseMultipleSchemas = documentationForMenuObject?.MultipleSchemas;
			pkTableDatabaseShowSchema = DatabaseRow.GetShowSchema(documentationForMenuObject.ShowSchema, documentationForMenuObject.ShowSchemaOverride);
			pkDatabaseType = DatabaseTypeEnum.StringToType(documentationForMenuObject?.Type);
			pkTableDatabaseName = PrepareValue.ToString(documentationForMenuObject?.Name);
			int num = 1;
			if (pkDatabaseId.HasValue)
			{
				List<TableWithSchemaByDatabaseObject> tablesAndViewsWithSchemaByDatabase = DB.Table.GetTablesAndViewsWithSchemaByDatabase(pkDatabaseId.Value, contextShowSchema);
				SetObjectsNames(tablesAndViewsWithSchemaByDatabase, pkDatabaseType, pkTableDatabaseShowSchema || contextShowSchema);
				num = (((tablesAndViewsWithSchemaByDatabase?.Count() ?? 1) < 1) ? 1 : (tablesAndViewsWithSchemaByDatabase?.Count() ?? 1));
				num = ((num > 25) ? 25 : num);
				pkTableLookUpEdit.Properties.DataSource = tablesAndViewsWithSchemaByDatabase;
			}
			else
			{
				pkTableLookUpEdit.Properties.DataSource = null;
			}
			PkTableId = null;
			pkTableType = null;
			pkTableSubtype = null;
			pkTableSource = null;
			pkTableLookUpEdit.EditValue = PkTableId;
		}
	}

	private void pkTableLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		TableWithSchemaByDatabaseObject tableWithSchemaByDatabaseObject = pkTableLookUpEdit.GetSelectedDataRow() as TableWithSchemaByDatabaseObject;
		if (!string.IsNullOrWhiteSpace(pkTableLookUpEdit.EditValue?.ToString()) && tableWithSchemaByDatabaseObject != null)
		{
			PkTableId = tableWithSchemaByDatabaseObject?.Id;
			pkTableType = SharedObjectTypeEnum.StringToType(tableWithSchemaByDatabaseObject?.ObjectType);
			pkTableSubtype = SharedObjectSubtypeEnum.StringToType(pkTableType, tableWithSchemaByDatabaseObject?.Subtype);
			pkTableSource = UserTypeEnum.ObjectToType(tableWithSchemaByDatabaseObject?.Source);
			pkTableSchema = tableWithSchemaByDatabaseObject?.Schema;
			pkTableName = tableWithSchemaByDatabaseObject?.BaseName;
			pkTableTitle = tableWithSchemaByDatabaseObject?.Title;
			PkSubtype = SharedObjectSubtypeEnum.StringToType(pkTableType, tableWithSchemaByDatabaseObject?.Subtype);
			PkSource = UserTypeEnum.ObjectToType(tableWithSchemaByDatabaseObject?.Source);
			PKStatus = ObjectStatusEnum.GetStatusFromString(tableWithSchemaByDatabaseObject?.Status);
		}
		else
		{
			PkTableId = null;
		}
		if (Columns != null)
		{
			Columns.ForEach(delegate(RelationColumnRow x)
			{
				x.ColumnPkId = -1;
				x.ColumnPkName = null;
				x.PkUniqueConstraintsDataContainer = null;
			});
		}
		if (autoName.Equals(nameTextEdit.Text) && pkTableLookUpEdit.EditValue != null && !string.IsNullOrEmpty(pkTableLookUpEdit.EditValue.ToString()))
		{
			nameTextEdit.Text = (autoName = RelationsRegexHelper.GetRelationName(fkTableName, pkTableName));
		}
		RefeshColumnsGrid();
		if (!isLinkMode && !isEditMode)
		{
			List<RelationColumnRow> columns = Columns;
			if (columns != null && !columns.Any((RelationColumnRow x) => x.RowGeneratedAutomatically))
			{
				List<RelationColumnRow> columns2 = Columns;
				if (columns2 != null && columns2.FirstOrDefault()?.ColumnFkId > 0)
				{
					List<RelationColumnRow> columns3 = Columns;
					if (columns3 != null && columns3.FirstOrDefault()?.ColumnPkId < 0)
					{
						return;
					}
				}
			}
			Columns?.Clear();
			BindingList<ColumnRow> bindingList = tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.DataSource as BindingList<ColumnRow>;
			BindingList<ColumnRow> bindingList2 = tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.DataSource as BindingList<ColumnRow>;
			if (bindingList != null)
			{
				foreach (ColumnRow column in bindingList.Where((ColumnRow x) => x.IsPk || x.UniqueConstraintsDataContainer.IsUserDefinedPk || x.UniqueConstraintsDataContainer.IsPk))
				{
					ColumnRow columnRow = bindingList2?.FirstOrDefault((ColumnRow x) => x.Name.Equals(column.Name));
					RelationColumn fkColumn = ((columnRow != null) ? new RelationColumn(columnRow) : null);
					RelationColumnRow.AddRelationRow(Columns, new RelationColumn(column), fkColumn);
				}
			}
			if (Columns.Count == 0)
			{
				RelationColumnRow.AddEmpty(Columns);
			}
			AddLastEmptyColumn();
			base.ActiveControl = tableRelationsColumnsGrid;
			tableRelationsColumnsGridView.FocusedRowHandle = 0;
			tableRelationsColumnsGridView.RefreshData();
			tableRelationsColumnsGridView.Invalidate();
			tableRelationsColumnsGridView.FocusedColumn = tableRelationsColumnsGridView.VisibleColumns[1];
			tableRelationsColumnsGridView.ShowEditor();
		}
		relationChanged = (sender as BaseEdit).IsModified;
	}

	private void AddLastEmptyColumn()
	{
		if (Columns.ToList().TrueForAll((RelationColumnRow x) => x.ColumnFkId > 0 && x.ColumnPkId > 0))
		{
			Columns.Add(new RelationColumnRow(Columns.Select((RelationColumnRow x) => x.OrdinalPosition).Max()));
		}
	}

	private void fkDatabaseLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		relationChanged = (sender as BaseEdit).IsModified;
		if (!string.IsNullOrWhiteSpace(fkDatabaseLookUpEdit.EditValue?.ToString()))
		{
			DocumentationForMenuObject documentationForMenuObject = fkDatabaseLookUpEdit.GetSelectedDataRow() as DocumentationForMenuObject;
			fkDatabaseId = documentationForMenuObject?.DatabaseId;
			fkTableDatabaseMultipleSchemas = documentationForMenuObject?.MultipleSchemas;
			fkTableDatabaseShowSchema = DatabaseRow.GetShowSchema(documentationForMenuObject.ShowSchema, documentationForMenuObject.ShowSchemaOverride);
			fkDatabaseType = DatabaseTypeEnum.StringToType(documentationForMenuObject?.Type);
			fkTableDatabaseName = documentationForMenuObject?.Name;
			fkTableId = null;
			fkTableType = null;
			fkTableLookUpEdit.EditValue = fkTableId;
			int num = 1;
			if (fkDatabaseId.HasValue)
			{
				List<TableWithSchemaByDatabaseObject> tablesAndViewsWithSchemaByDatabase = DB.Table.GetTablesAndViewsWithSchemaByDatabase(fkDatabaseId.Value, contextShowSchema);
				SetObjectsNames(tablesAndViewsWithSchemaByDatabase, fkDatabaseType, fkTableDatabaseShowSchema || contextShowSchema);
				num = (((tablesAndViewsWithSchemaByDatabase?.Count() ?? 1) < 1) ? 1 : (tablesAndViewsWithSchemaByDatabase?.Count() ?? 1));
				num = ((num > 25) ? 25 : num);
				fkTableLookUpEdit.Properties.DataSource = tablesAndViewsWithSchemaByDatabase;
			}
			else
			{
				fkTableLookUpEdit.Properties.DataSource = null;
			}
			RefeshColumnsGrid();
		}
	}

	private void fkTableLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		relationChanged = (sender as BaseEdit).IsModified;
		if (!string.IsNullOrWhiteSpace(fkTableLookUpEdit.EditValue?.ToString()))
		{
			TableWithSchemaByDatabaseObject tableWithSchemaByDatabaseObject = fkTableLookUpEdit.GetSelectedDataRow() as TableWithSchemaByDatabaseObject;
			fkTableId = tableWithSchemaByDatabaseObject?.Id;
			fkTableType = SharedObjectTypeEnum.StringToType(tableWithSchemaByDatabaseObject?.ObjectType);
			fkTableSchema = PrepareValue.ToString(tableWithSchemaByDatabaseObject?.Schema);
			fkTableName = PrepareValue.ToString(tableWithSchemaByDatabaseObject?.BaseName) ?? string.Empty;
			fkTableTitle = PrepareValue.ToString(tableWithSchemaByDatabaseObject?.Title) ?? string.Empty;
			FkSubtype = SharedObjectSubtypeEnum.StringToType(fkTableType, tableWithSchemaByDatabaseObject?.Subtype);
			FkSource = UserTypeEnum.ObjectToType(tableWithSchemaByDatabaseObject?.Source);
			FKStatus = ObjectStatusEnum.GetStatusFromString(tableWithSchemaByDatabaseObject?.Status);
			RefeshColumnsGrid();
			relationChanged = (sender as BaseEdit).IsModified;
			if (isUserRelation && autoName.Equals(nameTextEdit.Text) && !string.IsNullOrEmpty(pkTableLookUpEdit.EditValue?.ToString()) && !string.IsNullOrEmpty(fkTableLookUpEdit.EditValue?.ToString()))
			{
				nameTextEdit.Text = (autoName = RelationsRegexHelper.GetRelationName(fkTableName, pkTableName));
			}
		}
	}

	private void FieldEditValueChanged(object sender, EventArgs e)
	{
		relationChanged = (sender as BaseEdit).IsModified;
	}

	private void removeRowBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		deleteColumnRow();
	}

	private void gridPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		if (!Grids.GetBeforePopupIsRowClicked(sender) || Grids.GetBeforePopupShouldCancel(sender))
		{
			e.Cancel = true;
			return;
		}
		bool num = isUserRelation;
		RelationColumnRow relationColumnRow = tableRelationsColumnsGridView.GetFocusedRow() as RelationColumnRow;
		if (!num || (relationColumnRow.ColumnPkId == -1 && relationColumnRow.ColumnFkId == -1 && tableRelationsColumnsGridView.FocusedRowHandle == tableRelationsColumnsGridView.RowCount - 1))
		{
			e.Cancel = true;
		}
	}

	private void fkCardinalityLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		fkCardinality.Type = (fkCardinalityLookUpEdit.GetSelectedDataRow() as Cardinality).Type;
		pictureEdit2.EditValue = SetIcon(pkRelations: true);
		relationChanged = (sender as BaseEdit).IsModified;
	}

	private void pkCardinalityLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		pkCardinality.Type = (pkCardinalityLookUpEdit.GetSelectedDataRow() as Cardinality).Type;
		pictureEdit2.EditValue = SetIcon(pkRelations: true);
		relationChanged = (sender as BaseEdit).IsModified;
	}

	private void tableRelationsColumnsGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		relationChanged = true;
	}

	private void linkStyleRadioGroup_EditValueChanging(object sender, ChangingEventArgs e)
	{
		relationChanged = (sender as BaseEdit).IsModified;
	}

	private void RelationForm_Closing(object sender, CancelEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save();
		}
		else if (relationChanged)
		{
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Relationship has been changed, would you like to save these changes?", "Relationship has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				Save();
			}
			else if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	private void SetObjectsNames(IEnumerable<TableWithSchemaByDatabaseObject> objects, SharedDatabaseTypeEnum.DatabaseType? databaseType, bool? hasDatabaseMultipleSchemas)
	{
		foreach (TableWithSchemaByDatabaseObject @object in objects)
		{
			@object.Name = DBTreeMenu.SetNameOnlySchema(@object.Schema, @object.BaseName, hasDatabaseMultipleSchemas == true);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.RelationForm));
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
		this.pkCardinalityLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
		this.customGridUserControl2 = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.pkCardinalityTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.pkCardinalityDisplayNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fkCardinalityLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
		this.customGridUserControl1 = new Dataedo.App.UserControls.CustomGridUserControl();
		this.fkCardinalityTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fkCardinalityDisplayNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.pkCradinalityLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.fkCradinalityLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.pictureEdit2 = new DevExpress.XtraEditors.PictureEdit();
		this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
		this.pkDatabaseLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
		this.pkDatabaseGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.fkDatabaseLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
		this.fkDatabaseGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.showjoinConditionCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.titleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.linkStyleRadioGroup = new DevExpress.XtraEditors.RadioGroup();
		this.hideRelationCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.showTitleCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
		this.pkTableLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
		this.pkTableLookUpEditView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.pkTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.pkObjectNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fkTableLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.pkTableLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.fkTableLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
		this.fkTableLookUpEditView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.fkTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fkObjectNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tableRelationsColumnsGrid = new DevExpress.XtraGrid.GridControl();
		this.tableRelationsColumnsGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.tableRelationFkColumnKeyGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tableRelationFkColumnKeyRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.tableRelationFkColumnGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.tablesRelationsFkColumnsKeyGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tablesRelationsFkColumnsKeyRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.tablesRelationsFkColumnsNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fkColumnFullNameFormattedRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.tableRelationPkColumnKeyGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tableRelationPkColumnKeyRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.tableRelationPkColumnGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.tablesRelationsPkColumnsKeyGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tablesRelationsPkColumnsKeyRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.tablesRelationsPkColumnsNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.pkColumnFullNameFormattedRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.closeSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.nameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.descriptionMemoEdit = new DevExpress.XtraEditors.MemoEdit();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.descriptionLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.showLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.hideRelationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.linkStyleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.titleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.showJoinConditionLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.gridPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.gridBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		DevExpress.XtraBars.BarButtonItem barButtonItem = new DevExpress.XtraBars.BarButtonItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		this.tableLayoutPanel3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pkCardinalityLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customGridUserControl2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fkCardinalityLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customGridUserControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit2.Properties).BeginInit();
		this.tableLayoutPanel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pkDatabaseLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pkDatabaseGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fkDatabaseLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fkDatabaseGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.showjoinConditionCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linkStyleRadioGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hideRelationCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.showTitleCheckEdit.Properties).BeginInit();
		this.tableLayoutPanel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pkTableLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pkTableLookUpEditView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fkTableLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fkTableLookUpEditView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationsColumnsGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationsColumnsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationFkColumnKeyRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsFkColumnsKeyRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fkColumnFullNameFormattedRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationPkColumnKeyRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsPkColumnsKeyRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pkColumnFullNameFormattedRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionMemoEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.showLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hideRelationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linkStyleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.showJoinConditionLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridBarManager).BeginInit();
		base.SuspendLayout();
		barButtonItem.Caption = "Remove row";
		barButtonItem.Id = 0;
		barButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		barButtonItem.Name = "removeRowBarButtonItem";
		barButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(removeRowBarButtonItem_ItemClick);
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.tableLayoutPanel3);
		this.layoutControl.Controls.Add(this.tableLayoutPanel2);
		this.layoutControl.Controls.Add(this.showjoinConditionCheckEdit);
		this.layoutControl.Controls.Add(this.titleTextEdit);
		this.layoutControl.Controls.Add(this.linkStyleRadioGroup);
		this.layoutControl.Controls.Add(this.hideRelationCheckEdit);
		this.layoutControl.Controls.Add(this.showTitleCheckEdit);
		this.layoutControl.Controls.Add(this.tableLayoutPanel1);
		this.layoutControl.Controls.Add(this.tableRelationsColumnsGrid);
		this.layoutControl.Controls.Add(this.closeSimpleButton);
		this.layoutControl.Controls.Add(this.okSimpleButton);
		this.layoutControl.Controls.Add(this.nameTextEdit);
		this.layoutControl.Controls.Add(this.descriptionMemoEdit);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(797, 259, 650, 540);
		this.layoutControl.OptionsFocus.EnableAutoTabOrder = false;
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(568, 691);
		this.layoutControl.TabIndex = 1;
		this.layoutControl.Text = "layoutControl1";
		this.tableLayoutPanel3.ColumnCount = 3;
		this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37f));
		this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel3.Controls.Add(this.pkCardinalityLookUpEdit, 2, 1);
		this.tableLayoutPanel3.Controls.Add(this.fkCardinalityLookUpEdit, 0, 1);
		this.tableLayoutPanel3.Controls.Add(this.pkCradinalityLabelControl, 2, 0);
		this.tableLayoutPanel3.Controls.Add(this.fkCradinalityLabelControl, 0, 0);
		this.tableLayoutPanel3.Controls.Add(this.pictureEdit2, 1, 1);
		this.tableLayoutPanel3.Location = new System.Drawing.Point(13, 207);
		this.tableLayoutPanel3.MinimumSize = new System.Drawing.Size(542, 42);
		this.tableLayoutPanel3.Name = "tableLayoutPanel3";
		this.tableLayoutPanel3.RowCount = 2;
		this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15f));
		this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15f));
		this.tableLayoutPanel3.Size = new System.Drawing.Size(542, 42);
		this.tableLayoutPanel3.TabIndex = 12;
		this.pkCardinalityLookUpEdit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pkCardinalityLookUpEdit.Location = new System.Drawing.Point(292, 18);
		this.pkCardinalityLookUpEdit.Name = "pkCardinalityLookUpEdit";
		this.pkCardinalityLookUpEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.pkCardinalityLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.pkCardinalityLookUpEdit.Properties.DisplayMember = "displayname";
		this.pkCardinalityLookUpEdit.Properties.ImmediatePopup = true;
		this.pkCardinalityLookUpEdit.Properties.NullText = "";
		this.pkCardinalityLookUpEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
		this.pkCardinalityLookUpEdit.Properties.PopupView = this.customGridUserControl2;
		this.pkCardinalityLookUpEdit.Properties.ShowFooter = false;
		this.pkCardinalityLookUpEdit.Properties.ValueMember = "type";
		this.pkCardinalityLookUpEdit.Size = new System.Drawing.Size(247, 20);
		this.pkCardinalityLookUpEdit.TabIndex = 0;
		this.pkCardinalityLookUpEdit.TabStop = false;
		this.pkCardinalityLookUpEdit.EditValueChanged += new System.EventHandler(pkCardinalityLookUpEdit_EditValueChanged);
		this.customGridUserControl2.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.pkCardinalityTypeGridColumn, this.pkCardinalityDisplayNameGridColumn });
		this.customGridUserControl2.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.customGridUserControl2.Name = "customGridUserControl2";
		this.customGridUserControl2.OptionsBehavior.AutoPopulateColumns = false;
		this.customGridUserControl2.OptionsCustomization.AllowFilter = false;
		this.customGridUserControl2.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.customGridUserControl2.OptionsView.ShowColumnHeaders = false;
		this.customGridUserControl2.OptionsView.ShowGroupPanel = false;
		this.customGridUserControl2.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.customGridUserControl2.OptionsView.ShowIndicator = false;
		this.customGridUserControl2.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.pkCardinalityTypeGridColumn.FieldName = "Image";
		this.pkCardinalityTypeGridColumn.MaxWidth = 19;
		this.pkCardinalityTypeGridColumn.MinWidth = 19;
		this.pkCardinalityTypeGridColumn.Name = "pkCardinalityTypeGridColumn";
		this.pkCardinalityTypeGridColumn.Visible = true;
		this.pkCardinalityTypeGridColumn.VisibleIndex = 0;
		this.pkCardinalityTypeGridColumn.Width = 19;
		this.pkCardinalityDisplayNameGridColumn.Caption = "DisplayName";
		this.pkCardinalityDisplayNameGridColumn.FieldName = "DisplayName";
		this.pkCardinalityDisplayNameGridColumn.Name = "pkCardinalityDisplayNameGridColumn";
		this.pkCardinalityDisplayNameGridColumn.Visible = true;
		this.pkCardinalityDisplayNameGridColumn.VisibleIndex = 1;
		this.pkCardinalityDisplayNameGridColumn.Width = 365;
		this.fkCardinalityLookUpEdit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.fkCardinalityLookUpEdit.Location = new System.Drawing.Point(3, 18);
		this.fkCardinalityLookUpEdit.Name = "fkCardinalityLookUpEdit";
		this.fkCardinalityLookUpEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.fkCardinalityLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.fkCardinalityLookUpEdit.Properties.DisplayMember = "displayname";
		this.fkCardinalityLookUpEdit.Properties.ImmediatePopup = true;
		this.fkCardinalityLookUpEdit.Properties.NullText = "";
		this.fkCardinalityLookUpEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
		this.fkCardinalityLookUpEdit.Properties.PopupView = this.customGridUserControl1;
		this.fkCardinalityLookUpEdit.Properties.ShowFooter = false;
		this.fkCardinalityLookUpEdit.Properties.ValueMember = "type";
		this.fkCardinalityLookUpEdit.Size = new System.Drawing.Size(246, 20);
		this.fkCardinalityLookUpEdit.TabIndex = 0;
		this.fkCardinalityLookUpEdit.TabStop = false;
		this.fkCardinalityLookUpEdit.EditValueChanged += new System.EventHandler(fkCardinalityLookUpEdit_EditValueChanged);
		this.customGridUserControl1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.fkCardinalityTypeGridColumn, this.fkCardinalityDisplayNameGridColumn });
		this.customGridUserControl1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.customGridUserControl1.Name = "customGridUserControl1";
		this.customGridUserControl1.OptionsBehavior.AutoPopulateColumns = false;
		this.customGridUserControl1.OptionsCustomization.AllowFilter = false;
		this.customGridUserControl1.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.customGridUserControl1.OptionsView.ShowColumnHeaders = false;
		this.customGridUserControl1.OptionsView.ShowGroupPanel = false;
		this.customGridUserControl1.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.customGridUserControl1.OptionsView.ShowIndicator = false;
		this.customGridUserControl1.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.customGridUserControl1.RowHighlightingIsEnabled = true;
		this.fkCardinalityTypeGridColumn.FieldName = "Image";
		this.fkCardinalityTypeGridColumn.MaxWidth = 19;
		this.fkCardinalityTypeGridColumn.MinWidth = 19;
		this.fkCardinalityTypeGridColumn.Name = "fkCardinalityTypeGridColumn";
		this.fkCardinalityTypeGridColumn.Visible = true;
		this.fkCardinalityTypeGridColumn.VisibleIndex = 0;
		this.fkCardinalityTypeGridColumn.Width = 19;
		this.fkCardinalityDisplayNameGridColumn.Caption = "DisplayName";
		this.fkCardinalityDisplayNameGridColumn.FieldName = "DisplayName";
		this.fkCardinalityDisplayNameGridColumn.Name = "fkCardinalityDisplayNameGridColumn";
		this.fkCardinalityDisplayNameGridColumn.Visible = true;
		this.fkCardinalityDisplayNameGridColumn.VisibleIndex = 1;
		this.fkCardinalityDisplayNameGridColumn.Width = 365;
		this.pkCradinalityLabelControl.Location = new System.Drawing.Point(289, 0);
		this.pkCradinalityLabelControl.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
		this.pkCradinalityLabelControl.Name = "pkCradinalityLabelControl";
		this.pkCradinalityLabelControl.Size = new System.Drawing.Size(65, 13);
		this.pkCradinalityLabelControl.TabIndex = 1;
		this.pkCradinalityLabelControl.Text = "PK Cardinality";
		this.fkCradinalityLabelControl.Location = new System.Drawing.Point(0, 0);
		this.fkCradinalityLabelControl.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
		this.fkCradinalityLabelControl.Name = "fkCradinalityLabelControl";
		this.fkCradinalityLabelControl.Size = new System.Drawing.Size(64, 13);
		this.fkCradinalityLabelControl.TabIndex = 0;
		this.fkCradinalityLabelControl.Text = "FK Cardinality";
		this.pictureEdit2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pictureEdit2.EditValue = Dataedo.App.Properties.Resources.relation_mx_1x_24;
		this.pictureEdit2.Location = new System.Drawing.Point(255, 18);
		this.pictureEdit2.Name = "pictureEdit2";
		this.pictureEdit2.Properties.AllowFocused = false;
		this.pictureEdit2.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.pictureEdit2.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.pictureEdit2.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.pictureEdit2.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.pictureEdit2.Properties.Appearance.Options.UseBackColor = true;
		this.pictureEdit2.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pictureEdit2.Properties.ReadOnly = true;
		this.pictureEdit2.Properties.ShowMenu = false;
		this.pictureEdit2.Properties.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.False;
		this.pictureEdit2.Size = new System.Drawing.Size(31, 21);
		this.pictureEdit2.TabIndex = 4;
		this.tableLayoutPanel2.ColumnCount = 3;
		this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37f));
		this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel2.Controls.Add(this.pkDatabaseLookUpEdit, 2, 1);
		this.tableLayoutPanel2.Controls.Add(this.labelControl1, 0, 0);
		this.tableLayoutPanel2.Controls.Add(this.labelControl2, 2, 0);
		this.tableLayoutPanel2.Controls.Add(this.fkDatabaseLookUpEdit, 0, 1);
		this.tableLayoutPanel2.Location = new System.Drawing.Point(13, 111);
		this.tableLayoutPanel2.Name = "tableLayoutPanel2";
		this.tableLayoutPanel2.RowCount = 2;
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15f));
		this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15f));
		this.tableLayoutPanel2.Size = new System.Drawing.Size(542, 46);
		this.tableLayoutPanel2.TabIndex = 2;
		this.tableLayoutPanel2.TabStop = true;
		this.pkDatabaseLookUpEdit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pkDatabaseLookUpEdit.Location = new System.Drawing.Point(292, 18);
		this.pkDatabaseLookUpEdit.Name = "pkDatabaseLookUpEdit";
		this.pkDatabaseLookUpEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.pkDatabaseLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.pkDatabaseLookUpEdit.Properties.DisplayMember = "Title";
		this.pkDatabaseLookUpEdit.Properties.ImmediatePopup = true;
		this.pkDatabaseLookUpEdit.Properties.NullText = "";
		this.pkDatabaseLookUpEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
		this.pkDatabaseLookUpEdit.Properties.PopupView = this.pkDatabaseGridView;
		this.pkDatabaseLookUpEdit.Properties.ReadOnly = true;
		this.pkDatabaseLookUpEdit.Properties.ShowFooter = false;
		this.pkDatabaseLookUpEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.pkDatabaseLookUpEdit.Properties.ValueMember = "DatabaseId";
		this.pkDatabaseLookUpEdit.Size = new System.Drawing.Size(247, 20);
		this.pkDatabaseLookUpEdit.TabIndex = 0;
		this.pkDatabaseLookUpEdit.TabStop = false;
		this.pkDatabaseLookUpEdit.EditValueChanged += new System.EventHandler(pkDatabaseLookUpEdit_EditValueChanged);
		this.pkDatabaseGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn1, this.gridColumn2 });
		this.pkDatabaseGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.pkDatabaseGridView.Name = "pkDatabaseGridView";
		this.pkDatabaseGridView.OptionsBehavior.AutoPopulateColumns = false;
		this.pkDatabaseGridView.OptionsCustomization.AllowFilter = false;
		this.pkDatabaseGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.pkDatabaseGridView.OptionsView.ShowColumnHeaders = false;
		this.pkDatabaseGridView.OptionsView.ShowGroupPanel = false;
		this.pkDatabaseGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.pkDatabaseGridView.OptionsView.ShowIndicator = false;
		this.pkDatabaseGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.pkDatabaseGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(gridLookUpEditView_CustomDrawCell);
		this.gridColumn1.FieldName = "ObjectType";
		this.gridColumn1.MaxWidth = 19;
		this.gridColumn1.MinWidth = 19;
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 19;
		this.gridColumn2.Caption = "Object name";
		this.gridColumn2.FieldName = "Title";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 365;
		this.labelControl1.Location = new System.Drawing.Point(0, 0);
		this.labelControl1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(62, 13);
		this.labelControl1.TabIndex = 0;
		this.labelControl1.Text = "FK Database";
		this.labelControl2.Location = new System.Drawing.Point(292, 0);
		this.labelControl2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(63, 13);
		this.labelControl2.TabIndex = 1;
		this.labelControl2.Text = "PK Database";
		this.fkDatabaseLookUpEdit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.fkDatabaseLookUpEdit.Location = new System.Drawing.Point(3, 18);
		this.fkDatabaseLookUpEdit.Name = "fkDatabaseLookUpEdit";
		this.fkDatabaseLookUpEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.fkDatabaseLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.fkDatabaseLookUpEdit.Properties.DisplayMember = "Title";
		this.fkDatabaseLookUpEdit.Properties.ImmediatePopup = true;
		this.fkDatabaseLookUpEdit.Properties.NullText = "";
		this.fkDatabaseLookUpEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
		this.fkDatabaseLookUpEdit.Properties.PopupView = this.fkDatabaseGridView;
		this.fkDatabaseLookUpEdit.Properties.ReadOnly = true;
		this.fkDatabaseLookUpEdit.Properties.ShowFooter = false;
		this.fkDatabaseLookUpEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.fkDatabaseLookUpEdit.Properties.ValueMember = "DatabaseId";
		this.fkDatabaseLookUpEdit.Size = new System.Drawing.Size(246, 20);
		this.fkDatabaseLookUpEdit.TabIndex = 0;
		this.fkDatabaseLookUpEdit.TabStop = false;
		this.fkDatabaseLookUpEdit.EditValueChanged += new System.EventHandler(fkDatabaseLookUpEdit_EditValueChanged);
		this.fkDatabaseGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn3, this.gridColumn4 });
		this.fkDatabaseGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.fkDatabaseGridView.Name = "fkDatabaseGridView";
		this.fkDatabaseGridView.OptionsBehavior.AutoPopulateColumns = false;
		this.fkDatabaseGridView.OptionsCustomization.AllowFilter = false;
		this.fkDatabaseGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.fkDatabaseGridView.OptionsView.ShowColumnHeaders = false;
		this.fkDatabaseGridView.OptionsView.ShowGroupPanel = false;
		this.fkDatabaseGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.fkDatabaseGridView.OptionsView.ShowIndicator = false;
		this.fkDatabaseGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.fkDatabaseGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(gridLookUpEditView_CustomDrawCell);
		this.gridColumn3.FieldName = "ObjectType";
		this.gridColumn3.MaxWidth = 19;
		this.gridColumn3.MinWidth = 19;
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 0;
		this.gridColumn3.Width = 19;
		this.gridColumn4.Caption = "Object name";
		this.gridColumn4.FieldName = "Title";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 1;
		this.gridColumn4.Width = 365;
		this.showjoinConditionCheckEdit.Location = new System.Drawing.Point(13, 565);
		this.showjoinConditionCheckEdit.Name = "showjoinConditionCheckEdit";
		this.showjoinConditionCheckEdit.Properties.Caption = "Show join condition";
		this.showjoinConditionCheckEdit.Size = new System.Drawing.Size(542, 20);
		this.showjoinConditionCheckEdit.StyleController = this.layoutControl;
		this.showjoinConditionCheckEdit.TabIndex = 7;
		this.showjoinConditionCheckEdit.CheckedChanged += new System.EventHandler(FieldEditValueChanged);
		this.titleTextEdit.Location = new System.Drawing.Point(13, 78);
		this.titleTextEdit.Name = "titleTextEdit";
		this.titleTextEdit.Size = new System.Drawing.Size(542, 20);
		this.titleTextEdit.StyleController = this.layoutControl;
		this.titleTextEdit.TabIndex = 1;
		this.titleTextEdit.EditValueChanged += new System.EventHandler(FieldEditValueChanged);
		this.linkStyleRadioGroup.Location = new System.Drawing.Point(14, 631);
		this.linkStyleRadioGroup.Name = "linkStyleRadioGroup";
		this.linkStyleRadioGroup.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.linkStyleRadioGroup.Properties.Appearance.Options.UseBackColor = true;
		this.linkStyleRadioGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.linkStyleRadioGroup.Size = new System.Drawing.Size(165, 21);
		this.linkStyleRadioGroup.StyleController = this.layoutControl;
		this.linkStyleRadioGroup.TabIndex = 9;
		this.linkStyleRadioGroup.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(linkStyleRadioGroup_EditValueChanging);
		this.hideRelationCheckEdit.AutoSizeInLayoutControl = true;
		this.hideRelationCheckEdit.Location = new System.Drawing.Point(13, 589);
		this.hideRelationCheckEdit.Name = "hideRelationCheckEdit";
		this.hideRelationCheckEdit.Properties.Caption = "Hide relationship";
		this.hideRelationCheckEdit.Size = new System.Drawing.Size(82, 20);
		this.hideRelationCheckEdit.StyleController = this.layoutControl;
		this.hideRelationCheckEdit.TabIndex = 8;
		this.hideRelationCheckEdit.CheckedChanged += new System.EventHandler(FieldEditValueChanged);
		this.showTitleCheckEdit.AutoSizeInLayoutControl = true;
		this.showTitleCheckEdit.Location = new System.Drawing.Point(13, 541);
		this.showTitleCheckEdit.Name = "showTitleCheckEdit";
		this.showTitleCheckEdit.Properties.Caption = "Show title";
		this.showTitleCheckEdit.Size = new System.Drawing.Size(69, 20);
		this.showTitleCheckEdit.StyleController = this.layoutControl;
		this.showTitleCheckEdit.TabIndex = 6;
		this.showTitleCheckEdit.CheckedChanged += new System.EventHandler(FieldEditValueChanged);
		this.tableLayoutPanel1.ColumnCount = 3;
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37f));
		this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50f));
		this.tableLayoutPanel1.Controls.Add(this.pkTableLookUpEdit, 2, 1);
		this.tableLayoutPanel1.Controls.Add(this.fkTableLabelControl, 0, 0);
		this.tableLayoutPanel1.Controls.Add(this.pkTableLabelControl, 2, 0);
		this.tableLayoutPanel1.Controls.Add(this.fkTableLookUpEdit, 0, 1);
		this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 161);
		this.tableLayoutPanel1.Name = "tableLayoutPanel1";
		this.tableLayoutPanel1.RowCount = 2;
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15f));
		this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15f));
		this.tableLayoutPanel1.Size = new System.Drawing.Size(542, 42);
		this.tableLayoutPanel1.TabIndex = 3;
		this.tableLayoutPanel1.TabStop = true;
		this.pkTableLookUpEdit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pkTableLookUpEdit.Location = new System.Drawing.Point(292, 18);
		this.pkTableLookUpEdit.Name = "pkTableLookUpEdit";
		this.pkTableLookUpEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.pkTableLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.pkTableLookUpEdit.Properties.DisplayMember = "NameWithTitle";
		this.pkTableLookUpEdit.Properties.ImmediatePopup = true;
		this.pkTableLookUpEdit.Properties.NullText = "";
		this.pkTableLookUpEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
		this.pkTableLookUpEdit.Properties.PopupView = this.pkTableLookUpEditView;
		this.pkTableLookUpEdit.Properties.ReadOnly = true;
		this.pkTableLookUpEdit.Properties.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSearch;
		this.pkTableLookUpEdit.Properties.ShowFooter = false;
		this.pkTableLookUpEdit.Properties.ValueMember = "Id";
		this.pkTableLookUpEdit.Size = new System.Drawing.Size(247, 20);
		this.pkTableLookUpEdit.TabIndex = 0;
		this.pkTableLookUpEdit.TabStop = false;
		this.pkTableLookUpEdit.EditValueChanged += new System.EventHandler(pkTableLookUpEdit_EditValueChanged);
		this.pkTableLookUpEditView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.pkTypeGridColumn, this.pkObjectNameGridColumn });
		this.pkTableLookUpEditView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.pkTableLookUpEditView.Name = "pkTableLookUpEditView";
		this.pkTableLookUpEditView.OptionsBehavior.AutoPopulateColumns = false;
		this.pkTableLookUpEditView.OptionsCustomization.AllowFilter = false;
		this.pkTableLookUpEditView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.pkTableLookUpEditView.OptionsView.ShowColumnHeaders = false;
		this.pkTableLookUpEditView.OptionsView.ShowGroupPanel = false;
		this.pkTableLookUpEditView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.pkTableLookUpEditView.OptionsView.ShowIndicator = false;
		this.pkTableLookUpEditView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.pkTableLookUpEditView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(gridLookUpEditView_CustomDrawCell);
		this.pkTypeGridColumn.FieldName = "ObjectType";
		this.pkTypeGridColumn.MaxWidth = 19;
		this.pkTypeGridColumn.MinWidth = 19;
		this.pkTypeGridColumn.Name = "pkTypeGridColumn";
		this.pkTypeGridColumn.Visible = true;
		this.pkTypeGridColumn.VisibleIndex = 0;
		this.pkTypeGridColumn.Width = 19;
		this.pkObjectNameGridColumn.Caption = "Object name";
		this.pkObjectNameGridColumn.FieldName = "NameWithTitle";
		this.pkObjectNameGridColumn.Name = "pkObjectNameGridColumn";
		this.pkObjectNameGridColumn.Visible = true;
		this.pkObjectNameGridColumn.VisibleIndex = 1;
		this.pkObjectNameGridColumn.Width = 365;
		this.fkTableLabelControl.Location = new System.Drawing.Point(0, 0);
		this.fkTableLabelControl.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
		this.fkTableLabelControl.Name = "fkTableLabelControl";
		this.fkTableLabelControl.Size = new System.Drawing.Size(43, 13);
		this.fkTableLabelControl.TabIndex = 0;
		this.fkTableLabelControl.Text = "FK Table";
		this.pkTableLabelControl.Location = new System.Drawing.Point(292, 0);
		this.pkTableLabelControl.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
		this.pkTableLabelControl.Name = "pkTableLabelControl";
		this.pkTableLabelControl.Size = new System.Drawing.Size(44, 13);
		this.pkTableLabelControl.TabIndex = 1;
		this.pkTableLabelControl.Text = "PK Table";
		this.fkTableLookUpEdit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.fkTableLookUpEdit.Location = new System.Drawing.Point(3, 18);
		this.fkTableLookUpEdit.Name = "fkTableLookUpEdit";
		this.fkTableLookUpEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.fkTableLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.fkTableLookUpEdit.Properties.DisplayMember = "NameWithTitle";
		this.fkTableLookUpEdit.Properties.ImmediatePopup = true;
		this.fkTableLookUpEdit.Properties.NullText = "";
		this.fkTableLookUpEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
		this.fkTableLookUpEdit.Properties.PopupView = this.fkTableLookUpEditView;
		this.fkTableLookUpEdit.Properties.ReadOnly = true;
		this.fkTableLookUpEdit.Properties.ShowFooter = false;
		this.fkTableLookUpEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.fkTableLookUpEdit.Properties.ValueMember = "Id";
		this.fkTableLookUpEdit.Size = new System.Drawing.Size(246, 20);
		this.fkTableLookUpEdit.TabIndex = 0;
		this.fkTableLookUpEdit.TabStop = false;
		this.fkTableLookUpEdit.EditValueChanged += new System.EventHandler(fkTableLookUpEdit_EditValueChanged);
		this.fkTableLookUpEditView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.fkTypeGridColumn, this.fkObjectNameGridColumn });
		this.fkTableLookUpEditView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.fkTableLookUpEditView.Name = "fkTableLookUpEditView";
		this.fkTableLookUpEditView.OptionsBehavior.AutoPopulateColumns = false;
		this.fkTableLookUpEditView.OptionsCustomization.AllowFilter = false;
		this.fkTableLookUpEditView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.fkTableLookUpEditView.OptionsView.ShowColumnHeaders = false;
		this.fkTableLookUpEditView.OptionsView.ShowGroupPanel = false;
		this.fkTableLookUpEditView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.fkTableLookUpEditView.OptionsView.ShowIndicator = false;
		this.fkTableLookUpEditView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.fkTableLookUpEditView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(gridLookUpEditView_CustomDrawCell);
		this.fkTypeGridColumn.FieldName = "ObjectType";
		this.fkTypeGridColumn.MaxWidth = 19;
		this.fkTypeGridColumn.MinWidth = 19;
		this.fkTypeGridColumn.Name = "fkTypeGridColumn";
		this.fkTypeGridColumn.Visible = true;
		this.fkTypeGridColumn.VisibleIndex = 0;
		this.fkTypeGridColumn.Width = 19;
		this.fkObjectNameGridColumn.Caption = "Object name";
		this.fkObjectNameGridColumn.FieldName = "NameWithTitle";
		this.fkObjectNameGridColumn.Name = "fkObjectNameGridColumn";
		this.fkObjectNameGridColumn.Visible = true;
		this.fkObjectNameGridColumn.VisibleIndex = 1;
		this.fkObjectNameGridColumn.Width = 365;
		this.tableRelationsColumnsGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.tableRelationsColumnsGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.tableRelationsColumnsGrid.Location = new System.Drawing.Point(13, 269);
		this.tableRelationsColumnsGrid.MainView = this.tableRelationsColumnsGridView;
		this.tableRelationsColumnsGrid.Name = "tableRelationsColumnsGrid";
		this.tableRelationsColumnsGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit, this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit, this.tableRelationFkColumnKeyRepositoryItemPictureEdit, this.tableRelationPkColumnKeyRepositoryItemPictureEdit });
		this.tableRelationsColumnsGrid.Size = new System.Drawing.Size(542, 153);
		this.tableRelationsColumnsGrid.TabIndex = 4;
		this.tableRelationsColumnsGrid.ToolTipController = this.toolTipController;
		this.tableRelationsColumnsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tableRelationsColumnsGridView });
		this.tableRelationsColumnsGrid.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(tableRelationsColumnsGrid_ProcessGridKey);
		this.tableRelationsColumnsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.tableRelationFkColumnKeyGridColumn, this.tableRelationFkColumnGridColumn, this.tableRelationPkColumnKeyGridColumn, this.tableRelationPkColumnGridColumn });
		this.tableRelationsColumnsGridView.GridControl = this.tableRelationsColumnsGrid;
		this.tableRelationsColumnsGridView.Name = "tableRelationsColumnsGridView";
		this.tableRelationsColumnsGridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationsColumnsGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.tableRelationsColumnsGridView.OptionsBehavior.FocusLeaveOnTab = true;
		this.tableRelationsColumnsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationsColumnsGridView.OptionsCustomization.AllowFilter = false;
		this.tableRelationsColumnsGridView.OptionsCustomization.AllowSort = false;
		this.tableRelationsColumnsGridView.OptionsDetail.EnableMasterViewMode = false;
		this.tableRelationsColumnsGridView.OptionsSelection.MultiSelect = true;
		this.tableRelationsColumnsGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.tableRelationsColumnsGridView.OptionsView.ShowGroupPanel = false;
		this.tableRelationsColumnsGridView.OptionsView.ShowIndicator = false;
		this.tableRelationsColumnsGridView.RowHighlightingIsEnabled = true;
		this.tableRelationsColumnsGridView.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(tableRelationsColumnsGridView_RowCellStyle);
		this.tableRelationsColumnsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(tableRelationsColumnsGridView_PopupMenuShowing);
		this.tableRelationsColumnsGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(tableRelationsColumnsGridView_CellValueChanging);
		this.tableRelationFkColumnKeyGridColumn.ColumnEdit = this.tableRelationFkColumnKeyRepositoryItemPictureEdit;
		this.tableRelationFkColumnKeyGridColumn.FieldName = "ColumnFkIcon";
		this.tableRelationFkColumnKeyGridColumn.MaxWidth = 25;
		this.tableRelationFkColumnKeyGridColumn.MinWidth = 25;
		this.tableRelationFkColumnKeyGridColumn.Name = "tableRelationFkColumnKeyGridColumn";
		this.tableRelationFkColumnKeyGridColumn.OptionsColumn.AllowFocus = false;
		this.tableRelationFkColumnKeyGridColumn.OptionsColumn.ReadOnly = true;
		this.tableRelationFkColumnKeyGridColumn.OptionsColumn.ShowCaption = false;
		this.tableRelationFkColumnKeyGridColumn.Visible = true;
		this.tableRelationFkColumnKeyGridColumn.VisibleIndex = 0;
		this.tableRelationFkColumnKeyGridColumn.Width = 25;
		this.tableRelationFkColumnKeyRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationFkColumnKeyRepositoryItemPictureEdit.AllowScrollViaMouseDrag = true;
		this.tableRelationFkColumnKeyRepositoryItemPictureEdit.Name = "tableRelationFkColumnKeyRepositoryItemPictureEdit";
		this.tableRelationFkColumnKeyRepositoryItemPictureEdit.ReadOnly = true;
		this.tableRelationFkColumnKeyRepositoryItemPictureEdit.ShowMenu = false;
		this.tableRelationFkColumnGridColumn.Caption = "FK Column";
		this.tableRelationFkColumnGridColumn.ColumnEdit = this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit;
		this.tableRelationFkColumnGridColumn.FieldName = "ColumnFkId";
		this.tableRelationFkColumnGridColumn.Name = "tableRelationFkColumnGridColumn";
		this.tableRelationFkColumnGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationFkColumnGridColumn.Visible = true;
		this.tableRelationFkColumnGridColumn.VisibleIndex = 1;
		this.tableRelationFkColumnGridColumn.Width = 230;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.DisplayMember = "FullNameWithTitle";
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.Name = "tablesRelationsFkColumnsRepositoryItemGridLookUpEdit";
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.NullText = "";
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.PopupView = this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.tablesRelationsFkColumnsKeyRepositoryItemPictureEdit, this.fkColumnFullNameFormattedRepositoryItemCustomTextEdit });
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.None;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.ShowFooter = false;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.ValueMember = "Id";
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit.EditValueChanged += new System.EventHandler(tablesRelationsFkColumnsRepositoryItemGridLookUpEdit_EditValueChanged);
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.tablesRelationsFkColumnsKeyGridColumn, this.tablesRelationsFkColumnsNameGridColumn });
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.Name = "tablesRelationsFkColumnsRepositoryItemGridLookUpEditView";
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.OptionsBehavior.Editable = false;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.OptionsBehavior.ReadOnly = true;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.OptionsCustomization.AllowColumnMoving = false;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.OptionsCustomization.AllowColumnResizing = false;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.OptionsView.ShowColumnHeaders = false;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.OptionsView.ShowGroupPanel = false;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.OptionsView.ShowIndicator = false;
		this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.tablesRelationsFkColumnsKeyGridColumn.ColumnEdit = this.tablesRelationsFkColumnsKeyRepositoryItemPictureEdit;
		this.tablesRelationsFkColumnsKeyGridColumn.FieldName = "UniqueConstraintOrFkIcon";
		this.tablesRelationsFkColumnsKeyGridColumn.MinWidth = 16;
		this.tablesRelationsFkColumnsKeyGridColumn.Name = "tablesRelationsFkColumnsKeyGridColumn";
		this.tablesRelationsFkColumnsKeyGridColumn.Visible = true;
		this.tablesRelationsFkColumnsKeyGridColumn.VisibleIndex = 0;
		this.tablesRelationsFkColumnsKeyGridColumn.Width = 16;
		this.tablesRelationsFkColumnsKeyRepositoryItemPictureEdit.Name = "tablesRelationsFkColumnsKeyRepositoryItemPictureEdit";
		this.tablesRelationsFkColumnsNameGridColumn.ColumnEdit = this.fkColumnFullNameFormattedRepositoryItemCustomTextEdit;
		this.tablesRelationsFkColumnsNameGridColumn.FieldName = "FullNameFormattedWithTitle";
		this.tablesRelationsFkColumnsNameGridColumn.Name = "tablesRelationsFkColumnsNameGridColumn";
		this.tablesRelationsFkColumnsNameGridColumn.Visible = true;
		this.tablesRelationsFkColumnsNameGridColumn.VisibleIndex = 1;
		this.tablesRelationsFkColumnsNameGridColumn.Width = 384;
		this.fkColumnFullNameFormattedRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.fkColumnFullNameFormattedRepositoryItemCustomTextEdit.AutoHeight = false;
		this.fkColumnFullNameFormattedRepositoryItemCustomTextEdit.Name = "fkColumnFullNameFormattedRepositoryItemCustomTextEdit";
		this.tableRelationPkColumnKeyGridColumn.ColumnEdit = this.tableRelationPkColumnKeyRepositoryItemPictureEdit;
		this.tableRelationPkColumnKeyGridColumn.FieldName = "ColumnPkIcon";
		this.tableRelationPkColumnKeyGridColumn.MaxWidth = 25;
		this.tableRelationPkColumnKeyGridColumn.MinWidth = 25;
		this.tableRelationPkColumnKeyGridColumn.Name = "tableRelationPkColumnKeyGridColumn";
		this.tableRelationPkColumnKeyGridColumn.OptionsColumn.AllowFocus = false;
		this.tableRelationPkColumnKeyGridColumn.OptionsColumn.ReadOnly = true;
		this.tableRelationPkColumnKeyGridColumn.OptionsColumn.ShowCaption = false;
		this.tableRelationPkColumnKeyGridColumn.Visible = true;
		this.tableRelationPkColumnKeyGridColumn.VisibleIndex = 2;
		this.tableRelationPkColumnKeyGridColumn.Width = 25;
		this.tableRelationPkColumnKeyRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationPkColumnKeyRepositoryItemPictureEdit.AllowScrollViaMouseDrag = true;
		this.tableRelationPkColumnKeyRepositoryItemPictureEdit.Name = "tableRelationPkColumnKeyRepositoryItemPictureEdit";
		this.tableRelationPkColumnKeyRepositoryItemPictureEdit.ReadOnly = true;
		this.tableRelationPkColumnKeyRepositoryItemPictureEdit.ShowMenu = false;
		this.tableRelationPkColumnGridColumn.Caption = "PK Column";
		this.tableRelationPkColumnGridColumn.ColumnEdit = this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit;
		this.tableRelationPkColumnGridColumn.FieldName = "ColumnPkId";
		this.tableRelationPkColumnGridColumn.Name = "tableRelationPkColumnGridColumn";
		this.tableRelationPkColumnGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationPkColumnGridColumn.Visible = true;
		this.tableRelationPkColumnGridColumn.VisibleIndex = 3;
		this.tableRelationPkColumnGridColumn.Width = 278;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.DisplayMember = "FullNameWithTitle";
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.Name = "tablesRelationsPkColumnsRepositoryItemGridLookUpEdit";
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.NullText = "";
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.PopupView = this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.tablesRelationsPkColumnsKeyRepositoryItemPictureEdit, this.pkColumnFullNameFormattedRepositoryItemCustomTextEdit });
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.None;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.ShowFooter = false;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.ValueMember = "Id";
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit.EditValueChanged += new System.EventHandler(tablesRelationsPkColumnsRepositoryItemGridLookUpEdit_EditValueChanged);
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.tablesRelationsPkColumnsKeyGridColumn, this.tablesRelationsPkColumnsNameGridColumn });
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.Name = "tablesRelationsPkColumnsRepositoryItemGridLookUpEditView";
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.OptionsBehavior.Editable = false;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.OptionsBehavior.ReadOnly = true;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.OptionsCustomization.AllowColumnMoving = false;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.OptionsCustomization.AllowColumnResizing = false;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.OptionsView.ShowColumnHeaders = false;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.OptionsView.ShowGroupPanel = false;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.OptionsView.ShowIndicator = false;
		this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.tablesRelationsPkColumnsKeyGridColumn.ColumnEdit = this.tablesRelationsPkColumnsKeyRepositoryItemPictureEdit;
		this.tablesRelationsPkColumnsKeyGridColumn.FieldName = "UniqueConstraintOrFkIcon";
		this.tablesRelationsPkColumnsKeyGridColumn.Name = "tablesRelationsPkColumnsKeyGridColumn";
		this.tablesRelationsPkColumnsKeyGridColumn.Visible = true;
		this.tablesRelationsPkColumnsKeyGridColumn.VisibleIndex = 0;
		this.tablesRelationsPkColumnsKeyGridColumn.Width = 20;
		this.tablesRelationsPkColumnsKeyRepositoryItemPictureEdit.Name = "tablesRelationsPkColumnsKeyRepositoryItemPictureEdit";
		this.tablesRelationsPkColumnsNameGridColumn.ColumnEdit = this.pkColumnFullNameFormattedRepositoryItemCustomTextEdit;
		this.tablesRelationsPkColumnsNameGridColumn.FieldName = "FullNameFormattedWithTitle";
		this.tablesRelationsPkColumnsNameGridColumn.Name = "tablesRelationsPkColumnsNameGridColumn";
		this.tablesRelationsPkColumnsNameGridColumn.Visible = true;
		this.tablesRelationsPkColumnsNameGridColumn.VisibleIndex = 1;
		this.tablesRelationsPkColumnsNameGridColumn.Width = 380;
		this.pkColumnFullNameFormattedRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.pkColumnFullNameFormattedRepositoryItemCustomTextEdit.AutoHeight = false;
		this.pkColumnFullNameFormattedRepositoryItemCustomTextEdit.Name = "pkColumnFullNameFormattedRepositoryItemCustomTextEdit";
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.closeSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.closeSimpleButton.Location = new System.Drawing.Point(475, 656);
		this.closeSimpleButton.Name = "closeSimpleButton";
		this.closeSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.closeSimpleButton.StyleController = this.layoutControl;
		this.closeSimpleButton.TabIndex = 11;
		this.closeSimpleButton.Text = "Cancel";
		this.closeSimpleButton.Click += new System.EventHandler(closeSimpleButton_Click);
		this.okSimpleButton.Location = new System.Drawing.Point(379, 656);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.okSimpleButton.StyleController = this.layoutControl;
		this.okSimpleButton.TabIndex = 10;
		this.okSimpleButton.Text = "Save";
		this.okSimpleButton.Click += new System.EventHandler(okSimpleButton_Click);
		this.nameTextEdit.Location = new System.Drawing.Point(13, 29);
		this.nameTextEdit.Name = "nameTextEdit";
		this.nameTextEdit.Size = new System.Drawing.Size(542, 20);
		this.nameTextEdit.StyleController = this.layoutControl;
		this.nameTextEdit.TabIndex = 0;
		this.nameTextEdit.EditValueChanged += new System.EventHandler(FieldEditValueChanged);
		this.descriptionMemoEdit.Location = new System.Drawing.Point(13, 451);
		this.descriptionMemoEdit.Name = "descriptionMemoEdit";
		this.descriptionMemoEdit.Size = new System.Drawing.Size(542, 77);
		this.descriptionMemoEdit.StyleController = this.layoutControl;
		this.descriptionMemoEdit.TabIndex = 5;
		this.descriptionMemoEdit.EditValueChanged += new System.EventHandler(FieldEditValueChanged);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[16]
		{
			this.layoutControlItem1, this.layoutControlItem2, this.layoutControlItem3, this.emptySpaceItem1, this.emptySpaceItem2, this.layoutControlItem4, this.descriptionLayoutControlItem, this.showLabelLayoutControlItem, this.hideRelationLayoutControlItem, this.linkStyleLayoutControlItem,
			this.emptySpaceItem3, this.titleLayoutControlItem, this.showJoinConditionLayoutControlItem, this.layoutControlItem6, this.layoutControlItem5, this.layoutControlItem7
		});
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(11, 11, 11, 11);
		this.layoutControlGroup1.Size = new System.Drawing.Size(568, 691);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.nameTextEdit;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 49);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(54, 49);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 11);
		this.layoutControlItem1.Size = new System.Drawing.Size(546, 49);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.Text = "Name:";
		this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(69, 13);
		this.layoutControlItem2.Control = this.okSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(366, 643);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.closeSimpleButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(462, 643);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 643);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 26);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(366, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(450, 643);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(12, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(12, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(12, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.Control = this.tableRelationsColumnsGrid;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 240);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 11);
		this.layoutControlItem4.Size = new System.Drawing.Size(546, 182);
		this.layoutControlItem4.Text = "Columns:";
		this.layoutControlItem4.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(69, 13);
		this.descriptionLayoutControlItem.Control = this.descriptionMemoEdit;
		this.descriptionLayoutControlItem.CustomizationFormText = "Description";
		this.descriptionLayoutControlItem.Location = new System.Drawing.Point(0, 422);
		this.descriptionLayoutControlItem.MinSize = new System.Drawing.Size(61, 45);
		this.descriptionLayoutControlItem.Name = "descriptionLayoutControlItem";
		this.descriptionLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 11);
		this.descriptionLayoutControlItem.Size = new System.Drawing.Size(546, 106);
		this.descriptionLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.descriptionLayoutControlItem.Text = "Description:";
		this.descriptionLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.descriptionLayoutControlItem.TextSize = new System.Drawing.Size(69, 13);
		this.showLabelLayoutControlItem.Control = this.showTitleCheckEdit;
		this.showLabelLayoutControlItem.Location = new System.Drawing.Point(0, 528);
		this.showLabelLayoutControlItem.Name = "showLabelLayoutControlItem";
		this.showLabelLayoutControlItem.Size = new System.Drawing.Size(546, 24);
		this.showLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.showLabelLayoutControlItem.TextVisible = false;
		this.showLabelLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.hideRelationLayoutControlItem.Control = this.hideRelationCheckEdit;
		this.hideRelationLayoutControlItem.Location = new System.Drawing.Point(0, 576);
		this.hideRelationLayoutControlItem.Name = "hideRelationLayoutControlItem";
		this.hideRelationLayoutControlItem.Size = new System.Drawing.Size(546, 24);
		this.hideRelationLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.hideRelationLayoutControlItem.TextVisible = false;
		this.linkStyleLayoutControlItem.Control = this.linkStyleRadioGroup;
		this.linkStyleLayoutControlItem.Location = new System.Drawing.Point(0, 600);
		this.linkStyleLayoutControlItem.MaxSize = new System.Drawing.Size(170, 43);
		this.linkStyleLayoutControlItem.MinSize = new System.Drawing.Size(170, 43);
		this.linkStyleLayoutControlItem.Name = "linkStyleLayoutControlItem";
		this.linkStyleLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 2, 4, 2);
		this.linkStyleLayoutControlItem.Size = new System.Drawing.Size(170, 43);
		this.linkStyleLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.linkStyleLayoutControlItem.Text = "Link style";
		this.linkStyleLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.linkStyleLayoutControlItem.TextSize = new System.Drawing.Size(69, 13);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(170, 600);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(0, 43);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(1, 43);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(376, 43);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.titleLayoutControlItem.Control = this.titleTextEdit;
		this.titleLayoutControlItem.Location = new System.Drawing.Point(0, 49);
		this.titleLayoutControlItem.Name = "titleLayoutControlItem";
		this.titleLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 11);
		this.titleLayoutControlItem.Size = new System.Drawing.Size(546, 49);
		this.titleLayoutControlItem.Text = "Title: (optional)";
		this.titleLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.titleLayoutControlItem.TextSize = new System.Drawing.Size(69, 13);
		this.showJoinConditionLayoutControlItem.Control = this.showjoinConditionCheckEdit;
		this.showJoinConditionLayoutControlItem.Location = new System.Drawing.Point(0, 552);
		this.showJoinConditionLayoutControlItem.Name = "showJoinConditionLayoutControlItem";
		this.showJoinConditionLayoutControlItem.Size = new System.Drawing.Size(546, 24);
		this.showJoinConditionLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.showJoinConditionLayoutControlItem.TextVisible = false;
		this.layoutControlItem6.Control = this.tableLayoutPanel2;
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 98);
		this.layoutControlItem6.MaxSize = new System.Drawing.Size(0, 50);
		this.layoutControlItem6.MinSize = new System.Drawing.Size(104, 50);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(546, 50);
		this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.layoutControlItem5.Control = this.tableLayoutPanel1;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 148);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 46);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(104, 46);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(546, 46);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layoutControlItem7.Control = this.tableLayoutPanel3;
		this.layoutControlItem7.ImageOptions.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 194);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(0, 46);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(546, 46);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(546, 46);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.gridPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[1]
		{
			new DevExpress.XtraBars.LinkPersistInfo(barButtonItem)
		});
		this.gridPopupMenu.Manager = this.gridBarManager;
		this.gridPopupMenu.Name = "gridPopupMenu";
		this.gridPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(gridPopupMenu_BeforePopup);
		this.gridBarManager.DockControls.Add(this.barDockControlTop);
		this.gridBarManager.DockControls.Add(this.barDockControlBottom);
		this.gridBarManager.DockControls.Add(this.barDockControlLeft);
		this.gridBarManager.DockControls.Add(this.barDockControlRight);
		this.gridBarManager.Form = this;
		this.gridBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[1] { barButtonItem });
		this.gridBarManager.MaxItemId = 1;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.gridBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(568, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 691);
		this.barDockControlBottom.Manager = this.gridBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(568, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.gridBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 691);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(568, 0);
		this.barDockControlRight.Manager = this.gridBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 691);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(568, 691);
		base.Controls.Add(this.layoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("RelationForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		this.MinimumSize = new System.Drawing.Size(568, 32);
		base.Name = "RelationForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Relationship";
		base.Load += new System.EventHandler(ErdNewRelationForm_Load);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		this.tableLayoutPanel3.ResumeLayout(false);
		this.tableLayoutPanel3.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pkCardinalityLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customGridUserControl2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fkCardinalityLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customGridUserControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit2.Properties).EndInit();
		this.tableLayoutPanel2.ResumeLayout(false);
		this.tableLayoutPanel2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pkDatabaseLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pkDatabaseGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fkDatabaseLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fkDatabaseGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.showjoinConditionCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linkStyleRadioGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hideRelationCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.showTitleCheckEdit.Properties).EndInit();
		this.tableLayoutPanel1.ResumeLayout(false);
		this.tableLayoutPanel1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.pkTableLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pkTableLookUpEditView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fkTableLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fkTableLookUpEditView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationsColumnsGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationsColumnsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationFkColumnKeyRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsFkColumnsRepositoryItemGridLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsFkColumnsRepositoryItemGridLookUpEditView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsFkColumnsKeyRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fkColumnFullNameFormattedRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationPkColumnKeyRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsPkColumnsRepositoryItemGridLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsPkColumnsRepositoryItemGridLookUpEditView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tablesRelationsPkColumnsKeyRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pkColumnFullNameFormattedRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionMemoEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.showLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hideRelationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linkStyleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.showJoinConditionLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridBarManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
