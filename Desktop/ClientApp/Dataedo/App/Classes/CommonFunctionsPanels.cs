using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Helpers.Forms;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraTreeList.Menu;

namespace Dataedo.App.Classes;

public static class CommonFunctionsPanels
{
	private static List<string> columnsWithFixedWidth = new List<string> { "description", "title" };

	public static Dictionary<int, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue>> customFieldsForHistory = new Dictionary<int, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue>>();

	public static Dictionary<int, string> summaryObjectTitleHistory = new Dictionary<int, string>();

	public static void ShowUserControlInPanel(UserControl usercontrol, PanelControl panel)
	{
		(usercontrol as BasePanelControl)?.SetCustomFieldsPanelControlHeight();
		if (!panel.Controls.Contains(usercontrol) || panel.Controls.GetChildIndex(usercontrol) != 0)
		{
			foreach (Control control in panel.Controls)
			{
				if (control != usercontrol)
				{
					control.Visible = false;
				}
			}
			if (!panel.Controls.Contains(usercontrol))
			{
				panel.Controls.Add(usercontrol);
			}
			usercontrol.Visible = true;
			panel.Controls.SetChildIndex(usercontrol, 0);
			usercontrol.Dock = DockStyle.Fill;
		}
		(usercontrol as BasePanelControl)?.SetCustomFieldsPanelControlHeight();
		MainForm.Instance?.ShowOnboardings();
	}

	public static string SetTitle(bool isEdited, string title)
	{
		if (!string.IsNullOrEmpty(title))
		{
			bool flag = ContainsMark(title);
			if (isEdited)
			{
				if (!flag)
				{
					title += "*";
				}
			}
			else if (flag)
			{
				title = title.Remove(title.Length - 1);
			}
		}
		return title;
	}

	public static bool ContainsMark(string title)
	{
		return title.Substring(title.Length - 1).Equals("*");
	}

	public static void SetTabPageAndNodesTitle(bool isEdited, XtraTabPage xtraTabPage, DBTreeNode documentationNode, DBTreeNode objectNode, IEdit edit = null)
	{
		xtraTabPage.Text = SetTitle(isEdited, xtraTabPage.Text);
		documentationNode.Name = SetTitle(isEdited, documentationNode.Name);
		objectNode.Name = SetTitle(isEdited, objectNode.Name);
		edit?.SetValue(isEdited);
	}

	public static void SetTabPageTitle(bool isEdited, XtraTabPage xtraTabPage, IEdit edit = null)
	{
		xtraTabPage.Text = SetTitle(isEdited, xtraTabPage.Text);
		edit?.SetValue(isEdited);
	}

	public static void SetNodesTitle(bool isEdited, DBTreeNode documentationNode, DBTreeNode objectNode, IEdit edit = null)
	{
		documentationNode.Name = SetTitle(isEdited, documentationNode.Name);
		objectNode.Name = SetTitle(isEdited, objectNode.Name);
		edit?.SetValue(isEdited);
	}

	public static void SetTabPageAndNodesTitle(bool isEdited, XtraTabControl tabControl, XtraTabPage xtraTabPage, DBTreeNode documentationNode, DBTreeNode objectNode, IEdit edit = null)
	{
		xtraTabPage.Text = SetTitle(isEdited, xtraTabPage.Text);
		if (!tabControl.TabPages.Any((XtraTabPage p) => ContainsMark(p.Text)))
		{
			edit?.SetValue(isEdited);
		}
		documentationNode.Name = SetTitle(isEdited, documentationNode.Name);
		objectNode.Name = SetTitle(isEdited, objectNode.Name);
	}

	public static void SetLabelTitle(bool isEdited, Control labelControl)
	{
		labelControl.Text = SetTitle(isEdited, labelControl.Text);
	}

	public static void ClearTabPagesTitle(XtraTabControl databaseXtraTabControl, IEdit edit)
	{
		foreach (XtraTabPage tabPage in databaseXtraTabControl.TabPages)
		{
			SetTabPageTitle(isEdited: false, tabPage, edit);
		}
	}

	public static void AddEventsForSummaryTable(BulkCopyGridUserControl gridView, SharedObjectTypeEnum.ObjectType objectType, PopupMenu popupMenu, BarManager barManager, EventHandler showObjectControlEvent, GridColumn iconGridColumn, BarButtonItem deleteObjectBarButtonItem, ObjectEventArgs objectEventArgs, DBTreeMenu treeMenu, RepositoryItemCheckedComboBoxEdit repositoryItemCheckedComboBoxEdit, BaseSummaryUserControl baseSummaryUserControl, GridPanelUserControl gridPanel, BarButtonItem moveUpBarButtonItem, BarButtonItem moveDownBarButtonItem, BarButtonItem moveToTopBarButtonItem, BarButtonItem moveToBottomBarButtonItem, BarButtonItem sortalphabeticallyBarButtonItem, Form owner = null)
	{
		gridView.CustomDrawCell += delegate(object sender, RowCellCustomDrawEventArgs e)
		{
			ProgressPainter.SetEmptyProgressCellsBackground(e, baseSummaryUserControl.MainControl.ProgressType, baseSummaryUserControl.MainControl.ShowProgress);
		};
		gridView.RowCellStyle += delegate(object sender, RowCellStyleEventArgs e)
		{
			ColorDeletedRows(gridView, e);
		};
		gridView.DoubleClick += delegate
		{
			if (gridView.FocusedRowHandle >= 0)
			{
				Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
				GridHitInfo gridHitInfo = gridView.CalcHitInfo(pt);
				if ((gridHitInfo.InRow || gridHitInfo.InRowCell) && (gridView.FocusedColumn.ReadOnly || !gridView.FocusedColumn.OptionsColumn.AllowEdit))
				{
					ShowObjectFromRow(showObjectControlEvent, gridView, objectEventArgs);
				}
			}
		};
		gridView.CustomUnboundColumnData += delegate(object sender, CustomColumnDataEventArgs e)
		{
			Icons.SetIcon(e, iconGridColumn, objectType);
		};
		if (deleteObjectBarButtonItem != null)
		{
			deleteObjectBarButtonItem.ItemClick += delegate
			{
				CommonFunctionsDatabase.DeleteSelectedObjectsDB(gridView, objectType, objectEventArgs, baseSummaryUserControl.MainControl, baseSummaryUserControl, owner);
			};
		}
		if (moveUpBarButtonItem != null)
		{
			moveUpBarButtonItem.ItemClick += delegate
			{
				ModuleSummaryHelper.MoveModuleUpOnGridView(gridView, baseSummaryUserControl.MainControl, owner);
			};
		}
		if (moveDownBarButtonItem != null)
		{
			moveDownBarButtonItem.ItemClick += delegate
			{
				ModuleSummaryHelper.MoveModuleDownOnGridView(gridView, baseSummaryUserControl.MainControl, owner);
			};
		}
		if (moveToTopBarButtonItem != null)
		{
			moveToTopBarButtonItem.ItemClick += delegate
			{
				ModuleSummaryHelper.MoveModuleToTopOnGrid(gridView, baseSummaryUserControl.MainControl, owner);
			};
		}
		if (moveToBottomBarButtonItem != null)
		{
			moveToBottomBarButtonItem.ItemClick += delegate
			{
				ModuleSummaryHelper.MoveModuleToBottomOnGrid(gridView, baseSummaryUserControl.MainControl, owner);
			};
		}
		if (sortalphabeticallyBarButtonItem != null)
		{
			sortalphabeticallyBarButtonItem.ItemClick += delegate
			{
				baseSummaryUserControl?.MainControl?.SortModulesAlphabetically(fromCustomFocus: false);
			};
		}
		if (gridPanel != null)
		{
			gridPanel.Delete += delegate
			{
				CommonFunctionsDatabase.DeleteSelectedObjectsDB(gridView, objectType, objectEventArgs, baseSummaryUserControl.MainControl, baseSummaryUserControl, owner);
			};
		}
		gridView.CellValueChanged += delegate(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
		{
			if (!gridView.Copy.IsCopying)
			{
				if (gridView.GetRow(e.RowHandle) is ObjectWithModulesObject)
				{
					ObjectWithModulesObject row = gridView.GetRow(e.RowHandle) as ObjectWithModulesObject;
					CommonFunctionsDatabase.UpdateObjectFromRow(gridView, row, objectType, repositoryItemCheckedComboBoxEdit, baseSummaryUserControl, customFieldsForHistory, owner);
					gridView.RefreshRow(gridView.FocusedRowHandle);
				}
				else if (gridView.GetRow(e.RowHandle) is ModuleWithoutDescriptionObject)
				{
					ModuleWithoutDescriptionObject moduleWithoutDescriptionObject = gridView.GetRow(e.RowHandle) as ModuleWithoutDescriptionObject;
					CommonFunctionsDatabase.UpdateObjectFromRow(gridView, moduleWithoutDescriptionObject, objectType, repositoryItemCheckedComboBoxEdit, baseSummaryUserControl, customFieldsForHistory, owner);
					HistoryModulesHelper.InsertHistoryCustomFieldsOnModuleSummary(gridView, baseSummaryUserControl, moduleWithoutDescriptionObject, objectType);
					HistoryModulesHelper.InsertHistoryTitleOnModuleSummary(gridView, moduleWithoutDescriptionObject, objectType);
					gridView.RefreshRow(gridView.FocusedRowHandle);
				}
				else if (gridView.GetRow(e.RowHandle) is IBasicData)
				{
					IBasicData basicData = gridView.GetRow(e.RowHandle) as IBasicData;
					if (basicData is TermObject)
					{
						TermObject termObject = basicData as TermObject;
						if (DB.BusinessGlossary.UpdateTerm(termObject, baseSummaryUserControl.CustomFieldsSupport, owner))
						{
							CustomFieldContainer customFieldContainer = new CustomFieldContainer(objectType, termObject.TermId.Value, baseSummaryUserControl.CustomFieldsSupport);
							customFieldContainer.RetrieveCustomFields(termObject);
							customFieldContainer.UpdateCustomFieldDefinitionValues(objectType, DB.CustomField.UpdateCustomFieldValues);
							DBTreeMenu.RefeshNodeTitle(termObject.TermId.Value, termObject.Title, objectType);
							HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTermSummary(gridView, baseSummaryUserControl, termObject, objectType);
							if (baseSummaryUserControl.MainControl.ShowProgress && ProgressColumnHelper.IsProgressColumnChanged(gridView.GetSelectedCells(), baseSummaryUserControl.MainControl.ProgressType.ColumnName) && baseSummaryUserControl.MainControl.ShowProgress)
							{
								baseSummaryUserControl.MainControl.RefreshObjectProgress(showWaitForm: false, termObject.TermId.Value, objectType);
							}
						}
					}
				}
			}
		};
		gridView.PopupMenuShowing += delegate(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
		{
			ManageOptionsInHeaderPopup(e);
		};
		gridView.PopupMenuShowing += delegate(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
		{
			popupMenu.ShowPopupMenu(gridView, e);
		};
		popupMenu.BeforePopup += delegate
		{
			if (!(baseSummaryUserControl is ModuleSummaryUserControl))
			{
				BarItemLink barItemLink = popupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Tag?.Equals("profileTable") ?? false);
				if (barItemLink != null)
				{
					barItemLink.Visible = false;
				}
				BarItemLink barItemLink2 = popupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Tag?.Equals("dataProfiling") ?? false);
				if (barItemLink2 != null)
				{
					barItemLink2.Visible = false;
				}
				BarItemLink barItemLink3 = popupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Tag?.Equals("previewSampleData") ?? false);
				if (barItemLink3 != null)
				{
					barItemLink3.Visible = false;
				}
				BarItemLink barItemLink4 = popupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Tag?.Equals("clearAllProfilingData") ?? false);
				if (barItemLink4 != null)
				{
					barItemLink4.Visible = false;
				}
				if (baseSummaryUserControl.ObjectType == SharedObjectTypeEnum.ObjectType.Database || baseSummaryUserControl.ObjectType == SharedObjectTypeEnum.ObjectType.Module || baseSummaryUserControl.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
				{
					List<object> selectedRows = new List<object>();
					int[] selectedRows2 = gridView.GetSelectedRows();
					foreach (int rowHandle in selectedRows2)
					{
						selectedRows.Add(gridView.GetRow(rowHandle));
					}
					if (baseSummaryUserControl.ObjectType == SharedObjectTypeEnum.ObjectType.Database || baseSummaryUserControl.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
					{
						string newBarButtonItemTag2 = "modules";
						BarItemLink barItemLink5 = popupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Tag?.Equals(newBarButtonItemTag2) ?? false);
						if (barItemLink5 != null)
						{
							int num = popupMenu.ItemLinks.IndexOf(barItemLink5);
							BarItemLink beforeLink = popupMenu.ItemLinks.ElementAtOrDefault(num + 1);
							popupMenu.ItemLinks.Remove(barItemLink5);
							BarSubItem barSubItem = new BarSubItem();
							barSubItem.Caption = "Assign subject area";
							barSubItem.ImageOptions.Image = Resources.module_16;
							barSubItem.Tag = newBarButtonItemTag2;
							barSubItem.Enabled = baseSummaryUserControl.GetDocumentationModules().Count() > 0;
							BarItem[] items = baseSummaryUserControl.GetModulesSubmenuBarButtonItems(barManager, selectedRows).ToArray();
							barSubItem.AddItems(items);
							barSubItem.Visibility = ((barSubItem.ItemLinks.Count <= 0) ? BarItemVisibility.Never : BarItemVisibility.Always);
							popupMenu.InsertItem(beforeLink, barSubItem);
						}
					}
					if (baseSummaryUserControl.ObjectType == SharedObjectTypeEnum.ObjectType.Database && (objectType == SharedObjectTypeEnum.ObjectType.Procedure || objectType == SharedObjectTypeEnum.ObjectType.Function))
					{
						BarItemLink barItemLink6 = popupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Tag?.Equals("design") ?? false);
						barItemLink6.ImageOptions.Image = IconsForButtonsFinder.ReturnImageForDesignButtonItem16(objectType);
						barItemLink6.Visible = gridView.GetSelectedRows().Count() == 1;
						HistoryGeneralHelper.AddHistoryBarButton(gridView, popupMenu);
					}
					if (baseSummaryUserControl.ObjectType == SharedObjectTypeEnum.ObjectType.Database || baseSummaryUserControl.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
					{
						string newBarButtonItemTag = "addToBusinessGlossaryTerm";
						BarItemLink barItemLink7 = popupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Tag?.Equals(newBarButtonItemTag) ?? false);
						if (barItemLink7 != null && selectedRows.Count > 0)
						{
							int num2 = popupMenu.ItemLinks.IndexOf(barItemLink7);
							BarItemLink beforeLink2 = popupMenu.ItemLinks.ElementAtOrDefault(num2 + 1);
							popupMenu.ItemLinks.Remove(barItemLink7);
							List<DBTreeNode> businessGlossaryNodes = baseSummaryUserControl.GetBusinessGlossaryNodes();
							if (businessGlossaryNodes.Count <= 1)
							{
								BarButtonItem barButtonItem = new BarButtonItem();
								barButtonItem.Caption = ((selectedRows.Count == 1) ? "Add new linked Business Glossary term" : "Add new linked Business Glossary terms");
								barButtonItem.ImageOptions.Image = Resources.term_add_16;
								barButtonItem.Tag = newBarButtonItemTag;
								barButtonItem.ItemClick += delegate
								{
									baseSummaryUserControl.AddNewBusinessGlossaryTerm(businessGlossaryNodes.FirstOrDefault()?.DatabaseId, selectedRows);
								};
								popupMenu.InsertItem(beforeLink2, barButtonItem);
							}
							else
							{
								BarSubItem barSubItem2 = new BarSubItem();
								barSubItem2.Caption = ((selectedRows.Count == 1) ? "Add new linked Business Glossary term in" : "Add new linked Business Glossary terms in");
								barSubItem2.ImageOptions.Image = Resources.term_add_16;
								barSubItem2.Tag = newBarButtonItemTag;
								IEnumerable<BarButtonItem> source = businessGlossaryNodes.Select(delegate(DBTreeNode x)
								{
									BarButtonItem barButtonItem5 = new BarButtonItem();
									barButtonItem5.ImageOptions.Image = Resources.term_add_16;
									barButtonItem5.Caption = x.TreeDisplayNameUiEscaped;
									barButtonItem5.Manager = barManager;
									barButtonItem5.ItemClick += delegate
									{
										baseSummaryUserControl.AddNewBusinessGlossaryTerm(x.DatabaseId, selectedRows);
									};
									return barButtonItem5;
								});
								BarItem[] items = source.ToArray();
								barSubItem2.AddItems(items);
								popupMenu.InsertItem(beforeLink2, barSubItem2);
							}
							ObjectWithModulesObject objectWithModulesObject = gridView.GetRow(gridView.FocusedRowHandle) as ObjectWithModulesObject;
							if (objectType == SharedObjectTypeEnum.ObjectType.Table && objectWithModulesObject != null)
							{
								popupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Tag?.Equals("design") ?? false).Visible = gridView.GetSelectedRows().Count() == 1 && !DB.Table.HasMultipleLevelColumns(objectWithModulesObject.Id);
							}
							if (objectWithModulesObject != null)
							{
								HistoryGeneralHelper.AddHistoryBarButton(gridView, popupMenu);
							}
							if (DataProfilingUtils.ObjectCanBeProfilled(objectType))
							{
								if (DB.DataProfiling.IsDataProfilingDisabled())
								{
									bool flag2 = (barItemLink2.Visible = false);
									bool flag4 = (barItemLink3.Visible = flag2);
									bool visible = (barItemLink4.Visible = flag4);
									barItemLink.Visible = visible;
								}
								else if (selectedRows.Count > 1)
								{
									bool flag4 = (barItemLink3.Visible = false);
									bool visible = (barItemLink4.Visible = flag4);
									barItemLink.Visible = visible;
									if ((from x in selectedRows.OfType<ObjectWithModulesObject>()
										where DataProfilingUtils.ObjectCanBeProfilled(x)
										select x).Count() != selectedRows.Count)
									{
										visible = (barItemLink4.Visible = false);
										barItemLink2.Visible = visible;
									}
									else
									{
										visible = (barItemLink4.Visible = true);
										barItemLink2.Visible = visible;
									}
								}
								else if (objectWithModulesObject != null && DataProfilingUtils.ObjectCanBeProfilled(objectWithModulesObject) && !DB.Table.HasMultipleLevelColumns(objectWithModulesObject.Id))
								{
									barItemLink2.Visible = false;
									barItemLink.Caption = DataProfilingUtils.GetButtonNameByObjectType(objectType);
									bool flag4 = (barItemLink3.Visible = gridView.GetSelectedRows().Count() == 1);
									bool visible = (barItemLink4.Visible = flag4);
									barItemLink.Visible = visible;
								}
								else
								{
									bool flag2 = (barItemLink2.Visible = false);
									bool flag4 = (barItemLink3.Visible = flag2);
									bool visible = (barItemLink4.Visible = flag4);
									barItemLink.Visible = visible;
								}
							}
						}
					}
					else if (baseSummaryUserControl.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
					{
						string newBarButtonItemName = "newBarButtonItem";
						BarItemLink barItemLink8 = popupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Name?.Equals(newBarButtonItemName) ?? false);
						HistoryGeneralHelper.AddHistoryBarButton(gridView, popupMenu);
						if (barItemLink8 != null)
						{
							int num3 = popupMenu.ItemLinks.IndexOf(barItemLink8);
							BarItemLink beforeLink3 = popupMenu.ItemLinks.ElementAtOrDefault(num3 + 1);
							popupMenu.ItemLinks.Remove(barItemLink8);
							if (baseSummaryUserControl.MainControl.BusinessGlossarySupport.TermTypes.Count > 1)
							{
								BarSubItem barSubItem3 = new BarSubItem(barManager, "New");
								barSubItem3.ImageOptions.Image = Resources.add_16;
								popupMenu.InsertItem(beforeLink3, barSubItem3);
								BarButtonItem[] barButtonItems = baseSummaryUserControl.MainControl.BusinessGlossarySupport.GetBarButtonItems();
								BarButtonItem[] array = barButtonItems;
								foreach (BarButtonItem barButtonItem2 in array)
								{
									AddAddNewTermToolStripMenuItemClickEvent(baseSummaryUserControl, barButtonItem2);
								}
								BarItem[] items = barButtonItems;
								barSubItem3.AddItems(items);
							}
							else if (baseSummaryUserControl.MainControl.BusinessGlossarySupport.TermTypes.Count == 1)
							{
								TermTypeObject termTypeObject = baseSummaryUserControl.MainControl.BusinessGlossarySupport.TermTypes[0];
								BarButtonItem barButtonItem3 = new BarButtonItem(barManager, "New");
								barButtonItem3.Name = newBarButtonItemName;
								barButtonItem3.Caption = "New " + termTypeObject.TitleAsSuffixWord;
								barButtonItem3.ImageOptions.Image = BusinessGlossarySupport.GetTermIcon(termTypeObject.IconId);
								barButtonItem3.Tag = termTypeObject;
								popupMenu.InsertItem(beforeLink3, barButtonItem3);
								AddAddNewTermToolStripMenuItemClickEvent(baseSummaryUserControl, barButtonItem3);
							}
							else
							{
								BarButtonItem barButtonItem4 = new BarButtonItem();
								barButtonItem4.Name = newBarButtonItemName;
								barButtonItem4.Caption = "New term";
								barButtonItem4.ImageOptions.Image = Resources.term_add_16;
								AddAddNewTermToolStripMenuItemClickEvent(baseSummaryUserControl, barButtonItem4);
								popupMenu.InsertItem(beforeLink3, barButtonItem4);
							}
						}
					}
				}
			}
		};
		gridView.ShownEditor += delegate
		{
			if (gridView.ActiveEditor is MemoEdit memoEdit)
			{
				memoEdit.TextChanged += Editor_TextChanged;
				memoEdit.Paint += Editor_Paint;
			}
		};
	}

	private static void AddAddNewTermToolStripMenuItemClickEvent(BaseSummaryUserControl baseSummaryUserControl, ToolStripMenuItem addNewTermToolStripMenuItem)
	{
		addNewTermToolStripMenuItem.Click += delegate(object addNewTermToolStripMenuItemSender, EventArgs addNewTermToolStripMenuItemEventArgs)
		{
			StartAddingTermForToolStripMenuItem(baseSummaryUserControl, addNewTermToolStripMenuItemSender);
		};
	}

	private static void StartAddingTermForToolStripMenuItem(BaseSummaryUserControl baseSummaryUserControl, object addNewTermToolStripMenuItemSender)
	{
		baseSummaryUserControl.MainControl.BusinessGlossarySupport.StartAddingTerm(baseSummaryUserControl.MainControl.TreeListHelpers.GetFocusedTreeListNode(), fromCustomFocus: false, (addNewTermToolStripMenuItemSender as ToolStripMenuItem).Tag as TermTypeObject, baseSummaryUserControl?.FindForm());
	}

	private static void AddAddNewTermToolStripMenuItemClickEvent(BaseSummaryUserControl baseSummaryUserControl, BarButtonItem barButtonItem)
	{
		barButtonItem.ItemClick += delegate
		{
			StartAddingTermForToolStripMenuItem(baseSummaryUserControl, barButtonItem.Tag as TermTypeObject);
		};
	}

	private static void StartAddingTermForToolStripMenuItem(BaseSummaryUserControl baseSummaryUserControl, TermTypeObject termTypeObject)
	{
		baseSummaryUserControl.MainControl.BusinessGlossarySupport.StartAddingTerm(baseSummaryUserControl.MainControl.TreeListHelpers.GetFocusedTreeListNode(), fromCustomFocus: false, termTypeObject, baseSummaryUserControl?.FindForm());
	}

	private static void Editor_Paint(object sender, PaintEventArgs e)
	{
		ControlPaint.DrawFocusRectangle(e.Graphics, e.ClipRectangle);
	}

	private static void Editor_TextChanged(object sender, EventArgs e)
	{
		if (sender is MemoEdit memoEdit && memoEdit.GetViewInfo() is MemoEditViewInfo memoEditViewInfo)
		{
			GraphicsCache graphicsCache = new GraphicsCache(memoEdit.CreateGraphics());
			int height = ((IHeightAdaptable)memoEditViewInfo).CalcHeight(graphicsCache, memoEditViewInfo.MaskBoxRect.Width);
			ObjectInfoArgs objectInfoArgs = new ObjectInfoArgs();
			objectInfoArgs.Bounds = new Rectangle(0, 0, memoEditViewInfo.ClientRect.Width, height);
			Rectangle rectangle = memoEditViewInfo.BorderPainter.CalcBoundsByClientRectangle(objectInfoArgs);
			graphicsCache.Dispose();
			memoEdit.Height = rectangle.Height;
		}
	}

	public static void SetBestFitForColumns(GridView gridView, IEnumerable<GridColumn> gridColumnsToOmit = null)
	{
		IEnumerable<GridColumn> enumerable = null;
		enumerable = ((gridColumnsToOmit != null) ? gridView.Columns.Except(gridColumnsToOmit) : gridView.Columns);
		gridView.BeginUpdate();
		if (gridView.RowCount <= 100)
		{
			foreach (GridColumn item in enumerable.Where((GridColumn x) => x.Visible && x.Tag != null))
			{
				if (item.FieldName == "Position")
				{
					item.Width = 20;
				}
				else
				{
					item.BestFit();
				}
			}
		}
		else if (gridView.RowCount <= 500)
		{
			foreach (GridColumn item2 in enumerable.Where((GridColumn x) => x.Visible && x.Tag != null))
			{
				if (item2.FieldName == "Position")
				{
					item2.Width = 40;
				}
				else
				{
					item2.BestFit();
				}
			}
		}
		else if (gridView.RowCount <= 1000)
		{
			foreach (GridColumn item3 in enumerable.Where((GridColumn x) => x.Visible && x.Tag != null))
			{
				if (item3.FieldName == "Position")
				{
					item3.Width = 40;
				}
				else
				{
					item3.BestFit();
				}
			}
		}
		else
		{
			foreach (GridColumn item4 in enumerable.Where((GridColumn x) => x.Visible && x.Tag != null))
			{
				if (item4.FieldName == "Position")
				{
					item4.Width = 40;
				}
				else if (item4.FieldName == "Name" || item4.FieldName == "FKTableObjectName" || item4.FieldName == "PKTableObjectName" || item4.FieldName == "name")
				{
					item4.Width = 200;
				}
				else if (item4.FieldName == "Title" || item4.FieldName == "title")
				{
					item4.Width = 200;
				}
				else if (item4.FieldName == "DataType")
				{
					item4.Width = 90;
				}
				else if (item4.FieldName == "ReferencesString")
				{
					item4.Width = 200;
				}
				else if (item4.FieldName == "Description")
				{
					item4.Width = 300;
				}
				else if (item4.FieldName == "JoinColumnsFormatted")
				{
					item4.Width = 200;
				}
				else if (item4.FieldName == "ColumnsStringFormatted")
				{
					item4.Width = 150;
				}
				else if (item4.FieldName == "WhenRun")
				{
					item4.Width = 150;
				}
				else if (item4.FieldName == "defaultComputedTableColumnsGridColumn")
				{
					item4.Width = 120;
				}
				else if (item4.FieldName == "ParameterMode")
				{
					item4.Width = 42;
				}
				else if (item4.FieldName == "subtype_display_text")
				{
					item4.Width = 105;
				}
				else if (item4.FieldName == "database_title")
				{
					item4.Width = 160;
				}
				else if (item4.FieldName == "schema")
				{
					item4.Width = 105;
				}
				else if (item4.FieldName == "modules_id")
				{
					item4.Width = 140;
				}
				else if (item4.FieldName == "ObjectDocumentationTitle")
				{
					item4.Width = 160;
				}
				else if (item4.FieldName == "ObjectFullNameWithTitle" || item4.FieldName == "ObjectNameWithSchemaAndTitle" || item4.FieldName == "ElementFullNameWithTitle")
				{
					item4.Width = 250;
				}
				else if (item4.FieldName == "ElementFullDataType")
				{
					item4.Width = 90;
				}
				else
				{
					item4.Width = 200;
				}
			}
		}
		gridView.EndUpdate();
	}

	public static void RefreshDataAndWidths(GridView gridView)
	{
		gridView.RefreshData();
		SetBestFitForColumns(gridView);
	}

	public static void ManageOptionsInHeaderPopup(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e, bool withLockUnlockOption = true)
	{
		if (e.MenuType != GridMenuType.Column)
		{
			return;
		}
		for (int num = e.Menu.Items.Count - 1; num >= 0; num--)
		{
			object tag = e.Menu.Items[num].Tag;
			if (GridStringId.MenuColumnGroupBox.Equals(tag) || GridStringId.MenuColumnGroup.Equals(tag) || GridStringId.MenuColumnFilterMode.Equals(tag) || GridStringId.MenuColumnFilterEditor.Equals(tag) || GridStringId.MenuColumnFindFilterShow.Equals(tag) || GridStringId.MenuColumnAutoFilterRowShow.Equals(tag) || GridStringId.MenuColumnAutoFilterRowHide.Equals(tag))
			{
				e.Menu.Items.RemoveAt(num);
			}
		}
		if (withLockUnlockOption)
		{
			ManageColumnFixedStyle(e);
		}
	}

	public static void ManageOptionsInHeaderPopup(DevExpress.XtraTreeList.PopupMenuShowingEventArgs e, TreeListHitInfo treeListHitInfo)
	{
		for (int num = e.Menu.Items.Count - 1; num >= 0; num--)
		{
			object tag = e.Menu.Items[num].Tag;
			if (TreeListStringId.MenuColumnFilterEditor.Equals(tag) || TreeListStringId.MenuColumnFindFilterShow.Equals(tag) || TreeListStringId.MenuColumnAutoFilterRowShow.Equals(tag) || TreeListStringId.MenuColumnAutoFilterRowHide.Equals(tag))
			{
				e.Menu.Items.RemoveAt(num);
			}
		}
		ManageColumnFixedStyle(e, treeListHitInfo);
	}

	public static void ManageOptionsInSchemaImportsAndChanges(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		if (e.MenuType != GridMenuType.Column)
		{
			return;
		}
		for (int num = e.Menu.Items.Count - 1; num >= 0; num--)
		{
			object tag = e.Menu.Items[num].Tag;
			if (GridStringId.MenuColumnGroupBox.Equals(tag) || GridStringId.MenuColumnGroup.Equals(tag) || GridStringId.MenuColumnFilterMode.Equals(tag) || GridStringId.MenuColumnFilterEditor.Equals(tag) || GridStringId.MenuColumnFindFilterShow.Equals(tag) || GridStringId.MenuColumnAutoFilterRowShow.Equals(tag) || GridStringId.MenuColumnAutoFilterRowHide.Equals(tag) || GridStringId.MenuColumnSortDescending.Equals(tag) || GridStringId.MenuColumnSortAscending.Equals(tag) || GridStringId.MenuColumnClearSorting.Equals(tag))
			{
				e.Menu.Items.RemoveAt(num);
			}
			if (GridStringId.MenuColumnBestFitAllColumns.Equals(tag))
			{
				e.Menu.Items[num].BeginGroup = true;
			}
		}
	}

	public static void ManageOptionsInTreelistHeaderPopup(DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
	{
		for (int num = e.Menu.Items.Count - 1; num >= 0; num--)
		{
			object tag = e.Menu.Items[num].Tag;
			if (TreeListStringId.MenuColumnSortAscending.Equals(tag) || TreeListStringId.MenuColumnSortDescending.Equals(tag) || TreeListStringId.MenuColumnClearSorting.Equals(tag))
			{
				e.Menu.Items.RemoveAt(num);
			}
		}
	}

	public static void ManageOptionsInHeaderPopupWithoutSorting(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		if (e.MenuType != GridMenuType.Column)
		{
			return;
		}
		for (int num = e.Menu.Items.Count - 1; num >= 0; num--)
		{
			object tag = e.Menu.Items[num].Tag;
			if (GridStringId.MenuColumnGroupBox.Equals(tag) || GridStringId.MenuColumnGroup.Equals(tag) || GridStringId.MenuColumnFilterMode.Equals(tag) || GridStringId.MenuColumnFilterEditor.Equals(tag) || GridStringId.MenuColumnFindFilterShow.Equals(tag) || GridStringId.MenuColumnAutoFilterRowShow.Equals(tag) || GridStringId.MenuColumnAutoFilterRowHide.Equals(tag) || GridStringId.MenuColumnSortAscending.Equals(tag) || GridStringId.MenuColumnSortDescending.Equals(tag))
			{
				e.Menu.Items.RemoveAt(num);
			}
		}
		ManageColumnFixedStyle(e);
	}

	public static void ManageOptionsInHeaderPopupWithoutSorting(DevExpress.XtraTreeList.PopupMenuShowingEventArgs e, TreeListCustomizationForm customizationForm)
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
				e.Menu.Items[num].Enabled = customizationForm == null;
			}
			if (TreeListStringId.MenuColumnSortAscending.Equals(tag) || TreeListStringId.MenuColumnSortDescending.Equals(tag) || TreeListStringId.MenuColumnBestFit.Equals(tag) || TreeListStringId.MenuColumnFilterEditor.Equals(tag) || TreeListStringId.MenuColumnFindFilterShow.Equals(tag) || TreeListStringId.MenuColumnAutoFilterRowShow.Equals(tag))
			{
				e.Menu.Items.RemoveAt(num);
			}
		}
	}

	public static void ManageBandedGridViewPopup(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		if (e.MenuType != GridMenuType.Column)
		{
			return;
		}
		for (int num = e.Menu.Items.Count - 1; num >= 0; num--)
		{
			object tag = e.Menu.Items[num].Tag;
			if (GridStringId.MenuColumnGroupBox.Equals(tag) || GridStringId.MenuColumnGroup.Equals(tag) || GridStringId.MenuColumnFilterMode.Equals(tag) || GridStringId.MenuColumnFilterEditor.Equals(tag) || GridStringId.MenuColumnFindFilterShow.Equals(tag) || GridStringId.MenuColumnAutoFilterRowShow.Equals(tag) || GridStringId.MenuColumnAutoFilterRowHide.Equals(tag) || GridStringId.MenuColumnSortAscending.Equals(tag) || GridStringId.MenuColumnSortDescending.Equals(tag) || GridStringId.MenuColumnRemoveColumn.Equals(tag) || GridStringId.MenuColumnBandCustomization.Equals(tag))
			{
				e.Menu.Items.RemoveAt(num);
			}
		}
	}

	private static void ManageColumnFixedStyle(DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		if (e.HitInfo.Column == null)
		{
			e.Menu.Items[0].BeginGroup = false;
		}
		else if (e.HitInfo.Column.Fixed == DevExpress.XtraGrid.Columns.FixedStyle.None)
		{
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Lock " + e.HitInfo.Column.Caption + " to the left";
			dXMenuItem.Tag = e.HitInfo.Column;
			dXMenuItem.Click += GridColumnItem_Click;
			e.Menu.Items.Add(dXMenuItem);
		}
		else
		{
			DXMenuItem dXMenuItem2 = new DXMenuItem();
			dXMenuItem2.Caption = "Unlock " + e.HitInfo.Column.Caption;
			dXMenuItem2.Tag = e.HitInfo.Column;
			dXMenuItem2.Click += GridColumnItemClear_Click;
			e.Menu.Items.Add(dXMenuItem2);
		}
	}

	private static void GridColumnItemClear_Click(object sender, EventArgs e)
	{
		((sender as DXMenuItem).Tag as GridColumn).Fixed = DevExpress.XtraGrid.Columns.FixedStyle.None;
	}

	private static void GridColumnItem_Click(object sender, EventArgs e)
	{
		((sender as DXMenuItem).Tag as GridColumn).Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
	}

	private static void ManageColumnFixedStyle(DevExpress.XtraTreeList.PopupMenuShowingEventArgs e, TreeListHitInfo treeListHitInfo)
	{
		if (treeListHitInfo.Column == null)
		{
			e.Menu.Items[0].BeginGroup = false;
		}
		else if (treeListHitInfo.Column.Fixed == DevExpress.XtraTreeList.Columns.FixedStyle.None)
		{
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Lock " + treeListHitInfo.Column.Caption + " to the left";
			dXMenuItem.Tag = treeListHitInfo.Column;
			dXMenuItem.Click += TreeListColumnItem_Click;
			e.Menu.Items.Add(dXMenuItem);
		}
		else
		{
			DXMenuItem dXMenuItem2 = new DXMenuItem();
			dXMenuItem2.Caption = "Unlock " + treeListHitInfo.Column.Caption;
			dXMenuItem2.Tag = treeListHitInfo.Column;
			dXMenuItem2.Click += TreeListColumnItemClear_Click;
			e.Menu.Items.Add(dXMenuItem2);
		}
	}

	private static void TreeListColumnItemClear_Click(object sender, EventArgs e)
	{
		((sender as DXMenuItem).Tag as TreeListColumn).Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.None;
	}

	private static void TreeListColumnItem_Click(object sender, EventArgs e)
	{
		((sender as DXMenuItem).Tag as TreeListColumn).Fixed = DevExpress.XtraTreeList.Columns.FixedStyle.Left;
	}

	public static void DeleteFromGrid(SharedObjectTypeEnum.ObjectType objectType, GridView gridView, XtraTabPage xtraTabPage, MetadataEditorUserControl metadataControl, IEdit edit)
	{
		if (CommonFunctionsDatabase.DeleteSelectedRows(gridView, objectType))
		{
			SetTabPageAndNodesTitle(isEdited: true, xtraTabPage, metadataControl.TreeListHelpers.GetFocusedDocumentationDBTreeNode(), metadataControl.GetFocusedNode());
			edit.SetEdited();
		}
	}

	public static void DeleteObjectsFromGrid(SharedObjectTypeEnum.ObjectType objectType, GridView gridView, XtraTabPage xtraTabPage, MetadataEditorUserControl metadataControl, IEdit edit, BindingList<int> deletedRelationsConstraintsRows)
	{
		if (CommonFunctionsDatabase.DeleteSelectedObjects(gridView, deletedRelationsConstraintsRows, objectType))
		{
			SetTabPageAndNodesTitle(isEdited: true, xtraTabPage, metadataControl.TreeListHelpers.GetFocusedDocumentationDBTreeNode(), metadataControl.GetFocusedNode());
			edit.SetEdited();
		}
	}

	public static void ColorDeletedRows(GridView gridView, RowCellStyleEventArgs e, bool isObject = false)
	{
		if (e.RowHandle >= 0 && ((!isObject) ? new SynchronizeStateEnum.SynchronizeState?(SynchronizeStateEnum.DBStringToState(gridView.GetRowCellValue(e.RowHandle, "status") as string)) : (gridView.GetRow(e.RowHandle) as BasicRow)?.Status) == SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.DeletedObjectForeColor;
		}
	}

	public static void AddEventsForDeleting(SharedObjectTypeEnum.ObjectType objectType, GridView gridView, XtraTabPage xtraTabPage, MetadataEditorUserControl metadataControl, IEdit edit, PopupMenu popupMenu, BarButtonItem deleteBarButtonItem, bool isObject, BindingList<int> deletedRelationsConstraintsRows = null, bool ommitDeletingEvents = false)
	{
		AddEventForColoringDeletedRows(gridView, isObject);
		if (ommitDeletingEvents)
		{
			return;
		}
		gridView.GridControl.ProcessGridKey += delegate(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && !gridView.IsEditing)
			{
				if (isObject)
				{
					DeleteObjectsFromGrid(objectType, gridView, xtraTabPage, metadataControl, edit, deletedRelationsConstraintsRows);
				}
				else
				{
					DeleteFromGrid(objectType, gridView, xtraTabPage, metadataControl, edit);
				}
			}
		};
		deleteBarButtonItem.ItemClick += delegate
		{
			if (isObject)
			{
				DeleteObjectsFromGrid(objectType, gridView, xtraTabPage, metadataControl, edit, deletedRelationsConstraintsRows);
			}
			else
			{
				DeleteFromGrid(objectType, gridView, xtraTabPage, metadataControl, edit);
			}
		};
	}

	public static void AddEventsForToolTips(ToolTipController constraintsToolTipController, List<ToolTipData> toolTipDataList)
	{
		constraintsToolTipController.GetActiveObjectInfo += delegate(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
		{
			ToolTips.ShowToolTip(e, toolTipDataList);
		};
	}

	public static void SetName(TextEdit label, XtraTabPage tabPage, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, string schema, string name, string title, SharedDatabaseTypeEnum.DatabaseType? databaseType, string databaseTitle, bool appendDatabaseTitle, bool withSchema, UserTypeEnum.UserType source)
	{
		SetName(label, tabPage, objectType, subtype, schema, name, title, databaseType, databaseTitle, appendDatabaseTitle, withSchema, source, null);
	}

	public static void SetName(TextEdit label, XtraTabPage tabPage, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, string schema, string name, string title, SharedDatabaseTypeEnum.DatabaseType? databaseType, string databaseTitle, bool appendDatabaseTitle, bool withSchema, UserTypeEnum.UserType source, object customInfo)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (appendDatabaseTitle)
		{
			stringBuilder.Append(databaseTitle).Append(".");
		}
		if (withSchema && !string.IsNullOrEmpty(schema))
		{
			stringBuilder.Append(schema).Append(".");
		}
		stringBuilder.Append(name).ToString();
		if (!string.IsNullOrEmpty(title) && objectType != SharedObjectTypeEnum.ObjectType.Term)
		{
			stringBuilder.AppendFormat(" ({0})", title);
		}
		label.Text = stringBuilder.ToString();
		label.ToolTip = ToolTips.GetNodeDescription(objectType, subtype, SynchronizeStateEnum.SynchronizeState.Synchronized, source);
		tabPage.Text = SharedObjectSubtypeEnum.TypeToStringForSingle(objectType, subtype);
		SetLabelIcon(label, objectType, subtype, source, customInfo);
	}

	private static void SetLabelIcon(TextEdit label, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, UserTypeEnum.UserType source, object customInfo)
	{
		if (objectType != SharedObjectTypeEnum.ObjectType.Term)
		{
			label.Properties.ContextImage = IconsSupport.GetObjectIcon(objectType, subtype, source, SynchronizeStateEnum.SynchronizeState.Synchronized);
		}
		else
		{
			label.Properties.ContextImage = BusinessGlossarySupport.GetTermIcon(customInfo as int?);
		}
	}

	public static void SetSummaryObjectTitle(LabelControl label, SharedObjectTypeEnum.ObjectType? objectType, string name, string title)
	{
		StringBuilder stringBuilder = new StringBuilder(name).Append(" in ");
		if (objectType.Equals(SharedObjectTypeEnum.ObjectType.Module))
		{
			stringBuilder.Append("subject area ");
		}
		label.Text = stringBuilder.Append(title).ToString();
	}

	public static void SetModuleColumnVisibility(GridColumn gridColumn, bool isInDatabase)
	{
		gridColumn.Visible = isInDatabase;
		gridColumn.OptionsColumn.ShowInCustomizationForm = isInDatabase;
	}

	public static void FillObjectEventArgs(ObjectEventArgs objectEventArgs, DBTreeNode node)
	{
		objectEventArgs.ModuleId = node.Id;
		objectEventArgs.ModuleName = node.ParentNode.Name;
	}

	public static void ShowObjectFromRow(EventHandler ShowControlEvent, GridView gridView, ObjectEventArgs objectEventArgs)
	{
		gridView.GetFocusedDataRow();
		if (gridView.GetFocusedRow() is ObjectWithModulesObject)
		{
			ObjectWithModulesObject objectWithModulesObject = gridView.GetFocusedRow() as ObjectWithModulesObject;
			ShowControlEvent?.Invoke(null, new ObjectEventArgs(objectEventArgs.DatabaseId, objectWithModulesObject.Id, -1));
		}
		else if (gridView.GetFocusedRow() is IBasicData)
		{
			IBasicData basicData = gridView.GetFocusedRow() as IBasicData;
			ShowControlEvent?.Invoke(null, new ObjectEventArgs(objectEventArgs.DatabaseId, basicData.Id ?? (-1), -1));
		}
	}

	public static void SetSelectedTabPage(XtraTabControl xtraTabControl, Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType? dependencyType = null)
	{
		xtraTabControl.TabPages.Count((XtraTabPage x) => x.PageVisible);
		string caption = SelectedTab.selectedTabCaption;
		if (dependencyType.HasValue)
		{
			switch (dependencyType)
			{
			case Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Trigger:
				caption = "Triggers";
				break;
			case Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation:
				caption = "Relationships";
				break;
			default:
				caption = "info";
				break;
			}
		}
		if (caption == null)
		{
			return;
		}
		if (caption.Equals("info"))
		{
			xtraTabControl.SelectedTabPageIndex = 0;
			return;
		}
		XtraTabPage xtraTabPage = xtraTabControl.TabPages.SingleOrDefault((XtraTabPage x) => x.PageVisible && x.Text.Equals(caption));
		if (xtraTabPage != null)
		{
			xtraTabControl.SelectedTabPage = xtraTabPage;
		}
	}

	public static void AddSubtypeDisplayText(IEnumerable<ObjectWithModulesObject> data, SharedObjectTypeEnum.ObjectType? objectType = null)
	{
		foreach (ObjectWithModulesObject datum in data)
		{
			SharedObjectTypeEnum.ObjectType? mainType = objectType ?? SharedObjectTypeEnum.StringToType(datum?.ObjectType?.ToString());
			SharedObjectSubtypeEnum.ObjectSubtype value = SharedObjectSubtypeEnum.StringToType(mainType, datum?.Subtype?.ToString());
			datum.SubtypeDisplayText = SharedObjectSubtypeEnum.TypeToStringForSingle(mainType, value);
		}
	}

	public static void AddSubtypeDisplayText(List<ObjectWithModulesObject> data, SharedObjectTypeEnum.ObjectType? objectType = null)
	{
		foreach (ObjectWithModulesObject datum in data)
		{
			SharedObjectTypeEnum.ObjectType? mainType = objectType ?? SharedObjectTypeEnum.StringToType(datum?.ObjectType?.ToString());
			SharedObjectSubtypeEnum.ObjectSubtype value = SharedObjectSubtypeEnum.StringToType(mainType, datum?.Subtype?.ToString());
			datum.SubtypeDisplayText = SharedObjectSubtypeEnum.TypeToStringForSingle(mainType, value);
		}
	}

	public static void AddEventForAutoFilterRow(GridView gridView)
	{
		gridView.CustomDrawCell += delegate(object s, RowCellCustomDrawEventArgs e)
		{
			if (e.RowHandle == -2147483646 && (e.Column.Name.Equals("iconTableColumnsGridColumn") || e.Column.Name.Equals("UniqueConstraintIcon") || e.Column.Name.Equals("iconTableRelationsGridColumn") || e.Column.Name.Equals("iconTableConstraintsGridColumn") || e.Column.Name.Equals("iconFunctionParametersGridColumn") || e.Column.Name.Equals("iconProcedureParametersGridColumn") || e.Column.Name.Equals("keyTableColumnsGridColumn") || e.Column.Name.Equals("iconTableTriggersGridColumn") || e.Column.Name.Equals("iconTableGridColumn") || e.Column.Name.Equals("iconGridColumn") || e.Column.Name.Equals("keyGridColumn") || e.Column.Name.Equals("termIconDataLinksGridColumn") || e.Column.Name.Equals("objectIconDataLinksGridColumn") || e.Column.Name.Equals("relatedTermIconGridColumn") || e.Column.Name.Equals("iconDataLinksGridColumn") || e.Column.Name.Equals("iconModuleGridColumn")))
			{
				Rectangle rect = new Rectangle(e.Bounds.X, e.Bounds.Y, 16, 16);
				e.Cache.DrawImage(Resources.blank_16, rect);
				e.Handled = true;
			}
		};
	}

	public static void AddEventForColoringDeletedRows(GridView gridView, bool isObject)
	{
		gridView.RowCellStyle += delegate(object sender, RowCellStyleEventArgs e)
		{
			ColorDeletedRows(gridView, e, isObject);
		};
	}
}
