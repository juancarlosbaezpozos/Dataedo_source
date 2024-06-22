using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.CustomMessageBox;
using Dataedo.Data.Base.Commands.Parameters.Delegates;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.DataProcessing.Synchronize.Classes;
using Dataedo.Model.Data.CustomFields;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraWaitForm;

namespace Dataedo.App.Forms;

public class CustomFieldsSummaryForm : BaseXtraForm
{
	private bool isClosingEnabled = true;

	private List<CustomFieldRow> fieldsToRemove;

	private BindingList<CustomFieldRow> customFields;

	private IEnumerable<CustomFieldRow> dbCustomFields;

	private bool isProcessing;

	private bool isCancelled;

	private double? lastProgressValue;

	private string lastStepMessage = string.Empty;

	private bool showAllSuggestedCustomFields;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private SimpleButton cancelSimpleButton;

	private EmptySpaceItem emptySpaceItem1;

	private LayoutControlItem layoutControlItem3;

	private GridControl gridControl;

	private GridColumn titleGridColumn;

	private GridColumn tableGridColumn;

	private GridColumn procedureGridColumn;

	private GridColumn columnGridColumn;

	private GridColumn relationGridColumn;

	private GridColumn keyGridColumn;

	private GridColumn parameterGridColumn;

	private GridColumn moduleGridColumn;

	private GridColumn documentationGridColumn;

	private LayoutControlItem layoutControlItem4;

	private GridColumn descriptionGridColumn;

	private SimpleButton okSimpleButton;

	private LayoutControlItem layoutControlItem5;

	private EmptySpaceItem emptySpaceItem3;

	private FlowLayoutPanel suggestedFieldsFlowLayoutPanel;

	private LayoutControlItem suggestedFieldsLayoutControlItem;

	private GridColumn triggerGridColumn;

	private RepositoryItemCheckEdit repositoryItemCheckEdit;

	private InfoUserControl infoUserControl;

	private LayoutControlItem infoUserControlLayoutControlItem;

	private CustomGridUserControl gridView;

	private GridColumn typeGridColumn;

	private RibbonControl ribbonControl;

	private BarButtonItem barButtonItem1;

	private BarButtonItem addBarButtonItem;

	private BarButtonItem removeBarButtonItem;

	private BarButtonItem moveUpBarButtonItem;

	private BarButtonItem moveDownBarButtonItem;

	private BarButtonItem editBarButtonItem;

	private RibbonPage ribbonPage;

	private RibbonPageGroup manipulateRibbonPageGroup;

	private RibbonPageGroup sortRibbonPageGroup;

	private LayoutControlItem layoutControlItem10;

	private EmptySpaceItem botoomMarginEmptySpaceItem;

	private Panel progressPanelPanel;

	private PanelControl progressPanelControl;

	private ProgressPanel progressPanel;

	private NonCustomizableLayoutControl progressLayoutControl;

	private LabelControl progressLabelControl;

	private LayoutControlGroup progressLayoutControlGroup;

	private LayoutControlItem progressLayoutControlItem;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem rightEmptySpaceItem;

	private LinkUserControl trialUpgradeLinkUserControl;

	private LayoutControlItem trialUpgradeLayoutControlItem;

	private GridColumn termGridColumn;

	private GridColumn classGridColumn;

	private BarButtonItem addCustomFieldBarButtonItem;

	private BarButtonItem editCustomFieldBarButtonItem;

	private BarButtonItem removeCustomFieldBarButtonItem;

	private PopupMenu gridPopupMenu;

	private BarManager gridBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	public bool ValuesChanged { get; private set; }

	public CustomFieldsSummaryForm()
	{
		InitializeComponent();
		infoUserControl.Description = "<b>Please note: </b>Pro and Pro Plus plans support only 5 and 10 custom fields respectively. " + $"To be able to define up to {100} fields please ask for our Enterprise plan.";
		fieldsToRemove = new List<CustomFieldRow>();
		if (addBarButtonItem.SuperTip == null)
		{
			addBarButtonItem.SuperTip = new SuperToolTip
			{
				AllowHtmlText = DefaultBoolean.True
			};
		}
		gridView.RowCountChanged += GridView_RowCountChanged;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			SaveChanges();
			break;
		case Keys.Escape:
			Close();
			break;
		case Keys.Delete:
			if (!gridControl.MainView.IsEditing)
			{
				DeleteSelectedField();
			}
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void CustomFieldsSummaryForm_Load(object sender, EventArgs e)
	{
		try
		{
			ribbonControl.Manager.UseAltKeyForMenu = false;
			dbCustomFields = from x in DB.CustomField.GetCustomFields(null, false)
				select new CustomFieldRow(x);
			customFields = new BindingList<CustomFieldRow>(dbCustomFields.ToList());
			gridControl.DataSource = customFields;
			RefreshSuggestedFieldsDataSource();
			gridView.BestFitColumns();
			SetAddCustomFieldControlsAccesibility();
			SetFunctionality();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, this);
		}
	}

	protected async void CustomFieldsSummaryForm_Shown(object sender, EventArgs e)
	{
		gridView.BeginUpdate();
		await Task.Delay(100);
		gridView.EndUpdate();
	}

	private void SetFunctionality()
	{
		infoUserControlLayoutControlItem.Visibility = LayoutVisibility.Always;
		SetInfoTexts();
		trialUpgradeLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	private void GridView_RowCountChanged(object sender, EventArgs e)
	{
		SetInfoTexts();
	}

	private void SetInfoTexts()
	{
		if (gridView.RowCount < 100)
		{
			infoUserControl.Description = Licence.GetCustomFieldsLimitInfoBarMessage();
			addBarButtonItem.SuperTip.Items.Clear();
			addBarButtonItem.SuperTip.Items.Add(infoUserControl.Description);
		}
		else
		{
			string description = $"You reached out limit of {100} custom fields.";
			infoUserControl.Description = description;
			addBarButtonItem.SuperTip.Items.Clear();
			addBarButtonItem.SuperTip.Items.Add(description);
		}
	}

	private void RefreshSuggestedFieldsDataSource()
	{
		IEnumerable<string> exisitingTitles = (gridControl.DataSource as BindingList<CustomFieldRow>).Select((CustomFieldRow x) => x.Title.ToLower());
		List<BaseCustomFieldDB.SuggestedCustomField> list = DB.CustomField.SuggestedCustomFields.Where((BaseCustomFieldDB.SuggestedCustomField x) => !exisitingTitles.Contains(x.Title.ToLower())).ToList();
		suggestedFieldsFlowLayoutPanel.SuspendLayout();
		suggestedFieldsFlowLayoutPanel.Controls.Clear();
		foreach (BaseCustomFieldDB.SuggestedCustomField item in list.Where((BaseCustomFieldDB.SuggestedCustomField x) => !x.IsHiddenOnSuggestedList))
		{
			LabelControl suggestedFieldControl = GetSuggestedFieldControl(item);
			suggestedFieldsFlowLayoutPanel.Controls.Add(suggestedFieldControl);
		}
		if (showAllSuggestedCustomFields)
		{
			foreach (BaseCustomFieldDB.SuggestedCustomField item2 in list.Where((BaseCustomFieldDB.SuggestedCustomField x) => x.IsHiddenOnSuggestedList))
			{
				LabelControl suggestedFieldControl2 = GetSuggestedFieldControl(item2);
				suggestedFieldsFlowLayoutPanel.Controls.Add(suggestedFieldControl2);
			}
		}
		else
		{
			LabelControl moreSuggestedFieldsControl = GetMoreSuggestedFieldsControl();
			suggestedFieldsFlowLayoutPanel.Controls.Add(moreSuggestedFieldsControl);
		}
		suggestedFieldsFlowLayoutPanel.ResumeLayout();
		suggestedFieldsLayoutControlItem.Visibility = ((list.Count == 0) ? LayoutVisibility.Never : LayoutVisibility.Always);
	}

	private LabelControl GetSuggestedFieldControl(BaseCustomFieldDB.SuggestedCustomField field)
	{
		LabelControl labelControl = new LabelControl();
		labelControl.AllowHtmlString = true;
		labelControl.Cursor = Cursors.Hand;
		labelControl.ForeColor = Color.FromArgb(32, 31, 53);
		labelControl.Text = "<href>" + field.Title + "</href>";
		labelControl.Width = (int)Math.Ceiling(labelControl.CreateGraphics().MeasureString(field.Title, labelControl.Font).Width + 1f);
		labelControl.HyperlinkClick += delegate(object s, HyperlinkClickEventArgs e)
		{
			_ = e.Text;
			AddCustomField(field, showDialog: false);
			RefreshSuggestedFieldsDataSource();
			ValuesChanged = true;
		};
		return labelControl;
	}

	private LabelControl GetMoreSuggestedFieldsControl()
	{
		LabelControl labelControl = new LabelControl();
		labelControl.AllowHtmlString = true;
		labelControl.Cursor = Cursors.Hand;
		labelControl.ForeColor = Color.FromArgb(32, 31, 53);
		labelControl.Text = "<href>More fields...</href>";
		labelControl.Width = (int)Math.Ceiling(labelControl.CreateGraphics().MeasureString("More fields...", labelControl.Font).Width + 1f);
		labelControl.HyperlinkClick += delegate(object s, HyperlinkClickEventArgs e)
		{
			_ = e.Text;
			showAllSuggestedCustomFields = true;
			RefreshSuggestedFieldsDataSource();
		};
		return labelControl;
	}

	private void AddCustomField(BaseCustomFieldDB.SuggestedCustomField suggestedField = null, bool showDialog = true)
	{
		List<CustomFieldRow> list = GetAllFields().ToList();
		List<CustomFieldRow> collection = (from x in DB.CustomField.GetCustomFields(100, true)
			select new CustomFieldRow(x) into x
			where x.IsClassificatorField
			select x).ToList();
		list.AddRange(collection);
		IEnumerable<string> notUsedFields = GetNotUsedFields(list);
		if (notUsedFields.Count() == 0)
		{
			CustomMessageBoxForm.Show("There are no remaining custom fields to use. Please remove any existing field to add new one.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		int ordinalPosition = ((list.Count() == 0) ? 1 : (list.Max((CustomFieldRow x) => x.OrdinalPosition) + 1));
		string fieldName = notUsedFields.FirstOrDefault();
		CustomFieldRow customFieldRow = ((!showDialog) ? new CustomFieldRow(suggestedField) : new CustomFieldRow());
		customFieldRow.FieldName = fieldName;
		customFieldRow.OrdinalPosition = ordinalPosition;
		if (showDialog)
		{
			if (new CustomFieldForm(customFieldRow, list, editMode: false).ShowDialog() == DialogResult.OK)
			{
				customFields.Add(customFieldRow);
				ValuesChanged = true;
			}
		}
		else
		{
			customFieldRow.Code = DB.CustomField.GenerateCustomFieldCode(suggestedField.Title, list);
			customFields.Add(customFieldRow);
		}
		SetAddCustomFieldControlsAccesibility();
		RefreshSuggestedFieldsDataSource();
	}

	private void SetAddCustomFieldControlsAccesibility()
	{
		bool flag = ((!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataClassification)) ? (customFields.Count() < Licence.GetCustomFieldsLimit()) : (customFields.Count((CustomFieldRow x) => !x.IsClassificatorField) < Licence.GetCustomFieldsLimit()));
		BarButtonItem barButtonItem = addBarButtonItem;
		BarButtonItem barButtonItem2 = addCustomFieldBarButtonItem;
		bool flag3 = (suggestedFieldsFlowLayoutPanel.Enabled = flag);
		bool enabled = (barButtonItem2.Enabled = flag3);
		barButtonItem.Enabled = enabled;
	}

	private int GetCustomFieldsLimit()
	{
		int customFieldsLimit = Licence.GetCustomFieldsLimit();
		if (customFields.ToList().TrueForAll((CustomFieldRow x) => x.IsClassificatorField))
		{
			return customFieldsLimit + customFields.Count();
		}
		if (customFieldsLimit == 0)
		{
			return 0;
		}
		return customFieldsLimit + customFields.Count((CustomFieldRow x) => x.IsClassificatorField && x.OrdinalPosition < customFields.Where((CustomFieldRow y) => !y.IsClassificatorField).Take(customFieldsLimit).Max((CustomFieldRow y) => y.OrdinalPosition) + 1);
	}

	private void EditCustomField()
	{
		if (gridView.GetFocusedRow() is CustomFieldRow customFieldRow && (customFieldRow.IsClassificatorField || gridView.FocusedRowHandle <= GetCustomFieldsLimit() - 1) && !customFieldRow.IsClassificatorField)
		{
			List<CustomFieldRow> list = GetAllFields().ToList();
			list.Remove(customFieldRow);
			CustomFieldForm customFieldForm = new CustomFieldForm(customFieldRow, list);
			customFieldForm.ShowDialog();
			ValuesChanged = ValuesChanged || customFieldForm.ValuesChanged;
			RefreshSuggestedFieldsDataSource();
			gridView.RefreshData();
		}
	}

	private IEnumerable<string> GetNotUsedFields(IEnumerable<CustomFieldRow> existingFields)
	{
		IEnumerable<string> usedNames = existingFields.Select((CustomFieldRow x) => x.FieldName);
		return from x in Enumerable.Range(1, 100)
			select $"field{x}" into x
			where !usedNames.Contains(x)
			select x;
	}

	private void SaveChanges()
	{
		if (isProcessing)
		{
			return;
		}
		IEnumerable<CustomFieldRow> allFields = GetAllFields();
		string firstNonUniqueField = GetFirstNonUniqueField(allFields);
		if (!string.IsNullOrWhiteSpace(firstNonUniqueField))
		{
			CustomMessageBoxForm.Show("Field title must be unique. Please enter another title instead of " + firstNonUniqueField + ".", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		progressPanelPanel.Visible = true;
		ChangeLayout(isEnabled: false);
		isProcessing = true;
		isClosingEnabled = false;
		UpdateOrdinalPositions();
		CustomField[] convertedCustomFields = DB.CustomField.GetPreparedCustomFields(allFields, fieldsToRemove, dbCustomFields);
		bool rebuildValuesDictionary = true;
		if (convertedCustomFields.Any((CustomField x) => x.GeneralTypeChange != CustomField.GeneralTypeChangeEnum.None))
		{
			rebuildValuesDictionary = CustomMessageBoxForm.Show("Do you want to rebuild dictionary (recommended)?" + Environment.NewLine + Environment.NewLine + "This might take several minutes.", "Rebuild dictionary", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}
		new TaskFactory().StartNew(delegate
		{
			UpdateProgressDescription();
			DB.CustomField.UpdateCustomFields(convertedCustomFields, rebuildValuesDictionary, delegate(long? step, long? stepsCount, string stepName)
			{
				if (isCancelled)
				{
					UpdateProgressDescription(null, null, "Cancelling...");
				}
				else
				{
					UpdateProgressDescription(step, stepsCount, stepName);
				}
				return new ProgressSupport.CancelArgs(isCancelled);
			}, !StaticData.IsProjectFile);
		}).ContinueWith(delegate(Task x)
		{
			progressPanel.Invoke((Action)delegate
			{
				progressPanelPanel.Visible = false;
			});
			cancelSimpleButton.Invoke((Action)delegate
			{
				cancelSimpleButton.Enabled = true;
			});
			if (!isCancelled && x.Exception == null)
			{
				ValuesChanged = false;
				WorkWithDataedoTrackingHelper.TrackCustomFieldsSave();
				base.DialogResult = DialogResult.OK;
			}
			isClosingEnabled = true;
			isCancelled = false;
			isProcessing = false;
			ChangeLayout(isEnabled: true);
			if (x.Exception != null)
			{
				throw x.Exception;
			}
		}, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously);
	}

	private void UpdateProgressDescription()
	{
		progressPanel.Invoke((Action)delegate
		{
			progressPanel.Description = "Saving custom fields...";
		});
		progressLabelControl.Invoke((Action)delegate
		{
			progressLabelControl.Text = "Processing...";
		});
	}

	private void UpdateProgressDescription(long? step, long? stepsCount, string stepMessage)
	{
		if (lastStepMessage != stepMessage)
		{
			lastStepMessage = stepMessage;
			progressPanel.Invoke((Action)delegate
			{
				progressPanel.Description = lastStepMessage + "...";
			});
		}
		double? num = lastProgressValue;
		num = ((!stepsCount.HasValue || !(stepsCount > 1)) ? null : new double?(Math.Round((float)step.GetValueOrDefault() / (float)stepsCount.Value * 100f, 1)));
		if (num != lastProgressValue)
		{
			lastProgressValue = num;
			progressLabelControl.Invoke((Action)delegate
			{
				progressLabelControl.Text = (lastProgressValue.HasValue ? $"Progress: {lastProgressValue:0.0}%" : "Processing...");
			});
		}
		else if (stepsCount == step && step.HasValue && stepsCount.HasValue)
		{
			progressLabelControl.Invoke((Action)delegate
			{
				progressLabelControl.Text = "Finalizing...";
			});
		}
	}

	private void ChangeLayout(bool isEnabled)
	{
		ribbonControl.Invoke((Action)delegate
		{
			ribbonControl.Enabled = isEnabled;
		});
		gridControl.Invoke((Action)delegate
		{
			gridControl.Enabled = isEnabled;
		});
		suggestedFieldsFlowLayoutPanel.Invoke((Action)delegate
		{
			suggestedFieldsFlowLayoutPanel.Enabled = isEnabled;
		});
		okSimpleButton.Invoke((Action)delegate
		{
			okSimpleButton.Enabled = isEnabled;
		});
		Invoke((Action)delegate
		{
			Refresh();
		});
	}

	private string GetFirstNonUniqueField(IEnumerable<CustomFieldRow> newFields)
	{
		foreach (CustomFieldRow field in customFields)
		{
			CustomFieldRow customFieldRow = newFields.FirstOrDefault((CustomFieldRow x) => field.Code.Equals(x.Code) && field.CustomFieldId != x.CustomFieldId);
			if (customFieldRow != null)
			{
				return customFieldRow.Title;
			}
		}
		return string.Empty;
	}

	private void UpdateOrdinalPositions()
	{
		for (int i = 0; i < gridView.RowCount; i++)
		{
			int visibleRowHandle = gridView.GetVisibleRowHandle(i);
			if (gridView.IsDataRow(visibleRowHandle))
			{
				(gridView.GetRow(visibleRowHandle) as CustomFieldRow).OrdinalPosition = i + 1;
			}
		}
	}

	private IEnumerable<CustomFieldRow> GetAllFields()
	{
		for (int i = 0; i < gridView.RowCount; i++)
		{
			int visibleRowHandle = gridView.GetVisibleRowHandle(i);
			if (gridView.IsDataRow(visibleRowHandle))
			{
				CustomFieldRow customFieldRow;
				try
				{
					customFieldRow = gridView.GetRow(visibleRowHandle) as CustomFieldRow;
				}
				catch (Exception)
				{
					continue;
				}
				yield return customFieldRow;
			}
		}
	}

	private void removeSimpleButton_Click(object sender, EventArgs e)
	{
		DeleteSelectedField();
	}

	private void DeleteSelectedField()
	{
		int focusedRowHandle = gridView.FocusedRowHandle;
		if (gridView.IsDataRow(focusedRowHandle))
		{
			CustomFieldRow customFieldRow = gridView.GetRow(focusedRowHandle) as CustomFieldRow;
			if (GeneralMessageBoxesHandling.Show("Are you sure you want to remove <b>" + customFieldRow.Title + "</b> custom field? All content would be lost after saving changes.", "Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, null, 2, this)?.DialogResult == DialogResult.OK)
			{
				ValuesChanged = true;
				fieldsToRemove.Add(customFieldRow);
				gridView.DeleteSelectedRows();
				SetAddCustomFieldControlsAccesibility();
				RefreshSuggestedFieldsDataSource();
			}
		}
	}

	private void okSimpleButton_Click(object sender, EventArgs e)
	{
		SaveChanges();
	}

	private void cancelSimpleButton_Click(object sender, EventArgs e)
	{
		if (isProcessing)
		{
			isCancelled = true;
			UpdateProgressDescription(null, null, "Cancelling...");
			cancelSimpleButton.Enabled = false;
		}
		else
		{
			Close();
		}
	}

	private void gridView_DoubleClick(object sender, EventArgs e)
	{
		DXMouseEventArgs dXMouseEventArgs = (DXMouseEventArgs)e;
		GridHitInfo gridHitInfo = gridView.CalcHitInfo(dXMouseEventArgs.Location);
		if (gridHitInfo.InRow || gridHitInfo.InRowCell)
		{
			EditCustomField();
		}
	}

	private void SwapOrdinalPositions(int rowHandle, int step)
	{
		CustomFieldRow customFieldRow = gridView.GetRow(rowHandle) as CustomFieldRow;
		CustomFieldRow customFieldRow2 = gridView.GetRow(rowHandle + step) as CustomFieldRow;
		int index = customFields.IndexOf(customFieldRow2);
		int ordinalPosition = customFieldRow.OrdinalPosition;
		int num = (customFieldRow.OrdinalPosition = customFieldRow2.OrdinalPosition);
		customFieldRow2.OrdinalPosition = ordinalPosition;
		customFields.Remove(customFieldRow);
		customFields.Insert(index, customFieldRow);
		gridView.FocusedRowHandle = rowHandle + step;
		ValuesChanged = true;
	}

	private void CustomFieldsSummaryForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (isClosingEnabled)
		{
			if (ValuesChanged)
			{
				DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Custom fields has been changed, would you like to save these changes?", "Custom fields has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
				if (dialogResult == DialogResult.Yes)
				{
					e.Cancel = true;
					SaveChanges();
					base.DialogResult = DialogResult.OK;
				}
				else if (dialogResult != DialogResult.No)
				{
					base.DialogResult = DialogResult.Cancel;
					e.Cancel = true;
				}
			}
		}
		else
		{
			e.Cancel = true;
		}
	}

	private void addBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddCustomField();
	}

	private void editBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		EditCustomField();
	}

	private void removeBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		DeleteSelectedField();
	}

	private void moveUpBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		int focusedRowHandle = gridView.FocusedRowHandle;
		if (focusedRowHandle > 0)
		{
			SwapOrdinalPositions(focusedRowHandle, -1);
		}
	}

	private void moveDownBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		int focusedRowHandle = gridView.FocusedRowHandle;
		if (focusedRowHandle < gridView.RowCount - 1 && focusedRowHandle >= 0)
		{
			SwapOrdinalPositions(focusedRowHandle, 1);
		}
	}

	private void gridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		CustomFieldRow customFieldRow = gridView.GetRow(e.RowHandle) as CustomFieldRow;
		if (((!customFieldRow.IsClassificatorField && e.RowHandle > GetCustomFieldsLimit() - 1) || customFieldRow.IsClassificatorField) && gridView.FocusedRowHandle != e.RowHandle)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
		}
	}

	private void gridView_RowClick(object sender, RowClickEventArgs e)
	{
		CustomFieldRow customField = gridView.GetRow(e.RowHandle) as CustomFieldRow;
		SetEditCustomFieldButton(e.RowHandle, customField);
	}

	private void gridView_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(e.Location);
			if (gridView.GetRow(gridHitInfo.RowHandle) is CustomFieldRow customField)
			{
				SetEditCustomFieldButton(gridHitInfo.RowHandle, customField);
			}
		}
	}

	private void SetEditCustomFieldButton(int rowHandle, CustomFieldRow customField)
	{
		if ((!customField.IsClassificatorField && rowHandle > GetCustomFieldsLimit() - 1) || customField.IsClassificatorField)
		{
			editBarButtonItem.Enabled = false;
		}
		else
		{
			editBarButtonItem.Enabled = true;
		}
	}

	private void gridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopupWithoutSorting(e);
		gridPopupMenu.ShowPopupMenu(sender, e, inRowCellOnly: false);
	}

	private void addCustomFieldBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddCustomField();
	}

	private void editCustomFieldBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		EditCustomField();
	}

	private void removeCustomFieldBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		DeleteSelectedField();
	}

	private void gridPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		if (gridView.GetRow(gridView.FocusedRowHandle) is CustomFieldRow customFieldRow && !Grids.GetBeforePopupShouldCancel(sender))
		{
			bool beforePopupIsRowClicked = Grids.GetBeforePopupIsRowClicked(sender);
			if (beforePopupIsRowClicked)
			{
				_ = !customFieldRow.IsClassificatorField;
			}
			else
				_ = 0;
			editCustomFieldBarButtonItem.Enabled = !customFieldRow.IsClassificatorField;
			BarItemVisibility barItemVisibility3 = (editCustomFieldBarButtonItem.Visibility = (removeCustomFieldBarButtonItem.Visibility = ((!beforePopupIsRowClicked) ? BarItemVisibility.Never : BarItemVisibility.Always)));
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
		DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.CustomFieldsSummaryForm));
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.trialUpgradeLinkUserControl = new Dataedo.App.UserControls.LinkUserControl();
		this.infoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.suggestedFieldsFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.gridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.titleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.typeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.classGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.documentationGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.moduleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.columnGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.relationGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.keyGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.triggerGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.procedureGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.parameterGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.termGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.addBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveUpBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveDownBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.editBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.addCustomFieldBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.editCustomFieldBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeCustomFieldBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
		this.manipulateRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.sortRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.suggestedFieldsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.infoUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
		this.botoomMarginEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.rightEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.trialUpgradeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.progressPanelPanel = new System.Windows.Forms.Panel();
		this.progressPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.progressLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.progressLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.progressPanel = new DevExpress.XtraWaitForm.ProgressPanel();
		this.progressLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.progressLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.gridPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.gridBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.suggestedFieldsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.infoUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.botoomMarginEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rightEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.trialUpgradeLayoutControlItem).BeginInit();
		this.progressPanelPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressPanelControl).BeginInit();
		this.progressPanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControl).BeginInit();
		this.progressLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridBarManager).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.trialUpgradeLinkUserControl);
		this.layoutControl.Controls.Add(this.infoUserControl);
		this.layoutControl.Controls.Add(this.suggestedFieldsFlowLayoutPanel);
		this.layoutControl.Controls.Add(this.okSimpleButton);
		this.layoutControl.Controls.Add(this.gridControl);
		this.layoutControl.Controls.Add(this.cancelSimpleButton);
		this.layoutControl.Controls.Add(this.ribbonControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3690, 361, 250, 350);
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(1356, 576);
		this.layoutControl.TabIndex = 1;
		this.layoutControl.Text = "layoutControl1";
		this.trialUpgradeLinkUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.trialUpgradeLinkUserControl.Location = new System.Drawing.Point(0, 104);
		this.trialUpgradeLinkUserControl.Name = "trialUpgradeLinkUserControl";
		this.trialUpgradeLinkUserControl.Size = new System.Drawing.Size(1356, 36);
		this.trialUpgradeLinkUserControl.TabIndex = 33;
		this.infoUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.infoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.infoUserControl.Description = "<b>Please note: </b>Pro and Pro Plus plans support only 5 and 10 custom fields respectively. To be able to define up to 100 fields please ask for our Enterprise plan.";
		this.infoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.infoUserControl.Image = Dataedo.App.Properties.Resources.about_16;
		this.infoUserControl.Location = new System.Drawing.Point(0, 140);
		this.infoUserControl.MaximumSize = new System.Drawing.Size(0, 32);
		this.infoUserControl.MinimumSize = new System.Drawing.Size(564, 32);
		this.infoUserControl.Name = "infoUserControl";
		this.infoUserControl.Size = new System.Drawing.Size(1356, 32);
		this.infoUserControl.TabIndex = 31;
		this.suggestedFieldsFlowLayoutPanel.AutoScroll = true;
		this.suggestedFieldsFlowLayoutPanel.Location = new System.Drawing.Point(10, 508);
		this.suggestedFieldsFlowLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
		this.suggestedFieldsFlowLayoutPanel.Name = "suggestedFieldsFlowLayoutPanel";
		this.suggestedFieldsFlowLayoutPanel.Size = new System.Drawing.Size(1336, 30);
		this.suggestedFieldsFlowLayoutPanel.TabIndex = 14;
		this.okSimpleButton.Location = new System.Drawing.Point(1170, 542);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.okSimpleButton.StyleController = this.layoutControl;
		this.okSimpleButton.TabIndex = 9;
		this.okSimpleButton.Text = "Save";
		this.okSimpleButton.Click += new System.EventHandler(okSimpleButton_Click);
		this.gridControl.Location = new System.Drawing.Point(10, 182);
		this.gridControl.MainView = this.gridView;
		this.gridControl.Name = "gridControl";
		this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCheckEdit });
		this.gridControl.Size = new System.Drawing.Size(1336, 298);
		this.gridControl.TabIndex = 7;
		this.gridControl.ToolTipController = this.toolTipController;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView });
		this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.titleGridColumn, this.typeGridColumn, this.classGridColumn, this.documentationGridColumn, this.moduleGridColumn, this.tableGridColumn, this.columnGridColumn, this.relationGridColumn, this.keyGridColumn, this.triggerGridColumn,
			this.procedureGridColumn, this.parameterGridColumn, this.termGridColumn, this.descriptionGridColumn
		});
		this.gridView.GridControl = this.gridControl;
		this.gridView.Name = "gridView";
		this.gridView.OptionsBehavior.Editable = false;
		this.gridView.OptionsBehavior.ReadOnly = true;
		this.gridView.OptionsFilter.AllowFilterEditor = false;
		this.gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gridView.OptionsView.ShowGroupPanel = false;
		this.gridView.OptionsView.ShowIndicator = false;
		this.gridView.RowHighlightingIsEnabled = true;
		this.gridView.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(gridView_RowClick);
		this.gridView.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(gridView_RowCellStyle);
		this.gridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(gridView_PopupMenuShowing);
		this.gridView.MouseDown += new System.Windows.Forms.MouseEventHandler(gridView_MouseDown);
		this.gridView.DoubleClick += new System.EventHandler(gridView_DoubleClick);
		this.titleGridColumn.Caption = "Title";
		this.titleGridColumn.FieldName = "Title";
		this.titleGridColumn.MaxWidth = 200;
		this.titleGridColumn.MinWidth = 90;
		this.titleGridColumn.Name = "titleGridColumn";
		this.titleGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.titleGridColumn.OptionsFilter.AllowFilter = false;
		this.titleGridColumn.Visible = true;
		this.titleGridColumn.VisibleIndex = 0;
		this.titleGridColumn.Width = 90;
		this.typeGridColumn.Caption = "Type";
		this.typeGridColumn.FieldName = "TypeForDisplay";
		this.typeGridColumn.MaxWidth = 180;
		this.typeGridColumn.MinWidth = 180;
		this.typeGridColumn.Name = "typeGridColumn";
		this.typeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.typeGridColumn.OptionsFilter.AllowFilter = false;
		this.typeGridColumn.Visible = true;
		this.typeGridColumn.VisibleIndex = 1;
		this.typeGridColumn.Width = 180;
		this.classGridColumn.Caption = "Class";
		this.classGridColumn.FieldName = "CustomFieldClassName";
		this.classGridColumn.MaxWidth = 160;
		this.classGridColumn.MinWidth = 80;
		this.classGridColumn.Name = "classGridColumn";
		this.classGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.classGridColumn.OptionsFilter.AllowFilter = false;
		this.classGridColumn.Visible = true;
		this.classGridColumn.VisibleIndex = 2;
		this.classGridColumn.Width = 80;
		this.documentationGridColumn.Caption = "Documentation";
		this.documentationGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.documentationGridColumn.FieldName = "DocumentationVisibility";
		this.documentationGridColumn.MaxWidth = 80;
		this.documentationGridColumn.MinWidth = 80;
		this.documentationGridColumn.Name = "documentationGridColumn";
		this.documentationGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.documentationGridColumn.OptionsFilter.AllowFilter = false;
		this.documentationGridColumn.Visible = true;
		this.documentationGridColumn.VisibleIndex = 3;
		this.documentationGridColumn.Width = 80;
		this.repositoryItemCheckEdit.AutoHeight = false;
		this.repositoryItemCheckEdit.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
		this.repositoryItemCheckEdit.ImageOptions.ImageChecked = Dataedo.App.Properties.Resources.checkbox_preview_ok_16;
		this.repositoryItemCheckEdit.ImageOptions.ImageGrayed = Dataedo.App.Properties.Resources.checkbox_preview_no_16;
		this.repositoryItemCheckEdit.ImageOptions.ImageUnchecked = Dataedo.App.Properties.Resources.checkbox_preview_no_16;
		this.repositoryItemCheckEdit.Name = "repositoryItemCheckEdit";
		this.repositoryItemCheckEdit.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.moduleGridColumn.Caption = "Subject Area";
		this.moduleGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.moduleGridColumn.FieldName = "ModuleVisibility";
		this.moduleGridColumn.MaxWidth = 80;
		this.moduleGridColumn.MinWidth = 80;
		this.moduleGridColumn.Name = "moduleGridColumn";
		this.moduleGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.moduleGridColumn.OptionsFilter.AllowFilter = false;
		this.moduleGridColumn.Visible = true;
		this.moduleGridColumn.VisibleIndex = 4;
		this.moduleGridColumn.Width = 80;
		this.tableGridColumn.Caption = "Table/View/Object";
		this.tableGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.tableGridColumn.FieldName = "TableVisibility";
		this.tableGridColumn.MaxWidth = 100;
		this.tableGridColumn.MinWidth = 100;
		this.tableGridColumn.Name = "tableGridColumn";
		this.tableGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.tableGridColumn.OptionsFilter.AllowFilter = false;
		this.tableGridColumn.Visible = true;
		this.tableGridColumn.VisibleIndex = 5;
		this.tableGridColumn.Width = 100;
		this.columnGridColumn.Caption = "Column";
		this.columnGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.columnGridColumn.FieldName = "ColumnVisibility";
		this.columnGridColumn.MaxWidth = 80;
		this.columnGridColumn.MinWidth = 80;
		this.columnGridColumn.Name = "columnGridColumn";
		this.columnGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.columnGridColumn.OptionsFilter.AllowFilter = false;
		this.columnGridColumn.Visible = true;
		this.columnGridColumn.VisibleIndex = 6;
		this.columnGridColumn.Width = 80;
		this.relationGridColumn.Caption = "Relationship";
		this.relationGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.relationGridColumn.FieldName = "RelationVisibility";
		this.relationGridColumn.MaxWidth = 80;
		this.relationGridColumn.MinWidth = 80;
		this.relationGridColumn.Name = "relationGridColumn";
		this.relationGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.relationGridColumn.OptionsFilter.AllowFilter = false;
		this.relationGridColumn.Visible = true;
		this.relationGridColumn.VisibleIndex = 7;
		this.relationGridColumn.Width = 80;
		this.keyGridColumn.Caption = "Key";
		this.keyGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.keyGridColumn.FieldName = "KeyVisibility";
		this.keyGridColumn.MaxWidth = 80;
		this.keyGridColumn.MinWidth = 80;
		this.keyGridColumn.Name = "keyGridColumn";
		this.keyGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.keyGridColumn.OptionsFilter.AllowFilter = false;
		this.keyGridColumn.Visible = true;
		this.keyGridColumn.VisibleIndex = 8;
		this.keyGridColumn.Width = 80;
		this.triggerGridColumn.Caption = "Trigger";
		this.triggerGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.triggerGridColumn.FieldName = "TriggerVisibility";
		this.triggerGridColumn.MaxWidth = 80;
		this.triggerGridColumn.MinWidth = 80;
		this.triggerGridColumn.Name = "triggerGridColumn";
		this.triggerGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.triggerGridColumn.OptionsFilter.AllowFilter = false;
		this.triggerGridColumn.Visible = true;
		this.triggerGridColumn.VisibleIndex = 9;
		this.triggerGridColumn.Width = 80;
		this.procedureGridColumn.Caption = "Proc/Function";
		this.procedureGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.procedureGridColumn.FieldName = "ProcedureVisibility";
		this.procedureGridColumn.MaxWidth = 80;
		this.procedureGridColumn.MinWidth = 80;
		this.procedureGridColumn.Name = "procedureGridColumn";
		this.procedureGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.procedureGridColumn.OptionsFilter.AllowFilter = false;
		this.procedureGridColumn.Visible = true;
		this.procedureGridColumn.VisibleIndex = 10;
		this.procedureGridColumn.Width = 80;
		this.parameterGridColumn.Caption = "Parameter";
		this.parameterGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.parameterGridColumn.FieldName = "ParameterVisibility";
		this.parameterGridColumn.MaxWidth = 80;
		this.parameterGridColumn.MinWidth = 80;
		this.parameterGridColumn.Name = "parameterGridColumn";
		this.parameterGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.parameterGridColumn.OptionsFilter.AllowFilter = false;
		this.parameterGridColumn.Visible = true;
		this.parameterGridColumn.VisibleIndex = 11;
		this.parameterGridColumn.Width = 80;
		this.termGridColumn.Caption = "Term";
		this.termGridColumn.ColumnEdit = this.repositoryItemCheckEdit;
		this.termGridColumn.FieldName = "TermVisibility";
		this.termGridColumn.MaxWidth = 80;
		this.termGridColumn.MinWidth = 80;
		this.termGridColumn.Name = "termGridColumn";
		this.termGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.termGridColumn.OptionsFilter.AllowFilter = false;
		this.termGridColumn.Visible = true;
		this.termGridColumn.VisibleIndex = 12;
		this.termGridColumn.Width = 80;
		this.descriptionGridColumn.Caption = "Description";
		this.descriptionGridColumn.FieldName = "Description";
		this.descriptionGridColumn.Name = "descriptionGridColumn";
		this.descriptionGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.descriptionGridColumn.OptionsFilter.AllowFilter = false;
		this.descriptionGridColumn.Visible = true;
		this.descriptionGridColumn.VisibleIndex = 13;
		this.descriptionGridColumn.Width = 164;
		this.toolTipController.KeepWhileHovered = true;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.cancelSimpleButton.Location = new System.Drawing.Point(1264, 542);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl;
		this.cancelSimpleButton.TabIndex = 5;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(cancelSimpleButton_Click);
		this.ribbonControl.AllowMinimizeRibbon = false;
		this.ribbonControl.Dock = System.Windows.Forms.DockStyle.None;
		this.ribbonControl.ExpandCollapseItem.Id = 0;
		this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[11]
		{
			this.ribbonControl.ExpandCollapseItem,
			this.ribbonControl.SearchEditItem,
			this.barButtonItem1,
			this.addBarButtonItem,
			this.removeBarButtonItem,
			this.moveUpBarButtonItem,
			this.moveDownBarButtonItem,
			this.editBarButtonItem,
			this.addCustomFieldBarButtonItem,
			this.editCustomFieldBarButtonItem,
			this.removeCustomFieldBarButtonItem
		});
		this.ribbonControl.Location = new System.Drawing.Point(2, 2);
		this.ribbonControl.Margin = new System.Windows.Forms.Padding(0);
		this.ribbonControl.MaxItemId = 14;
		this.ribbonControl.Name = "ribbonControl";
		this.ribbonControl.OptionsPageCategories.ShowCaptions = false;
		this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.ribbonPage });
		this.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
		this.ribbonControl.ShowToolbarCustomizeItem = false;
		this.ribbonControl.Size = new System.Drawing.Size(1352, 102);
		this.ribbonControl.Toolbar.ShowCustomizeItem = false;
		this.ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
		this.barButtonItem1.Caption = "barButtonItem1";
		this.barButtonItem1.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
		this.barButtonItem1.Id = 1;
		this.barButtonItem1.Name = "barButtonItem1";
		this.addBarButtonItem.Caption = "Add";
		this.addBarButtonItem.Id = 2;
		this.addBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.about_16;
		this.addBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.add_32;
		this.addBarButtonItem.Name = "addBarButtonItem";
		superToolTip.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
		toolTipItem.Text = " ";
		superToolTip.Items.Add(toolTipItem);
		this.addBarButtonItem.SuperTip = superToolTip;
		this.addBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addBarButtonItem_ItemClick);
		this.removeBarButtonItem.Caption = "Remove";
		this.removeBarButtonItem.Id = 3;
		this.removeBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.removeBarButtonItem.Name = "removeBarButtonItem";
		this.removeBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(removeBarButtonItem_ItemClick);
		this.moveUpBarButtonItem.Caption = "Move up";
		this.moveUpBarButtonItem.Id = 4;
		this.moveUpBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_up_16;
		this.moveUpBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_up_32;
		this.moveUpBarButtonItem.Name = "moveUpBarButtonItem";
		this.moveUpBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveUpBarButtonItem_ItemClick);
		this.moveDownBarButtonItem.Caption = "Move down";
		this.moveDownBarButtonItem.Id = 9;
		this.moveDownBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_down_16;
		this.moveDownBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_down_32;
		this.moveDownBarButtonItem.Name = "moveDownBarButtonItem";
		this.moveDownBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveDownBarButtonItem_ItemClick);
		this.editBarButtonItem.Caption = "Edit";
		this.editBarButtonItem.Id = 10;
		this.editBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.editBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.edit_32;
		this.editBarButtonItem.Name = "editBarButtonItem";
		this.editBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(editBarButtonItem_ItemClick);
		this.addCustomFieldBarButtonItem.Caption = "Add";
		this.addCustomFieldBarButtonItem.Id = 11;
		this.addCustomFieldBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
		this.addCustomFieldBarButtonItem.Name = "addCustomFieldBarButtonItem";
		this.addCustomFieldBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addCustomFieldBarButtonItem_ItemClick);
		this.editCustomFieldBarButtonItem.Caption = "Edit";
		this.editCustomFieldBarButtonItem.Id = 12;
		this.editCustomFieldBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.editCustomFieldBarButtonItem.Name = "editCustomFieldBarButtonItem";
		this.editCustomFieldBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(editCustomFieldBarButtonItem_ItemClick);
		this.removeCustomFieldBarButtonItem.Caption = "Remove";
		this.removeCustomFieldBarButtonItem.Id = 13;
		this.removeCustomFieldBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeCustomFieldBarButtonItem.Name = "removeCustomFieldBarButtonItem";
		this.removeCustomFieldBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(removeCustomFieldBarButtonItem_ItemClick);
		this.ribbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[2] { this.manipulateRibbonPageGroup, this.sortRibbonPageGroup });
		this.ribbonPage.Name = "ribbonPage";
		this.ribbonPage.Text = "ribbonPage";
		this.manipulateRibbonPageGroup.AllowTextClipping = false;
		this.manipulateRibbonPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.addBarButtonItem);
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.editBarButtonItem);
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.removeBarButtonItem);
		this.manipulateRibbonPageGroup.Name = "manipulateRibbonPageGroup";
		this.manipulateRibbonPageGroup.Text = "Actions";
		this.sortRibbonPageGroup.AllowTextClipping = false;
		this.sortRibbonPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.sortRibbonPageGroup.ItemLinks.Add(this.moveUpBarButtonItem);
		this.sortRibbonPageGroup.ItemLinks.Add(this.moveDownBarButtonItem);
		this.sortRibbonPageGroup.Name = "sortRibbonPageGroup";
		this.sortRibbonPageGroup.Text = "Order";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[11]
		{
			this.emptySpaceItem1, this.layoutControlItem3, this.layoutControlItem4, this.layoutControlItem5, this.suggestedFieldsLayoutControlItem, this.infoUserControlLayoutControlItem, this.layoutControlItem10, this.botoomMarginEmptySpaceItem, this.emptySpaceItem3, this.rightEmptySpaceItem,
			this.trialUpgradeLayoutControlItem
		});
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(1356, 576);
		this.layoutControlGroup1.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 540);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 26);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 2, 2, 2);
		this.emptySpaceItem1.Size = new System.Drawing.Size(1168, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.Control = this.cancelSimpleButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(1262, 540);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem4.Control = this.gridControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 172);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 10);
		this.layoutControlItem4.Size = new System.Drawing.Size(1356, 318);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.Control = this.okSimpleButton;
		this.layoutControlItem5.Location = new System.Drawing.Point(1168, 540);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.suggestedFieldsLayoutControlItem.Control = this.suggestedFieldsFlowLayoutPanel;
		this.suggestedFieldsLayoutControlItem.Location = new System.Drawing.Point(0, 490);
		this.suggestedFieldsLayoutControlItem.MaxSize = new System.Drawing.Size(0, 50);
		this.suggestedFieldsLayoutControlItem.MinSize = new System.Drawing.Size(104, 50);
		this.suggestedFieldsLayoutControlItem.Name = "suggestedFieldsLayoutControlItem";
		this.suggestedFieldsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 2, 2);
		this.suggestedFieldsLayoutControlItem.Size = new System.Drawing.Size(1356, 50);
		this.suggestedFieldsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.suggestedFieldsLayoutControlItem.Text = "Suggested fields";
		this.suggestedFieldsLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.suggestedFieldsLayoutControlItem.TextSize = new System.Drawing.Size(79, 13);
		this.infoUserControlLayoutControlItem.Control = this.infoUserControl;
		this.infoUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 140);
		this.infoUserControlLayoutControlItem.Name = "infoUserControlLayoutControlItem";
		this.infoUserControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.infoUserControlLayoutControlItem.Size = new System.Drawing.Size(1356, 32);
		this.infoUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.infoUserControlLayoutControlItem.TextVisible = false;
		this.layoutControlItem10.Control = this.ribbonControl;
		this.layoutControlItem10.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem10.MaxSize = new System.Drawing.Size(0, 104);
		this.layoutControlItem10.MinSize = new System.Drawing.Size(104, 104);
		this.layoutControlItem10.Name = "layoutControlItem10";
		this.layoutControlItem10.Size = new System.Drawing.Size(1356, 104);
		this.layoutControlItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.TextVisible = false;
		this.botoomMarginEmptySpaceItem.AllowHotTrack = false;
		this.botoomMarginEmptySpaceItem.CustomizationFormText = "emptySpaceItem3";
		this.botoomMarginEmptySpaceItem.Location = new System.Drawing.Point(0, 566);
		this.botoomMarginEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 10);
		this.botoomMarginEmptySpaceItem.MinSize = new System.Drawing.Size(10, 10);
		this.botoomMarginEmptySpaceItem.Name = "botoomMarginEmptySpaceItem";
		this.botoomMarginEmptySpaceItem.Size = new System.Drawing.Size(1356, 10);
		this.botoomMarginEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.botoomMarginEmptySpaceItem.Text = "emptySpaceItem3";
		this.botoomMarginEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(1252, 540);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.rightEmptySpaceItem.AllowHotTrack = false;
		this.rightEmptySpaceItem.Location = new System.Drawing.Point(1346, 540);
		this.rightEmptySpaceItem.MaxSize = new System.Drawing.Size(10, 26);
		this.rightEmptySpaceItem.MinSize = new System.Drawing.Size(10, 26);
		this.rightEmptySpaceItem.Name = "rightEmptySpaceItem";
		this.rightEmptySpaceItem.Size = new System.Drawing.Size(10, 26);
		this.rightEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rightEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.trialUpgradeLayoutControlItem.Control = this.trialUpgradeLinkUserControl;
		this.trialUpgradeLayoutControlItem.Location = new System.Drawing.Point(0, 104);
		this.trialUpgradeLayoutControlItem.Name = "trialUpgradeLayoutControlItem";
		this.trialUpgradeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.trialUpgradeLayoutControlItem.Size = new System.Drawing.Size(1356, 36);
		this.trialUpgradeLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.trialUpgradeLayoutControlItem.TextVisible = false;
		this.progressPanelPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
		this.progressPanelPanel.Controls.Add(this.progressPanelControl);
		this.progressPanelPanel.Location = new System.Drawing.Point(528, 247);
		this.progressPanelPanel.Name = "progressPanelPanel";
		this.progressPanelPanel.Size = new System.Drawing.Size(300, 81);
		this.progressPanelPanel.TabIndex = 2;
		this.progressPanelPanel.Visible = false;
		this.progressPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Office2003;
		this.progressPanelControl.Controls.Add(this.progressLayoutControl);
		this.progressPanelControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.progressPanelControl.Location = new System.Drawing.Point(0, 0);
		this.progressPanelControl.Name = "progressPanelControl";
		this.progressPanelControl.Size = new System.Drawing.Size(300, 81);
		this.progressPanelControl.TabIndex = 5;
		this.progressLayoutControl.AllowCustomization = false;
		this.progressLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.progressLayoutControl.Controls.Add(this.progressLabelControl);
		this.progressLayoutControl.Controls.Add(this.progressPanel);
		this.progressLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.progressLayoutControl.Location = new System.Drawing.Point(2, 2);
		this.progressLayoutControl.Name = "progressLayoutControl";
		this.progressLayoutControl.Root = this.progressLayoutControlGroup;
		this.progressLayoutControl.Size = new System.Drawing.Size(296, 77);
		this.progressLayoutControl.TabIndex = 6;
		this.progressLayoutControl.Text = "layoutControl1";
		this.progressLabelControl.AutoEllipsis = true;
		this.progressLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.progressLabelControl.Location = new System.Drawing.Point(12, 52);
		this.progressLabelControl.Name = "progressLabelControl";
		this.progressLabelControl.Padding = new System.Windows.Forms.Padding(41, 0, 0, 0);
		this.progressLabelControl.Size = new System.Drawing.Size(272, 13);
		this.progressLabelControl.StyleController = this.progressLayoutControl;
		this.progressLabelControl.TabIndex = 6;
		this.progressLabelControl.Text = "Processing...";
		this.progressPanel.AppearanceCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f);
		this.progressPanel.AppearanceCaption.Options.UseFont = true;
		this.progressPanel.AppearanceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.progressPanel.AppearanceDescription.Options.UseFont = true;
		this.progressPanel.Description = "Saving custom fields...";
		this.progressPanel.Location = new System.Drawing.Point(12, 12);
		this.progressPanel.Name = "progressPanel";
		this.progressPanel.Size = new System.Drawing.Size(272, 36);
		this.progressPanel.StyleController = this.progressLayoutControl;
		this.progressPanel.TabIndex = 5;
		this.progressLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.progressLayoutControlGroup.GroupBordersVisible = false;
		this.progressLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.progressLayoutControlItem, this.layoutControlItem1 });
		this.progressLayoutControlGroup.Name = "progressLayoutControlGroup";
		this.progressLayoutControlGroup.Size = new System.Drawing.Size(296, 77);
		this.progressLayoutControlGroup.TextVisible = false;
		this.progressLayoutControlItem.Control = this.progressPanel;
		this.progressLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.progressLayoutControlItem.MinSize = new System.Drawing.Size(1, 1);
		this.progressLayoutControlItem.Name = "progressLayoutControlItem";
		this.progressLayoutControlItem.Size = new System.Drawing.Size(276, 40);
		this.progressLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.progressLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.progressLayoutControlItem.TextVisible = false;
		this.layoutControlItem1.Control = this.progressLabelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 40);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(276, 17);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.gridPopupMenu.ItemLinks.Add(this.addCustomFieldBarButtonItem);
		this.gridPopupMenu.ItemLinks.Add(this.editCustomFieldBarButtonItem);
		this.gridPopupMenu.ItemLinks.Add(this.removeCustomFieldBarButtonItem);
		this.gridPopupMenu.Name = "gridPopupMenu";
		this.gridPopupMenu.Ribbon = this.ribbonControl;
		this.gridPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(gridPopupMenu_BeforePopup);
		this.gridBarManager.DockControls.Add(this.barDockControlTop);
		this.gridBarManager.DockControls.Add(this.barDockControlBottom);
		this.gridBarManager.DockControls.Add(this.barDockControlLeft);
		this.gridBarManager.DockControls.Add(this.barDockControlRight);
		this.gridBarManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.gridBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1356, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 576);
		this.barDockControlBottom.Manager = this.gridBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1356, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.gridBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 576);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1356, 0);
		this.barDockControlRight.Manager = this.gridBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 576);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1356, 576);
		base.Controls.Add(this.progressPanelPanel);
		base.Controls.Add(this.layoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		this.DoubleBuffered = true;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("CustomFieldsSummaryForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "CustomFieldsSummaryForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Custom fields";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(CustomFieldsSummaryForm_FormClosing);
		base.Load += new System.EventHandler(CustomFieldsSummaryForm_Load);
		base.Shown += new System.EventHandler(CustomFieldsSummaryForm_Shown);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		this.layoutControl.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.suggestedFieldsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.infoUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.botoomMarginEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rightEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.trialUpgradeLayoutControlItem).EndInit();
		this.progressPanelPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressPanelControl).EndInit();
		this.progressPanelControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControl).EndInit();
		this.progressLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridBarManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
