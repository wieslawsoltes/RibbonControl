// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using Avalonia.Controls;
using Avalonia.Headless.XUnit;
using RibbonControl.Core.Controls;
using RibbonControl.Core.Models;

namespace RibbonControl.VisualRegression.Tests;

public class RibbonVisualSmokeTests
{
    [AvaloniaFact]
    public void StructuralSnapshot_MatchesBaseline_ForStaticRibbon()
    {
        var ribbon = new Ribbon();
        ribbon.Tabs.Add(new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "g1",
                    Header = "Group",
                    Items =
                    {
                        new RibbonItem { Id = "copy", Label = "Copy" },
                    },
                },
            },
        });

        var window = new Window
        {
            Width = 1000,
            Height = 700,
            Content = ribbon,
        };

        window.Show();

        Assert.NotNull(TopLevel.GetTopLevel(ribbon));

        var snapshot = BuildSnapshot(ribbon);
        var baselinePath = FindBaselinePath("static-ribbon.snapshot");
        var baseline = File.ReadAllText(baselinePath).Trim();

        Assert.Equal(baseline, snapshot);
    }

    private static string BuildSnapshot(Ribbon ribbon)
    {
        var lines = new List<string>();
        foreach (var tab in ribbon.MergedTabs.OrderBy(t => t.Order).ThenBy(t => t.Id, StringComparer.Ordinal))
        {
            lines.Add($"TAB|{tab.Id}|{tab.Header}|{tab.MergedGroups.Count}");
            foreach (var group in tab.MergedGroups.OrderBy(g => g.Order).ThenBy(g => g.Id, StringComparer.Ordinal))
            {
                lines.Add($"GROUP|{tab.Id}|{group.Id}|{group.Header}|{group.MergedItems.Count}");
                foreach (var item in group.MergedItems.OrderBy(i => i.Order).ThenBy(i => i.Id, StringComparer.Ordinal))
                {
                    lines.Add($"ITEM|{group.Id}|{item.Id}|{item.Label}");
                }
            }
        }

        return string.Join('\n', lines);
    }

    private static string FindBaselinePath(string fileName)
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "Baselines", fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            candidate = Path.Combine(current.FullName, "tests", "RibbonControl.VisualRegression.Tests", "Baselines", fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        throw new FileNotFoundException($"Baseline file '{fileName}' was not found.");
    }
}
