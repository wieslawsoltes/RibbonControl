// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.ViewModels;

public class RibbonTabViewModel : RibbonObservableObject, IRibbonTabNode
{
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

    public ObservableCollection<RibbonGroupViewModel> GroupsViewModel { get; } = [];

    IEnumerable<IRibbonGroupNode>? IRibbonTabNode.Groups => GroupsViewModel;

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
}
