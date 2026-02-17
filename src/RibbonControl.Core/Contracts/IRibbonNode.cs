// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace RibbonControl.Core.Contracts;

public interface IRibbonNode
{
    string Id { get; }

    int Order { get; }

    bool IsVisible { get; }

    bool ReplaceTemplate { get; }
}
