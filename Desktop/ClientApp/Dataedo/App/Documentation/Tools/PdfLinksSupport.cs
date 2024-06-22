using System;
using System.Collections;
using System.Linq;
using Dataedo.App.Classes.Documentation;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Common.Interfaces;
using Dataedo.Model.Data.Common.Objects;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

namespace Dataedo.App.Documentation.Tools;

internal class PdfLinksSupport
{
	private readonly string[] LinkObjectsNames = new string[29]
	{
		"xrTableCell23", "xrTableCell33", "tableCell6", "tableCell14", "tableCell94", "tableCell98", "tableCell112", "tableCell115", "tableCell117", "tableUsesDependencies1LevelNameCell",
		"tableUsesDependencies2LevelNameCell", "tableUsedByDependencies1LevelNameCell", "tableUsedByDependencies2LevelNameCell", "viewUsesDependencies1LevelNameCell", "viewUsesDependencies2LevelNameCell", "viewUsedByDependencies1LevelNameCell", "viewUsedByDependencies2LevelNameCell", "procedureUsesDependencies1LevelNameCell", "procedureUsesDependencies2LevelNameCell", "procedureUsedByDependencies1LevelNameCell",
		"procedureUsedByDependencies2LevelNameCell", "functionUsesDependencies1LevelNameCell", "functionUsesDependencies2LevelNameCell", "functionUsedByDependencies1LevelNameCell", "functionUsedByDependencies2LevelNameCell", "structureUsesDependencies1LevelNameCell", "structureUsesDependencies2LevelNameCell", "structureUsedByDependencies1LevelNameCell", "structureUsedByDependencies2LevelNameCell"
	};

	private readonly string[] HeaderObjectsNames = new string[8] { "documentationHeaderXrLabel", "tableHeaderXrLabel", "viewHeaderXrLabel", "procedureHeaderXrLabel", "functionHeaderXrLabel", "xrTableCell17", "structureHeaderXrLabel", "glossaryEntryHeaderXrLabel" };

	public static string CreateIdString(int databaseId)
	{
		return $"{databaseId}";
	}

	public static string CreateIdString(int databaseId, int id)
	{
		return $"{databaseId}." + $"{id}";
	}

	public static string CreateIdString(string host, string databaseName, string schema, string name)
	{
		return host?.ToLower() + "." + databaseName?.ToLower() + "." + schema + "." + name;
	}

	public static string CreateIdString(ObjectDocObject row)
	{
		return CreateIdString(row.DatabaseHost, row.DatabaseName, row.Schema, row.Name);
	}

	public static string CreateIdString(DatabaseDoc database, TableViewDoc table, IBasicIdentification row)
	{
		return CreateIdString(database.Server, database.Title, table.Schema, PrepareValue.ToString(row.Name));
	}

	public void AfterPrint(object sender, EventArgs e)
	{
		XtraReport xtraReport = (XtraReport)sender;
		foreach (Page page in xtraReport.Pages)
		{
			foreach (BrickBase innerBrick in page.InnerBricks)
			{
				CheckBrick(xtraReport, innerBrick);
			}
		}
	}

	private void CheckBrick(XtraReport report, BrickBase brick)
	{
		IEnumerator enumerator = null;
		if (brick is VisualBrick)
		{
			VisualBrick visualBrick = (VisualBrick)brick;
			if ((visualBrick.BrickOwner is XRLabel || visualBrick.BrickOwner is XRTableCell) && (LinkObjectsNames.Contains((visualBrick.BrickOwner as XRLabel)?.Name) || LinkObjectsNames.Contains((visualBrick.BrickOwner as XRTableCell)?.Name)))
			{
				if (!AddReference(report, visualBrick))
				{
					visualBrick.Url = null;
				}
				return;
			}
			enumerator = (brick as VisualBrick).Bricks.GetEnumerator();
		}
		else if (brick is BrickContainerBase)
		{
			enumerator = (brick as BrickContainer).Bricks.GetEnumerator();
		}
		else if (brick is CompositeBrick)
		{
			enumerator = (brick as CompositeBrick).InnerBricks.GetEnumerator();
		}
		if (enumerator != null)
		{
			while (enumerator.MoveNext())
			{
				CheckBrick(report, (Brick)enumerator.Current);
			}
		}
	}

	private bool AddReference(XtraReport report, VisualBrick sourceBrick)
	{
		bool flag = false;
		foreach (Page page in report.Pages)
		{
			foreach (BrickBase innerBrick in page.InnerBricks)
			{
				flag = AddReference(report, page, innerBrick, sourceBrick) || flag;
			}
		}
		return flag;
	}

	private bool AddReference(XtraReport report, Page basePage, BrickBase baseBrick, VisualBrick sourceBrick)
	{
		bool flag = false;
		if (sourceBrick.Value != null)
		{
			IEnumerator enumerator = null;
			if (baseBrick is VisualBrick)
			{
				VisualBrick visualBrick = (VisualBrick)baseBrick;
				if ((visualBrick.BrickOwner is XRLabel || visualBrick.BrickOwner is XRTableCell) && (HeaderObjectsNames.Contains((visualBrick.BrickOwner as XRLabel)?.Name) || HeaderObjectsNames.Contains((visualBrick.BrickOwner as XRTableCell)?.Name)))
				{
					object value = visualBrick.Value;
					if (value != null && value.Equals(sourceBrick.Value))
					{
						sourceBrick.NavigationPair = BrickPagePair.Create(visualBrick, basePage);
						return true;
					}
					if (visualBrick.Bricks.Count == 0)
					{
						return false;
					}
				}
				enumerator = (baseBrick as VisualBrick).Bricks.GetEnumerator();
			}
			else if (baseBrick is BrickContainerBase)
			{
				enumerator = (baseBrick as BrickContainer).Bricks.GetEnumerator();
			}
			else if (baseBrick is CompositeBrick)
			{
				enumerator = (baseBrick as CompositeBrick).InnerBricks.GetEnumerator();
			}
			if (enumerator != null)
			{
				while (enumerator.MoveNext())
				{
					flag = AddReference(report, basePage, (Brick)enumerator.Current, sourceBrick) || flag;
				}
			}
		}
		return flag;
	}
}
