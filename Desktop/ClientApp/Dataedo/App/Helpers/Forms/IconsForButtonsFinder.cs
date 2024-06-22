using System.Drawing;
using Dataedo.App.Properties;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Helpers.Forms;

public static class IconsForButtonsFinder
{
	private static readonly Bitmap DefaultIcon16 = Resources.edit_16;

	private static readonly Bitmap DefaultIcon32 = Resources.edit_32;

	public static Bitmap ReturnImageForDesignButtonItem16(SharedObjectTypeEnum.ObjectType? objectType)
	{
		return objectType switch
		{
			SharedObjectTypeEnum.ObjectType.Table => Resources.design_table_16, 
			SharedObjectTypeEnum.ObjectType.View => Resources.design_view_16, 
			SharedObjectTypeEnum.ObjectType.Structure => Resources.design_structure_16, 
			SharedObjectTypeEnum.ObjectType.Function => Resources.design_function_16, 
			SharedObjectTypeEnum.ObjectType.Procedure => Resources.design_procedure_16, 
			_ => DefaultIcon16, 
		};
	}

	public static Bitmap ReturnImageForDesignButtonItem32(SharedObjectTypeEnum.ObjectType? objectType)
	{
		return objectType switch
		{
			SharedObjectTypeEnum.ObjectType.Table => Resources.design_table_32, 
			SharedObjectTypeEnum.ObjectType.View => Resources.design_view_32, 
			SharedObjectTypeEnum.ObjectType.Structure => Resources.design_structure_32, 
			SharedObjectTypeEnum.ObjectType.Function => Resources.design_function_32, 
			SharedObjectTypeEnum.ObjectType.Procedure => Resources.design_procedure_32, 
			_ => DefaultIcon32, 
		};
	}

	public static Bitmap ReturnImageForDesignButtonItem16(NodeTypeEnum.NodeType nodeType)
	{
		return nodeType switch
		{
			NodeTypeEnum.NodeType.Table => Resources.design_table_16, 
			NodeTypeEnum.NodeType.View => Resources.design_view_16, 
			NodeTypeEnum.NodeType.Structure => Resources.design_structure_16, 
			_ => DefaultIcon16, 
		};
	}

	public static Bitmap ReturnImageForDesignButtonItem32(NodeTypeEnum.NodeType nodeType)
	{
		return nodeType switch
		{
			NodeTypeEnum.NodeType.Table => Resources.design_table_32, 
			NodeTypeEnum.NodeType.View => Resources.design_view_32, 
			NodeTypeEnum.NodeType.Structure => Resources.design_structure_32, 
			_ => DefaultIcon32, 
		};
	}

	public static Bitmap ReturnImageForAddButtonItem16(SharedObjectTypeEnum.ObjectType? objectType)
	{
		return objectType switch
		{
			SharedObjectTypeEnum.ObjectType.Table => Resources.table_add_16, 
			SharedObjectTypeEnum.ObjectType.View => Resources.view_new_16, 
			SharedObjectTypeEnum.ObjectType.Structure => Resources.structure_add_16, 
			SharedObjectTypeEnum.ObjectType.Function => Resources.function_new_16, 
			SharedObjectTypeEnum.ObjectType.Procedure => Resources.procedure_new_16, 
			_ => DefaultIcon16, 
		};
	}

	public static Bitmap ReturnImageForAddButtonItem32(SharedObjectTypeEnum.ObjectType? objectType)
	{
		return objectType switch
		{
			SharedObjectTypeEnum.ObjectType.Table => Resources.table_add_32, 
			SharedObjectTypeEnum.ObjectType.View => Resources.view_new_32, 
			SharedObjectTypeEnum.ObjectType.Structure => Resources.structure_add_32, 
			SharedObjectTypeEnum.ObjectType.Function => Resources.function_new_32, 
			SharedObjectTypeEnum.ObjectType.Procedure => Resources.procedure_new_32, 
			_ => DefaultIcon32, 
		};
	}
}
