// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.ViewModels;

public class RibbonItemViewModel : RibbonObservableObject, IRibbonItemNode
{
    private string _id = string.Empty;
    private string _label = string.Empty;
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
    private string? _commandId;
    private ICommand? _command;
    private object? _commandParameter;
    private RibbonItemPrimitive _primitive;
    private RibbonSplitButtonMode _splitButtonMode = RibbonSplitButtonMode.SideBySide;
    private RibbonItemDisplayMode _displayMode = RibbonItemDisplayMode.Auto;
    private RibbonItemSize _size = RibbonItemSize.Large;
    private RibbonItemLayoutDock _layoutDock = RibbonItemLayoutDock.Auto;
    private bool _isToggle;
    private RibbonToggleGroupSelectionMode _toggleGroupSelectionMode = RibbonToggleGroupSelectionMode.Multiple;
    private int _toggleGroupColumns = 2;
    private bool _isChecked;
    private string? _selectedMenuItemId;
    private object? _content;
    private object? _popupContent;
    private string? _popupTitle;
    private object? _popupFooterContent;
    private double _popupMinWidth = 220;
    private double _popupMaxHeight = 460;
    private int _galleryPreviewMaxItems = 3;
    private bool _galleryShowCategoryHeaders = true;
    private bool _isDropDownOpen;
    private string? _description;
    private string? _secondaryCommandId;
    private ICommand? _secondaryCommand;
    private object? _secondaryCommandParameter;
    private string? _keyTip;
    private string? _screenTip;

    public ObservableCollection<RibbonMenuItemViewModel> MenuItemsViewModel { get; } = [];

    IEnumerable<IRibbonMenuItemNode>? IRibbonItemNode.MenuItems => MenuItemsViewModel;

    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Label
    {
        get => _label;
        set => SetProperty(ref _label, value);
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

    public string? CommandId
    {
        get => _commandId;
        set => SetProperty(ref _commandId, value);
    }

    public ICommand? Command
    {
        get => _command;
        set => SetProperty(ref _command, value);
    }

    public object? CommandParameter
    {
        get => _commandParameter;
        set => SetProperty(ref _commandParameter, value);
    }

    public RibbonItemPrimitive Primitive
    {
        get => _primitive;
        set => SetProperty(ref _primitive, value);
    }

    public RibbonSplitButtonMode SplitButtonMode
    {
        get => _splitButtonMode;
        set => SetProperty(ref _splitButtonMode, value);
    }

    public RibbonItemDisplayMode DisplayMode
    {
        get => _displayMode;
        set => SetProperty(ref _displayMode, value);
    }

    public RibbonItemSize Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }

    public RibbonItemLayoutDock LayoutDock
    {
        get => _layoutDock;
        set => SetProperty(ref _layoutDock, value);
    }

    public bool IsToggle
    {
        get => _isToggle;
        set => SetProperty(ref _isToggle, value);
    }

    public RibbonToggleGroupSelectionMode ToggleGroupSelectionMode
    {
        get => _toggleGroupSelectionMode;
        set => SetProperty(ref _toggleGroupSelectionMode, value);
    }

    public int ToggleGroupColumns
    {
        get => _toggleGroupColumns;
        set => SetProperty(ref _toggleGroupColumns, Math.Max(1, value));
    }

    public bool IsChecked
    {
        get => _isChecked;
        set => SetProperty(ref _isChecked, value);
    }

    public string? SelectedMenuItemId
    {
        get => _selectedMenuItemId;
        set => SetProperty(ref _selectedMenuItemId, value);
    }

    public object? Content
    {
        get => _content;
        set => SetProperty(ref _content, value);
    }

    public object? PopupContent
    {
        get => _popupContent;
        set => SetProperty(ref _popupContent, value);
    }

    public string? PopupTitle
    {
        get => _popupTitle;
        set => SetProperty(ref _popupTitle, value);
    }

    public object? PopupFooterContent
    {
        get => _popupFooterContent;
        set => SetProperty(ref _popupFooterContent, value);
    }

    public double PopupMinWidth
    {
        get => _popupMinWidth;
        set => SetProperty(ref _popupMinWidth, value);
    }

    public double PopupMaxHeight
    {
        get => _popupMaxHeight;
        set => SetProperty(ref _popupMaxHeight, value);
    }

    public int GalleryPreviewMaxItems
    {
        get => _galleryPreviewMaxItems;
        set => SetProperty(ref _galleryPreviewMaxItems, value);
    }

    public bool GalleryShowCategoryHeaders
    {
        get => _galleryShowCategoryHeaders;
        set => SetProperty(ref _galleryShowCategoryHeaders, value);
    }

    public bool IsDropDownOpen
    {
        get => _isDropDownOpen;
        set => SetProperty(ref _isDropDownOpen, value);
    }

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string? SecondaryCommandId
    {
        get => _secondaryCommandId;
        set => SetProperty(ref _secondaryCommandId, value);
    }

    public ICommand? SecondaryCommand
    {
        get => _secondaryCommand;
        set => SetProperty(ref _secondaryCommand, value);
    }

    public object? SecondaryCommandParameter
    {
        get => _secondaryCommandParameter;
        set => SetProperty(ref _secondaryCommandParameter, value);
    }

    public string? KeyTip
    {
        get => _keyTip;
        set => SetProperty(ref _keyTip, value);
    }

    public string? ScreenTip
    {
        get => _screenTip;
        set => SetProperty(ref _screenTip, value);
    }
}
