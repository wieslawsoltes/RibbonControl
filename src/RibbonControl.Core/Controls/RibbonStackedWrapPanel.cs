// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Controls;

namespace RibbonControl.Core.Controls;

public class RibbonStackedWrapPanel : Panel
{
    public static readonly StyledProperty<int> MaxRowsProperty =
        AvaloniaProperty.Register<RibbonStackedWrapPanel, int>(nameof(MaxRows), 3);

    public static readonly StyledProperty<double> HorizontalSpacingProperty =
        AvaloniaProperty.Register<RibbonStackedWrapPanel, double>(nameof(HorizontalSpacing), 4);

    public static readonly StyledProperty<double> VerticalSpacingProperty =
        AvaloniaProperty.Register<RibbonStackedWrapPanel, double>(nameof(VerticalSpacing), 4);

    static RibbonStackedWrapPanel()
    {
        AffectsMeasure<RibbonStackedWrapPanel>(
            MaxRowsProperty,
            HorizontalSpacingProperty,
            VerticalSpacingProperty);
    }

    public int MaxRows
    {
        get => GetValue(MaxRowsProperty);
        set => SetValue(MaxRowsProperty, Math.Max(1, value));
    }

    public double HorizontalSpacing
    {
        get => GetValue(HorizontalSpacingProperty);
        set => SetValue(HorizontalSpacingProperty, Math.Max(0, value));
    }

    public double VerticalSpacing
    {
        get => GetValue(VerticalSpacingProperty);
        set => SetValue(VerticalSpacingProperty, Math.Max(0, value));
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var visibleChildren = Children
            .Where(child => child.IsVisible)
            .ToArray();

        if (visibleChildren.Length == 0)
        {
            return default;
        }

        var unconstrained = new Size(double.PositiveInfinity, double.PositiveInfinity);
        foreach (var child in visibleChildren)
        {
            child.Measure(unconstrained);
        }

        return CalculateExtent(visibleChildren);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var visibleChildren = Children
            .Where(child => child.IsVisible)
            .ToArray();

        if (visibleChildren.Length == 0)
        {
            return finalSize;
        }

        var rows = Math.Max(1, MaxRows);
        var columnCount = (int)Math.Ceiling(visibleChildren.Length / (double)rows);
        var usedRows = Math.Min(rows, visibleChildren.Length);
        var horizontalSpacing = Math.Max(0, HorizontalSpacing);
        var verticalSpacing = Math.Max(0, VerticalSpacing);

        var columnWidths = new double[columnCount];
        var rowHeights = new double[usedRows];

        for (var index = 0; index < visibleChildren.Length; index++)
        {
            var row = index % rows;
            var column = index / rows;
            if (row >= usedRows)
            {
                continue;
            }

            var desired = visibleChildren[index].DesiredSize;
            if (desired.Width > columnWidths[column])
            {
                columnWidths[column] = desired.Width;
            }

            if (desired.Height > rowHeights[row])
            {
                rowHeights[row] = desired.Height;
            }
        }

        var xOffsets = new double[columnCount];
        for (var column = 1; column < columnCount; column++)
        {
            xOffsets[column] = xOffsets[column - 1] + columnWidths[column - 1] + horizontalSpacing;
        }

        var yOffsets = new double[usedRows];
        for (var row = 1; row < usedRows; row++)
        {
            yOffsets[row] = yOffsets[row - 1] + rowHeights[row - 1] + verticalSpacing;
        }

        for (var index = 0; index < visibleChildren.Length; index++)
        {
            var row = index % rows;
            var column = index / rows;
            if (row >= usedRows)
            {
                continue;
            }

            visibleChildren[index].Arrange(
                new Rect(
                    xOffsets[column],
                    yOffsets[row],
                    columnWidths[column],
                    rowHeights[row]));
        }

        return finalSize;
    }

    private Size CalculateExtent(IReadOnlyList<Control> visibleChildren)
    {
        var rows = Math.Max(1, MaxRows);
        var columnCount = (int)Math.Ceiling(visibleChildren.Count / (double)rows);
        var usedRows = Math.Min(rows, visibleChildren.Count);
        var horizontalSpacing = Math.Max(0, HorizontalSpacing);
        var verticalSpacing = Math.Max(0, VerticalSpacing);

        var columnWidths = new double[columnCount];
        var rowHeights = new double[usedRows];

        for (var index = 0; index < visibleChildren.Count; index++)
        {
            var row = index % rows;
            var column = index / rows;
            if (row >= usedRows)
            {
                continue;
            }

            var desired = visibleChildren[index].DesiredSize;
            if (desired.Width > columnWidths[column])
            {
                columnWidths[column] = desired.Width;
            }

            if (desired.Height > rowHeights[row])
            {
                rowHeights[row] = desired.Height;
            }
        }

        var width = columnWidths.Sum();
        if (columnCount > 1)
        {
            width += horizontalSpacing * (columnCount - 1);
        }

        var height = rowHeights.Sum();
        if (usedRows > 1)
        {
            height += verticalSpacing * (usedRows - 1);
        }

        return new Size(width, height);
    }
}
