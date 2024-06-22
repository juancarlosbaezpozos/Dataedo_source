using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Dataedo.App.Classes.Synchronize.Tools;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Classes.Synchronize;

public class RelationRow : RelationContraintRow
{
	public SharedDatabaseTypeEnum.DatabaseType? FKTableDatabaseType;

	public SharedDatabaseTypeEnum.DatabaseType? PKTableDatabaseType;

	public int? CurrentTableId { get; set; }

	public bool ContextShowSchema { get; private set; }

	public string FKTableDatabaseName { get; set; }

	public bool? FKTableDatabaseMultipleSchemas { get; set; }

	public bool FKTableDatabaseShowSchema { get; set; }

	public string FKTableName { get; set; }

	public string FKTableSchema { get; set; }

	public string FKTableTitle { get; set; }

	public SharedObjectTypeEnum.ObjectType? FKObjectType { get; set; }

	public string PKTableDatabaseName { get; set; }

	public bool? PKTableDatabaseMultipleSchemas { get; set; }

	public bool PKTableDatabaseShowSchema { get; set; }

	public string PKTableName { get; set; }

	public string PKTableSchema { get; set; }

	public string PKTableTitle { get; set; }

	public SharedObjectTypeEnum.ObjectType? PKObjectType { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype? PKObjectSubtype { get; set; }

	public UserTypeEnum.UserType PKObjectSource { get; set; }

	public int OrdinalPosition { get; set; }

	public string UpdateRule { get; set; }

	public string DeleteRule { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype FKSubtype { get; set; }

	public UserTypeEnum.UserType? FKSource { get; set; }

	public ObjectStatusEnum.ObjectStatus FKStatus { get; set; }

	public int PKTableDatabaseId { get; set; }

	public int PKTableId { get; set; }

	public bool IsPKTableIdSet { get; set; }

	public int FKTableDatabaseId { get; set; }

	public int FKTableId { get; set; }

	public bool IsFKTableIdSet { get; set; }

	public Cardinality PKCardinality { get; set; } = new Cardinality();


	public Cardinality FKCardinality { get; set; } = new Cardinality();


	public string FullTypeString
	{
		get
		{
			if (FKCardinality.Type.HasValue && PKCardinality.Type.HasValue)
			{
				return "relationship-" + FKCardinality.Id + "-" + PKCardinality.Id + (string.IsNullOrEmpty(base.SourceString) ? string.Empty : ("-" + base.SourceString));
			}
			return string.Empty;
		}
	}

	public string RelationDescription => ToolTips.GetRelationDescription(this);

	public string CardinalityDescription
	{
		get
		{
			if (FKCardinality.Type.HasValue && PKCardinality.Type.HasValue)
			{
				if (base.Source != UserTypeEnum.UserType.USER)
				{
					return CardinalityTypeEnum.TypeToStringForDisplay(FKCardinality.Type) + " to " + CardinalityTypeEnum.TypeToStringForDisplay(PKCardinality.Type).ToLower();
				}
				return "User-defined " + CardinalityTypeEnum.TypeToStringForDisplay(FKCardinality.Type).ToLower() + " to " + CardinalityTypeEnum.TypeToStringForDisplay(PKCardinality.Type).ToLower();
			}
			return string.Empty;
		}
	}

	public string JoinCondition { get; set; }

	public BindingList<RelationColumnRow> Columns { get; set; } = new BindingList<RelationColumnRow>();


	public string JoinColumns
	{
		get
		{
			IEnumerable<string> enumerable = (from x in Columns?.Where((RelationColumnRow x) => !string.IsNullOrWhiteSpace(x.ColumnFkName) && !string.IsNullOrWhiteSpace(x.ColumnPkName))
				select x.ColumnFkFullName + " = " + x.ColumnPkFullName);
			if (enumerable != null)
			{
				return string.Join(Environment.NewLine, enumerable);
			}
			return string.Empty;
		}
	}

	public string JoinColumnsFormatted
	{
		get
		{
			IEnumerable<string> enumerable = (from x in Columns?.Where((RelationColumnRow x) => !string.IsNullOrWhiteSpace(x.ColumnFkName) && !string.IsNullOrWhiteSpace(x.ColumnPkName))
				select x.ColumnFkFullNameFormatted + " = " + x.ColumnPkFullNameFormatted);
			if (enumerable != null)
			{
				return string.Join(Environment.NewLine, enumerable);
			}
			return string.Empty;
		}
	}

	public string JoinColumnsFormattedWithTitles
	{
		get
		{
			IEnumerable<string> enumerable = (from x in Columns?.Where((RelationColumnRow x) => !string.IsNullOrWhiteSpace(x.ColumnFkName) && !string.IsNullOrWhiteSpace(x.ColumnPkName))
				select x.ColumnFkFullNameFormattedWithTitle + " = " + x.ColumnPkFullNameFormattedWithTitle);
			if (enumerable != null)
			{
				return string.Join(Environment.NewLine, enumerable);
			}
			return string.Empty;
		}
	}

	public bool IsReady
	{
		get
		{
			if (base.RowState == ManagingRowsEnum.ManagingRows.Unchanged || base.RowState == ManagingRowsEnum.ManagingRows.Deleted || (string.IsNullOrEmpty(base.Name) && string.IsNullOrEmpty(base.Description) && (Columns.Count == 0 || (Columns.Count == 1 && Columns[0].IsReady))) || Columns.Count == 0 || Columns.All((RelationColumnRow c) => c.IsReady))
			{
				return true;
			}
			return false;
		}
	}

	public bool IsEmpty
	{
		get
		{
			if (PKTableId == -1 && FKTableId == -1 && string.IsNullOrEmpty(base.Name))
			{
				return string.IsNullOrEmpty(base.Description);
			}
			return false;
		}
	}

	public Color RowColoring
	{
		get
		{
			if (!IsReady)
			{
				return Color.FromArgb(150, 255, 255, 224);
			}
			return Color.White;
		}
	}

	public virtual string PKTableObjectName => ObjectNames.GetTableObjectName(ContextShowSchema, CurrentTableId, PKTableDatabaseId, PKTableDatabaseShowSchema, PKTableDatabaseName, PKTableId, PKTableSchema, PKTableName, FKTableDatabaseId, useDatabaseName: true);

	public virtual string PKTableObjectNameWithTitle => ObjectNames.GetTableObjectName(ContextShowSchema, CurrentTableId, PKTableDatabaseId, PKTableDatabaseShowSchema, PKTableDatabaseName, PKTableId, PKTableSchema, PKTableName, PKTableTitle, FKTableDatabaseId, useDatabaseName: true);

	public virtual string PKTableObjectNameWithoutServer => ObjectNames.GetTableObjectName(ContextShowSchema, CurrentTableId, PKTableDatabaseId, PKTableDatabaseShowSchema, PKTableDatabaseName, PKTableId, PKTableSchema, PKTableName, FKTableDatabaseId, useDatabaseName: false);

	public virtual string PKTableObjectIdString => IdStrings.GetObjectIdString(PKTableDatabaseType, PKTableDatabaseMultipleSchemas, PKTableSchema, PKTableName, PKTableId);

	public virtual string PKTableObjectDatabaseIdString => IdStrings.GetDocumentationIdString(PKTableDatabaseName, PKTableDatabaseId);

	public string PKTableDisplayName
	{
		get
		{
			if (string.IsNullOrEmpty(PKTableTitle))
			{
				return PKTableObjectName;
			}
			return PKTableObjectName + " (" + PKTableTitle + ")";
		}
	}

	public string PKTableDisplayNameWithoutServer
	{
		get
		{
			if (string.IsNullOrEmpty(PKTableTitle))
			{
				return PKTableObjectNameWithoutServer;
			}
			return PKTableObjectNameWithoutServer + " (" + PKTableTitle + ")";
		}
	}

	public virtual string FKTableObjectName => ObjectNames.GetTableObjectName(ContextShowSchema, CurrentTableId, FKTableDatabaseId, FKTableDatabaseShowSchema, FKTableDatabaseName, FKTableId, FKTableSchema, FKTableName, PKTableDatabaseId, useDatabaseName: true);

	public virtual string FKTableObjectNameWithTitle => ObjectNames.GetTableObjectName(ContextShowSchema, CurrentTableId, FKTableDatabaseId, FKTableDatabaseShowSchema, FKTableDatabaseName, FKTableId, FKTableSchema, FKTableName, FKTableTitle, PKTableDatabaseId, useDatabaseName: true);

	public virtual string FKTableObjectNameWithoutServer => ObjectNames.GetTableObjectName(ContextShowSchema, CurrentTableId, FKTableDatabaseId, FKTableDatabaseShowSchema, FKTableDatabaseName, FKTableId, FKTableSchema, FKTableName, PKTableDatabaseId, useDatabaseName: false);

	public virtual string FKTableObjectIdString => IdStrings.GetObjectIdString(FKTableDatabaseType, FKTableDatabaseMultipleSchemas, FKTableSchema, FKTableName, FKTableId);

	public virtual string FKTableObjectDatabaseIdString => IdStrings.GetDocumentationIdString(FKTableDatabaseName, FKTableDatabaseId);

	public string FKTableDisplayName
	{
		get
		{
			if (string.IsNullOrEmpty(FKTableTitle))
			{
				return FKTableObjectName;
			}
			return FKTableObjectName + " (" + FKTableTitle + ")";
		}
	}

	public string FKTableDisplayNameWithoutServer
	{
		get
		{
			if (string.IsNullOrEmpty(FKTableTitle))
			{
				return FKTableObjectNameWithoutServer;
			}
			return FKTableObjectNameWithoutServer + " (" + FKTableTitle + ")";
		}
	}

	public string NotEmptyName
	{
		get
		{
			if (!string.IsNullOrEmpty(base.Name))
			{
				return base.Name;
			}
			return "User-defined relationship";
		}
	}

	public SharedObjectSubtypeEnum.ObjectSubtype PKSubtype { get; set; }

	public UserTypeEnum.UserType? PKSource { get; set; }

	public ObjectStatusEnum.ObjectStatus PKStatus { get; set; }

	public void Clear()
	{
		Columns.Clear();
		base.Source = UserTypeEnum.UserType.NotSet;
		PKTableId = -1;
		FKTableId = -1;
		base.Name = string.Empty;
		base.Title = string.Empty;
		base.Description = string.Empty;
	}

	public RelationRow(Link link)
	{
		base.Id = link.RelationId;
		PKTableId = link.FromNode.TableId;
		FKTableId = link.ToNode.TableId;
		base.Name = link.Name;
		base.Title = link.Title;
		base.Description = link.Description;
		base.Source = ((!link.UserRelation) ? UserTypeEnum.UserType.DBMS : UserTypeEnum.UserType.USER);
		Columns = new BindingList<RelationColumnRow>(link.Columns);
		FKCardinality = new Cardinality(link.ToNodeCardinality);
		PKCardinality = new Cardinality(link.FromNodeCardinality);
	}

	public RelationRow(RelationSynchronizationObject data, CustomFieldsSupport customFieldsSupport = null)
	{
		base.Name = data.Name;
		FKTableDatabaseName = data.FkTableDatabaseName;
		FKTableName = data.FkTableName;
		PKTableDatabaseName = data.RefTableDatabaseName;
		PKTableName = data.RefTableName;
		FKTableSchema = data.FkTableSchema;
		PKTableSchema = data.RefTableSchema;
		base.Description = data.Description;
		UpdateRule = data.UpdateRule;
		DeleteRule = data.DeleteRule;
		Columns = new BindingList<RelationColumnRow>();
		if (data is RelationSynchronizationObjectForOracle)
		{
			FKTableDatabaseMultipleSchemas = (data as RelationSynchronizationObjectForOracle).FkDatabaseMultipleSchema;
			PKTableDatabaseMultipleSchemas = (data as RelationSynchronizationObjectForOracle).RefDatabaseMultipleSchema;
		}
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(data);
		}
	}

	public void Initialize(RelationWithColumnAndUniqueConstraint row)
	{
		base.Id = row.RelationWithUniqueConstraint.TableRelationId;
		FKTableDatabaseId = row.FkTableObject.DatabaseId;
		FKTableDatabaseType = DatabaseTypeEnum.StringToType(row.FkTableObject.DatabaseType);
		FKTableDatabaseName = row.FkTableObject.DatabaseTitle;
		FKTableDatabaseMultipleSchemas = row.FkTableObject.DatabaseMultipleSchemas;
		FKTableDatabaseShowSchema = DatabaseRow.GetShowSchema(row.FkTableObject.DatabaseShowSchema, row.FkTableObject.DatabaseShowSchemaOverride, ContextShowSchema);
		FKTableId = row.FkTableObject.Id;
		FKTableName = row.FkTableObject.Name;
		FKTableSchema = row.FkTableObject.Schema;
		FKTableTitle = row.FkTableObject.Title;
		FKObjectType = SharedObjectTypeEnum.StringToType(row.FkTableObject.ObjectType);
		FKCardinality.Type = CardinalityTypeEnum.StringToType(row.FkType);
		FKSubtype = SharedObjectSubtypeEnum.StringToType(FKObjectType, row.FkTableObject.Subtype);
		FKSource = UserTypeEnum.ObjectToType(row.FkTableObject.Source);
		FKStatus = ObjectStatusEnum.GetStatusFromString(row.FkTableObject.Status);
		PKTableDatabaseId = row.PkTableObject.DatabaseId;
		PKTableDatabaseType = DatabaseTypeEnum.StringToType(row.PkTableObject.DatabaseType);
		PKTableDatabaseName = row.PkTableObject.DatabaseTitle;
		PKTableDatabaseMultipleSchemas = row.PkTableObject.DatabaseMultipleSchemas;
		PKTableDatabaseShowSchema = DatabaseRow.GetShowSchema(row.PkTableObject.DatabaseShowSchema, row.PkTableObject.DatabaseShowSchemaOverride, ContextShowSchema);
		PKTableId = row.PkTableObject.Id;
		PKTableName = row.PkTableObject.Name;
		PKTableSchema = row.PkTableObject.Schema;
		PKTableTitle = row.PkTableObject.Title;
		PKObjectType = SharedObjectTypeEnum.StringToType(row.PkTableObject.ObjectType);
		PKCardinality.Type = CardinalityTypeEnum.StringToType(row.PkType);
		PKSubtype = SharedObjectSubtypeEnum.StringToType(PKObjectType, row.PkTableObject.Subtype);
		PKSource = UserTypeEnum.ObjectToType(row.PkTableObject.Source);
		PKStatus = ObjectStatusEnum.GetStatusFromString(row.PkTableObject.Status);
		base.Source = UserTypeEnum.ObjectToType(row.Source) ?? UserTypeEnum.UserType.DBMS;
		base.Name = PrepareValue.ToString(row.Name);
		base.Title = PrepareValue.ToString(row.Title);
		base.Description = PrepareValue.ToString(row.Description);
	}

	public RelationRow(RelationWithColumnAndUniqueConstraint row, CustomFieldsSupport customFieldsSupport, bool contextShowSchema)
		: base(row.RelationWithUniqueConstraint, isEditingUserObjectsEnabled: true)
	{
		Initialize(row);
		ContextShowSchema = contextShowSchema;
		if (row.RelationWithUniqueConstraint.TableRelationColumnId.HasValue)
		{
			Columns = new BindingList<RelationColumnRow>
			{
				new RelationColumnRow(row.RelationWithUniqueConstraint, loadUniqueConstraintData: true)
			};
		}
		base.CustomFields = new CustomFieldContainer(SharedObjectTypeEnum.ObjectType.Relation, base.Id, customFieldsSupport);
		base.CustomFields.RetrieveCustomFields(row);
	}

	public RelationRow(RelationWithColumnAndUniqueConstraint row, bool isForXml, CustomFieldsSupport customFieldsSupport)
		: base(row.RelationWithUniqueConstraint)
	{
		Initialize(row);
		Columns = new BindingList<RelationColumnRow>
		{
			new RelationColumnRow(row.RelationWithUniqueConstraint, !isForXml)
		};
		base.CustomFields = new CustomFieldContainer(customFieldsSupport);
		base.CustomFields.RetrieveCustomFields(row);
	}

	public RelationRow(int tableId, CustomFieldsSupport customFieldsSupport)
		: base(isEditingUserObjectsEnabled: true)
	{
		base.Source = UserTypeEnum.UserType.NotSet;
		base.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
		FKTableId = -1;
		PKTableId = -1;
		Columns = new BindingList<RelationColumnRow>();
		FKCardinality.Type = CardinalityTypeEnum.CardinalityType.Many;
		PKCardinality.Type = CardinalityTypeEnum.CardinalityType.One;
		base.CustomFields = new CustomFieldContainer(customFieldsSupport);
		base.CustomFields.RetrieveCustomFields();
	}

	public new bool SetModified()
	{
		if (!string.IsNullOrEmpty(base.Name) && PKTableId != -1)
		{
			base.SetModified();
			return true;
		}
		return false;
	}

	public override bool CanBeDeleted()
	{
		if (base.IsEditable || base.Status != SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			if (base.IsEditable && base.Source == UserTypeEnum.UserType.USER)
			{
				return !IsEmpty;
			}
			return false;
		}
		return true;
	}

	public override bool IsDeletable()
	{
		if (base.Status != SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			return base.Source == UserTypeEnum.UserType.USER;
		}
		return true;
	}
}
