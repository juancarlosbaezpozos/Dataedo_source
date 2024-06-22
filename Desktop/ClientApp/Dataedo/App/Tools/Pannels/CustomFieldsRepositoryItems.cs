using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools.Pannels;

public static class CustomFieldsRepositoryItems
{
	private static readonly string[] RepositoryItemTokenEditTokenSeparators = new string[1] { "," };

	public static RepositoryItem GetProperRepositoryItem(CustomFieldRowExtended customField, bool isForGrid, bool isForSummaryTable, string additionalJoinedValuesForClosedTypes)
	{
		RepositoryItem edit = null;
		if (customField.Type == CustomFieldTypeEnum.CustomFieldType.Text)
		{
			edit = GetTextRepositoryItem(isForSummaryTable);
		}
		else if (CustomFieldTypeEnum.IsTagType(customField.Type))
		{
			edit = GetTagsRepositoryItem(customField, additionalJoinedValuesForClosedTypes);
		}
		else if (customField.Type == CustomFieldTypeEnum.CustomFieldType.ListClosed)
		{
			edit = GetClosedListRepositoryItem(customField, additionalJoinedValuesForClosedTypes);
		}
		else if (customField.Type == CustomFieldTypeEnum.CustomFieldType.ListOpen)
		{
			edit = GetOpenListRepositoryItem(customField);
		}
		else if (customField.Type == CustomFieldTypeEnum.CustomFieldType.MultiValueListClosed)
		{
			edit = GetClosedMultiValueListRepositoryItem(customField, additionalJoinedValuesForClosedTypes);
		}
		else if (customField.Type == CustomFieldTypeEnum.CustomFieldType.Hyperlink)
		{
			edit = GetHyperlinkRepositoryItem();
		}
		else if (customField.Type == CustomFieldTypeEnum.CustomFieldType.Integer)
		{
			edit = GetIntegerRepositoryItem();
		}
		else if (customField.Type == CustomFieldTypeEnum.CustomFieldType.Date)
		{
			edit = GetDateRepositoryItem();
		}
		else if (customField.Type == CustomFieldTypeEnum.CustomFieldType.Checkbox)
		{
			edit = GetCheckboxRepositoryItem(isForGrid);
		}
		edit.EditValueChanging += delegate(object s, ChangingEventArgs e)
		{
			e.Cancel = !CheckValueWithMessage(e.NewValue, edit?.OwnerEdit?.FindForm());
		};
		if (!isForGrid)
		{
			edit.KeyDown += delegate(object s, KeyEventArgs e)
			{
				if (e.KeyCode == Keys.Tab)
				{
					try
					{
						GridControl gridControl = (s as Control).Parent as GridControl;
						CustomFieldsPanelControl customFieldsPanelControl = gridControl?.Parent?.Parent?.Parent?.Parent?.Parent as CustomFieldsPanelControl;
						if (e.Shift)
						{
							customFieldsPanelControl?.FocusPrevious(gridControl?.Parent?.Parent as CustomFieldControl);
						}
						else
						{
							customFieldsPanelControl?.FocusNext(gridControl?.Parent?.Parent as CustomFieldControl);
						}
					}
					catch
					{
					}
				}
			};
		}
		if (edit != null)
		{
			edit.Tag = customField;
		}
		return edit;
	}

	public static void RefreshEditOpenValues(RepositoryItem repositoryItem, CustomFieldRowExtended customField)
	{
		if (repositoryItem is RepositoryItemMRUEdit)
		{
			RepositoryItemMRUEdit obj = repositoryItem as RepositoryItemMRUEdit;
			UpdateOpenListRepositoryItems(obj, customField);
			obj.DropDownRows = GetPopupRowsCount(obj.Items.Count);
		}
	}

	private static RepositoryItem GetTextRepositoryItem(bool isForSummaryTable)
	{
		RepositoryItem repositoryItem = null;
		repositoryItem = ((!isForSummaryTable) ? ((RepositoryItemMemoEdit)new RepositoryItemAutoHeightMemoEdit
		{
			MaskBoxPadding = new Padding(-2, 2, 0, 0)
		}) : ((RepositoryItemMemoEdit)new RepositoryItemBaseAutoHeightMemoEdit()));
		LengthValidation.SetCustomFieldLength(repositoryItem as RepositoryItemTextEdit);
		return repositoryItem;
	}

	private static RepositoryItem GetTagsRepositoryItem(CustomFieldRowExtended customField, string additionalJoinedValuesForClosedTypes)
	{
		RepositoryItemTokenEdit repositoryItemTokenEdit = new RepositoryItemTokenEdit
		{
			TokenGlyphLocation = TokenEditGlyphLocation.Right
		};
		repositoryItemTokenEdit.ValidateToken += delegate(object s, TokenEditValidateTokenEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Description) || !string.IsNullOrEmpty(e.Value as string))
			{
				e.IsValid = true;
			}
			else
			{
				e.IsValid = false;
			}
		};
		if (customField.IsOpenDefinitionType)
		{
			repositoryItemTokenEdit.EditValueChanged += delegate(object s, EventArgs e)
			{
				string text = (s as TokenEdit)?.EditValue as string;
				if (!string.IsNullOrEmpty(text))
				{
					customField.UpdateAddedDefinitionValues(text);
				}
			};
			repositoryItemTokenEdit.Enter += delegate(object s, EventArgs e)
			{
				TokenEdit obj3 = s as TokenEdit;
				UpdateTagsRepositoryItemTags(obj3, customField, additionalJoinedValuesForClosedTypes);
				obj3.Refresh();
			};
		}
		repositoryItemTokenEdit.KeyDown += delegate(object s, KeyEventArgs e)
		{
			if (s is TokenEdit)
			{
				if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up || e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
				{
					try
					{
						((s as TokenEdit).Parent as GridControl)?.Focus();
						SendKeys.Send("{" + e.KeyCode.ToString().ToUpper() + "}");
						return;
					}
					catch
					{
						return;
					}
				}
				if (e.KeyCode == Keys.Return)
				{
					try
					{
						((s as TokenEdit).Parent as GridControl)?.Focus();
						SendKeys.Send("{ENTER}");
					}
					catch
					{
					}
				}
			}
		};
		repositoryItemTokenEdit.Separators.AddRange(RepositoryItemTokenEditTokenSeparators);
		UpdateTagsRepositoryItemTags(repositoryItemTokenEdit, customField, additionalJoinedValuesForClosedTypes);
		if (customField.Type == CustomFieldTypeEnum.CustomFieldType.TagsOpen)
		{
			repositoryItemTokenEdit.EditMode = TokenEditMode.Manual;
		}
		else if (customField.Type == CustomFieldTypeEnum.CustomFieldType.TagsClosed)
		{
			repositoryItemTokenEdit.EditMode = TokenEditMode.TokenList;
		}
		if (customField.Type == CustomFieldTypeEnum.CustomFieldType.TagsOpen || customField.Type == CustomFieldTypeEnum.CustomFieldType.TagsClosed)
		{
			repositoryItemTokenEdit.MaxTokenCount = -1;
		}
		return repositoryItemTokenEdit;
	}

	private static void UpdateTagsRepositoryItemTags(RepositoryItemTokenEdit edit, CustomFieldRowExtended customField, string additionalJoinedValuesForClosedTypes)
	{
		IEnumerable<string> source = (CustomFieldTypeEnum.IsClosedDefinitionType(customField.Type) ? customField.GetDefinitionValues(additionalJoinedValuesForClosedTypes, isSingleValue: false, addEmptyValue: false) : customField.GetDefinitionValues());
		edit.BeginUpdate();
		edit.Tokens.BeginUpdate();
		edit.Tokens.Clear();
		edit.Tokens.AddRange(source.Select((string x) => new TokenEditToken(x, x)));
		edit.Tokens.EndUpdate();
		edit.EndUpdate();
	}

	private static void UpdateTagsRepositoryItemTags(TokenEdit edit, CustomFieldRowExtended customField, string additionalJoinedValuesForClosedTypes)
	{
		IEnumerable<string> enumerable = BaseCustomFieldsSupport.SplitDefinitionValues(additionalJoinedValuesForClosedTypes);
		if (enumerable.Count() > 0 || customField.HasAddedDefinitionValues)
		{
			IEnumerable<string> source = (CustomFieldTypeEnum.IsClosedDefinitionType(customField.Type) ? customField.GetDefinitionValues(enumerable, addEmptyValue: false) : customField.GetDefinitionValues());
			edit.Properties.Tokens.BeginUpdate();
			edit.Properties.Tokens.Clear();
			edit.Properties.Tokens.AddRange(source.Select((string x) => new TokenEditToken(x, x)));
			edit.Properties.Tokens.EndUpdate();
		}
	}

	private static RepositoryItem GetClosedListRepositoryItem(CustomFieldRowExtended customField, string additionalJoinedValuesForClosedTypes)
	{
		IEnumerable<string> definitionValues = customField.GetDefinitionValues(additionalJoinedValuesForClosedTypes, isSingleValue: true, addEmptyValue: true);
		RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
		repositoryItemLookUpEdit.DataSource = definitionValues;
		repositoryItemLookUpEdit.NullText = string.Empty;
		repositoryItemLookUpEdit.ShowFooter = false;
		repositoryItemLookUpEdit.ShowHeader = false;
		repositoryItemLookUpEdit.ShowLines = false;
		repositoryItemLookUpEdit.DropDownRows = GetPopupRowsCount(definitionValues);
		repositoryItemLookUpEdit.Closed += delegate(object s, ClosedEventArgs e)
		{
			CloseEditor(s);
		};
		return repositoryItemLookUpEdit;
	}

	public static RepositoryItemMRUEdit GetClassificatorOpenListRepositoryItem(CustomFieldRowExtended field, GridView gridView)
	{
		RepositoryItemMRUEdit edit = new RepositoryItemMRUEdit
		{
			AllowRemoveMRUItems = false
		};
		if (field != null)
		{
			edit.Leave += delegate(object s, EventArgs e)
			{
				if (gridView.FocusedRowHandle >= 0)
				{
					string value = (s as MRUEdit)?.EditValue as string;
					if (!string.IsNullOrEmpty(value))
					{
						field.UpdateAddedDefinitionSingleValue(value);
					}
					UpdateOpenListRepositoryItems(edit, field);
					edit.DropDownRows = GetPopupRowsCount(edit.Items.Count);
				}
			};
			UpdateOpenListRepositoryItems(edit, field);
		}
		return edit;
	}

	private static RepositoryItem GetOpenListRepositoryItem(CustomFieldRowExtended customField)
	{
		RepositoryItemMRUEdit edit = new RepositoryItemMRUEdit
		{
			AllowRemoveMRUItems = false
		};
		edit.Leave += delegate(object s, EventArgs e)
		{
			string value = (s as MRUEdit)?.EditValue as string;
			if (!string.IsNullOrEmpty(value))
			{
				customField.UpdateAddedDefinitionSingleValue(value);
			}
			UpdateOpenListRepositoryItems(edit, customField);
			edit.DropDownRows = GetPopupRowsCount(edit.Items.Count);
		};
		UpdateOpenListRepositoryItems(edit, customField);
		return edit;
	}

	private static void UpdateOpenListRepositoryItems(RepositoryItemMRUEdit edit, CustomFieldRowExtended customField)
	{
		IEnumerable<string> singleDefinitionValues = customField.GetSingleDefinitionValues();
		edit.BeginUpdate();
		edit.Items.BeginUpdate();
		edit.Items.Clear();
		foreach (string item in singleDefinitionValues)
		{
			edit.Items.Add(item, insertAtTop: false);
		}
		edit.Items.EndUpdate();
		edit.EndUpdate();
	}

	private static RepositoryItem GetClosedMultiValueListRepositoryItem(CustomFieldRowExtended customField, string additionalJoinedValuesForClosedTypes)
	{
		IEnumerable<string> definitionValues = customField.GetDefinitionValues(additionalJoinedValuesForClosedTypes, isSingleValue: false, addEmptyValue: false);
		RepositoryItemCheckedComboBoxEdit edit = new RepositoryItemCheckedComboBoxEdit
		{
			NullText = string.Empty,
			DropDownRows = GetPopupRowsCount(definitionValues) + 1
		};
		edit.Items.AddRange(definitionValues.Select((string x) => new CheckedListBoxItem(x)).ToArray());
		edit.EditValueChanged += delegate(object s, EventArgs e)
		{
			if (s is CheckedComboBoxEdit checkedComboBoxEdit)
			{
				checkedComboBoxEdit.Properties.Items.BeginUpdate();
				checkedComboBoxEdit.Properties.Items.Clear();
				CheckedListBoxItem[] items = (from x in customField.GetDefinitionValues(checkedComboBoxEdit.EditValue as string, isSingleValue: false, addEmptyValue: false)
					select new CheckedListBoxItem(x)).ToArray();
				checkedComboBoxEdit.Properties.Items.AddRange(items);
				edit.DropDownRows = GetPopupRowsCount(definitionValues) + 1;
				checkedComboBoxEdit.Properties.Items.EndUpdate();
				checkedComboBoxEdit.Invalidate();
			}
		};
		edit.Popup += delegate(object s, EventArgs e)
		{
			IEnumerable<Control> enumerable = ((s as IPopupControl)?.PopupWindow as PopupContainerForm)?.Controls?.Cast<Control>();
			if (enumerable != null)
			{
				foreach (Control item in enumerable)
				{
					if (item is PopupContainerControl)
					{
						foreach (object item2 in item?.Controls)
						{
							if (item2 is CheckedListBoxControl)
							{
								(item2 as CheckedListBoxControl).IncrementalSearch = true;
								break;
							}
						}
						break;
					}
				}
			}
		};
		edit.Closed += delegate(object s, ClosedEventArgs e)
		{
			CloseEditor(s);
		};
		return edit;
	}

	private static RepositoryItem GetCheckboxRepositoryItem(bool isForGrid)
	{
		RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit
		{
			Caption = string.Empty,
			ValueChecked = "Y",
			ValueUnchecked = "N"
		};
		repositoryItemCheckEdit.EditValueChangedFiringMode = EditValueChangedFiringMode.Default;
		if (!isForGrid)
		{
			repositoryItemCheckEdit.GlyphAlignment = HorzAlignment.Near;
		}
		repositoryItemCheckEdit.KeyDown += delegate(object s, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Delete && s is CheckEdit checkEdit2)
			{
				checkEdit2.EditValue = null;
			}
		};
		repositoryItemCheckEdit.MouseDown += delegate(object s, MouseEventArgs e)
		{
			CheckEdit checkEdit = (CheckEdit)s;
			Rectangle glyphRect = ((CheckEditViewInfo)checkEdit.GetViewInfo()).CheckInfo.GlyphRect;
			Rectangle rectangle = new Rectangle(new Point(0, 0), checkEdit.Size);
			if (!glyphRect.Contains(e.Location) && rectangle.Contains(e.Location))
			{
				((DXMouseEventArgs)e).Handled = true;
			}
		};
		return repositoryItemCheckEdit;
	}

	private static RepositoryItem GetDateRepositoryItem()
	{
		return new RepositoryItemDateEdit();
	}

	private static RepositoryItem GetHyperlinkRepositoryItem()
	{
		RepositoryItemHyperLinkEdit repositoryItemHyperLinkEdit = new RepositoryItemHyperLinkEdit();
		EditorButton button = new EditorButton();
		repositoryItemHyperLinkEdit.Buttons.Add(button);
		repositoryItemHyperLinkEdit.ButtonClick += delegate
		{
		};
		return repositoryItemHyperLinkEdit;
	}

	private static RepositoryItem GetIntegerRepositoryItem()
	{
		return new RepositoryItemSpinEdit
		{
			IsFloatValue = false
		};
	}

	private static int GetPopupRowsCount(int itemsCount)
	{
		if (itemsCount >= 20)
		{
			return 20;
		}
		return itemsCount;
	}

	private static int GetPopupRowsCount(IEnumerable<string> values)
	{
		return GetPopupRowsCount(values.Count());
	}

	private static void CloseEditor(object sender)
	{
		((sender as BaseEdit)?.Parent as GridControl)?.DefaultView?.CloseEditor();
	}

	private static bool CheckValueWithMessage(object value, Form owner = null)
	{
		if (!LengthValidation.IsCustomFieldValueLenghtValid(value as string))
		{
			GeneralMessageBoxesHandling.Show("Value is too long.", "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return false;
		}
		return true;
	}
}
