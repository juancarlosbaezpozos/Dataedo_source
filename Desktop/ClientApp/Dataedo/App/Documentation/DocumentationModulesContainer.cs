using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;

namespace Dataedo.App.Documentation;

public class DocumentationModulesContainer
{
	public List<DocumentationModules> Data { get; set; }

	public IEnumerable<DocumentationModules> SelectedDocumentations => Data.Where((DocumentationModules x) => x.IsSelected);

	public int SelectedDocumentationsCount => SelectedDocumentations.Count();

	public DocumentationModulesContainer()
	{
		Data = new List<DocumentationModules>();
	}

	public DocumentationModulesContainer(IEnumerable<DocumentationModules> documentationModules)
	{
		Data = new List<DocumentationModules>(documentationModules);
	}

	public IEnumerable<DocumentationModules> GetDocumentations(int? documentationId)
	{
		return Data.Where((DocumentationModules x) => x.Documentation.Id == documentationId);
	}

	public bool IsDatabaseSelected(int documentationId)
	{
		return SelectedDocumentations.Where((DocumentationModules x) => x.Documentation.IdValue == documentationId).Count() > 0;
	}

	public bool IsDocumentationSelected(int documentationId)
	{
		return SelectedDocumentations.Where((DocumentationModules x) => x.Documentation.IdValue == documentationId && x.Documentation.Type.HasValue).Count() > 0;
	}

	public IEnumerable<DocumentationModules> GetSelectedDocumentations(int? documentationId = null)
	{
		if (documentationId.HasValue)
		{
			return Data.Where((DocumentationModules x) => x.Documentation.Id == documentationId && x.IsSelected);
		}
		return Data.Where((DocumentationModules x) => x.IsSelected);
	}

	public IEnumerable<ModuleRow> GetSelectedModules(int? documentationId)
	{
		return GetSelectedDocumentations(documentationId).SelectMany((DocumentationModules x) => x.Modules.Where((ModuleRow y) => y.IsShown)).ToList();
	}

	public DocumentationModules[] GetSelectedModules()
	{
		IEnumerable<DocumentationModules> selectedDocumentations = GetSelectedDocumentations();
		if (selectedDocumentations.Count() > 0 && selectedDocumentations.SelectMany((DocumentationModules x) => x.Modules).Any())
		{
			return selectedDocumentations.Select((DocumentationModules x) => new DocumentationModules
			{
				Documentation = x.Documentation,
				Modules = x.Modules.Where((ModuleRow y) => y.IsShown).ToList()
			}).ToArray();
		}
		return new DocumentationModules[0];
	}

	public IEnumerable<int> GetSelectedModulesIds(int? documentationId)
	{
		IEnumerable<DocumentationModules> selectedDocumentations = GetSelectedDocumentations(documentationId);
		if (selectedDocumentations.Count() > 0)
		{
			if (selectedDocumentations.SelectMany((DocumentationModules x) => x.Modules).Any())
			{
				return from x in selectedDocumentations.SelectMany((DocumentationModules x) => x.Modules.Where((ModuleRow y) => y.IsShown))
					select x.Id ?? (-1);
			}
			return new int[1] { -1 };
		}
		return new int[0];
	}

	public bool IsModuleSelected(int moduleId)
	{
		return (from x in GetSelectedModules()
			where x.ModulesId.Contains(moduleId)
			select x).Count() > 0;
	}

	public bool IsModuleSelected(int documentationId, int moduleId)
	{
		return Data.Any((DocumentationModules x) => x.Documentation.IdValue == documentationId && x.IsSelected && (x.Modules.Any((ModuleRow y) => y.IdValue == moduleId && y.IsShown) || x.Modules.Count == 0));
	}
}
