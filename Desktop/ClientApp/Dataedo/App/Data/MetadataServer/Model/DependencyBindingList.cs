using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.XtraTreeList;

namespace Dataedo.App.Data.MetadataServer.Model;

internal class DependencyBindingList : BindingList<DependencyRow>, TreeList.IVirtualTreeListData
{
	public DependencyBindingList(IList<DependencyRow> list)
		: base(list)
	{
	}

	public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
	{
		if (info.Node is DependencyRow dependencyRow && info.Column.FieldName == "DisplayName")
		{
			info.CellData = dependencyRow.DisplayName;
		}
	}

	public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
	{
		info.Children = (info.Node as DependencyRow)?.Children;
	}

	public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
	{
	}

	public DependencyRow GetFirstSameDependency(DependencyRow dependencyRow)
	{
		DependencyRow dependencyRow2 = null;
		using IEnumerator<DependencyRow> enumerator = GetEnumerator();
		while (enumerator.MoveNext())
		{
			dependencyRow2 = enumerator.Current.GetFirstSameDependency(dependencyRow);
			if (dependencyRow2 != null)
			{
				return dependencyRow2;
			}
		}
		return dependencyRow2;
	}
}
