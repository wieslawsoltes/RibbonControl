// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Windows.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace RibbonControl.Core.Contracts;

public interface IRibbonBackstageItemNode : IRibbonNode
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

    string? Description { get; }

    string? InputGestureText { get; }

    bool IsEnabled { get; }

    bool IsSeparator { get; }

    bool ShowChevron { get; }

    bool ExecuteCommandOnSelect { get; }

    bool CloseBackstageOnExecute { get; }

    string? CommandId { get; }

    ICommand? Command { get; }

    object? CommandParameter { get; }

    object? Content { get; }

    IEnumerable<IRibbonBackstageItemNode>? SubItems { get; }

    string? KeyTip { get; }

    string? ScreenTip { get; }
}
