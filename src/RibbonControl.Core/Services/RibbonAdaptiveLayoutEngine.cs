// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.Services;

public sealed class RibbonAdaptiveLayoutEngine : IRibbonAdaptiveLayoutEngine
{
    private const double InlineGroupChromeWidth = 24;
    private const double InterGroupSpacing = 6;
    private const double FallbackCollapsedWidth = 72;
    private const double FallbackLargeButtonWidth = 86;
    private const double FallbackSmallButtonWidth = 58;
    private const double FallbackLargeIconOnlyButtonWidth = 42;
    private const double FallbackSmallIconOnlyButtonWidth = 32;
    private const double FallbackLargeToggleButtonWidth = 64;
    private const double FallbackSmallToggleButtonWidth = 42;
    private const double FallbackLargeIconOnlyToggleButtonWidth = 42;
    private const double FallbackSmallIconOnlyToggleButtonWidth = 32;
    private const double FallbackLargeComboBoxWidth = 176;
    private const double FallbackSmallComboBoxWidth = 84;
    private const double FallbackLargeSplitButtonWidth = 92;
    private const double FallbackSmallSplitButtonWidth = 66;
    private const double FallbackLargeIconOnlySplitButtonWidth = 62;
    private const double FallbackSmallIconOnlySplitButtonWidth = 54;
    private const double FallbackMenuButtonWidth = 90;
    private const double FallbackGalleryWidth = 176;
    private const double FallbackLargeToggleGroupOptionWidth = 114;
    private const double FallbackSmallToggleGroupOptionWidth = 96;
    private const double FallbackCustomWidth = 168;

    public bool ApplyLayout(IReadOnlyList<RibbonGroup> groups, double availableWidth)
    {
        ArgumentNullException.ThrowIfNull(groups);

        var visibleGroups = groups
            .Where(group => group.IsVisible)
            .OrderBy(group => group.Order)
            .ThenBy(group => group.Id, StringComparer.Ordinal)
            .ToList();

        if (visibleGroups.Count == 0)
        {
            return false;
        }

        var changed = false;
        foreach (var group in visibleGroups)
        {
            if (group.DisplayMode != RibbonGroupDisplayMode.Expanded)
            {
                group.DisplayMode = RibbonGroupDisplayMode.Expanded;
                changed = true;
            }
        }

        var clampedAvailableWidth = Math.Max(0, availableWidth);
        if (CalculateTotalWidth(visibleGroups) <= clampedAvailableWidth)
        {
            return changed;
        }

        var collapseCandidates = visibleGroups
            .Where(group => group.CanAutoCollapse)
            .OrderByDescending(group => group.CollapsePriority)
            .ThenByDescending(group => group.Order)
            .ThenByDescending(group => group.Id, StringComparer.Ordinal)
            .ToList();

        foreach (var group in collapseCandidates)
        {
            if (!SupportsCompactMode(group))
            {
                continue;
            }

            if (group.DisplayMode != RibbonGroupDisplayMode.Compact)
            {
                group.DisplayMode = RibbonGroupDisplayMode.Compact;
                changed = true;
            }

            if (CalculateTotalWidth(visibleGroups) <= clampedAvailableWidth)
            {
                return changed;
            }
        }

        foreach (var group in collapseCandidates)
        {
            if (group.DisplayMode != RibbonGroupDisplayMode.Collapsed)
            {
                group.DisplayMode = RibbonGroupDisplayMode.Collapsed;
                changed = true;
            }

            if (CalculateTotalWidth(visibleGroups) <= clampedAvailableWidth)
            {
                return changed;
            }
        }

        return changed;
    }

    private static bool SupportsCompactMode(RibbonGroup group)
    {
        return group.MergedItems.Any(item =>
            item.IsVisible &&
            item.Size == RibbonItemSize.Large &&
            SupportsCompactSize(item.Primitive));
    }

    private static double CalculateTotalWidth(IReadOnlyList<RibbonGroup> groups)
    {
        if (groups.Count == 0)
        {
            return 0;
        }

        var width = 0d;
        for (var index = 0; index < groups.Count; index++)
        {
            width += EstimateGroupWidth(groups[index], groups[index].DisplayMode);
        }

        width += (groups.Count - 1) * InterGroupSpacing;
        return width;
    }

    private static double EstimateGroupWidth(RibbonGroup group, RibbonGroupDisplayMode mode)
    {
        return mode switch
        {
            RibbonGroupDisplayMode.Compact when group.CompactWidthHint > 0 => group.CompactWidthHint,
            RibbonGroupDisplayMode.Collapsed when group.CollapsedWidthHint > 0 => group.CollapsedWidthHint,
            RibbonGroupDisplayMode.Expanded when group.ExpandedWidthHint > 0 => group.ExpandedWidthHint,
            RibbonGroupDisplayMode.Collapsed => FallbackCollapsedWidth,
            RibbonGroupDisplayMode.Compact => EstimateInlineGroupWidth(group, compact: true),
            _ => EstimateInlineGroupWidth(group, compact: false),
        };
    }

    private static double EstimateInlineGroupWidth(RibbonGroup group, bool compact)
    {
        var visibleItems = group.MergedItems.Where(item => item.IsVisible).ToList();
        if (visibleItems.Count == 0)
        {
            return InlineGroupChromeWidth;
        }

        var effectiveLayout = group.EffectiveItemsLayoutMode;
        return effectiveLayout switch
        {
            RibbonGroupItemsLayoutMode.Horizontal or RibbonGroupItemsLayoutMode.Wrap
                => EstimateHorizontalGroupWidth(visibleItems, compact),
            RibbonGroupItemsLayoutMode.Vertical
                => EstimateVerticalGroupWidth(visibleItems, compact),
            RibbonGroupItemsLayoutMode.Stacked
                => EstimateStackedGroupWidth(visibleItems, compact, group.StackedRows),
            RibbonGroupItemsLayoutMode.Docked
                => EstimateDockedGroupWidth(group, visibleItems, compact),
            _ => EstimateHorizontalGroupWidth(visibleItems, compact),
        };
    }

    private static double EstimateHorizontalGroupWidth(IReadOnlyList<RibbonItem> visibleItems, bool compact)
    {
        return InlineGroupChromeWidth + EstimateHorizontalItemsWidth(visibleItems, compact);
    }

    private static double EstimateVerticalGroupWidth(IReadOnlyList<RibbonItem> visibleItems, bool compact)
    {
        return InlineGroupChromeWidth + EstimateVerticalItemsWidth(visibleItems, compact);
    }

    private static double EstimateStackedGroupWidth(IReadOnlyList<RibbonItem> visibleItems, bool compact, int stackedRows)
    {
        return InlineGroupChromeWidth + EstimateStackedItemsWidth(visibleItems, compact, stackedRows);
    }

    private static double EstimateDockedGroupWidth(
        RibbonGroup group,
        IReadOnlyList<RibbonItem> visibleItems,
        bool compact)
    {
        var topItems = FilterDockedItems(visibleItems, RibbonItemLayoutDock.Top);
        var bottomItems = FilterDockedItems(visibleItems, RibbonItemLayoutDock.Bottom);
        var leftItems = FilterDockedItems(visibleItems, RibbonItemLayoutDock.Left);
        var rightItems = FilterDockedItems(visibleItems, RibbonItemLayoutDock.Right);
        var centerItems = FilterDockedItems(visibleItems, RibbonItemLayoutDock.Center, RibbonItemLayoutDock.Auto);

        var topWidth = EstimateHorizontalItemsWidth(topItems, compact);
        var bottomWidth = EstimateHorizontalItemsWidth(bottomItems, compact);
        var leftWidth = EstimateVerticalItemsWidth(leftItems, compact);
        var rightWidth = EstimateVerticalItemsWidth(rightItems, compact);
        var centerWidth = EstimateDockedCenterItemsWidth(
            centerItems,
            compact,
            group.EffectiveDockedCenterLayoutMode,
            group.StackedRows);

        var middleWidth = leftWidth + rightWidth + centerWidth;
        var contentWidth = Math.Max(middleWidth, Math.Max(topWidth, bottomWidth));
        return InlineGroupChromeWidth + contentWidth;
    }

    private static IReadOnlyList<RibbonItem> FilterDockedItems(
        IEnumerable<RibbonItem> items,
        params RibbonItemLayoutDock[] docks)
    {
        if (docks.Length == 0)
        {
            return [];
        }

        return items
            .Where(item => docks.Contains(ResolveLayoutDock(item)))
            .ToList();
    }

    private static RibbonItemLayoutDock ResolveLayoutDock(RibbonItem item)
    {
        return item.LayoutDock == RibbonItemLayoutDock.Auto
            ? RibbonItemLayoutDock.Center
            : item.LayoutDock;
    }

    private static double EstimateDockedCenterItemsWidth(
        IReadOnlyList<RibbonItem> centerItems,
        bool compact,
        RibbonGroupDockedCenterLayoutMode centerLayoutMode,
        int stackedRows)
    {
        if (centerItems.Count == 0)
        {
            return 0;
        }

        return centerLayoutMode switch
        {
            RibbonGroupDockedCenterLayoutMode.Horizontal => EstimateHorizontalItemsWidth(centerItems, compact),
            RibbonGroupDockedCenterLayoutMode.Vertical => EstimateVerticalItemsWidth(centerItems, compact),
            RibbonGroupDockedCenterLayoutMode.Stacked => EstimateStackedItemsWidth(centerItems, compact, stackedRows),
            _ => EstimateHorizontalItemsWidth(centerItems, compact),
        };
    }

    private static double EstimateHorizontalItemsWidth(IReadOnlyList<RibbonItem> visibleItems, bool compact)
    {
        if (visibleItems.Count == 0)
        {
            return 0;
        }

        var width = 0d;
        for (var index = 0; index < visibleItems.Count; index++)
        {
            width += EstimateItemWidth(visibleItems[index], compact);
        }

        if (visibleItems.Count > 1)
        {
            width += (visibleItems.Count - 1) * 4;
        }

        return width;
    }

    private static double EstimateVerticalItemsWidth(IReadOnlyList<RibbonItem> visibleItems, bool compact)
    {
        if (visibleItems.Count == 0)
        {
            return 0;
        }

        var widest = 0d;
        for (var index = 0; index < visibleItems.Count; index++)
        {
            widest = Math.Max(widest, EstimateItemWidth(visibleItems[index], compact));
        }

        return widest;
    }

    private static double EstimateStackedItemsWidth(IReadOnlyList<RibbonItem> visibleItems, bool compact, int stackedRows)
    {
        var rows = Math.Max(1, stackedRows);
        var columnCount = (int)Math.Ceiling(visibleItems.Count / (double)rows);
        var columnWidths = new double[columnCount];

        for (var index = 0; index < visibleItems.Count; index++)
        {
            var column = index / rows;
            columnWidths[column] = Math.Max(columnWidths[column], EstimateItemWidth(visibleItems[index], compact));
        }

        var width = columnWidths.Sum();
        if (columnCount > 1)
        {
            width += (columnCount - 1) * 4;
        }

        return width;
    }

    private static double EstimateItemWidth(RibbonItem item, bool compact)
    {
        return item.Primitive switch
        {
            RibbonItemPrimitive.Button => item.IsToggle
                ? EstimateToggleButtonWidth(item, compact)
                : EstimateButtonWidth(item, compact),
            RibbonItemPrimitive.ToggleButton => EstimateToggleButtonWidth(item, compact),
            RibbonItemPrimitive.ComboBox => EstimateComboBoxWidth(item, compact),
            RibbonItemPrimitive.SplitButton => EstimateSplitButtonWidth(item, compact),
            RibbonItemPrimitive.PasteSplitButton => EstimateSplitButtonWidth(item, compact),
            RibbonItemPrimitive.MenuButton => FallbackMenuButtonWidth,
            RibbonItemPrimitive.Gallery => FallbackGalleryWidth,
            RibbonItemPrimitive.ToggleGroup => EstimateToggleGroupWidth(item, compact),
            RibbonItemPrimitive.Custom => FallbackCustomWidth,
            _ => FallbackLargeButtonWidth,
        };
    }

    private static double EstimateButtonWidth(RibbonItem item, bool compact)
    {
        var size = compact && item.Size == RibbonItemSize.Large
            ? RibbonItemSize.Small
            : item.Size;

        if (item.DisplayMode == RibbonItemDisplayMode.IconOnly)
        {
            return size == RibbonItemSize.Small
                ? FallbackSmallIconOnlyButtonWidth
                : FallbackLargeIconOnlyButtonWidth;
        }

        return size == RibbonItemSize.Small
            ? FallbackSmallButtonWidth
            : FallbackLargeButtonWidth;
    }

    private static double EstimateToggleButtonWidth(RibbonItem item, bool compact)
    {
        var size = compact && item.Size == RibbonItemSize.Large
            ? RibbonItemSize.Small
            : item.Size;

        if (item.DisplayMode == RibbonItemDisplayMode.IconOnly)
        {
            return size == RibbonItemSize.Small
                ? FallbackSmallIconOnlyToggleButtonWidth
                : FallbackLargeIconOnlyToggleButtonWidth;
        }

        return size == RibbonItemSize.Small
            ? FallbackSmallToggleButtonWidth
            : FallbackLargeToggleButtonWidth;
    }

    private static double EstimateComboBoxWidth(RibbonItem item, bool compact)
    {
        var size = compact && item.Size == RibbonItemSize.Large
            ? RibbonItemSize.Small
            : item.Size;

        return size == RibbonItemSize.Small
            ? FallbackSmallComboBoxWidth
            : FallbackLargeComboBoxWidth;
    }

    private static double EstimateSplitButtonWidth(RibbonItem item, bool compact)
    {
        var size = compact && item.Size == RibbonItemSize.Large
            ? RibbonItemSize.Small
            : item.Size;

        if (item.DisplayMode == RibbonItemDisplayMode.IconOnly)
        {
            return size == RibbonItemSize.Small
                ? FallbackSmallIconOnlySplitButtonWidth
                : FallbackLargeIconOnlySplitButtonWidth;
        }

        return size == RibbonItemSize.Small
            ? FallbackSmallSplitButtonWidth
            : FallbackLargeSplitButtonWidth;
    }

    private static double EstimateToggleGroupWidth(RibbonItem item, bool compact)
    {
        var size = compact && item.Size == RibbonItemSize.Large
            ? RibbonItemSize.Small
            : item.Size;

        var optionWidth = size == RibbonItemSize.Small
            ? FallbackSmallToggleGroupOptionWidth
            : FallbackLargeToggleGroupOptionWidth;
        var columns = Math.Max(1, item.ToggleGroupColumns);
        var visibleOptionCount = Math.Max(1, item.ToggleGroupMenuItems.Count);
        var effectiveColumns = Math.Min(columns, visibleOptionCount);
        var width = effectiveColumns * optionWidth;
        if (effectiveColumns > 1)
        {
            width += (effectiveColumns - 1) * 6;
        }

        return width;
    }

    private static bool SupportsCompactSize(RibbonItemPrimitive primitive)
    {
        return primitive is RibbonItemPrimitive.Button
            or RibbonItemPrimitive.ToggleButton
            or RibbonItemPrimitive.ComboBox
            or RibbonItemPrimitive.SplitButton
            or RibbonItemPrimitive.ToggleGroup;
    }
}
