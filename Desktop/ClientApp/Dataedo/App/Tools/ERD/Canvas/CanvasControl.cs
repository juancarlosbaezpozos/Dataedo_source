using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.ERD.Canvas.CanvasEventHandlers;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.ERD.Extensions;

namespace Dataedo.App.Tools.ERD.Canvas;

public class CanvasControl : Control
{
	public ElementsContainer Elements;

	private IContainer components;

	public Point StartPoint { get; set; } = new Point(0, 0);


	public Point GridStartPoint { get; set; } = new Point(0, 0);


	public CanvasObject MouseOverElement { get; private set; }

	public CanvasObject MousePressElement { get; private set; }

	public RectangularObject MouseOverElementToResize { get; private set; }

	public CanvasControl()
	{
		DoubleBuffered = true;
		base.ResizeRedraw = true;
		InitializeComponent();
		Elements = new ElementsContainer();
		Elements.Changed += LayersChanged;
		base.MouseMove += NotifyMouseMove;
		base.MouseDown += NotifyMouseDown;
		base.MouseUp += NotifyMouseUp;
		base.MouseClick += NotifyMouseClick;
	}

	public void UseLayers(ElementsContainer outerContainer)
	{
		Elements.Changed -= LayersChanged;
		Elements = outerContainer;
		Elements.Changed += LayersChanged;
		Invalidate();
	}

	private void LayersChanged(object sender, EventArgs e)
	{
		Invalidate();
	}

	public virtual Point ToLocal(int x, int y)
	{
		return new Point(x, y);
	}

	private CMouseEventArgs Convert(MouseEventArgs e)
	{
		return new CMouseEventArgs(e.X, e.Y, e.Button, Control.ModifierKeys.HasFlag(Keys.Control));
	}

	private void NotifyMouseClick(object sender, MouseEventArgs e)
	{
		if (MouseOverElement != null)
		{
			MouseOverElement.NotifyMouseClick(Convert(e));
		}
	}

	private void NotifyMouseDown(object sender, MouseEventArgs e)
	{
		MousePressElement = MouseOverElement;
		if (MousePressElement != null)
		{
			MousePressElement.NotifyMouseDown(Convert(e));
		}
	}

	private void NotifyMouseUp(object sender, MouseEventArgs e)
	{
		CMouseEventArgs e2 = Convert(e);
		if (MouseOverElement != null)
		{
			MouseOverElement.NotifyMouseUp(e2);
		}
		MousePressElement = null;
	}

	private void NotifyMouseMove(object sender, MouseEventArgs e)
	{
		CMouseEventArgs e2 = Convert(e);
		if (MousePressElement != null)
		{
			MousePressElement.NotifyMouseMove(e2);
		}
		PickingMouseOverElement();
	}

	protected void PickingMouseOverElement()
	{
		Point point = PointToClient(Control.MousePosition);
		CMouseEventArgs cMouseEventArgs = new CMouseEventArgs(point.X, point.Y, Control.MouseButtons, Control.ModifierKeys.HasFlag(Keys.Control));
		Point point2 = ToLocal(cMouseEventArgs.X, cMouseEventArgs.Y).Subtract(StartPoint);
		CanvasObject canvasObject = null;
		RectangularObject rectangularObject = null;
		foreach (CanvasObject item in Elements.FrontToBackAllElementsNodesFirst)
		{
			if (item.Contains(point2))
			{
				canvasObject = item;
				break;
			}
		}
		foreach (RectangularObject rectangularObject2 in Elements.RectangularObjects)
		{
			if (rectangularObject2 == canvasObject && rectangularObject2.IsOnEdge(point2))
			{
				canvasObject = null;
				rectangularObject = rectangularObject2;
				break;
			}
		}
		if (canvasObject != MouseOverElement)
		{
			if (MouseOverElement != null)
			{
				MouseOverElement.NotifyMouseLeave(cMouseEventArgs);
			}
			canvasObject?.NotifyMouseEnter(cMouseEventArgs);
			MouseOverElement = canvasObject;
		}
		if (MouseOverElement != null)
		{
			MouseOverElement.NotifyMouseMove(cMouseEventArgs);
		}
		if (rectangularObject != MouseOverElementToResize)
		{
			MouseOverElementToResize = rectangularObject;
		}
	}

	public virtual void Render(Graphics g, Point startPoint)
	{
		foreach (CanvasObject item in Elements.AllElementsLinksFirst)
		{
			item.Render(g, startPoint);
		}
	}

	protected sealed override void OnPaint(PaintEventArgs e)
	{
		Render(e.Graphics, StartPoint);
		base.OnPaint(e);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
	}
}
