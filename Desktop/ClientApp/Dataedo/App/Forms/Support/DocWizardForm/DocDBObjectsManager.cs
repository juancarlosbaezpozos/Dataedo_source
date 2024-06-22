using System.ComponentModel;
using Dataedo.App.Enums;
using Dataedo.App.Licences;
using Dataedo.App.Tools;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;

namespace Dataedo.App.Forms.Support.DocWizardForm;

public class DocDBObjectsManager
{
	public static int NextId { get; set; }

	public static BindingList<DocDBObject> GetExportObjects(DocFormatEnum.DocFormat format, bool canExportBG = false)
	{
		bool flag = true;
		BindingList<DocDBObject> bindingList = new BindingList<DocDBObject>();
		if (format == DocFormatEnum.DocFormat.HTML)
		{
			DocDBObject docDBObject = new DocDBObject(CustomExcludedTypeEnum.CustomExcludedType.Documentation, flag, 0, flag);
			bindingList.Add(docDBObject);
			bindingList.Add(new DocDBObject(CustomExcludedTypeEnum.CustomExcludedType.DatabaseName, docDBObject.Id, flag));
			bindingList.Add(new DocDBObject(CustomExcludedTypeEnum.CustomExcludedType.HostName, docDBObject.Id, flag));
		}
		bool? isEnabled = flag;
		DocDBObject docDBObject2 = new DocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Table, 0, flag, null, isEnabled);
		bindingList.Add(docDBObject2);
		int id = docDBObject2.Id;
		isEnabled = flag;
		bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Key, id, flag, null, isEnabled));
		int id2 = docDBObject2.Id;
		isEnabled = flag;
		DocDBObject docDBObject3 = new DocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Trigger, id2, flag, null, isEnabled);
		bindingList.Add(docDBObject3);
		if (format != DocFormatEnum.DocFormat.Excel)
		{
			int id3 = docDBObject3.Id;
			isEnabled = flag;
			DocDBObject docDBObject4 = new DocDBObject(SharedObjectTypeEnum.ObjectType.Trigger, SharedObjectTypeEnum.ObjectType.Script, id3, flag, null, isEnabled);
			docDBObject4.SetParentObjectType(SharedObjectTypeEnum.ObjectType.Table);
			bindingList.Add(docDBObject4);
		}
		int id4 = docDBObject2.Id;
		isEnabled = flag;
		bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Relation, id4, flag, null, isEnabled));
		if (format != DocFormatEnum.DocFormat.Excel)
		{
			int id5 = docDBObject2.Id;
			isEnabled = flag;
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Dependency, id5, flag, null, isEnabled));
		}
		isEnabled = flag;
		DocDBObject docDBObject5 = new DocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.View, 0, flag, null, isEnabled);
		bindingList.Add(docDBObject5);
		int id6 = docDBObject5.Id;
		isEnabled = flag;
		bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Key, id6, flag, null, isEnabled));
		int id7 = docDBObject5.Id;
		isEnabled = flag;
		DocDBObject docDBObject6 = new DocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Trigger, id7, flag, null, isEnabled);
		bindingList.Add(docDBObject6);
		if (format != DocFormatEnum.DocFormat.Excel)
		{
			int id8 = docDBObject6.Id;
			isEnabled = flag;
			DocDBObject docDBObject7 = new DocDBObject(SharedObjectTypeEnum.ObjectType.Trigger, SharedObjectTypeEnum.ObjectType.Script, id8, flag, null, isEnabled);
			docDBObject7.SetParentObjectType(SharedObjectTypeEnum.ObjectType.View);
			bindingList.Add(docDBObject7);
		}
		int id9 = docDBObject5.Id;
		isEnabled = flag;
		bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Relation, id9, flag, null, isEnabled));
		if (format != DocFormatEnum.DocFormat.Excel)
		{
			int id10 = docDBObject5.Id;
			isEnabled = flag;
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Dependency, id10, flag, null, isEnabled));
			int id11 = docDBObject5.Id;
			isEnabled = flag;
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Script, id11, flag, null, isEnabled));
		}
		isEnabled = flag;
		DocDBObject docDBObject8 = new DocDBObject(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Function, 0, flag, null, isEnabled);
		bindingList.Add(docDBObject8);
		if (format != DocFormatEnum.DocFormat.Excel)
		{
			int id12 = docDBObject8.Id;
			isEnabled = flag;
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Dependency, id12, flag, null, isEnabled));
			int id13 = docDBObject8.Id;
			isEnabled = flag;
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Script, id13, flag, null, isEnabled));
		}
		isEnabled = flag;
		DocDBObject docDBObject9 = new DocDBObject(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Procedure, 0, flag, null, isEnabled);
		bindingList.Add(docDBObject9);
		if (format != DocFormatEnum.DocFormat.Excel)
		{
			int id14 = docDBObject9.Id;
			isEnabled = flag;
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Dependency, id14, flag, null, isEnabled));
			int id15 = docDBObject9.Id;
			isEnabled = flag;
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Script, id15, flag, null, isEnabled));
		}
		isEnabled = flag;
		DocDBObject docDBObject10 = new DocDBObject(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Structure, 0, flag, null, isEnabled);
		bindingList.Add(docDBObject10);
		int id16 = docDBObject10.Id;
		isEnabled = flag;
		bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Key, id16, flag, null, isEnabled));
		int id17 = docDBObject10.Id;
		isEnabled = flag;
		bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Relation, id17, flag, null, isEnabled));
		if (format != DocFormatEnum.DocFormat.Excel)
		{
			int id18 = docDBObject10.Id;
			isEnabled = flag;
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Dependency, id18, flag, null, isEnabled));
			int id19 = docDBObject10.Id;
			isEnabled = flag;
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.Structure, SharedObjectTypeEnum.ObjectType.Schema, id19, flag, null, isEnabled));
		}
		isEnabled = flag;
		DocDBObject item = new DocDBObject(SharedObjectTypeEnum.ObjectType.Erd, SharedObjectTypeEnum.ObjectType.Erd, 0, flag, null, isEnabled);
		bindingList.Add(item);
		if (format == DocFormatEnum.DocFormat.HTML && canExportBG && Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary))
		{
			DocDBObject docDBObject11 = new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.BusinessGlossary, 0, isCheckedByDefault: true, isEnabled: flag, docFormat: format);
			bindingList.Add(docDBObject11);
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Term, flag, docDBObject11.Id, flag));
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Category, flag, docDBObject11.Id, flag));
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Rule, flag, docDBObject11.Id, flag));
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Policy, flag, docDBObject11.Id, flag));
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Other, flag, docDBObject11.Id, flag));
		}
		else if (format == DocFormatEnum.DocFormat.PDF)
		{
			DocDBObject docDBObject12 = new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.BusinessGlossary, 0, isEnabled: flag, isCheckedByDefault: flag, docFormat: format);
			bindingList.Add(docDBObject12);
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Term, flag, docDBObject12.Id, flag));
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Category, flag, docDBObject12.Id, flag));
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Rule, flag, docDBObject12.Id, flag));
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Policy, flag, docDBObject12.Id, flag));
			bindingList.Add(new DocDBObject(SharedObjectTypeEnum.ObjectType.BusinessGlossary, SharedObjectTypeEnum.ObjectType.Other, flag, docDBObject12.Id, flag));
		}
		return bindingList;
	}

	private static BindingList<DocDBObject> GetCommentsDBObjects()
	{
		BindingList<DocDBObject> bindingList = new BindingList<DocDBObject>();
		DescriptionDocDBObject descriptionDocDBObject = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.UnresolvedEntity, SharedObjectTypeEnum.ObjectType.Table);
		bindingList.Add(descriptionDocDBObject);
		DescriptionDocDBObject item = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Table, descriptionDocDBObject.Id);
		bindingList.Add(item);
		bindingList.Add(new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Column, descriptionDocDBObject.Id));
		DescriptionDocDBObject descriptionDocDBObject2 = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.UnresolvedEntity, SharedObjectTypeEnum.ObjectType.View);
		bindingList.Add(descriptionDocDBObject2);
		DescriptionDocDBObject item2 = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.View, descriptionDocDBObject2.Id);
		bindingList.Add(item2);
		bindingList.Add(new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Column, descriptionDocDBObject2.Id));
		return bindingList;
	}

	private static BindingList<DocDBObject> GetPostgreDBObjects()
	{
		BindingList<DocDBObject> bindingList = new BindingList<DocDBObject>();
		DescriptionDocDBObject descriptionDocDBObject = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.UnresolvedEntity, SharedObjectTypeEnum.ObjectType.Table);
		bindingList.Add(descriptionDocDBObject);
		DescriptionDocDBObject item = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Table, descriptionDocDBObject.Id);
		bindingList.Add(item);
		bindingList.Add(new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Column, descriptionDocDBObject.Id));
		return bindingList;
	}

	private static BindingList<DocDBObject> GetSnowflakeDBObjects()
	{
		BindingList<DocDBObject> bindingList = new BindingList<DocDBObject>();
		DescriptionDocDBObject descriptionDocDBObject = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.UnresolvedEntity, SharedObjectTypeEnum.ObjectType.Table);
		bindingList.Add(descriptionDocDBObject);
		DescriptionDocDBObject item = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Table, descriptionDocDBObject.Id);
		bindingList.Add(item);
		bindingList.Add(new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Column, descriptionDocDBObject.Id));
		DescriptionDocDBObject descriptionDocDBObject2 = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.UnresolvedEntity, SharedObjectTypeEnum.ObjectType.View);
		bindingList.Add(descriptionDocDBObject2);
		DescriptionDocDBObject item2 = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.View, descriptionDocDBObject2.Id);
		bindingList.Add(item2);
		return bindingList;
	}

	private static BindingList<DocDBObject> GetExtendedPropertiesDBObjects()
	{
		BindingList<DocDBObject> bindingList = new BindingList<DocDBObject>();
		DescriptionDocDBObject descriptionDocDBObject = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.UnresolvedEntity, SharedObjectTypeEnum.ObjectType.Table);
		bindingList.Add(descriptionDocDBObject);
		bindingList.Add(new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Table, descriptionDocDBObject.Id));
		bindingList.Add(new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Column, descriptionDocDBObject.Id));
		bindingList.Add(new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Key, descriptionDocDBObject.Id));
		DescriptionDocDBObject item = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Table, SharedObjectTypeEnum.ObjectType.Trigger, descriptionDocDBObject.Id);
		bindingList.Add(item);
		DescriptionDocDBObject descriptionDocDBObject2 = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.UnresolvedEntity, SharedObjectTypeEnum.ObjectType.View);
		bindingList.Add(descriptionDocDBObject2);
		DescriptionDocDBObject item2 = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.View, descriptionDocDBObject2.Id);
		bindingList.Add(item2);
		bindingList.Add(new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.View, SharedObjectTypeEnum.ObjectType.Column, descriptionDocDBObject2.Id));
		DescriptionDocDBObject descriptionDocDBObject3 = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.UnresolvedEntity, SharedObjectTypeEnum.ObjectType.Function);
		bindingList.Add(descriptionDocDBObject3);
		DescriptionDocDBObject item3 = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Function, descriptionDocDBObject3.Id);
		bindingList.Add(item3);
		bindingList.Add(new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Parameter, descriptionDocDBObject3.Id));
		DescriptionDocDBObject descriptionDocDBObject4 = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.UnresolvedEntity, SharedObjectTypeEnum.ObjectType.Procedure);
		bindingList.Add(descriptionDocDBObject4);
		DescriptionDocDBObject item4 = new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Procedure, descriptionDocDBObject4.Id);
		bindingList.Add(item4);
		bindingList.Add(new DescriptionDocDBObject(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Parameter, descriptionDocDBObject4.Id));
		return bindingList;
	}

	public static BindingList<DocDBObject> GetExportObjects(LoadObjectTypeEnum type)
	{
		BindingList<DocDBObject> result = new BindingList<DocDBObject>();
		switch (type)
		{
		case LoadObjectTypeEnum.ExtendedPropertiesObjects:
			result = GetExtendedPropertiesDBObjects();
			break;
		case LoadObjectTypeEnum.OracleComments:
			result = GetCommentsDBObjects();
			break;
		case LoadObjectTypeEnum.PostgreComments:
			result = GetPostgreDBObjects();
			break;
		case LoadObjectTypeEnum.SnowflakeComments:
			result = GetSnowflakeDBObjects();
			break;
		}
		return result;
	}
}
