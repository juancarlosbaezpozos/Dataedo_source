using System;

namespace Dataedo.App.Tools.ERD.Canvas;

public interface IChangeable
{
	event EventHandler Changed;
}
