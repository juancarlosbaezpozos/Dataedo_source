using System;
using System.Data;
using DevExpress.XtraEditors;

namespace Dataedo.App.Classes;

public static class TableauProduct
{
	public static DataTable tableauProducts;

	public static void CreateTableauProductDataSource()
	{
		tableauProducts = new DataTable();
		tableauProducts.Columns.Add("id", typeof(int));
		tableauProducts.Columns.Add("type", typeof(string));
		tableauProducts.Rows.Add(0, "Tableau Online");
		tableauProducts.Rows.Add(1, "Tableau Server");
	}

	public static void SetTableauProductDataSource(LookUpEdit tableauProductLookUpEdit)
	{
		tableauProductLookUpEdit.Properties.DataSource = tableauProducts;
		tableauProductLookUpEdit.Properties.DropDownRows = tableauProducts.Rows.Count;
	}

	public static bool IsServerProduct(LookUpEdit tableauProductLookUpEdit)
	{
		if (tableauProductLookUpEdit.EditValue != null)
		{
			return Convert.ToInt32(tableauProductLookUpEdit.EditValue) == 1;
		}
		return false;
	}

	public static int GetIndex(LookUpEdit tableauProductLookUpEdit)
	{
		return Convert.ToInt32(tableauProductLookUpEdit.EditValue);
	}
}
