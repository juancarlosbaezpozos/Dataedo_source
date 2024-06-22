using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Procedures.Parameters;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors.DXErrorProvider;

namespace Dataedo.App.Classes.Synchronize;

public class ParameterRow : BasicRow, IDXDataErrorInfo
{
	public enum ModeEnum
	{
		In = 1,
		Out = 2,
		InOut = 3
	}

	public string LastModificationDateString { get; set; }

	public string CreationDateString { get; set; }

	public string CreatedBy { get; set; }

	public string ModifiedBy { get; set; }

	public string DatabaseName { get; set; }

	public string ProcedureName { get; set; }

	public string ProcedureSchema { get; set; }

	public string DataType { get; set; }

	public int Position { get; set; }

	public string ParameterMode { get; set; }

	public string DataLength { get; set; }

	public ModeEnum? Mode { get; set; }

	public bool AlreadyExists { get; set; }

	public bool IsNameEmpty { get; set; }

	public ParameterRow()
	{
	}

	public ParameterRow(CustomFieldsSupport customFieldsSupport = null)
	{
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields();
		}
	}

	public ParameterRow(ParameterObject row, CustomFieldsSupport customFieldsSupport = null, bool getDataTypeWithoutLength = false)
		: base(row)
	{
		RetrieveCommonData(row, getDataTypeWithoutLength);
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(row);
		}
	}

	public ParameterRow(int prcedureId, ParameterObject row, CustomFieldsSupport customFieldsSupport = null)
		: base(row)
	{
		RetrieveCommonData(row);
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(SharedObjectTypeEnum.ObjectType.Parameter, base.Id, customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(row);
		}
	}

	public ParameterRow(ParameterSynchronizationObject dataReader, CustomFieldsSupport customFieldsSupport = null)
	{
		base.Name = PrepareValue.ToString(dataReader.Name);
		DatabaseName = dataReader.DatabaseName;
		ProcedureName = dataReader.ProcedureName;
		ProcedureSchema = dataReader.ProcedureSchema;
		Position = dataReader.Position;
		DataType = dataReader.Datatype;
		ParameterMode = dataReader.ParameterMode;
		DataLength = (string.IsNullOrEmpty(dataReader.DataLength) ? null : dataReader.DataLength);
		base.Description = dataReader.Description;
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(dataReader);
		}
	}

	public ParameterRow(ParameterSynchronizationObjectForInterfaceTables dataReader, CustomFieldsSupport customFieldsSupport = null)
	{
		base.Name = PrepareValue.ToString(dataReader.Name);
		DatabaseName = dataReader.DatabaseName;
		ProcedureName = dataReader.ProcedureName;
		ProcedureSchema = dataReader.ProcedureSchema;
		Position = dataReader.Position;
		DataType = dataReader.Datatype;
		ParameterMode = dataReader.ParameterMode;
		DataLength = (string.IsNullOrEmpty(dataReader.DataLength) ? null : dataReader.DataLength);
		base.Description = dataReader.Description;
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(dataReader);
		}
	}

	public ParameterRow(ParameterSynchronizationObjectForNeo4j dataReader, CustomFieldsSupport customFieldsSupport = null)
	{
		base.Name = PrepareValue.ToString(dataReader.Name);
		DatabaseName = dataReader.DatabaseName;
		ProcedureName = dataReader.ProcedureName;
		ProcedureSchema = dataReader.ProcedureSchema;
		Position = dataReader.Position;
		DataType = dataReader.Datatype;
		ParameterMode = dataReader.ParameterMode;
		DataLength = (string.IsNullOrEmpty(dataReader.DataLength) ? null : dataReader.DataLength);
		if (customFieldsSupport != null)
		{
			base.CustomFields = new CustomFieldContainer(customFieldsSupport);
			base.CustomFields.RetrieveCustomFields(dataReader);
		}
	}

	public static ModeEnum? GetMode(string mode)
	{
		switch (mode)
		{
		case "IN":
			return ModeEnum.In;
		case "OUT":
			return ModeEnum.Out;
		case "INOUT":
		case "IN/OUT":
			return ModeEnum.InOut;
		default:
			return null;
		}
	}

	public static string GetModeText(ModeEnum mode)
	{
		return mode switch
		{
			ModeEnum.In => "IN", 
			ModeEnum.Out => "OUT", 
			ModeEnum.InOut => "INOUT", 
			_ => null, 
		};
	}

	private void RetrieveCommonData(ParameterObject row, bool getDataTypeWithoutLength = false)
	{
		base.Id = row.ParameterId;
		base.Name = row.Name;
		Position = row.OrdinalPosition;
		base.Description = row.Description;
		DataType = row.Datatype;
		DataLength = row.DataLength;
		if (getDataTypeWithoutLength)
		{
			DataType = row.DatatypeWithoutLength;
		}
		ParameterMode = row.ParameterMode;
		LastModificationDateString = PrepareValue.SetDateTimeWithFormatting(row.LastModificationDate);
		ModifiedBy = row.ModifiedBy;
		CreationDateString = PrepareValue.SetDateTimeWithFormatting(row.CreationDate);
		CreatedBy = row.CreatedBy;
		Mode = GetMode(row.ParameterMode);
		base.Source = UserTypeEnum.ObjectToTypeOrDbms(row.Source);
	}

	public void GetPropertyError(string propertyName, ErrorInfo info)
	{
		if (!(propertyName != "Name"))
		{
			if (AlreadyExists)
			{
				info.ErrorText = "Parameter names can not be duplicated";
				info.ErrorType = ErrorType.Critical;
			}
			else if (IsNameEmpty)
			{
				info.ErrorText = "Parameter name can not be empty";
				info.ErrorType = ErrorType.Critical;
			}
		}
	}

	public void GetError(ErrorInfo info)
	{
	}
}
