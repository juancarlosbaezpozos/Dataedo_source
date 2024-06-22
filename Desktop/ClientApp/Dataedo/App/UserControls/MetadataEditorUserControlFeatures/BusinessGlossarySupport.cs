using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Forms;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.App.UserControls.WindowControls.MetadataEditorUserControlHelpers;
using Dataedo.CustomMessageBox;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.XtraBars;
using DevExpress.XtraRichEdit;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls.MetadataEditorUserControlFeatures;

public class BusinessGlossarySupport
{
	public class ObjectDefinition
	{
		public string Name { get; set; }

		public SharedObjectTypeEnum.ObjectType? ObjectType { get; set; }

		public int? ObjectId { get; set; }

		public int? ElementId { get; set; }

		public string Description { get; set; }

		public string DescriptionPlain { get; set; }

		public string ObjectTypeString => SharedObjectTypeEnum.TypeToString(ObjectType);

		public bool HasHtmlDescription
		{
			get
			{
				if (ObjectType != SharedObjectTypeEnum.ObjectType.Table && ObjectType != SharedObjectTypeEnum.ObjectType.View && ObjectType != SharedObjectTypeEnum.ObjectType.Procedure && ObjectType != SharedObjectTypeEnum.ObjectType.Function && ObjectType != SharedObjectTypeEnum.ObjectType.Structure)
				{
					return ObjectType == SharedObjectTypeEnum.ObjectType.Term;
				}
				return true;
			}
		}

		public ObjectDefinition(ObjectWithModulesObject row)
		{
			Name = (string.IsNullOrEmpty(row.Title) ? row.Name : row.Title);
			ObjectType = SharedObjectTypeEnum.StringToType(row.ObjectType);
			ObjectId = row.Id;
		}

		public ObjectDefinition(DBTreeNode node)
		{
			Name = (string.IsNullOrEmpty(node.Title) ? node.BaseName : node.Title);
			ObjectType = node.ObjectType;
			ObjectId = node.Id;
		}

		public ObjectDefinition(string name, SharedObjectTypeEnum.ObjectType? objectType, int? objectId, int? elementId)
		{
			Name = name;
			ObjectType = objectType;
			ObjectId = objectId;
			ElementId = elementId;
		}

		public void RetrieveDescriptions()
		{
			if (ObjectId.HasValue)
			{
				if (ObjectType == SharedObjectTypeEnum.ObjectType.Table || ObjectType == SharedObjectTypeEnum.ObjectType.View)
				{
					TableObject dataById = DB.Table.GetDataById(ObjectId.Value);
					string description = dataById.Description;
					string descriptionPlain = dataById.DescriptionPlain;
					Description = description;
					DescriptionPlain = descriptionPlain;
				}
				else if (ObjectType == SharedObjectTypeEnum.ObjectType.Column && ElementId.HasValue)
				{
					string text3 = (DescriptionPlain = (Description = PrepareValue.ToString(DB.Column.GetDataById(ObjectId.Value, ElementId.Value).Description)));
				}
			}
		}
	}

	public class ImageDefinition
	{
		public string Name { get; set; }

		public Bitmap Image { get; set; }

		public ImageDefinition(string name, Bitmap image)
		{
			Name = name;
			Image = image;
		}
	}

	private static readonly List<ImageDefinition> termIconsSmall;

	private static readonly List<ImageDefinition> termIconsLarge;

	private readonly MetadataEditorUserControl metadataEditorUserControl;

	private readonly ModuleSummaryUserControl moduleSummaryUserControl;

	private readonly DBTreeMenu dbTreeMenu;

	private readonly TreeList metadataTreeList;

	public List<TermTypeObject> TermTypes { get; protected set; }

	public List<TermRelationshipTypeObject> TermRelationshipTypes { get; protected set; }

	private TermUserControl VisibleTermUserControl => metadataEditorUserControl.GetVisibleUserControl() as TermUserControl;

	private TreeListHelpers TreeListHelpers => metadataEditorUserControl.TreeListHelpers;

	private SearchTreeNodeOperation SearchTreeNodeOperation => metadataEditorUserControl.SearchTreeNodeOperation;

	private void RefreshSearchDatabasesFromMetadataTreeList(int? id = null, string title = null)
	{
		metadataEditorUserControl.RefreshSearchDatabasesFromMetadataTreeList(id, title);
	}

	private bool ContinueAfterPossibleChanges(Action ifChangedNoCancelAction = null, Action ifChangedNoAction = null)
	{
		return metadataEditorUserControl.ContinueAfterPossibleChanges(ifChangedNoCancelAction, ifChangedNoAction);
	}

	private void SetControlsDuringAddingModuleEnabled(bool enabled)
	{
		metadataEditorUserControl.SetControlsDuringAddingModuleEnabled(enabled);
	}

	static BusinessGlossarySupport()
	{
		termIconsSmall = new List<ImageDefinition>
		{
			new ImageDefinition("term", Resources.term_16),
			new ImageDefinition("category", Resources.category_16),
			new ImageDefinition("rule", Resources.rule_16),
			new ImageDefinition("policy", Resources.policy_16)
		};
		termIconsLarge = new List<ImageDefinition>
		{
			new ImageDefinition("term", Resources.term_32),
			new ImageDefinition("category", Resources.category_32),
			new ImageDefinition("rule", Resources.rule_32),
			new ImageDefinition("policy", Resources.policy_32)
		};
	}

	public BusinessGlossarySupport(MetadataEditorUserControl metadataEditorUserControl, ModuleSummaryUserControl moduleSummaryUserControl, DBTreeMenu dbTreeMenu, TreeList metadataTreeList)
	{
		this.metadataEditorUserControl = metadataEditorUserControl;
		this.moduleSummaryUserControl = moduleSummaryUserControl;
		this.dbTreeMenu = dbTreeMenu;
		this.metadataTreeList = metadataTreeList;
	}

	public void LoadTermTypes()
	{
		try
		{
			TermTypes = (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary) ? DB.BusinessGlossary.GetTermTypes().ToList() : new List<TermTypeObject>());
		}
		catch (Exception)
		{
			TermTypes = new List<TermTypeObject>();
		}
	}

	public void LoadTermRelationshipTypes()
	{
		try
		{
			TermRelationshipTypes = (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary) ? DB.BusinessGlossary.GetTermRelationshipTypes() : new List<TermRelationshipTypeObject>());
		}
		catch (Exception)
		{
			TermRelationshipTypes = new List<TermRelationshipTypeObject>();
		}
	}

	public BarButtonItem[] GetTermTypesBarButtonItems(BarManager barManager, ItemClickEventHandler itemClick, bool smallIcons = true)
	{
		return metadataEditorUserControl.BusinessGlossarySupport.TermTypes.Select(delegate(TermTypeObject x)
		{
			BarButtonItem barButtonItem = new BarButtonItem(barManager, x.Title, 0);
			barButtonItem.Tag = x;
			barButtonItem.Glyph = GetTermIcon(x.IconId, smallIcons);
			barButtonItem.ItemClick += itemClick;
			return barButtonItem;
		}).ToArray();
	}

	public ToolStripMenuItem[] GetTermTypesToolStripMenuItems(EventHandler itemClick)
	{
		return metadataEditorUserControl.BusinessGlossarySupport.TermTypes.Select(delegate(TermTypeObject x)
		{
			ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem.Image = GetTermIcon(x.IconId);
			toolStripMenuItem.Text = x.Title;
			toolStripMenuItem.Tag = x;
			toolStripMenuItem.Click += itemClick;
			return toolStripMenuItem;
		}).ToArray();
	}

	public BarButtonItem[] GetBarButtonItems()
	{
		return metadataEditorUserControl.BusinessGlossarySupport.TermTypes.Select(delegate(TermTypeObject x)
		{
			BarButtonItem barButtonItem = new BarButtonItem();
			barButtonItem.ImageOptions.Image = GetTermIcon(x.IconId);
			barButtonItem.Caption = x.Title;
			barButtonItem.Tag = x;
			return barButtonItem;
		}).ToArray();
	}

	public void AddTermBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddTermFromContextMenu(e.Item.Tag as TermTypeObject);
	}

	public void AddTermRelatedTermBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddTermRelatedTermFromContextMenu(metadataEditorUserControl?.FindForm());
	}

	public static Bitmap GetTermIcon(int? iconId, bool smallIcons = true)
	{
		if (smallIcons)
		{
			return termIconsSmall[GetTermValidTermIconId(iconId)].Image;
		}
		return termIconsLarge[GetTermValidTermIconId(iconId)].Image;
	}

	public static string GetTermIconName(int? iconId, string suffix = "_16")
	{
		return termIconsSmall[GetTermValidTermIconId(iconId)].Name + suffix;
	}

	public static int GetTermValidTermIconId(int? iconId)
	{
		iconId = (iconId ?? 1) - 1;
		if (iconId >= termIconsSmall.Count || iconId < 0)
		{
			iconId = 0;
		}
		return iconId.Value;
	}

	private void AddTermFromContextMenu(TermTypeObject termTypeObject, Form owner = null)
	{
		moduleSummaryUserControl?.SetEmptyModulesListPanelVisibility(visible: false);
		StartAddingTerm(TreeListHelpers.GetFocusedTreeListNode(fromCustomFocus: true), fromCustomFocus: true, termTypeObject, owner);
	}

	private void AddTermRelatedTermFromContextMenu(Form owner = null)
	{
		moduleSummaryUserControl?.SetEmptyModulesListPanelVisibility(visible: false);
		if (StartAddingTermRelatedTerm(TreeListHelpers.GetFocusedTreeListNode(fromCustomFocus: true), fromCustomFocus: true, owner) == DialogResult.OK)
		{
			VisibleTermUserControl?.RefreshRelatedTerms(forceRefresh: true);
		}
	}

	public int? StartAddingNewBusinessGlossary(bool openNameEditor = true, Form owner = null)
	{
		InsertBusinessGlossary(out var title, out var databaseId, owner);
		if (databaseId.HasValue)
		{
			AddBusinessGlossaryNode(databaseId);
		}
		if (openNameEditor)
		{
			try
			{
				metadataTreeList.FocusedNode = SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes, title);
				metadataTreeList.OptionsBehavior.Editable = true;
				metadataTreeList.ShowEditor();
			}
			catch (Exception)
			{
			}
		}
		DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.BusinessGlossary, databaseId);
		WorkWithDataedoTrackingHelper.TrackNewGlossaryAdd();
		return databaseId;
	}

	private static void InsertBusinessGlossary(out string title, out int? databaseId, Form owner = null)
	{
		title = FindingNewName.GetNewName(!DB.Database.GetBusinessGlossaryNewBusinessGlossaryTitle(), "New Business Glossary", DB.Database.GetBusinessGlossaryNewTitle());
		databaseId = DB.BusinessGlossary.InsertBusinessGlossary(title, owner);
		DB.History.InsertHistoryRow(databaseId, databaseId, title, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.BusinessGlossary), saveTitle: true, saveDescription: false, SharedObjectTypeEnum.ObjectType.BusinessGlossary);
		DB.BusinessGlossary.UpdateBusinessGlossaryName(databaseId, owner);
	}

	public void AddBusinessGlossaryNode(int? databaseId)
	{
		if (databaseId.HasValue)
		{
			dbTreeMenu.AddBusinessGlossary(databaseId.Value, metadataEditorUserControl?.FindForm());
			metadataTreeList.FocusedNode = SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes, databaseId.Value);
			RefreshSearchDatabasesFromMetadataTreeList();
			metadataEditorUserControl.RebuildHomePage(forceReload: false);
		}
	}

	public void StartAddingTerm(TreeListNode callerNode, bool fromCustomFocus, TermTypeObject termTypeObject, Form owner = null)
	{
		if (callerNode == null || !Licenses.CheckRepositoryVersionAfterLogin() || !ContinueAfterPossibleChanges())
		{
			return;
		}
		DBTreeNode node = TreeListHelpers.GetNode(callerNode);
		DBTreeNode dBTreeNode = TreeListHelpers.GetNode(callerNode);
		DBTreeNode parentNode = dBTreeNode.ParentNode;
		if (parentNode == null || parentNode.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Database)
		{
			DBTreeNode parentNode2 = dBTreeNode.ParentNode;
			if (parentNode2 == null || parentNode2.ObjectType != SharedObjectTypeEnum.ObjectType.Term)
			{
				if (dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
				{
					dBTreeNode = dBTreeNode.Nodes.FirstOrDefault();
					callerNode.Expanded = true;
					callerNode = callerNode.Nodes.FirstOrDefault();
				}
				goto IL_009a;
			}
		}
		dBTreeNode = dBTreeNode.ParentNode;
		callerNode = callerNode.ParentNode;
		goto IL_009a;
		IL_009a:
		if (dBTreeNode == null || (dBTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Term && dBTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Database) || callerNode == null)
		{
			return;
		}
		metadataTreeList.CloseEditor();
		try
		{
			int? parentId = ((dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Term) ? new int?(dBTreeNode.Id) : null);
			string text = ((termTypeObject == null) ? "New term" : ("New " + termTypeObject.Title.ToLower()));
			string newName = FindingNewName.GetNewName(!DB.BusinessGlossary.CheckIfNewTermTitleExists(text), text, DB.BusinessGlossary.GetTitleExistingNumbers(text));
			int? num = DB.BusinessGlossary.InsertTerm(dBTreeNode.DatabaseId, parentId, newName, termTypeObject?.TermTypeId);
			DBTreeNode dBTreeNode2 = new DBTreeNode(dBTreeNode, num.Value, newName, SharedObjectTypeEnum.ObjectType.Term, "term", dBTreeNode.DatabaseId);
			DB.History.InsertHistoryRow(dBTreeNode.DatabaseId, num.Value, newName, null, null, "glossary_terms", !string.IsNullOrEmpty(newName), saveDescription: false, SharedObjectTypeEnum.ObjectType.Term);
			DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Term, num.Value);
			WorkWithDataedoTrackingHelper.TrackFirstInSessionTermAdd();
			dBTreeNode2.CustomInfo = termTypeObject?.IconId;
			if (dBTreeNode.Nodes.IndexOf(node) > -1)
			{
				dBTreeNode.Nodes.Insert(dBTreeNode.Nodes.IndexOf(node) + 1, dBTreeNode2);
			}
			else
			{
				dBTreeNode.Nodes.Insert(0, dBTreeNode2);
			}
			DBTreeMenu.RefreshFolderInDataBase(dBTreeNode);
			callerNode.Expanded = true;
			TreeListNode focusedNode = ((!fromCustomFocus) ? metadataEditorUserControl.SearchTreeNodeOperation.FindNode(callerNode.Nodes, dBTreeNode2.Id) : metadataEditorUserControl.SearchTreeNodeOperation.FindNode(callerNode.Nodes, dBTreeNode2.Id));
			TreeListHelpers.AggregateProgressUpIfProgressVisible(dBTreeNode2);
			metadataEditorUserControl.OpenPageControl(showControl: true, dBTreeNode2);
			metadataTreeList.FocusedNode = focusedNode;
			metadataTreeList.OptionsBehavior.Editable = true;
			metadataTreeList.ShowEditor();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding term.", owner);
			SetControlsDuringAddingModuleEnabled(enabled: true);
		}
	}

	public DialogResult StartAddingTermRelatedTerm(int rootTermId, int? termIdFilter, bool fromCustomFocus, Form owner = null)
	{
		try
		{
			if (!Licenses.CheckRepositoryVersionAfterLogin() || !ContinueAfterPossibleChanges())
			{
				return DialogResult.Abort;
			}
			AddTermRelationshipsForm addTermRelationshipsForm = new AddTermRelationshipsForm();
			addTermRelationshipsForm.SetParameters(rootTermId, termIdFilter, metadataEditorUserControl, metadataEditorUserControl.CustomFieldsSupport);
			TermRelationshipTypes = addTermRelationshipsForm.TermRelationshipTypes;
			VisibleTermUserControl?.SetTermRelationshipTypes();
			addTermRelationshipsForm.ShowDialog();
			return addTermRelationshipsForm.DialogResult;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
			return DialogResult.Abort;
		}
	}

	public DialogResult StartAddingTermRelatedTerm(TreeListNode node, int? termIdFilter, bool fromCustomFocus, Form owner = null)
	{
		DBTreeNode node2 = TreeListHelpers.GetNode(node);
		return StartAddingTermRelatedTerm(node2.Id, termIdFilter, fromCustomFocus, owner);
	}

	public DialogResult StartAddingTermRelatedTerm(TreeListNode node, bool fromCustomFocus, Form owner = null)
	{
		return StartAddingTermRelatedTerm(node, null, fromCustomFocus, owner);
	}

	public void AddNewBusinessGlossaryTerm(int? businessGlossaryId, Form owner, params ObjectDefinition[] selectedObjects)
	{
		if (selectedObjects.Count() == 0)
		{
			return;
		}
		bool flag = selectedObjects.Count() == 1;
		if (flag)
		{
			string text = PrepareTermName(selectedObjects.ElementAt(0));
			if (CustomMessageBoxForm.Show("Do you want to add <b>" + text + "</b> term?", "Add term", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
		}
		else if (CustomMessageBoxForm.Show($"Do you want to add <b>{selectedObjects.Count()}</b> terms?", "Add terms", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		BusinessGlossaryObject businessGlossaryObject = DB.BusinessGlossary.GetBusinessGlossaries(businessGlossaryId).FirstOrDefault();
		if (businessGlossaryObject == null)
		{
			int? documentationId = StartAddingNewBusinessGlossary(openNameEditor: false, owner);
			businessGlossaryObject = DB.BusinessGlossary.GetBusinessGlossaries(documentationId).FirstOrDefault();
		}
		if (businessGlossaryObject == null)
		{
			return;
		}
		TreeListNode treeListNode = metadataEditorUserControl.SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes, businessGlossaryObject.DocumentationId.Value, SharedObjectTypeEnum.ObjectType.BusinessGlossary);
		treeListNode.Expanded = true;
		TreeListNode treeListNode2 = treeListNode.Nodes.FirstOrDefault();
		DBTreeNode dBTreeNode = TreeListHelpers.GetDBTreeNode(treeListNode2);
		int? defaultTermTypeId = DB.BusinessGlossary.GetDefaultTermTypeId();
		RichEditControl richEditControl = new RichEditControl();
		richEditControl.Appearance.Text.Font = new Font("Segoe UI", 10f, FontStyle.Regular, GraphicsUnit.Point, 238);
		try
		{
			metadataTreeList.SuspendLayout();
			foreach (ObjectDefinition objectDefinition in selectedObjects)
			{
				objectDefinition.RetrieveDescriptions();
				string text2 = PrepareTermName(objectDefinition);
				if (objectDefinition.HasHtmlDescription)
				{
					richEditControl.HtmlText = objectDefinition.Description;
				}
				else
				{
					richEditControl.Text = objectDefinition.Description;
				}
				string text3 = richEditControl.HtmlText.Replace("font-family:Calibri;", "font-family:'Segoe UI';");
				int? num = null;
				if (string.IsNullOrEmpty(objectDefinition.Description))
				{
					num = DB.BusinessGlossary.InsertTerm(businessGlossaryObject.DocumentationId.Value, null, text2, defaultTermTypeId, null, null);
					DB.History.InsertHistoryRow(businessGlossaryObject.DocumentationId.Value, num, text2, null, null, "glossary_terms", !string.IsNullOrEmpty(text2), saveDescription: false, SharedObjectTypeEnum.ObjectType.Term);
				}
				else
				{
					num = DB.BusinessGlossary.InsertTerm(businessGlossaryObject.DocumentationId.Value, null, text2, defaultTermTypeId, text3, richEditControl.Text);
					DB.History.InsertHistoryRow(businessGlossaryObject.DocumentationId.Value, num, text2, text3, richEditControl.Text, "glossary_terms", !string.IsNullOrEmpty(text2), !string.IsNullOrEmpty(text3), SharedObjectTypeEnum.ObjectType.Term);
				}
				if (objectDefinition.ObjectType.HasValue && objectDefinition.ObjectId.HasValue)
				{
					DB.BusinessGlossary.InsertDataLink(new DataLinkObjectBase(num.Value, objectDefinition.ObjectTypeString, objectDefinition.ObjectId.Value, objectDefinition.ElementId));
					WorkWithDataedoTrackingHelper.TrackFirstInSessionTermAdd();
					DBTreeNode dBTreeNode2 = new DBTreeNode(dBTreeNode, num.Value, text2, SharedObjectTypeEnum.ObjectType.Term, "term", dBTreeNode.DatabaseId);
					dBTreeNode2.CustomInfo = defaultTermTypeId;
					dBTreeNode.Nodes.Add(dBTreeNode2);
					TreeListHelpers.AggregateProgressUpIfProgressVisible(dBTreeNode2);
					treeListNode2.Expanded = true;
					DBTreeMenu.RefreshFolderInDataBase(DBTreeMenu.FindMainObjectsFolder(dBTreeNode2));
					if (flag)
					{
						TreeListNode focusedNode = metadataEditorUserControl.SearchTreeNodeOperation.FindNode(treeListNode2.Nodes, dBTreeNode2.Id);
						metadataEditorUserControl.TreeListHelpers.LockFocus = false;
						metadataEditorUserControl.OpenPageControl(showControl: true, dBTreeNode2);
						metadataTreeList.FocusedNode = focusedNode;
						metadataTreeList.OptionsBehavior.Editable = true;
						metadataTreeList.ShowEditor();
						break;
					}
				}
			}
		}
		finally
		{
			richEditControl?.Dispose();
			richEditControl = null;
			metadataTreeList.ResumeLayout();
		}
	}

	private static string PrepareTermName(ObjectDefinition item)
	{
		try
		{
			string text = item.Name;
			if (text.ToUpper() == text)
			{
				text = text.ToLower();
			}
			text = Regex.Replace(text, "(^\\p{Lu}|_\\p{Lu}|^\\p{Ll}|_\\p{Ll}| \\p{Ll}| \\p{Lu})", (Match m) => m.ToString().Replace("_", " ").ToUpper());
			text = Regex.Replace(text, "(\\p{Ll})(\\p{Lu})", "$1 $2");
			text = Regex.Replace(text, "(\\p{Lu})(\\p{Lu}\\p{Ll})", "$1 $2");
			bool num = DB.BusinessGlossary.CheckIfNewTermTitleExists(text);
			List<string> titleExistingNumbers = DB.BusinessGlossary.GetTitleExistingNumbers(text);
			return FindingNewName.GetNewName(!num, text, titleExistingNumbers);
		}
		catch (Exception)
		{
			return item.Name;
		}
	}
}
