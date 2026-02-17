// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Windows.Input;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Models;

public class RibbonMenuItemDefinition : IRibbonMenuItemNode
{
    public string Id { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;

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

    public int Order { get; set; }

    public bool IsVisible { get; set; } = true;

    public bool ReplaceTemplate { get; set; }

    public string? CommandId { get; set; }

    public string? Description { get; set; }

    public string? InputGestureText { get; set; }

    public string? Category { get; set; }

    public string? PopupSectionId { get; set; }

    public string? PopupSectionHeader { get; set; }

    public int PopupSectionOrder { get; set; }

    public RibbonPopupSectionLayout PopupSectionLayout { get; set; } = RibbonPopupSectionLayout.CommandList;

    public bool IsSeparator { get; set; }

    public bool ShowChevron { get; set; }

    public bool IsSelected { get; set; }

    public bool ShowInRibbonPreview { get; set; } = true;

    public bool ShowInPopup { get; set; } = true;

    public object? Content { get; set; }

    public IList<RibbonMenuItemDefinition> SubMenuItems { get; set; } = [];

    IEnumerable<IRibbonMenuItemNode>? IRibbonMenuItemNode.SubMenuItems => SubMenuItems;

    public string? KeyTip { get; set; }

    public string? ScreenTip { get; set; }

    public ICommand? Command => null;

    public object? CommandParameter => null;
}
