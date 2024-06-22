using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Onboarding.Controls;
using DevExpress.Utils;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.Onboarding;

public static class OnboardingPanels
{
    private static OnboardingPanel visiblePanel;

    private static readonly Queue<OnboardingPanel> queuedPanels = new();

    private static readonly Dictionary<OnboardingPanel, EventHandler> ownerLocationChangedHandlers = new();

    public static OnboardingPanel ShowPanel(OnboardingSupport.OnboardingMessages onboardingType,
        OnboardingMessage message, Control owner, Func<Rectangle> getRectangle)
    {
        if (!queuedPanels.Any(x => x.OnboardingType == onboardingType))
        {
            var onboardingPanel = visiblePanel;
            if (onboardingPanel == null || onboardingPanel.OnboardingType != onboardingType)
            {
                Color? backColor = Color.Gray;
                double? opacity = 0.01;
                Image image = new Bitmap(1, 1);
                new OverlayWindowOptions(true, true, backColor, null, opacity, image);
                var flyoutPanel = new OnboardingPanel(onboardingType, message, message.Size, getRectangle)
                {
                    Parent = owner,
                    OwnerControl = owner,
                    OwnerLocation = owner.Location
                };
                flyoutPanel.Options.AnchorType = PopupToolWindowAnchor.Manual;
                flyoutPanel.OptionsBeakPanel.BeakLocation = message.BeakPanelBeakLocation;
                flyoutPanel.SkipClick += Panel_SkipClick;
                flyoutPanel.Hidden += FlyoutPanel_Hidden;
                EventHandler value = delegate { Owner_LocationChanged(flyoutPanel, owner); };
                owner.LocationChanged += value;
                ownerLocationChangedHandlers.Add(flyoutPanel, value);
                if (visiblePanel == null)
                {
                    if (flyoutPanel.ShowOnboardingPanel())
                    {
                        visiblePanel = flyoutPanel;
                    }
                    else
                    {
                        ClearAfterPanel(flyoutPanel);
                    }
                }
                else
                {
                    queuedPanels.Enqueue(flyoutPanel);
                }

                return flyoutPanel;
            }
        }

        return null;
    }

    private static void FlyoutPanel_Hidden(object sender, FlyoutPanelEventArgs e)
    {
        var obj = sender as OnboardingPanel;
        ClearAfterPanel(obj);
        OnboardingSupport.SetMessageCondition(obj.OnboardingType);
        obj.Dispose();
        if (queuedPanels.Any())
        {
            var onboardingPanel = queuedPanels.Dequeue();
            if (onboardingPanel.ShowOnboardingPanel())
            {
                visiblePanel = onboardingPanel;
                return;
            }

            ClearAfterPanel(onboardingPanel);
            visiblePanel = null;
        }
        else
        {
            visiblePanel = null;
        }
    }

    private static void ClearAfterPanel(OnboardingPanel flyoutPanel)
    {
        if (flyoutPanel != null)
        {
            if (flyoutPanel.OwnerControl != null && ownerLocationChangedHandlers.ContainsKey(flyoutPanel))
            {
                flyoutPanel.OwnerControl.LocationChanged -= ownerLocationChangedHandlers[flyoutPanel];
            }

            flyoutPanel.SkipClick -= Panel_SkipClick;
            ownerLocationChangedHandlers.Remove(flyoutPanel);
        }
    }

    private static void Owner_LocationChanged(OnboardingPanel flyoutPanel, Control owner)
    {
        var currentObjectInfo = flyoutPanel.GetCurrentObjectInfo();
        if (flyoutPanel.OwnerLocation != Point.Empty && currentObjectInfo.Form != null)
        {
            var dx = owner.Location.X - flyoutPanel.OwnerLocation.X;
            var dy = owner.Location.Y - flyoutPanel.OwnerLocation.Y;
            var location = currentObjectInfo.Form.Location;
            location.Offset(dx, dy);
            currentObjectInfo.Form.Location = location;
        }

        flyoutPanel.OwnerLocation = owner.Location;
    }

    private static void Panel_SkipClick(object sender, EventArgs e)
    {
        OnboardingSupport.SetOnboardingCompleted();
        foreach (var queuedPanel in queuedPanels)
        {
            ClearAfterPanel(queuedPanel);
        }

        queuedPanels.Clear();
        ((sender as LabelControl)?.Parent?.Parent as OnboardingPanel)?.Close();
    }
}