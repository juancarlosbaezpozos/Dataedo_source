using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Classes.Synchronize.Tools;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Properties;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Erd;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Tools.ERD;

public class NodeColumnDB : CommonDBSupport, IRelationColumn, ISelectableColumn
{
	public int? NodeColumnId { get; set; }

	public bool Selected { get; set; }

	public int ColumnId { get; set; }

	public int? ParentId { get; set; }

	public int ModuleId { get; set; }

	public int? NodeId { get; set; }

	public int? OrdinalPosition { get; set; }

	public int TableId { get; set; }

	public string Name { get; set; }

	public string FullName => ColumnNames.GetFullName(Path, Name);

	public string FullNameFormatted => ColumnNames.GetFullNameFormatted(Path, Name);

	public string Path { get; set; }

	public string DisplayPath
	{
		get
		{
			if (!string.IsNullOrEmpty(Path))
			{
				return Path + ".";
			}
			return string.Empty;
		}
	}

	public int Level { get; set; }

	public string Title { get; set; }

	public bool PrimaryKey { get; set; }

	public Image PrimaryKeyIcon
	{
		get
		{
			if (!PrimaryKey)
			{
				return Resources.blank_16;
			}
			return Resources.primary_key_16;
		}
	}

	public string DataType { get; set; }

	public ColumnUniqueConstraintWithFkDataContainer UniqueConstraintsDataContainer { get; set; }

	public Image UniqueConstraintIcon => UniqueConstraintsDataContainer.FirstItemIcon;

	public string DisplayName
	{
		get
		{
			if (!string.IsNullOrEmpty(Title))
			{
				return Name + " (" + Title + ")";
			}
			return Name;
		}
	}

	public string DisplayNameForHtml => ColumnNames.GetFullNameFormattedForHtml(Path, Name, Title);

	public bool Focused { get; internal set; }

	public int? Sort { get; internal set; }

	public bool IsNullable { get; set; }

	public NodeColumnDB()
	{
		UniqueConstraintsDataContainer = new ColumnUniqueConstraintWithFkDataContainer();
		commands = StaticData.Commands;
	}

	public NodeColumnDB(ErdNodeColumnObject row, int moduleId)
		: this()
	{
		NodeColumnId = row.NodeColumnId;
		Selected = PrepareValue.ToBool(row.Selected);
		ColumnId = row.ColumnId;
		NodeId = row.NodeId;
		ModuleId = moduleId;
		OrdinalPosition = row.OrdinalPosition;
		TableId = row.TableId;
		Name = row.Name;
		Path = row.Path;
		Level = row.Level;
		PrimaryKey = PrepareValue.ToBool(row.PrimaryKey);
		DataType = row.DatatypeLen;
		Title = row.Title;
		ParentId = row.ParentId;
		Sort = row.Sort;
		IsNullable = row.IsNullable;
		ColumnUniqueConstraintWithFkData columnUniqueConstraintWithFkData = new ColumnUniqueConstraintWithFkData(row);
		if (columnUniqueConstraintWithFkData.HasData)
		{
			UniqueConstraintsDataContainer.Data.Add(columnUniqueConstraintWithFkData);
		}
	}

	public NodeColumnDB(NodeColumnDB column)
	{
		NodeColumnId = column.NodeColumnId;
		Selected = column.Selected;
		ColumnId = column.ColumnId;
		ModuleId = column.ModuleId;
		NodeId = column.NodeId;
		OrdinalPosition = column.OrdinalPosition;
		TableId = column.TableId;
		Name = column.Name;
		Path = column.Path;
		PrimaryKey = column.PrimaryKey;
		DataType = column.DataType;
		Title = column.Title;
		ParentId = column.ParentId;
		IsNullable = column.IsNullable;
		UniqueConstraintsDataContainer = column.UniqueConstraintsDataContainer;
	}

	public List<ErdNodeColumnObject> GetErdColumns(int moduleId, int tableId)
	{
		return commands.Select.Erd.GetErdNodeColumns(moduleId, tableId);
	}

	public List<ErdNodeColumn> GetExistingErdColumnsByModuleId(int moduleId)
	{
		return commands.Select.Erd.GetExistingErdColumnsByModuleId(moduleId);
	}

	public int InsertOrUpdateErdNodeColumns(IEnumerable<NodeColumnDB> columns, Form owner = null)
	{
		int? num = 0;
		string text = string.Empty;
		Exception ex = null;
		try
		{
			commands.Manipulation.Erd.InsertOrDeleteErdNodeColumns(ConvertToErdNodeColumns(columns));
		}
		catch (Exception ex2)
		{
			ex = ex2;
			text = ex2.Message;
			num = -1;
		}
		Messages.CheckAndShowErrorMessage(ex, "Error while updating database: " + text, num, owner);
		return Convert.ToInt32(num);
	}

	public int InsertOrUpdateErdNodeColumns(ErdNodeColumn[] columns, Form owner = null)
	{
		int? num = 0;
		string text = string.Empty;
		Exception ex = null;
		try
		{
			commands.Manipulation.Erd.InsertOrDeleteErdNodeColumns(columns);
		}
		catch (Exception ex2)
		{
			ex = ex2;
			text = ex2.Message;
			num = -1;
		}
		Messages.CheckAndShowErrorMessage(ex, "Error while updating database: " + text, num, owner);
		return Convert.ToInt32(num);
	}

	private ErdNodeColumn[] ConvertToErdNodeColumns(IEnumerable<NodeColumnDB> columns)
	{
		return columns.Select((NodeColumnDB x) => new ErdNodeColumn
		{
			Id = x.NodeColumnId,
			ModuleId = x.ModuleId,
			NodeId = x.NodeId,
			ColumnId = x.ColumnId,
			Selected = x.Selected
		}).ToArray();
	}
}
