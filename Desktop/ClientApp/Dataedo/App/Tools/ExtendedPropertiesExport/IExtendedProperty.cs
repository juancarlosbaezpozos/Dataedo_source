using Dataedo.Model.Data.Interfaces;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public interface IExtendedProperty : ISelectableWithTitle, ISelectable
{
	new bool IsSelected { get; set; }

	new string Title { get; set; }

	string ExtendedProperty { get; set; }
}
