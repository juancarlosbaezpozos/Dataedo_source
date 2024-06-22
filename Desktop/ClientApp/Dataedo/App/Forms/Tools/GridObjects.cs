using System.Collections.Generic;
using System.Linq;
using Dataedo.Model.Data.Common.Interfaces;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Forms.Tools;

public static class GridObjects
{
	public static IEnumerable<int> MoveUp<T>(GridView gridView) where T : class, IMoveable
	{
		List<int> list = new List<int>();
		gridView.CloseEditor();
		List<int> list2 = gridView.GetSelectedRows().ToList();
		for (int i = 0; i < list2.Count; i++)
		{
			gridView.UnselectRow(list2[i]);
			if (list2[i] > 0 && (i - 1 < 0 || list2[i] == list2.FirstOrDefault() || list2[i] - 1 != list2[i - 1]))
			{
				SortColumns<T>(list2[i], -1, gridView, list);
				list2[i]--;
			}
		}
		list2.ForEach(delegate(int x)
		{
			gridView.SelectRow(x);
		});
		gridView.RefreshData();
		return list;
	}

	public static IEnumerable<int> MoveDown<T>(GridView gridView) where T : class, IMoveable
	{
		List<int> list = new List<int>();
		gridView.CloseEditor();
		List<int> list2 = gridView.GetSelectedRows().ToList();
		for (int num = list2.Count - 1; num >= 0; num--)
		{
			gridView.UnselectRow(list2[num]);
			if (list2[num] < gridView.RowCount - 1 && list2[num] >= 0 && (num + 1 >= gridView.RowCount - 1 || list2[num] == list2.LastOrDefault() || list2[num] + 1 != list2[num + 1]))
			{
				SortColumns<T>(list2[num], 1, gridView, list);
				list2[num]++;
			}
		}
		list2.ForEach(delegate(int x)
		{
			gridView.SelectRow(x);
		});
		gridView.RefreshData();
		return list;
	}

	private static void SwapColumns<T>(T row1, T row2, GridView gridView)
	{
		List<T> list = gridView.DataSource as List<T>;
		int index = list.IndexOf(row2);
		Swap(row1, index, list);
	}

	private static void Swap<T>(T row, int index, List<T> datasource)
	{
		if (index >= 0)
		{
			datasource.Remove(row);
			datasource.Insert(index, row);
		}
	}

	private static void SortColumns<T>(int rowHandle, int step, GridView gridView, List<int> touchedRowHandles) where T : class, IMoveable
	{
		T row = gridView.GetRow(rowHandle) as T;
		T row2 = gridView.GetRow(rowHandle + step) as T;
		SwapColumns(row, row2, gridView);
		gridView.FocusedRowHandle = rowHandle + step;
		List<GridCell> list = gridView.GetSelectedCells().ToList();
		if (list.Count > 0)
		{
			list.ForEach(delegate(GridCell x)
			{
				gridView.UnselectCell(x);
			});
		}
		gridView.UnselectRow(rowHandle);
		gridView.SelectRow(rowHandle + step);
		if (!touchedRowHandles.Contains(rowHandle))
		{
			touchedRowHandles.Add(rowHandle);
		}
		if (!touchedRowHandles.Contains(rowHandle + step))
		{
			touchedRowHandles.Add(rowHandle + step);
		}
	}
}
