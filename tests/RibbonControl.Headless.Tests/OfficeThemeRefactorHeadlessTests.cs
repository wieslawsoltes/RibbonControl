// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.XUnit;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using Avalonia.VisualTree;
using RibbonControl.Core.Controls;
using RibbonControl.Core.Models;

namespace RibbonControl.Headless.Tests;

public class OfficeThemeRefactorHeadlessTests
{
    [AvaloniaFact]
    public void OfficeTheme_LegacyControlThemeKeys_AreAvailable()
    {
        EnsureOfficeThemeLoaded();

        var resources = Application.Current;
        Assert.NotNull(resources);

        Assert.True(resources!.TryFindResource("OfficeRibbonTheme", out var ribbonThemeValue));
        var ribbonTheme = Assert.IsType<ControlTheme>(ribbonThemeValue);
        Assert.Equal(typeof(Ribbon), ribbonTheme.TargetType);

        Assert.True(resources.TryFindResource("OfficeRibbonTabControlTheme", out var tabControlThemeValue));
        var tabControlTheme = Assert.IsType<ControlTheme>(tabControlThemeValue);
        Assert.Equal(typeof(TabControl), tabControlTheme.TargetType);

        Assert.True(resources.TryFindResource("OfficeRibbonTabItemTheme", out var tabItemThemeValue));
        var tabItemTheme = Assert.IsType<ControlTheme>(tabItemThemeValue);
        Assert.Equal(typeof(TabItem), tabItemTheme.TargetType);

        Assert.True(resources.TryFindResource("OfficeRibbonQuickAccessToolBarTheme", out var quickAccessThemeValue));
        var quickAccessTheme = Assert.IsType<ControlTheme>(quickAccessThemeValue);
        Assert.Equal(typeof(RibbonQuickAccessToolBar), quickAccessTheme.TargetType);

        Assert.True(resources.TryFindResource("OfficeRibbonContextualTabBandTheme", out var contextBandThemeValue));
        var contextBandTheme = Assert.IsType<ControlTheme>(contextBandThemeValue);
        Assert.Equal(typeof(RibbonContextualTabBand), contextBandTheme.TargetType);
    }

    [AvaloniaFact]
    public void OfficeTheme_TemplateSlots_ResolveToOfficeTemplates()
    {
        EnsureOfficeThemeLoaded();

        var resources = Application.Current;
        Assert.NotNull(resources);

        Assert.True(resources!.TryFindResource("OfficeRibbonCollapsedTabContentTemplate", out var officeCollapsedTemplate));
        Assert.True(resources.TryFindResource("RibbonThemeCollapsedTabContentTemplate", out var slotCollapsedTemplate));
        Assert.Same(officeCollapsedTemplate, slotCollapsedTemplate);

        Assert.True(resources.TryFindResource("OfficeRibbonTabHeaderTemplate", out var officeTabHeaderTemplate));
        Assert.True(resources.TryFindResource("RibbonThemeTabHeaderTemplate", out var slotTabHeaderTemplate));
        Assert.Same(officeTabHeaderTemplate, slotTabHeaderTemplate);
    }

    [AvaloniaFact]
    public void OfficeTheme_AppliesTokenizedRibbonHostMargins()
    {
        EnsureOfficeThemeLoaded();

        var ribbon = new Ribbon
        {
            TopBarStartContent = new TextBlock { Text = "Start" },
            TopBarEndContent = new TextBlock { Text = "End" },
            HeaderStartContent = new TextBlock { Text = "Header Start" },
            HeaderEndContent = new TextBlock { Text = "Header End" },
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Items =
                    {
                        new RibbonItem { Id = "copy", Label = "Copy" },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1200,
            Height = 800,
            Content = ribbon,
        };

        window.Show();
        window.UpdateLayout();

        var topBar = ribbon.GetVisualDescendants()
            .OfType<DockPanel>()
            .Single(panel => panel.Name == "PART_TopBar");
        Assert.Equal(new Thickness(0), topBar.Margin);

        var topBarStartHost = ribbon.GetVisualDescendants()
            .OfType<ContentPresenter>()
            .Single(presenter => presenter.Name == "PART_TopBarStartContentHost");
        Assert.Equal(new Thickness(6, 0, 10, 0), topBarStartHost.Margin);

        var topBarEndHost = ribbon.GetVisualDescendants()
            .OfType<ContentPresenter>()
            .Single(presenter => presenter.Name == "PART_TopBarEndContentHost");
        Assert.Equal(new Thickness(8, 0, 6, 0), topBarEndHost.Margin);

        var headerStartHost = ribbon.GetVisualDescendants()
            .OfType<ContentPresenter>()
            .Single(presenter => presenter.Name == "PART_HeaderStartContentHost");
        Assert.Equal(new Thickness(6, 0, 10, 0), headerStartHost.Margin);

        var headerEndHost = ribbon.GetVisualDescendants()
            .OfType<ContentPresenter>()
            .Single(presenter => presenter.Name == "PART_HeaderEndContentHost");
        Assert.Equal(new Thickness(8, 0, 6, 0), headerEndHost.Margin);
    }

    private static void EnsureOfficeThemeLoaded()
    {
        if (Application.Current is null)
        {
            return;
        }

        var uri = new Uri("avares://RibbonControl.Themes.Office/Themes/OfficeTheme.axaml");
        var alreadyLoaded = Application.Current.Styles
            .OfType<StyleInclude>()
            .Any(x => x.Source == uri);

        if (alreadyLoaded)
        {
            return;
        }

        Application.Current.Styles.Add(new StyleInclude(uri)
        {
            Source = uri,
        });
    }
}
