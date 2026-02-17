// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Diagnostics;
using System.Text.Json;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;
using RibbonControl.Core.Services;

namespace RibbonControl.Performance.Tests;

public class MergePerformanceTests
{
    [Fact]
    public void Merge_250Commands_CompletesUnderThreshold()
    {
        var threshold = LoadThreshold();

        var staticTab = new RibbonTab
        {
            Id = "home",
            Header = "Home",
            Groups =
            {
                new RibbonGroup
                {
                    Id = "core",
                    Header = "Core",
                },
            },
        };

        for (var i = 0; i < threshold.Commands; i++)
        {
            staticTab.Groups[0].Items.Add(new RibbonItem
            {
                Id = $"cmd-{i}",
                Label = $"Command {i}",
                Order = i,
            });
        }

        var policy = RibbonMergePolicy.StaticThenDynamic;
        var sw = Stopwatch.StartNew();

        for (var i = 0; i < threshold.Iterations; i++)
        {
            _ = policy.MergeTabs([staticTab], [], RibbonMergeMode.Merge);
        }

        sw.Stop();

        WriteTrendSnapshot(sw.ElapsedMilliseconds, threshold);
        Assert.True(sw.ElapsedMilliseconds < threshold.MaxElapsedMilliseconds,
            $"Elapsed {sw.ElapsedMilliseconds} ms, threshold {threshold.MaxElapsedMilliseconds} ms");
    }

    private static PerfThreshold LoadThreshold()
    {
        var path = FindBaselinePath("merge-threshold.json");
        var threshold = JsonSerializer.Deserialize<PerfThreshold>(File.ReadAllText(path));
        if (threshold is null)
        {
            throw new InvalidOperationException("Unable to parse performance baseline threshold.");
        }

        return threshold;
    }

    private static void WriteTrendSnapshot(long elapsedMs, PerfThreshold threshold)
    {
        var snapshot = new PerfTrend
        {
            TimestampUtc = DateTime.UtcNow,
            Runtime = Environment.Version.ToString(),
            Commands = threshold.Commands,
            Iterations = threshold.Iterations,
            MaxElapsedMilliseconds = threshold.MaxElapsedMilliseconds,
            ObservedElapsedMilliseconds = elapsedMs,
        };

        var outputDir = Path.Combine(AppContext.BaseDirectory, "perf-trend");
        Directory.CreateDirectory(outputDir);

        var latestPath = Path.Combine(outputDir, "merge-latest.json");
        File.WriteAllText(latestPath, JsonSerializer.Serialize(snapshot, new JsonSerializerOptions { WriteIndented = true }));

        var historyPath = Path.Combine(outputDir, "merge-history.ndjson");
        File.AppendAllText(historyPath, JsonSerializer.Serialize(snapshot) + Environment.NewLine);
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

            candidate = Path.Combine(current.FullName, "tests", "RibbonControl.Performance.Tests", "Baselines", fileName);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        throw new FileNotFoundException($"Performance baseline '{fileName}' was not found.");
    }

    private sealed class PerfThreshold
    {
        public long MaxElapsedMilliseconds { get; set; } = 1500;

        public int Commands { get; set; } = 250;

        public int Iterations { get; set; } = 200;
    }

    private sealed class PerfTrend
    {
        public DateTime TimestampUtc { get; set; }

        public string Runtime { get; set; } = string.Empty;

        public int Commands { get; set; }

        public int Iterations { get; set; }

        public long MaxElapsedMilliseconds { get; set; }

        public long ObservedElapsedMilliseconds { get; set; }
    }
}
