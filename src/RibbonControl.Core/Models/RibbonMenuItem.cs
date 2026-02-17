// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Models;

public class RibbonMenuItem : RibbonObservableObject, IRibbonMenuItemNode
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
    private string? _popupSectionId;
    private string? _popupSectionHeader;
    private int _popupSectionOrder;
    private RibbonPopupSectionLayout _popupSectionLayout = RibbonPopupSectionLayout.CommandList;
    private bool _isSeparator;
    private bool _showChevron;
    private bool _isSelected;
    private bool _showInRibbonPreview = true;
    private bool _showInPopup = true;
    private object? _content;
    private string? _keyTip;
    private string? _screenTip;
    private bool _isSubMenuOpen;
    private RibbonMenuItem? _parentMenuItem;

    public RibbonMenuItem()
    {
        SubMenuItems.CollectionChanged += OnSubMenuItemsCollectionChanged;
    }

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
        set
        {
            if (SetProperty(ref _icon, value))
            {
                RaisePropertyChanged(nameof(HasIcon));
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
                RaisePropertyChanged(nameof(HasIcon));
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
                RaisePropertyChanged(nameof(HasIcon));
                RaisePropertyChanged(nameof(HasIconPathData));
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
                RaisePropertyChanged(nameof(HasIcon));
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
        set
        {
            if (SetProperty(ref _overlay, value))
            {
                RaisePropertyChanged(nameof(HasOverlay));
            }
        }
    }

    public object? OverlayResourceKey
    {
        get => _overlayResourceKey;
        set
        {
            if (SetProperty(ref _overlayResourceKey, value))
            {
                RaisePropertyChanged(nameof(HasOverlay));
            }
        }
    }

    public string? OverlayPathData
    {
        get => _overlayPathData;
        set
        {
            if (SetProperty(ref _overlayPathData, value))
            {
                RaisePropertyChanged(nameof(HasOverlay));
            }
        }
    }

    public string? OverlayEmoji
    {
        get => _overlayEmoji;
        set
        {
            if (SetProperty(ref _overlayEmoji, value))
            {
                RaisePropertyChanged(nameof(HasOverlay));
            }
        }
    }

    public int? OverlayCount
    {
        get => _overlayCount;
        set
        {
            if (SetProperty(ref _overlayCount, value))
            {
                RaisePropertyChanged(nameof(HasOverlayCount));
            }
        }
    }

    public string? OverlayCountText
    {
        get => _overlayCountText;
        set
        {
            if (SetProperty(ref _overlayCountText, value))
            {
                RaisePropertyChanged(nameof(HasOverlayCount));
            }
        }
    }

    public bool ShowOverlayCountWhenZero
    {
        get => _showOverlayCountWhenZero;
        set
        {
            if (SetProperty(ref _showOverlayCountWhenZero, value))
            {
                RaisePropertyChanged(nameof(HasOverlayCount));
            }
        }
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

    public string? PopupSectionId
    {
        get => _popupSectionId;
        set
        {
            if (SetProperty(ref _popupSectionId, value))
            {
                RaisePropertyChanged(nameof(HasPopupSectionMetadata));
            }
        }
    }

    public string? PopupSectionHeader
    {
        get => _popupSectionHeader;
        set
        {
            if (SetProperty(ref _popupSectionHeader, value))
            {
                RaisePropertyChanged(nameof(HasPopupSectionMetadata));
            }
        }
    }

    public int PopupSectionOrder
    {
        get => _popupSectionOrder;
        set
        {
            if (SetProperty(ref _popupSectionOrder, value))
            {
                RaisePropertyChanged(nameof(HasPopupSectionMetadata));
            }
        }
    }

    public RibbonPopupSectionLayout PopupSectionLayout
    {
        get => _popupSectionLayout;
        set
        {
            if (SetProperty(ref _popupSectionLayout, value))
            {
                RaisePropertyChanged(nameof(HasPopupSectionMetadata));
            }
        }
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
        set
        {
            if (SetProperty(ref _showChevron, value))
            {
                RaisePropertyChanged(nameof(ShowsChevronGlyph));
            }
        }
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

    public ObservableCollection<RibbonMenuItem> SubMenuItems { get; } = [];

    IEnumerable<IRibbonMenuItemNode>? IRibbonMenuItemNode.SubMenuItems => SubMenuItems;

    public bool IsSubMenuOpen
    {
        get => _isSubMenuOpen;
        set
        {
            if (!SetProperty(ref _isSubMenuOpen, value))
            {
                return;
            }

            if (value)
            {
                _parentMenuItem?.CloseSubMenusExcept(this);
            }
            else
            {
                CloseChildSubMenus();
            }
        }
    }

    public bool IsCommand => !IsSeparator && !HasSubMenuItems;

    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

    public bool HasInputGestureText => !string.IsNullOrWhiteSpace(InputGestureText);

    public bool HasIcon =>
        Icon is not null ||
        IconResourceKey is not null ||
        !string.IsNullOrWhiteSpace(IconPathData) ||
        !string.IsNullOrWhiteSpace(IconEmoji);

    public bool HasIconPathData => !string.IsNullOrWhiteSpace(IconPathData);

    public bool HasOverlay =>
        Overlay is not null ||
        OverlayResourceKey is not null ||
        !string.IsNullOrWhiteSpace(OverlayPathData) ||
        !string.IsNullOrWhiteSpace(OverlayEmoji);

    public bool HasOverlayCount =>
        !string.IsNullOrWhiteSpace(OverlayCountText) ||
        (OverlayCount.HasValue && (ShowOverlayCountWhenZero || OverlayCount.Value != 0));

    public bool HasContent => Content is not null;

    public bool HasNoContent => Content is null;

    public bool HasPopupSectionMetadata =>
        !string.IsNullOrWhiteSpace(PopupSectionId) ||
        !string.IsNullOrWhiteSpace(PopupSectionHeader) ||
        PopupSectionOrder != 0 ||
        PopupSectionLayout != RibbonPopupSectionLayout.CommandList;

    public bool HasSubMenuItems => SubMenuItems.Count > 0;

    public bool ShowsChevronGlyph => ShowChevron || HasSubMenuItems;

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

    internal void CloseSubMenus()
    {
        IsSubMenuOpen = false;

        foreach (var child in SubMenuItems)
        {
            child.CloseSubMenus();
        }
    }

    private void CloseSubMenusExcept(RibbonMenuItem keepOpenChild)
    {
        foreach (var child in SubMenuItems)
        {
            if (ReferenceEquals(child, keepOpenChild))
            {
                continue;
            }

            child.CloseSubMenus();
        }
    }

    private void CloseChildSubMenus()
    {
        foreach (var child in SubMenuItems)
        {
            child.CloseSubMenus();
        }
    }

    private void OnSubMenuItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (SubMenuItems.Count == 0)
        {
            IsSubMenuOpen = false;
        }

        if (e.OldItems is not null)
        {
            foreach (var oldItem in e.OldItems.OfType<RibbonMenuItem>())
            {
                if (ReferenceEquals(oldItem._parentMenuItem, this))
                {
                    oldItem._parentMenuItem = null;
                }

                oldItem.CloseSubMenus();
            }
        }

        if (e.NewItems is not null)
        {
            foreach (var newItem in e.NewItems.OfType<RibbonMenuItem>())
            {
                newItem._parentMenuItem = this;
            }
        }

        RaisePropertyChanged(nameof(HasSubMenuItems));
        RaisePropertyChanged(nameof(IsCommand));
        RaisePropertyChanged(nameof(ShowsChevronGlyph));
    }
}
