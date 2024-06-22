using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;

namespace Dataedo.App.Import.DataLake.Processing.CsvSupport;

public class CsvTable
{
	public bool HeadersCorrectness { get; set; }

	public int ColumnCount { get; set; }

	public string[] HeaderRow { get; set; }

	public string[] OrginalHeaders { get; set; }

	public List<CsvColumn> Columns { get; set; }

	public string HeaderInfo { get; set; }

	public HashSet<int> UncorrectHeaders { get; set; }

	private static int NumberOfReadLines { get; } = 1000;


	public CsvTable(TextReader textReader, int? lineCount = null)
	{
		UncorrectHeaders = new HashSet<int>();
		using (CsvReader csvReader = new CsvReader(textReader, CultureInfo.InvariantCulture))
		{
			csvReader.Configuration.BadDataFound = null;
			csvReader.Configuration.IgnoreBlankLines = true;
			csvReader.Configuration.IgnoreQuotes = true;
			csvReader.Configuration.MissingFieldFound = null;
			csvReader.Read();
			csvReader.ReadHeader();
			HeaderRow = csvReader.Context.HeaderRecord;
			OrginalHeaders = HeaderRow;
			HeadersCorrectness = CheckHeaders();
			ColumnCount = HeaderRow.Count();
			Columns = new List<CsvColumn>();
			if (!HeadersCorrectness)
			{
				string[] array = new string[ColumnCount];
				for (int i = 0; i < ColumnCount; i++)
				{
					array[i] = "Col_" + i;
				}
				int num = 1;
				string[] array2 = array;
				foreach (string header in array2)
				{
					Columns.Add(new CsvColumn(header, num));
					num++;
				}
				for (int k = 0; k < ColumnCount; k++)
				{
					Columns[k].AddFieldDataType(CsvDataTypeSupport.DetermineFieldType(HeaderRow[k]));
				}
				HeaderRow = array;
			}
			else
			{
				int num2 = 1;
				string[] array2 = HeaderRow;
				for (int j = 0; j < array2.Length; j++)
				{
					string text;
					if ((text = array2[j]).StartsWith("\""))
					{
						text = text.Substring(1);
					}
					if (text.EndsWith("\""))
					{
						text = text.Substring(0, text.Length - 1);
					}
					Columns.Add(new CsvColumn(text, num2));
					num2++;
				}
			}
			try
			{
				int num3 = 0;
				using CsvDataReader csvDataReader = new CsvDataReader(csvReader);
				do
				{
					string[] array3 = new string[ColumnCount];
					object[] values = array3;
					csvDataReader.GetValues(values);
					if (!array3[0].StartsWith("#"))
					{
						for (int l = 0; l < ColumnCount; l++)
						{
							Columns[l].AddFieldDataType(CsvDataTypeSupport.DetermineFieldType(array3[l]));
						}
					}
					num3++;
				}
				while (csvDataReader.Read() && num3 < NumberOfReadLines);
			}
			catch (CsvHelperException ex)
			{
				if (!(ex.Message == "No header record was found."))
				{
					throw ex;
				}
				for (int m = 0; m < ColumnCount; m++)
				{
					Columns[m].AddFieldDataType(CsvDataType.Null);
				}
			}
		}
		foreach (CsvColumn column in Columns)
		{
			column.DetermineColumnType();
		}
	}

	public CsvTable(string data)
		: this(new StringReader(data))
	{
	}

	public bool CheckHeaders()
	{
		bool flag = true;
		for (int i = 0; i < HeaderRow.Length; i++)
		{
			CsvDataType csvDataType = CsvDataTypeSupport.DetermineFieldType(HeaderRow[i]);
			if (csvDataType != CsvDataType.String && csvDataType != 0)
			{
				HeaderInfo = $"One of the headers is of type: {csvDataType}";
				UncorrectHeaders.Add(i);
				flag = false;
				break;
			}
		}
		if (flag)
		{
			HeaderInfo = "Headers are correct";
		}
		return flag;
	}
}
