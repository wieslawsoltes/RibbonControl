// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Automation;
using Avalonia.Automation.Peers;
using RibbonControl.Core.Controls;

namespace RibbonControl.Core.Automation.Peers;

public sealed class RibbonAutomationPeer : ControlAutomationPeer
{
    public RibbonAutomationPeer(Ribbon owner)
        : base(owner)
    {
    }

    protected override AutomationControlType GetAutomationControlTypeCore()
        => AutomationControlType.ToolBar;

    protected override string? GetNameCore()
        => AutomationProperties.GetName(Owner) ?? "Ribbon";
}
