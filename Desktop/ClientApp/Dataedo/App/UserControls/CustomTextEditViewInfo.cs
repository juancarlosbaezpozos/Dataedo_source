using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Dataedo.App.UserControls;

public class CustomTextEditViewInfo : TextEditViewInfo
{
	public CustomTextEditViewInfo(RepositoryItem item)
		: base(item)
	{
	}
}
