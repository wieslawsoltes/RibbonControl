// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.Services;

public sealed class RibbonMergePolicy : IRibbonMergePolicy
{
    private static readonly StringComparer IdComparer = StringComparer.Ordinal;

    public static RibbonMergePolicy StaticThenDynamic { get; } = new();

    private RibbonMergePolicy()
    {
    }

    public IReadOnlyList<RibbonTab> MergeTabs(
        IEnumerable<RibbonTab> staticTabs,
        IEnumerable<IRibbonTabNode>? dynamicTabs,
        RibbonMergeMode mode)
    {
        var staticList = staticTabs
            .Select(RibbonModelConverter.Clone)
            .Where(t => t.IsVisible)
            .OrderBy(t => t.Order)
            .ThenBy(t => t.Id, IdComparer)
            .ToList();

        var dynamicList = dynamicTabs
            ?.Select(RibbonModelConverter.ToRibbonTab)
            .Where(t => t.IsVisible)
            .OrderBy(t => t.Order)
            .ThenBy(t => t.Id, IdComparer)
            .ToList()
            ?? [];

        return mode switch
        {
            RibbonMergeMode.StaticOnly => staticList,
            RibbonMergeMode.DynamicOnly => dynamicList,
            _ => MergeTabs(staticList, dynamicList),
        };
    }

    public IReadOnlyList<RibbonGroup> MergeGroups(
        IEnumerable<RibbonGroup> staticGroups,
        IEnumerable<IRibbonGroupNode>? dynamicGroups,
        RibbonMergeMode mode)
    {
        var staticList = staticGroups
            .Select(RibbonModelConverter.Clone)
            .Where(g => g.IsVisible)
            .OrderBy(g => g.Order)
            .ThenBy(g => g.Id, IdComparer)
            .ToList();

        var dynamicList = dynamicGroups
            ?.Select(RibbonModelConverter.ToRibbonGroup)
            .Where(g => g.IsVisible)
            .OrderBy(g => g.Order)
            .ThenBy(g => g.Id, IdComparer)
            .ToList()
            ?? [];

        return mode switch
        {
            RibbonMergeMode.StaticOnly => staticList,
            RibbonMergeMode.DynamicOnly => dynamicList,
            _ => MergeGroups(staticList, dynamicList),
        };
    }

    public IReadOnlyList<RibbonItem> MergeItems(
        IEnumerable<RibbonItem> staticItems,
        IEnumerable<IRibbonItemNode>? dynamicItems,
        RibbonMergeMode mode)
    {
        var staticList = staticItems
            .Select(RibbonModelConverter.Clone)
            .Where(i => i.IsVisible)
            .OrderBy(i => i.Order)
            .ThenBy(i => i.Id, IdComparer)
            .ToList();

        var dynamicList = dynamicItems
            ?.Select(RibbonModelConverter.ToRibbonItem)
            .Where(i => i.IsVisible)
            .OrderBy(i => i.Order)
            .ThenBy(i => i.Id, IdComparer)
            .ToList()
            ?? [];

        return mode switch
        {
            RibbonMergeMode.StaticOnly => staticList,
            RibbonMergeMode.DynamicOnly => dynamicList,
            _ => MergeItems(staticList, dynamicList),
        };
    }

    private IReadOnlyList<RibbonTab> MergeTabs(List<RibbonTab> staticTabs, List<RibbonTab> dynamicTabs)
    {
        var result = staticTabs;
        var indexById = result
            .Select((tab, index) => (tab.Id, index))
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Id))
            .ToDictionary(pair => pair.Id, pair => pair.index, IdComparer);

        foreach (var dynamicTab in dynamicTabs)
        {
            if (!string.IsNullOrWhiteSpace(dynamicTab.Id) && indexById.TryGetValue(dynamicTab.Id, out var index))
            {
                result[index] = MergeTab(result[index], dynamicTab);
                continue;
            }

            indexById[dynamicTab.Id] = result.Count;
            result.Add(dynamicTab);
        }

        return result;
    }

    private IReadOnlyList<RibbonGroup> MergeGroups(List<RibbonGroup> staticGroups, List<RibbonGroup> dynamicGroups)
    {
        var result = staticGroups;
        var indexById = result
            .Select((group, index) => (group.Id, index))
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Id))
            .ToDictionary(pair => pair.Id, pair => pair.index, IdComparer);

        foreach (var dynamicGroup in dynamicGroups)
        {
            if (!string.IsNullOrWhiteSpace(dynamicGroup.Id) && indexById.TryGetValue(dynamicGroup.Id, out var index))
            {
                result[index] = MergeGroup(result[index], dynamicGroup);
                continue;
            }

            indexById[dynamicGroup.Id] = result.Count;
            result.Add(dynamicGroup);
        }

        return result;
    }

    private IReadOnlyList<RibbonItem> MergeItems(List<RibbonItem> staticItems, List<RibbonItem> dynamicItems)
    {
        var result = staticItems;
        var indexById = result
            .Select((item, index) => (item.Id, index))
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Id))
            .ToDictionary(pair => pair.Id, pair => pair.index, IdComparer);

        foreach (var dynamicItem in dynamicItems)
        {
            if (!string.IsNullOrWhiteSpace(dynamicItem.Id) && indexById.TryGetValue(dynamicItem.Id, out var index))
            {
                result[index] = MergeItem(result[index], dynamicItem);
                continue;
            }

            indexById[dynamicItem.Id] = result.Count;
            result.Add(dynamicItem);
        }

        return result;
    }

    private RibbonTab MergeTab(RibbonTab staticTab, RibbonTab dynamicTab)
    {
        var merged = RibbonModelConverter.Clone(staticTab);
        merged.Header = dynamicTab.Header;
        merged.Order = dynamicTab.Order;
        merged.IsVisible = dynamicTab.IsVisible;
        merged.IsContextual = dynamicTab.IsContextual || staticTab.IsContextual;
        merged.ContextGroupId = !string.IsNullOrWhiteSpace(dynamicTab.ContextGroupId)
            ? dynamicTab.ContextGroupId
            : staticTab.ContextGroupId;
        merged.ContextGroupHeader = dynamicTab.ContextGroupHeader ?? staticTab.ContextGroupHeader;
        merged.ContextGroupAccentColor = dynamicTab.ContextGroupAccentColor ?? staticTab.ContextGroupAccentColor;
        merged.ContextGroupOrder = dynamicTab.ContextGroupOrder ?? staticTab.ContextGroupOrder;
        merged.ReplaceTemplate = dynamicTab.ReplaceTemplate;
        merged.GroupMergeMode = RibbonMergeMode.Merge;
        merged.GroupsSource = dynamicTab.MergedGroups;
        merged.RebuildMergedGroups();
        return merged;
    }

    private RibbonGroup MergeGroup(RibbonGroup staticGroup, RibbonGroup dynamicGroup)
    {
        var merged = RibbonModelConverter.Clone(staticGroup);
        merged.Header = dynamicGroup.Header;
        merged.Icon = dynamicGroup.Icon ?? staticGroup.Icon;
        merged.IconResourceKey = dynamicGroup.IconResourceKey ?? staticGroup.IconResourceKey;
        merged.IconPathData = dynamicGroup.IconPathData ?? staticGroup.IconPathData;
        merged.IconEmoji = dynamicGroup.IconEmoji ?? staticGroup.IconEmoji;
        merged.IconStretch = dynamicGroup.IconStretch;
        merged.IconStretchDirection = dynamicGroup.IconStretchDirection;
        merged.IconWidth = MergeLength(dynamicGroup.IconWidth, staticGroup.IconWidth);
        merged.IconHeight = MergeLength(dynamicGroup.IconHeight, staticGroup.IconHeight);
        merged.IconMinWidth = MergeMinLength(dynamicGroup.IconMinWidth, staticGroup.IconMinWidth);
        merged.IconMinHeight = MergeMinLength(dynamicGroup.IconMinHeight, staticGroup.IconMinHeight);
        merged.IconMaxWidth = MergeMaxLength(dynamicGroup.IconMaxWidth, staticGroup.IconMaxWidth);
        merged.IconMaxHeight = MergeMaxLength(dynamicGroup.IconMaxHeight, staticGroup.IconMaxHeight);
        merged.Overlay = dynamicGroup.Overlay ?? staticGroup.Overlay;
        merged.OverlayResourceKey = dynamicGroup.OverlayResourceKey ?? staticGroup.OverlayResourceKey;
        merged.OverlayPathData = dynamicGroup.OverlayPathData ?? staticGroup.OverlayPathData;
        merged.OverlayEmoji = dynamicGroup.OverlayEmoji ?? staticGroup.OverlayEmoji;
        merged.OverlayCount = dynamicGroup.OverlayCount ?? staticGroup.OverlayCount;
        merged.OverlayCountText = dynamicGroup.OverlayCountText ?? staticGroup.OverlayCountText;
        merged.ShowOverlayCountWhenZero = dynamicGroup.ShowOverlayCountWhenZero || staticGroup.ShowOverlayCountWhenZero;
        merged.OverlayHorizontalAlignment = dynamicGroup.OverlayHorizontalAlignment;
        merged.OverlayVerticalAlignment = dynamicGroup.OverlayVerticalAlignment;
        merged.OverlayMargin = dynamicGroup.OverlayMargin;
        merged.OverlayCountHorizontalAlignment = dynamicGroup.OverlayCountHorizontalAlignment;
        merged.OverlayCountVerticalAlignment = dynamicGroup.OverlayCountVerticalAlignment;
        merged.OverlayCountMargin = dynamicGroup.OverlayCountMargin;
        merged.HeaderPlacement = dynamicGroup.HeaderPlacement;
        merged.ItemsLayoutMode = dynamicGroup.ItemsLayoutMode != RibbonGroupItemsLayoutMode.Auto
            ? dynamicGroup.ItemsLayoutMode
            : staticGroup.ItemsLayoutMode;
        merged.DockedCenterLayoutMode = dynamicGroup.DockedCenterLayoutMode != RibbonGroupDockedCenterLayoutMode.Auto
            ? dynamicGroup.DockedCenterLayoutMode
            : staticGroup.DockedCenterLayoutMode;
        var dynamicUsesStackedRows = dynamicGroup.ItemsLayoutMode == RibbonGroupItemsLayoutMode.Stacked
            || (dynamicGroup.ItemsLayoutMode == RibbonGroupItemsLayoutMode.Docked &&
                (dynamicGroup.DockedCenterLayoutMode == RibbonGroupDockedCenterLayoutMode.Stacked ||
                 (dynamicGroup.DockedCenterLayoutMode == RibbonGroupDockedCenterLayoutMode.Auto &&
                  staticGroup.DockedCenterLayoutMode == RibbonGroupDockedCenterLayoutMode.Stacked)));
        merged.StackedRows = dynamicUsesStackedRows && dynamicGroup.StackedRows > 0
            ? dynamicGroup.StackedRows
            : staticGroup.StackedRows;
        merged.Order = dynamicGroup.Order;
        merged.IsVisible = dynamicGroup.IsVisible;
        merged.ReplaceTemplate = dynamicGroup.ReplaceTemplate;
        merged.CanAutoCollapse = dynamicGroup.CanAutoCollapse;
        merged.CollapsePriority = dynamicGroup.CollapsePriority;
        merged.ExpandedWidthHint = dynamicGroup.ExpandedWidthHint > 0
            ? dynamicGroup.ExpandedWidthHint
            : staticGroup.ExpandedWidthHint;
        merged.CompactWidthHint = dynamicGroup.CompactWidthHint > 0
            ? dynamicGroup.CompactWidthHint
            : staticGroup.CompactWidthHint;
        merged.CollapsedWidthHint = dynamicGroup.CollapsedWidthHint > 0
            ? dynamicGroup.CollapsedWidthHint
            : staticGroup.CollapsedWidthHint;
        merged.ItemMergeMode = RibbonMergeMode.Merge;
        merged.ItemsSource = dynamicGroup.MergedItems;
        merged.RebuildMergedItems();
        return merged;
    }

    private static RibbonItem MergeItem(RibbonItem staticItem, RibbonItem dynamicItem)
    {
        var merged = RibbonModelConverter.Clone(staticItem);
        merged.Label = dynamicItem.Label;
        merged.Icon = dynamicItem.Icon ?? staticItem.Icon;
        merged.IconResourceKey = dynamicItem.IconResourceKey ?? staticItem.IconResourceKey;
        merged.IconPathData = dynamicItem.IconPathData ?? staticItem.IconPathData;
        merged.IconEmoji = dynamicItem.IconEmoji ?? staticItem.IconEmoji;
        merged.IconStretch = dynamicItem.IconStretch;
        merged.IconStretchDirection = dynamicItem.IconStretchDirection;
        merged.IconWidth = MergeLength(dynamicItem.IconWidth, staticItem.IconWidth);
        merged.IconHeight = MergeLength(dynamicItem.IconHeight, staticItem.IconHeight);
        merged.IconMinWidth = MergeMinLength(dynamicItem.IconMinWidth, staticItem.IconMinWidth);
        merged.IconMinHeight = MergeMinLength(dynamicItem.IconMinHeight, staticItem.IconMinHeight);
        merged.IconMaxWidth = MergeMaxLength(dynamicItem.IconMaxWidth, staticItem.IconMaxWidth);
        merged.IconMaxHeight = MergeMaxLength(dynamicItem.IconMaxHeight, staticItem.IconMaxHeight);
        merged.Overlay = dynamicItem.Overlay ?? staticItem.Overlay;
        merged.OverlayResourceKey = dynamicItem.OverlayResourceKey ?? staticItem.OverlayResourceKey;
        merged.OverlayPathData = dynamicItem.OverlayPathData ?? staticItem.OverlayPathData;
        merged.OverlayEmoji = dynamicItem.OverlayEmoji ?? staticItem.OverlayEmoji;
        merged.OverlayCount = dynamicItem.OverlayCount ?? staticItem.OverlayCount;
        merged.OverlayCountText = dynamicItem.OverlayCountText ?? staticItem.OverlayCountText;
        merged.ShowOverlayCountWhenZero = dynamicItem.ShowOverlayCountWhenZero || staticItem.ShowOverlayCountWhenZero;
        merged.OverlayHorizontalAlignment = dynamicItem.OverlayHorizontalAlignment;
        merged.OverlayVerticalAlignment = dynamicItem.OverlayVerticalAlignment;
        merged.OverlayMargin = dynamicItem.OverlayMargin;
        merged.OverlayCountHorizontalAlignment = dynamicItem.OverlayCountHorizontalAlignment;
        merged.OverlayCountVerticalAlignment = dynamicItem.OverlayCountVerticalAlignment;
        merged.OverlayCountMargin = dynamicItem.OverlayCountMargin;
        merged.Order = dynamicItem.Order;
        merged.IsVisible = dynamicItem.IsVisible;
        merged.CommandId = dynamicItem.CommandId;
        merged.Command = dynamicItem.Command;
        merged.CommandParameter = dynamicItem.CommandParameter;
        merged.Primitive = dynamicItem.Primitive;
        merged.SplitButtonMode = dynamicItem.SplitButtonMode;
        merged.DisplayMode = dynamicItem.DisplayMode != RibbonItemDisplayMode.Auto
            ? dynamicItem.DisplayMode
            : staticItem.DisplayMode;
        merged.Size = dynamicItem.Size;
        merged.LayoutDock = dynamicItem.LayoutDock != RibbonItemLayoutDock.Auto
            ? dynamicItem.LayoutDock
            : staticItem.LayoutDock;
        merged.IsToggle = dynamicItem.IsToggle || staticItem.IsToggle;
        merged.ToggleGroupSelectionMode = dynamicItem.ToggleGroupSelectionMode;
        merged.ToggleGroupColumns = dynamicItem.ToggleGroupColumns > 0
            ? dynamicItem.ToggleGroupColumns
            : staticItem.ToggleGroupColumns;
        merged.IsChecked = dynamicItem.IsChecked;
        merged.SelectedMenuItemId = dynamicItem.SelectedMenuItemId ?? staticItem.SelectedMenuItemId;
        merged.Content = dynamicItem.Content ?? staticItem.Content;
        merged.PopupContent = dynamicItem.PopupContent ?? staticItem.PopupContent;
        merged.PopupTitle = dynamicItem.PopupTitle ?? staticItem.PopupTitle;
        merged.PopupFooterContent = dynamicItem.PopupFooterContent ?? staticItem.PopupFooterContent;
        merged.PopupMinWidth = dynamicItem.PopupMinWidth > 0
            ? dynamicItem.PopupMinWidth
            : staticItem.PopupMinWidth;
        merged.PopupMaxHeight = dynamicItem.PopupMaxHeight > 0
            ? dynamicItem.PopupMaxHeight
            : staticItem.PopupMaxHeight;
        merged.GalleryPreviewMaxItems = dynamicItem.GalleryPreviewMaxItems;
        merged.GalleryShowCategoryHeaders = dynamicItem.GalleryShowCategoryHeaders;
        merged.IsDropDownOpen = dynamicItem.IsDropDownOpen;
        merged.Description = dynamicItem.Description ?? staticItem.Description;
        merged.SecondaryCommandId = dynamicItem.SecondaryCommandId;
        merged.SecondaryCommand = dynamicItem.SecondaryCommand;
        merged.SecondaryCommandParameter = dynamicItem.SecondaryCommandParameter;
        merged.KeyTip = dynamicItem.KeyTip;
        merged.ScreenTip = dynamicItem.ScreenTip;
        merged.MenuItems.Clear();

        foreach (var menuItem in MergeMenuItems(staticItem.MenuItems, dynamicItem.MenuItems))
        {
            merged.MenuItems.Add(menuItem);
        }

        if (dynamicItem.ReplaceTemplate)
        {
            merged.ReplaceTemplate = true;
        }

        if (dynamicItem.StateSyncSource is { } dynamicStateSource)
        {
            var synchronizeToggle = dynamicItem.IsToggle || !staticItem.IsToggle;
            merged.BindStateSyncSource(dynamicStateSource, synchronizeToggle, synchronizeChecked: true);
        }
        else if (staticItem.StateSyncSource is { } staticStateSource)
        {
            merged.BindStateSyncSource(staticStateSource);
        }

        return merged;
    }

    private static IReadOnlyList<RibbonMenuItem> MergeMenuItems(
        IEnumerable<RibbonMenuItem> staticItems,
        IEnumerable<RibbonMenuItem> dynamicItems)
    {
        var result = staticItems
            .Select(RibbonModelConverter.Clone)
            .Where(i => i.IsVisible)
            .OrderBy(i => i.Order)
            .ThenBy(i => i.Id, IdComparer)
            .ToList();

        var dynamicList = dynamicItems
            .Select(RibbonModelConverter.Clone)
            .Where(i => i.IsVisible)
            .OrderBy(i => i.Order)
            .ThenBy(i => i.Id, IdComparer)
            .ToList();

        var indexById = result
            .Select((item, index) => (item.Id, index))
            .Where(pair => !string.IsNullOrWhiteSpace(pair.Id))
            .ToDictionary(pair => pair.Id, pair => pair.index, IdComparer);

        foreach (var dynamicItem in dynamicList)
        {
            if (!string.IsNullOrWhiteSpace(dynamicItem.Id) &&
                indexById.TryGetValue(dynamicItem.Id, out var index))
            {
                result[index] = MergeMenuItem(result[index], dynamicItem);
                continue;
            }

            indexById[dynamicItem.Id] = result.Count;
            result.Add(dynamicItem);
        }

        return result;
    }

    private static RibbonMenuItem MergeMenuItem(RibbonMenuItem staticItem, RibbonMenuItem dynamicItem)
    {
        var merged = RibbonModelConverter.Clone(staticItem);
        merged.Label = dynamicItem.Label;
        merged.Icon = dynamicItem.Icon ?? staticItem.Icon;
        merged.IconResourceKey = dynamicItem.IconResourceKey ?? staticItem.IconResourceKey;
        merged.IconPathData = dynamicItem.IconPathData ?? staticItem.IconPathData;
        merged.IconEmoji = dynamicItem.IconEmoji ?? staticItem.IconEmoji;
        merged.IconStretch = dynamicItem.IconStretch;
        merged.IconStretchDirection = dynamicItem.IconStretchDirection;
        merged.IconWidth = MergeLength(dynamicItem.IconWidth, staticItem.IconWidth);
        merged.IconHeight = MergeLength(dynamicItem.IconHeight, staticItem.IconHeight);
        merged.IconMinWidth = MergeMinLength(dynamicItem.IconMinWidth, staticItem.IconMinWidth);
        merged.IconMinHeight = MergeMinLength(dynamicItem.IconMinHeight, staticItem.IconMinHeight);
        merged.IconMaxWidth = MergeMaxLength(dynamicItem.IconMaxWidth, staticItem.IconMaxWidth);
        merged.IconMaxHeight = MergeMaxLength(dynamicItem.IconMaxHeight, staticItem.IconMaxHeight);
        merged.Overlay = dynamicItem.Overlay ?? staticItem.Overlay;
        merged.OverlayResourceKey = dynamicItem.OverlayResourceKey ?? staticItem.OverlayResourceKey;
        merged.OverlayPathData = dynamicItem.OverlayPathData ?? staticItem.OverlayPathData;
        merged.OverlayEmoji = dynamicItem.OverlayEmoji ?? staticItem.OverlayEmoji;
        merged.OverlayCount = dynamicItem.OverlayCount ?? staticItem.OverlayCount;
        merged.OverlayCountText = dynamicItem.OverlayCountText ?? staticItem.OverlayCountText;
        merged.ShowOverlayCountWhenZero = dynamicItem.ShowOverlayCountWhenZero || staticItem.ShowOverlayCountWhenZero;
        merged.OverlayHorizontalAlignment = dynamicItem.OverlayHorizontalAlignment;
        merged.OverlayVerticalAlignment = dynamicItem.OverlayVerticalAlignment;
        merged.OverlayMargin = dynamicItem.OverlayMargin;
        merged.OverlayCountHorizontalAlignment = dynamicItem.OverlayCountHorizontalAlignment;
        merged.OverlayCountVerticalAlignment = dynamicItem.OverlayCountVerticalAlignment;
        merged.OverlayCountMargin = dynamicItem.OverlayCountMargin;
        merged.Order = dynamicItem.Order;
        merged.IsVisible = dynamicItem.IsVisible;
        merged.CommandId = dynamicItem.CommandId;
        merged.Command = dynamicItem.Command;
        merged.CommandParameter = dynamicItem.CommandParameter;
        merged.Description = dynamicItem.Description ?? staticItem.Description;
        merged.InputGestureText = dynamicItem.InputGestureText ?? staticItem.InputGestureText;
        merged.Category = dynamicItem.Category ?? staticItem.Category;
        merged.IsSeparator = dynamicItem.IsSeparator;
        merged.ShowChevron = dynamicItem.ShowChevron;
        merged.IsSelected = dynamicItem.IsSelected || staticItem.IsSelected;
        merged.ShowInRibbonPreview = dynamicItem.ShowInRibbonPreview;
        merged.ShowInPopup = dynamicItem.ShowInPopup;
        merged.Content = dynamicItem.Content ?? staticItem.Content;
        merged.KeyTip = dynamicItem.KeyTip;
        merged.ScreenTip = dynamicItem.ScreenTip;
        merged.SubMenuItems.Clear();

        foreach (var subMenuItem in MergeMenuItems(staticItem.SubMenuItems, dynamicItem.SubMenuItems))
        {
            merged.SubMenuItems.Add(subMenuItem);
        }

        if (dynamicItem.ReplaceTemplate)
        {
            merged.ReplaceTemplate = true;
        }

        return merged;
    }

    private static double MergeLength(double dynamicValue, double staticValue)
    {
        return !double.IsNaN(dynamicValue)
            ? dynamicValue
            : staticValue;
    }

    private static double MergeMinLength(double dynamicValue, double staticValue)
    {
        return dynamicValue > 0
            ? dynamicValue
            : staticValue;
    }

    private static double MergeMaxLength(double dynamicValue, double staticValue)
    {
        return !double.IsNaN(dynamicValue) && !double.IsPositiveInfinity(dynamicValue)
            ? dynamicValue
            : staticValue;
    }
}
