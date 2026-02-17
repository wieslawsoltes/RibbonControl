// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Windows.Input;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Contracts;

public interface IRibbonMenuItemNode : IRibbonNode
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

    string? Description { get; }

    string? InputGestureText { get; }

    string? Category { get; }

    string? PopupSectionId { get; }

    string? PopupSectionHeader { get; }

    int PopupSectionOrder { get; }

    RibbonPopupSectionLayout PopupSectionLayout { get; }

    bool IsSeparator { get; }

    bool ShowChevron { get; }

    bool IsSelected { get; }

    bool ShowInRibbonPreview { get; }

    bool ShowInPopup { get; }

    object? Content { get; }

    IEnumerable<IRibbonMenuItemNode>? SubMenuItems { get; }

    string? KeyTip { get; }

    string? ScreenTip { get; }
}
