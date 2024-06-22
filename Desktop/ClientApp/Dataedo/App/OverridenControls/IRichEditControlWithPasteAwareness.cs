using System;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;

namespace Dataedo.App.OverridenControls;

public interface IRichEditControlWithPasteAwareness : IRichEditControl, IBatchUpdateable, IServiceContainer, IServiceProvider, IWin32Window, ICommandAwareControl<RichEditCommandId>
{
	event EventHandler TextPastedEvent;

	void TriggerTextPastedEvent();
}
