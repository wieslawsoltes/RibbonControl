// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Enums;

namespace RibbonControl.Core.Models;

public sealed class RibbonPopupMenuSection
{
    public RibbonPopupMenuSection(
        string id,
        string? header,
        int order,
        RibbonPopupSectionLayout layout,
        bool showSeparator,
        IReadOnlyList<RibbonMenuItem> items)
    {
        Id = id;
        Header = header;
        Order = order;
        Layout = layout;
        ShowSeparator = showSeparator;
        Items = items;
    }

    public string Id { get; }

    public string? Header { get; }

    public int Order { get; }

    public RibbonPopupSectionLayout Layout { get; }

    public bool ShowSeparator { get; }

    public IReadOnlyList<RibbonMenuItem> Items { get; }

    public bool HasHeader => !string.IsNullOrWhiteSpace(Header);

    public bool IsGalleryLayout => Layout == RibbonPopupSectionLayout.GalleryWrap;

    public bool IsCommandListLayout => Layout == RibbonPopupSectionLayout.CommandList;
}
