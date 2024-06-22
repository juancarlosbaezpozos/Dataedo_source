using System;
using System.Linq;
using System.Windows;
using Dataedo.App.Tools.UI;

namespace Dataedo.App.Tools;

public abstract class BulkHelper
{
	public string Template { get; protected set; }

	public string ClipboardData
	{
		get
		{
			try
			{
				IDataObject dataObject = Clipboard.GetDataObject();
				if (dataObject == null)
				{
					return string.Empty;
				}
				if (dataObject.GetDataPresent(DataFormats.UnicodeText))
				{
					return dataObject.GetData(DataFormats.UnicodeText)?.ToString();
				}
				return string.Empty;
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}
		set
		{
			ClipboardSupport.SetDataObject(value);
		}
	}

	public bool IsEmptyLine(string[] data)
	{
		return data.ToList().TrueForAll((string x) => string.IsNullOrEmpty(x) || string.IsNullOrWhiteSpace(x) || x.Equals("Unchecked"));
	}

	public string ShortenedString(string value, int length)
	{
		return new string(value.Take(length).ToArray());
	}

	public bool CheckCheckBoxColumnsValues(string data)
	{
		return CheckboxValues.IsPositive(data);
	}

	public void CopyTemplate()
	{
		ClipboardData = Template + "\t";
	}
}
