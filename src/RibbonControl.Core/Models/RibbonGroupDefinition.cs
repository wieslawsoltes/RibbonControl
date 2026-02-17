// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Models;

public class RibbonGroupDefinition : IRibbonGroupNode
{
    public string Id { get; set; } = string.Empty;

    public string Header { get; set; } = string.Empty;

    public object? Icon { get; set; }

    public object? IconResourceKey { get; set; }

    public string? IconPathData { get; set; }

    public string? IconEmoji { get; set; }

    public Stretch IconStretch { get; set; } = Stretch.Uniform;

    public StretchDirection IconStretchDirection { get; set; } = StretchDirection.Both;

    public double IconWidth { get; set; } = double.NaN;

    public double IconHeight { get; set; } = double.NaN;

    public double IconMinWidth { get; set; }

    public double IconMinHeight { get; set; }

    public double IconMaxWidth { get; set; } = double.PositiveInfinity;

    public double IconMaxHeight { get; set; } = double.PositiveInfinity;

    public object? Overlay { get; set; }

    public object? OverlayResourceKey { get; set; }

    public string? OverlayPathData { get; set; }

    public string? OverlayEmoji { get; set; }

    public int? OverlayCount { get; set; }

    public string? OverlayCountText { get; set; }

    public bool ShowOverlayCountWhenZero { get; set; }

    public HorizontalAlignment OverlayHorizontalAlignment { get; set; } = HorizontalAlignment.Right;

    public VerticalAlignment OverlayVerticalAlignment { get; set; } = VerticalAlignment.Bottom;

    public Thickness OverlayMargin { get; set; }

    public HorizontalAlignment OverlayCountHorizontalAlignment { get; set; } = HorizontalAlignment.Right;

    public VerticalAlignment OverlayCountVerticalAlignment { get; set; } = VerticalAlignment.Bottom;

    public Thickness OverlayCountMargin { get; set; }

    public RibbonGroupHeaderPlacement HeaderPlacement { get; set; } = RibbonGroupHeaderPlacement.Bottom;

    public RibbonGroupItemsLayoutMode ItemsLayoutMode { get; set; } = RibbonGroupItemsLayoutMode.Auto;

    public RibbonGroupDockedCenterLayoutMode DockedCenterLayoutMode { get; set; } = RibbonGroupDockedCenterLayoutMode.Auto;

    public int StackedRows { get; set; } = 3;

    public int Order { get; set; }

    public bool IsVisible { get; set; } = true;

    public bool ReplaceTemplate { get; set; }

    public bool CanAutoCollapse { get; set; } = true;

    public int CollapsePriority { get; set; }

    public double ExpandedWidthHint { get; set; }

    public double CompactWidthHint { get; set; }

    public double CollapsedWidthHint { get; set; }

    public IList<RibbonItemDefinition> Items { get; set; } = [];

    IEnumerable<IRibbonItemNode>? IRibbonGroupNode.Items => Items;
}
