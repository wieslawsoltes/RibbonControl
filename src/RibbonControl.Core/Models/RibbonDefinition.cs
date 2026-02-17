// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace RibbonControl.Core.Models;

public class RibbonDefinition
{
    public int Version { get; set; } = 1;

    public IList<RibbonTabDefinition> Tabs { get; set; } = [];
}
