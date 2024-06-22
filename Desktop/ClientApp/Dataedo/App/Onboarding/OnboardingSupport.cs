using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Onboarding.Controls;
using Dataedo.App.Tools;
using DevExpress.Utils;

namespace Dataedo.App.Onboarding;

public class OnboardingSupport
{
	public enum OnboardingMessages
	{
		Run = 0,
		Import = 1,
		DocumentationExpandTables = 2,
		DocumentationExpandViews = 3,
		DocumentationExpandStructures = 4,
		DocumentationExpandProcedures = 5,
		DocumentationExpandFunctions = 6,
		ColumnsOpen = 7,
		ColumnsOpenMessageClosed = 8,
		DiagramSave = 9,
		DataProfilingButtonsShown = 10,
		DataLineageShown = 11
	}

	private static Dictionary<OnboardingMessages, OnboardingMessage> OnboardingMessagesData;

	private static OnboardingMessage Run;

	private static OnboardingMessage Import;

	private static OnboardingMessage DocumentationExpandTables;

	private static OnboardingMessage DocumentationExpandViews;

	private static OnboardingMessage DocumentationExpandProcedures;

	private static OnboardingMessage DocumentationExpandFunctions;

	private static OnboardingMessage DocumentationExpandStructures;

	private static OnboardingMessage ColumnsOpen;

	private static OnboardingMessage ColumnsOpenMessageClosed;

	private static OnboardingMessage DiagramSave;

	private static OnboardingMessage DataProfilingButtonsShown;

	private static OnboardingMessage DataLineageShown;

	static OnboardingSupport()
	{
		Run = new OnboardingMessage("Start your work by adding a <b>“New connection”</b> to your data source.", new Size(543, 78), BeakPanelBeakLocation.Top);
		Import = new OnboardingMessage("Expand to browse metadata/schema imported from your data source", new Size(395, 78), BeakPanelBeakLocation.Left, new Point(-20, 0), offsetFromEnd: true);
		DocumentationExpandTables = new OnboardingMessage("Expand and browse tables imported from the data source." + Environment.NewLine + "Select a table to see its details.", new Size(347, 91), BeakPanelBeakLocation.Left, new Point(-100, 0), offsetFromEnd: false);
		DocumentationExpandViews = new OnboardingMessage("Expand and browse views imported from the data source." + Environment.NewLine + "Select a view to see its details.", new Size(347, 91), BeakPanelBeakLocation.Left, new Point(-100, 0), offsetFromEnd: false);
		DocumentationExpandProcedures = new OnboardingMessage("Expand and browse procedures imported from the data source." + Environment.NewLine + "Select a procedure to see its details.", new Size(377, 91), BeakPanelBeakLocation.Left, new Point(-100, 0), offsetFromEnd: false);
		DocumentationExpandFunctions = new OnboardingMessage("Expand and browse functions imported from the data source." + Environment.NewLine + "Select a function to see its details.", new Size(377, 91), BeakPanelBeakLocation.Left, new Point(-100, 0), offsetFromEnd: false);
		DocumentationExpandStructures = new OnboardingMessage("Expand and browse structures imported from the data source." + Environment.NewLine + "Select structure to see its details.", new Size(377, 91), BeakPanelBeakLocation.Left, new Point(-100, 0), offsetFromEnd: false);
		ColumnsOpen = new OnboardingMessage("This is a table's structure imported from the data source.<br>1. Use the <b>description</b> field to describe columns,<br>2. Use the <b>title</b> field to provide an alias for column name,<br>3. Define additional <b>custom fields</b> to collect additional metadata,<br>4. Define <b>relationships</b> (foreign keys) between objects in the entire catalog,<br>5. Define <b>primary and unique keys</b> of tables, views, and other structures.", new Size(459, 143), BeakPanelBeakLocation.Bottom, new Point(0, -5), offsetFromEnd: true);
		ColumnsOpenMessageClosed = new OnboardingMessage("1. Create a subject area to document a subset of the database." + Environment.NewLine + "2. Assign tables and other objects from the catalog." + Environment.NewLine + "3. Visualize the data model with ER Diagram (ERD).", new Size(430, 103), BeakPanelBeakLocation.Left, new Point(-100, 0), offsetFromEnd: false);
		DiagramSave = new OnboardingMessage("You can share your documentation in various formats.", new Size(330, 78), BeakPanelBeakLocation.Top);
		DataProfilingButtonsShown = new OnboardingMessage("You can work on real data using Profile or Preview Sample data options.", new Size(430, 78), BeakPanelBeakLocation.Top);
		DataLineageShown = new OnboardingMessage("You can dive deeper into data lineage by enabling columns level flows.", new Size(430, 78), BeakPanelBeakLocation.Bottom, new Point(-100, 0), offsetFromEnd: false);
		OnboardingMessagesData = new Dictionary<OnboardingMessages, OnboardingMessage>
		{
			{
				OnboardingMessages.Run,
				Run
			},
			{
				OnboardingMessages.Import,
				Import
			},
			{
				OnboardingMessages.DocumentationExpandTables,
				DocumentationExpandTables
			},
			{
				OnboardingMessages.DocumentationExpandViews,
				DocumentationExpandViews
			},
			{
				OnboardingMessages.DocumentationExpandProcedures,
				DocumentationExpandProcedures
			},
			{
				OnboardingMessages.DocumentationExpandFunctions,
				DocumentationExpandFunctions
			},
			{
				OnboardingMessages.DocumentationExpandStructures,
				DocumentationExpandStructures
			},
			{
				OnboardingMessages.ColumnsOpen,
				ColumnsOpen
			},
			{
				OnboardingMessages.ColumnsOpenMessageClosed,
				ColumnsOpenMessageClosed
			},
			{
				OnboardingMessages.DiagramSave,
				DiagramSave
			},
			{
				OnboardingMessages.DataProfilingButtonsShown,
				DataProfilingButtonsShown
			},
			{
				OnboardingMessages.DataLineageShown,
				DataLineageShown
			}
		};
	}

	public static OnboardingPanel ShowPanel(OnboardingMessages onboardingMessage, Control owner, Func<Rectangle> getRectangle)
	{
		if (owner == null || !CheckMessageCondition(onboardingMessage))
		{
			return null;
		}
		OnboardingMessage message = OnboardingMessagesData[onboardingMessage];
		return OnboardingPanels.ShowPanel(onboardingMessage, message, owner, () => GetRectangleWithOffset(message, getRectangle));
	}

	public static void SetOnboardingCompleted()
	{
		LastConnectionInfo.LOGIN_INFO.OnboardingRun = true;
		LastConnectionInfo.LOGIN_INFO.OnboardingImport = true;
		LastConnectionInfo.LOGIN_INFO.OnboardingDocumentationExpand = true;
		LastConnectionInfo.LOGIN_INFO.OnboardingColumnsOpen = true;
		LastConnectionInfo.LOGIN_INFO.OnboardingColumnsOpenMessageClose = true;
		LastConnectionInfo.LOGIN_INFO.OnboardingDiagramSave = true;
		LastConnectionInfo.LOGIN_INFO.OnboardingDataProfilingButtonsShown = true;
		LastConnectionInfo.LOGIN_INFO.OnboardingDataLineageShown = true;
		LastConnectionInfo.Save();
	}

	public static void ResetOnboarding()
	{
		LastConnectionInfo.LOGIN_INFO.OnboardingRun = false;
		LastConnectionInfo.LOGIN_INFO.OnboardingImport = false;
		LastConnectionInfo.LOGIN_INFO.OnboardingDocumentationExpand = false;
		LastConnectionInfo.LOGIN_INFO.OnboardingColumnsOpen = false;
		LastConnectionInfo.LOGIN_INFO.OnboardingColumnsOpenMessageClose = false;
		LastConnectionInfo.LOGIN_INFO.OnboardingDiagramSave = false;
		LastConnectionInfo.LOGIN_INFO.OnboardingDataProfilingButtonsShown = false;
		LastConnectionInfo.LOGIN_INFO.OnboardingDataLineageShown = false;
		LastConnectionInfo.Save();
	}

	public static bool CheckMessageCondition(OnboardingMessages onboardingMessage)
	{
		switch (onboardingMessage)
		{
		case OnboardingMessages.Run:
			return !LastConnectionInfo.LOGIN_INFO.OnboardingRun;
		case OnboardingMessages.Import:
			return !LastConnectionInfo.LOGIN_INFO.OnboardingImport;
		case OnboardingMessages.DocumentationExpandTables:
		case OnboardingMessages.DocumentationExpandViews:
		case OnboardingMessages.DocumentationExpandStructures:
		case OnboardingMessages.DocumentationExpandProcedures:
		case OnboardingMessages.DocumentationExpandFunctions:
			return !LastConnectionInfo.LOGIN_INFO.OnboardingDocumentationExpand;
		case OnboardingMessages.ColumnsOpen:
			return !LastConnectionInfo.LOGIN_INFO.OnboardingColumnsOpen;
		case OnboardingMessages.ColumnsOpenMessageClosed:
			return !LastConnectionInfo.LOGIN_INFO.OnboardingColumnsOpenMessageClose;
		case OnboardingMessages.DiagramSave:
			return !LastConnectionInfo.LOGIN_INFO.OnboardingDiagramSave;
		case OnboardingMessages.DataProfilingButtonsShown:
			return !LastConnectionInfo.LOGIN_INFO.OnboardingDataProfilingButtonsShown;
		case OnboardingMessages.DataLineageShown:
			return !LastConnectionInfo.LOGIN_INFO.OnboardingDataLineageShown;
		default:
			return false;
		}
	}

	private static Rectangle GetRectangleWithOffset(OnboardingMessage message, Func<Rectangle> getRectangle)
	{
		Rectangle result = getRectangle();
		if (message.OffsetFromEnd)
		{
			result.Offset(message.Offset);
			result.Width += message.Offset.X;
			result.Height += message.Offset.Y;
		}
		else
		{
			result.Width = ((message.Offset.X != 0) ? (-message.Offset.X) : result.Width);
			result.Height = ((message.Offset.Y != 0) ? (-message.Offset.Y) : result.Height);
		}
		return result;
	}

	public static void SetMessageCondition(OnboardingMessages onboardingMessage)
	{
		switch (onboardingMessage)
		{
		case OnboardingMessages.Run:
			LastConnectionInfo.LOGIN_INFO.OnboardingRun = true;
			break;
		case OnboardingMessages.Import:
			LastConnectionInfo.LOGIN_INFO.OnboardingImport = true;
			break;
		case OnboardingMessages.DocumentationExpandTables:
		case OnboardingMessages.DocumentationExpandViews:
		case OnboardingMessages.DocumentationExpandStructures:
		case OnboardingMessages.DocumentationExpandProcedures:
		case OnboardingMessages.DocumentationExpandFunctions:
			LastConnectionInfo.LOGIN_INFO.OnboardingDocumentationExpand = true;
			break;
		case OnboardingMessages.ColumnsOpen:
			LastConnectionInfo.LOGIN_INFO.OnboardingColumnsOpen = true;
			break;
		case OnboardingMessages.ColumnsOpenMessageClosed:
			LastConnectionInfo.LOGIN_INFO.OnboardingColumnsOpenMessageClose = true;
			break;
		case OnboardingMessages.DiagramSave:
			LastConnectionInfo.LOGIN_INFO.OnboardingDiagramSave = true;
			break;
		case OnboardingMessages.DataProfilingButtonsShown:
			LastConnectionInfo.LOGIN_INFO.OnboardingDataProfilingButtonsShown = true;
			break;
		case OnboardingMessages.DataLineageShown:
			LastConnectionInfo.LOGIN_INFO.OnboardingDataLineageShown = true;
			break;
		}
		LastConnectionInfo.Save();
	}
}
