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

public class GenericThemeCompatibilityHeadlessTests
{
    [AvaloniaFact]
    public void GenericTheme_ControlThemeKeys_AreAvailable()
    {
        EnsureCoreThemeLoaded();

        var resources = Application.Current;
        Assert.NotNull(resources);

        Assert.True(resources!.TryFindResource(typeof(Ribbon), out var ribbonThemeValue));
        var ribbonTheme = Assert.IsType<ControlTheme>(ribbonThemeValue);
        Assert.Equal(typeof(Ribbon), ribbonTheme.TargetType);

        Assert.True(resources.TryFindResource("RibbonTabControlTheme", out var tabControlThemeValue));
        var tabControlTheme = Assert.IsType<ControlTheme>(tabControlThemeValue);
        Assert.Equal(typeof(TabControl), tabControlTheme.TargetType);

        Assert.True(resources.TryFindResource("RibbonTabItemTheme", out var tabItemThemeValue));
        var tabItemTheme = Assert.IsType<ControlTheme>(tabItemThemeValue);
        Assert.Equal(typeof(TabItem), tabItemTheme.TargetType);

        Assert.True(resources.TryFindResource(typeof(RibbonQuickAccessToolBar), out var quickAccessThemeValue));
        var quickAccessTheme = Assert.IsType<ControlTheme>(quickAccessThemeValue);
        Assert.Equal(typeof(RibbonQuickAccessToolBar), quickAccessTheme.TargetType);

        Assert.True(resources.TryFindResource(typeof(RibbonContextualTabBand), out var contextBandThemeValue));
        var contextBandTheme = Assert.IsType<ControlTheme>(contextBandThemeValue);
        Assert.Equal(typeof(RibbonContextualTabBand), contextBandTheme.TargetType);
    }

    [AvaloniaFact]
    public void GenericTheme_TemplateSlots_ResolveToActiveTemplates()
    {
        EnsureCoreThemeLoaded();

        var resources = Application.Current;
        Assert.NotNull(resources);

        Assert.True(resources!.TryFindResource("RibbonCollapsedTabContentTemplate", out var collapsedTemplate));
        Assert.True(resources.TryFindResource("RibbonThemeCollapsedTabContentTemplate", out var slotCollapsedTemplate));
        Assert.Same(collapsedTemplate, slotCollapsedTemplate);

        Assert.True(resources.TryFindResource("RibbonTabHeaderTemplate", out var tabHeaderTemplate));
        Assert.True(resources.TryFindResource("RibbonThemeTabHeaderTemplate", out var slotTabHeaderTemplate));
        Assert.Same(tabHeaderTemplate, slotTabHeaderTemplate);
    }

    [AvaloniaFact]
    public void GenericTheme_AppliesTokenizedRibbonHostMargins()
    {
        EnsureCoreThemeLoaded();

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

    private static void EnsureCoreThemeLoaded()
    {
        if (Application.Current is null)
        {
            return;
        }

        var uri = new Uri("avares://RibbonControl.Core/Themes/Generic.axaml");
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
