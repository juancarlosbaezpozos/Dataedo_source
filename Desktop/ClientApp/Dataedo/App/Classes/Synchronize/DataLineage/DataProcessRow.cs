using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.DataLineage;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize.DataLineage;

public class DataProcessRow : BasicRow, IName
{
    public const string AUTOMATIC_PROCESS_NAME = "Automatic lineage";

    public Guid Guid { get; } = Guid.NewGuid();


    public int ParentId { get; set; }

    public int DatabaseId { get; set; }

    public string CreationDateString { get; set; }

    public string CreatedBy { get; set; }

    public string LastModificationDateString { get; set; }

    public string ModifiedBy { get; set; }

    public string ParentName { get; set; }

    public string ParentSchema { get; set; }

    public string Script { get; set; }

    public BindingList<DataFlowRow> InflowRows { get; set; } = new BindingList<DataFlowRow>();


    public BindingList<DataFlowRow> OutflowRows { get; set; } = new BindingList<DataFlowRow>();


    public List<DataLineageColumnsFlowRow> Columns { get; set; } = new List<DataLineageColumnsFlowRow>();


    public DataProcessRow()
    {
    }

    public DataProcessRow(DataProcessObject dataProcessObject)
    {
        base.Id = dataProcessObject.ProcessId;
        base.ParentObjectType = SharedObjectTypeEnum.StringToType(dataProcessObject.ProcessorType);
        ParentId = dataProcessObject.ProcessorId;
        LastModificationDateString = PrepareValue.SetDateTimeWithFormatting(dataProcessObject.LastModificationDate);
        ModifiedBy = dataProcessObject.ModifiedBy;
        CreationDateString = PrepareValue.SetDateTimeWithFormatting(dataProcessObject.CreationDate);
        CreatedBy = dataProcessObject.CreatedBy;
        base.Source = UserTypeEnum.ObjectToType(dataProcessObject.Source) ?? UserTypeEnum.UserType.USER;
        base.Name = dataProcessObject.Name;
        DatabaseId = dataProcessObject.ProcessorDatabaseId;
        Script = dataProcessObject.Script;
    }

    public override bool CanBeDeleted()
    {
        return true;
    }

    public static DataProcessRow AddDefaultProcess(IFlowDraggable objectNode, Form parentForm)
    {
        int id = objectNode.Id;
        SharedObjectTypeEnum.ObjectType objectType = objectNode.ObjectType;
        if (objectType != SharedObjectTypeEnum.ObjectType.Function && objectType != SharedObjectTypeEnum.ObjectType.Procedure && objectType != SharedObjectTypeEnum.ObjectType.View)
        {
            return null;
        }
        DataProcessRow dataProcessRow = new DataProcessRow
        {
            Name = "Default process",
            ParentId = id,
            ParentObjectType = objectType,
            Source = UserTypeEnum.UserType.USER
        };
        int? num = DB.DataProcess.Insert(dataProcessRow, parentForm);
        if (num.HasValue && num.HasValue)
        {
            dataProcessRow.Id = num.Value;
            if (objectType == SharedObjectTypeEnum.ObjectType.View)
            {
                DataFlowRow dataFlowRow = new DataFlowRow(num.Value, dataProcessRow.Name, isProcessInSameProcessor: true, "OUT", objectNode.Id, objectNode.ObjectType, objectNode.Subtype, objectNode.Source ?? UserTypeEnum.UserType.USER, UserTypeEnum.UserType.USER, objectNode.DatabaseTitle, objectNode.DatabaseId, objectNode.BaseName, objectNode.Title);
                DB.DataFlows.Insert(dataFlowRow, parentForm);
            }
            return dataProcessRow;
        }
        return null;
    }

    [SpecialName]
    string Name
    {
        get
        {
            return base.Name;
        }
        set
        {
            base.Name = value;
        }
    }
}
