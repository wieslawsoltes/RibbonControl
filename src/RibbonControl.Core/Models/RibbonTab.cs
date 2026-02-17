// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Metadata;
using RibbonControl.Core.Collections;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Services;

namespace RibbonControl.Core.Models;

public class RibbonTab : RibbonObservableObject, IRibbonTabNode
{
    private readonly ObservableCollection<RibbonGroup> _mergedGroups = new();
    private readonly ReadOnlyObservableCollection<RibbonGroup> _readonlyMergedGroups;

    private string _id = string.Empty;
    private string _header = string.Empty;
    private int _order;
    private bool _isVisible = true;
    private bool _replaceTemplate;
    private bool _isContextual;
    private string? _contextGroupId;
    private string? _contextGroupHeader;
    private string? _contextGroupAccentColor;
    private int? _contextGroupOrder;
    private IEnumerable<IRibbonGroupNode>? _groupsSource;
    private RibbonMergeMode _groupMergeMode = RibbonMergeMode.Merge;
    private IRibbonMergePolicy _mergePolicy = RibbonMergePolicy.StaticThenDynamic;

    public RibbonTab()
    {
        _readonlyMergedGroups = new ReadOnlyObservableCollection<RibbonGroup>(_mergedGroups);
        Groups.CollectionChanged += OnGroupsCollectionChanged;
        RebuildMergedGroups();
    }

    [Content]
    public ObservableCollection<RibbonGroup> Groups { get; } = new();

    IEnumerable<IRibbonGroupNode>? IRibbonTabNode.Groups => MergedGroups;

    public string Id
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Header
    {
        get => _header;
        set => SetProperty(ref _header, value);
    }

    public int Order
    {
        get => _order;
        set => SetProperty(ref _order, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public bool ReplaceTemplate
    {
        get => _replaceTemplate;
        set => SetProperty(ref _replaceTemplate, value);
    }

    public bool IsContextual
    {
        get => _isContextual;
        set => SetProperty(ref _isContextual, value);
    }

    public string? ContextGroupId
    {
        get => _contextGroupId;
        set => SetProperty(ref _contextGroupId, value);
    }

    public string? ContextGroupHeader
    {
        get => _contextGroupHeader;
        set => SetProperty(ref _contextGroupHeader, value);
    }

    public string? ContextGroupAccentColor
    {
        get => _contextGroupAccentColor;
        set => SetProperty(ref _contextGroupAccentColor, value);
    }

    public int? ContextGroupOrder
    {
        get => _contextGroupOrder;
        set => SetProperty(ref _contextGroupOrder, value);
    }

    public IEnumerable<IRibbonGroupNode>? GroupsSource
    {
        get => _groupsSource;
        set
        {
            if (SetProperty(ref _groupsSource, value))
            {
                RebuildMergedGroups();
            }
        }
    }

    public RibbonMergeMode GroupMergeMode
    {
        get => _groupMergeMode;
        set
        {
            if (SetProperty(ref _groupMergeMode, value))
            {
                RebuildMergedGroups();
            }
        }
    }

    public IRibbonMergePolicy MergePolicy
    {
        get => _mergePolicy;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (SetProperty(ref _mergePolicy, value))
            {
                RebuildMergedGroups();
            }
        }
    }

    public ReadOnlyObservableCollection<RibbonGroup> MergedGroups => _readonlyMergedGroups;

    public void RebuildMergedGroups()
    {
        var merged = MergePolicy.MergeGroups(Groups, GroupsSource, GroupMergeMode);

        foreach (var group in _mergedGroups)
        {
            group.ReleaseMergedItemBindings();
        }

        _mergedGroups.ReplaceWith(merged);
        RaisePropertyChanged(nameof(MergedGroups));
    }

    private void OnGroupsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildMergedGroups();
    }
}
