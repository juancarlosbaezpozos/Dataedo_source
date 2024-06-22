using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.CustomControls;
using Microsoft.Win32;
using Salesforce.Common;

namespace Dataedo.App.Forms.Salesforce;

public class SalesforceInteractiveSignInForm : BaseXtraForm
{
	private const string ProductionLoginURL = "https://login.salesforce.com/services/oauth2/authorize?";

	private const string SandboxLoginURL = "https://test.salesforce.com/services/oauth2/authorize?";

	private const string ResponseTypeQueryPart = "response_type=token";

	private const string ClientIdQueryPart = "&client_id=";

	private const string RedirectUrlQueryPart = "&redirect_uri=";

	private AuthenticationClient authenticationClient;

	private IContainer components;

	private WebBrowser webBrowser;

	public SalesforceInteractiveSignInForm(bool isSandboxInstance = false)
	{
		InitializeComponent();
		base.VerticalScroll.Visible = false;
		try
		{
			using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", writable: true);
			string fileName = Path.GetFileName(Application.ExecutablePath);
			if (registryKey.GetValue(fileName)?.ToString() != "11001")
			{
				registryKey.SetValue(fileName, 11001, RegistryValueKind.DWord);
			}
			registryKey.Close();
		}
		catch
		{
		}
		LoadAuthorizationPage(isSandboxInstance);
	}

	public void LoadAuthorizationPage(bool isSandboxInstance = false)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(isSandboxInstance ? "https://test.salesforce.com/services/oauth2/authorize?" : "https://login.salesforce.com/services/oauth2/authorize?");
		stringBuilder.Append("response_type=token");
		stringBuilder.Append("&client_id=");
		stringBuilder.Append(ConfigHelper.GetSalesforceClientIdConfigValue());
		stringBuilder.Append("&redirect_uri=");
		stringBuilder.Append(ConfigHelper.GetSalesforceLoginRedirectPageConfigValue());
		webBrowser.Navigate(new Uri(stringBuilder.ToString()));
	}

	private void WebBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
	{
		string salesforceLoginRedirectPageConfigValue = ConfigHelper.GetSalesforceLoginRedirectPageConfigValue();
		if (e.Url.AbsoluteUri.StartsWith(salesforceLoginRedirectPageConfigValue, StringComparison.OrdinalIgnoreCase))
		{
			Uri uri = new Uri(e.Url.AbsoluteUri.Replace(salesforceLoginRedirectPageConfigValue + "/#", salesforceLoginRedirectPageConfigValue + "/?"));
			string text = HttpUtility.ParseQueryString(uri.Query).Get("access_token");
			string text2 = HttpUtility.ParseQueryString(uri.Query).Get("instance_url");
			string text3 = HttpUtility.ParseQueryString(uri.Query).Get("id");
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text3))
			{
				AuthenticationClient authenticationClient = (this.authenticationClient = new AuthenticationClient
				{
					AccessToken = text,
					Id = text3,
					InstanceUrl = text2
				});
			}
			Invoke((MethodInvoker)delegate
			{
				Close();
			});
		}
	}

	public AuthenticationClient GetAuthenticationClient()
	{
		return authenticationClient;
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
		this.webBrowser = new System.Windows.Forms.WebBrowser();
		base.SuspendLayout();
		this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
		this.webBrowser.Location = new System.Drawing.Point(0, 0);
		this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
		this.webBrowser.Name = "webBrowser";
		this.webBrowser.ScriptErrorsSuppressed = true;
		this.webBrowser.Size = new System.Drawing.Size(633, 629);
		this.webBrowser.TabIndex = 0;
		this.webBrowser.Navigated += new System.Windows.Forms.WebBrowserNavigatedEventHandler(WebBrowser_Navigated);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(633, 629);
		base.Controls.Add(this.webBrowser);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_32;
		base.Name = "SalesforceInteractiveSignInForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Salesforce Interactive Sign-In";
		base.ResumeLayout(false);
	}
}
