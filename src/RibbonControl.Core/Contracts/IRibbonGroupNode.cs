// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Contracts;

public interface IRibbonGroupNode : IRibbonNode
{
    string Header { get; }

    object? Icon { get; }

    object? IconResourceKey { get; }

    string? IconPathData { get; }

    string? IconEmoji { get; }

    Stretch IconStretch { get; }

    StretchDirection IconStretchDirection { get; }

    double IconWidth { get; }

    double IconHeight { get; }

    double IconMinWidth { get; }

    double IconMinHeight { get; }

    double IconMaxWidth { get; }

    double IconMaxHeight { get; }

    object? Overlay { get; }

    object? OverlayResourceKey { get; }

    string? OverlayPathData { get; }

    string? OverlayEmoji { get; }

    int? OverlayCount { get; }

    string? OverlayCountText { get; }

    bool ShowOverlayCountWhenZero { get; }

    HorizontalAlignment OverlayHorizontalAlignment { get; }

    VerticalAlignment OverlayVerticalAlignment { get; }

    Thickness OverlayMargin { get; }

    HorizontalAlignment OverlayCountHorizontalAlignment { get; }

    VerticalAlignment OverlayCountVerticalAlignment { get; }

    Thickness OverlayCountMargin { get; }

    RibbonGroupHeaderPlacement HeaderPlacement { get; }

    RibbonGroupItemsLayoutMode ItemsLayoutMode { get; }

    RibbonGroupDockedCenterLayoutMode DockedCenterLayoutMode { get; }

    int StackedRows { get; }

    bool CanAutoCollapse { get; }

    int CollapsePriority { get; }

    double ExpandedWidthHint { get; }

    double CompactWidthHint { get; }

    double CollapsedWidthHint { get; }

    IEnumerable<IRibbonItemNode>? Items { get; }
}
