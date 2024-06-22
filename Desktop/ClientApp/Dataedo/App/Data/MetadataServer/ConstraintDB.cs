using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class ConstraintDB : CommonDBSupport
{
	public ConstraintDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<UniqueConstraintObject> GetDataByTable(int tableId, bool notDeletedOnly = false)
	{
		return commands.Select.Constraints.GetUniqueConstraints(tableId, GetNotStatusValue(notDeletedOnly));
	}

	public List<UniqueConstraintWithColumnObject> GetDataByTableWithColumns(int tableId, bool notDeletedOnly = false)
	{
		return commands.Select.Constraints.GetUniqueConstraintsWithColumns(tableId, GetNotStatusValue(notDeletedOnly));
	}

	public List<UniqueConstraintWithColumnObject> GetDataWithColumnsByTableDoc(int tableId, bool notDeletedOnly = false)
	{
		return commands.Select.Constraints.GetUniqueConstraintsWithColumns(tableId, GetNotStatusValue(notDeletedOnly));
	}

	public BindingList<UniqueConstraintRow> GetDataObjectByTableId(int tableId, bool notDeletedOnly = false, CustomFieldsSupport customFieldsSupport = null)
	{
		return new BindingList<UniqueConstraintRow>(GroupUniqueConstraints(from UniqueConstraintWithColumnObject constraint in commands.Select.Constraints.GetUniqueConstraintsWithColumns(tableId)
			select new UniqueConstraintRow(constraint, customFieldsSupport)).ToList());
	}

	public static IEnumerable<UniqueConstraintRow> GroupUniqueConstraints(IEnumerable<UniqueConstraintRow> uniqueConstraints)
	{
		return (from x in uniqueConstraints
			group x by x.Id).Select(delegate(IGrouping<int, UniqueConstraintRow> x)
		{
			UniqueConstraintRow uniqueConstraintRow = x.First();
			if (x.Count() == 1)
			{
				return uniqueConstraintRow;
			}
			uniqueConstraintRow.Columns = new BindingList<UniqueConstraintColumnRow>(x.SelectMany((UniqueConstraintRow y) => y.Columns).ToList());
			return uniqueConstraintRow;
		}).ToList();
	}

	public int Synchronize(IEnumerable<UniqueConstraintRow> uniqueConstraints, string tableName, string schema, int databaseId, bool isDbAdded, int updateId, Form owner = null)
	{
		try
		{
			return commands.Synchronization.Constraints.SynchronizeUniqueConstraints(uniqueConstraints.Select((UniqueConstraintRow r) => ConvertRowToSynchronizationItem(r, tableName, schema, databaseId, updateId)).ToArray(), tableName, schema, databaseId, isDbAdded, updateId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing the unique constraint:", owner);
			return -1;
		}
	}

	private UniqueConstraintForSynchronization ConvertRowToSynchronizationItem(UniqueConstraintRow row, string tableName, string schema, int databaseId, int updateId)
	{
		UniqueConstraintForSynchronization uniqueConstraintForSynchronization = new UniqueConstraintForSynchronization
		{
			BaseName = tableName,
			BaseSchema = schema,
			BaseId = databaseId,
			Name = row.Name,
			IsPrimaryKey = row.IsPK,
			Description = row.Description,
			IsDisabled = row.Disabled,
			UpdateId = updateId
		};
		SetCustomFields(uniqueConstraintForSynchronization, row);
		return uniqueConstraintForSynchronization;
	}

	public bool Update(UniqueConstraintRow uniqueConstraintRow, Form owner = null)
	{
		try
		{
			List<ObjectIdColumnId> list = new List<ObjectIdColumnId>();
			foreach (UniqueConstraintColumnRow column in uniqueConstraintRow.Columns)
			{
				if (column.ColumnId > 0)
				{
					list.Add(new ObjectIdColumnId
					{
						BaseId = uniqueConstraintRow.Id,
						ColumnId = column.ColumnId,
						OrdinalPosition = column.OrdinalPosition
					});
				}
			}
			ObjectUniqueConstraint objectUniqueConstraint = ConvertRowToTable(uniqueConstraintRow, list);
			if (objectUniqueConstraint.IsUserDefined)
			{
				commands.Manipulation.Constraints.UpdateUserUniqueConstraintAndColumns(objectUniqueConstraint);
			}
			else
			{
				commands.Manipulation.Constraints.UpdateUniqueConstraintAndColumns(objectUniqueConstraint);
			}
			uniqueConstraintRow.Id = objectUniqueConstraint.UniqueConstraintId;
			uniqueConstraintRow.CustomFields.SetObjectId(uniqueConstraintRow.Id);
			uniqueConstraintRow.CustomFields.SetObjectType(SharedObjectTypeEnum.ObjectType.Key);
			uniqueConstraintRow.CustomFields.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
			uniqueConstraintRow.SetUnchanged();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the constraint:", owner);
			return false;
		}
		return true;
	}

	public bool Insert(UniqueConstraintRow uniqueConstraintRow, Form owner = null)
	{
		try
		{
			List<ObjectIdColumnId> list = new List<ObjectIdColumnId>();
			foreach (UniqueConstraintColumnRow column in uniqueConstraintRow.Columns)
			{
				if (column.ColumnId != -1)
				{
					list.Add(new ObjectIdColumnId
					{
						BaseId = uniqueConstraintRow.Id,
						ColumnId = column.ColumnId,
						OrdinalPosition = column.OrdinalPosition
					});
				}
			}
			ObjectUniqueConstraint objectUniqueConstraint = ConvertRowToTable(uniqueConstraintRow, list);
			commands.Manipulation.Constraints.InsertUniqueConstraintAndColumns(objectUniqueConstraint);
			uniqueConstraintRow.Id = objectUniqueConstraint.UniqueConstraintId;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting the constraint:", owner);
			return false;
		}
		return true;
	}

	private ObjectUniqueConstraint ConvertRowToTable(UniqueConstraintRow row, List<ObjectIdColumnId> objectsToUse)
	{
		ObjectUniqueConstraint objectUniqueConstraint = new ObjectUniqueConstraint
		{
			UniqueConstraintId = row.Id,
			TableId = row.TableId,
			Source = UserTypeEnum.TypeToString(row.Source),
			Name = row.NotEmptyName,
			Description = row.Description,
			IsPrimaryKey = row.IsPK,
			Columns = objectsToUse.ToArray(),
			IsUserDefined = (row.Source == UserTypeEnum.UserType.USER)
		};
		SetCustomFields(objectUniqueConstraint, row);
		return objectUniqueConstraint;
	}

	public bool Delete(BindingList<int> uniqueConstraints, Form owner = null)
	{
		try
		{
			commands.Manipulation.Constraints.DeleteUniqueConstraints(uniqueConstraints.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the constraint:", owner);
			return false;
		}
		return true;
	}
}
