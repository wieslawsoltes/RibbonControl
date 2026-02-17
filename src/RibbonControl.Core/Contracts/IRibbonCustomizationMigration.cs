// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Models;

namespace RibbonControl.Core.Contracts;

public interface IRibbonCustomizationMigration
{
    int FromVersion { get; }

    int ToVersion { get; }

    RibbonRuntimeState Migrate(RibbonRuntimeState state);
}
