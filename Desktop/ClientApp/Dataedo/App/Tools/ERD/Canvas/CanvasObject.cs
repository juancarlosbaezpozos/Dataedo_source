using System;
using System.Drawing;
using System.IO;
using System.Xml;
using Dataedo.App.Tools.ERD.Canvas.CanvasEventHandlers;
using Dataedo.App.Tools.ERD.Diagram;

namespace Dataedo.App.Tools.ERD.Canvas;

public abstract class CanvasObject : IChangeable
{
	public enum Output
	{
		Control = 0,
		Image = 1
	}

	public const int EMPTY_KEY = int.MinValue;

	private bool _MouseOver;

	public int Key { get; protected set; } = int.MinValue;


	public bool MouseOver
	{
		get
		{
			return _MouseOver;
		}
		protected set
		{
			_MouseOver = value;
			NotifyChange();
		}
	}

	public event EventHandler Changed;

	public event CMouseEvent MouseEnter;

	public event CMouseEvent MouseMove;

	public event CMouseEvent MouseLeave;

	public event CMouseEvent MouseDown;

	public event CMouseEvent MouseUp;

	public event CMouseEvent MouseClick;

	protected void OnMouseEnter(object sender, CMouseEventArgs e)
	{
		MouseOver = true;
	}

	protected void OnMouseLeave(object sender, CMouseEventArgs e)
	{
		MouseOver = false;
	}

	protected void NotifyChange(EventArgs e = null)
	{
		EventArgs e2 = ((e != null) ? e : EventArgs.Empty);
		this.Changed?.Invoke(this, e2);
	}

	public abstract void Render(Graphics g, Point startPoint, Output dest = Output.Control);

	public abstract bool Contains(Point p);

	internal void NotifyMouseEnter(CMouseEventArgs e)
	{
		this.MouseEnter?.Invoke(this, e);
	}

	internal void NotifyMouseMove(CMouseEventArgs e)
	{
		this.MouseMove?.Invoke(this, e);
	}

	internal void NotifyMouseLeave(CMouseEventArgs e)
	{
		this.MouseLeave?.Invoke(this, e);
	}

	internal void NotifyMouseDown(CMouseEventArgs e)
	{
		this.MouseDown?.Invoke(this, e);
	}

	internal void NotifyMouseUp(CMouseEventArgs e)
	{
		this.MouseUp?.Invoke(this, e);
	}

	internal void NotifyMouseClick(CMouseEventArgs e)
	{
		this.MouseClick?.Invoke(this, e);
	}

	public virtual XmlElement ToSvg(XmlDocument xml, Output destination)
	{
		return xml.CreateElement("g");
	}

	protected static XmlElement CreateTextElement(XmlDocument xml, int x, int y, int width, int height, string innerText, string horzAlign = null, string maskId = null, bool setXml = false, string vertAlign = null)
	{
		XmlElement xmlElement = xml.CreateElement("switch");
		XmlElement xmlElement2 = xml.CreateElement("foreignObject");
		xmlElement2.SetAttribute("x", x.ToString());
		xmlElement2.SetAttribute("y", y.ToString());
		xmlElement2.SetAttribute("width", width.ToString());
		xmlElement2.SetAttribute("height", height.ToString());
		XmlElement xmlElement3 = xml.CreateElement("div");
		string text = $"width: {width}px;height: {height}px;";
		if (vertAlign != null)
		{
			text = text + "vertical-align: " + vertAlign + ";";
		}
		xmlElement3.SetAttribute("xmlns", "http://www/w3/org/1999/xhtml");
		xmlElement3.SetAttribute("style", text);
		XmlElement xmlElement4 = xml.CreateElement("p");
		string text2 = $"max-width: {width}px;max-height: {height / 16 * 16}px;white-space: pre-wrap;";
		if (horzAlign != null)
		{
			text2 = text2 + "text-align: " + horzAlign + ";";
		}
		xmlElement4.SetAttribute("style", text2);
		if (!setXml)
		{
			xmlElement4.InnerText = innerText;
		}
		else
		{
			xmlElement4.InnerXml = innerText;
		}
		xmlElement3.AppendChild(xmlElement4);
		XmlElement xmlElement5 = xml.CreateElement("text");
		if (maskId != null)
		{
			xmlElement5.SetAttribute("mask", $"url('#{maskId}')");
		}
		if (horzAlign == null || (horzAlign.ToLower() != "right" && horzAlign.ToLower() != "center"))
		{
			xmlElement5.SetAttribute("x", x.ToString());
		}
		else if (horzAlign.ToLower() == "right")
		{
			xmlElement5.SetAttribute("x", (x + width).ToString());
			xmlElement5.SetAttribute("text-anchor", "end");
		}
		else if (horzAlign.ToLower() == "center")
		{
			xmlElement5.SetAttribute("x", (x + width / 2).ToString());
			xmlElement5.SetAttribute("text-anchor", "middle");
		}
		xmlElement5.SetAttribute("y", (y + height / 2).ToString());
		xmlElement5.SetAttribute("font-size", Styles.SvgNodeTextSize.ToString());
		xmlElement5.SetAttribute("font-family", "Arial");
		xmlElement5.SetAttribute("dy", "0.4em");
		xmlElement5.InnerText = innerText;
		xmlElement2.AppendChild(xmlElement3);
		xmlElement.AppendChild(xmlElement2);
		xmlElement.AppendChild(xmlElement5);
		return xmlElement;
	}

	protected XmlElement CreateIconElement(XmlDocument xml, Image iconImage, int x, int y, int width, int height)
	{
		XmlElement xmlElement = xml.CreateElement("image");
		xmlElement.SetAttribute("x", x.ToString());
		xmlElement.SetAttribute("y", y.ToString());
		xmlElement.SetAttribute("width", Styles.NodeIconSize.ToString());
		xmlElement.SetAttribute("height", Styles.NodeIconSize.ToString());
		using MemoryStream memoryStream = new MemoryStream();
		iconImage.Save(memoryStream, iconImage.RawFormat);
		xmlElement.SetAttribute("href", "http://www.w3.org/1999/xlink", "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray()));
		return xmlElement;
	}

	protected void CreateMask(XmlDocument xml, string id, int x, int y, int width, int height)
	{
		XmlElement obj = xml.SelectSingleNode("/svg/defs") as XmlElement;
		XmlElement xmlElement = xml.CreateElement("mask");
		xmlElement.SetAttribute("id", id);
		obj.AppendChild(xmlElement);
		XmlElement xmlElement2 = xml.CreateElement("rect");
		xmlElement2.SetAttribute("x", x.ToString());
		xmlElement2.SetAttribute("y", y.ToString());
		xmlElement2.SetAttribute("width", width.ToString());
		xmlElement2.SetAttribute("height", height.ToString());
		xmlElement2.SetAttribute("fill", "#ffffff");
		xmlElement.AppendChild(xmlElement2);
	}
}
