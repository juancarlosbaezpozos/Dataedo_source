using System.Drawing;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Dataedo.App.Tools;

public class Icons
{
	public static void SetIcon(CustomColumnDataEventArgs e, GridColumn iconGridColumn, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!e.Column.Equals(iconGridColumn) || !e.IsGetData)
		{
			return;
		}
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Module:
			e.Value = Resources.module_16;
			break;
		case SharedObjectTypeEnum.ObjectType.Function:
		case SharedObjectTypeEnum.ObjectType.Procedure:
		case SharedObjectTypeEnum.ObjectType.Table:
		case SharedObjectTypeEnum.ObjectType.View:
		case SharedObjectTypeEnum.ObjectType.Structure:
		{
			ObjectWithModulesObject objectWithModulesObject = e.Row as ObjectWithModulesObject;
			e.Value = IconsSupport.GetObjectIcon(objectType, SharedObjectSubtypeEnum.StringToType(objectType, objectWithModulesObject.Subtype), UserTypeEnum.ObjectToType(objectWithModulesObject.Source) ?? UserTypeEnum.UserType.DBMS, CommonFunctionsDatabase.IsDeletedFromDB(e.Row) ? new SynchronizeStateEnum.SynchronizeState?(SynchronizeStateEnum.SynchronizeState.Deleted) : null);
			break;
		}
		case SharedObjectTypeEnum.ObjectType.Term:
			if (e.Row is TermObject)
			{
				e.Value = BusinessGlossarySupport.GetTermIcon((e.Row as TermObject)?.TypeIconId);
			}
			else if (e.Row is TermObjectWithRelationship)
			{
				e.Value = BusinessGlossarySupport.GetTermIcon((e.Row as TermObjectWithRelationship)?.TypeIconId);
			}
			break;
		case SharedObjectTypeEnum.ObjectType.Column:
		{
			ColumnRow columnRow = e.Row as ColumnRow;
			e.Value = IconsSupport.GetObjectIcon(columnRow.ObjectType, columnRow.ObjectSubtype, columnRow.Source, CommonFunctionsDatabase.IsDeletedFromDB(e.Row) ? new SynchronizeStateEnum.SynchronizeState?(SynchronizeStateEnum.SynchronizeState.Deleted) : null);
			break;
		}
		case SharedObjectTypeEnum.ObjectType.Trigger:
		{
			TriggerRow triggerRow = e.Row as TriggerRow;
			e.Value = IconsSupport.GetObjectIcon(objectType, triggerRow.Subtype, triggerRow.Source, CommonFunctionsDatabase.IsDeletedFromDB(e.Row) ? new SynchronizeStateEnum.SynchronizeState?(SynchronizeStateEnum.SynchronizeState.Deleted) : null, !triggerRow.Disabled);
			break;
		}
		case SharedObjectTypeEnum.ObjectType.Parameter:
		{
			ParameterRow obj = e.Row as ParameterRow;
			bool isDeleted = CommonFunctionsDatabase.IsDeletedFromDB(obj);
			ParameterRow.ModeEnum? mode = ParameterRow.GetMode(obj.ParameterMode);
			UserTypeEnum.UserType source = obj.Source;
			e.Value = GetParameterIcon(mode, source, isDeleted);
			break;
		}
		case SharedObjectTypeEnum.ObjectType.Relation:
			SetRelationIcon(e);
			break;
		case SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Module:
		case SharedObjectTypeEnum.ObjectType.Key:
		case SharedObjectTypeEnum.ObjectType.ColumnPK:
		case SharedObjectTypeEnum.ObjectType.UnresolvedEntity:
		case SharedObjectTypeEnum.ObjectType.Dependency:
		case SharedObjectTypeEnum.ObjectType.Script:
		case SharedObjectTypeEnum.ObjectType.Schema:
		case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
			break;
		}
	}

	public static Bitmap GetParameterIcon(ParameterRow.ModeEnum? mode, UserTypeEnum.UserType? source, bool isDeleted = false)
	{
		switch (mode)
		{
		case ParameterRow.ModeEnum.In:
			if (!isDeleted)
			{
				if (source != UserTypeEnum.UserType.USER)
				{
					return Resources.parameter_in_16;
				}
				return Resources.parameter_in_user_16;
			}
			return Resources.parameter_in_deleted_16;
		case ParameterRow.ModeEnum.Out:
			if (!isDeleted)
			{
				if (source != UserTypeEnum.UserType.USER)
				{
					return Resources.parameter_out_16;
				}
				return Resources.parameter_out_user_16;
			}
			return Resources.parameter_out_deleted_16;
		case ParameterRow.ModeEnum.InOut:
			if (!isDeleted)
			{
				if (source != UserTypeEnum.UserType.USER)
				{
					return Resources.parameter_inout_16;
				}
				return Resources.parameter_inout_user_16;
			}
			return Resources.parameter_inout_deleted_16;
		default:
			return Resources.blank_16;
		}
	}

	private static void SetRelationIcon(CustomColumnDataEventArgs e)
	{
		RelationRow relationRow = e.Row as RelationRow;
		if (relationRow.FKCardinality.Type == CardinalityTypeEnum.CardinalityType.Many && relationRow.PKCardinality.Type == CardinalityTypeEnum.CardinalityType.One)
		{
			if (relationRow.Source == UserTypeEnum.UserType.DBMS)
			{
				e.Value = (relationRow.CanBeDeleted() ? Resources.relation_deleted_24 : Resources.relation_mx_1x_24);
			}
			else
			{
				e.Value = Resources.relation_mx_1x_user_24;
			}
		}
		else if (relationRow.FKCardinality.Type == CardinalityTypeEnum.CardinalityType.One && relationRow.PKCardinality.Type == CardinalityTypeEnum.CardinalityType.Many)
		{
			if (relationRow.Source == UserTypeEnum.UserType.DBMS)
			{
				e.Value = (relationRow.CanBeDeleted() ? Resources.relation_deleted_24 : Resources.relation_1x_mx_24);
			}
			else
			{
				e.Value = Resources.relation_1x_mx_user_24;
			}
		}
		else if (relationRow.FKCardinality.Type == CardinalityTypeEnum.CardinalityType.One && relationRow.PKCardinality.Type == CardinalityTypeEnum.CardinalityType.One)
		{
			if (relationRow.Source == UserTypeEnum.UserType.DBMS)
			{
				e.Value = (relationRow.CanBeDeleted() ? Resources.relation_deleted_24 : Resources.relation_1x_1x_24);
			}
			else
			{
				e.Value = Resources.relation_1x_1x_user_24;
			}
		}
		else if (relationRow.FKCardinality.Type == CardinalityTypeEnum.CardinalityType.Many && relationRow.PKCardinality.Type == CardinalityTypeEnum.CardinalityType.Many)
		{
			if (relationRow.Source == UserTypeEnum.UserType.DBMS)
			{
				e.Value = (relationRow.CanBeDeleted() ? Resources.relation_deleted_24 : Resources.relation_mx_mx_24);
			}
			else
			{
				e.Value = Resources.relation_mx_mx_user_24;
			}
		}
		else
		{
			e.Value = new Bitmap(24, 16);
		}
	}

	public static Bitmap GetImageForContextMenu(BaseRow column, SharedObjectTypeEnum.ObjectType objectType)
	{
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Column:
			if (column.Source != UserTypeEnum.UserType.DBMS)
			{
				return Resources.column_user_16;
			}
			return Resources.column_16;
		case SharedObjectTypeEnum.ObjectType.Trigger:
			return Resources.trigger_active_16;
		case SharedObjectTypeEnum.ObjectType.Relation:
		{
			RelationRow relationRow = column as RelationRow;
			return SetIconDocForRelation(relationRow.Source == UserTypeEnum.UserType.USER, relationRow.PKCardinality, relationRow.FKCardinality);
		}
		case SharedObjectTypeEnum.ObjectType.Key:
		{
			UniqueConstraintRow uniqueConstraintRow = column as UniqueConstraintRow;
			return SetIconForConstraint(uniqueConstraintRow.Source == UserTypeEnum.UserType.USER, uniqueConstraintRow.IsPK, 16);
		}
		case SharedObjectTypeEnum.ObjectType.Parameter:
			return Resources.parameter_16;
		default:
			return null;
		}
	}

	public static Bitmap SetIconForConstraint(bool isUserDefined, bool isPk, int size)
	{
		string arg = (isPk ? "primary_key" : "unique_key");
		string arg2 = (isUserDefined ? "_user" : string.Empty);
		return Resources.ResourceManager.GetObject($"{arg}{arg2}_{size}") as Bitmap;
	}

	public static Bitmap SetIconDocForRelation(bool isUserDefined, Cardinality fkCardinality, Cardinality pkCardinality, bool pkRelations = false)
	{
		string text = (isUserDefined ? "user" : string.Empty);
		if (fkCardinality.Type.HasValue && pkCardinality.Type.HasValue)
		{
			string name = "relation_" + (pkRelations ? (fkCardinality.Id + "_" + pkCardinality.Id) : (pkCardinality.Id + "_" + fkCardinality.Id)) + (string.IsNullOrEmpty(text) ? string.Empty : ("_" + text)) + "_24";
			return Resources.ResourceManager.GetObject(name) as Bitmap;
		}
		return new Bitmap(64, 64);
	}

	public static Bitmap SetObjectIcon(string type, string subtype, bool isUserSource, bool isDeleted)
	{
		SharedObjectTypeEnum.ObjectType? mainType = SharedObjectTypeEnum.StringToType(type);
		return IconsSupport.GetObjectIcon(SharedObjectTypeEnum.StringToType(type), SharedObjectSubtypeEnum.StringToType(mainType, subtype?.ToUpper()), (!isUserSource) ? UserTypeEnum.UserType.DBMS : UserTypeEnum.UserType.USER, isDeleted ? new SynchronizeStateEnum.SynchronizeState?(SynchronizeStateEnum.SynchronizeState.Deleted) : null);
	}

	public static Bitmap SetIconForDependency(Dataedo.App.Data.MetadataServer.Model.DependencyRow row)
	{
		string text = ((row.DependencyCommonType != Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType.Relation) ? "_16" : "_24");
		Bitmap bitmap = Resources.ResourceManager.GetObject(row.FullTypeString + text) as Bitmap;
		if (bitmap == null)
		{
			bitmap = Resources.ResourceManager.GetObject(row.FullTypeString + text) as Bitmap;
		}
		if (bitmap == null)
		{
			bitmap = Resources.ResourceManager.GetObject(row.FullTypeStringForMainType + text) as Bitmap;
		}
		return bitmap;
	}

	public static void SetColumnPKIcon(CustomColumnDataEventArgs e, GridColumn iconGridColumn)
	{
		if (e.Column.Equals(iconGridColumn) && e.IsGetData && e.Row is ColumnRow columnRow)
		{
			e.Value = SetColumnPKIcon(columnRow.IsPk);
		}
	}

	public static Bitmap SetColumnPKIcon(bool isPk)
	{
		if (!isPk)
		{
			return Resources.blank_16;
		}
		return Resources.primary_key_16;
	}
}
