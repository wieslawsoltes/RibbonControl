// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Input;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Models;

public class RibbonItem : RibbonObservableObject, IRibbonItemNode
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
    private RibbonItemSize _effectiveSize = RibbonItemSize.Large;
    private bool _hasAdaptiveEffectiveSizeOverride;
    private bool _isChecked;
    private IRibbonItemNode? _stateSyncSource;
    private INotifyPropertyChanged? _stateSyncSourceNotifier;
    private PropertyChangedEventHandler? _stateSyncSourceHandler;
    private bool _isSynchronizingStateSync;
    private bool _stateSyncToggleEnabled = true;
    private bool _stateSyncCheckedEnabled = true;
    private string? _selectedMenuItemId;
    private RibbonMenuItem? _selectedComboBoxMenuItem;
    private bool _isSynchronizingMenuSelection;
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

    public RibbonItem()
    {
        _effectiveSize = _size;
        MenuItems.CollectionChanged += OnMenuItemsCollectionChanged;
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

    public RibbonItemPrimitive Primitive
    {
        get => _primitive;
        set
        {
            if (SetProperty(ref _primitive, value))
            {
                RaisePrimitiveFlagsChanged();
                SyncSelectedMenuItem();
            }
        }
    }

    public RibbonSplitButtonMode SplitButtonMode
    {
        get => _splitButtonMode;
        set
        {
            if (SetProperty(ref _splitButtonMode, value))
            {
                RaiseSplitPresentationFlagsChanged();
            }
        }
    }

    public RibbonItemDisplayMode DisplayMode
    {
        get => _displayMode;
        set
        {
            if (SetProperty(ref _displayMode, value))
            {
                RaiseDisplayModeFlagsChanged();
            }
        }
    }

    public RibbonItemSize Size
    {
        get => _size;
        set
        {
            if (SetProperty(ref _size, value))
            {
                if (!_hasAdaptiveEffectiveSizeOverride)
                {
                    SetEffectiveSizeCore(value);
                }

                RaiseSizeFlagsChanged();
            }
        }
    }

    public RibbonItemLayoutDock LayoutDock
    {
        get => _layoutDock;
        set => SetProperty(ref _layoutDock, value);
    }

    public bool IsToggle
    {
        get => _isToggle;
        set
        {
            if (SetProperty(ref _isToggle, value))
            {
                RaiseToggleFlagsChanged();
                PushStateSyncValue(nameof(IsToggle), value);
            }
        }
    }

    public RibbonToggleGroupSelectionMode ToggleGroupSelectionMode
    {
        get => _toggleGroupSelectionMode;
        set
        {
            if (SetProperty(ref _toggleGroupSelectionMode, value))
            {
                RaiseToggleGroupFlagsChanged();
                if (UsesSingleMenuSelection)
                {
                    SyncSelectedMenuItem();
                }
            }
        }
    }

    public int ToggleGroupColumns
    {
        get => _toggleGroupColumns;
        set => SetProperty(ref _toggleGroupColumns, Math.Max(1, value));
    }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (SetProperty(ref _isChecked, value))
            {
                PushStateSyncValue(nameof(IsChecked), value);
            }
        }
    }

    public RibbonItemSize EffectiveSize
    {
        get => _effectiveSize;
        private set => SetEffectiveSizeCore(value);
    }

    public object? Content
    {
        get => _content;
        set
        {
            if (SetProperty(ref _content, value))
            {
                RaisePropertyChanged(nameof(HasCustomContent));
            }
        }
    }

    public object? PopupContent
    {
        get => _popupContent;
        set
        {
            if (SetProperty(ref _popupContent, value))
            {
                RaisePropertyChanged(nameof(HasPopupContent));
                RaisePropertyChanged(nameof(HasSplitPopup));
                RaiseSplitPresentationFlagsChanged();
            }
        }
    }

    public string? PopupTitle
    {
        get => _popupTitle;
        set
        {
            if (SetProperty(ref _popupTitle, value))
            {
                RaisePropertyChanged(nameof(HasPopupTitle));
            }
        }
    }

    public object? PopupFooterContent
    {
        get => _popupFooterContent;
        set
        {
            if (SetProperty(ref _popupFooterContent, value))
            {
                RaisePropertyChanged(nameof(HasPopupFooterContent));
            }
        }
    }

    public double PopupMinWidth
    {
        get => _popupMinWidth;
        set => SetProperty(ref _popupMinWidth, Math.Max(0, value));
    }

    public double PopupMaxHeight
    {
        get => _popupMaxHeight;
        set => SetProperty(ref _popupMaxHeight, Math.Max(0, value));
    }

    public int GalleryPreviewMaxItems
    {
        get => _galleryPreviewMaxItems;
        set
        {
            var normalized = Math.Max(0, value);
            if (!SetProperty(ref _galleryPreviewMaxItems, normalized))
            {
                return;
            }

            RaisePropertyChanged(nameof(RibbonPreviewMenuItems));
            RaisePropertyChanged(nameof(HasRibbonPreviewMenuItems));
        }
    }

    public bool GalleryShowCategoryHeaders
    {
        get => _galleryShowCategoryHeaders;
        set
        {
            if (!SetProperty(ref _galleryShowCategoryHeaders, value))
            {
                return;
            }

            RaisePropertyChanged(nameof(ShowPopupMenuCategories));
            RaisePropertyChanged(nameof(ShowFlatCategorizedPopupMenuItems));
        }
    }

    public bool IsDropDownOpen
    {
        get => _isDropDownOpen;
        set
        {
            if (SetProperty(ref _isDropDownOpen, value))
            {
                if (!value)
                {
                    CloseAllSubMenus();
                }

                RaiseDropDownFlagsChanged();
            }
        }
    }

    public string? Description
    {
        get => _description;
        set
        {
            if (SetProperty(ref _description, value))
            {
                RaisePropertyChanged(nameof(HasDescription));
            }
        }
    }

    public string? SecondaryCommandId
    {
        get => _secondaryCommandId;
        set
        {
            if (SetProperty(ref _secondaryCommandId, value))
            {
                RaisePropertyChanged(nameof(HasSecondaryCommand));
                RaiseSplitPresentationFlagsChanged();
            }
        }
    }

    public ICommand? SecondaryCommand
    {
        get => _secondaryCommand;
        set
        {
            if (SetProperty(ref _secondaryCommand, value))
            {
                RaisePropertyChanged(nameof(HasSecondaryCommand));
                RaiseSplitPresentationFlagsChanged();
            }
        }
    }

    public object? SecondaryCommandParameter
    {
        get => _secondaryCommandParameter;
        set => SetProperty(ref _secondaryCommandParameter, value);
    }

    public string? SelectedMenuItemId
    {
        get => _selectedMenuItemId;
        set
        {
            if (!SetProperty(ref _selectedMenuItemId, value))
            {
                return;
            }

            SyncSelectedMenuItem(preserveSelectedMenuItemId: true);
        }
    }

    public ObservableCollection<RibbonMenuItem> MenuItems { get; } = [];

    public ObservableCollection<RibbonItem> Items { get; } = [];

    IEnumerable<IRibbonMenuItemNode>? IRibbonItemNode.MenuItems => MenuItems;

    IEnumerable<IRibbonItemNode>? IRibbonItemNode.Items => Items;

    public bool IsButtonPrimitive => Primitive == RibbonItemPrimitive.Button;

    public bool IsSplitButtonPrimitive => Primitive == RibbonItemPrimitive.SplitButton;

    public bool IsPasteSplitButtonPrimitive => Primitive == RibbonItemPrimitive.PasteSplitButton;

    public bool IsSideBySideSplitButtonPrimitive =>
        IsSplitButtonPrimitive && SplitButtonMode == RibbonSplitButtonMode.SideBySide;

    public bool IsSideBySideLargeSplitButtonPrimitive =>
        IsSideBySideSplitButtonPrimitive && IsEffectiveLargeSize && IsAutoDisplayMode;

    public bool IsSideBySideSmallSplitButtonPrimitive =>
        IsSideBySideSplitButtonPrimitive && IsEffectiveSmallSize && IsAutoDisplayMode;

    public bool IsSideBySideLargeIconOnlySplitButtonPrimitive =>
        IsSideBySideSplitButtonPrimitive && IsEffectiveLargeSize && IsIconOnlyDisplayMode;

    public bool IsSideBySideSmallIconOnlySplitButtonPrimitive =>
        IsSideBySideSplitButtonPrimitive && IsEffectiveSmallSize && IsIconOnlyDisplayMode;

    public bool IsStackedSplitButtonPrimitive =>
        IsPasteSplitButtonPrimitive || (IsSplitButtonPrimitive && SplitButtonMode == RibbonSplitButtonMode.Stacked);

    public bool IsMenuButtonPrimitive => Primitive == RibbonItemPrimitive.MenuButton;

    public bool IsGalleryPrimitive => Primitive == RibbonItemPrimitive.Gallery;

    public bool IsCustomPrimitive => Primitive == RibbonItemPrimitive.Custom;

    public bool IsToggleButtonPrimitive => Primitive == RibbonItemPrimitive.ToggleButton || (IsButtonPrimitive && IsToggle);

    public bool IsComboBoxPrimitive => Primitive == RibbonItemPrimitive.ComboBox;

    public bool IsToggleGroupPrimitive => Primitive == RibbonItemPrimitive.ToggleGroup;

    public bool IsGroupPrimitive => Primitive == RibbonItemPrimitive.Group;

    public bool IsRowPrimitive => Primitive == RibbonItemPrimitive.Row;

    public bool IsToggleGroupSingleSelectionMode =>
        IsToggleGroupPrimitive && ToggleGroupSelectionMode == RibbonToggleGroupSelectionMode.Single;

    public bool IsToggleGroupMultipleSelectionMode =>
        IsToggleGroupPrimitive && ToggleGroupSelectionMode == RibbonToggleGroupSelectionMode.Multiple;

    public bool IsAutoDisplayMode => DisplayMode == RibbonItemDisplayMode.Auto;

    public bool IsIconOnlyDisplayMode => DisplayMode == RibbonItemDisplayMode.IconOnly;

    public bool IsLargeSize => Size == RibbonItemSize.Large;

    public bool IsSmallSize => Size == RibbonItemSize.Small;

    public bool IsLargeButtonPrimitive => IsButtonPrimitive && IsLargeSize;

    public bool IsSmallButtonPrimitive => IsButtonPrimitive && IsSmallSize;

    public bool IsEffectiveLargeSize => EffectiveSize == RibbonItemSize.Large;

    public bool IsEffectiveSmallSize => EffectiveSize == RibbonItemSize.Small;

    public bool IsEffectiveLargeButtonPrimitive => IsButtonPrimitive && !IsToggle && IsEffectiveLargeSize && IsAutoDisplayMode;

    public bool IsEffectiveSmallButtonPrimitive => IsButtonPrimitive && !IsToggle && IsEffectiveSmallSize && IsAutoDisplayMode;

    public bool IsEffectiveLargeIconOnlyButtonPrimitive => IsButtonPrimitive && !IsToggle && IsEffectiveLargeSize && IsIconOnlyDisplayMode;

    public bool IsEffectiveSmallIconOnlyButtonPrimitive => IsButtonPrimitive && !IsToggle && IsEffectiveSmallSize && IsIconOnlyDisplayMode;

    public bool IsEffectiveLargeToggleButtonPrimitive => IsToggleButtonPrimitive && IsEffectiveLargeSize && IsAutoDisplayMode;

    public bool IsEffectiveSmallToggleButtonPrimitive => IsToggleButtonPrimitive && IsEffectiveSmallSize && IsAutoDisplayMode;

    public bool IsEffectiveLargeIconOnlyToggleButtonPrimitive => IsToggleButtonPrimitive && IsEffectiveLargeSize && IsIconOnlyDisplayMode;

    public bool IsEffectiveSmallIconOnlyToggleButtonPrimitive => IsToggleButtonPrimitive && IsEffectiveSmallSize && IsIconOnlyDisplayMode;

    public bool IsEffectiveLargeComboBoxPrimitive => IsComboBoxPrimitive && IsEffectiveLargeSize;

    public bool IsEffectiveSmallComboBoxPrimitive => IsComboBoxPrimitive && IsEffectiveSmallSize;

    public bool IsEffectiveLargeToggleGroupPrimitive => IsToggleGroupPrimitive && IsEffectiveLargeSize;

    public bool IsEffectiveSmallToggleGroupPrimitive => IsToggleGroupPrimitive && IsEffectiveSmallSize;

    public bool IsSplitDropDownOpen
    {
        get => (IsSplitButtonPrimitive || IsPasteSplitButtonPrimitive) && IsDropDownOpen;
        set
        {
            if ((IsSplitButtonPrimitive || IsPasteSplitButtonPrimitive) && IsDropDownOpen != value)
            {
                IsDropDownOpen = value;
            }
        }
    }

    public bool IsSideBySideSplitDropDownOpen
    {
        get => IsSideBySideSplitButtonPrimitive && IsSplitDropDownOpen;
        set
        {
            if (IsSideBySideSplitButtonPrimitive && IsSplitDropDownOpen != value)
            {
                IsSplitDropDownOpen = value;
            }
        }
    }

    public bool IsStackedSplitDropDownOpen
    {
        get => IsStackedSplitButtonPrimitive && IsSplitDropDownOpen;
        set
        {
            if (IsStackedSplitButtonPrimitive && IsSplitDropDownOpen != value)
            {
                IsSplitDropDownOpen = value;
            }
        }
    }

    public bool IsMenuDropDownOpen
    {
        get => IsMenuButtonPrimitive && IsDropDownOpen;
        set
        {
            if (IsMenuButtonPrimitive && IsDropDownOpen != value)
            {
                IsDropDownOpen = value;
            }
        }
    }

    public bool IsGalleryDropDownOpen
    {
        get => IsGalleryPrimitive && IsDropDownOpen;
        set
        {
            if (IsGalleryPrimitive && IsDropDownOpen != value)
            {
                IsDropDownOpen = value;
            }
        }
    }

    public bool HasMenuItems => MenuItems.Count > 0;

    public IReadOnlyList<RibbonMenuItem> ToggleGroupMenuItems
        => BuildToggleGroupMenuItems();

    public bool HasToggleGroupMenuItems => ToggleGroupMenuItems.Count > 0;

    public IReadOnlyList<RibbonMenuItem> ComboBoxMenuItems
        => BuildComboBoxMenuItems();

    public bool HasComboBoxMenuItems => ComboBoxMenuItems.Count > 0;

    public RibbonMenuItem? SelectedComboBoxMenuItem
    {
        get => _selectedComboBoxMenuItem;
        set => SetSelectedComboBoxMenuItem(value, synchronizeMenuFlags: true);
    }

    public IReadOnlyList<RibbonMenuItem> RibbonPreviewMenuItems
        => BuildRibbonPreviewMenuItems();

    public IReadOnlyList<RibbonMenuItem> PopupMenuItems
        => MenuItems.Where(menuItem => menuItem.ShowInPopup).ToList();

    public IReadOnlyList<RibbonPopupMenuSection> PopupMenuSections
        => BuildPopupMenuSections(PopupMenuItems);

    public IReadOnlyList<RibbonMenuItem> CategorizedPopupMenuItems
        => PopupMenuItems.Where(menuItem => !string.IsNullOrWhiteSpace(menuItem.Category)).ToList();

    public IReadOnlyList<RibbonMenuItem> UncategorizedPopupMenuItems
        => PopupMenuItems.Where(menuItem => string.IsNullOrWhiteSpace(menuItem.Category)).ToList();

    public IReadOnlyList<RibbonGalleryCategory> PopupMenuCategories
        => BuildPopupMenuCategories(CategorizedPopupMenuItems);

    public bool HasRibbonPreviewMenuItems => RibbonPreviewMenuItems.Count > 0;

    public bool HasPopupMenuItems => PopupMenuItems.Count > 0;

    public bool HasPopupMenuSections => PopupMenuSections.Count > 0;

    public bool HasExplicitPopupMenuSections => HasExplicitPopupMenuSectionsCore(PopupMenuItems);

    public bool HasPopupMenuCategories => PopupMenuCategories.Count > 0;

    public bool HasUncategorizedPopupMenuItems => UncategorizedPopupMenuItems.Count > 0;

    public bool ShowStructuredPopupMenuSections => HasExplicitPopupMenuSections && HasPopupMenuSections;

    public bool ShowPopupMenuCategories => !HasExplicitPopupMenuSections && GalleryShowCategoryHeaders && HasPopupMenuCategories;

    public bool ShowFlatCategorizedPopupMenuItems =>
        !HasExplicitPopupMenuSections && !GalleryShowCategoryHeaders && HasPopupMenuCategories;

    public bool ShowUncategorizedPopupMenuItems =>
        !HasExplicitPopupMenuSections && HasPopupMenuCategories && HasUncategorizedPopupMenuItems;

    public bool ShowLegacyPopupMenuItems => !HasExplicitPopupMenuSections && !HasPopupMenuCategories && HasPopupMenuItems;

    public RibbonMenuItem? SelectedGalleryMenuItem
        => IsGalleryPrimitive
            ? MenuItems.FirstOrDefault(menuItem => menuItem.IsSelected)
            : null;

    public bool HasSelectedGalleryMenuItem => SelectedGalleryMenuItem is not null;

    public bool HasSelectedComboBoxMenuItem => SelectedComboBoxMenuItem is not null;

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

    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

    public bool HasCustomContent => Content is not null;

    public bool HasPopupContent => PopupContent is not null;

    public bool HasPopupTitle => !string.IsNullOrWhiteSpace(PopupTitle);

    public bool HasPopupFooterContent => PopupFooterContent is not null;

    public bool HasSecondaryCommand => SecondaryCommand is not null || !string.IsNullOrWhiteSpace(SecondaryCommandId);

    public bool HasSplitPopup => HasPopupContent || HasPopupMenuItems;

    public bool IsSplitDropDownToggleVisible => IsSideBySideSplitButtonPrimitive && HasSplitPopup;

    public bool IsSplitSecondaryCommandVisible => IsSideBySideSplitButtonPrimitive && !HasSplitPopup && HasSecondaryCommand;

    public bool IsSplitFallbackToggleVisible => IsSideBySideSplitButtonPrimitive && !HasSplitPopup && !HasSecondaryCommand;

    public bool IsStackedSplitBottomSplitVisible =>
        IsStackedSplitButtonPrimitive && !IsPasteSplitButtonPrimitive && HasSecondaryCommand && HasSplitPopup;

    public bool IsStackedSplitBottomCommandOnlyVisible =>
        IsStackedSplitButtonPrimitive && !IsPasteSplitButtonPrimitive && HasSecondaryCommand && !HasSplitPopup;

    public bool IsStackedSplitBottomDropDownOnlyVisible =>
        IsStackedSplitButtonPrimitive && (IsPasteSplitButtonPrimitive || !HasSecondaryCommand) && HasSplitPopup;

    public bool IsStackedSplitBottomFallbackVisible =>
        IsStackedSplitButtonPrimitive && (IsPasteSplitButtonPrimitive || !HasSecondaryCommand) && !HasSplitPopup;

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

    private bool UsesSingleMenuSelection =>
        IsGalleryPrimitive ||
        IsComboBoxPrimitive ||
        IsToggleGroupSingleSelectionMode;

    internal IRibbonItemNode? StateSyncSource => _stateSyncSource;

    internal bool IsStateSyncToggleEnabled => _stateSyncToggleEnabled;

    internal bool IsStateSyncCheckedEnabled => _stateSyncCheckedEnabled;

    internal void BindStateSyncSource(
        IRibbonItemNode source,
        bool synchronizeToggle = true,
        bool synchronizeChecked = true)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (ReferenceEquals(source, this))
        {
            ClearStateSyncSource();
            return;
        }

        if (ReferenceEquals(_stateSyncSource, source) &&
            _stateSyncToggleEnabled == synchronizeToggle &&
            _stateSyncCheckedEnabled == synchronizeChecked)
        {
            PullStateSyncValuesFromSource(updateToggle: true, updateChecked: true);
            return;
        }

        ClearStateSyncSource();
        _stateSyncSource = source;
        _stateSyncToggleEnabled = synchronizeToggle;
        _stateSyncCheckedEnabled = synchronizeChecked;
        PullStateSyncValuesFromSource(updateToggle: true, updateChecked: true);

        if (source is INotifyPropertyChanged notifier)
        {
            _stateSyncSourceNotifier = notifier;
            _stateSyncSourceHandler = OnStateSyncSourcePropertyChanged;
            notifier.PropertyChanged += _stateSyncSourceHandler;
        }
    }

    internal void ClearStateSyncSource()
    {
        if (_stateSyncSourceNotifier is not null && _stateSyncSourceHandler is not null)
        {
            _stateSyncSourceNotifier.PropertyChanged -= _stateSyncSourceHandler;
        }

        _stateSyncSource = null;
        _stateSyncSourceNotifier = null;
        _stateSyncSourceHandler = null;
        _isSynchronizingStateSync = false;
        _stateSyncToggleEnabled = true;
        _stateSyncCheckedEnabled = true;
    }

    internal void SetAdaptiveEffectiveSize(RibbonItemSize? size)
    {
        if (size is null)
        {
            _hasAdaptiveEffectiveSizeOverride = false;
            EffectiveSize = Size;
            return;
        }

        _hasAdaptiveEffectiveSizeOverride = true;
        EffectiveSize = size.Value;
    }

    private void CloseAllSubMenus()
    {
        foreach (var menuItem in MenuItems)
        {
            menuItem.CloseSubMenus();
        }
    }

    private void OnMenuItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RibbonMenuItem? newSelection = null;

        if (e.OldItems is not null)
        {
            foreach (var item in e.OldItems.OfType<RibbonMenuItem>())
            {
                item.PropertyChanged -= OnMenuItemPropertyChanged;
            }
        }

        if (e.NewItems is not null)
        {
            foreach (var item in e.NewItems.OfType<RibbonMenuItem>())
            {
                item.PropertyChanged += OnMenuItemPropertyChanged;
                if (item.IsSelected)
                {
                    newSelection = item;
                }
            }
        }

        if (UsesSingleMenuSelection)
        {
            EnsureSingleSelectionConsistency(newSelection);
            SyncSelectedMenuItem(newSelection);
        }
        RaiseMenuCollectionsChanged();
    }

    private void OnMenuItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(RibbonMenuItem.IsSubMenuOpen) &&
            sender is RibbonMenuItem openedMenuItem &&
            openedMenuItem.IsSubMenuOpen)
        {
            foreach (var sibling in MenuItems)
            {
                if (ReferenceEquals(sibling, openedMenuItem) || !sibling.IsSubMenuOpen)
                {
                    continue;
                }

                sibling.CloseSubMenus();
            }
        }

        if (e.PropertyName == nameof(RibbonMenuItem.IsSelected))
        {
            if (_isSynchronizingMenuSelection)
            {
                return;
            }

            if (sender is RibbonMenuItem selectedMenuItem && selectedMenuItem.IsSelected)
            {
                if (UsesSingleMenuSelection)
                {
                    EnsureSingleSelectionConsistency(selectedMenuItem);
                    SyncSelectedMenuItem(selectedMenuItem);
                }
            }
            else
            {
                if (UsesSingleMenuSelection)
                {
                    SyncSelectedMenuItem();
                }
            }
        }

        if (e.PropertyName is not nameof(RibbonMenuItem.ShowInRibbonPreview)
            and not nameof(RibbonMenuItem.ShowInPopup)
            and not nameof(RibbonMenuItem.Category)
            and not nameof(RibbonMenuItem.PopupSectionId)
            and not nameof(RibbonMenuItem.PopupSectionHeader)
            and not nameof(RibbonMenuItem.PopupSectionOrder)
            and not nameof(RibbonMenuItem.PopupSectionLayout)
            and not nameof(RibbonMenuItem.IsSelected))
        {
            return;
        }

        RaiseMenuCollectionsChanged();
    }

    private void EnsureSingleSelectionConsistency(RibbonMenuItem? selected = null)
    {
        if (!UsesSingleMenuSelection)
        {
            return;
        }

        var selectionOwner = selected;
        if (selectionOwner is null)
        {
            selectionOwner = MenuItems.FirstOrDefault(menuItem => menuItem.IsSelected);
        }

        if (selectionOwner is null)
        {
            return;
        }

        foreach (var menuItem in MenuItems)
        {
            if (ReferenceEquals(menuItem, selectionOwner) || !menuItem.IsSelected)
            {
                continue;
            }

            menuItem.IsSelected = false;
        }
    }

    private void SyncSelectedMenuItem(
        RibbonMenuItem? preferredSelection = null,
        bool preserveSelectedMenuItemId = false)
    {
        RibbonMenuItem? selected = null;

        if (preserveSelectedMenuItemId && !string.IsNullOrWhiteSpace(_selectedMenuItemId))
        {
            selected = MenuItems.FirstOrDefault(menuItem =>
                string.Equals(menuItem.Id, _selectedMenuItemId, StringComparison.Ordinal));
        }

        if (selected is null)
        {
            selected = preferredSelection is not null && preferredSelection.IsSelected
                ? preferredSelection
                : MenuItems.FirstOrDefault(menuItem => menuItem.IsSelected);
        }

        if (selected is null && !string.IsNullOrWhiteSpace(_selectedMenuItemId))
        {
            selected = MenuItems.FirstOrDefault(menuItem =>
                string.Equals(menuItem.Id, _selectedMenuItemId, StringComparison.Ordinal));
        }

        if (selected is null &&
            IsComboBoxPrimitive &&
            string.IsNullOrWhiteSpace(_selectedMenuItemId))
        {
            selected = BuildComboBoxMenuItems().FirstOrDefault();
        }

        SetSelectedComboBoxMenuItem(
            selected,
            synchronizeMenuFlags: true,
            preserveSelectedMenuItemId: preserveSelectedMenuItemId);
    }

    private void SetSelectedComboBoxMenuItem(
        RibbonMenuItem? selected,
        bool synchronizeMenuFlags,
        bool preserveSelectedMenuItemId = false)
    {
        _isSynchronizingMenuSelection = true;
        try
        {
            if (synchronizeMenuFlags)
            {
                SynchronizeMenuSelectionFlags(selected);
            }

            var selectedId = selected?.Id;
            if (selectedId is null &&
                (preserveSelectedMenuItemId || !string.IsNullOrWhiteSpace(_selectedMenuItemId)))
            {
                selectedId = _selectedMenuItemId;
            }

            var selectedItemChanged = SetProperty(ref _selectedComboBoxMenuItem, selected, nameof(SelectedComboBoxMenuItem));
            var selectedIdChanged = SetProperty(ref _selectedMenuItemId, selectedId, nameof(SelectedMenuItemId));

            if (!selectedItemChanged && !selectedIdChanged)
            {
                return;
            }

            RaisePropertyChanged(nameof(HasSelectedComboBoxMenuItem));
            RaisePropertyChanged(nameof(SelectedGalleryMenuItem));
            RaisePropertyChanged(nameof(HasSelectedGalleryMenuItem));
        }
        finally
        {
            _isSynchronizingMenuSelection = false;
        }
    }

    private void SynchronizeMenuSelectionFlags(RibbonMenuItem? selected)
    {
        foreach (var menuItem in MenuItems)
        {
            var shouldSelect = selected is not null && ReferenceEquals(menuItem, selected);
            if (menuItem.IsSelected != shouldSelect)
            {
                menuItem.IsSelected = shouldSelect;
            }
        }
    }

    private void RaiseMenuCollectionsChanged()
    {
        RaisePropertyChanged(nameof(HasMenuItems));
        RaisePropertyChanged(nameof(ToggleGroupMenuItems));
        RaisePropertyChanged(nameof(HasToggleGroupMenuItems));
        RaisePropertyChanged(nameof(ComboBoxMenuItems));
        RaisePropertyChanged(nameof(HasComboBoxMenuItems));
        RaisePropertyChanged(nameof(SelectedComboBoxMenuItem));
        RaisePropertyChanged(nameof(SelectedMenuItemId));
        RaisePropertyChanged(nameof(HasSelectedComboBoxMenuItem));
        RaisePropertyChanged(nameof(RibbonPreviewMenuItems));
        RaisePropertyChanged(nameof(PopupMenuItems));
        RaisePropertyChanged(nameof(PopupMenuSections));
        RaisePropertyChanged(nameof(CategorizedPopupMenuItems));
        RaisePropertyChanged(nameof(UncategorizedPopupMenuItems));
        RaisePropertyChanged(nameof(PopupMenuCategories));
        RaisePropertyChanged(nameof(HasRibbonPreviewMenuItems));
        RaisePropertyChanged(nameof(HasPopupMenuItems));
        RaisePropertyChanged(nameof(HasPopupMenuSections));
        RaisePropertyChanged(nameof(HasExplicitPopupMenuSections));
        RaisePropertyChanged(nameof(HasPopupMenuCategories));
        RaisePropertyChanged(nameof(HasUncategorizedPopupMenuItems));
        RaisePropertyChanged(nameof(ShowStructuredPopupMenuSections));
        RaisePropertyChanged(nameof(ShowPopupMenuCategories));
        RaisePropertyChanged(nameof(ShowFlatCategorizedPopupMenuItems));
        RaisePropertyChanged(nameof(ShowUncategorizedPopupMenuItems));
        RaisePropertyChanged(nameof(ShowLegacyPopupMenuItems));
        RaisePropertyChanged(nameof(SelectedGalleryMenuItem));
        RaisePropertyChanged(nameof(HasSelectedGalleryMenuItem));
        RaisePropertyChanged(nameof(HasSplitPopup));
        RaiseSplitPresentationFlagsChanged();
    }

    private void RaisePrimitiveFlagsChanged()
    {
        RaisePropertyChanged(nameof(IsButtonPrimitive));
        RaisePropertyChanged(nameof(IsSplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsPasteSplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsSideBySideSplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsSideBySideLargeSplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsSideBySideSmallSplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsSideBySideLargeIconOnlySplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsSideBySideSmallIconOnlySplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsStackedSplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsMenuButtonPrimitive));
        RaisePropertyChanged(nameof(IsGalleryPrimitive));
        RaisePropertyChanged(nameof(IsCustomPrimitive));
        RaisePropertyChanged(nameof(IsToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsComboBoxPrimitive));
        RaisePropertyChanged(nameof(IsToggleGroupPrimitive));
        RaisePropertyChanged(nameof(IsGroupPrimitive));
        RaisePropertyChanged(nameof(IsRowPrimitive));
        RaisePropertyChanged(nameof(IsToggleGroupSingleSelectionMode));
        RaisePropertyChanged(nameof(IsToggleGroupMultipleSelectionMode));
        RaisePropertyChanged(nameof(IsLargeButtonPrimitive));
        RaisePropertyChanged(nameof(IsSmallButtonPrimitive));
        RaisePropertyChanged(nameof(IsAutoDisplayMode));
        RaisePropertyChanged(nameof(IsIconOnlyDisplayMode));
        RaisePropertyChanged(nameof(IsEffectiveLargeButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeIconOnlyButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallIconOnlyButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeIconOnlyToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallIconOnlyToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeComboBoxPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallComboBoxPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeToggleGroupPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallToggleGroupPrimitive));
        RaisePropertyChanged(nameof(ToggleGroupMenuItems));
        RaisePropertyChanged(nameof(HasToggleGroupMenuItems));
        RaisePropertyChanged(nameof(ComboBoxMenuItems));
        RaisePropertyChanged(nameof(HasComboBoxMenuItems));
        RaisePropertyChanged(nameof(SelectedComboBoxMenuItem));
        RaisePropertyChanged(nameof(SelectedMenuItemId));
        RaisePropertyChanged(nameof(HasSelectedComboBoxMenuItem));
        RaisePropertyChanged(nameof(RibbonPreviewMenuItems));
        RaisePropertyChanged(nameof(PopupMenuSections));
        RaisePropertyChanged(nameof(HasPopupMenuSections));
        RaisePropertyChanged(nameof(ShowStructuredPopupMenuSections));
        RaisePropertyChanged(nameof(HasRibbonPreviewMenuItems));
        if (UsesSingleMenuSelection)
        {
            EnsureSingleSelectionConsistency();
            SyncSelectedMenuItem();
        }
        RaiseSplitPresentationFlagsChanged();
        RaiseDropDownFlagsChanged();
    }

    private void RaiseSizeFlagsChanged()
    {
        RaisePropertyChanged(nameof(IsLargeSize));
        RaisePropertyChanged(nameof(IsSmallSize));
        RaisePropertyChanged(nameof(IsLargeButtonPrimitive));
        RaisePropertyChanged(nameof(IsSmallButtonPrimitive));
        RaisePropertyChanged(nameof(IsAutoDisplayMode));
        RaisePropertyChanged(nameof(IsIconOnlyDisplayMode));
        RaisePropertyChanged(nameof(IsEffectiveLargeSize));
        RaisePropertyChanged(nameof(IsEffectiveSmallSize));
        RaisePropertyChanged(nameof(IsEffectiveLargeButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeIconOnlyButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallIconOnlyButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeIconOnlyToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallIconOnlyToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeComboBoxPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallComboBoxPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeToggleGroupPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallToggleGroupPrimitive));
        RaiseSplitPresentationFlagsChanged();
    }

    private void RaiseDisplayModeFlagsChanged()
    {
        RaisePropertyChanged(nameof(IsAutoDisplayMode));
        RaisePropertyChanged(nameof(IsIconOnlyDisplayMode));
        RaisePropertyChanged(nameof(IsEffectiveLargeButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeIconOnlyButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallIconOnlyButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeIconOnlyToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallIconOnlyToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeToggleGroupPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallToggleGroupPrimitive));
        RaiseSplitPresentationFlagsChanged();
    }

    private void RaiseToggleFlagsChanged()
    {
        RaisePropertyChanged(nameof(IsToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeIconOnlyButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallIconOnlyButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeIconOnlyToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallIconOnlyToggleButtonPrimitive));
    }

    private void RaiseToggleGroupFlagsChanged()
    {
        RaisePropertyChanged(nameof(IsToggleGroupSingleSelectionMode));
        RaisePropertyChanged(nameof(IsToggleGroupMultipleSelectionMode));
        RaisePropertyChanged(nameof(ToggleGroupMenuItems));
        RaisePropertyChanged(nameof(HasToggleGroupMenuItems));
        if (UsesSingleMenuSelection)
        {
            EnsureSingleSelectionConsistency();
            SyncSelectedMenuItem();
        }
    }

    private void RaiseSplitPresentationFlagsChanged()
    {
        RaisePropertyChanged(nameof(IsSideBySideSplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsSideBySideLargeSplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsSideBySideSmallSplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsSideBySideLargeIconOnlySplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsSideBySideSmallIconOnlySplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsStackedSplitButtonPrimitive));
        RaisePropertyChanged(nameof(IsSideBySideSplitDropDownOpen));
        RaisePropertyChanged(nameof(IsStackedSplitDropDownOpen));
        RaisePropertyChanged(nameof(IsSplitDropDownToggleVisible));
        RaisePropertyChanged(nameof(IsSplitSecondaryCommandVisible));
        RaisePropertyChanged(nameof(IsSplitFallbackToggleVisible));
        RaisePropertyChanged(nameof(IsStackedSplitBottomSplitVisible));
        RaisePropertyChanged(nameof(IsStackedSplitBottomCommandOnlyVisible));
        RaisePropertyChanged(nameof(IsStackedSplitBottomDropDownOnlyVisible));
        RaisePropertyChanged(nameof(IsStackedSplitBottomFallbackVisible));
    }

    private IReadOnlyList<RibbonMenuItem> BuildComboBoxMenuItems()
    {
        return MenuItems
            .Where(menuItem => menuItem.IsVisible && !menuItem.IsSeparator)
            .OrderBy(menuItem => menuItem.Order)
            .ThenBy(menuItem => menuItem.Id, StringComparer.Ordinal)
            .ToList();
    }

    private IReadOnlyList<RibbonMenuItem> BuildToggleGroupMenuItems()
    {
        return MenuItems
            .Where(menuItem => menuItem.IsVisible && !menuItem.IsSeparator)
            .OrderBy(menuItem => menuItem.Order)
            .ThenBy(menuItem => menuItem.Id, StringComparer.Ordinal)
            .ToList();
    }

    private IReadOnlyList<RibbonMenuItem> BuildRibbonPreviewMenuItems()
    {
        var previewItems = MenuItems
            .Where(menuItem => menuItem.ShowInRibbonPreview)
            .ToList();

        if (!IsGalleryPrimitive || GalleryPreviewMaxItems <= 0 || previewItems.Count <= GalleryPreviewMaxItems)
        {
            return previewItems;
        }

        return previewItems.Take(GalleryPreviewMaxItems).ToList();
    }

    private IReadOnlyList<RibbonPopupMenuSection> BuildPopupMenuSections(IReadOnlyList<RibbonMenuItem> popupItems)
    {
        if (popupItems.Count == 0)
        {
            return [];
        }

        if (!HasExplicitPopupMenuSectionsCore(popupItems))
        {
            var defaultLayout = IsGalleryPrimitive
                ? RibbonPopupSectionLayout.GalleryWrap
                : RibbonPopupSectionLayout.CommandList;

            return
            [
                new RibbonPopupMenuSection(
                    "default",
                    header: null,
                    order: 0,
                    layout: defaultLayout,
                    showSeparator: false,
                    items: popupItems),
            ];
        }

        var sectionOrder = new List<string>();
        var sections = new Dictionary<string, PopupSectionAccumulator>(StringComparer.Ordinal);

        foreach (var menuItem in popupItems)
        {
            var sectionId = NormalizePopupSectionId(menuItem.PopupSectionId);
            if (!sections.TryGetValue(sectionId, out var section))
            {
                section = new PopupSectionAccumulator(
                    sectionId,
                    menuItem.PopupSectionHeader,
                    menuItem.PopupSectionOrder,
                    menuItem.PopupSectionLayout,
                    menuItem.Order);
                sections[sectionId] = section;
                sectionOrder.Add(sectionId);
            }
            else
            {
                section.TryApply(menuItem);
            }

            section.Items.Add(menuItem);
        }

        var ordered = sectionOrder
            .Select(sectionId => sections[sectionId])
            .OrderBy(section => section.Order)
            .ThenBy(section => section.FirstItemOrder)
            .ThenBy(section => section.Id, StringComparer.Ordinal)
            .ToList();

        var result = new List<RibbonPopupMenuSection>(ordered.Count);
        for (var index = 0; index < ordered.Count; index++)
        {
            var section = ordered[index];
            result.Add(new RibbonPopupMenuSection(
                section.Id,
                section.Header,
                section.Order,
                section.Layout,
                showSeparator: index > 0,
                items: section.Items));
        }

        return result;
    }

    private static bool HasExplicitPopupMenuSectionsCore(IReadOnlyList<RibbonMenuItem> popupItems)
    {
        foreach (var menuItem in popupItems)
        {
            if (menuItem.HasPopupSectionMetadata)
            {
                return true;
            }
        }

        return false;
    }

    private static IReadOnlyList<RibbonGalleryCategory> BuildPopupMenuCategories(IEnumerable<RibbonMenuItem> categorizedItems)
    {
        var grouped = new Dictionary<string, List<RibbonMenuItem>>(StringComparer.Ordinal);
        var categoryOrder = new List<string>();

        foreach (var menuItem in categorizedItems)
        {
            var category = menuItem.Category?.Trim();
            if (string.IsNullOrWhiteSpace(category))
            {
                continue;
            }

            if (!grouped.TryGetValue(category, out var items))
            {
                items = [];
                grouped[category] = items;
                categoryOrder.Add(category);
            }

            items.Add(menuItem);
        }

        var result = new List<RibbonGalleryCategory>(categoryOrder.Count);
        foreach (var category in categoryOrder)
        {
            result.Add(new RibbonGalleryCategory(category, grouped[category]));
        }

        return result;
    }

    private static string NormalizePopupSectionId(string? sectionId)
    {
        if (string.IsNullOrWhiteSpace(sectionId))
        {
            return "default";
        }

        return sectionId.Trim();
    }

    private sealed class PopupSectionAccumulator
    {
        public PopupSectionAccumulator(
            string id,
            string? header,
            int order,
            RibbonPopupSectionLayout layout,
            int firstItemOrder)
        {
            Id = id;
            Header = string.IsNullOrWhiteSpace(header)
                ? null
                : header.Trim();
            Order = order;
            Layout = layout;
            FirstItemOrder = firstItemOrder;
        }

        public string Id { get; }

        public string? Header { get; private set; }

        public int Order { get; private set; }

        public RibbonPopupSectionLayout Layout { get; private set; }

        public int FirstItemOrder { get; }

        public List<RibbonMenuItem> Items { get; } = [];

        public void TryApply(RibbonMenuItem menuItem)
        {
            if (string.IsNullOrWhiteSpace(Header) &&
                !string.IsNullOrWhiteSpace(menuItem.PopupSectionHeader))
            {
                Header = menuItem.PopupSectionHeader.Trim();
            }

            if (Order == 0 && menuItem.PopupSectionOrder != 0)
            {
                Order = menuItem.PopupSectionOrder;
            }

            if (Layout == RibbonPopupSectionLayout.CommandList &&
                menuItem.PopupSectionLayout != RibbonPopupSectionLayout.CommandList)
            {
                Layout = menuItem.PopupSectionLayout;
            }
        }
    }

    private void RaiseDropDownFlagsChanged()
    {
        RaisePropertyChanged(nameof(IsSplitDropDownOpen));
        RaisePropertyChanged(nameof(IsSideBySideSplitDropDownOpen));
        RaisePropertyChanged(nameof(IsStackedSplitDropDownOpen));
        RaisePropertyChanged(nameof(IsMenuDropDownOpen));
        RaisePropertyChanged(nameof(IsGalleryDropDownOpen));
    }

    private void OnStateSyncSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_stateSyncSource is null || _isSynchronizingStateSync)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(e.PropertyName))
        {
            PullStateSyncValuesFromSource(updateToggle: true, updateChecked: true);
            return;
        }

        if (e.PropertyName == nameof(IsToggle) && _stateSyncToggleEnabled)
        {
            PullStateSyncValuesFromSource(updateToggle: true, updateChecked: false);
            return;
        }

        if (e.PropertyName == nameof(IsChecked) && _stateSyncCheckedEnabled)
        {
            PullStateSyncValuesFromSource(updateToggle: false, updateChecked: true);
        }
    }

    private void PullStateSyncValuesFromSource(bool updateToggle, bool updateChecked)
    {
        if (_stateSyncSource is null || _isSynchronizingStateSync)
        {
            return;
        }

        _isSynchronizingStateSync = true;
        try
        {
            if (updateToggle && _stateSyncToggleEnabled && _isToggle != _stateSyncSource.IsToggle)
            {
                IsToggle = _stateSyncSource.IsToggle;
            }

            if (updateChecked && _stateSyncCheckedEnabled && _isChecked != _stateSyncSource.IsChecked)
            {
                IsChecked = _stateSyncSource.IsChecked;
            }
        }
        finally
        {
            _isSynchronizingStateSync = false;
        }
    }

    private void PushStateSyncValue(string propertyName, bool value)
    {
        if (_stateSyncSource is null || _isSynchronizingStateSync || ReferenceEquals(_stateSyncSource, this))
        {
            return;
        }

        if (propertyName == nameof(IsToggle) && !_stateSyncToggleEnabled)
        {
            return;
        }

        if (propertyName == nameof(IsChecked) && !_stateSyncCheckedEnabled)
        {
            return;
        }

        _isSynchronizingStateSync = true;
        try
        {
            _ = TrySetBooleanProperty(_stateSyncSource, propertyName, value);
        }
        finally
        {
            _isSynchronizingStateSync = false;
        }
    }

    private static bool TrySetBooleanProperty(object source, string propertyName, bool value)
    {
        var property = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        if (property is null || !property.CanWrite || property.PropertyType != typeof(bool))
        {
            return false;
        }

        if (property.GetIndexParameters().Length != 0)
        {
            return false;
        }

        try
        {
            property.SetValue(source, value);
            return true;
        }
        catch
        {
            return false;
        }
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

    private void SetEffectiveSizeCore(RibbonItemSize value)
    {
        if (!SetProperty(ref _effectiveSize, value, nameof(EffectiveSize)))
        {
            return;
        }

        RaisePropertyChanged(nameof(IsEffectiveLargeSize));
        RaisePropertyChanged(nameof(IsEffectiveSmallSize));
        RaisePropertyChanged(nameof(IsEffectiveLargeButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeIconOnlyButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallIconOnlyButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeIconOnlyToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallIconOnlyToggleButtonPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeComboBoxPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallComboBoxPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveLargeToggleGroupPrimitive));
        RaisePropertyChanged(nameof(IsEffectiveSmallToggleGroupPrimitive));
        RaiseSplitPresentationFlagsChanged();
    }
}
