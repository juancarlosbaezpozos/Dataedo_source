using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using Dataedo.App.Classes.Synchronize.Tools;
using Dataedo.App.Properties;
using Dataedo.App.Tools.UI;
using Dataedo.DataProcessing.Classes;
using Dataedo.DBTConnector.Models;
using Dataedo.Model.Data.DdlScript;
using Dataedo.Model.Data.Neo4j;
using Dataedo.Model.Data.Salesforce;
using Dataedo.Model.Data.SSAS;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;
using DevExpress.XtraEditors.DXErrorProvider;

namespace Dataedo.App.Classes.Synchronize;

public class RelationColumnRow : RelationConstraintColumnRow, IColumn, IDXDataErrorInfo
{
	public bool RowGeneratedAutomatically { get; set; }

	public int TableRelationId { get; set; }

	public int TableFkId { get; set; }

	public int TablePkId { get; set; }

	public int ColumnFkId { get; set; }

	public int ColumnPkId { get; set; }

	public string ColumnFkName { get; set; }

	public string ColumnFkTitle { get; set; }

	public string ColumnFkPath { get; set; }

	public string ColumnFkFullNameFormatted => ColumnNames.GetFullNameFormatted(ColumnFkPath, ColumnFkName);

	public string ColumnFkFullNameFormattedWithTitle => ColumnNames.GetFullNameFormatted(ColumnFkPath, ColumnFkName, ColumnFkTitle);

	public string ColumnFkFullNameFormattedForHtml => ColumnNames.GetFullNameFormattedForHtml(ColumnFkPath, ColumnFkName);

	public string ColumnFkFullName => ColumnNames.GetFullName(ColumnFkPath, ColumnFkName);

	public string ColumnPkName { get; set; }

	public string ColumnPkTitle { get; set; }

	public string ColumnPkPath { get; set; }

	public string ColumnPkFullNameFormatted => ColumnNames.GetFullNameFormatted(ColumnPkPath, ColumnPkName);

	public string ColumnPkFullNameFormattedWithTitle => ColumnNames.GetFullNameFormatted(ColumnPkPath, ColumnPkName, ColumnPkTitle);

	public string ColumnPkFullNameFormattedForHtml => ColumnNames.GetFullNameFormattedForHtml(ColumnPkPath, ColumnPkName);

	public string ColumnPkFullName => ColumnNames.GetFullName(ColumnPkPath, ColumnPkName);

	public bool IsReady => !IsPartiallyReady;

	public bool IsPartiallyReady
	{
		get
		{
			if (ColumnPkId == -1 || ColumnFkId != -1)
			{
				if (ColumnPkId == -1)
				{
					return ColumnFkId != -1;
				}
				return false;
			}
			return true;
		}
	}

	public bool IsEmpty
	{
		get
		{
			if (ColumnPkId == -1)
			{
				return ColumnFkId == -1;
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
				return SkinsManager.CurrentSkin.GridAccentGridRowBackColor;
			}
			return SkinColors.ControlColorFromSystemColors;
		}
	}

	public Color RowColoringForeColor
	{
		get
		{
			if (!IsReady)
			{
				return SkinsManager.CurrentSkin.GridAccentGridRowForeColor;
			}
			return SkinColors.ControlForeColorFromSystemColors;
		}
	}

	public bool IsValidated { get; set; }

	public ColumnUniqueConstraintWithFkDataContainer FkUniqueConstraintsDataContainer { get; set; }

	public ColumnUniqueConstraintWithFkDataContainer PkUniqueConstraintsDataContainer { get; set; }

	public Image ColumnFkIcon => FkUniqueConstraintsDataContainer?.FirstItemIcon ?? Resources.blank_16;

	public Image ColumnPkIcon
	{
		get
		{
			object obj = PkUniqueConstraintsDataContainer?.FirstItemIcon;
			if (obj == null)
			{
				if (ColumnPkId == -1)
				{
					return Resources.blank_16;
				}
				obj = Resources.primary_key_16;
			}
			return (Image)obj;
		}
	}

	public void Clear()
	{
		ColumnPkId = -1;
		ColumnFkId = -1;
	}

	public RelationColumnRow()
	{
	}

	public RelationColumnRow(RelationColumnRow other)
	{
		base.Id = other.Id;
		base.OrdinalPosition = other.OrdinalPosition;
		TableRelationId = other.TableRelationId;
		TableFkId = other.TableFkId;
		TablePkId = other.TablePkId;
		ColumnFkId = other.ColumnFkId;
		ColumnPkId = other.ColumnPkId;
		FkUniqueConstraintsDataContainer = other.FkUniqueConstraintsDataContainer;
		PkUniqueConstraintsDataContainer = other.PkUniqueConstraintsDataContainer;
		ColumnFkName = other.ColumnFkName;
		ColumnFkTitle = other.ColumnFkTitle;
		ColumnFkPath = other.ColumnFkPath;
		ColumnPkName = other.ColumnPkName;
		ColumnPkTitle = other.ColumnPkTitle;
		ColumnPkPath = other.ColumnPkPath;
	}

	public RelationColumnRow(DbDataReader dataReader)
	{
		ColumnFkName = PrepareValue.ToString(dataReader["FK_COLUMN"]);
		ColumnPkName = PrepareValue.ToString(dataReader["REF_COLUMN"]);
		base.OrdinalPosition = PrepareValue.ToInt(dataReader["ORDINAL_POSITION"]);
		ColumnFkPath = PrepareValue.ReaderValueToString(dataReader, "fk_column_path");
		ColumnPkPath = PrepareValue.ReaderValueToString(dataReader, "ref_column_path");
	}

	public RelationColumnRow(Neo4jRelation dataReader)
	{
		ColumnFkName = PrepareValue.ToString(dataReader.FkTable);
		ColumnPkName = PrepareValue.ToString(dataReader.PkTable);
		base.OrdinalPosition = PrepareValue.ToInt(1);
	}

	public RelationColumnRow(SsasRelation dataReader)
	{
		ColumnFkName = PrepareValue.ToString(dataReader.FkColumn);
		ColumnPkName = PrepareValue.ToString(dataReader.RefColumn);
		base.OrdinalPosition = PrepareValue.ToInt(1);
	}

	public RelationColumnRow(SalesforceRelation dataReader)
	{
		ColumnFkName = PrepareValue.ToString(dataReader.FkColumn);
		ColumnPkName = PrepareValue.ToString(dataReader.RefColumn);
		base.OrdinalPosition = PrepareValue.ToInt(1);
	}

	public RelationColumnRow(DdlScriptRelation dataReader)
	{
		ColumnFkName = PrepareValue.ToString(dataReader.FkColumn);
		ColumnPkName = PrepareValue.ToString(dataReader.RefColumn);
		base.OrdinalPosition = PrepareValue.ToInt(1);
	}

	public RelationColumnRow(DataverseRelationWrapper dataReader)
	{
		if (dataReader.IsManyToMany)
		{
			ColumnFkName = PrepareValue.ToString(dataReader.ManyToManyRelation.Entity1IntersectAttribute);
			ColumnPkName = PrepareValue.ToString(dataReader.ManyToManyRelation.Entity2IntersectAttribute);
		}
		else
		{
			ColumnFkName = PrepareValue.ToString(dataReader.OneToManyRelation.ReferencingAttribute);
			ColumnPkName = PrepareValue.ToString(dataReader.OneToManyRelation.ReferencedAttribute);
		}
		base.OrdinalPosition = PrepareValue.ToInt(1);
	}

	public RelationColumnRow(RelationSynchronizationObject dataReader)
	{
		ColumnFkName = PrepareValue.ToString(dataReader.FkColumn);
		ColumnPkName = PrepareValue.ToString(dataReader.RefColumn);
		base.OrdinalPosition = PrepareValue.ToInt(dataReader.OridnalPosition);
	}

	public RelationColumnRow(RelationWithUniqueConstraintsObject row, bool loadUniqueConstraintData)
	{
		base.Id = row.TableRelationColumnId ?? (-1);
		TableRelationId = row.TableRelationId;
		TableFkId = row.TableFkId ?? (-1);
		ColumnFkId = row.ColumnFkId ?? (-1);
		ColumnFkName = row.ColumnFkName;
		ColumnFkTitle = row.ColumnFkTitle;
		ColumnFkPath = row.ColumnFkPath;
		TablePkId = row.TablePkId ?? (-1);
		ColumnPkId = row.ColumnPkId ?? (-1);
		ColumnPkName = row.ColumnPkName;
		ColumnPkTitle = row.ColumnPkTitle;
		ColumnPkPath = row.ColumnPkPath;
		base.OrdinalPosition = row.OrdinalPosition;
		base.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
		PkUniqueConstraintsDataContainer = new ColumnUniqueConstraintWithFkDataContainer();
		FkUniqueConstraintsDataContainer = new ColumnUniqueConstraintWithFkDataContainer();
		if (loadUniqueConstraintData)
		{
			ColumnUniqueConstraintWithFkData columnUniqueConstraintWithFkData = new ColumnUniqueConstraintWithFkData(row, null, UserTypeEnum.ObjectToType(row.RelationSource), isPk: true);
			if (columnUniqueConstraintWithFkData.HasData)
			{
				PkUniqueConstraintsDataContainer.Data.Add(columnUniqueConstraintWithFkData);
			}
			ColumnUniqueConstraintWithFkData columnUniqueConstraintWithFkData2 = new ColumnUniqueConstraintWithFkData(row, ColumnFkId, UserTypeEnum.ObjectToType(row.RelationSource), isPk: false);
			if (columnUniqueConstraintWithFkData2.HasData)
			{
				FkUniqueConstraintsDataContainer.Data.Add(columnUniqueConstraintWithFkData2);
			}
		}
	}

	public RelationColumnRow(int? maxOrdinalPosition)
	{
		ColumnFkId = -1;
		ColumnPkId = -1;
		ColumnPkName = string.Empty;
		base.OrdinalPosition = ((!maxOrdinalPosition.HasValue) ? new int?(1) : (maxOrdinalPosition + 1));
		base.Id = -base.OrdinalPosition.Value;
		base.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
		ColumnPkPath = string.Empty;
	}

	public RelationColumnRow(int? maxOrdinalPosition, IRelationColumn pkColumn, IRelationColumn fkColumn)
	{
		ColumnFkId = fkColumn?.ColumnId ?? (-1);
		ColumnPkId = pkColumn.ColumnId;
		ColumnPkName = pkColumn?.Name;
		ColumnPkTitle = pkColumn?.Title;
		ColumnFkName = fkColumn?.Name;
		ColumnFkTitle = fkColumn?.Title;
		ColumnPkPath = pkColumn?.Path;
		ColumnFkPath = fkColumn?.Path;
		base.OrdinalPosition = ((!maxOrdinalPosition.HasValue) ? new int?(1) : (maxOrdinalPosition + 1));
		base.Id = -base.OrdinalPosition.Value;
		base.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
		PkUniqueConstraintsDataContainer = pkColumn?.UniqueConstraintsDataContainer;
		FkUniqueConstraintsDataContainer = fkColumn?.UniqueConstraintsDataContainer;
		RowGeneratedAutomatically = true;
	}

	public RelationColumnRow(Relationship relationship)
	{
		ColumnFkName = PrepareValue.ToString(relationship.FKColumnName);
		ColumnPkName = PrepareValue.ToString(relationship.PKColumnName);
		base.OrdinalPosition = 1;
	}

	public static void AddEmpty(IList<RelationColumnRow> referencesColumnRows)
	{
		referencesColumnRows.Where((RelationColumnRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding).ToList().ForEach(delegate(RelationColumnRow x)
		{
			x.RowState = ManagingRowsEnum.ManagingRows.Added;
		});
		referencesColumnRows.Add(new RelationColumnRow(referencesColumnRows.Select((RelationColumnRow x) => x.OrdinalPosition).Max()));
	}

	public static void AddRelationRow(IList<RelationColumnRow> referencesColumnRows, IRelationColumn pkColumn, IRelationColumn fkColumn)
	{
		referencesColumnRows.Where((RelationColumnRow x) => x.RowState == ManagingRowsEnum.ManagingRows.ForAdding).ToList().ForEach(delegate(RelationColumnRow x)
		{
			x.RowState = ManagingRowsEnum.ManagingRows.Added;
		});
		referencesColumnRows.Add(new RelationColumnRow(referencesColumnRows.Select((RelationColumnRow x) => x.OrdinalPosition).Max(), pkColumn, fkColumn));
	}

	void IDXDataErrorInfo.GetPropertyError(string propertyName, ErrorInfo info)
	{
		if (IsReady || !IsValidated)
		{
			return;
		}
		if (propertyName == "ColumnFkId")
		{
			if (ColumnFkId == -1)
			{
				info.ErrorText = "FK column name is required";
				info.ErrorType = ErrorType.Critical;
			}
		}
		else if (propertyName == "ColumnPkId" && ColumnPkId == -1)
		{
			info.ErrorText = "PK column name is required";
			info.ErrorType = ErrorType.Critical;
		}
	}

	void IDXDataErrorInfo.GetError(ErrorInfo info)
	{
	}
}
