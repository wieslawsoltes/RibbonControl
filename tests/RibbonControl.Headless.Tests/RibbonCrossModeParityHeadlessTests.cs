// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using RibbonControl.Core.Controls;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;
using RibbonControl.Core.Services;

namespace RibbonControl.Headless.Tests;

public class RibbonCrossModeParityHeadlessTests
{
    [AvaloniaFact]
    public void SameScenario_XamlMvvmHybrid_ProducesEquivalentMergedBehavior()
    {
        var xamlExec = 0;
        var mvvmExec = 0;
        var hybridExec = 0;

        var xamlRibbon = BuildXamlRibbon(BuildCatalog(() => xamlExec++));
        var mvvmRibbon = BuildMvvmRibbon(BuildCatalog(() => mvvmExec++));
        var hybridRibbon = BuildHybridRibbon(BuildCatalog(() => hybridExec++));

        ShowRibbon(xamlRibbon);
        ShowRibbon(mvvmRibbon);
        ShowRibbon(hybridRibbon);

        var expectedShape = Snapshot(xamlRibbon);
        Assert.Equal(expectedShape, Snapshot(mvvmRibbon));
        Assert.Equal(expectedShape, Snapshot(hybridRibbon));

        ExecutePrimaryCommand(xamlRibbon);
        ExecutePrimaryCommand(mvvmRibbon);
        ExecutePrimaryCommand(hybridRibbon);

        Assert.Equal(1, xamlExec);
        Assert.Equal(1, mvvmExec);
        Assert.Equal(1, hybridExec);

        var runtimeState = new RibbonRuntimeState
        {
            SelectedTabId = "home",
            QuickAccessPlacement = RibbonQuickAccessPlacement.Below,
            NodeCustomizations =
            {
                new RibbonNodeCustomization { Id = "paste", ParentId = "clipboard", IsHidden = true },
            },
        };

        xamlRibbon.ApplyRuntimeState(runtimeState);
        mvvmRibbon.ApplyRuntimeState(runtimeState);
        hybridRibbon.ApplyRuntimeState(runtimeState);

        var customizedShape = Snapshot(xamlRibbon);
        Assert.Equal(customizedShape, Snapshot(mvvmRibbon));
        Assert.Equal(customizedShape, Snapshot(hybridRibbon));
        Assert.Equal(RibbonQuickAccessPlacement.Below, xamlRibbon.QuickAccessPlacement);
        Assert.Equal(RibbonQuickAccessPlacement.Below, mvvmRibbon.QuickAccessPlacement);
        Assert.Equal(RibbonQuickAccessPlacement.Below, hybridRibbon.QuickAccessPlacement);
    }

    [AvaloniaFact]
    public void ContextualBackstageAndKeyTipCollisions_AreEquivalentAcrossModes()
    {
        var xamlRibbon = BuildXamlRibbonExtended(BuildCatalog(() => { }));
        var mvvmRibbon = BuildMvvmRibbonExtended(BuildCatalog(() => { }));
        var hybridRibbon = BuildHybridRibbonExtended(BuildCatalog(() => { }));

        ShowRibbon(xamlRibbon);
        ShowRibbon(mvvmRibbon);
        ShowRibbon(hybridRibbon);

        Assert.DoesNotContain(xamlRibbon.MergedTabs, tab => tab.Id == "picture-format");
        Assert.DoesNotContain(mvvmRibbon.MergedTabs, tab => tab.Id == "picture-format");
        Assert.DoesNotContain(hybridRibbon.MergedTabs, tab => tab.Id == "picture-format");

        xamlRibbon.ActiveContextGroupIds = ["picture-tools"];
        mvvmRibbon.ActiveContextGroupIds = ["picture-tools"];
        hybridRibbon.ActiveContextGroupIds = ["picture-tools"];

        Assert.Contains(xamlRibbon.MergedTabs, tab => tab.Id == "picture-format");
        Assert.Contains(mvvmRibbon.MergedTabs, tab => tab.Id == "picture-format");
        Assert.Contains(hybridRibbon.MergedTabs, tab => tab.Id == "picture-format");

        var contextualShape = Snapshot(xamlRibbon);
        Assert.Equal(contextualShape, Snapshot(mvvmRibbon));
        Assert.Equal(contextualShape, Snapshot(hybridRibbon));
        var contextualGroups = SnapshotContextGroups(xamlRibbon);
        Assert.Equal(contextualGroups, SnapshotContextGroups(mvvmRibbon));
        Assert.Equal(contextualGroups, SnapshotContextGroups(hybridRibbon));
        Assert.Contains("picture-tools|Picture Tools|#0F6CBD|30", contextualGroups, StringComparison.Ordinal);

        xamlRibbon.ToggleBackstageCommand.Execute(null);
        mvvmRibbon.ToggleBackstageCommand.Execute(null);
        hybridRibbon.ToggleBackstageCommand.Execute(null);

        Assert.True(xamlRibbon.Backstage!.IsOpen);
        Assert.True(mvvmRibbon.Backstage!.IsOpen);
        Assert.True(hybridRibbon.Backstage!.IsOpen);

        xamlRibbon.ToggleBackstageCommand.Execute(null);
        mvvmRibbon.ToggleBackstageCommand.Execute(null);
        hybridRibbon.ToggleBackstageCommand.Execute(null);

        Assert.False(xamlRibbon.Backstage!.IsOpen);
        Assert.False(mvvmRibbon.Backstage!.IsOpen);
        Assert.False(hybridRibbon.Backstage!.IsOpen);

        xamlRibbon.IsKeyTipMode = true;
        mvvmRibbon.IsKeyTipMode = true;
        hybridRibbon.IsKeyTipMode = true;

        var keyTipSnapshot = SnapshotKeyTips(xamlRibbon);
        Assert.Equal(keyTipSnapshot, SnapshotKeyTips(mvvmRibbon));
        Assert.Equal(keyTipSnapshot, SnapshotKeyTips(hybridRibbon));
        Assert.Contains("__backstage|F", keyTipSnapshot, StringComparison.Ordinal);
        Assert.Contains("tab:home|H", keyTipSnapshot, StringComparison.Ordinal);
        Assert.Contains("tab:history|H2", keyTipSnapshot, StringComparison.Ordinal);
        Assert.Contains("qat:qat-help|H3", keyTipSnapshot, StringComparison.Ordinal);
        Assert.Contains("qat:qat-history|H4", keyTipSnapshot, StringComparison.Ordinal);
    }

    [AvaloniaFact]
    public void AdaptiveResize_XamlMvvmHybrid_ProducesEquivalentGroupModes()
    {
        var xamlRibbon = BuildAdaptiveXamlRibbon();
        var mvvmRibbon = BuildAdaptiveMvvmRibbon();
        var hybridRibbon = BuildAdaptiveHybridRibbon();

        var xamlWindow = ShowRibbon(xamlRibbon, 1200);
        var mvvmWindow = ShowRibbon(mvvmRibbon, 1200);
        var hybridWindow = ShowRibbon(hybridRibbon, 1200);

        var expandedSnapshot = SnapshotAdaptiveModes(xamlRibbon);
        Assert.Equal(expandedSnapshot, SnapshotAdaptiveModes(mvvmRibbon));
        Assert.Equal(expandedSnapshot, SnapshotAdaptiveModes(hybridRibbon));

        xamlRibbon.AdaptiveLayoutHorizontalPadding = 1300;
        mvvmRibbon.AdaptiveLayoutHorizontalPadding = 1300;
        hybridRibbon.AdaptiveLayoutHorizontalPadding = 1300;
        xamlWindow.UpdateLayout();
        mvvmWindow.UpdateLayout();
        hybridWindow.UpdateLayout();

        var compactedSnapshot = SnapshotAdaptiveModes(xamlRibbon);
        Assert.Equal(compactedSnapshot, SnapshotAdaptiveModes(mvvmRibbon));
        Assert.Equal(compactedSnapshot, SnapshotAdaptiveModes(hybridRibbon));
        Assert.True(
            compactedSnapshot.Contains("Compact", StringComparison.Ordinal) ||
            compactedSnapshot.Contains("Collapsed", StringComparison.Ordinal));
    }

    [AvaloniaFact]
    public void PopupMenus_XamlMvvmHybrid_ProduceEquivalentTopologyAndSubMenuBehavior()
    {
        var catalog = BuildCatalog(() => { });
        var xamlRibbon = BuildPopupXamlRibbon(catalog);
        var mvvmRibbon = BuildPopupMvvmRibbon(catalog);
        var hybridRibbon = BuildPopupHybridRibbon(catalog);

        ShowRibbon(xamlRibbon);
        ShowRibbon(mvvmRibbon);
        ShowRibbon(hybridRibbon);

        var expectedTopology = SnapshotPopupTopology(xamlRibbon);
        Assert.Equal(expectedTopology, SnapshotPopupTopology(mvvmRibbon));
        Assert.Equal(expectedTopology, SnapshotPopupTopology(hybridRibbon));

        var xamlItem = GetPopupSampleItem(xamlRibbon);
        var mvvmItem = GetPopupSampleItem(mvvmRibbon);
        var hybridItem = GetPopupSampleItem(hybridRibbon);

        xamlItem.IsDropDownOpen = true;
        mvvmItem.IsDropDownOpen = true;
        hybridItem.IsDropDownOpen = true;

        var xamlWeight = xamlItem.MenuItems.Single(menuItem => menuItem.Id == "weight");
        var mvvmWeight = mvvmItem.MenuItems.Single(menuItem => menuItem.Id == "weight");
        var hybridWeight = hybridItem.MenuItems.Single(menuItem => menuItem.Id == "weight");

        xamlWeight.IsSubMenuOpen = true;
        mvvmWeight.IsSubMenuOpen = true;
        hybridWeight.IsSubMenuOpen = true;

        Assert.True(xamlWeight.IsSubMenuOpen);
        Assert.True(mvvmWeight.IsSubMenuOpen);
        Assert.True(hybridWeight.IsSubMenuOpen);

        var xamlColor = xamlItem.MenuItems.Single(menuItem => menuItem.Id == "color");
        var mvvmColor = mvvmItem.MenuItems.Single(menuItem => menuItem.Id == "color");
        var hybridColor = hybridItem.MenuItems.Single(menuItem => menuItem.Id == "color");

        xamlColor.IsSubMenuOpen = true;
        mvvmColor.IsSubMenuOpen = true;
        hybridColor.IsSubMenuOpen = true;

        Assert.False(xamlWeight.IsSubMenuOpen);
        Assert.False(mvvmWeight.IsSubMenuOpen);
        Assert.False(hybridWeight.IsSubMenuOpen);
        Assert.True(xamlColor.IsSubMenuOpen);
        Assert.True(mvvmColor.IsSubMenuOpen);
        Assert.True(hybridColor.IsSubMenuOpen);
    }

    private static Ribbon BuildXamlRibbon(DictionaryRibbonCommandCatalog catalog)
    {
        var ribbon = new Ribbon
        {
            CommandCatalog = catalog,
            SelectedTabId = "home",
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Order = 0,
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Order = 0,
                    Items =
                    {
                        new RibbonItem { Id = "copy", Label = "Copy", CommandId = "copy", Order = 0 },
                        new RibbonItem { Id = "paste", Label = "Paste", CommandId = "paste", Order = 1 },
                    },
                },
            },
        });

        return ribbon;
    }

    private static Ribbon BuildMvvmRibbon(DictionaryRibbonCommandCatalog catalog)
    {
        return new Ribbon
        {
            CommandCatalog = catalog,
            SelectedTabId = "home",
            TabsSource =
            [
                new RibbonTabDefinition
                {
                    Id = "home",
                    Header = "Home",
                    Order = 0,
                    Groups =
                    {
                        new RibbonGroupDefinition
                        {
                            Id = "clipboard",
                            Header = "Clipboard",
                            Order = 0,
                            Items =
                            {
                                new RibbonItemDefinition { Id = "copy", Label = "Copy", CommandId = "copy", Order = 0 },
                                new RibbonItemDefinition { Id = "paste", Label = "Paste", CommandId = "paste", Order = 1 },
                            },
                        },
                    },
                },
            ],
        };
    }

    private static Ribbon BuildHybridRibbon(DictionaryRibbonCommandCatalog catalog)
    {
        var ribbon = new Ribbon
        {
            CommandCatalog = catalog,
            SelectedTabId = "home",
            TabMergeMode = RibbonMergeMode.Merge,
            TabsSource =
            [
                new RibbonTabDefinition
                {
                    Id = "home",
                    Header = "Home",
                    Order = 0,
                    Groups =
                    {
                        new RibbonGroupDefinition
                        {
                            Id = "clipboard",
                            Header = "Clipboard",
                            Order = 0,
                            Items =
                            {
                                new RibbonItemDefinition { Id = "copy", Label = "Copy", CommandId = "copy", Order = 0 },
                                new RibbonItemDefinition { Id = "paste", Label = "Paste", CommandId = "paste", Order = 1 },
                            },
                        },
                    },
                },
            ],
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Order = 0,
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Order = 0,
                    Items =
                    {
                        new RibbonItem { Id = "copy", Label = "Copy", CommandId = "copy", Order = 0 },
                        new RibbonItem { Id = "paste", Label = "Paste", CommandId = "paste", Order = 1 },
                    },
                },
            },
        });

        return ribbon;
    }

    private static Ribbon BuildXamlRibbonExtended(DictionaryRibbonCommandCatalog catalog)
    {
        var ribbon = new Ribbon
        {
            CommandCatalog = catalog,
            SelectedTabId = "home",
            Backstage = new RibbonBackstage
            {
                Content = new TextBlock { Text = "Backstage" },
            },
            QuickAccessItems =
            [
                new RibbonItem { Id = "qat-help", Label = "Help", CommandId = "copy", KeyTip = "H", Order = 0 },
                new RibbonItem { Id = "qat-history", Label = "History", CommandId = "paste", KeyTip = "H", Order = 1 },
            ],
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Order = 0,
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Order = 0,
                    Items =
                    {
                        new RibbonItem { Id = "copy", Label = "Copy", CommandId = "copy", Order = 0 },
                        new RibbonItem { Id = "paste", Label = "Paste", CommandId = "paste", Order = 1 },
                    },
                },
            },
        });

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "history",
            Header = "History",
            Order = 1,
            Groups =
            {
                new RibbonGroup
                {
                    Id = "history-tools",
                    Header = "History",
                    Order = 0,
                    Items =
                    {
                        new RibbonItem { Id = "history-copy", Label = "Copy", CommandId = "copy", Order = 0 },
                    },
                },
            },
        });

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "picture-format",
            Header = "Picture Format",
            IsContextual = true,
            ContextGroupId = "picture-tools",
            ContextGroupHeader = "Picture Tools",
            ContextGroupAccentColor = "#0F6CBD",
            ContextGroupOrder = 30,
            Order = 10,
            Groups =
            {
                new RibbonGroup
                {
                    Id = "picture-adjust",
                    Header = "Adjust",
                    Order = 0,
                    Items =
                    {
                        new RibbonItem { Id = "compress", Label = "Compress", CommandId = "paste", Order = 0 },
                    },
                },
            },
        });

        return ribbon;
    }

    private static Ribbon BuildMvvmRibbonExtended(DictionaryRibbonCommandCatalog catalog)
    {
        return new Ribbon
        {
            CommandCatalog = catalog,
            SelectedTabId = "home",
            Backstage = new RibbonBackstage
            {
                Content = new TextBlock { Text = "Backstage" },
            },
            QuickAccessItems =
            [
                new RibbonItem { Id = "qat-help", Label = "Help", CommandId = "copy", KeyTip = "H", Order = 0 },
                new RibbonItem { Id = "qat-history", Label = "History", CommandId = "paste", KeyTip = "H", Order = 1 },
            ],
            TabsSource =
            [
                new RibbonTabDefinition
                {
                    Id = "home",
                    Header = "Home",
                    Order = 0,
                    Groups =
                    {
                        new RibbonGroupDefinition
                        {
                            Id = "clipboard",
                            Header = "Clipboard",
                            Order = 0,
                            Items =
                            {
                                new RibbonItemDefinition { Id = "copy", Label = "Copy", CommandId = "copy", Order = 0 },
                                new RibbonItemDefinition { Id = "paste", Label = "Paste", CommandId = "paste", Order = 1 },
                            },
                        },
                    },
                },
                new RibbonTabDefinition
                {
                    Id = "history",
                    Header = "History",
                    Order = 1,
                    Groups =
                    {
                        new RibbonGroupDefinition
                        {
                            Id = "history-tools",
                            Header = "History",
                            Order = 0,
                            Items =
                            {
                                new RibbonItemDefinition { Id = "history-copy", Label = "Copy", CommandId = "copy", Order = 0 },
                            },
                        },
                    },
                },
                new RibbonTabDefinition
                {
                    Id = "picture-format",
                    Header = "Picture Format",
                    IsContextual = true,
                    ContextGroupId = "picture-tools",
                    ContextGroupHeader = "Picture Tools",
                    ContextGroupAccentColor = "#0F6CBD",
                    ContextGroupOrder = 30,
                    Order = 10,
                    Groups =
                    {
                        new RibbonGroupDefinition
                        {
                            Id = "picture-adjust",
                            Header = "Adjust",
                            Order = 0,
                            Items =
                            {
                                new RibbonItemDefinition { Id = "compress", Label = "Compress", CommandId = "paste", Order = 0 },
                            },
                        },
                    },
                },
            ],
        };
    }

    private static Ribbon BuildHybridRibbonExtended(DictionaryRibbonCommandCatalog catalog)
    {
        var ribbon = new Ribbon
        {
            CommandCatalog = catalog,
            SelectedTabId = "home",
            TabMergeMode = RibbonMergeMode.Merge,
            Backstage = new RibbonBackstage
            {
                Content = new TextBlock { Text = "Backstage" },
            },
            QuickAccessItems =
            [
                new RibbonItem { Id = "qat-help", Label = "Help", CommandId = "copy", KeyTip = "H", Order = 0 },
                new RibbonItem { Id = "qat-history", Label = "History", CommandId = "paste", KeyTip = "H", Order = 1 },
            ],
            TabsSource =
            [
                new RibbonTabDefinition
                {
                    Id = "home",
                    Header = "Home",
                    Order = 0,
                    Groups =
                    {
                        new RibbonGroupDefinition
                        {
                            Id = "clipboard",
                            Header = "Clipboard",
                            Order = 0,
                            Items =
                            {
                                new RibbonItemDefinition { Id = "copy", Label = "Copy", CommandId = "copy", Order = 0 },
                                new RibbonItemDefinition { Id = "paste", Label = "Paste", CommandId = "paste", Order = 1 },
                            },
                        },
                    },
                },
                new RibbonTabDefinition
                {
                    Id = "history",
                    Header = "History",
                    Order = 1,
                    Groups =
                    {
                        new RibbonGroupDefinition
                        {
                            Id = "history-tools",
                            Header = "History",
                            Order = 0,
                            Items =
                            {
                                new RibbonItemDefinition { Id = "history-copy", Label = "Copy", CommandId = "copy", Order = 0 },
                            },
                        },
                    },
                },
                new RibbonTabDefinition
                {
                    Id = "picture-format",
                    Header = "Picture Format",
                    IsContextual = true,
                    ContextGroupId = "picture-tools",
                    ContextGroupHeader = "Picture Tools",
                    ContextGroupAccentColor = "#0F6CBD",
                    ContextGroupOrder = 30,
                    Order = 10,
                    Groups =
                    {
                        new RibbonGroupDefinition
                        {
                            Id = "picture-adjust",
                            Header = "Adjust",
                            Order = 0,
                            Items =
                            {
                                new RibbonItemDefinition { Id = "compress", Label = "Compress", CommandId = "paste", Order = 0 },
                            },
                        },
                    },
                },
            ],
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Order = 0,
            Groups =
            {
                new RibbonGroup
                {
                    Id = "clipboard",
                    Header = "Clipboard",
                    Order = 0,
                    Items =
                    {
                        new RibbonItem { Id = "copy", Label = "Copy", CommandId = "copy", Order = 0 },
                        new RibbonItem { Id = "paste", Label = "Paste", CommandId = "paste", Order = 1 },
                    },
                },
            },
        });

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "history",
            Header = "History",
            Order = 1,
            Groups =
            {
                new RibbonGroup
                {
                    Id = "history-tools",
                    Header = "History",
                    Order = 0,
                    Items =
                    {
                        new RibbonItem { Id = "history-copy", Label = "Copy", CommandId = "copy", Order = 0 },
                    },
                },
            },
        });

        return ribbon;
    }

    private static Ribbon BuildAdaptiveXamlRibbon()
    {
        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                BuildAdaptiveGroup("clipboard", "Clipboard", 0, "paste"),
                BuildAdaptiveGroup("font", "Font", 1, "bold"),
                BuildAdaptiveGroup("styles", "Styles", 2, "normal"),
            },
        });

        return ribbon;
    }

    private static Ribbon BuildAdaptiveMvvmRibbon()
    {
        return new Ribbon
        {
            SelectedTabId = "home",
            TabsSource =
            [
                new RibbonTabDefinition
                {
                    Id = "home",
                    Header = "Home",
                    Groups =
                    {
                        BuildAdaptiveGroupDefinition("clipboard", "Clipboard", 0, "paste"),
                        BuildAdaptiveGroupDefinition("font", "Font", 1, "bold"),
                        BuildAdaptiveGroupDefinition("styles", "Styles", 2, "normal"),
                    },
                },
            ],
        };
    }

    private static Ribbon BuildAdaptiveHybridRibbon()
    {
        var ribbon = new Ribbon
        {
            SelectedTabId = "home",
            TabMergeMode = RibbonMergeMode.Merge,
            TabsSource =
            [
                new RibbonTabDefinition
                {
                    Id = "home",
                    Header = "Home",
                    Groups =
                    {
                        BuildAdaptiveGroupDefinition("clipboard", "Clipboard", 0, "paste"),
                        BuildAdaptiveGroupDefinition("font", "Font", 1, "bold"),
                        BuildAdaptiveGroupDefinition("styles", "Styles", 2, "normal"),
                    },
                },
            ],
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                BuildAdaptiveGroup("clipboard", "Clipboard", 0, "paste"),
                BuildAdaptiveGroup("font", "Font", 1, "bold"),
                BuildAdaptiveGroup("styles", "Styles", 2, "normal"),
            },
        });

        return ribbon;
    }

    private static Ribbon BuildPopupXamlRibbon(DictionaryRibbonCommandCatalog catalog)
    {
        var ribbon = new Ribbon
        {
            CommandCatalog = catalog,
            SelectedTabId = "layout",
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "layout",
            Header = "Layout",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "page-background",
                    Header = "Page Background",
                    Items =
                    {
                        new RibbonItem
                        {
                            Id = "page-borders",
                            Label = "Page Borders",
                            Primitive = RibbonItemPrimitive.MenuButton,
                            PopupTitle = "Border Options",
                            PopupFooterContent = "Esc to close",
                            PopupMinWidth = 320,
                            PopupMaxHeight = 380,
                            CommandId = "copy",
                            MenuItems =
                            {
                                new RibbonMenuItem
                                {
                                    Id = "weight",
                                    Label = "Weight",
                                    ShowChevron = true,
                                    Order = 0,
                                    SubMenuItems =
                                    {
                                        new RibbonMenuItem { Id = "weight-050", Label = "0.5 pt", CommandId = "copy", Order = 0 },
                                        new RibbonMenuItem { Id = "weight-100", Label = "1.0 pt", CommandId = "copy", Order = 1 },
                                    },
                                },
                                new RibbonMenuItem
                                {
                                    Id = "color",
                                    Label = "Color",
                                    ShowChevron = true,
                                    Order = 1,
                                    SubMenuItems =
                                    {
                                        new RibbonMenuItem { Id = "color-black", Label = "Black", CommandId = "copy", Order = 0 },
                                        new RibbonMenuItem { Id = "color-blue", Label = "Blue", CommandId = "copy", Order = 1 },
                                    },
                                },
                                new RibbonMenuItem
                                {
                                    Id = "remove",
                                    Label = "Remove Page Border",
                                    CommandId = "copy",
                                    ShowInRibbonPreview = false,
                                    Order = 2,
                                },
                            },
                        },
                    },
                },
            },
        });

        return ribbon;
    }

    private static Ribbon BuildPopupMvvmRibbon(DictionaryRibbonCommandCatalog catalog)
    {
        return new Ribbon
        {
            CommandCatalog = catalog,
            SelectedTabId = "layout",
            TabsSource =
            [
                new RibbonTabDefinition
                {
                    Id = "layout",
                    Header = "Layout",
                    Groups =
                    {
                        new RibbonGroupDefinition
                        {
                            Id = "page-background",
                            Header = "Page Background",
                            Items =
                            {
                                new RibbonItemDefinition
                                {
                                    Id = "page-borders",
                                    Label = "Page Borders",
                                    Primitive = RibbonItemPrimitive.MenuButton,
                                    PopupTitle = "Border Options",
                                    PopupFooterContent = "Esc to close",
                                    PopupMinWidth = 320,
                                    PopupMaxHeight = 380,
                                    CommandId = "copy",
                                    MenuItems =
                                    {
                                        new RibbonMenuItemDefinition
                                        {
                                            Id = "weight",
                                            Label = "Weight",
                                            ShowChevron = true,
                                            Order = 0,
                                            SubMenuItems =
                                            {
                                                new RibbonMenuItemDefinition { Id = "weight-050", Label = "0.5 pt", CommandId = "copy", Order = 0 },
                                                new RibbonMenuItemDefinition { Id = "weight-100", Label = "1.0 pt", CommandId = "copy", Order = 1 },
                                            },
                                        },
                                        new RibbonMenuItemDefinition
                                        {
                                            Id = "color",
                                            Label = "Color",
                                            ShowChevron = true,
                                            Order = 1,
                                            SubMenuItems =
                                            {
                                                new RibbonMenuItemDefinition { Id = "color-black", Label = "Black", CommandId = "copy", Order = 0 },
                                                new RibbonMenuItemDefinition { Id = "color-blue", Label = "Blue", CommandId = "copy", Order = 1 },
                                            },
                                        },
                                        new RibbonMenuItemDefinition
                                        {
                                            Id = "remove",
                                            Label = "Remove Page Border",
                                            CommandId = "copy",
                                            ShowInRibbonPreview = false,
                                            Order = 2,
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            ],
        };
    }

    private static Ribbon BuildPopupHybridRibbon(DictionaryRibbonCommandCatalog catalog)
    {
        var ribbon = new Ribbon
        {
            CommandCatalog = catalog,
            SelectedTabId = "layout",
            TabMergeMode = RibbonMergeMode.Merge,
            TabsSource =
            [
                new RibbonTabDefinition
                {
                    Id = "layout",
                    Header = "Layout",
                    Groups =
                    {
                        new RibbonGroupDefinition
                        {
                            Id = "page-background",
                            Header = "Page Background",
                            Items =
                            {
                                new RibbonItemDefinition
                                {
                                    Id = "page-borders",
                                    Label = "Page Borders",
                                    Primitive = RibbonItemPrimitive.MenuButton,
                                    PopupTitle = "Border Options",
                                    PopupFooterContent = "Esc to close",
                                    PopupMinWidth = 320,
                                    PopupMaxHeight = 380,
                                    CommandId = "copy",
                                    MenuItems =
                                    {
                                        new RibbonMenuItemDefinition
                                        {
                                            Id = "weight",
                                            Label = "Weight",
                                            ShowChevron = true,
                                            Order = 0,
                                            SubMenuItems =
                                            {
                                                new RibbonMenuItemDefinition { Id = "weight-050", Label = "0.5 pt", CommandId = "copy", Order = 0 },
                                                new RibbonMenuItemDefinition { Id = "weight-100", Label = "1.0 pt", CommandId = "copy", Order = 1 },
                                            },
                                        },
                                        new RibbonMenuItemDefinition
                                        {
                                            Id = "color",
                                            Label = "Color",
                                            ShowChevron = true,
                                            Order = 1,
                                            SubMenuItems =
                                            {
                                                new RibbonMenuItemDefinition { Id = "color-black", Label = "Black", CommandId = "copy", Order = 0 },
                                                new RibbonMenuItemDefinition { Id = "color-blue", Label = "Blue", CommandId = "copy", Order = 1 },
                                            },
                                        },
                                    },
                                },
                            },
                        },
                    },
                },
            ],
        };

        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "layout",
            Header = "Layout",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "page-background",
                    Header = "Page Background",
                    Items =
                    {
                        new RibbonItem
                        {
                            Id = "page-borders",
                            Label = "Page Borders",
                            Primitive = RibbonItemPrimitive.MenuButton,
                            PopupTitle = "Border Options",
                            PopupMinWidth = 260,
                            PopupMaxHeight = 340,
                            CommandId = "copy",
                            MenuItems =
                            {
                                new RibbonMenuItem
                                {
                                    Id = "weight",
                                    Label = "Weight",
                                    ShowChevron = true,
                                    Order = 0,
                                    SubMenuItems =
                                    {
                                        new RibbonMenuItem { Id = "weight-050", Label = "0.5 pt", CommandId = "copy", Order = 0 },
                                    },
                                },
                                new RibbonMenuItem
                                {
                                    Id = "remove",
                                    Label = "Remove Page Border",
                                    CommandId = "copy",
                                    ShowInRibbonPreview = false,
                                    Order = 2,
                                },
                            },
                        },
                    },
                },
            },
        });

        return ribbon;
    }

    private static RibbonItem GetPopupSampleItem(Ribbon ribbon)
    {
        return ribbon.MergedTabs
            .Single(tab => tab.Id == "layout")
            .MergedGroups
            .Single(group => group.Id == "page-background")
            .MergedItems
            .Single(item => item.Id == "page-borders");
    }

    private static string SnapshotPopupTopology(Ribbon ribbon)
    {
        var item = GetPopupSampleItem(ribbon);
        var lines = new List<string>
        {
            $"ITEM|{item.Id}|{item.Primitive}|{item.PopupTitle}|{item.PopupMinWidth}|{item.PopupMaxHeight}|{item.HasPopupFooterContent}",
        };

        foreach (var menuItem in item.PopupMenuItems.OrderBy(menuItem => menuItem.Order).ThenBy(menuItem => menuItem.Id, StringComparer.Ordinal))
        {
            lines.Add($"MENU|{menuItem.Id}|{menuItem.Label}|{menuItem.Order}|{menuItem.SubMenuItems.Count}");

            foreach (var subMenuItem in menuItem.SubMenuItems.OrderBy(subMenuItem => subMenuItem.Order).ThenBy(subMenuItem => subMenuItem.Id, StringComparer.Ordinal))
            {
                lines.Add($"SUB|{menuItem.Id}|{subMenuItem.Id}|{subMenuItem.Label}|{subMenuItem.Order}");
            }
        }

        return string.Join('\n', lines);
    }

    private static DictionaryRibbonCommandCatalog BuildCatalog(Action onExecute)
    {
        return new DictionaryRibbonCommandCatalog()
            .Register("copy", new RelayCommand(_ => onExecute()))
            .Register("paste", new RelayCommand(_ => onExecute()));
    }

    private static void ExecutePrimaryCommand(Ribbon ribbon)
    {
        var firstItem = ribbon.MergedTabs[0].MergedGroups[0].MergedItems[0];
        Assert.NotNull(firstItem.Command);
        Assert.True(firstItem.Command!.CanExecute(firstItem.CommandParameter));
        firstItem.Command.Execute(firstItem.CommandParameter);
    }

    private static string Snapshot(Ribbon ribbon)
    {
        var lines = new List<string>();
        foreach (var tab in ribbon.MergedTabs)
        {
            lines.Add(
                $"TAB|{tab.Id}|{tab.Header}|{tab.Order}|{tab.IsContextual}|{tab.ContextGroupId}|{tab.ContextGroupHeader}|{tab.ContextGroupAccentColor}|{tab.ContextGroupOrder}");
            foreach (var group in tab.MergedGroups)
            {
                lines.Add($"GROUP|{group.Id}|{group.Header}|{group.Order}");
                foreach (var item in group.MergedItems)
                {
                    lines.Add($"ITEM|{item.Id}|{item.Label}|{item.Order}");
                }
            }
        }

        return string.Join('\n', lines);
    }

    private static string SnapshotContextGroups(Ribbon ribbon)
    {
        var groups = ribbon.MergedTabs
            .Where(tab => tab.IsContextual)
            .GroupBy(
                tab => tab.ContextGroupId ?? $"tab:{tab.Id}",
                StringComparer.Ordinal)
            .Select(group =>
            {
                var first = group.OrderBy(tab => tab.Order).ThenBy(tab => tab.Id, StringComparer.Ordinal).First();
                var header = group
                    .Select(tab => tab.ContextGroupHeader)
                    .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))
                    ?? first.ContextGroupId
                    ?? first.Header;
                var accent = group
                    .Select(tab => tab.ContextGroupAccentColor)
                    .FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
                var order = group
                    .Select(tab => tab.ContextGroupOrder)
                    .FirstOrDefault(value => value.HasValue)
                    ?? first.Order;
                return $"{group.Key}|{header}|{accent}|{order}";
            })
            .OrderBy(line => line, StringComparer.Ordinal);

        return string.Join('\n', groups);
    }

    private static string SnapshotKeyTips(Ribbon ribbon)
    {
        var lines = ribbon.ActiveKeyTips
            .OrderBy(pair => pair.Key, StringComparer.Ordinal)
            .Select(pair => $"{pair.Key}|{pair.Value}");

        return string.Join('\n', lines);
    }

    private static Window ShowRibbon(Ribbon ribbon, double width = 1000)
    {
        var window = new Window
        {
            Width = width,
            Height = 700,
            Content = ribbon,
        };

        window.Show();
        window.UpdateLayout();
        return window;
    }

    private static string SnapshotAdaptiveModes(Ribbon ribbon)
    {
        var selectedTab = ribbon.MergedTabs.First(tab => tab.Id == ribbon.SelectedTabId);
        var lines = selectedTab.MergedGroups
            .OrderBy(group => group.Order)
            .ThenBy(group => group.Id, StringComparer.Ordinal)
            .Select(group => $"{group.Id}|{group.DisplayMode}");

        return string.Join('\n', lines);
    }

    private static RibbonGroup BuildAdaptiveGroup(string id, string header, int order, string itemId)
    {
        return new RibbonGroup
        {
            Id = id,
            Header = header,
            Order = order,
            Items =
            {
                new RibbonItem
                {
                    Id = itemId,
                    Label = header,
                    Primitive = RibbonItemPrimitive.Button,
                    Size = RibbonItemSize.Large,
                },
            },
        };
    }

    private static RibbonGroupDefinition BuildAdaptiveGroupDefinition(string id, string header, int order, string itemId)
    {
        return new RibbonGroupDefinition
        {
            Id = id,
            Header = header,
            Order = order,
            Items =
            {
                new RibbonItemDefinition
                {
                    Id = itemId,
                    Label = header,
                    Primitive = RibbonItemPrimitive.Button,
                    Size = RibbonItemSize.Large,
                },
            },
        };
    }

    private sealed class RelayCommand : System.Windows.Input.ICommand
    {
        private readonly Action<object?> _execute;

        public RelayCommand(Action<object?> execute)
        {
            _execute = execute;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { }
            remove { }
        }

        public bool CanExecute(object? parameter) => true;

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }
    }
}
