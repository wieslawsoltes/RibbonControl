// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;
using RibbonControl.Core.Services;
using RibbonControl.Core.ViewModels;
using Avalonia.Media;

namespace RibbonControl.Core.Tests.Services;

public class MergePolicyTests
{
    [Fact]
    public void MergeTabs_StaticAndDynamicWithSameId_UsesMergedMetadataAndKeepsStaticOrder()
    {
        var staticTab = new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Order = 0,
        };
        staticTab.Groups.Add(new RibbonGroup { Id = "static-group", Header = "Static Group", Order = 0 });

        var dynamicTab = new RibbonTabDefinition
        {
            Id = "home",
            Header = "Home+",
            Order = 0,
            Groups =
            {
                new RibbonGroupDefinition
                {
                    Id = "dynamic-group",
                    Header = "Dynamic Group",
                    Order = 1,
                    Items = { new RibbonItemDefinition { Id = "share", Label = "Share", Order = 0 } },
                },
            },
        };

        var pluginTab = new RibbonTabDefinition
        {
            Id = "plugin",
            Header = "Plugin",
            Order = 10,
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeTabs([staticTab], [dynamicTab, pluginTab], RibbonMergeMode.Merge);

        Assert.Equal(2, merged.Count);
        Assert.Equal("home", merged[0].Id);
        Assert.Equal("Home+", merged[0].Header);
        Assert.Equal("plugin", merged[1].Id);
        Assert.Contains(merged[0].MergedGroups, g => g.Id == "static-group");
        Assert.Contains(merged[0].MergedGroups, g => g.Id == "dynamic-group");
    }

    [Fact]
    public void MergeTabs_ContextualMetadata_FallsBackToStaticWhenDynamicOmitsContextGroupSettings()
    {
        var staticTab = new RibbonTab
        {
            Id = "table-design",
            Header = "Table Design",
            Order = 10,
            IsContextual = true,
            ContextGroupId = "table-tools",
            ContextGroupHeader = "Table Tools",
            ContextGroupAccentColor = "#0F6CBD",
            ContextGroupOrder = 40,
        };

        var dynamicTab = new RibbonTabDefinition
        {
            Id = "table-design",
            Header = "Table Design+",
            Order = 9,
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeTabs([staticTab], [dynamicTab], RibbonMergeMode.Merge);

        var tab = Assert.Single(merged);
        Assert.Equal("Table Design+", tab.Header);
        Assert.True(tab.IsContextual);
        Assert.Equal("table-tools", tab.ContextGroupId);
        Assert.Equal("Table Tools", tab.ContextGroupHeader);
        Assert.Equal("#0F6CBD", tab.ContextGroupAccentColor);
        Assert.Equal(40, tab.ContextGroupOrder);
    }

    [Fact]
    public void MergeItems_PrimitiveAndMenuItems_AreMergedById()
    {
        var staticContent = new object();
        var staticItem = new RibbonItem
        {
            Id = "paste",
            Label = "Paste",
            Primitive = RibbonItemPrimitive.SplitButton,
            Size = RibbonItemSize.Large,
            Content = staticContent,
            CommandId = "paste",
            SecondaryCommandId = "paste",
            Order = 0,
            MenuItems =
            {
                new RibbonMenuItem
                {
                    Id = "menu-default",
                    Label = "Default Paste",
                    CommandId = "paste",
                    Order = 0,
                },
            },
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "paste",
            Label = "Paste+",
            Primitive = RibbonItemPrimitive.MenuButton,
            Size = RibbonItemSize.Small,
            CommandId = "paste",
            Order = 0,
            MenuItems =
            {
                new RibbonMenuItemDefinition
                {
                    Id = "menu-default",
                    Label = "Keep Source Formatting",
                    CommandId = "paste",
                    InputGestureText = "Cmd+V",
                    ShowInRibbonPreview = true,
                    ShowInPopup = true,
                    Order = 0,
                },
                new RibbonMenuItemDefinition
                {
                    Id = "menu-text-only",
                    Label = "Text Only",
                    CommandId = "paste",
                    ShowInRibbonPreview = false,
                    ShowInPopup = true,
                    Order = 1,
                },
            },
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var mergedItem = Assert.Single(merged);
        Assert.Equal("Paste+", mergedItem.Label);
        Assert.Equal(RibbonItemPrimitive.MenuButton, mergedItem.Primitive);
        Assert.Equal(RibbonItemSize.Small, mergedItem.Size);
        Assert.Same(staticContent, mergedItem.Content);
        Assert.Equal(2, mergedItem.MenuItems.Count);
        Assert.Contains(mergedItem.MenuItems, x => x.Id == "menu-text-only");
        Assert.Contains(mergedItem.MenuItems, x => x.Id == "menu-default" && x.Label == "Keep Source Formatting");
        Assert.Contains(mergedItem.MenuItems, x => x.Id == "menu-default" && x.InputGestureText == "Cmd+V");
        Assert.Contains(mergedItem.MenuItems, x => x.Id == "menu-text-only" && !x.ShowInRibbonPreview && x.ShowInPopup);
    }

    [Fact]
    public void MergeItems_NestedItems_AreMergedById()
    {
        var staticItem = new RibbonItem
        {
            Id = "font-group",
            Label = "Font Group",
            Primitive = RibbonItemPrimitive.Group,
            Items =
            {
                new RibbonItem
                {
                    Id = "font-row",
                    Label = "Font Row",
                    Primitive = RibbonItemPrimitive.Row,
                    Order = 0,
                    Items =
                    {
                        new RibbonItem
                        {
                            Id = "bold",
                            Label = "Bold",
                            Primitive = RibbonItemPrimitive.ToggleButton,
                            IsToggle = true,
                            IsChecked = false,
                            Order = 0,
                        },
                        new RibbonItem
                        {
                            Id = "italic",
                            Label = "Italic",
                            Primitive = RibbonItemPrimitive.ToggleButton,
                            IsToggle = true,
                            IsChecked = false,
                            Order = 1,
                        },
                    },
                },
            },
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "font-group",
            Label = "Font Group+",
            Primitive = RibbonItemPrimitive.Group,
            Items =
            {
                new RibbonItemDefinition
                {
                    Id = "font-row",
                    Label = "Font Row+",
                    Primitive = RibbonItemPrimitive.Row,
                    Order = 0,
                    Items =
                    {
                        new RibbonItemDefinition
                        {
                            Id = "bold",
                            Label = "Bold+",
                            Primitive = RibbonItemPrimitive.ToggleButton,
                            IsToggle = true,
                            IsChecked = true,
                            Order = 0,
                        },
                        new RibbonItemDefinition
                        {
                            Id = "underline",
                            Label = "Underline",
                            Primitive = RibbonItemPrimitive.ToggleButton,
                            IsToggle = true,
                            IsChecked = false,
                            Order = 2,
                        },
                    },
                },
            },
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var mergedItem = Assert.Single(merged);
        var mergedRow = Assert.Single(mergedItem.Items);
        var mergedBold = Assert.Single(mergedRow.Items, item => item.Id == "bold");
        Assert.Equal("Font Group+", mergedItem.Label);
        Assert.Equal("Font Row+", mergedRow.Label);
        Assert.Equal("Bold+", mergedBold.Label);
        Assert.True(mergedBold.IsChecked);
        Assert.Contains(mergedRow.Items, item => item.Id == "italic");
        Assert.Contains(mergedRow.Items, item => item.Id == "underline");
    }

    [Fact]
    public void MergeItems_IconMetadata_UsesDynamicValuesAndStaticFallback()
    {
        var staticItem = new RibbonItem
        {
            Id = "font-color",
            Label = "Font Color",
            Primitive = RibbonItemPrimitive.Button,
            IconResourceKey = "Ribbon.Static.FontColor",
            IconPathData = "M0 0",
            IconEmoji = "🎨",
            IconStretch = Stretch.UniformToFill,
            IconStretchDirection = StretchDirection.DownOnly,
            IconWidth = 14,
            IconHeight = 16,
            IconMinWidth = 12,
            IconMinHeight = 12,
            IconMaxWidth = 22,
            IconMaxHeight = 24,
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "font-color",
            Label = "Font Color",
            Primitive = RibbonItemPrimitive.Button,
            IconResourceKey = "Ribbon.Dynamic.FontColor",
            IconPathData = null,
            IconEmoji = null,
            IconStretch = Stretch.Fill,
            IconStretchDirection = StretchDirection.Both,
            IconWidth = 18,
            IconHeight = double.NaN,
            IconMinWidth = 0,
            IconMinHeight = 14,
            IconMaxWidth = 26,
            IconMaxHeight = double.PositiveInfinity,
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var item = Assert.Single(merged);
        Assert.Equal("Ribbon.Dynamic.FontColor", item.IconResourceKey);
        Assert.Equal("M0 0", item.IconPathData);
        Assert.Equal("🎨", item.IconEmoji);
        Assert.Equal(Stretch.Fill, item.IconStretch);
        Assert.Equal(StretchDirection.Both, item.IconStretchDirection);
        Assert.Equal(18, item.IconWidth);
        Assert.Equal(16, item.IconHeight);
        Assert.Equal(12, item.IconMinWidth);
        Assert.Equal(14, item.IconMinHeight);
        Assert.Equal(26, item.IconMaxWidth);
        Assert.Equal(24, item.IconMaxHeight);
    }

    [Fact]
    public void MergeItems_MenuIconMetadata_UsesDynamicValuesAndStaticFallback()
    {
        var staticItem = new RibbonItem
        {
            Id = "paste",
            Label = "Paste",
            Primitive = RibbonItemPrimitive.MenuButton,
            MenuItems =
            {
                new RibbonMenuItem
                {
                    Id = "paste-default",
                    Label = "Paste",
                    IconPathData = "M1 1",
                    IconEmoji = "📋",
                    IconMaxHeight = 18,
                    Order = 0,
                },
            },
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "paste",
            Label = "Paste",
            Primitive = RibbonItemPrimitive.MenuButton,
            MenuItems =
            {
                new RibbonMenuItemDefinition
                {
                    Id = "paste-default",
                    Label = "Paste",
                    IconResourceKey = "Ribbon.Dynamic.Paste",
                    IconPathData = null,
                    IconEmoji = null,
                    IconWidth = 15,
                    IconHeight = 15,
                    Order = 0,
                },
            },
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var item = Assert.Single(merged);
        var menuItem = Assert.Single(item.MenuItems);
        Assert.Equal("Ribbon.Dynamic.Paste", menuItem.IconResourceKey);
        Assert.Equal("M1 1", menuItem.IconPathData);
        Assert.Equal("📋", menuItem.IconEmoji);
        Assert.Equal(15, menuItem.IconWidth);
        Assert.Equal(15, menuItem.IconHeight);
        Assert.Equal(18, menuItem.IconMaxHeight);
    }

    [Fact]
    public void MergeItems_SplitButtonMode_UsesDynamicValue()
    {
        var staticItem = new RibbonItem
        {
            Id = "cell-shading",
            Label = "Cell Shading",
            Primitive = RibbonItemPrimitive.SplitButton,
            SplitButtonMode = RibbonSplitButtonMode.SideBySide,
            CommandId = "page-color",
            SecondaryCommandId = "page-color",
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "cell-shading",
            Label = "Cell Shading",
            Primitive = RibbonItemPrimitive.SplitButton,
            SplitButtonMode = RibbonSplitButtonMode.Stacked,
            CommandId = "page-color",
            SecondaryCommandId = "page-color",
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var item = Assert.Single(merged);
        Assert.Equal(RibbonSplitButtonMode.Stacked, item.SplitButtonMode);
        Assert.True(item.IsStackedSplitButtonPrimitive);
    }

    [Fact]
    public void MergeItems_DisplayMode_FallsBackToStaticWhenDynamicIsAuto()
    {
        var staticItem = new RibbonItem
        {
            Id = "undo",
            Label = "Undo",
            Primitive = RibbonItemPrimitive.Button,
            Size = RibbonItemSize.Small,
            DisplayMode = RibbonItemDisplayMode.IconOnly,
            CommandId = "undo",
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "undo",
            Label = "Undo+",
            Primitive = RibbonItemPrimitive.Button,
            Size = RibbonItemSize.Small,
            DisplayMode = RibbonItemDisplayMode.Auto,
            CommandId = "undo",
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var item = Assert.Single(merged);
        Assert.Equal("Undo+", item.Label);
        Assert.Equal(RibbonItemDisplayMode.IconOnly, item.DisplayMode);
    }

    [Fact]
    public void MergeItems_ToggleAndComboState_UsesDynamicSelectionMetadata()
    {
        var staticItem = new RibbonItem
        {
            Id = "font-size",
            Label = "Size",
            Primitive = RibbonItemPrimitive.ComboBox,
            Size = RibbonItemSize.Small,
            SelectedMenuItemId = "font-size-10",
            MenuItems =
            {
                new RibbonMenuItem { Id = "font-size-10", Label = "10", CommandId = "font-size-10", Order = 0 },
                new RibbonMenuItem { Id = "font-size-12", Label = "12", CommandId = "font-size-12", Order = 1 },
            },
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "font-size",
            Label = "Size",
            Primitive = RibbonItemPrimitive.ToggleButton,
            Size = RibbonItemSize.Small,
            IsChecked = true,
            SelectedMenuItemId = "font-size-12",
            MenuItems =
            {
                new RibbonMenuItemDefinition { Id = "font-size-12", Label = "12", CommandId = "font-size-12", Order = 1 },
            },
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var item = Assert.Single(merged);
        Assert.Equal(RibbonItemPrimitive.ToggleButton, item.Primitive);
        Assert.True(item.IsChecked);
        Assert.Equal("font-size-12", item.SelectedMenuItemId);
        Assert.Equal("font-size-12", item.SelectedComboBoxMenuItem?.Id);
    }

    [Fact]
    public void MergeItems_ToggleEnabledButton_PreservesStaticOrDynamicToggleMetadata()
    {
        var staticItem = new RibbonItem
        {
            Id = "ruler",
            Label = "Ruler",
            Primitive = RibbonItemPrimitive.Button,
            IsToggle = true,
            IsChecked = true,
            CommandId = "ruler",
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "ruler",
            Label = "Ruler+",
            Primitive = RibbonItemPrimitive.Button,
            IsToggle = false,
            IsChecked = false,
            CommandId = "ruler",
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var item = Assert.Single(merged);
        Assert.Equal("Ruler+", item.Label);
        Assert.True(item.IsToggle);
        Assert.True(item.IsToggleButtonPrimitive);
        Assert.False(item.IsChecked);
    }

    [Fact]
    public void MergeItems_ToggleEnabledButton_SynchronizesWithDynamicObservableNode()
    {
        var staticItem = new RibbonItem
        {
            Id = "ruler",
            Label = "Ruler",
            Primitive = RibbonItemPrimitive.Button,
            IsToggle = true,
            IsChecked = true,
            CommandId = "ruler",
        };

        var dynamicItem = new RibbonItemViewModel
        {
            Id = "ruler",
            Label = "Ruler",
            Primitive = RibbonItemPrimitive.Button,
            IsToggle = true,
            IsChecked = false,
            CommandId = "ruler",
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);
        var item = Assert.Single(merged);

        Assert.False(item.IsChecked);
        dynamicItem.IsChecked = true;
        Assert.True(item.IsChecked);

        item.IsChecked = false;
        Assert.False(dynamicItem.IsChecked);
    }

    [Fact]
    public void MergeItems_ToggleGroupSelectionMetadata_UsesDynamicDefinition()
    {
        var staticItem = new RibbonItem
        {
            Id = "table-style-options",
            Label = "Table Style Options",
            Primitive = RibbonItemPrimitive.ToggleGroup,
            ToggleGroupSelectionMode = RibbonToggleGroupSelectionMode.Multiple,
            ToggleGroupColumns = 2,
            MenuItems =
            {
                new RibbonMenuItem { Id = "header-row", Label = "Header Row", IsSelected = true, Order = 0 },
                new RibbonMenuItem { Id = "first-column", Label = "First Column", Order = 1 },
            },
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "table-style-options",
            Label = "Table Style Options+",
            Primitive = RibbonItemPrimitive.ToggleGroup,
            ToggleGroupSelectionMode = RibbonToggleGroupSelectionMode.Single,
            ToggleGroupColumns = 1,
            SelectedMenuItemId = "first-column",
            MenuItems =
            {
                new RibbonMenuItemDefinition { Id = "first-column", Label = "First Column", IsSelected = true, Order = 0 },
            },
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var item = Assert.Single(merged);
        Assert.Equal("Table Style Options+", item.Label);
        Assert.Equal(RibbonItemPrimitive.ToggleGroup, item.Primitive);
        Assert.Equal(RibbonToggleGroupSelectionMode.Single, item.ToggleGroupSelectionMode);
        Assert.True(item.IsToggleGroupSingleSelectionMode);
        Assert.Equal(1, item.ToggleGroupColumns);
        Assert.Equal("first-column", item.SelectedMenuItemId);
        Assert.Equal("first-column", item.SelectedComboBoxMenuItem?.Id);
    }

    [Fact]
    public void MergeGroups_AdaptiveMetadata_IsResolvedFromDynamicAndStaticFallbacks()
    {
        var staticGroup = new RibbonGroup
        {
            Id = "font",
            Header = "Font",
            IconPathData = "M0 0",
            HeaderPlacement = RibbonGroupHeaderPlacement.Right,
            ItemsLayoutMode = RibbonGroupItemsLayoutMode.Stacked,
            StackedRows = 2,
            ExpandedWidthHint = 240,
            CompactWidthHint = 180,
            CollapsedWidthHint = 92,
            CanAutoCollapse = true,
            CollapsePriority = 1,
            Items =
            {
                new RibbonItem { Id = "bold", Label = "Bold", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Large },
            },
        };

        var dynamicGroup = new RibbonGroupDefinition
        {
            Id = "font",
            Header = "Font+",
            HeaderPlacement = RibbonGroupHeaderPlacement.Left,
            ItemsLayoutMode = RibbonGroupItemsLayoutMode.Auto,
            StackedRows = 0,
            CollapsePriority = 10,
            CompactWidthHint = 160,
            Items =
            {
                new RibbonItemDefinition { Id = "bold", Label = "Bold+", Primitive = RibbonItemPrimitive.Button, Size = RibbonItemSize.Large },
            },
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeGroups([staticGroup], [dynamicGroup], RibbonMergeMode.Merge);

        var group = Assert.Single(merged);
        Assert.Equal("Font+", group.Header);
        Assert.Equal("M0 0", group.IconPathData);
        Assert.Equal(RibbonGroupHeaderPlacement.Left, group.HeaderPlacement);
        Assert.Equal(RibbonGroupItemsLayoutMode.Stacked, group.ItemsLayoutMode);
        Assert.Equal(2, group.StackedRows);
        Assert.Equal(240, group.ExpandedWidthHint);
        Assert.Equal(160, group.CompactWidthHint);
        Assert.Equal(92, group.CollapsedWidthHint);
        Assert.Equal(10, group.CollapsePriority);
        Assert.True(group.CanAutoCollapse);
    }

    [Fact]
    public void MergeGroups_DockedCenterLayoutMode_UsesDynamicValueAndFallback()
    {
        var staticGroup = new RibbonGroup
        {
            Id = "footnotes",
            Header = "Footnotes",
            ItemsLayoutMode = RibbonGroupItemsLayoutMode.Docked,
            DockedCenterLayoutMode = RibbonGroupDockedCenterLayoutMode.Stacked,
            StackedRows = 2,
            Items =
            {
                new RibbonItem
                {
                    Id = "insert-footnote",
                    Label = "Insert Footnote",
                    Primitive = RibbonItemPrimitive.Button,
                    LayoutDock = RibbonItemLayoutDock.Left,
                },
            },
        };

        var fallbackDynamic = new RibbonGroupDefinition
        {
            Id = "footnotes",
            Header = "Footnotes+",
            ItemsLayoutMode = RibbonGroupItemsLayoutMode.Docked,
            DockedCenterLayoutMode = RibbonGroupDockedCenterLayoutMode.Auto,
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var fallbackMerged = mergePolicy.MergeGroups([staticGroup], [fallbackDynamic], RibbonMergeMode.Merge);
        var fallbackGroup = Assert.Single(fallbackMerged);
        Assert.Equal(RibbonGroupDockedCenterLayoutMode.Stacked, fallbackGroup.DockedCenterLayoutMode);

        var overrideDynamic = new RibbonGroupDefinition
        {
            Id = "footnotes",
            Header = "Footnotes+",
            ItemsLayoutMode = RibbonGroupItemsLayoutMode.Docked,
            DockedCenterLayoutMode = RibbonGroupDockedCenterLayoutMode.Horizontal,
        };

        var overrideMerged = mergePolicy.MergeGroups([staticGroup], [overrideDynamic], RibbonMergeMode.Merge);
        var overrideGroup = Assert.Single(overrideMerged);
        Assert.Equal(RibbonGroupDockedCenterLayoutMode.Horizontal, overrideGroup.DockedCenterLayoutMode);
    }

    [Fact]
    public void MergeItems_LayoutDock_UsesDynamicValueAndFallback()
    {
        var staticItem = new RibbonItem
        {
            Id = "insert-footnote",
            Label = "Insert Footnote",
            Primitive = RibbonItemPrimitive.Button,
            LayoutDock = RibbonItemLayoutDock.Left,
        };

        var fallbackDynamic = new RibbonItemDefinition
        {
            Id = "insert-footnote",
            Label = "Insert Footnote+",
            Primitive = RibbonItemPrimitive.Button,
            LayoutDock = RibbonItemLayoutDock.Auto,
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var fallbackMerged = mergePolicy.MergeItems([staticItem], [fallbackDynamic], RibbonMergeMode.Merge);
        var fallbackItem = Assert.Single(fallbackMerged);
        Assert.Equal(RibbonItemLayoutDock.Left, fallbackItem.LayoutDock);

        var overrideDynamic = new RibbonItemDefinition
        {
            Id = "insert-footnote",
            Label = "Insert Footnote+",
            Primitive = RibbonItemPrimitive.Button,
            LayoutDock = RibbonItemLayoutDock.Center,
        };

        var overrideMerged = mergePolicy.MergeItems([staticItem], [overrideDynamic], RibbonMergeMode.Merge);
        var overrideItem = Assert.Single(overrideMerged);
        Assert.Equal(RibbonItemLayoutDock.Center, overrideItem.LayoutDock);
    }

    [Fact]
    public void MergeItems_PopupMetadataAndNestedSubMenus_AreMergedById()
    {
        var staticItem = new RibbonItem
        {
            Id = "style-gallery",
            Label = "Styles",
            Primitive = RibbonItemPrimitive.Gallery,
            PopupTitle = "Styles",
            PopupMinWidth = 240,
            PopupMaxHeight = 480,
            MenuItems =
            {
                new RibbonMenuItem
                {
                    Id = "style-manage",
                    Label = "Manage Default Styles",
                    ShowChevron = true,
                    PopupSectionId = "style-actions",
                    PopupSectionOrder = 1,
                    Order = 0,
                    SubMenuItems =
                    {
                        new RibbonMenuItem { Id = "style-manage-document", Label = "Document", CommandId = "style-manage-document", Order = 0 },
                        new RibbonMenuItem { Id = "style-manage-all", Label = "All Documents", CommandId = "style-manage-all", Order = 1 },
                    },
                },
            },
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "style-gallery",
            Label = "Styles+",
            Primitive = RibbonItemPrimitive.Gallery,
            PopupTitle = "Quick Styles",
            PopupFooterContent = "Esc to close",
            PopupMinWidth = 300,
            PopupMaxHeight = 360,
            MenuItems =
            {
                new RibbonMenuItemDefinition
                {
                    Id = "style-manage",
                    Label = "Manage Default Styles",
                    ShowChevron = true,
                    PopupSectionHeader = "Actions",
                    PopupSectionOrder = 1,
                    Order = 0,
                    SubMenuItems =
                    {
                        new RibbonMenuItemDefinition { Id = "style-manage-document", Label = "This Document", CommandId = "style-manage-document", Order = 0 },
                        new RibbonMenuItemDefinition { Id = "style-manage-team", Label = "Team Templates", CommandId = "style-manage-team", Order = 2 },
                    },
                },
            },
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var mergedItem = Assert.Single(merged);
        Assert.Equal("Styles+", mergedItem.Label);
        Assert.Equal("Quick Styles", mergedItem.PopupTitle);
        Assert.Equal("Esc to close", mergedItem.PopupFooterContent);
        Assert.Equal(300, mergedItem.PopupMinWidth);
        Assert.Equal(360, mergedItem.PopupMaxHeight);

        var manage = Assert.Single(mergedItem.MenuItems);
        Assert.Equal("style-manage", manage.Id);
        Assert.Equal("style-actions", manage.PopupSectionId);
        Assert.Equal("Actions", manage.PopupSectionHeader);
        Assert.Equal(1, manage.PopupSectionOrder);
        Assert.Equal(3, manage.SubMenuItems.Count);
        Assert.Contains(manage.SubMenuItems, x => x.Id == "style-manage-document" && x.Label == "This Document");
        Assert.Contains(manage.SubMenuItems, x => x.Id == "style-manage-all" && x.Label == "All Documents");
        Assert.Contains(manage.SubMenuItems, x => x.Id == "style-manage-team" && x.Label == "Team Templates");
    }

    [Fact]
    public void MergeItems_GalleryCategoryAndSelectionMetadata_AreMergedById()
    {
        var staticItem = new RibbonItem
        {
            Id = "table-styles",
            Label = "Table Styles",
            Primitive = RibbonItemPrimitive.Gallery,
            GalleryPreviewMaxItems = 4,
            GalleryShowCategoryHeaders = true,
            MenuItems =
            {
                new RibbonMenuItem
                {
                    Id = "style-plain",
                    Label = "Plain",
                    Category = "Plain Tables",
                    IsSelected = true,
                    ShowInRibbonPreview = true,
                    ShowInPopup = true,
                    Order = 0,
                },
            },
        };

        var dynamicItem = new RibbonItemDefinition
        {
            Id = "table-styles",
            Label = "Table Styles+",
            Primitive = RibbonItemPrimitive.Gallery,
            GalleryPreviewMaxItems = 6,
            GalleryShowCategoryHeaders = false,
            MenuItems =
            {
                new RibbonMenuItemDefinition
                {
                    Id = "style-plain",
                    Label = "Plain Modern",
                    Category = "Grid Tables",
                    IsSelected = false,
                    ShowInRibbonPreview = true,
                    ShowInPopup = true,
                    Order = 0,
                },
                new RibbonMenuItemDefinition
                {
                    Id = "style-grid",
                    Label = "Grid",
                    Category = "Grid Tables",
                    IsSelected = true,
                    ShowInRibbonPreview = true,
                    ShowInPopup = true,
                    Order = 1,
                },
            },
        };

        var mergePolicy = RibbonMergePolicy.StaticThenDynamic;
        var merged = mergePolicy.MergeItems([staticItem], [dynamicItem], RibbonMergeMode.Merge);

        var mergedItem = Assert.Single(merged);
        Assert.Equal("Table Styles+", mergedItem.Label);
        Assert.Equal(6, mergedItem.GalleryPreviewMaxItems);
        Assert.False(mergedItem.GalleryShowCategoryHeaders);
        Assert.Equal(2, mergedItem.MenuItems.Count);
        Assert.Contains(mergedItem.MenuItems, x => x.Id == "style-plain" && x.Category == "Grid Tables");
        Assert.Equal("style-grid", mergedItem.SelectedGalleryMenuItem?.Id);
    }
}
