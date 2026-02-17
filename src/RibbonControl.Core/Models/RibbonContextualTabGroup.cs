// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace RibbonControl.Core.Models;

public sealed class RibbonContextualTabGroup
{
    public RibbonContextualTabGroup(
        string id,
        string header,
        int order,
        string? accentColor = null)
    {
        Id = id;
        Header = header;
        Order = order;
        AccentColor = accentColor;
    }

    public string Id { get; }

    public string Header { get; }

    public int Order { get; }

    public string? AccentColor { get; }

    public bool HasAccentColor => !string.IsNullOrWhiteSpace(AccentColor);
}
