// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Automation;
using Avalonia.Automation.Peers;
using RibbonControl.Core.Controls;

namespace RibbonControl.Core.Automation.Peers;

public sealed class RibbonContextualTabBandAutomationPeer : ControlAutomationPeer
{
    public RibbonContextualTabBandAutomationPeer(RibbonContextualTabBand owner)
        : base(owner)
    {
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.Header;

    protected override string? GetNameCore()
        => AutomationProperties.GetName(Owner) ?? "Contextual Tabs";
}
