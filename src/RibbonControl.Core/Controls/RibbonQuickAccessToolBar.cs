// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using RibbonControl.Core.Automation.Peers;
using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Controls;

public class RibbonQuickAccessToolBar : ItemsControl
{
    public static readonly StyledProperty<RibbonQuickAccessPlacement> PlacementProperty =
        AvaloniaProperty.Register<RibbonQuickAccessToolBar, RibbonQuickAccessPlacement>(nameof(Placement), RibbonQuickAccessPlacement.Above);

    public RibbonQuickAccessPlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    protected override AutomationPeer OnCreateAutomationPeer()
        => new RibbonQuickAccessToolBarAutomationPeer(this);
}
