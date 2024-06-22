using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.Erd;

namespace Dataedo.App.Tools.ERD;

public class NodeDB : CommonDBSupport
{
	public int? node_id { get; set; }

	public int table_id { get; set; }

	public int? pos_x { get; set; }

	public int? pos_y { get; set; }

	public int module_id { get; set; }

	public string color { get; set; }

	public int width { get; set; }

	public string title { get; set; }

	public List<NodeColumnDB> Columns { get; set; }

	public NodeDB(Node node)
	{
		node_id = node.Id;
		table_id = node.TableId;
		pos_x = node.Position.X;
		pos_y = node.Position.Y;
		module_id = node.SubjectAreaId;
		color = ColorTranslator.ToHtml(node.Color);
		width = node.Width;
		title = node.Title;
		Columns = node.Columns;
	}

	public NodeDB()
	{
		Columns = new List<NodeColumnDB>();
		commands = StaticData.Commands;
	}

	public int InsertOrUpdateNodes(List<NodeDB> nodes, Form owner = null)
	{
		int? num = 0;
		string text = string.Empty;
		Exception ex = null;
		try
		{
			ErdNode[] array = ConvertToErdNodes(nodes);
			commands.Manipulation.Erd.InsertOrUpdateErdNodes(array);
			for (int i = 0; i < array.Length; i++)
			{
				foreach (NodeColumnDB column in nodes[i].Columns)
				{
					if (array[i].Id.HasValue)
					{
						column.NodeId = array[i].Id;
					}
				}
			}
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

	public int InsertOrUpdateNodes(ErdNode[] nodes, Form owner = null)
	{
		int? num = 0;
		string text = string.Empty;
		Exception ex = null;
		try
		{
			commands.Manipulation.Erd.InsertOrUpdateErdNodes(nodes);
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

	public bool DeleteNodes(IEnumerable<int> ids, Form owner = null)
	{
		try
		{
			commands.Manipulation.Erd.DeleteErdNodes(ids.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the ERD nodes:", owner);
			return false;
		}
		return true;
	}

	public void DeleteERDColumnByColumnId(int columnId, Form owner = null)
	{
		try
		{
			commands.Manipulation.Erd.DeleteErdNodeColumnByColumnId(columnId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting ERD column", owner);
		}
	}

	public List<ErdNodeObject> GetDataByModuleId(int moduleId)
	{
		return commands.Select.Erd.GetDataByModuleId(moduleId);
	}

	public List<ErdNode> GetExistingErdNodesByModuleId(int moduleId)
	{
		return commands.Select.Erd.GetExistingErdNodesByModuleId(moduleId);
	}

	private ErdNode[] ConvertToErdNodes(IEnumerable<NodeDB> nodes)
	{
		return nodes.Select((NodeDB x) => new ErdNode
		{
			Id = x.node_id,
			TableId = x.table_id,
			ModuleId = x.module_id,
			PositionX = x.pos_x,
			PositionY = x.pos_y,
			Color = x.color,
			Width = x.width,
			Title = x.title
		}).ToArray();
	}
}
