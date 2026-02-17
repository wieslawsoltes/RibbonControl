// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.ViewModels;

public class RibbonMenuItemViewModel : RibbonObservableObject, IRibbonMenuItemNode
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
    private string? _description;
    private string? _inputGestureText;
    private string? _category;
    private bool _isSeparator;
    private bool _showChevron;
    private bool _isSelected;
    private bool _showInRibbonPreview = true;
    private bool _showInPopup = true;
    private object? _content;
    private string? _keyTip;
    private string? _screenTip;

    public RibbonMenuItemViewModel()
    {
        SubMenuItemsViewModel.CollectionChanged += (_, _) => RaisePropertyChanged(nameof(IsCommand));
    }

    public ObservableCollection<RibbonMenuItemViewModel> SubMenuItemsViewModel { get; } = [];

    IEnumerable<IRibbonMenuItemNode>? IRibbonMenuItemNode.SubMenuItems => SubMenuItemsViewModel;

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

    public string? Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string? InputGestureText
    {
        get => _inputGestureText;
        set => SetProperty(ref _inputGestureText, value);
    }

    public string? Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public bool IsSeparator
    {
        get => _isSeparator;
        set
        {
            if (SetProperty(ref _isSeparator, value))
            {
                RaisePropertyChanged(nameof(IsCommand));
            }
        }
    }

    public bool ShowChevron
    {
        get => _showChevron;
        set => SetProperty(ref _showChevron, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetProperty(ref _isSelected, value);
    }

    public bool ShowInRibbonPreview
    {
        get => _showInRibbonPreview;
        set => SetProperty(ref _showInRibbonPreview, value);
    }

    public bool ShowInPopup
    {
        get => _showInPopup;
        set => SetProperty(ref _showInPopup, value);
    }

    public object? Content
    {
        get => _content;
        set
        {
            if (SetProperty(ref _content, value))
            {
                RaisePropertyChanged(nameof(HasContent));
                RaisePropertyChanged(nameof(HasNoContent));
            }
        }
    }

    public bool IsCommand => !IsSeparator && SubMenuItemsViewModel.Count == 0;

    public bool HasContent => Content is not null;

    public bool HasNoContent => Content is null;

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
