// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.Tests.Models;

public class RibbonGroupTests
{
    [Fact]
    public void HeaderPlacement_DefaultsToBottom()
    {
        var group = new RibbonGroup();

        Assert.Equal(RibbonGroupHeaderPlacement.Bottom, group.HeaderPlacement);
        Assert.True(group.IsHeaderBottom);
        Assert.False(group.IsHeaderTop);
        Assert.False(group.IsHeaderLeft);
        Assert.False(group.IsHeaderRight);
    }

    [Fact]
    public void HeaderPlacement_FlagsFollowSelectedPlacement()
    {
        var group = new RibbonGroup();

        group.HeaderPlacement = RibbonGroupHeaderPlacement.Top;
        Assert.True(group.IsHeaderTop);
        Assert.False(group.IsHeaderBottom);

        group.HeaderPlacement = RibbonGroupHeaderPlacement.Left;
        Assert.True(group.IsHeaderLeft);
        Assert.False(group.IsHeaderTop);

        group.HeaderPlacement = RibbonGroupHeaderPlacement.Right;
        Assert.True(group.IsHeaderRight);
        Assert.False(group.IsHeaderLeft);
    }

    [Fact]
    public void ItemsLayoutMode_DefaultsToWrapViaAuto()
    {
        var group = new RibbonGroup();

        Assert.Equal(RibbonGroupItemsLayoutMode.Auto, group.ItemsLayoutMode);
        Assert.Equal(RibbonGroupItemsLayoutMode.Wrap, group.EffectiveItemsLayoutMode);
        Assert.True(group.IsItemsWrapLayout);
        Assert.False(group.IsItemsHorizontalLayout);
        Assert.False(group.IsItemsVerticalLayout);
        Assert.False(group.IsItemsStackedLayout);
        Assert.False(group.IsItemsDockedLayout);
        Assert.Equal(RibbonGroupDockedCenterLayoutMode.Auto, group.DockedCenterLayoutMode);
        Assert.Equal(RibbonGroupDockedCenterLayoutMode.Wrap, group.EffectiveDockedCenterLayoutMode);
    }

    [Fact]
    public void ItemsLayoutMode_FlagsFollowSelectedLayout()
    {
        var group = new RibbonGroup();

        group.ItemsLayoutMode = RibbonGroupItemsLayoutMode.Horizontal;
        Assert.True(group.IsItemsHorizontalLayout);
        Assert.False(group.IsItemsWrapLayout);

        group.ItemsLayoutMode = RibbonGroupItemsLayoutMode.Vertical;
        Assert.True(group.IsItemsVerticalLayout);
        Assert.False(group.IsItemsHorizontalLayout);

        group.ItemsLayoutMode = RibbonGroupItemsLayoutMode.Stacked;
        Assert.True(group.IsItemsStackedLayout);
        Assert.False(group.IsItemsVerticalLayout);

        group.ItemsLayoutMode = RibbonGroupItemsLayoutMode.Docked;
        Assert.True(group.IsItemsDockedLayout);
        Assert.False(group.IsItemsStackedLayout);
    }

    [Fact]
    public void DockedCenterLayoutMode_FlagsFollowSelectedLayout()
    {
        var group = new RibbonGroup();

        group.DockedCenterLayoutMode = RibbonGroupDockedCenterLayoutMode.Horizontal;
        Assert.True(group.IsDockedCenterHorizontalLayout);
        Assert.False(group.IsDockedCenterWrapLayout);

        group.DockedCenterLayoutMode = RibbonGroupDockedCenterLayoutMode.Vertical;
        Assert.True(group.IsDockedCenterVerticalLayout);
        Assert.False(group.IsDockedCenterHorizontalLayout);

        group.DockedCenterLayoutMode = RibbonGroupDockedCenterLayoutMode.Stacked;
        Assert.True(group.IsDockedCenterStackedLayout);
        Assert.False(group.IsDockedCenterVerticalLayout);
    }

    [Fact]
    public void DockedCollections_FollowLayoutDock()
    {
        var group = new RibbonGroup
        {
            ItemsLayoutMode = RibbonGroupItemsLayoutMode.Docked,
        };

        group.Items.Add(new RibbonItem { Id = "left", Label = "Left", LayoutDock = RibbonItemLayoutDock.Left });
        group.Items.Add(new RibbonItem { Id = "center", Label = "Center", LayoutDock = RibbonItemLayoutDock.Center });
        group.Items.Add(new RibbonItem { Id = "top", Label = "Top", LayoutDock = RibbonItemLayoutDock.Top });

        Assert.Single(group.DockedLeftItems);
        Assert.Single(group.DockedCenterItems);
        Assert.Single(group.DockedTopItems);
        Assert.True(group.HasDockedLeftItems);
        Assert.True(group.HasDockedCenterItems);
        Assert.True(group.HasDockedTopItems);
    }

    [Fact]
    public void StackedRows_IsNormalizedToPositiveValue()
    {
        var group = new RibbonGroup();

        group.StackedRows = 0;
        Assert.Equal(1, group.StackedRows);

        group.StackedRows = 4;
        Assert.Equal(4, group.StackedRows);
    }

    [Fact]
    public void CollapsedIconMetadata_FallsBackToFirstVisibleItemWithIcon()
    {
        var group = new RibbonGroup
        {
            Id = "font",
            Header = "Font",
        };

        group.Items.Add(new RibbonItem { Id = "no-icon", Label = "No Icon", IsVisible = true });
        group.Items.Add(new RibbonItem
        {
            Id = "with-icon",
            Label = "With Icon",
            IconResourceKey = "Ribbon.Sample.FontIcon",
            IconEmoji = "🅰️",
            IsVisible = true,
        });

        Assert.True(group.HasCollapsedIcon);
        Assert.Equal("Ribbon.Sample.FontIcon", group.CollapsedIconResourceKey);
        Assert.Equal("🅰️", group.CollapsedIconEmoji);
        Assert.Null(group.CollapsedIconPathData);
    }

    [Fact]
    public void CollapsedIconMetadata_PrefersGroupIconOverItemFallback()
    {
        var group = new RibbonGroup
        {
            Id = "font",
            Header = "Font",
            IconResourceKey = "Ribbon.Sample.GroupIcon",
            IconEmoji = "✂️",
        };

        group.Items.Add(new RibbonItem
        {
            Id = "with-icon",
            Label = "With Icon",
            IconResourceKey = "Ribbon.Sample.ItemIcon",
            IconEmoji = "🖍️",
            IsVisible = true,
        });

        Assert.True(group.HasCollapsedIcon);
        Assert.Equal("Ribbon.Sample.GroupIcon", group.CollapsedIconResourceKey);
        Assert.Equal("✂️", group.CollapsedIconEmoji);
    }
}
