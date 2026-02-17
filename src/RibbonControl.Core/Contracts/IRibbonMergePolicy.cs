// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.Contracts;

public interface IRibbonMergePolicy
{
    IReadOnlyList<RibbonTab> MergeTabs(
        IEnumerable<RibbonTab> staticTabs,
        IEnumerable<IRibbonTabNode>? dynamicTabs,
        RibbonMergeMode mode);

    IReadOnlyList<RibbonGroup> MergeGroups(
        IEnumerable<RibbonGroup> staticGroups,
        IEnumerable<IRibbonGroupNode>? dynamicGroups,
        RibbonMergeMode mode);

    IReadOnlyList<RibbonItem> MergeItems(
        IEnumerable<RibbonItem> staticItems,
        IEnumerable<IRibbonItemNode>? dynamicItems,
        RibbonMergeMode mode);
}
