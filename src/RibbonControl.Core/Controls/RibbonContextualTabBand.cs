// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using RibbonControl.Core.Automation.Peers;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.Controls;

public class RibbonContextualTabBand : ItemsControl
{
    public static readonly StyledProperty<IEnumerable<RibbonContextualTabGroup>?> ContextGroupsProperty =
        AvaloniaProperty.Register<RibbonContextualTabBand, IEnumerable<RibbonContextualTabGroup>?>(nameof(ContextGroups));

    public IEnumerable<RibbonContextualTabGroup>? ContextGroups
    {
        get => GetValue(ContextGroupsProperty);
        set => SetValue(ContextGroupsProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContextGroupsProperty)
        {
            IsVisible = HasAny(ContextGroups);
        }
    }

    protected override AutomationPeer OnCreateAutomationPeer()
        => new RibbonContextualTabBandAutomationPeer(this);

    private static bool HasAny(IEnumerable<RibbonContextualTabGroup>? source)
    {
        if (source is null)
        {
            return false;
        }

        using var enumerator = source.GetEnumerator();
        return enumerator.MoveNext();
    }
}
