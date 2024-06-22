using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.ERD;

public class LinkDB : CommonDBSupport
{
	public int? link_id { get; set; }

	public int module_id { get; set; }

	public int relation_id { get; set; }

	public bool show_label { get; set; }

	public bool show_join_condition { get; set; }

	public bool hidden { get; set; }

	public string link_style { get; set; }

	public LinkDB()
	{
		commands = StaticData.Commands;
	}

	public LinkDB(Link link)
	{
		link_id = link.Id;
		module_id = link.SubjecrAreaId;
		relation_id = link.RelationId;
		show_label = link.ShowTitle == true;
		show_join_condition = link.ShowJoinCondition == true;
		hidden = link.Hidden == true;
		link_style = LinkStyleEnum.TypeToString(link.LinkStyle);
		link_id = link.Id;
	}

	public int InsertOrUpdateLinks(IEnumerable<LinkDB> links, Form owner = null)
	{
		int? num = 0;
		string text = string.Empty;
		Exception ex = null;
		try
		{
			commands.Manipulation.Erd.InsertOrUpdateErdLinks(ConvertToErdLinks(links));
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

	public void InsertLink(LinkDB link, Form owner = null)
	{
		try
		{
			commands.Manipulation.Erd.InsertErdLink(ConvertToErdLink(link));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting link.", owner);
		}
	}

	public void UpdateLink(LinkDB link, Form owner = null)
	{
		try
		{
			commands.Manipulation.Erd.UpdateErdLink(ConvertToErdLink(link));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating link.", owner);
		}
	}

	private ErdLink[] ConvertToErdLinks(IEnumerable<LinkDB> links)
	{
		return links.Select((LinkDB x) => ConvertToErdLink(x)).ToArray();
	}

	private ErdLink ConvertToErdLink(LinkDB x)
	{
		return new ErdLink
		{
			LinkId = x.link_id,
			ModuleId = x.module_id,
			RelationId = x.relation_id,
			ShowLabel = x.show_label,
			ShowJoinCondition = x.show_join_condition,
			Hidden = x.hidden,
			LinkStyle = x.link_style
		};
	}
}
