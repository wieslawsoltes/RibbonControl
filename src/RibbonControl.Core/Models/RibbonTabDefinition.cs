// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Contracts;

namespace RibbonControl.Core.Models;

public class RibbonTabDefinition : IRibbonTabNode
{
    public string Id { get; set; } = string.Empty;

    public string Header { get; set; } = string.Empty;

    public int Order { get; set; }

    public bool IsVisible { get; set; } = true;

    public bool ReplaceTemplate { get; set; }

    public bool IsContextual { get; set; }

    public string? ContextGroupId { get; set; }

    public string? ContextGroupHeader { get; set; }

    public string? ContextGroupAccentColor { get; set; }

    public int? ContextGroupOrder { get; set; }

    public IList<RibbonGroupDefinition> Groups { get; set; } = [];

    IEnumerable<IRibbonGroupNode>? IRibbonTabNode.Groups => Groups;
}
