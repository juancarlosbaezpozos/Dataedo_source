using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Dataedo.Model.Data.SchemaImportsAndChanges;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.UserControls.SchemaImportsAndChanges.Model;

[DebuggerDisplay("{DateOperationObject}")]
public class SchemaImportsAndChangesObjectModel : INotifyPropertyChanged
{
	public SchemaImportsAndChangesObjectModel ParentNode { get; set; }

	public SchemaImportsAndChangesObject Data { get; set; }

	public List<SchemaImportsAndChangesObject> AllData { get; private set; }

	public SchemaImportsAndChangesBindingList Children { get; set; }

	public List<DifferenceModel> Differences { get; set; }

	public SchemaChangeLevelEnum.SchemaChangeLevel Level { get; set; }

	public bool ShowSchema { get; private set; }

	public bool IsCommentModified { get; set; }

	public string ConnectionDetails
	{
		get
		{
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date)
			{
				return Data?.ConnectionDetails;
			}
			return null;
		}
	}

	public string User
	{
		get
		{
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date)
			{
				return Data?.User;
			}
			return null;
		}
	}

	public string DBMSVersion
	{
		get
		{
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date)
			{
				return Data?.DBMSVersion;
			}
			return null;
		}
	}

	public string DateOperationObject
	{
		get
		{
			switch (Level)
			{
			case SchemaChangeLevelEnum.SchemaChangeLevel.NoResults:
				NoChangesFound = true;
				return "No changes found";
			case SchemaChangeLevelEnum.SchemaChangeLevel.LicenseWitoutSCT:
				return "Schema Change Tracking not available in your plan";
			case SchemaChangeLevelEnum.SchemaChangeLevel.Root:
				return Data?.DatabaseId?.ToString();
			case SchemaChangeLevelEnum.SchemaChangeLevel.Date:
			{
				SchemaImportsAndChangesObject data = Data;
				if (data != null && data.AddedCount == 0)
				{
					SchemaImportsAndChangesObject data2 = Data;
					if (data2 != null && data2.UpdatedCount == 0)
					{
						SchemaImportsAndChangesObject data3 = Data;
						if (data3 != null && data3.DeletedCount == 0)
						{
							NoChangesFound = true;
							return UpdateDateString + " - No changes found";
						}
					}
				}
				if (IsObjectImportDateLevel)
				{
					return UpdateDateString + FormatAddedUpdatedDeletedForObject();
				}
				return UpdateDateString + FormatAddedUpdatedDeleted();
			}
			case SchemaChangeLevelEnum.SchemaChangeLevel.Operation:
			{
				string text = string.Empty;
				if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Added)
				{
					text = "Added";
				}
				else if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Updated)
				{
					text = "Updated";
				}
				else if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Deleted)
				{
					text = "Deleted";
				}
				if (Children.Count == 1)
				{
					return text + " 1 object";
				}
				return $"{text} {Children.Count} objects";
			}
			case SchemaChangeLevelEnum.SchemaChangeLevel.Object:
				return ObjectDisplayName;
			case SchemaChangeLevelEnum.SchemaChangeLevel.Element:
				return ColumnNames.GetFullNameFormatted(Data?.Path, Data?.ElementName);
			default:
				return string.Empty;
			}
		}
	}

	public string Operation
	{
		get
		{
			if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Added)
			{
				return "added";
			}
			if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Updated)
			{
				return "updated";
			}
			if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Deleted)
			{
				return "deleted";
			}
			return Level switch
			{
				SchemaChangeLevelEnum.SchemaChangeLevel.Object => Data?.ObjectChange, 
				SchemaChangeLevelEnum.SchemaChangeLevel.Element => Data?.ElementChange, 
				_ => string.Empty, 
			};
		}
	}

	public string Type
	{
		get
		{
			string text = string.Empty;
			switch (Level)
			{
			case SchemaChangeLevelEnum.SchemaChangeLevel.Date:
				if (IsObjectImportDateLevel)
				{
					text = SharedObjectSubtypeEnum.TypeToStringForSingle(ObjectType, ObjectSubtype);
				}
				break;
			case SchemaChangeLevelEnum.SchemaChangeLevel.Operation:
				if (IsObjectImportDateLevel)
				{
					text = SharedObjectSubtypeEnum.TypeToStringForSingle(ObjectType, ObjectSubtype);
				}
				break;
			case SchemaChangeLevelEnum.SchemaChangeLevel.Object:
				text = SharedObjectSubtypeEnum.TypeToStringForSingle(ObjectType, ObjectSubtype);
				break;
			case SchemaChangeLevelEnum.SchemaChangeLevel.Element:
				text = SharedObjectSubtypeEnum.TypeToStringForSingle(ElementType, ElementSubtype);
				if (ElementType == SharedObjectTypeEnum.ObjectType.Key)
				{
					SchemaImportsAndChangesObject data = Data;
					text = ((data == null || data.PrimaryKey != true) ? ("Unique " + text?.ToLower()) : ("Primary " + text?.ToLower()));
				}
				break;
			}
			return text;
		}
	}

	public string TypeWithState
	{
		get
		{
			string text = Type;
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element)
			{
				SchemaImportsAndChangesObject data = Data;
				if (data != null && data.Disabled == true)
				{
					text += " disabled";
				}
			}
			return text;
		}
	}

	public SchemaChangeTypeEnum.SchemaChangeType ChangeType
	{
		get
		{
			string schemaChangeChangeTypeString = null;
			switch (Level)
			{
			case SchemaChangeLevelEnum.SchemaChangeLevel.Date:
				if (IsObjectImportDateLevel && !NoChangesFound)
				{
					schemaChangeChangeTypeString = DateOperationObject;
				}
				break;
			case SchemaChangeLevelEnum.SchemaChangeLevel.Operation:
				schemaChangeChangeTypeString = Data?.ObjectChange;
				break;
			case SchemaChangeLevelEnum.SchemaChangeLevel.Object:
				schemaChangeChangeTypeString = Data?.ObjectChange;
				break;
			case SchemaChangeLevelEnum.SchemaChangeLevel.Element:
				schemaChangeChangeTypeString = Data?.ElementChange;
				break;
			}
			return SchemaChangeTypeEnum.StringToType(schemaChangeChangeTypeString);
		}
	}

	public string ObjectDisplayName => ((!ShowSchema || string.IsNullOrEmpty(Data?.ObjectSchema)) ? string.Empty : (Data.ObjectSchema + ".")) + Data?.ObjectName;

	public string FullChangeText
	{
		get
		{
			string text = null;
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element || IsObjectImportDateLevel)
			{
				text = (string.IsNullOrEmpty(Data?.ObjectSchema) ? string.Empty : (Data.ObjectSchema + ".")) + Data?.ObjectName;
			}
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element)
			{
				text = text + "." + ColumnNames.GetFullName(Data?.Path, Data?.ElementName);
			}
			string text2 = string.Empty;
			if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Added)
			{
				text2 = "added";
			}
			else if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Updated)
			{
				text2 = "updated";
			}
			else if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Deleted)
			{
				text2 = "deleted";
			}
			return text + " " + text2 + " on " + UpdateDateString;
		}
	}

	public string TypeWithActionText
	{
		get
		{
			string text = string.Empty;
			if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Added)
			{
				text = "added";
			}
			else if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Updated)
			{
				text = "updated";
			}
			else if (ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Deleted)
			{
				text = "deleted";
			}
			return TypeWithState + " " + text;
		}
	}

	public SharedObjectTypeEnum.ObjectType? ObjectType => SharedObjectTypeEnum.StringToType(Data?.ObjectType);

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype => SharedObjectSubtypeEnum.StringToType(ObjectType, Data?.ObjectSubtype);

	public bool IsMainType
	{
		get
		{
			if (!(Data?.ObjectType == Data?.ObjectSubtype))
			{
				return Data?.ObjectSubtype == null;
			}
			return true;
		}
	}

	public SharedObjectTypeEnum.ObjectType? ElementType => SharedObjectTypeEnum.StringToType(Data?.ElementType);

	public SharedObjectSubtypeEnum.ObjectSubtype ElementSubtype => SharedObjectSubtypeEnum.StringToType(ElementType, Data?.ElementSubtype ?? Data.ItemType);

	public string Comments
	{
		get
		{
			if (SupportsComments)
			{
				if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || IsObjectImportDateLevel)
				{
					return Data.ObjectDescription;
				}
				if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element)
				{
					return Data.ElementDescription;
				}
				if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date)
				{
					return Data.SchemaUpdateDescription;
				}
			}
			return null;
		}
		set
		{
			if (SupportsComments && Data != null)
			{
				if ((Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || IsObjectImportDateLevel) && Data.ObjectDescription != value)
				{
					Data.ObjectDescription = value;
					IsCommentModified = true;
				}
				else if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element && Data.ElementDescription != value)
				{
					Data.ElementDescription = value;
					IsCommentModified = true;
				}
				else if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date && Data.SchemaUpdateDescription != value)
				{
					Data.SchemaUpdateDescription = value;
					IsCommentModified = true;
				}
			}
		}
	}

	public string CommentedBy
	{
		get
		{
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || IsObjectImportDateLevel)
			{
				return Data?.ObjectDescriptionBy;
			}
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element)
			{
				return Data?.ElementDescriptionBy;
			}
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date)
			{
				return Data?.SchemaUpdateDescriptionBy;
			}
			return null;
		}
		set
		{
			if (Data != null)
			{
				if ((Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || IsObjectImportDateLevel) && Data.ObjectDescriptionBy != value)
				{
					Data.ObjectDescriptionBy = value;
					OnPropertyChanged("CommentedBy");
				}
				else if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element && Data.ElementDescriptionBy != value)
				{
					Data.ElementDescriptionBy = value;
					OnPropertyChanged("CommentedBy");
				}
				else if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date && Data.SchemaUpdateDescriptionBy != value)
				{
					Data.SchemaUpdateDescriptionBy = value;
					OnPropertyChanged("CommentedBy");
				}
			}
		}
	}

	public DateTime? CommentDate
	{
		get
		{
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || IsObjectImportDateLevel)
			{
				return Data?.ObjectDescriptionDate;
			}
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element)
			{
				return Data?.ElementDescriptionDate;
			}
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date)
			{
				return Data?.SchemaUpdateDescriptionDate;
			}
			return null;
		}
		set
		{
			if (Data != null)
			{
				if ((Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || IsObjectImportDateLevel) && Data.ObjectDescriptionDate != value)
				{
					Data.ObjectDescriptionDate = value;
					OnPropertyChanged("CommentDate");
				}
				else if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element && Data.ElementDescriptionDate != value)
				{
					Data.ElementDescriptionDate = value;
					OnPropertyChanged("CommentDate");
				}
				else if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date && Data.SchemaUpdateDescriptionDate != value)
				{
					Data.SchemaUpdateDescriptionDate = value;
					OnPropertyChanged("CommentDate");
				}
			}
		}
	}

	public string UpdateDateString => Data?.UpdateDate.ToString();

	public bool SupportsComments => MayHaveChanges;

	public bool MayHaveChanges
	{
		get
		{
			if (Level != SchemaChangeLevelEnum.SchemaChangeLevel.Object && Level != SchemaChangeLevelEnum.SchemaChangeLevel.Element && Level != SchemaChangeLevelEnum.SchemaChangeLevel.Date)
			{
				return IsObjectImportDateLevel;
			}
			return true;
		}
	}

	public bool NoChangesFound { get; private set; }

	public bool IsObjectImportDateLevel { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;

	public SchemaImportsAndChangesObjectModel(SchemaChangeLevelEnum.SchemaChangeLevel level, bool showSchema)
	{
		Level = level;
		ShowSchema = showSchema;
		Children = new SchemaImportsAndChangesBindingList();
	}

	public SchemaImportsAndChangesObjectModel(SchemaChangeLevelEnum.SchemaChangeLevel level, SchemaImportsAndChangesObject data, SchemaImportsAndChangesObjectModel parentNode, bool showSchema)
		: this(level, showSchema)
	{
		Data = data;
		ParentNode = parentNode;
		IsObjectImportDateLevel = !NoChangesFound && Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date && data?.ObjectType != null;
		ParentNode?.Children.Add(this);
		Differences = GetDifferences();
	}

	public SchemaImportsAndChangesObjectModel(SchemaChangeLevelEnum.SchemaChangeLevel level, SchemaImportsAndChangesObject data, List<SchemaImportsAndChangesObject> allData, SchemaImportsAndChangesObjectModel parentNode, bool showSchema)
		: this(level, showSchema)
	{
		Data = data;
		AllData = allData;
		ParentNode = parentNode;
		ParentNode?.Children.Add(this);
		Differences = GetDifferences();
	}

	public static IEnumerable<SchemaImportsAndChangesObjectModel> Flatten(SchemaImportsAndChangesObjectModel model)
	{
		List<SchemaImportsAndChangesObjectModel> list = new List<SchemaImportsAndChangesObjectModel> { model };
		foreach (SchemaImportsAndChangesObjectModel item in model?.Children)
		{
			list.AddRange(item.Flatten());
		}
		return list;
	}

	public IEnumerable<SchemaImportsAndChangesObjectModel> Flatten()
	{
		return Flatten(this);
	}

	protected void OnPropertyChanged(string name)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}

	private List<DifferenceModel> GetDifferences()
	{
		List<DifferenceModel> list = new List<DifferenceModel>();
		SchemaChangeTypeEnum.SchemaChangeType schemaChangeType = SchemaChangeTypeEnum.StringToType(Operation);
		bool flag = schemaChangeType == SchemaChangeTypeEnum.SchemaChangeType.Deleted;
		if (ObjectType == SharedObjectTypeEnum.ObjectType.Table || ObjectType == SharedObjectTypeEnum.ObjectType.View)
		{
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || IsObjectImportDateLevel)
			{
				if (ObjectType == SharedObjectTypeEnum.ObjectType.View)
				{
					list.Add(new DifferenceModel("Definition", Data.BeforeDefinition, Data.ObjectDefinition, flag));
				}
			}
			else if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element)
			{
				if (ElementType == SharedObjectTypeEnum.ObjectType.Column)
				{
					list.Add(new DifferenceModel("Type", (schemaChangeType == SchemaChangeTypeEnum.SchemaChangeType.Added || (flag && schemaChangeType == SchemaChangeTypeEnum.SchemaChangeType.Deleted)) ? null : SharedObjectSubtypeEnum.TypeToStringForSingle(ElementType, SharedObjectSubtypeEnum.StringToType(ElementType, Data.BeforeItemType)), SharedObjectSubtypeEnum.TypeToStringForSingle(ElementType, SharedObjectSubtypeEnum.StringToType(ElementType, Data.ItemType)), flag));
					list.Add(new DifferenceModel("Data type", Data.BeforeDatatype, Data.Datatype, flag));
					list.Add(new DifferenceModel("Length", Data.BeforeDataLength, Data.DataLength, flag));
					list.Add(new DifferenceModel("Nullable", Data.BeforeNullable, Data.Nullable, flag));
					list.Add(new DifferenceModel("Default value", Data.BeforeDefaultValue, Data.DefaultValue, flag));
					list.Add(new DifferenceModel("Identity", Data.BeforeIsIdentity, Data.IsIdentity, flag));
					list.Add(new DifferenceModel("Is computed", Data.BeforeIsComputed, Data.IsComputed, flag));
					list.Add(new DifferenceModel("Computed formula", Data.BeforeComputedFormula, Data.ComputedFormula, flag));
				}
				else if (ElementType == SharedObjectTypeEnum.ObjectType.Relation)
				{
					string text = (string.IsNullOrEmpty(Data.BeforeFkTableSchema) ? Data.BeforeFkTableName : (Data.BeforeFkTableSchema + "." + Data.BeforeFkTableName));
					string text2 = (string.IsNullOrEmpty(Data.BeforePkTableSchema) ? Data.BeforePkTableName : (Data.BeforePkTableSchema + "." + Data.BeforePkTableName));
					string text3 = (string.IsNullOrEmpty(Data.FkTableSchema) ? Data.FkTableName : (Data.FkTableSchema + "." + Data.FkTableName));
					string text4 = (string.IsNullOrEmpty(Data.BeforePkTableSchema) ? Data.PkTableName : (Data.PkTableSchema + "." + Data.PkTableName));
					list.Add(new DifferenceModel("Tables", (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2)) ? null : (text + " >- " + text2), (string.IsNullOrEmpty(text3) && string.IsNullOrEmpty(text4)) ? null : (text3 + " >- " + text4), flag));
					if (AllData != null)
					{
						List<SchemaImportsAndChangesObject> list2 = AllData.OrderBy((SchemaImportsAndChangesObject x) => x.ChildOrdinalPosition).ToList();
						List<string> list3 = new List<string>();
						List<string> list4 = new List<string>();
						for (int i = 0; i < AllData.Count; i++)
						{
							SchemaImportsAndChangesObject schemaImportsAndChangesObject = list2[i];
							SchemaChangeTypeEnum.StringToType(schemaImportsAndChangesObject.ChildOperation);
							if (!string.IsNullOrEmpty(schemaImportsAndChangesObject.ChildBeforeColumnFkName) && !string.IsNullOrEmpty(schemaImportsAndChangesObject.ChildBeforeColumnPkName))
							{
								if (flag)
								{
									list4.Add(schemaImportsAndChangesObject.ChildBeforeColumnFkName + " = " + schemaImportsAndChangesObject.ChildBeforeColumnPkName);
								}
								else
								{
									list3.Add(schemaImportsAndChangesObject.ChildBeforeColumnFkName + " = " + schemaImportsAndChangesObject.ChildBeforeColumnPkName);
								}
							}
							else if (flag)
							{
								list4.Add(string.Empty);
							}
							else
							{
								list3.Add(string.Empty);
							}
							if (!string.IsNullOrEmpty(schemaImportsAndChangesObject.ChildColumnFkName) && !string.IsNullOrEmpty(schemaImportsAndChangesObject.ChildColumnPkName))
							{
								if (flag)
								{
									list3.Add(schemaImportsAndChangesObject.ChildColumnFkName + " = " + schemaImportsAndChangesObject.ChildColumnPkName);
								}
								else
								{
									list4.Add(schemaImportsAndChangesObject.ChildColumnFkName + " = " + schemaImportsAndChangesObject.ChildColumnPkName);
								}
							}
							else if (flag)
							{
								list3.Add(string.Empty);
							}
							else
							{
								list4.Add(string.Empty);
							}
						}
						if (list3.Count > 0 && list3.Count == list4.Count)
						{
							list.Add(new DifferenceModel("Join", list3.Any((string x) => !string.IsNullOrEmpty(x)) ? string.Join(Environment.NewLine, list3) : null, list4.Any((string x) => !string.IsNullOrEmpty(x)) ? string.Join(Environment.NewLine, list4) : null));
						}
					}
				}
				else if (ElementType == SharedObjectTypeEnum.ObjectType.Key)
				{
					list.Add(new DifferenceModel("Primary key", Data.BeforePrimaryKey, Data.PrimaryKey, flag));
					list.Add(new DifferenceModel("Disabled", Data.BeforeDisabled, Data.Disabled, flag));
					if (AllData != null)
					{
						List<SchemaImportsAndChangesObject> list5 = AllData.OrderBy((SchemaImportsAndChangesObject x) => x.ChildOrdinalPosition).ToList();
						List<string> list6 = new List<string>();
						List<string> list7 = new List<string>();
						for (int j = 0; j < AllData.Count; j++)
						{
							SchemaImportsAndChangesObject schemaImportsAndChangesObject2 = list5[j];
							bool flag2 = SchemaChangeTypeEnum.StringToType(schemaImportsAndChangesObject2.ChildOperation) == SchemaChangeTypeEnum.SchemaChangeType.Deleted;
							if (!string.IsNullOrEmpty(schemaImportsAndChangesObject2.ChildBeforeColumnName))
							{
								if (flag2)
								{
									list7.Add(schemaImportsAndChangesObject2.ChildBeforeColumnName);
								}
								else
								{
									list6.Add(schemaImportsAndChangesObject2.ChildBeforeColumnName);
								}
							}
							if (!string.IsNullOrEmpty(schemaImportsAndChangesObject2.ChildColumnName))
							{
								if (flag2)
								{
									list6.Add(schemaImportsAndChangesObject2.ChildColumnName);
								}
								else
								{
									list7.Add(schemaImportsAndChangesObject2.ChildColumnName);
								}
							}
						}
						list.Add(new DifferenceModel("Columns", list6.Any((string x) => !string.IsNullOrEmpty(x)) ? string.Join(Environment.NewLine, list6) : null, list7.Any((string x) => !string.IsNullOrEmpty(x)) ? string.Join(Environment.NewLine, list7) : null));
					}
				}
				else if (ElementType == SharedObjectTypeEnum.ObjectType.Trigger)
				{
					list.Add(new DifferenceModel("Before", Data.BeforeBefore, Data.Before, flag));
					list.Add(new DifferenceModel("After", Data.BeforeAfter, Data.After, flag));
					list.Add(new DifferenceModel("Instead of", Data.BeforeInsteadOf, Data.InsteadOf, flag));
					list.Add(new DifferenceModel("On insert", Data.BeforeOnInsert, Data.OnInsert, flag));
					list.Add(new DifferenceModel("On update", Data.BeforeOnUpdate, Data.OnUpdate, flag));
					list.Add(new DifferenceModel("On delete", Data.BeforeOnDelete, Data.OnDelete, flag));
					list.Add(new DifferenceModel("Disabled", Data.BeforeDisabled, Data.Disabled, flag));
					list.Add(new DifferenceModel("Script", Data.BeforeElementDefinition, Data.ElementDefinition, flag));
				}
			}
		}
		else if (ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || ObjectType == SharedObjectTypeEnum.ObjectType.Function)
		{
			if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Object || IsObjectImportDateLevel)
			{
				list.Add(new DifferenceModel("Function type", Data.BeforeFunctionType, Data.FunctionType, flag));
				list.Add(new DifferenceModel("Script", Data.BeforeDefinition, Data.ObjectDefinition, flag));
			}
			else if (Level == SchemaChangeLevelEnum.SchemaChangeLevel.Element && ElementType == SharedObjectTypeEnum.ObjectType.Parameter)
			{
				list.Add(new DifferenceModel("Mode", Data.BeforeParameterMode, Data.ParameterMode, flag));
				list.Add(new DifferenceModel("Data type", Data.BeforeDatatype, Data.Datatype, flag));
				list.Add(new DifferenceModel("Length", Data.BeforeDataLength, Data.DataLength, flag));
				list.Add(new DifferenceModel("Position", Data.BeforeOrdinalPosition, Data.OrdinalPosition, flag));
			}
		}
		return list;
	}

	private string FormatAddedUpdatedDeletedForObject()
	{
		string text = string.Empty;
		SchemaImportsAndChangesObject data = Data;
		if (data != null && data.AddedCount == 0)
		{
			SchemaImportsAndChangesObject data2 = Data;
			if (data2 != null && data2.UpdatedCount == 0)
			{
				SchemaImportsAndChangesObject data3 = Data;
				if (data3 != null && data3.DeletedCount == 0)
				{
					goto IL_0057;
				}
			}
		}
		text += " ";
		goto IL_0057;
		IL_011d:
		return text;
		IL_0111:
		text += "deleted";
		goto IL_011d;
		IL_0057:
		SchemaImportsAndChangesObject data4 = Data;
		if (data4 == null || data4.AddedCount != 0)
		{
			text += "added";
		}
		SchemaImportsAndChangesObject data5 = Data;
		if (data5 == null || data5.UpdatedCount != 0)
		{
			SchemaImportsAndChangesObject data6 = Data;
			if (data6 == null || data6.AddedCount != 0)
			{
				text += ", ";
			}
			text += "updated";
		}
		SchemaImportsAndChangesObject data7 = Data;
		if (data7 == null || data7.DeletedCount != 0)
		{
			SchemaImportsAndChangesObject data8 = Data;
			if (data8 != null && data8.UpdatedCount == 0)
			{
				SchemaImportsAndChangesObject data9 = Data;
				if (data9 != null && data9.AddedCount == 0)
				{
					goto IL_0111;
				}
			}
			text += ", ";
			goto IL_0111;
		}
		goto IL_011d;
	}

	private string FormatAddedUpdatedDeleted()
	{
		string text = string.Empty;
		SchemaImportsAndChangesObject data = Data;
		if (data != null && data.AddedCount == 0)
		{
			SchemaImportsAndChangesObject data2 = Data;
			if (data2 != null && data2.UpdatedCount == 0)
			{
				SchemaImportsAndChangesObject data3 = Data;
				if (data3 != null && data3.DeletedCount == 0)
				{
					goto IL_0057;
				}
			}
		}
		text += " - ";
		goto IL_0057;
		IL_0198:
		return text;
		IL_0163:
		text += $"{Data?.DeletedCount} deleted";
		goto IL_0198;
		IL_0057:
		SchemaImportsAndChangesObject data4 = Data;
		if (data4 == null || data4.AddedCount != 0)
		{
			text += $"{Data?.AddedCount} added";
		}
		SchemaImportsAndChangesObject data5 = Data;
		if (data5 == null || data5.UpdatedCount != 0)
		{
			SchemaImportsAndChangesObject data6 = Data;
			if (data6 == null || data6.AddedCount != 0)
			{
				text += ", ";
			}
			text += $"{Data?.UpdatedCount} updated";
		}
		SchemaImportsAndChangesObject data7 = Data;
		if (data7 == null || data7.DeletedCount != 0)
		{
			SchemaImportsAndChangesObject data8 = Data;
			if (data8 != null && data8.UpdatedCount == 0)
			{
				SchemaImportsAndChangesObject data9 = Data;
				if (data9 != null && data9.AddedCount == 0)
				{
					goto IL_0163;
				}
			}
			text += ", ";
			goto IL_0163;
		}
		goto IL_0198;
	}

	private List<string> GetOperationsList()
	{
		List<string> list = new List<string>();
		int? num = Children?.Where((SchemaImportsAndChangesObjectModel x) => x.ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Added).Sum((SchemaImportsAndChangesObjectModel x) => x.Children?.Count);
		int? num2 = Children?.Where((SchemaImportsAndChangesObjectModel x) => x.ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Updated).Sum((SchemaImportsAndChangesObjectModel x) => x.Children?.Count);
		int? num3 = Children?.Where((SchemaImportsAndChangesObjectModel x) => x.ChangeType == SchemaChangeTypeEnum.SchemaChangeType.Deleted).Sum((SchemaImportsAndChangesObjectModel x) => x.Children?.Count);
		if (num > 0)
		{
			list.Add($"{num} {SchemaChangeTypeEnum.TypeToLowerString(SchemaChangeTypeEnum.SchemaChangeType.Added)}");
		}
		if (num2 > 0)
		{
			list.Add($"{num2} {SchemaChangeTypeEnum.TypeToLowerString(SchemaChangeTypeEnum.SchemaChangeType.Updated)}");
		}
		if (num3 > 0)
		{
			list.Add($"{num3} {SchemaChangeTypeEnum.TypeToLowerString(SchemaChangeTypeEnum.SchemaChangeType.Deleted)}");
		}
		return list;
	}
}
