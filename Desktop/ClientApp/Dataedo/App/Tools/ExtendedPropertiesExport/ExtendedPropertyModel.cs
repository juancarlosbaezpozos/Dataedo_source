using Dataedo.Model.Data.Interfaces;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public class ExtendedPropertyModel : IExtendedProperty, ISelectableWithTitle, ISelectable
{
	public bool IsSelected { get; set; }

	public string Title { get; set; }

	public string ExtendedProperty { get; set; }
}
