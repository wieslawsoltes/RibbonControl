// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Contracts;

namespace RibbonControl.Core.Models;

public static class RibbonModelConverter
{
    public static RibbonTab ToRibbonTab(IRibbonTabNode node)
    {
        if (node is RibbonTab tab)
        {
            return Clone(tab);
        }

        var converted = new RibbonTab
        {
            Id = node.Id,
            Header = node.Header,
            Order = node.Order,
            IsVisible = node.IsVisible,
            ReplaceTemplate = node.ReplaceTemplate,
            IsContextual = node.IsContextual,
            ContextGroupId = node.ContextGroupId,
            ContextGroupHeader = node.ContextGroupHeader,
            ContextGroupAccentColor = node.ContextGroupAccentColor,
            ContextGroupOrder = node.ContextGroupOrder,
        };

        foreach (var group in node.Groups ?? Enumerable.Empty<IRibbonGroupNode>())
        {
            converted.Groups.Add(ToRibbonGroup(group));
        }

        converted.RebuildMergedGroups();
        return converted;
    }

    public static RibbonGroup ToRibbonGroup(IRibbonGroupNode node)
    {
        if (node is RibbonGroup group)
        {
            return Clone(group);
        }

        var converted = new RibbonGroup
        {
            Id = node.Id,
            Header = node.Header,
            Icon = node.Icon,
            IconResourceKey = node.IconResourceKey,
            IconPathData = node.IconPathData,
            IconEmoji = node.IconEmoji,
            IconStretch = node.IconStretch,
            IconStretchDirection = node.IconStretchDirection,
            IconWidth = node.IconWidth,
            IconHeight = node.IconHeight,
            IconMinWidth = node.IconMinWidth,
            IconMinHeight = node.IconMinHeight,
            IconMaxWidth = node.IconMaxWidth,
            IconMaxHeight = node.IconMaxHeight,
            Overlay = node.Overlay,
            OverlayResourceKey = node.OverlayResourceKey,
            OverlayPathData = node.OverlayPathData,
            OverlayEmoji = node.OverlayEmoji,
            OverlayCount = node.OverlayCount,
            OverlayCountText = node.OverlayCountText,
            ShowOverlayCountWhenZero = node.ShowOverlayCountWhenZero,
            OverlayHorizontalAlignment = node.OverlayHorizontalAlignment,
            OverlayVerticalAlignment = node.OverlayVerticalAlignment,
            OverlayMargin = node.OverlayMargin,
            OverlayCountHorizontalAlignment = node.OverlayCountHorizontalAlignment,
            OverlayCountVerticalAlignment = node.OverlayCountVerticalAlignment,
            OverlayCountMargin = node.OverlayCountMargin,
            HeaderPlacement = node.HeaderPlacement,
            ItemsLayoutMode = node.ItemsLayoutMode,
            DockedCenterLayoutMode = node.DockedCenterLayoutMode,
            StackedRows = node.StackedRows,
            Order = node.Order,
            IsVisible = node.IsVisible,
            ReplaceTemplate = node.ReplaceTemplate,
            CanAutoCollapse = node.CanAutoCollapse,
            CollapsePriority = node.CollapsePriority,
            ExpandedWidthHint = node.ExpandedWidthHint,
            CompactWidthHint = node.CompactWidthHint,
            CollapsedWidthHint = node.CollapsedWidthHint,
        };

        foreach (var item in node.Items ?? Enumerable.Empty<IRibbonItemNode>())
        {
            converted.Items.Add(ToRibbonItem(item));
        }

        converted.RebuildMergedItems();
        return converted;
    }

    public static RibbonItem ToRibbonItem(IRibbonItemNode node)
    {
        if (node is RibbonItem item)
        {
            return Clone(item);
        }

        var converted = new RibbonItem
        {
            Id = node.Id,
            Label = node.Label,
            Icon = node.Icon,
            IconResourceKey = node.IconResourceKey,
            IconPathData = node.IconPathData,
            IconEmoji = node.IconEmoji,
            IconStretch = node.IconStretch,
            IconStretchDirection = node.IconStretchDirection,
            IconWidth = node.IconWidth,
            IconHeight = node.IconHeight,
            IconMinWidth = node.IconMinWidth,
            IconMinHeight = node.IconMinHeight,
            IconMaxWidth = node.IconMaxWidth,
            IconMaxHeight = node.IconMaxHeight,
            Overlay = node.Overlay,
            OverlayResourceKey = node.OverlayResourceKey,
            OverlayPathData = node.OverlayPathData,
            OverlayEmoji = node.OverlayEmoji,
            OverlayCount = node.OverlayCount,
            OverlayCountText = node.OverlayCountText,
            ShowOverlayCountWhenZero = node.ShowOverlayCountWhenZero,
            OverlayHorizontalAlignment = node.OverlayHorizontalAlignment,
            OverlayVerticalAlignment = node.OverlayVerticalAlignment,
            OverlayMargin = node.OverlayMargin,
            OverlayCountHorizontalAlignment = node.OverlayCountHorizontalAlignment,
            OverlayCountVerticalAlignment = node.OverlayCountVerticalAlignment,
            OverlayCountMargin = node.OverlayCountMargin,
            Order = node.Order,
            IsVisible = node.IsVisible,
            ReplaceTemplate = node.ReplaceTemplate,
            CommandId = node.CommandId,
            Command = node.Command,
            CommandParameter = node.CommandParameter,
            Primitive = node.Primitive,
            SplitButtonMode = node.SplitButtonMode,
            DisplayMode = node.DisplayMode,
            Size = node.Size,
            LayoutDock = node.LayoutDock,
            IsToggle = node.IsToggle,
            ToggleGroupSelectionMode = node.ToggleGroupSelectionMode,
            ToggleGroupColumns = node.ToggleGroupColumns,
            IsChecked = node.IsChecked,
            SelectedMenuItemId = node.SelectedMenuItemId,
            Content = node.Content,
            PopupContent = node.PopupContent,
            PopupTitle = node.PopupTitle,
            PopupFooterContent = node.PopupFooterContent,
            PopupMinWidth = node.PopupMinWidth,
            PopupMaxHeight = node.PopupMaxHeight,
            GalleryPreviewMaxItems = node.GalleryPreviewMaxItems,
            GalleryShowCategoryHeaders = node.GalleryShowCategoryHeaders,
            IsDropDownOpen = node.IsDropDownOpen,
            Description = node.Description,
            SecondaryCommandId = node.SecondaryCommandId,
            SecondaryCommand = node.SecondaryCommand,
            SecondaryCommandParameter = node.SecondaryCommandParameter,
            KeyTip = node.KeyTip,
            ScreenTip = node.ScreenTip,
        };

        foreach (var menuItem in node.MenuItems ?? Enumerable.Empty<IRibbonMenuItemNode>())
        {
            converted.MenuItems.Add(ToRibbonMenuItem(menuItem));
        }

        foreach (var subItem in node.Items ?? Enumerable.Empty<IRibbonItemNode>())
        {
            converted.Items.Add(ToRibbonItem(subItem));
        }

        converted.BindStateSyncSource(node);
        return converted;
    }

    public static RibbonTab Clone(RibbonTab source)
    {
        var clone = new RibbonTab
        {
            Id = source.Id,
            Header = source.Header,
            Order = source.Order,
            IsVisible = source.IsVisible,
            ReplaceTemplate = source.ReplaceTemplate,
            IsContextual = source.IsContextual,
            ContextGroupId = source.ContextGroupId,
            ContextGroupHeader = source.ContextGroupHeader,
            ContextGroupAccentColor = source.ContextGroupAccentColor,
            ContextGroupOrder = source.ContextGroupOrder,
            GroupMergeMode = source.GroupMergeMode,
            MergePolicy = source.MergePolicy,
        };

        foreach (var group in source.MergedGroups)
        {
            clone.Groups.Add(Clone(group));
        }

        clone.RebuildMergedGroups();
        return clone;
    }

    public static RibbonGroup Clone(RibbonGroup source)
    {
        var clone = new RibbonGroup
        {
            Id = source.Id,
            Header = source.Header,
            Icon = source.Icon,
            IconResourceKey = source.IconResourceKey,
            IconPathData = source.IconPathData,
            IconEmoji = source.IconEmoji,
            IconStretch = source.IconStretch,
            IconStretchDirection = source.IconStretchDirection,
            IconWidth = source.IconWidth,
            IconHeight = source.IconHeight,
            IconMinWidth = source.IconMinWidth,
            IconMinHeight = source.IconMinHeight,
            IconMaxWidth = source.IconMaxWidth,
            IconMaxHeight = source.IconMaxHeight,
            Overlay = source.Overlay,
            OverlayResourceKey = source.OverlayResourceKey,
            OverlayPathData = source.OverlayPathData,
            OverlayEmoji = source.OverlayEmoji,
            OverlayCount = source.OverlayCount,
            OverlayCountText = source.OverlayCountText,
            ShowOverlayCountWhenZero = source.ShowOverlayCountWhenZero,
            OverlayHorizontalAlignment = source.OverlayHorizontalAlignment,
            OverlayVerticalAlignment = source.OverlayVerticalAlignment,
            OverlayMargin = source.OverlayMargin,
            OverlayCountHorizontalAlignment = source.OverlayCountHorizontalAlignment,
            OverlayCountVerticalAlignment = source.OverlayCountVerticalAlignment,
            OverlayCountMargin = source.OverlayCountMargin,
            HeaderPlacement = source.HeaderPlacement,
            ItemsLayoutMode = source.ItemsLayoutMode,
            DockedCenterLayoutMode = source.DockedCenterLayoutMode,
            StackedRows = source.StackedRows,
            Order = source.Order,
            IsVisible = source.IsVisible,
            ReplaceTemplate = source.ReplaceTemplate,
            CanAutoCollapse = source.CanAutoCollapse,
            CollapsePriority = source.CollapsePriority,
            ExpandedWidthHint = source.ExpandedWidthHint,
            CompactWidthHint = source.CompactWidthHint,
            CollapsedWidthHint = source.CollapsedWidthHint,
            ItemMergeMode = source.ItemMergeMode,
            MergePolicy = source.MergePolicy,
        };

        foreach (var item in source.MergedItems)
        {
            clone.Items.Add(Clone(item));
        }

        clone.RebuildMergedItems();
        return clone;
    }

    public static RibbonItem Clone(RibbonItem source)
    {
        var clone = new RibbonItem
        {
            Id = source.Id,
            Label = source.Label,
            Icon = source.Icon,
            IconResourceKey = source.IconResourceKey,
            IconPathData = source.IconPathData,
            IconEmoji = source.IconEmoji,
            IconStretch = source.IconStretch,
            IconStretchDirection = source.IconStretchDirection,
            IconWidth = source.IconWidth,
            IconHeight = source.IconHeight,
            IconMinWidth = source.IconMinWidth,
            IconMinHeight = source.IconMinHeight,
            IconMaxWidth = source.IconMaxWidth,
            IconMaxHeight = source.IconMaxHeight,
            Overlay = source.Overlay,
            OverlayResourceKey = source.OverlayResourceKey,
            OverlayPathData = source.OverlayPathData,
            OverlayEmoji = source.OverlayEmoji,
            OverlayCount = source.OverlayCount,
            OverlayCountText = source.OverlayCountText,
            ShowOverlayCountWhenZero = source.ShowOverlayCountWhenZero,
            OverlayHorizontalAlignment = source.OverlayHorizontalAlignment,
            OverlayVerticalAlignment = source.OverlayVerticalAlignment,
            OverlayMargin = source.OverlayMargin,
            OverlayCountHorizontalAlignment = source.OverlayCountHorizontalAlignment,
            OverlayCountVerticalAlignment = source.OverlayCountVerticalAlignment,
            OverlayCountMargin = source.OverlayCountMargin,
            Order = source.Order,
            IsVisible = source.IsVisible,
            ReplaceTemplate = source.ReplaceTemplate,
            CommandId = source.CommandId,
            Command = source.Command,
            CommandParameter = source.CommandParameter,
            Primitive = source.Primitive,
            SplitButtonMode = source.SplitButtonMode,
            DisplayMode = source.DisplayMode,
            Size = source.Size,
            LayoutDock = source.LayoutDock,
            IsToggle = source.IsToggle,
            ToggleGroupSelectionMode = source.ToggleGroupSelectionMode,
            ToggleGroupColumns = source.ToggleGroupColumns,
            IsChecked = source.IsChecked,
            SelectedMenuItemId = source.SelectedMenuItemId,
            Content = source.Content,
            PopupContent = source.PopupContent,
            PopupTitle = source.PopupTitle,
            PopupFooterContent = source.PopupFooterContent,
            PopupMinWidth = source.PopupMinWidth,
            PopupMaxHeight = source.PopupMaxHeight,
            GalleryPreviewMaxItems = source.GalleryPreviewMaxItems,
            GalleryShowCategoryHeaders = source.GalleryShowCategoryHeaders,
            IsDropDownOpen = source.IsDropDownOpen,
            Description = source.Description,
            SecondaryCommandId = source.SecondaryCommandId,
            SecondaryCommand = source.SecondaryCommand,
            SecondaryCommandParameter = source.SecondaryCommandParameter,
            KeyTip = source.KeyTip,
            ScreenTip = source.ScreenTip,
        };

        foreach (var menuItem in source.MenuItems)
        {
            clone.MenuItems.Add(Clone(menuItem));
        }

        foreach (var item in source.Items)
        {
            clone.Items.Add(Clone(item));
        }

        if (source.StateSyncSource is { } boundSource)
        {
            clone.BindStateSyncSource(
                boundSource,
                source.IsStateSyncToggleEnabled,
                source.IsStateSyncCheckedEnabled);
        }
        else
        {
            clone.BindStateSyncSource(source);
        }

        return clone;
    }

    public static RibbonMenuItem ToRibbonMenuItem(IRibbonMenuItemNode node)
    {
        if (node is RibbonMenuItem item)
        {
            return Clone(item);
        }

        var converted = new RibbonMenuItem
        {
            Id = node.Id,
            Label = node.Label,
            Icon = node.Icon,
            IconResourceKey = node.IconResourceKey,
            IconPathData = node.IconPathData,
            IconEmoji = node.IconEmoji,
            IconStretch = node.IconStretch,
            IconStretchDirection = node.IconStretchDirection,
            IconWidth = node.IconWidth,
            IconHeight = node.IconHeight,
            IconMinWidth = node.IconMinWidth,
            IconMinHeight = node.IconMinHeight,
            IconMaxWidth = node.IconMaxWidth,
            IconMaxHeight = node.IconMaxHeight,
            Overlay = node.Overlay,
            OverlayResourceKey = node.OverlayResourceKey,
            OverlayPathData = node.OverlayPathData,
            OverlayEmoji = node.OverlayEmoji,
            OverlayCount = node.OverlayCount,
            OverlayCountText = node.OverlayCountText,
            ShowOverlayCountWhenZero = node.ShowOverlayCountWhenZero,
            OverlayHorizontalAlignment = node.OverlayHorizontalAlignment,
            OverlayVerticalAlignment = node.OverlayVerticalAlignment,
            OverlayMargin = node.OverlayMargin,
            OverlayCountHorizontalAlignment = node.OverlayCountHorizontalAlignment,
            OverlayCountVerticalAlignment = node.OverlayCountVerticalAlignment,
            OverlayCountMargin = node.OverlayCountMargin,
            Order = node.Order,
            IsVisible = node.IsVisible,
            ReplaceTemplate = node.ReplaceTemplate,
            CommandId = node.CommandId,
            Command = node.Command,
            CommandParameter = node.CommandParameter,
            Description = node.Description,
            InputGestureText = node.InputGestureText,
            Category = node.Category,
            PopupSectionId = node.PopupSectionId,
            PopupSectionHeader = node.PopupSectionHeader,
            PopupSectionOrder = node.PopupSectionOrder,
            PopupSectionLayout = node.PopupSectionLayout,
            IsSeparator = node.IsSeparator,
            ShowChevron = node.ShowChevron,
            IsSelected = node.IsSelected,
            ShowInRibbonPreview = node.ShowInRibbonPreview,
            ShowInPopup = node.ShowInPopup,
            Content = node.Content,
            KeyTip = node.KeyTip,
            ScreenTip = node.ScreenTip,
        };

        foreach (var subMenuItem in node.SubMenuItems ?? Enumerable.Empty<IRibbonMenuItemNode>())
        {
            converted.SubMenuItems.Add(ToRibbonMenuItem(subMenuItem));
        }

        return converted;
    }

    public static RibbonMenuItem Clone(RibbonMenuItem source)
    {
        var clone = new RibbonMenuItem
        {
            Id = source.Id,
            Label = source.Label,
            Icon = source.Icon,
            IconResourceKey = source.IconResourceKey,
            IconPathData = source.IconPathData,
            IconEmoji = source.IconEmoji,
            IconStretch = source.IconStretch,
            IconStretchDirection = source.IconStretchDirection,
            IconWidth = source.IconWidth,
            IconHeight = source.IconHeight,
            IconMinWidth = source.IconMinWidth,
            IconMinHeight = source.IconMinHeight,
            IconMaxWidth = source.IconMaxWidth,
            IconMaxHeight = source.IconMaxHeight,
            Overlay = source.Overlay,
            OverlayResourceKey = source.OverlayResourceKey,
            OverlayPathData = source.OverlayPathData,
            OverlayEmoji = source.OverlayEmoji,
            OverlayCount = source.OverlayCount,
            OverlayCountText = source.OverlayCountText,
            ShowOverlayCountWhenZero = source.ShowOverlayCountWhenZero,
            OverlayHorizontalAlignment = source.OverlayHorizontalAlignment,
            OverlayVerticalAlignment = source.OverlayVerticalAlignment,
            OverlayMargin = source.OverlayMargin,
            OverlayCountHorizontalAlignment = source.OverlayCountHorizontalAlignment,
            OverlayCountVerticalAlignment = source.OverlayCountVerticalAlignment,
            OverlayCountMargin = source.OverlayCountMargin,
            Order = source.Order,
            IsVisible = source.IsVisible,
            ReplaceTemplate = source.ReplaceTemplate,
            CommandId = source.CommandId,
            Command = source.Command,
            CommandParameter = source.CommandParameter,
            Description = source.Description,
            InputGestureText = source.InputGestureText,
            Category = source.Category,
            PopupSectionId = source.PopupSectionId,
            PopupSectionHeader = source.PopupSectionHeader,
            PopupSectionOrder = source.PopupSectionOrder,
            PopupSectionLayout = source.PopupSectionLayout,
            IsSeparator = source.IsSeparator,
            ShowChevron = source.ShowChevron,
            IsSelected = source.IsSelected,
            ShowInRibbonPreview = source.ShowInRibbonPreview,
            ShowInPopup = source.ShowInPopup,
            Content = source.Content,
            KeyTip = source.KeyTip,
            ScreenTip = source.ScreenTip,
        };

        foreach (var subMenuItem in source.SubMenuItems)
        {
            clone.SubMenuItems.Add(Clone(subMenuItem));
        }

        return clone;
    }

    public static RibbonBackstageItem ToRibbonBackstageItem(IRibbonBackstageItemNode node)
    {
        if (node is RibbonBackstageItem item)
        {
            return Clone(item);
        }

        var converted = new RibbonBackstageItem
        {
            Id = node.Id,
            Label = node.Label,
            Icon = node.Icon,
            IconResourceKey = node.IconResourceKey,
            IconPathData = node.IconPathData,
            IconEmoji = node.IconEmoji,
            IconStretch = node.IconStretch,
            IconStretchDirection = node.IconStretchDirection,
            IconWidth = node.IconWidth,
            IconHeight = node.IconHeight,
            IconMinWidth = node.IconMinWidth,
            IconMinHeight = node.IconMinHeight,
            IconMaxWidth = node.IconMaxWidth,
            IconMaxHeight = node.IconMaxHeight,
            Order = node.Order,
            IsVisible = node.IsVisible,
            ReplaceTemplate = node.ReplaceTemplate,
            IsEnabled = node.IsEnabled,
            IsSeparator = node.IsSeparator,
            ShowChevron = node.ShowChevron,
            ExecuteCommandOnSelect = node.ExecuteCommandOnSelect,
            CloseBackstageOnExecute = node.CloseBackstageOnExecute,
            CommandId = node.CommandId,
            Command = node.Command,
            CommandParameter = node.CommandParameter,
            Content = node.Content,
            Description = node.Description,
            InputGestureText = node.InputGestureText,
            KeyTip = node.KeyTip,
            ScreenTip = node.ScreenTip,
        };

        foreach (var subItem in node.SubItems ?? Enumerable.Empty<IRibbonBackstageItemNode>())
        {
            converted.SubItems.Add(ToRibbonBackstageItem(subItem));
        }

        return converted;
    }

    public static RibbonBackstageItem Clone(RibbonBackstageItem source)
    {
        var clone = new RibbonBackstageItem
        {
            Id = source.Id,
            Label = source.Label,
            Icon = source.Icon,
            IconResourceKey = source.IconResourceKey,
            IconPathData = source.IconPathData,
            IconEmoji = source.IconEmoji,
            IconStretch = source.IconStretch,
            IconStretchDirection = source.IconStretchDirection,
            IconWidth = source.IconWidth,
            IconHeight = source.IconHeight,
            IconMinWidth = source.IconMinWidth,
            IconMinHeight = source.IconMinHeight,
            IconMaxWidth = source.IconMaxWidth,
            IconMaxHeight = source.IconMaxHeight,
            Order = source.Order,
            IsVisible = source.IsVisible,
            ReplaceTemplate = source.ReplaceTemplate,
            IsEnabled = source.IsEnabled,
            IsSeparator = source.IsSeparator,
            ShowChevron = source.ShowChevron,
            ExecuteCommandOnSelect = source.ExecuteCommandOnSelect,
            CloseBackstageOnExecute = source.CloseBackstageOnExecute,
            CommandId = source.CommandId,
            Command = source.Command,
            CommandParameter = source.CommandParameter,
            Content = source.Content,
            Description = source.Description,
            InputGestureText = source.InputGestureText,
            KeyTip = source.KeyTip,
            ScreenTip = source.ScreenTip,
            IsSubMenuOpen = source.IsSubMenuOpen,
        };

        foreach (var subItem in source.SubItems)
        {
            clone.SubItems.Add(Clone(subItem));
        }

        return clone;
    }
}
