using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.DataLake.Processing.CsvSupport;
using Dataedo.App.Import.Exceptions;
using Dataedo.Shared.Enums;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Dataedo.App.Import.DataLake.Processing;

internal class ExcelImport : IDataLakeImport
{
	private enum Formats
	{
		General = 0,
		Number = 1,
		Decimal = 2,
		Percentage = 10,
		Scientific = 11,
		Fraction = 12,
		Duration = 46,
		Text = 49,
		Currency = 166,
		Date = 168,
		DateTime = 170
	}

	public SharedObjectTypeEnum.ObjectType ObjectType { get; protected set; }

	public DataLakeTypeEnum.DataLakeType DataLakeType => DataLakeTypeEnum.DataLakeType.EXCEL_TABLE;

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype => SharedObjectSubtypeEnum.ObjectSubtype.Table;

	public string DefaultExtension => ".xlsx";

	public IEnumerable<string> Extensions => new string[1] { DefaultExtension };

	public bool DetermineByExtensionPriority
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public string DocumentationLink => "https://dataedo.com/docs/microsoft-excel?utm_source=App&utm_medium=App";

	public ExcelImport(SharedObjectTypeEnum.ObjectType objectType)
	{
		ObjectType = objectType;
	}

	public IEnumerable<ObjectModel> GetObjectsFromData(string data)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<ObjectModel> GetObjectsFromFile(string path)
	{
		List<ObjectModel> list = new List<ObjectModel>();
		try
		{
			if (string.IsNullOrEmpty(path))
			{
				throw new InvalidDataProvidedException("Path is incorrect");
			}
			using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(path, isEditable: false))
			{
				if (spreadsheetDocument == null || spreadsheetDocument?.WorkbookPart == null)
				{
					ThrowExceptionWhenEmpty(list);
				}
				Dictionary<uint, string> formatMappings = SetFormatMappings(spreadsheetDocument);
				IEnumerable<WorksheetPart> obj = spreadsheetDocument?.WorkbookPart?.WorksheetParts;
				if (obj == null)
				{
					ThrowExceptionWhenEmpty(list);
				}
				foreach (WorksheetPart item2 in obj)
				{
					if (item2 == null)
					{
						continue;
					}
					IEnumerable<TableDefinitionPart> obj2 = item2?.TableDefinitionParts;
					if (obj2 == null)
					{
						ThrowExceptionWhenEmpty(list);
					}
					foreach (TableDefinitionPart item3 in obj2)
					{
						if (item3 == null)
						{
							continue;
						}
						Table table = item3?.Table;
						if (table == null)
						{
							continue;
						}
						string text = table?.DisplayName;
						if (string.IsNullOrEmpty(text))
						{
							continue;
						}
						TableColumns tableColumns = table?.TableColumns;
						ObjectModel objectModel = new ObjectModel(Path.GetFileName(path) + "." + text, path, path, DataLakeTypeEnum.DataLakeType.EXCEL_TABLE, SharedObjectTypeEnum.ObjectType.Structure, SharedObjectSubtypeEnum.ObjectSubtype.ExcelTable);
						string[] array = (table?.Reference)?.Value?.Split(':');
						if (array == null || (array != null && array.Length < 2))
						{
							continue;
						}
						string text2 = array[0];
						string text3 = array[1];
						WorkbookPart workbookPart = spreadsheetDocument?.WorkbookPart;
						SheetData sheetData = item2?.Worksheet?.GetFirstChild<SheetData>();
						int num = 1;
						if (tableColumns == null || string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text3) || workbookPart == null || sheetData == null)
						{
							continue;
						}
						int tableRowCount = GetCellNumber(text3) - GetCellNumber(text2);
						foreach (TableColumn item4 in tableColumns)
						{
							HashSet<CsvDataType> columnData = new HashSet<CsvDataType>();
							string dataCell = GetNextRow(text2);
							string dataType = CheckDataType(formatMappings, workbookPart, sheetData, tableRowCount, columnData, ref dataCell);
							text2 = GetNextColumn(text2);
							FieldModel item = new FieldModel
							{
								Name = item4.Name,
								Position = num,
								Id = num,
								DataType = dataType
							};
							num++;
							objectModel.Fields.Add(item);
						}
						list.Add(objectModel);
					}
				}
			}
			ThrowExceptionWhenEmpty(list);
			return list;
		}
		catch (Exception ex)
		{
			if (ex is OpenXmlPackageException || ex is FileFormatException || ex is InvalidDataProvidedException)
			{
				throw new InvalidDataProvidedException("Unable to load Excel.", ex);
			}
			throw ex;
		}
	}

	private string CheckDataType(Dictionary<uint, string> formatMappings, WorkbookPart workbookPart, SheetData sheetData, int tableRowCount, HashSet<CsvDataType> columnData, ref string dataCell)
	{
		string text = "Null";
		for (int i = 0; i < tableRowCount && i < 5; i++)
		{
			Cell cell = GetCell(sheetData, dataCell);
			if (cell != null)
			{
				text = DetermineDataType(workbookPart, columnData, cell, formatMappings, dataCell);
			}
			if (text != "Null")
			{
				break;
			}
			dataCell = GetNextRow(dataCell);
		}
		return text;
	}

	private Dictionary<uint, string> SetFormatMappings(SpreadsheetDocument spreadSheetDocument)
	{
		Dictionary<uint, string> dictionary = new Dictionary<uint, string>();
		IEnumerable<NumberingFormats> enumerable = (spreadSheetDocument?.WorkbookPart?.WorkbookStylesPart)?.Stylesheet?.ChildElements?.OfType<NumberingFormats>();
		if (enumerable == null)
		{
			return dictionary;
		}
		foreach (NumberingFormats item in enumerable)
		{
			IEnumerable<NumberingFormat> enumerable2 = item?.ChildElements?.OfType<NumberingFormat>();
			if (enumerable2 == null)
			{
				continue;
			}
			foreach (NumberingFormat item2 in enumerable2)
			{
				uint? num = item2?.NumberFormatId?.Value;
				if (num.HasValue)
				{
					dictionary.Add(num.Value, item2?.FormatCode);
				}
			}
		}
		return dictionary;
	}

	public bool IsValidData(string data)
	{
		throw new NotImplementedException();
	}

	public bool IsValidFile(string path)
	{
		try
		{
			using SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(path, isEditable: false);
			if (spreadsheetDocument != null && spreadsheetDocument.WorkbookPart != null)
			{
				return true;
			}
		}
		catch (FileFormatException)
		{
			return false;
		}
		return false;
	}

	private string DetermineDataType(WorkbookPart workbookPart, HashSet<CsvDataType> columnData, Cell cell, Dictionary<uint, string> formatMappings, string startCell)
	{
		string text = "Null";
		int valueOrDefault = (int)(cell?.StyleIndex?.Value).GetValueOrDefault();
		uint valueOrDefault2 = (((CellFormat)(workbookPart?.WorkbookStylesPart?.Stylesheet?.CellFormats?.ElementAt(valueOrDefault)))?.NumberFormatId?.Value).GetValueOrDefault();
		formatMappings.TryGetValue(valueOrDefault2, out var value);
		if (!(value == "yyyy\\-mm\\-dd") && !(value == "yyyy\\-mm\\-dd;@") && !(value == "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy") && !(value == "d\\-mm;@") && !(value == "d\\-mm; @") && !(value == "d\\-mm;@") && !(value == "yy\\-mm\\-dd;@") && !(value == "yy\\-mm\\-dd; @") && !(value == "yyyy\\-mm\\-dd; @") && !(value == "[$-415]d\\ mmm;@") && !(value == "[$-415]d\\-mmm\\-yyyy;@") && !(value == "[$-415]d\\ mmm\\ yy;@") && !(value == "[$-415]dd\\ mmm\\ yy;@") && !(value == "[$-415]mmm\\ yy;@") && !(value == "[$-415]mmmm\\ yy;@") && !(value == "[$-415]d\\ mmmm\\ yyyy; @") && !(value == "[$-415]mmmmm;@") && !(value == "[$-415]mmmmm\\.yy;@") && !(value == "[$-415]d\\ mmmm\\ yyyy") && !(value == "[$-415]mmmmm") && !(value == "[$-415]d\\ mmm\\ yy") && !(value == "d\\-m\\-yyyy;@") && !(value == "[$-415]d\\ mmm") && !(value == "[$-415]mmm\\ yy") && !(value == "[$-415]d\\-mmm\\-yyyy; @") && !(value == "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy"))
		{
			switch (value)
			{
			default:
				if (valueOrDefault2 == 20 || valueOrDefault2 == 14 || valueOrDefault2 == 15 || valueOrDefault2 == 16 || valueOrDefault2 == 17)
				{
					break;
				}
				switch (value)
				{
				default:
					switch (valueOrDefault2)
					{
					case 21u:
					case 22u:
						break;
					case 0u:
						columnData.Add(CsvDataTypeSupport.DetermineFieldType(GetCellValue(cell, workbookPart)));
						return CsvDataTypeSupport.DetermineColumnType(columnData).ToString();
					case 10u:
						return Formats.Percentage.ToString();
					case 11u:
						return Formats.Scientific.ToString();
					default:
						if (value == null || !value.Contains("#\" \"?"))
						{
							switch (valueOrDefault2)
							{
							case 12u:
							case 13u:
								break;
							case 46u:
								return Formats.Duration.ToString();
							case 49u:
								return Formats.Text.ToString();
							default:
								if ((value != null && value.Contains("#,##0.00")) || (value != null && value.Contains("#,##0\\")))
								{
									return Formats.Decimal.ToString();
								}
								if (value != null && value.Contains("0.00"))
								{
									return Formats.Number.ToString();
								}
								if ((value != null && value.Contains("hh")) || (value != null && value.Contains("mm")) || (value != null && value.Contains("ss")))
								{
									return Formats.DateTime.ToString();
								}
								if ((value != null && value.Contains("d")) || (value != null && value.Contains("y")) || (value != null && value.Contains("m")))
								{
									return Formats.Number.ToString();
								}
								if (valueOrDefault2 == 2 || valueOrDefault2 == 1 || valueOrDefault2 == 4)
								{
									return Formats.Number.ToString();
								}
								columnData.Add(CsvDataTypeSupport.DetermineFieldType(GetCellValue(cell, workbookPart)));
								return CsvDataTypeSupport.DetermineColumnType(columnData).ToString();
							}
						}
						return Formats.Fraction.ToString();
					}
					break;
				case "[h]\":\"mm\":\"":
				case "yyyy\\-mm\\-dd\\ hh: mm: ss":
				case "yyyy\\-mm\\-dd\\ hh:mm:ss":
				case "\":\"mm\":\"ss":
				case "[$-409]dd\\-mm\\-yy\\ h:mm\\ AM/PM;@":
				case "[$-409]dd\\-mm\\-yy\\ h:mm\\ AM/PM":
				case "dd\\-mm\\-yy\\ h:mm;@":
				case "dd\\-mm\\-yy\\ h:mm":
				case "mm\"-\"dd\" \"hh\":\"mm":
				case "dddd\", \"d\" \"mmmm\" \"yyyy\", \"hh\":\"mm\":\"ss":
				case "HH:mm:ss":
				case "yyyy-MM-dd HH:mm:ss":
					break;
				}
				return Formats.DateTime.ToString();
			case "[$-F800]dddd\\,\\ mmmm\\ dd\\,\\ yyyy":
			case "[$-415]mmmm\\ yy":
			case "[$-415]mmmmm\\.yy":
			case "d\\-m\\-yyyy":
			case "[$-415]d\\-mmm\\-yyyy":
			case "[$-415]dddd\\,\\ d\\ mmmm\\ yyyy":
			case "[$-415]d\\ mmmm\\ yyyy;@":
			case "dd\"-\"mm\"-\"yyyy":
			case "yy\"-\"mm\"-\"dd":
			case "d\"-\"mmm\"-\"yyyy":
			case "mm\"-\"dd":
			case "d\" \"mmmm":
			case "d\"-\"mmm":
			case "dddd\", \"d\" \"mmmm\" \"yyyy":
			case "dd\".\"mm\".\"yyyy":
			case "yyyy-MM-dd":
				break;
			}
		}
		return Formats.Date.ToString();
	}

	private int GetCellNumber(string cell)
	{
		string text = string.Empty;
		for (int i = 0; i < cell.Length; i++)
		{
			if (char.IsDigit(cell[i]))
			{
				text += cell[i];
			}
		}
		return int.Parse(text);
	}

	private string GetCellLetter(string cell)
	{
		string text = string.Empty;
		for (int i = 0; i < cell.Length; i++)
		{
			if (!char.IsDigit(cell[i]))
			{
				text += cell[i];
			}
		}
		return text;
	}

	private string GetNextRow(string cell)
	{
		return string.Concat(str1: (GetCellNumber(cell) + 1).ToString(), str0: GetCellLetter(cell));
	}

	private string GetLast(string letter)
	{
		if (string.IsNullOrEmpty(letter))
		{
			return "AA";
		}
		char c = letter.Last();
		if (c == 'Z')
		{
			return GetLast(letter.Remove(letter.Length - 1));
		}
		return letter.Remove(letter.Length - 1) + (char)(c + 1);
	}

	private string GetNextColumn(string cell)
	{
		return string.Concat(str1: GetCellNumber(cell).ToString(), str0: GetLast(GetCellLetter(cell)));
	}

	private Cell GetCell(SheetData sheetData, string cellAddress)
	{
		uint rowIndex = uint.Parse(Regex.Match(cellAddress, "[0-9]+").Value);
		return sheetData.Descendants<Row>().FirstOrDefault((Row p) => (uint)p.RowIndex == rowIndex)?.Descendants<Cell>().FirstOrDefault((Cell p) => p.CellReference == cellAddress);
	}

	private string GetCellValue(Cell cell, WorkbookPart workbookPart)
	{
		string text = cell.InnerText;
		if (cell.DataType != null)
		{
			switch (cell.DataType.Value)
			{
			case CellValues.SharedString:
			{
				SharedStringTablePart sharedStringTablePart = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
				if (sharedStringTablePart != null)
				{
					text = sharedStringTablePart.SharedStringTable.ElementAt(int.Parse(text)).InnerText;
				}
				break;
			}
			case CellValues.Boolean:
				text = ((!(text == "0")) ? "TRUE" : "FALSE");
				break;
			}
		}
		return text;
	}

	private void ThrowExceptionWhenEmpty(List<ObjectModel> objectModels)
	{
		if (!objectModels.Any())
		{
			throw new InvalidDataProvidedException("No tables found in spreadsheet");
		}
	}
}
