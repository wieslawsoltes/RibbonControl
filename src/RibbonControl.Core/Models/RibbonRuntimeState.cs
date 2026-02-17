// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Models;

public class RibbonRuntimeState
{
    public int SchemaVersion { get; set; } = 1;

    public string? SelectedTabId { get; set; }

    public bool IsMinimized { get; set; }

    public bool IsKeyTipMode { get; set; }

    public RibbonQuickAccessPlacement QuickAccessPlacement { get; set; } = RibbonQuickAccessPlacement.Above;

    public List<string> ActiveContextGroupIds { get; set; } = [];

    public List<RibbonNodeCustomization> NodeCustomizations { get; set; } = [];
}
