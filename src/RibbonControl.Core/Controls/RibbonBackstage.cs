// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.Specialized;
using System.Collections;
using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Automation.Peers;

namespace RibbonControl.Core.Controls;

public class RibbonBackstage : ContentControl
{
    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<RibbonBackstage, bool>(nameof(IsOpen), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<RibbonBackstage, IEnumerable?>(nameof(ItemsSource));

    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<RibbonBackstage, object?>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<RibbonBackstage, IDataTemplate?>(nameof(ItemTemplate));

    public static readonly StyledProperty<bool> AutoSelectFirstItemProperty =
        AvaloniaProperty.Register<RibbonBackstage, bool>(nameof(AutoSelectFirstItem), true);

    public static readonly StyledProperty<object?> HeaderContentProperty =
        AvaloniaProperty.Register<RibbonBackstage, object?>(nameof(HeaderContent));

    public static readonly StyledProperty<IDataTemplate?> HeaderContentTemplateProperty =
        AvaloniaProperty.Register<RibbonBackstage, IDataTemplate?>(nameof(HeaderContentTemplate));

    public static readonly StyledProperty<object?> FooterContentProperty =
        AvaloniaProperty.Register<RibbonBackstage, object?>(nameof(FooterContent));

    public static readonly StyledProperty<IDataTemplate?> FooterContentTemplateProperty =
        AvaloniaProperty.Register<RibbonBackstage, IDataTemplate?>(nameof(FooterContentTemplate));

    public static readonly StyledProperty<IDataTemplate?> SelectedContentTemplateProperty =
        AvaloniaProperty.Register<RibbonBackstage, IDataTemplate?>(nameof(SelectedContentTemplate));

    public static readonly StyledProperty<double> NavigationPaneWidthProperty =
        AvaloniaProperty.Register<RibbonBackstage, double>(nameof(NavigationPaneWidth), 300);

    public static readonly DirectProperty<RibbonBackstage, object?> SelectedContentProperty =
        AvaloniaProperty.RegisterDirect<RibbonBackstage, object?>(
            nameof(SelectedContent),
            owner => owner.SelectedContent);

    private object? _selectedContent;
    private INotifyCollectionChanged? _itemsSourceCollectionChangedNotifier;

    public RibbonBackstage()
    {
        AttachItemsSourceNotifications(ItemsSource);
        EnsureSelectedItem();
        UpdateSelectedContent();
    }

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
    }

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public bool AutoSelectFirstItem
    {
        get => GetValue(AutoSelectFirstItemProperty);
        set => SetValue(AutoSelectFirstItemProperty, value);
    }

    public object? HeaderContent
    {
        get => GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    public IDataTemplate? HeaderContentTemplate
    {
        get => GetValue(HeaderContentTemplateProperty);
        set => SetValue(HeaderContentTemplateProperty, value);
    }

    public object? FooterContent
    {
        get => GetValue(FooterContentProperty);
        set => SetValue(FooterContentProperty, value);
    }

    public IDataTemplate? FooterContentTemplate
    {
        get => GetValue(FooterContentTemplateProperty);
        set => SetValue(FooterContentTemplateProperty, value);
    }

    public IDataTemplate? SelectedContentTemplate
    {
        get => GetValue(SelectedContentTemplateProperty);
        set => SetValue(SelectedContentTemplateProperty, value);
    }

    public double NavigationPaneWidth
    {
        get => GetValue(NavigationPaneWidthProperty);
        set => SetValue(NavigationPaneWidthProperty, value);
    }

    public object? SelectedContent => _selectedContent;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty)
        {
            AttachItemsSourceNotifications(change.NewValue as IEnumerable);
            EnsureSelectedItem();
            UpdateSelectedContent();
            return;
        }

        if (change.Property == SelectedItemProperty)
        {
            UpdateSelectedContent();
            TryExecuteSelectedItem();
            return;
        }

        if (change.Property == ContentProperty)
        {
            UpdateSelectedContent();
            return;
        }

        if (change.Property == IsOpenProperty && IsOpen)
        {
            EnsureSelectedItem();
            UpdateSelectedContent();
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
        => new RibbonBackstageAutomationPeer(this);

    private void TryExecuteSelectedItem()
    {
        if (SelectedItem is not IRibbonBackstageItemNode item ||
            !item.ExecuteCommandOnSelect)
        {
            return;
        }

        var canExecute = item.Command is null || item.Command.CanExecute(item.CommandParameter);
        if (!canExecute)
        {
            return;
        }

        item.Command?.Execute(item.CommandParameter);

        if (item.CloseBackstageOnExecute)
        {
            IsOpen = false;
        }
    }

    private void EnsureSelectedItem()
    {
        if (!AutoSelectFirstItem)
        {
            return;
        }

        if (SelectedItem is not null &&
            IsSelectable(SelectedItem) &&
            ContainsItem(SelectedItem))
        {
            return;
        }

        object? firstSelectable = null;

        if (ItemsSource is not null)
        {
            foreach (var candidate in ItemsSource)
            {
                if (!IsSelectable(candidate))
                {
                    continue;
                }

                firstSelectable = candidate;
                break;
            }
        }

        if (firstSelectable is null)
        {
            return;
        }

        SetCurrentValue(SelectedItemProperty, firstSelectable);
    }

    private void UpdateSelectedContent()
    {
        object? resolvedContent = Content;

        if (SelectedItem is IRibbonBackstageItemNode backstageItem && backstageItem.Content is not null)
        {
            resolvedContent = backstageItem.Content;
        }
        else if (SelectedItem is not IRibbonBackstageItemNode && SelectedItem is not null)
        {
            resolvedContent = SelectedItem;
        }

        SetAndRaise(SelectedContentProperty, ref _selectedContent, resolvedContent);
    }

    private void AttachItemsSourceNotifications(IEnumerable? source)
    {
        if (ReferenceEquals(_itemsSourceCollectionChangedNotifier, source))
        {
            return;
        }

        if (_itemsSourceCollectionChangedNotifier is not null)
        {
            _itemsSourceCollectionChangedNotifier.CollectionChanged -= OnItemsSourceCollectionChanged;
        }

        _itemsSourceCollectionChangedNotifier = source as INotifyCollectionChanged;

        if (_itemsSourceCollectionChangedNotifier is not null)
        {
            _itemsSourceCollectionChangedNotifier.CollectionChanged += OnItemsSourceCollectionChanged;
        }
    }

    private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        EnsureSelectedItem();
        UpdateSelectedContent();
    }

    private static bool IsSelectable(object? candidate)
    {
        if (candidate is null)
        {
            return false;
        }

        if (candidate is not IRibbonBackstageItemNode backstageItem)
        {
            return true;
        }

        return backstageItem.IsVisible && !backstageItem.IsSeparator;
    }

    private bool ContainsItem(object candidate)
    {
        if (ItemsSource is null)
        {
            return false;
        }

        foreach (var item in ItemsSource)
        {
            if (ReferenceEquals(item, candidate) || Equals(item, candidate))
            {
                return true;
            }
        }

        return false;
    }
}
