// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Avalonia.Layout;
using Avalonia.Media;
using RibbonControl.Core.Contracts;

namespace RibbonControl.Core.Models;

public class RibbonBackstageItem : RibbonObservableObject, IRibbonBackstageItemNode
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
    private int _order;
    private bool _isVisible = true;
    private bool _replaceTemplate;
    private bool _isEnabled = true;
    private bool _isSeparator;
    private bool _showChevron;
    private bool _executeCommandOnSelect;
    private bool _closeBackstageOnExecute;
    private string? _commandId;
    private ICommand? _command;
    private object? _commandParameter;
    private object? _content;
    private string? _description;
    private string? _inputGestureText;
    private string? _keyTip;
    private string? _screenTip;
    private bool _isSubMenuOpen;
    private RibbonBackstageItem? _parentItem;

    public RibbonBackstageItem()
    {
        SubItems.CollectionChanged += OnSubItemsCollectionChanged;
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

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public bool IsSeparator
    {
        get => _isSeparator;
        set
        {
            if (SetProperty(ref _isSeparator, value))
            {
                RaisePropertyChanged(nameof(IsActionItem));
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

    public bool ExecuteCommandOnSelect
    {
        get => _executeCommandOnSelect;
        set => SetProperty(ref _executeCommandOnSelect, value);
    }

    public bool CloseBackstageOnExecute
    {
        get => _closeBackstageOnExecute;
        set => SetProperty(ref _closeBackstageOnExecute, value);
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

    public string? InputGestureText
    {
        get => _inputGestureText;
        set
        {
            if (SetProperty(ref _inputGestureText, value))
            {
                RaisePropertyChanged(nameof(HasInputGestureText));
            }
        }
    }

    public ObservableCollection<RibbonBackstageItem> SubItems { get; } = [];

    IEnumerable<IRibbonBackstageItemNode>? IRibbonBackstageItemNode.SubItems => SubItems;

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
                _parentItem?.CloseSubMenusExcept(this);
            }
            else
            {
                CloseChildSubMenus();
            }
        }
    }

    public bool IsActionItem => !IsSeparator;

    public bool HasIcon =>
        Icon is not null ||
        IconResourceKey is not null ||
        !string.IsNullOrWhiteSpace(IconPathData) ||
        !string.IsNullOrWhiteSpace(IconEmoji);

    public bool HasDescription => !string.IsNullOrWhiteSpace(Description);

    public bool HasInputGestureText => !string.IsNullOrWhiteSpace(InputGestureText);

    public bool HasContent => Content is not null;

    public bool HasNoContent => Content is null;

    public bool HasSubItems => SubItems.Count > 0;

    public bool ShowsChevronGlyph => ShowChevron || HasSubItems;

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

    internal void CloseSubMenus()
    {
        IsSubMenuOpen = false;

        foreach (var child in SubItems)
        {
            child.CloseSubMenus();
        }
    }

    private void CloseSubMenusExcept(RibbonBackstageItem keepOpenChild)
    {
        foreach (var child in SubItems)
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
        foreach (var child in SubItems)
        {
            child.CloseSubMenus();
        }
    }

    private void OnSubItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (SubItems.Count == 0)
        {
            IsSubMenuOpen = false;
        }

        if (e.OldItems is not null)
        {
            foreach (var oldItem in e.OldItems.OfType<RibbonBackstageItem>())
            {
                if (ReferenceEquals(oldItem._parentItem, this))
                {
                    oldItem._parentItem = null;
                }

                oldItem.CloseSubMenus();
            }
        }

        if (e.NewItems is not null)
        {
            foreach (var newItem in e.NewItems.OfType<RibbonBackstageItem>())
            {
                newItem._parentItem = this;
            }
        }

        RaisePropertyChanged(nameof(HasSubItems));
        RaisePropertyChanged(nameof(ShowsChevronGlyph));
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
}
