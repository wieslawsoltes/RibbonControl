// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace RibbonControl.Core.Contracts;

public interface IRibbonTabNode : IRibbonNode
{
    string Header { get; }

    bool IsContextual { get; }

    string? ContextGroupId { get; }

    string? ContextGroupHeader { get; }

    string? ContextGroupAccentColor { get; }

    int? ContextGroupOrder { get; }

    IEnumerable<IRibbonGroupNode>? Groups { get; }
}
