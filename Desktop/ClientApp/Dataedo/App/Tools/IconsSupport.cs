using System.Drawing;
using System.Linq;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Search;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraTreeList;

namespace Dataedo.App.Tools;

public static class IconsSupport
{
	public static string GetFullTypeString(SharedObjectTypeEnum.ObjectType? objectType)
	{
		return SharedObjectSubtypeEnum.TypeToString(objectType, null).ToString().ToLower();
	}

	public static string GetFullTypeString(SharedObjectTypeEnum.ObjectType? objectType, SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype, UserTypeEnum.UserType source, bool isDisabled, bool isActive)
	{
		return SharedObjectSubtypeEnum.TypeToString(objectType, objectSubtype).ToString().ToLower() + ((source == UserTypeEnum.UserType.USER) ? "_user" : string.Empty) + (isDisabled ? "_disabled" : ((objectType != SharedObjectTypeEnum.ObjectType.Trigger) ? string.Empty : "_active")) + (isActive ? string.Empty : "_deleted");
	}

	public static string GetFullTypeString(SharedObjectTypeEnum.ObjectType? objectType, SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype, UserTypeEnum.UserType source, SynchronizeStateEnum.SynchronizeState? synchronizeState, bool? isActive = null, string status = null, SharedDatabaseTypeEnum.DatabaseType? databaseType = null)
	{
		string text = SharedObjectSubtypeEnum.TypeToString(objectType, objectSubtype);
		if (text == "DATABASE")
		{
			text = (databaseType.HasValue ? (SharedDatabaseTypeEnum.TypeToString(databaseType).ToLowerInvariant() + "_16") : "DOCUMENTATION");
		}
		if (objectType == SharedObjectTypeEnum.ObjectType.Repository)
		{
			text = (StaticData.IsProjectFile ? ("FILE_" + text) : text);
		}
		if (objectType == SharedObjectTypeEnum.ObjectType.Column && (objectSubtype == SharedObjectSubtypeEnum.ObjectSubtype.Object || objectSubtype == SharedObjectSubtypeEnum.ObjectSubtype.Folder))
		{
			text = "COLUMN_" + text;
		}
		string text2 = null;
		if (synchronizeState.HasValue)
		{
			text2 = SynchronizeStateEnum.StateToStringForImage(synchronizeState.Value);
		}
		if (string.IsNullOrEmpty(text2) && isActive.HasValue)
		{
			text2 = ((isActive == true) ? "active" : "disabled");
		}
		string text3 = ((string.IsNullOrEmpty(status) || status.Equals("A")) ? string.Empty : "_deleted");
		return text.ToString().ToLower() + ((source == UserTypeEnum.UserType.USER && string.IsNullOrEmpty(text2)) ? "_user" : string.Empty) + (string.IsNullOrEmpty(text2) ? string.Empty : ("_" + text2)) + text3;
	}

	public static string GetFullTypeString(SharedObjectTypeEnum.ObjectType? objectType, string customObjectType, SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype, UserTypeEnum.UserType source, SynchronizeStateEnum.SynchronizeState? synchronizeState, bool? isActive = null, string status = null, SharedDatabaseTypeEnum.DatabaseType? databaseType = null)
	{
		if (!string.IsNullOrEmpty(customObjectType))
		{
			return customObjectType.ToLower();
		}
		return GetFullTypeString(objectType, objectSubtype, source, synchronizeState, isActive, status, databaseType);
	}

	public static string GetFullTypeString(ResultItem resultItem)
	{
		if (resultItem.ObjectType == SharedObjectTypeEnum.ObjectType.Term)
		{
			return BusinessGlossarySupport.GetTermIconName(resultItem.ObjectTypeId);
		}
		SharedObjectTypeEnum.ObjectType? objectType = resultItem.ObjectType;
		string customObjectType = resultItem.CustomObjectType;
		SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype = resultItem.ObjectSubtype;
		UserTypeEnum.UserType objectSource = resultItem.ObjectSource;
		SynchronizeStateEnum.SynchronizeState? synchronizeState = SynchronizeStateEnum.SynchronizeState.Synchronized;
		string objectStatus = resultItem.ObjectStatus;
		SharedDatabaseTypeEnum.DatabaseType? documentationType = resultItem.DocumentationType;
		return GetFullTypeString(objectType, customObjectType, objectSubtype, objectSource, synchronizeState, null, objectStatus, documentationType);
	}

	public static Bitmap GetObjectIcon(IGoTo goToObject)
	{
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(goToObject.ObjectType);
		return GetObjectIcon(objectType, SharedObjectSubtypeEnum.StringToType(objectType, goToObject.ObjectSubtype), UserTypeEnum.ObjectToType(goToObject.ObjectSource));
	}

	public static Bitmap GetObjectIcon(SharedObjectTypeEnum.ObjectType? objectType, SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype, UserTypeEnum.UserType source, SynchronizeStateEnum.SynchronizeState? synchronizeState, bool? isActive = null, SharedDatabaseTypeEnum.DatabaseType? databaseType = null)
	{
		Bitmap bitmap = Resources.ResourceManager.GetObject(GetFullTypeString(objectType, objectSubtype, source, synchronizeState, isActive, null, databaseType) + "_16") as Bitmap;
		if (bitmap == null)
		{
			bitmap = Resources.ResourceManager.GetObject(GetFullTypeString(objectType, null, source, synchronizeState, isActive, null, databaseType) + "_16") as Bitmap;
		}
		return bitmap ?? Resources.unresolved_16;
	}

	public static Bitmap GetObjectIcon(SharedObjectTypeEnum.ObjectType? objectType, SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype, UserTypeEnum.UserType? source)
	{
		return GetObjectIcon(objectType, objectSubtype, source ?? UserTypeEnum.UserType.DBMS, null);
	}

	public static Bitmap GetObjectIcon(string objectTypeString, string objectSubtypeString, string sourceString)
	{
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(objectTypeString);
		return GetObjectIcon(objectType, SharedObjectSubtypeEnum.StringToType(objectType, objectSubtypeString), UserTypeEnum.ObjectToType(sourceString));
	}

	public static Bitmap GetObjectIcon(string objectTypeString, string objectSubtypeString, string sourceString, string synchronizeState)
	{
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(objectTypeString);
		return GetObjectIcon(objectType, SharedObjectSubtypeEnum.StringToType(objectType, objectSubtypeString), UserTypeEnum.ObjectToType(sourceString) ?? UserTypeEnum.UserType.DBMS, SynchronizeStateEnum.DBStringToState(synchronizeState));
	}

	public static Bitmap GetObjectIcon(SharedObjectTypeEnum.ObjectType? objectType)
	{
		return GetObjectIcon(objectType, null, UserTypeEnum.UserType.NotSet, null);
	}

	public static void SetNodeImageIndex(ImageCollection treeMenuImageCollection, GetSelectImageEventArgs e, DependencyRow node)
	{
		Image image = null;
		if (node != null)
		{
			string text = node.FullTypeString + ((node.DependencyCommonType != DependencyRow.DependencyNodeCommonType.Relation) ? "_16" : "_24");
			if (!treeMenuImageCollection.Images.Keys.Contains(text))
			{
				text = node.FullTypeStringForMainType + ((node.DependencyCommonType != DependencyRow.DependencyNodeCommonType.Relation) ? "_16" : "_24");
			}
			image = ((!treeMenuImageCollection.Images.Keys.Contains(text)) ? treeMenuImageCollection.Images["unresolved_16"] : treeMenuImageCollection.Images[text]);
		}
		else
		{
			image = treeMenuImageCollection.Images["unresolved_16"];
		}
		int num = ((image != null) ? treeMenuImageCollection.Images.IndexOf(image) : (-1));
		if (num >= 0)
		{
			e.NodeImageIndex = num;
		}
	}

	public static void SetNodeImageIndex(ImageCollection treeMenuImageCollection, GetSelectImageEventArgs e, DBTreeNode dbTreeNode)
	{
		Image image = null;
		if (dbTreeNode != null)
		{
			string text = null;
			if (dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Term)
			{
				SharedObjectTypeEnum.ObjectType? objectType = dbTreeNode.ObjectType;
				SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype = dbTreeNode.Subtype;
				UserTypeEnum.UserType source = dbTreeNode.Source ?? UserTypeEnum.UserType.DBMS;
				SynchronizeStateEnum.SynchronizeState? synchronizeState = dbTreeNode.SynchronizeState;
				SharedDatabaseTypeEnum.DatabaseType? databaseType = dbTreeNode?.DatabaseType;
				text = GetFullTypeString(objectType, objectSubtype, source, synchronizeState, null, null, databaseType);
			}
			else
			{
				text = BusinessGlossarySupport.GetTermIconName(dbTreeNode.CustomInfo as int?);
			}
			if (!treeMenuImageCollection.Images.Keys.Contains(text))
			{
				SharedObjectTypeEnum.ObjectType? objectType2 = dbTreeNode.ObjectType;
				UserTypeEnum.UserType source2 = dbTreeNode.Source ?? UserTypeEnum.UserType.DBMS;
				SynchronizeStateEnum.SynchronizeState? synchronizeState2 = dbTreeNode.SynchronizeState;
				SharedDatabaseTypeEnum.DatabaseType? databaseType = dbTreeNode?.DatabaseType;
				text = GetFullTypeString(objectType2, null, source2, synchronizeState2, null, null, databaseType);
			}
			if (!treeMenuImageCollection.Images.Keys.Contains(text))
			{
				text = SharedObjectTypeEnum.TypeToString(dbTreeNode.ObjectType);
			}
			image = ((!treeMenuImageCollection.Images.Keys.Contains(text)) ? treeMenuImageCollection.Images["unresolved"] : treeMenuImageCollection.Images[text]);
		}
		else
		{
			image = treeMenuImageCollection.Images["unresolved"];
		}
		int num = ((image != null) ? treeMenuImageCollection.Images.IndexOf(image) : (-1));
		if (num >= 0)
		{
			e.NodeImageIndex = num;
		}
	}

	public static void SetNodeImageIndex(ImageCollection treeMenuImageCollection, GetSelectImageEventArgs e, ResultItem resultItem)
	{
		Image nodeImage = GetNodeImage(treeMenuImageCollection, resultItem);
		int num = ((nodeImage != null) ? treeMenuImageCollection.Images.IndexOf(nodeImage) : (-1));
		if (num >= 0)
		{
			e.NodeImageIndex = num;
		}
	}

	public static Image GetNodeImage(ImageCollection treeMenuImageCollection, ResultItem resultItem)
	{
		if (resultItem != null)
		{
			if (resultItem.Type == SharedObjectTypeEnum.ObjectType.Database)
			{
				return DatabaseSupportFactory.GetDatabaseSupportImage(resultItem.DocumentationType);
			}
			string text;
			if (!resultItem.ElementType.HasValue)
			{
				text = GetFullTypeString(resultItem);
				if (!treeMenuImageCollection.Images.Keys.Contains(text))
				{
					SharedObjectTypeEnum.ObjectType? objectType = resultItem.ObjectType;
					SynchronizeStateEnum.SynchronizeState? synchronizeState = SynchronizeStateEnum.SynchronizeState.Synchronized;
					SharedDatabaseTypeEnum.DatabaseType? documentationType = resultItem.DocumentationType;
					text = GetFullTypeString(objectType, null, UserTypeEnum.UserType.DBMS, synchronizeState, null, null, documentationType);
				}
			}
			else
			{
				text = SharedObjectSubtypeEnum.TypeToString(resultItem.ElementType, resultItem.ElementSubtype).ToLower();
				if (!treeMenuImageCollection.Images.Keys.Contains(text) || text.Equals("relation") || text.Equals("column"))
				{
					if (resultItem.ElementType == SharedObjectTypeEnum.ObjectType.Column)
					{
						string text2 = ((resultItem.Source == UserTypeEnum.UserType.USER) ? "_user_16" : string.Empty);
						string text3 = (resultItem.Status.Equals("D") ? "_deleted" : string.Empty);
						text = "column" + text2 + text3;
					}
					else if (resultItem.ElementType == SharedObjectTypeEnum.ObjectType.Key)
					{
						string obj = (resultItem.IsPrimaryKey ? "primary" : "unique");
						string text4 = (resultItem.Status.Equals("D") ? "_deleted" : string.Empty);
						string text5 = ((resultItem.Source == UserTypeEnum.UserType.USER) ? "_user" : string.Empty);
						text = obj + "_key" + text5 + text4;
					}
					else if (resultItem.ElementType == SharedObjectTypeEnum.ObjectType.Relation)
					{
						string text6 = ((resultItem.FkType == CardinalityTypeEnum.CardinalityType.Many) ? "_mx_1x" : "_1x_1x");
						string text7 = ((resultItem.Source == UserTypeEnum.UserType.USER) ? "_user" : string.Empty);
						string text8 = (resultItem.Status.Equals("D") ? "_deleted" : string.Empty);
						text = (text8.Equals("_deleted") ? ("relation" + text6 + text8 + "_16") : ("relation" + text6 + text7 + "_16"));
					}
					else if (resultItem.ElementType == SharedObjectTypeEnum.ObjectType.Trigger)
					{
						text += "_active";
						if (!treeMenuImageCollection.Images.Keys.Contains(text))
						{
							text = SharedObjectTypeEnum.TypeToString(resultItem.ElementType).ToLower() + "_active";
						}
					}
				}
			}
			if (!treeMenuImageCollection.Images.Keys.Contains(text))
			{
				text = SharedObjectTypeEnum.TypeToString(resultItem.ObjectType);
			}
			if (treeMenuImageCollection.Images.Keys.Contains(text))
			{
				return treeMenuImageCollection.Images[text];
			}
			return treeMenuImageCollection.Images["unresolved"];
		}
		return treeMenuImageCollection.Images["unresolved"];
	}

	public static Bitmap GetDatabaseIconByName16(SharedDatabaseTypeEnum.DatabaseType? sharedDatabaseTypeEnum, SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (objectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary)
		{
			return Resources.business_glossary_16;
		}
		if (!sharedDatabaseTypeEnum.HasValue)
		{
			return Resources.documentation_16;
		}
		return new Bitmap(DatabaseSupportFactory.GetDatabaseSupportImage(sharedDatabaseTypeEnum));
	}

	public static void SetTreeImage(SharedObjectTypeEnum.ObjectType objectType, SharedDatabaseTypeEnum.DatabaseType? databaseType, GetSelectImageEventArgs e, ImageCollection treeMenuImageCollection)
	{
		if (databaseType.HasValue)
		{
			Image image = treeMenuImageCollection.Images.ToArray().FirstOrDefault((Image x) => x.Tag is SharedDatabaseTypeEnum.DatabaseType databaseType2 && databaseType2 == databaseType);
			if (image == null)
			{
				Image databaseSupportImage = DatabaseSupportFactory.GetDatabaseSupportImage(databaseType);
				databaseSupportImage.Tag = objectType;
				e.NodeImageIndex = treeMenuImageCollection.Images.Add(databaseSupportImage);
			}
			else
			{
				e.NodeImageIndex = treeMenuImageCollection.Images.IndexOf(image);
			}
		}
	}
}
