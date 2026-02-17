// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;
using RibbonControl.Core.Collections;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Services;

namespace RibbonControl.Core.Models;

public class RibbonGroup : RibbonObservableObject, IRibbonGroupNode
{
    private readonly ObservableCollection<RibbonItem> _mergedItems = new();
    private readonly ReadOnlyObservableCollection<RibbonItem> _readonlyMergedItems;
    private readonly Dictionary<RibbonItem, PropertyChangedEventHandler> _mergedItemHandlers = [];

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
    private double _collapsedWidthHint = 72;
    private RibbonGroupHeaderPlacement _headerPlacement = RibbonGroupHeaderPlacement.Bottom;
    private RibbonGroupItemsLayoutMode _itemsLayoutMode = RibbonGroupItemsLayoutMode.Auto;
    private RibbonGroupDockedCenterLayoutMode _dockedCenterLayoutMode = RibbonGroupDockedCenterLayoutMode.Auto;
    private int _stackedRows = 3;
    private RibbonGroupDisplayMode _displayMode = RibbonGroupDisplayMode.Expanded;
    private bool _isDropDownOpen;
    private IEnumerable<IRibbonItemNode>? _itemsSource;
    private RibbonMergeMode _itemMergeMode = RibbonMergeMode.Merge;
    private IRibbonMergePolicy _mergePolicy = RibbonMergePolicy.StaticThenDynamic;

    public RibbonGroup()
    {
        _readonlyMergedItems = new ReadOnlyObservableCollection<RibbonItem>(_mergedItems);
        Items.CollectionChanged += OnItemsCollectionChanged;
        RebuildMergedItems();
    }

    [Content]
    public ObservableCollection<RibbonItem> Items { get; } = new();

    IEnumerable<IRibbonItemNode>? IRibbonGroupNode.Items => MergedItems;

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

    public object? Icon
    {
        get => _icon;
        set
        {
            if (SetProperty(ref _icon, value))
            {
                RaiseCollapsedIconChanged();
            }
        }
    }

    public object? IconResourceKey
    {
        get => _iconResourceKey;
        set
        {
            if (SetProperty(ref _iconResourceKey, value))
            {
                RaiseCollapsedIconChanged();
            }
        }
    }

    public string? IconPathData
    {
        get => _iconPathData;
        set
        {
            if (SetProperty(ref _iconPathData, value))
            {
                RaiseCollapsedIconChanged();
            }
        }
    }

    public string? IconEmoji
    {
        get => _iconEmoji;
        set
        {
            if (SetProperty(ref _iconEmoji, value))
            {
                RaiseCollapsedIconChanged();
            }
        }
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
        set => SetProperty(ref _iconWidth, NormalizeLength(value));
    }

    public double IconHeight
    {
        get => _iconHeight;
        set => SetProperty(ref _iconHeight, NormalizeLength(value));
    }

    public double IconMinWidth
    {
        get => _iconMinWidth;
        set => SetProperty(ref _iconMinWidth, NormalizeMinLength(value));
    }

    public double IconMinHeight
    {
        get => _iconMinHeight;
        set => SetProperty(ref _iconMinHeight, NormalizeMinLength(value));
    }

    public double IconMaxWidth
    {
        get => _iconMaxWidth;
        set => SetProperty(ref _iconMaxWidth, NormalizeMaxLength(value));
    }

    public double IconMaxHeight
    {
        get => _iconMaxHeight;
        set => SetProperty(ref _iconMaxHeight, NormalizeMaxLength(value));
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

    public int Order
    {
        get => _order;
        set => SetProperty(ref _order, value);
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
        set => SetProperty(ref _expandedWidthHint, Math.Max(0, value));
    }

    public double CompactWidthHint
    {
        get => _compactWidthHint;
        set => SetProperty(ref _compactWidthHint, Math.Max(0, value));
    }

    public double CollapsedWidthHint
    {
        get => _collapsedWidthHint;
        set => SetProperty(ref _collapsedWidthHint, Math.Max(0, value));
    }

    public RibbonGroupHeaderPlacement HeaderPlacement
    {
        get => _headerPlacement;
        set
        {
            if (SetProperty(ref _headerPlacement, value))
            {
                RaiseHeaderPlacementFlagsChanged();
            }
        }
    }

    public RibbonGroupItemsLayoutMode ItemsLayoutMode
    {
        get => _itemsLayoutMode;
        set
        {
            if (SetProperty(ref _itemsLayoutMode, value))
            {
                RaiseItemsLayoutFlagsChanged();
            }
        }
    }

    public RibbonGroupDockedCenterLayoutMode DockedCenterLayoutMode
    {
        get => _dockedCenterLayoutMode;
        set
        {
            if (SetProperty(ref _dockedCenterLayoutMode, value))
            {
                RaiseDockedCenterLayoutFlagsChanged();
            }
        }
    }

    public int StackedRows
    {
        get => _stackedRows;
        set
        {
            SetProperty(ref _stackedRows, Math.Max(1, value));
        }
    }

    public RibbonGroupDisplayMode DisplayMode
    {
        get => _displayMode;
        set
        {
            if (!SetProperty(ref _displayMode, value))
            {
                return;
            }

            RaisePropertyChanged(nameof(IsExpandedMode));
            RaisePropertyChanged(nameof(IsCompactMode));
            RaisePropertyChanged(nameof(IsCollapsedMode));
            RaisePropertyChanged(nameof(IsInlineMode));
            ApplyDisplayModeToItems();

            if (!IsCollapsedMode)
            {
                IsDropDownOpen = false;
            }
        }
    }

    public bool IsDropDownOpen
    {
        get => _isDropDownOpen;
        set => SetProperty(ref _isDropDownOpen, value);
    }

    public bool IsExpandedMode => DisplayMode == RibbonGroupDisplayMode.Expanded;

    public bool IsCompactMode => DisplayMode == RibbonGroupDisplayMode.Compact;

    public bool IsCollapsedMode => DisplayMode == RibbonGroupDisplayMode.Collapsed;

    public bool IsInlineMode => DisplayMode != RibbonGroupDisplayMode.Collapsed;

    public bool IsHeaderBottom => HeaderPlacement == RibbonGroupHeaderPlacement.Bottom;

    public bool IsHeaderTop => HeaderPlacement == RibbonGroupHeaderPlacement.Top;

    public bool IsHeaderLeft => HeaderPlacement == RibbonGroupHeaderPlacement.Left;

    public bool IsHeaderRight => HeaderPlacement == RibbonGroupHeaderPlacement.Right;

    public RibbonGroupItemsLayoutMode EffectiveItemsLayoutMode
        => ItemsLayoutMode == RibbonGroupItemsLayoutMode.Auto
            ? RibbonGroupItemsLayoutMode.Wrap
            : ItemsLayoutMode;

    public bool IsItemsWrapLayout => EffectiveItemsLayoutMode == RibbonGroupItemsLayoutMode.Wrap;

    public bool IsItemsHorizontalLayout => EffectiveItemsLayoutMode == RibbonGroupItemsLayoutMode.Horizontal;

    public bool IsItemsVerticalLayout => EffectiveItemsLayoutMode == RibbonGroupItemsLayoutMode.Vertical;

    public bool IsItemsStackedLayout => EffectiveItemsLayoutMode == RibbonGroupItemsLayoutMode.Stacked;

    public bool IsItemsDockedLayout => EffectiveItemsLayoutMode == RibbonGroupItemsLayoutMode.Docked;

    public RibbonGroupDockedCenterLayoutMode EffectiveDockedCenterLayoutMode
        => DockedCenterLayoutMode == RibbonGroupDockedCenterLayoutMode.Auto
            ? RibbonGroupDockedCenterLayoutMode.Wrap
            : DockedCenterLayoutMode;

    public bool IsDockedCenterWrapLayout => EffectiveDockedCenterLayoutMode == RibbonGroupDockedCenterLayoutMode.Wrap;

    public bool IsDockedCenterHorizontalLayout => EffectiveDockedCenterLayoutMode == RibbonGroupDockedCenterLayoutMode.Horizontal;

    public bool IsDockedCenterVerticalLayout => EffectiveDockedCenterLayoutMode == RibbonGroupDockedCenterLayoutMode.Vertical;

    public bool IsDockedCenterStackedLayout => EffectiveDockedCenterLayoutMode == RibbonGroupDockedCenterLayoutMode.Stacked;

    public IReadOnlyList<RibbonItem> DockedTopItems => BuildDockedItems(RibbonItemLayoutDock.Top);

    public IReadOnlyList<RibbonItem> DockedBottomItems => BuildDockedItems(RibbonItemLayoutDock.Bottom);

    public IReadOnlyList<RibbonItem> DockedLeftItems => BuildDockedItems(RibbonItemLayoutDock.Left);

    public IReadOnlyList<RibbonItem> DockedRightItems => BuildDockedItems(RibbonItemLayoutDock.Right);

    public IReadOnlyList<RibbonItem> DockedCenterItems
        => BuildDockedItems(RibbonItemLayoutDock.Auto, RibbonItemLayoutDock.Center);

    public bool HasDockedTopItems => DockedTopItems.Count > 0;

    public bool HasDockedBottomItems => DockedBottomItems.Count > 0;

    public bool HasDockedLeftItems => DockedLeftItems.Count > 0;

    public bool HasDockedRightItems => DockedRightItems.Count > 0;

    public bool HasDockedCenterItems => DockedCenterItems.Count > 0;

    public bool HasIcon =>
        Icon is not null ||
        IconResourceKey is not null ||
        !string.IsNullOrWhiteSpace(IconPathData) ||
        !string.IsNullOrWhiteSpace(IconEmoji);

    public bool HasOverlay =>
        Overlay is not null ||
        OverlayResourceKey is not null ||
        !string.IsNullOrWhiteSpace(OverlayPathData) ||
        !string.IsNullOrWhiteSpace(OverlayEmoji);

    public bool HasOverlayCount =>
        !string.IsNullOrWhiteSpace(OverlayCountText) ||
        (OverlayCount.HasValue && (ShowOverlayCountWhenZero || OverlayCount.Value != 0));

    public object? CollapsedIcon
    {
        get
        {
            if (Icon is not null)
            {
                return Icon;
            }

            return GetFirstVisibleItemWithIcon()?.Icon;
        }
    }

    public object? CollapsedIconResourceKey
    {
        get
        {
            if (IconResourceKey is not null)
            {
                return IconResourceKey;
            }

            return GetFirstVisibleItemWithIcon()?.IconResourceKey;
        }
    }

    public string? CollapsedIconPathData
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(IconPathData))
            {
                return IconPathData;
            }

            return GetFirstVisibleItemWithIcon()?.IconPathData;
        }
    }

    public string? CollapsedIconEmoji
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(IconEmoji))
            {
                return IconEmoji;
            }

            return GetFirstVisibleItemWithIcon()?.IconEmoji;
        }
    }

    public bool HasCollapsedIcon =>
        CollapsedIcon is not null ||
        CollapsedIconResourceKey is not null ||
        !string.IsNullOrWhiteSpace(CollapsedIconPathData) ||
        !string.IsNullOrWhiteSpace(CollapsedIconEmoji);

    public bool HasCollapsedIconPathData => !string.IsNullOrWhiteSpace(CollapsedIconPathData);

    public IEnumerable<IRibbonItemNode>? ItemsSource
    {
        get => _itemsSource;
        set
        {
            if (SetProperty(ref _itemsSource, value))
            {
                RebuildMergedItems();
            }
        }
    }

    public RibbonMergeMode ItemMergeMode
    {
        get => _itemMergeMode;
        set
        {
            if (SetProperty(ref _itemMergeMode, value))
            {
                RebuildMergedItems();
            }
        }
    }

    public IRibbonMergePolicy MergePolicy
    {
        get => _mergePolicy;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (SetProperty(ref _mergePolicy, value))
            {
                RebuildMergedItems();
            }
        }
    }

    public ReadOnlyObservableCollection<RibbonItem> MergedItems => _readonlyMergedItems;

    internal void ReleaseMergedItemBindings()
    {
        DetachMergedItemHandlers();
    }

    public void RebuildMergedItems()
    {
        var merged = MergePolicy.MergeItems(Items, ItemsSource, ItemMergeMode);
        DetachMergedItemHandlers();
        _mergedItems.ReplaceWith(merged);
        AttachMergedItemHandlers();
        ApplyDisplayModeToItems();
        RaisePropertyChanged(nameof(MergedItems));
        RaiseCollapsedIconChanged();
        RaiseDockedItemCollectionsChanged();
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildMergedItems();
    }

    private void AttachMergedItemHandlers()
    {
        foreach (var item in _mergedItems)
        {
            var capturedItem = item;
            PropertyChangedEventHandler handler = (_, args) =>
            {
                if (args.PropertyName == nameof(RibbonItem.IsVisible) || IsIconProperty(args.PropertyName))
                {
                    RaiseCollapsedIconChanged();
                }

                if (args.PropertyName is nameof(RibbonItem.LayoutDock) or nameof(RibbonItem.IsVisible))
                {
                    RaiseDockedItemCollectionsChanged();
                }
            };

            capturedItem.PropertyChanged += handler;
            _mergedItemHandlers[capturedItem] = handler;
        }
    }

    private void DetachMergedItemHandlers()
    {
        foreach (var pair in _mergedItemHandlers)
        {
            pair.Key.PropertyChanged -= pair.Value;
        }

        _mergedItemHandlers.Clear();

        foreach (var item in _mergedItems)
        {
            item.ClearStateSyncSource();
        }
    }

    private void ApplyDisplayModeToItems()
    {
        foreach (var item in _mergedItems)
        {
            if (DisplayMode == RibbonGroupDisplayMode.Compact &&
                item.Size == RibbonItemSize.Large &&
                SupportsCompactSize(item.Primitive))
            {
                item.SetAdaptiveEffectiveSize(RibbonItemSize.Small);
            }
            else
            {
                item.SetAdaptiveEffectiveSize(null);
            }

            if (DisplayMode == RibbonGroupDisplayMode.Collapsed && item.IsDropDownOpen)
            {
                item.IsDropDownOpen = false;
            }
        }
    }

    private static bool SupportsCompactSize(RibbonItemPrimitive primitive)
    {
        return primitive is RibbonItemPrimitive.Button
            or RibbonItemPrimitive.ToggleButton
            or RibbonItemPrimitive.ComboBox
            or RibbonItemPrimitive.SplitButton
            or RibbonItemPrimitive.ToggleGroup;
    }

    private void RaiseHeaderPlacementFlagsChanged()
    {
        RaisePropertyChanged(nameof(IsHeaderBottom));
        RaisePropertyChanged(nameof(IsHeaderTop));
        RaisePropertyChanged(nameof(IsHeaderLeft));
        RaisePropertyChanged(nameof(IsHeaderRight));
    }

    private void RaiseItemsLayoutFlagsChanged()
    {
        RaisePropertyChanged(nameof(EffectiveItemsLayoutMode));
        RaisePropertyChanged(nameof(IsItemsWrapLayout));
        RaisePropertyChanged(nameof(IsItemsHorizontalLayout));
        RaisePropertyChanged(nameof(IsItemsVerticalLayout));
        RaisePropertyChanged(nameof(IsItemsStackedLayout));
        RaisePropertyChanged(nameof(IsItemsDockedLayout));
        RaiseDockedCenterLayoutFlagsChanged();
    }

    private void RaiseDockedCenterLayoutFlagsChanged()
    {
        RaisePropertyChanged(nameof(EffectiveDockedCenterLayoutMode));
        RaisePropertyChanged(nameof(IsDockedCenterWrapLayout));
        RaisePropertyChanged(nameof(IsDockedCenterHorizontalLayout));
        RaisePropertyChanged(nameof(IsDockedCenterVerticalLayout));
        RaisePropertyChanged(nameof(IsDockedCenterStackedLayout));
    }

    private void RaiseDockedItemCollectionsChanged()
    {
        RaisePropertyChanged(nameof(DockedTopItems));
        RaisePropertyChanged(nameof(DockedBottomItems));
        RaisePropertyChanged(nameof(DockedLeftItems));
        RaisePropertyChanged(nameof(DockedRightItems));
        RaisePropertyChanged(nameof(DockedCenterItems));
        RaisePropertyChanged(nameof(HasDockedTopItems));
        RaisePropertyChanged(nameof(HasDockedBottomItems));
        RaisePropertyChanged(nameof(HasDockedLeftItems));
        RaisePropertyChanged(nameof(HasDockedRightItems));
        RaisePropertyChanged(nameof(HasDockedCenterItems));
    }

    private void RaiseCollapsedIconChanged()
    {
        RaisePropertyChanged(nameof(HasIcon));
        RaisePropertyChanged(nameof(CollapsedIcon));
        RaisePropertyChanged(nameof(CollapsedIconResourceKey));
        RaisePropertyChanged(nameof(CollapsedIconPathData));
        RaisePropertyChanged(nameof(CollapsedIconEmoji));
        RaisePropertyChanged(nameof(HasCollapsedIcon));
        RaisePropertyChanged(nameof(HasCollapsedIconPathData));
    }

    private RibbonItem? GetFirstVisibleItemWithIcon()
    {
        return MergedItems.FirstOrDefault(item => item.IsVisible && item.HasIcon);
    }

    private static bool IsIconProperty(string? propertyName)
    {
        return propertyName is nameof(RibbonItem.Icon)
            or nameof(RibbonItem.IconResourceKey)
            or nameof(RibbonItem.IconPathData)
            or nameof(RibbonItem.IconEmoji)
            or nameof(RibbonItem.HasIcon);
    }

    private static double NormalizeLength(double value)
    {
        return double.IsNaN(value) || value >= 0
            ? value
            : double.NaN;
    }

    private static double NormalizeMinLength(double value)
    {
        return value <= 0
            ? 0
            : value;
    }

    private static double NormalizeMaxLength(double value)
    {
        return double.IsNaN(value) || value <= 0 || double.IsPositiveInfinity(value)
            ? double.PositiveInfinity
            : value;
    }

    private IReadOnlyList<RibbonItem> BuildDockedItems(params RibbonItemLayoutDock[] docks)
    {
        if (docks.Length == 0)
        {
            return [];
        }

        return _mergedItems
            .Where(item => item.IsVisible && docks.Contains(ResolveLayoutDock(item)))
            .ToList();
    }

    private static RibbonItemLayoutDock ResolveLayoutDock(RibbonItem item)
    {
        return item.LayoutDock == RibbonItemLayoutDock.Auto
            ? RibbonItemLayoutDock.Center
            : item.LayoutDock;
    }
}
