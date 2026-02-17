// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace RibbonControl.Themes.Office;

public static class ThemeLoader
{
    public static void UseRibbonOfficeTheme(this Styles styles)
    {
        styles.Add(new StyleInclude(new Uri("avares://RibbonControl.Themes.Office/Themes/OfficeTheme.axaml"))
        {
            Source = new Uri("avares://RibbonControl.Themes.Office/Themes/OfficeTheme.axaml"),
        });
    }
}
