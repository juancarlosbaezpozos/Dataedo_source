using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Dataedo.App.Tools.Exceptions;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Shared.Enums;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.UserControls.Columns;

public static class UserViewData
{
	private static List<UserViewInfo> UserViewInfos = new List<UserViewInfo>();

	public static string GetViewName(Control control, GridView gridView, SharedObjectTypeEnum.ObjectType objectType)
	{
		return $"{control.Name}_{gridView.Name}_{objectType}";
	}

	public static void SaveColumns(string viewName, GridView gridView)
	{
		UserViewInfo userViewInfo = UserViewInfos.Where((UserViewInfo x) => x.ViewName == viewName).FirstOrDefault();
		if (userViewInfo == null)
		{
			userViewInfo = new UserViewInfo
			{
				ViewName = viewName
			};
			UserViewInfos.Add(userViewInfo);
		}
		userViewInfo.SaveColumns(gridView);
	}

	public static bool LoadColumns(string viewName, GridView gridView)
	{
		UserViewInfo userViewInfo = UserViewInfos.Where((UserViewInfo x) => x.ViewName == viewName).FirstOrDefault();
		if (userViewInfo == null)
		{
			return false;
		}
		userViewInfo.LoadColumns(gridView);
		return true;
	}

	private static string GetFilePathForSaving()
	{
		string confFolderPath = ConfigurationFileHelper.GetConfFolderPath();
		if (string.IsNullOrWhiteSpace(confFolderPath))
		{
			return string.Empty;
		}
		return Path.Combine(confFolderPath, "userLayout.xml");
	}

	private static string GetFilePathForLoading()
	{
		for (int num = Assembly.GetExecutingAssembly().GetName().Version.Major; num >= 4; num--)
		{
			string confFolderPath = ConfigurationFileHelper.GetConfFolderPath(num);
			if (!string.IsNullOrWhiteSpace(confFolderPath) && Directory.Exists(confFolderPath))
			{
				string text = Path.Combine(confFolderPath, "userLayout.xml");
				if (File.Exists(text))
				{
					return text;
				}
			}
		}
		return string.Empty;
	}

	public static void SaveToXML()
	{
		try
		{
			string filePathForSaving = GetFilePathForSaving();
			if (string.IsNullOrWhiteSpace(filePathForSaving))
			{
				return;
			}
			using StreamWriter textWriter = new StreamWriter(filePathForSaving);
			new XmlSerializer(typeof(List<UserViewInfo>)).Serialize(textWriter, UserViewInfos);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, HandlingMethodEnumeration.HandlingMethod.LogInErrorLog);
		}
	}

	public static void LoadFromXML()
	{
		try
		{
			string filePathForLoading = GetFilePathForLoading();
			if (string.IsNullOrWhiteSpace(filePathForLoading) || !File.Exists(filePathForLoading))
			{
				return;
			}
			using FileStream input = new FileStream(filePathForLoading, FileMode.Open);
			XmlReader xmlReader = XmlReader.Create(input);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<UserViewInfo>));
			if (!xmlSerializer.CanDeserialize(xmlReader))
			{
				throw new Exception("The file " + filePathForLoading + " could not be deserialized");
			}
			UserViewInfos = (List<UserViewInfo>)xmlSerializer.Deserialize(xmlReader);
			if (UserViewInfos == null)
			{
				UserViewInfos = new List<UserViewInfo>();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, HandlingMethodEnumeration.HandlingMethod.LogInErrorLog);
		}
	}
}
