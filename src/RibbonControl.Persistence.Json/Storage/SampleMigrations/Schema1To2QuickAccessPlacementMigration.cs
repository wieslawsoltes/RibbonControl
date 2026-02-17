// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;

namespace RibbonControl.Persistence.Json.Storage.SampleMigrations;

public sealed class Schema1To2QuickAccessPlacementMigration : IRibbonCustomizationMigration
{
    public int FromVersion => 1;

    public int ToVersion => 2;

    public RibbonRuntimeState Migrate(RibbonRuntimeState state)
    {
        state.QuickAccessPlacement = RibbonQuickAccessPlacement.Below;
        return state;
    }
}
