using Dataedo.App.DataRepository.Repositories;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools.Export;

namespace Dataedo.App.DataRepository;

internal static class DataFactory
{
	public static IRepository Make()
	{
		return new DbCachedRepository();
	}

	public static IRepository Make(WizardOptions settings)
	{
		ExcludedObjects excludedObjects = new ExcludedObjects();
		foreach (ObjectTypeHierarchy item in settings.ExcludedObject)
		{
			if (item.CustomType.HasValue)
			{
				excludedObjects.ExcludeType(item.CustomType.Value);
			}
			else if (!item.ParentObjectType.HasValue && item.ObjectType.HasValue && item.ObjectSubtype.HasValue)
			{
				excludedObjects.ExcludeType(item.ObjectType.Value, item.ObjectSubtype.Value);
			}
			else if (item.ParentObjectType.HasValue && item.ObjectType.HasValue && item.ObjectSubtype.HasValue)
			{
				excludedObjects.ExcludeType(item.ParentObjectType.Value, item.ObjectType.Value, item.ObjectSubtype.Value);
			}
		}
		return new DbCachedRepository(settings.Scope, excludedObjects, settings.CustomFields, settings.OtherFields);
	}
}
