// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace RibbonControl.Core.Models;

public class RibbonNodeCustomization
{
    public string Id { get; set; } = string.Empty;

    public string? ParentId { get; set; }

    public int? Order { get; set; }

    public bool? IsHidden { get; set; }
}
