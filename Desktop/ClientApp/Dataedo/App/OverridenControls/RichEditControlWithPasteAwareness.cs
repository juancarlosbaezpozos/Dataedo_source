using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Dataedo.App.UserControls;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Services;

namespace Dataedo.App.OverridenControls;

public class RichEditControlWithPasteAwareness : RichEditUserControl, IRichEditControlWithPasteAwareness, IRichEditControl, IBatchUpdateable, IServiceContainer, IServiceProvider, IWin32Window, ICommandAwareControl<RichEditCommandId>
{
	[Browsable(true)]
	public event EventHandler TextPastedEvent;

	public RichEditControlWithPasteAwareness()
	{
		CustomRichEditCommandFactoryService serviceInstance = new CustomRichEditCommandFactoryService(this, GetService<IRichEditCommandFactoryService>());
		RemoveService(typeof(IRichEditCommandFactoryService));
		AddService(typeof(IRichEditCommandFactoryService), serviceInstance);
	}

	public void TriggerTextPastedEvent()
	{
		this.TextPastedEvent?.Invoke(this, null);
	}
}
