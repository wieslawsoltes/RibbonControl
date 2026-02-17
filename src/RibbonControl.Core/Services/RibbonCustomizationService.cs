// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Contracts;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.Services;

public class RibbonCustomizationService : IRibbonCustomizationService
{
    private static readonly StringComparer Comparer = StringComparer.Ordinal;

    public IReadOnlyList<RibbonTab> ApplyState(IEnumerable<RibbonTab> tabs, RibbonRuntimeState state)
    {
        var tabList = tabs.Select(RibbonModelConverter.Clone).ToList();
        var stateLookup = BuildLookup(state.NodeCustomizations);

        foreach (var tab in tabList)
        {
            ApplyNodeCustomization(tab, parentId: null, stateLookup);
            foreach (var group in tab.MergedGroups)
            {
                ApplyNodeCustomization(group, tab.Id, stateLookup);
                foreach (var item in group.MergedItems)
                {
                    ApplyNodeCustomization(item, group.Id, stateLookup);
                }
            }

            SortGroupChildren(tab);
        }

        return tabList
            .Where(tab => tab.IsVisible)
            .OrderBy(tab => tab.Order)
            .ThenBy(tab => tab.Id, Comparer)
            .ToList();
    }

    public RibbonRuntimeState ExportState(IEnumerable<RibbonTab> tabs, RibbonRuntimeState? seed = null)
    {
        var state = seed is null
            ? new RibbonRuntimeState()
            : new RibbonRuntimeState
            {
                SchemaVersion = seed.SchemaVersion,
                SelectedTabId = seed.SelectedTabId,
                IsMinimized = seed.IsMinimized,
                IsKeyTipMode = seed.IsKeyTipMode,
                QuickAccessPlacement = seed.QuickAccessPlacement,
                ActiveContextGroupIds = [.. seed.ActiveContextGroupIds],
            };

        var lookup = new Dictionary<string, RibbonNodeCustomization>(Comparer);
        if (seed is not null)
        {
            foreach (var existing in seed.NodeCustomizations)
            {
                lookup[CreateKey(existing.ParentId, existing.Id)] = Clone(existing);
            }
        }

        foreach (var tab in tabs)
        {
            lookup[CreateKey(null, tab.Id)] = new RibbonNodeCustomization
            {
                Id = tab.Id,
                ParentId = null,
                Order = tab.Order,
                IsHidden = !tab.IsVisible,
            };

            foreach (var group in tab.MergedGroups)
            {
                lookup[CreateKey(tab.Id, group.Id)] = new RibbonNodeCustomization
                {
                    Id = group.Id,
                    ParentId = tab.Id,
                    Order = group.Order,
                    IsHidden = !group.IsVisible,
                };

                foreach (var item in group.MergedItems)
                {
                    lookup[CreateKey(group.Id, item.Id)] = new RibbonNodeCustomization
                    {
                        Id = item.Id,
                        ParentId = group.Id,
                        Order = item.Order,
                        IsHidden = !item.IsVisible,
                    };
                }
            }
        }

        state.NodeCustomizations = lookup.Values
            .OrderBy(x => x.ParentId ?? string.Empty, Comparer)
            .ThenBy(x => x.Id, Comparer)
            .ToList();

        return state;
    }

    public IReadOnlyList<RibbonTab> Reset(IEnumerable<RibbonTab> tabs)
    {
        var resetTabs = tabs.Select(RibbonModelConverter.Clone).ToList();

        foreach (var tab in resetTabs)
        {
            tab.IsVisible = true;
            foreach (var group in tab.MergedGroups)
            {
                group.IsVisible = true;
                foreach (var item in group.MergedItems)
                {
                    item.IsVisible = true;
                }
            }

            SortGroupChildren(tab);
        }

        return resetTabs
            .OrderBy(tab => tab.Order)
            .ThenBy(tab => tab.Id, Comparer)
            .ToList();
    }

    private static void ApplyNodeCustomization(
        RibbonTab tab,
        string? parentId,
        IReadOnlyDictionary<string, RibbonNodeCustomization> stateLookup)
    {
        if (!stateLookup.TryGetValue(CreateKey(parentId, tab.Id), out var customization))
        {
            return;
        }

        if (customization.Order is int order)
        {
            tab.Order = order;
        }

        if (customization.IsHidden is bool isHidden)
        {
            tab.IsVisible = !isHidden;
        }
    }

    private static void ApplyNodeCustomization(
        RibbonGroup group,
        string? parentId,
        IReadOnlyDictionary<string, RibbonNodeCustomization> stateLookup)
    {
        if (!stateLookup.TryGetValue(CreateKey(parentId, group.Id), out var customization))
        {
            return;
        }

        if (customization.Order is int order)
        {
            group.Order = order;
        }

        if (customization.IsHidden is bool isHidden)
        {
            group.IsVisible = !isHidden;
        }
    }

    private static void ApplyNodeCustomization(
        RibbonItem item,
        string? parentId,
        IReadOnlyDictionary<string, RibbonNodeCustomization> stateLookup)
    {
        if (!stateLookup.TryGetValue(CreateKey(parentId, item.Id), out var customization))
        {
            return;
        }

        if (customization.Order is int order)
        {
            item.Order = order;
        }

        if (customization.IsHidden is bool isHidden)
        {
            item.IsVisible = !isHidden;
        }
    }

    private static void SortGroupChildren(RibbonTab tab)
    {
        var sortedGroups = tab.MergedGroups
            .Where(group => group.IsVisible)
            .OrderBy(group => group.Order)
            .ThenBy(group => group.Id, Comparer)
            .ToList();

        tab.Groups.Clear();
        foreach (var group in sortedGroups)
        {
            var sortedItems = group.MergedItems
                .Where(item => item.IsVisible)
                .OrderBy(item => item.Order)
                .ThenBy(item => item.Id, Comparer)
                .ToList();

            group.Items.Clear();
            foreach (var item in sortedItems)
            {
                group.Items.Add(item);
            }

            group.RebuildMergedItems();
            tab.Groups.Add(group);
        }

        tab.RebuildMergedGroups();
    }

    private static Dictionary<string, RibbonNodeCustomization> BuildLookup(IEnumerable<RibbonNodeCustomization> items)
    {
        var lookup = new Dictionary<string, RibbonNodeCustomization>(Comparer);
        foreach (var item in items)
        {
            lookup[CreateKey(item.ParentId, item.Id)] = item;
        }

        return lookup;
    }

    private static string CreateKey(string? parentId, string id)
    {
        return $"{parentId ?? string.Empty}\u001f{id}";
    }

    private static RibbonNodeCustomization Clone(RibbonNodeCustomization source)
    {
        return new RibbonNodeCustomization
        {
            Id = source.Id,
            ParentId = source.ParentId,
            IsHidden = source.IsHidden,
            Order = source.Order,
        };
    }
}
