using System;

namespace Dataedo.App.Documentation.Template;

public interface IBaseTemplateModel
{
	string Name { get; set; }

	string Description { get; set; }

	TemplateTypeEnum.TemplateType Type { get; set; }

	bool? IsCustomizable { get; set; }

	Exception ExceptionValue { get; set; }

	string CoreTemplateFilePath { get; }
}
