// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Windows.Input;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Contracts;

public interface IRibbonItemNode : IRibbonNode
{
    string Label { get; }

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

    string? CommandId { get; }

    ICommand? Command { get; }

    object? CommandParameter { get; }

    RibbonItemPrimitive Primitive { get; }

    RibbonSplitButtonMode SplitButtonMode { get; }

    RibbonItemDisplayMode DisplayMode { get; }

    RibbonItemSize Size { get; }

    RibbonItemLayoutDock LayoutDock { get; }

    bool IsToggle { get; }

    RibbonToggleGroupSelectionMode ToggleGroupSelectionMode { get; }

    int ToggleGroupColumns { get; }

    bool IsChecked { get; }

    string? SelectedMenuItemId { get; }

    object? Content { get; }

    object? PopupContent { get; }

    string? PopupTitle { get; }

    object? PopupFooterContent { get; }

    double PopupMinWidth { get; }

    double PopupMaxHeight { get; }

    int GalleryPreviewMaxItems { get; }

    bool GalleryShowCategoryHeaders { get; }

    bool IsDropDownOpen { get; }

    string? Description { get; }

    string? SecondaryCommandId { get; }

    ICommand? SecondaryCommand { get; }

    object? SecondaryCommandParameter { get; }

    IEnumerable<IRibbonMenuItemNode>? MenuItems { get; }

    string? KeyTip { get; }

    string? ScreenTip { get; }
}
