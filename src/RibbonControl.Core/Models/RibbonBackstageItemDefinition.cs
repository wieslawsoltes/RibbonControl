// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Windows.Input;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Contracts;

namespace RibbonControl.Core.Models;

public class RibbonBackstageItemDefinition : IRibbonBackstageItemNode
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

    public int Order { get; set; }

    public bool IsVisible { get; set; } = true;

    public bool ReplaceTemplate { get; set; }

    public bool IsEnabled { get; set; } = true;

    public bool IsSeparator { get; set; }

    public bool ShowChevron { get; set; }

    public bool ExecuteCommandOnSelect { get; set; }

    public bool CloseBackstageOnExecute { get; set; }

    public string? CommandId { get; set; }

    public string? Description { get; set; }

    public string? InputGestureText { get; set; }

    public object? Content { get; set; }

    public IList<RibbonBackstageItemDefinition> SubItems { get; set; } = [];

    IEnumerable<IRibbonBackstageItemNode>? IRibbonBackstageItemNode.SubItems => SubItems;

    public string? KeyTip { get; set; }

    public string? ScreenTip { get; set; }

    public ICommand? Command => null;

    public object? CommandParameter => null;
}
