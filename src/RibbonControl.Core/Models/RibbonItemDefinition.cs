// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Windows.Input;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Models;

public class RibbonItemDefinition : IRibbonItemNode
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

    public RibbonItemPrimitive Primitive { get; set; }

    public RibbonSplitButtonMode SplitButtonMode { get; set; }

    public RibbonItemDisplayMode DisplayMode { get; set; } = RibbonItemDisplayMode.Auto;

    public RibbonItemSize Size { get; set; }

    public RibbonItemLayoutDock LayoutDock { get; set; } = RibbonItemLayoutDock.Auto;

    public bool IsToggle { get; set; }

    public RibbonToggleGroupSelectionMode ToggleGroupSelectionMode { get; set; } = RibbonToggleGroupSelectionMode.Multiple;

    public int ToggleGroupColumns { get; set; } = 2;

    public bool IsChecked { get; set; }

    public string? SelectedMenuItemId { get; set; }

    public object? Content { get; set; }

    public object? PopupContent { get; set; }

    public string? PopupTitle { get; set; }

    public object? PopupFooterContent { get; set; }

    public double PopupMinWidth { get; set; } = 220;

    public double PopupMaxHeight { get; set; } = 460;

    public int GalleryPreviewMaxItems { get; set; } = 3;

    public bool GalleryShowCategoryHeaders { get; set; } = true;

    public bool IsDropDownOpen { get; set; }

    public string? Description { get; set; }

    public string? SecondaryCommandId { get; set; }

    public IList<RibbonMenuItemDefinition> MenuItems { get; set; } = [];

    IEnumerable<IRibbonMenuItemNode>? IRibbonItemNode.MenuItems => MenuItems;

    public IList<RibbonItemDefinition> Items { get; set; } = [];

    IEnumerable<IRibbonItemNode>? IRibbonItemNode.Items => Items;

    public string? KeyTip { get; set; }

    public string? ScreenTip { get; set; }

    public ICommand? Command => null;

    public object? CommandParameter => null;

    public ICommand? SecondaryCommand => null;

    public object? SecondaryCommandParameter => null;
}
