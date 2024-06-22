using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.Procedures.Parameters;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class ParameterDoc : ObjectDoc
{
	public string DataType { get; set; }

	public Bitmap Icon { get; set; }

	public override string NewLineSeparator => ObjectDoc.HtmlNewLineSeparator;

	public override string CustomFieldsStringValuesSeparator => NewLineSeparator;

	public ParameterDoc()
	{
	}

	public ParameterDoc(DocGeneratingOptions docGeneratingOptions, ParameterObject row)
		: base(docGeneratingOptions, row, SharedObjectTypeEnum.ObjectType.Parameter)
	{
		DataType = row.Datatype;
		ParameterRow.ModeEnum? mode = ParameterRow.GetMode(row.ParameterMode);
		if (mode.HasValue)
		{
			switch (mode.GetValueOrDefault())
			{
			case ParameterRow.ModeEnum.In:
				Icon = Resources.parameter_in_64;
				break;
			case ParameterRow.ModeEnum.Out:
				Icon = Resources.parameter_in_out_64;
				break;
			case ParameterRow.ModeEnum.InOut:
				Icon = Resources.parameter_in_out_64;
				break;
			}
		}
	}

	public static BindingList<ParameterDoc> GetParameters(DocGeneratingOptions docGeneratingOptions, int procedureId, Form owner = null)
	{
		try
		{
			return new BindingList<ParameterDoc>(new List<ParameterDoc>(from parameter in DB.Parameter.GetDataByProcedureDoc(procedureId)
				select new ParameterDoc(docGeneratingOptions, parameter)));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting parameters from the procedure.", owner);
			return null;
		}
	}
}
