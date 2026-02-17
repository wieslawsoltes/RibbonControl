// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;

namespace RibbonControl.Core.Collections;

internal static class ObservableCollectionExtensions
{
    public static void ReplaceWith<T>(this ObservableCollection<T> source, IEnumerable<T> items)
    {
        source.Clear();
        foreach (var item in items)
        {
            source.Add(item);
        }
    }
}
