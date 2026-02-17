// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.ViewModels;

public class RibbonGroupViewModel : RibbonObservableObject, IRibbonGroupNode
{
    private string _id = string.Empty;
    private string _header = string.Empty;
    private object? _icon;
    private object? _iconResourceKey;
    private string? _iconPathData;
    private string? _iconEmoji;
    private Stretch _iconStretch = Stretch.Uniform;
    private StretchDirection _iconStretchDirection = StretchDirection.Both;
    private double _iconWidth = double.NaN;
    private double _iconHeight = double.NaN;
    private double _iconMinWidth;
    private double _iconMinHeight;
    private double _iconMaxWidth = double.PositiveInfinity;
    private double _iconMaxHeight = double.PositiveInfinity;
    private object? _overlay;
    private object? _overlayResourceKey;
    private string? _overlayPathData;
    private string? _overlayEmoji;
    private int? _overlayCount;
    private string? _overlayCountText;
    private bool _showOverlayCountWhenZero;
    private HorizontalAlignment _overlayHorizontalAlignment = HorizontalAlignment.Right;
    private VerticalAlignment _overlayVerticalAlignment = VerticalAlignment.Bottom;
    private Thickness _overlayMargin;
    private HorizontalAlignment _overlayCountHorizontalAlignment = HorizontalAlignment.Right;
    private VerticalAlignment _overlayCountVerticalAlignment = VerticalAlignment.Bottom;
    private Thickness _overlayCountMargin;
    private int _order;
    private bool _isVisible = true;
    private bool _replaceTemplate;
    private bool _canAutoCollapse = true;
    private int _collapsePriority;
    private double _expandedWidthHint;
    private double _compactWidthHint;
    private double _collapsedWidthHint;
    private RibbonGroupHeaderPlacement _headerPlacement = RibbonGroupHeaderPlacement.Bottom;
    private RibbonGroupItemsLayoutMode _itemsLayoutMode = RibbonGroupItemsLayoutMode.Auto;
    private RibbonGroupDockedCenterLayoutMode _dockedCenterLayoutMode = RibbonGroupDockedCenterLayoutMode.Auto;
    private int _stackedRows = 3;

    public ObservableCollection<RibbonItemViewModel> ItemsViewModel { get; } = [];

    IEnumerable<IRibbonItemNode>? IRibbonGroupNode.Items => ItemsViewModel;

    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Header
    {
        get => _header;
        set => SetProperty(ref _header, value);
    }

    public int Order
    {
        get => _order;
        set => SetProperty(ref _order, value);
    }

    public object? Icon
    {
        get => _icon;
        set => SetProperty(ref _icon, value);
    }

    public object? IconResourceKey
    {
        get => _iconResourceKey;
        set => SetProperty(ref _iconResourceKey, value);
    }

    public string? IconPathData
    {
        get => _iconPathData;
        set => SetProperty(ref _iconPathData, value);
    }

    public string? IconEmoji
    {
        get => _iconEmoji;
        set => SetProperty(ref _iconEmoji, value);
    }

    public Stretch IconStretch
    {
        get => _iconStretch;
        set => SetProperty(ref _iconStretch, value);
    }

    public StretchDirection IconStretchDirection
    {
        get => _iconStretchDirection;
        set => SetProperty(ref _iconStretchDirection, value);
    }

    public double IconWidth
    {
        get => _iconWidth;
        set => SetProperty(ref _iconWidth, value);
    }

    public double IconHeight
    {
        get => _iconHeight;
        set => SetProperty(ref _iconHeight, value);
    }

    public double IconMinWidth
    {
        get => _iconMinWidth;
        set => SetProperty(ref _iconMinWidth, Math.Max(0, value));
    }

    public double IconMinHeight
    {
        get => _iconMinHeight;
        set => SetProperty(ref _iconMinHeight, Math.Max(0, value));
    }

    public double IconMaxWidth
    {
        get => _iconMaxWidth;
        set => SetProperty(ref _iconMaxWidth, double.IsNaN(value) || value <= 0 ? double.PositiveInfinity : value);
    }

    public double IconMaxHeight
    {
        get => _iconMaxHeight;
        set => SetProperty(ref _iconMaxHeight, double.IsNaN(value) || value <= 0 ? double.PositiveInfinity : value);
    }

    public object? Overlay
    {
        get => _overlay;
        set => SetProperty(ref _overlay, value);
    }

    public object? OverlayResourceKey
    {
        get => _overlayResourceKey;
        set => SetProperty(ref _overlayResourceKey, value);
    }

    public string? OverlayPathData
    {
        get => _overlayPathData;
        set => SetProperty(ref _overlayPathData, value);
    }

    public string? OverlayEmoji
    {
        get => _overlayEmoji;
        set => SetProperty(ref _overlayEmoji, value);
    }

    public int? OverlayCount
    {
        get => _overlayCount;
        set => SetProperty(ref _overlayCount, value);
    }

    public string? OverlayCountText
    {
        get => _overlayCountText;
        set => SetProperty(ref _overlayCountText, value);
    }

    public bool ShowOverlayCountWhenZero
    {
        get => _showOverlayCountWhenZero;
        set => SetProperty(ref _showOverlayCountWhenZero, value);
    }

    public HorizontalAlignment OverlayHorizontalAlignment
    {
        get => _overlayHorizontalAlignment;
        set => SetProperty(ref _overlayHorizontalAlignment, value);
    }

    public VerticalAlignment OverlayVerticalAlignment
    {
        get => _overlayVerticalAlignment;
        set => SetProperty(ref _overlayVerticalAlignment, value);
    }

    public Thickness OverlayMargin
    {
        get => _overlayMargin;
        set => SetProperty(ref _overlayMargin, value);
    }

    public HorizontalAlignment OverlayCountHorizontalAlignment
    {
        get => _overlayCountHorizontalAlignment;
        set => SetProperty(ref _overlayCountHorizontalAlignment, value);
    }

    public VerticalAlignment OverlayCountVerticalAlignment
    {
        get => _overlayCountVerticalAlignment;
        set => SetProperty(ref _overlayCountVerticalAlignment, value);
    }

    public Thickness OverlayCountMargin
    {
        get => _overlayCountMargin;
        set => SetProperty(ref _overlayCountMargin, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public bool ReplaceTemplate
    {
        get => _replaceTemplate;
        set => SetProperty(ref _replaceTemplate, value);
    }

    public bool CanAutoCollapse
    {
        get => _canAutoCollapse;
        set => SetProperty(ref _canAutoCollapse, value);
    }

    public int CollapsePriority
    {
        get => _collapsePriority;
        set => SetProperty(ref _collapsePriority, value);
    }

    public double ExpandedWidthHint
    {
        get => _expandedWidthHint;
        set => SetProperty(ref _expandedWidthHint, value);
    }

    public double CompactWidthHint
    {
        get => _compactWidthHint;
        set => SetProperty(ref _compactWidthHint, value);
    }

    public double CollapsedWidthHint
    {
        get => _collapsedWidthHint;
        set => SetProperty(ref _collapsedWidthHint, value);
    }

    public RibbonGroupHeaderPlacement HeaderPlacement
    {
        get => _headerPlacement;
        set => SetProperty(ref _headerPlacement, value);
    }

    public RibbonGroupItemsLayoutMode ItemsLayoutMode
    {
        get => _itemsLayoutMode;
        set => SetProperty(ref _itemsLayoutMode, value);
    }

    public RibbonGroupDockedCenterLayoutMode DockedCenterLayoutMode
    {
        get => _dockedCenterLayoutMode;
        set => SetProperty(ref _dockedCenterLayoutMode, value);
    }

    public int StackedRows
    {
        get => _stackedRows;
        set => SetProperty(ref _stackedRows, Math.Max(1, value));
    }
}
