using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.DataSources.Base.DataProfiling.Model;
using Dataedo.DataSources.Enums;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.DataProfiling.Tools;

public static class DataProfilingUtils
{
	public static bool IsDataProfilingFormShown => Application.OpenForms["DataProfilingForm"] is DataProfilingForm;

	public static void ShowRowDistributionSparklines(GraphicsCache cache, Rectangle bounds, ColumnProfiledDataObject column)
	{
		int num = 6;
		int num2 = num / 2;
		double? num3 = (1.0 * (double)column.ValuesNullRowCount.GetValueOrDefault() / (double?)column.RowCount * 100.0).GetValueOrDefault();
		double? num4 = (1.0 * (double)column.ValuesEmptyRowCount.GetValueOrDefault() / (double?)column.RowCount * 100.0).GetValueOrDefault();
		double? num5 = (1.0 * (double)column.ValuesDistinctRowCount.GetValueOrDefault() / (double?)column.RowCount * 100.0).GetValueOrDefault();
		double? num6 = (1.0 * (double)column.ValuesNondistinctRowCount.GetValueOrDefault() / (double?)column.RowCount * 100.0).GetValueOrDefault();
		int num7 = (int)(((double)bounds.Width * num5 - (double)num) / 100.0).Value;
		int num8 = (int)(((double)bounds.Width * num6 - (double)num) / 100.0).Value;
		int num9 = (int)(((double)bounds.Width * num4 - (double)num) / 100.0).Value;
		int width = (int)(((double)bounds.Width * num3 - (double)num) / 100.0).Value;
		Color color = Color.FromArgb(255, 214, 132, 41);
		Color color2 = Color.FromArgb(255, 68, 114, 196);
		Color color3 = Color.FromArgb(255, 164, 164, 164);
		Color color4 = Color.FromArgb(255, 226, 226, 226);
		cache.FillRectangle(color, new Rectangle(bounds.Location.X, bounds.Location.Y + num2, num7, bounds.Height - num));
		cache.FillRectangle(color2, new Rectangle(bounds.Location.X + num7, bounds.Location.Y + num2, num8, bounds.Height - num));
		cache.FillRectangle(color3, new Rectangle(bounds.Location.X + num7 + num8, bounds.Location.Y + num2, num9, bounds.Height - num));
		cache.FillRectangle(color4, new Rectangle(bounds.Location.X + num9 + num7 + num8, bounds.Location.Y + num2, width, bounds.Height - num));
	}

	public static bool CheckIfDataProfilingFormAlreadyOpened(Form owner)
	{
		if (IsDataProfilingFormShown)
		{
			GeneralMessageBoxesHandling.Show("The Data Profiling form is already opened." + Environment.NewLine + Environment.NewLine + "To profile new objects close the opened form and try again.", "Profiling Form already opened", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
		}
		return IsDataProfilingFormShown;
	}

	public static bool CanViewDataProfilingForms(SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner)
	{
		DB.DataProfiling.SetSelectKillerSwitchMode();
		if (DB.DataProfiling.IsDataProfilingDisabled())
		{
			return false;
		}
		if (EnumToEnumChanger.IsDatabaseTypeNotSupportedForDataProfiling(databaseType))
		{
			GeneralMessageBoxesHandling.Show(SharedDatabaseTypeEnum.TypeToStringForDisplay(databaseType) + " databases are not supported for data profiling." + Environment.NewLine + "Browse the <href=https://dataedo.com/docs/supported-sources-data-profiling>list of the supported sources</href>.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, owner);
			return false;
		}
		return true;
	}

	public static bool NodeCanBeProfilled(DBTreeNode dbTreeNode)
	{
		if (dbTreeNode == null)
		{
			return false;
		}
		if ((dbTreeNode.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Synchronized || dbTreeNode.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized || dbTreeNode.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New) && dbTreeNode.Source == UserTypeEnum.UserType.DBMS)
		{
			if (dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Table)
			{
				return dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.View;
			}
			return true;
		}
		return false;
	}

	public static bool NodeFolderCanBeProfilled(DBTreeNode dbTreeNode)
	{
		if (dbTreeNode == null)
		{
			return false;
		}
		if (dbTreeNode.IsFolder)
		{
			if (!(dbTreeNode.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Table)))
			{
				return dbTreeNode.Name == SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.View);
			}
			return true;
		}
		return false;
	}

	public static bool ObjectCanBeProfilled(SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (!objectType.HasValue)
		{
			return false;
		}
		if (objectType != SharedObjectTypeEnum.ObjectType.Table)
		{
			return objectType == SharedObjectTypeEnum.ObjectType.View;
		}
		return true;
	}

	public static bool ObjectCanBeProfilled(ColumnRow columnRow)
	{
		if (columnRow == null)
		{
			return false;
		}
		if (columnRow.Status == SynchronizeStateEnum.SynchronizeState.Synchronized || columnRow.Status == SynchronizeStateEnum.SynchronizeState.Unsynchronized || columnRow.Status == SynchronizeStateEnum.SynchronizeState.New)
		{
			return columnRow.Source == UserTypeEnum.UserType.DBMS;
		}
		return false;
	}

	public static bool ObjectCanBeProfilled(TableObject tableObject)
	{
		if (tableObject == null)
		{
			return false;
		}
		if (!(tableObject.ObjectType == "TABLE"))
		{
			return tableObject.ObjectType == "VIEW";
		}
		return true;
	}

	public static bool ObjectCanBeProfilled(BaseDataObjectWithCustomFields baseDataObject)
	{
		if (baseDataObject == null)
		{
			return false;
		}
		if (!(baseDataObject.ObjectType == SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Table)))
		{
			return baseDataObject.ObjectType == SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.View);
		}
		return true;
	}

	public static string GetButtonNameByObjectType(SharedObjectTypeEnum.ObjectType? objectType)
	{
		return "Profile " + (objectType?.ToString() ?? "Object");
	}

	public static bool ObjectCanBeProfilled(ObjectWithModulesObject objectWithModules)
	{
		if (objectWithModules == null)
		{
			return false;
		}
		SynchronizeStateEnum.SynchronizeState synchronizeState = SynchronizeStateEnum.DBStringToState(objectWithModules.Status);
		if ((synchronizeState == SynchronizeStateEnum.SynchronizeState.Synchronized || synchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized || synchronizeState == SynchronizeStateEnum.SynchronizeState.New) && objectWithModules.Source == UserTypeEnum.TypeToString(UserTypeEnum.UserType.DBMS))
		{
			if (!(objectWithModules.ObjectType == SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Table)))
			{
				return objectWithModules.ObjectType == SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.View);
			}
			return true;
		}
		return false;
	}

	public static void PopulateSampleDataGridFromDownloadedSampleData<T>(List<T> columns, GridControl sampleDataGridControl, GridView sampleDataGridControlGridView, SampleData sampleDataFromTableProfiling, ProfilingDatabaseTypeEnum.ProfilingDatabaseType profilingDatabaseType) where T : IColumnBase
	{
		sampleDataGridControlGridView.OptionsSelection.MultiSelect = false;
		sampleDataGridControlGridView.OptionsBehavior.AutoPopulateColumns = false;
		sampleDataGridControlGridView.OptionsBehavior.Editable = false;
		sampleDataGridControlGridView.OptionsView.ColumnAutoWidth = false;
		sampleDataGridControlGridView.Columns.Clear();
		GridColumn[] source = PrepareColumnsForSampleData(columns, profilingDatabaseType);
		List<object> dataSource = PrepareDataSourceForSampleData(columns, sampleDataFromTableProfiling, profilingDatabaseType);
		sampleDataGridControl.BeginUpdate();
		sampleDataGridControlGridView.BeginUpdate();
		sampleDataGridControlGridView.Columns.AddRange(source.ToArray());
		sampleDataGridControl.DataSource = dataSource;
		sampleDataGridControl.EndUpdate();
		sampleDataGridControlGridView.EndUpdate();
		BestFitColumnsWhenGridsAreNotDisposed(sampleDataGridControl, sampleDataGridControlGridView);
	}

	private static List<dynamic> PrepareDataSourceForSampleData<T>(List<T> columns, SampleData sampleDataFromTableProfiling, ProfilingDatabaseTypeEnum.ProfilingDatabaseType profilingDatabaseType) where T : IColumnBase
	{
		int num = 0;
		int valueOrDefault = (sampleDataFromTableProfiling?.Values?.Count()).GetValueOrDefault();
		List<object> list = new List<object>();
		while (list.Count < 10 && valueOrDefault > num)
		{
			ExpandoObject expandoObject = new ExpandoObject();
			int num2 = 0;
			for (int i = 0; i < columns.Count(); i++)
			{
				T val = columns[i];
				if (!DataTypeChecker.TypeIsNotSupportedForProfiling(val.DataType, profilingDatabaseType))
				{
					AddProperty(expandoObject, val.Name, MapValueToString(sampleDataFromTableProfiling.Values[num][num2]));
					num2++;
				}
			}
			list.Add(expandoObject);
			num++;
		}
		return list;
	}

	private static bool AddProperty(ExpandoObject obj, string key, object value)
	{
		if (((IDictionary<string, object>)obj).ContainsKey(key))
		{
			return false;
		}
		((IDictionary<string, object>)obj).Add(key, value);
		return true;
	}

	private static GridColumn[] PrepareColumnsForSampleData<T>(IEnumerable<T> columns, ProfilingDatabaseTypeEnum.ProfilingDatabaseType profilingDatabaseType) where T : IColumnBase
	{
		List<GridColumn> list = new List<GridColumn>();
		foreach (T column in columns)
		{
			GridColumn gridColumn = new GridColumn
			{
				Name = column.Name,
				FieldName = column.Name,
				Caption = column.Name,
				Visible = true,
				MaxWidth = 400
			};
			if (DataTypeChecker.TypeIsNotSupportedForProfiling(column.DataType, profilingDatabaseType))
			{
				gridColumn.ToolTip = "This data type is not supported";
				gridColumn.ImageOptions.Image = Resources.about_16;
			}
			list.Add(gridColumn);
		}
		return list.ToArray();
	}

	private static string MapValueToString(object value)
	{
		if (value == null)
		{
			return "null";
		}
		return Convert.ToString(value, CultureInfo.InvariantCulture);
	}

	public static bool IsConnectorInLicense(SharedDatabaseTypeEnum.DatabaseType? dbType, Form owner)
	{
		if (!Connectors.HasDatabaseTypeConnector(dbType))
		{
			GeneralMessageBoxesHandling.Show("Your license does not contain the " + SharedDatabaseTypeEnum.TypeToStringForDisplay(dbType) + " connector.\r\n\r\nYou can only view profiling data.", "License information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, owner);
			return false;
		}
		return true;
	}

	public static void DrawValuesSparklines(List<long?> values, GraphicsCache drawCellGraphicsCache, Rectangle cellBounds, int marginFromTop)
	{
		int num = 3;
		int num2 = num + 2;
		Color color = Color.FromArgb(255, 68, 114, 196);
		int num3 = 0;
		double? num4 = (double?)values[0] * 1.0;
		int num5 = cellBounds.Height - marginFromTop;
		foreach (long? value in values)
		{
			if (value.HasValue)
			{
				double num6 = ((double)num5 * (1.0 - (double?)value / num4)).Value;
				int y = cellBounds.Location.Y + (int)num6 + marginFromTop;
				drawCellGraphicsCache.FillRectangle(color, new Rectangle(num + cellBounds.Location.X + num2 * num3, y, num, cellBounds.Height - marginFromTop - (int)num6));
				num3++;
			}
		}
	}

	private static void BestFitColumnsWhenGridsAreNotDisposed(GridControl sampleDataGridControl, GridView sampleDataGridControlGridView)
	{
		if (sampleDataGridControlGridView != null && sampleDataGridControl != null && sampleDataGridControlGridView.GridControl != null && !sampleDataGridControlGridView.IsDisposing && !sampleDataGridControl.IsDisposed)
		{
			sampleDataGridControlGridView.BestFitColumns();
		}
	}
}
