// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace RibbonControl.Core.Models;

public sealed class RibbonGalleryCategory
{
    public RibbonGalleryCategory(string header, IReadOnlyList<RibbonMenuItem> items)
    {
        Header = header;
        Items = items;
    }

    public string Header { get; }

    public IReadOnlyList<RibbonMenuItem> Items { get; }
}
