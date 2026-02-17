// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Models;

namespace RibbonControl.Core.Contracts;

public interface IRibbonCustomizationService
{
    IReadOnlyList<RibbonTab> ApplyState(IEnumerable<RibbonTab> tabs, RibbonRuntimeState state);

    RibbonRuntimeState ExportState(IEnumerable<RibbonTab> tabs, RibbonRuntimeState? seed = null);

    IReadOnlyList<RibbonTab> Reset(IEnumerable<RibbonTab> tabs);
}
