using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;

namespace Dataedo.App.Tools.ERD;

public class PostItDB : CommonDBSupport
{
	public int post_it_id { get; set; }

	public int module_id { get; set; }

	public int? pos_x { get; set; }

	public int? pos_y { get; set; }

	public int? pos_z { get; set; }

	public string color { get; set; }

	public int width { get; set; }

	public int height { get; set; }

	public string text { get; set; }

	public string text_position { get; set; }

	public PostItDB(PostIt postIt)
		: this()
	{
		post_it_id = postIt.Id.Value;
		module_id = postIt.SubjectAreaId;
		pos_x = postIt.Position.X;
		pos_y = postIt.Position.Y;
		pos_z = PostItLayerEnum.TypeToInt(postIt.Layer);
		color = ColorTranslator.ToHtml(postIt.Color);
		width = postIt.Width;
		height = postIt.Height;
		text = postIt.Text;
		text_position = PostItTextPositionEnum.TypeToString(postIt.TextPosition);
	}

	public PostItDB()
	{
		commands = StaticData.Commands;
	}

	public int InsertOrUpdatePostIts(List<PostItDB> postIts, Form owner = null)
	{
		int? num = 0;
		string text = string.Empty;
		Exception ex = null;
		try
		{
			ErdPostIt[] items = ConvertToErdPostIts(postIts);
			commands.Manipulation.Erd.InsertOrUpdateErdPostIts(items);
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

	public int InsertOrUpdatePostIts(ErdPostIt[] postIts, Form owner = null)
	{
		int? num = 0;
		string text = string.Empty;
		Exception ex = null;
		try
		{
			commands.Manipulation.Erd.InsertOrUpdateErdPostIts(postIts);
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

	public bool DeletePostIts(IEnumerable<int> ids, Form owner = null)
	{
		try
		{
			commands.Manipulation.Erd.DeleteErdPostIts(ids.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the ERD post-its:", owner);
			return false;
		}
		return true;
	}

	public List<ErdPostIt> GetPostItsByModuleId(int moduleId)
	{
		return commands.Select.Erd.GetPostItsByModuleId(moduleId);
	}

	private ErdPostIt[] ConvertToErdPostIts(IEnumerable<PostItDB> postIts)
	{
		return postIts.Select((PostItDB x) => new ErdPostIt
		{
			Id = x.post_it_id,
			ModuleId = x.module_id,
			PositionX = x.pos_x,
			PositionY = x.pos_y,
			PositionZ = x.pos_z,
			Color = x.color,
			Width = x.width,
			Height = x.height,
			Text = x.text,
			TextPosition = x.text_position
		}).ToArray();
	}
}
