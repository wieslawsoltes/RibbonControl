// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using RibbonControl.Core.Controls;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;
using RibbonControl.Core.Services;

namespace RibbonControl.Headless.Tests;

public class RibbonStatePersistenceHeadlessTests
{
    [AvaloniaFact]
    public async Task LoadStateAsync_AppliesMergedCustomizationFromStore()
    {
        var store = new InMemoryRibbonStateStore();
        await store.SaveAsync(new RibbonRuntimeState
        {
            SelectedTabId = "plugin",
            QuickAccessPlacement = RibbonQuickAccessPlacement.Below,
            NodeCustomizations =
            {
                new RibbonNodeCustomization { Id = "insert", ParentId = null, IsHidden = true, Order = 99 },
                new RibbonNodeCustomization { Id = "plugin", ParentId = null, Order = -1 },
            },
        });

        var ribbon = BuildHybridRibbon();
        ribbon.StateStore = store;

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();

        var loaded = await ribbon.LoadStateAsync();

        Assert.True(loaded);
        Assert.Equal("plugin", ribbon.SelectedTabId);
        Assert.Equal(RibbonQuickAccessPlacement.Below, ribbon.QuickAccessPlacement);
        Assert.Equal(2, ribbon.MergedTabs.Count);
        Assert.Equal("plugin", ribbon.MergedTabs[0].Id);
        Assert.DoesNotContain(ribbon.MergedTabs, tab => tab.Id == "insert");
    }

    [AvaloniaFact]
    public async Task SaveStateAsync_PreservesHiddenNodesFromSeedState()
    {
        var store = new InMemoryRibbonStateStore();
        await store.SaveAsync(new RibbonRuntimeState
        {
            SelectedTabId = "plugin",
            NodeCustomizations =
            {
                new RibbonNodeCustomization { Id = "insert", ParentId = null, IsHidden = true, Order = 5 },
                new RibbonNodeCustomization { Id = "plugin", ParentId = null, Order = -1 },
            },
        });

        var ribbon = BuildHybridRibbon();
        ribbon.StateStore = store;

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();

        await ribbon.LoadStateAsync();
        await ribbon.SaveStateAsync();

        var saved = await store.LoadAsync();
        Assert.NotNull(saved);
        Assert.Contains(saved!.NodeCustomizations, x => x.Id == "insert" && x.ParentId is null && x.IsHidden == true);
        Assert.Contains(saved.NodeCustomizations, x => x.Id == "plugin" && x.ParentId is null);
    }

    [AvaloniaFact]
    public void ApplyRuntimeState_InternalOwnership_DoesNotOverrideTopLevelState()
    {
        var ribbon = BuildHybridRibbon();
        ribbon.StateOwnershipMode = RibbonStateOwnershipMode.Internal;
        ribbon.SelectedTabId = "home";
        ribbon.QuickAccessPlacement = RibbonQuickAccessPlacement.Above;

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();

        ribbon.ApplyRuntimeState(new RibbonRuntimeState
        {
            SelectedTabId = "plugin",
            QuickAccessPlacement = RibbonQuickAccessPlacement.Below,
            NodeCustomizations =
            {
                new RibbonNodeCustomization { Id = "insert", ParentId = null, IsHidden = true },
            },
        });

        Assert.Equal("home", ribbon.SelectedTabId);
        Assert.Equal(RibbonQuickAccessPlacement.Above, ribbon.QuickAccessPlacement);
        Assert.DoesNotContain(ribbon.MergedTabs, tab => tab.Id == "insert");
    }

    private static Ribbon BuildHybridRibbon()
    {
        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Order = 0,
        });

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "insert",
            Header = "Insert",
            Order = 1,
        });

        ribbon.TabsSource =
        [
            new RibbonTabDefinition
            {
                Id = "plugin",
                Header = "Plugin",
                Order = 10,
            },
        ];

        return ribbon;
    }
}
