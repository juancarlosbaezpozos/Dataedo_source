using System;
using System.Windows.Forms;
using Dataedo.App.Tools.Export.Universal.Storage;

namespace Dataedo.App.Tools.Export.Universal;

public interface ITemplateCustomizer
{
	bool IsCustomizable(ITemplate template, Type destination);

	string Customize(ITemplate template, IStorage destination, object customData = null, Form owner = null);
}
