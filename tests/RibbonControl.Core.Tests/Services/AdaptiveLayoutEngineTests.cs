// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;
using RibbonControl.Core.Services;

namespace RibbonControl.Core.Tests.Services;

public class AdaptiveLayoutEngineTests
{
    [Fact]
    public void ApplyLayout_WhenWidthIsSufficient_KeepsExpandedModes()
    {
        var groups = BuildTwoButtonGroups();
        var engine = new RibbonAdaptiveLayoutEngine();

        var changed = engine.ApplyLayout(groups, availableWidth: 240);

        Assert.False(changed);
        Assert.All(groups, group => Assert.Equal(RibbonGroupDisplayMode.Expanded, group.DisplayMode));
    }

    [Fact]
    public void ApplyLayout_WhenModeratelyNarrow_CompactsRightmostGroupFirst()
    {
        var groups = BuildTwoButtonGroups();
        var engine = new RibbonAdaptiveLayoutEngine();

        var changed = engine.ApplyLayout(groups, availableWidth: 200);

        Assert.True(changed);
        Assert.Equal(RibbonGroupDisplayMode.Expanded, groups[0].DisplayMode);
        Assert.Equal(RibbonGroupDisplayMode.Compact, groups[1].DisplayMode);
        Assert.Equal(RibbonItemSize.Small, groups[1].MergedItems[0].EffectiveSize);
        Assert.Equal(RibbonItemSize.Large, groups[0].MergedItems[0].EffectiveSize);
    }

    [Fact]
    public void ApplyLayout_WhenVeryNarrow_CollapsesGroupsAfterCompacting()
    {
        var groups = BuildTwoButtonGroups();
        var engine = new RibbonAdaptiveLayoutEngine();

        var changed = engine.ApplyLayout(groups, availableWidth: 150);

        Assert.True(changed);
        Assert.All(groups, group => Assert.Equal(RibbonGroupDisplayMode.Collapsed, group.DisplayMode));
        Assert.All(groups, group => Assert.Equal(RibbonItemSize.Large, group.MergedItems[0].EffectiveSize));
    }

    [Fact]
    public void ApplyLayout_UsesCollapsePriorityBeforeRightToLeftOrder()
    {
        var groups = BuildTwoButtonGroups();
        groups[0].CollapsePriority = 10;
        groups[1].CollapsePriority = 0;

        var engine = new RibbonAdaptiveLayoutEngine();

        var changed = engine.ApplyLayout(groups, availableWidth: 200);

        Assert.True(changed);
        Assert.Equal(RibbonGroupDisplayMode.Compact, groups[0].DisplayMode);
        Assert.Equal(RibbonGroupDisplayMode.Expanded, groups[1].DisplayMode);
    }

    [Fact]
    public void ApplyLayout_CompactsComboAndToggleItems()
    {
        var groups = BuildComboAndToggleGroups();
        var engine = new RibbonAdaptiveLayoutEngine();

        var changed = engine.ApplyLayout(groups, availableWidth: 460);

        Assert.True(changed);
        Assert.All(groups, group => Assert.Equal(RibbonGroupDisplayMode.Compact, group.DisplayMode));
        Assert.All(groups.SelectMany(g => g.MergedItems), item => Assert.Equal(RibbonItemSize.Small, item.EffectiveSize));
    }

    [Fact]
    public void ApplyLayout_CompactsLargeSideBySideSplitButtons()
    {
        IReadOnlyList<RibbonGroup> groups =
        [
            new RibbonGroup
            {
                Id = "paragraph",
                Header = "Paragraph",
                Order = 0,
                Items =
                {
                    new RibbonItem
                    {
                        Id = "bullets",
                        Label = "Bullets",
                        Primitive = RibbonItemPrimitive.SplitButton,
                        SplitButtonMode = RibbonSplitButtonMode.SideBySide,
                        Size = RibbonItemSize.Large,
                    },
                },
            },
        ];

        var engine = new RibbonAdaptiveLayoutEngine();
        var changed = engine.ApplyLayout(groups, availableWidth: 100);

        Assert.True(changed);
        Assert.Equal(RibbonGroupDisplayMode.Compact, groups[0].DisplayMode);
        Assert.Equal(RibbonItemSize.Small, groups[0].MergedItems[0].EffectiveSize);
        Assert.True(groups[0].MergedItems[0].IsSideBySideSmallSplitButtonPrimitive);
    }

    [Fact]
    public void ApplyLayout_StackedGroupLayout_UsesNarrowColumnBasedEstimate()
    {
        IReadOnlyList<RibbonGroup> groups =
        [
            new RibbonGroup
            {
                Id = "history",
                Header = "Undo",
                Order = 0,
                ItemsLayoutMode = RibbonGroupItemsLayoutMode.Stacked,
                StackedRows = 2,
                Items =
                {
                    new RibbonItem
                    {
                        Id = "undo",
                        Label = "Undo",
                        Primitive = RibbonItemPrimitive.Button,
                        Size = RibbonItemSize.Small,
                        DisplayMode = RibbonItemDisplayMode.IconOnly,
                    },
                    new RibbonItem
                    {
                        Id = "redo",
                        Label = "Redo",
                        Primitive = RibbonItemPrimitive.Button,
                        Size = RibbonItemSize.Small,
                        DisplayMode = RibbonItemDisplayMode.IconOnly,
                    },
                },
            },
        ];

        var engine = new RibbonAdaptiveLayoutEngine();
        var changed = engine.ApplyLayout(groups, availableWidth: 70);

        Assert.False(changed);
        Assert.Equal(RibbonGroupDisplayMode.Expanded, groups[0].DisplayMode);
    }

    [Fact]
    public void ApplyLayout_DockedGroup_CanCompactMixedDockedItems()
    {
        IReadOnlyList<RibbonGroup> groups =
        [
            new RibbonGroup
            {
                Id = "footnotes",
                Header = "Footnotes",
                Order = 0,
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
                        Size = RibbonItemSize.Large,
                        LayoutDock = RibbonItemLayoutDock.Left,
                    },
                    new RibbonItem
                    {
                        Id = "insert-endnote",
                        Label = "Insert Endnote",
                        Primitive = RibbonItemPrimitive.Button,
                        Size = RibbonItemSize.Small,
                        LayoutDock = RibbonItemLayoutDock.Center,
                    },
                    new RibbonItem
                    {
                        Id = "show-footnotes",
                        Label = "Show Footnotes",
                        Primitive = RibbonItemPrimitive.Button,
                        Size = RibbonItemSize.Small,
                        LayoutDock = RibbonItemLayoutDock.Center,
                    },
                },
            },
        ];

        var engine = new RibbonAdaptiveLayoutEngine();
        var changed = engine.ApplyLayout(groups, availableWidth: 120);

        Assert.True(changed);
        Assert.Equal(RibbonGroupDisplayMode.Collapsed, groups[0].DisplayMode);
    }

    private static IReadOnlyList<RibbonGroup> BuildTwoButtonGroups()
    {
        return
        [
            new RibbonGroup
            {
                Id = "clipboard",
                Header = "Clipboard",
                Order = 0,
                Items =
                {
                    new RibbonItem
                    {
                        Id = "paste",
                        Label = "Paste",
                        Primitive = RibbonItemPrimitive.Button,
                        Size = RibbonItemSize.Large,
                    },
                },
            },
            new RibbonGroup
            {
                Id = "font",
                Header = "Font",
                Order = 1,
                Items =
                {
                    new RibbonItem
                    {
                        Id = "bold",
                        Label = "Bold",
                        Primitive = RibbonItemPrimitive.Button,
                        Size = RibbonItemSize.Large,
                    },
                },
            },
        ];
    }

    private static IReadOnlyList<RibbonGroup> BuildComboAndToggleGroups()
    {
        return
        [
            new RibbonGroup
            {
                Id = "font-family",
                Header = "Font",
                Order = 0,
                Items =
                {
                    new RibbonItem
                    {
                        Id = "font-family",
                        Label = "Font",
                        Primitive = RibbonItemPrimitive.ComboBox,
                        Size = RibbonItemSize.Large,
                        MenuItems =
                        {
                            new RibbonMenuItem { Id = "font-family-aptos", Label = "Aptos (Body)", IsSelected = true, Order = 0 },
                            new RibbonMenuItem { Id = "font-family-calibri", Label = "Calibri", Order = 1 },
                        },
                    },
                },
            },
            new RibbonGroup
            {
                Id = "font-style",
                Header = "Style",
                Order = 1,
                Items =
                {
                    new RibbonItem
                    {
                        Id = "bold",
                        Label = "B",
                        Primitive = RibbonItemPrimitive.ToggleButton,
                        Size = RibbonItemSize.Large,
                    },
                },
            },
            new RibbonGroup
            {
                Id = "table-style-options",
                Header = "Table Style Options",
                Order = 2,
                Items =
                {
                    new RibbonItem
                    {
                        Id = "table-style-options",
                        Label = "Table Style Options",
                        Primitive = RibbonItemPrimitive.ToggleGroup,
                        ToggleGroupColumns = 2,
                        ToggleGroupSelectionMode = RibbonToggleGroupSelectionMode.Multiple,
                        Size = RibbonItemSize.Large,
                        MenuItems =
                        {
                            new RibbonMenuItem { Id = "header-row", Label = "Header Row", Order = 0, IsSelected = true },
                            new RibbonMenuItem { Id = "first-column", Label = "First Column", Order = 1, IsSelected = true },
                            new RibbonMenuItem { Id = "banded-rows", Label = "Banded Rows", Order = 2 },
                        },
                    },
                },
            },
        ];
    }
}
