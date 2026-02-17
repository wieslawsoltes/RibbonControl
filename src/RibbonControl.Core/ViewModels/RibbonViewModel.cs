// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Collections.ObjectModel;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.ViewModels;

public class RibbonViewModel : RibbonObservableObject
{
    private string? _selectedTabId;
    private bool _isMinimized;
    private bool _isKeyTipMode;

    public ObservableCollection<RibbonTabViewModel> Tabs { get; } = [];

    public ObservableCollection<string> ActiveContextGroupIds { get; } = [];

    public string? SelectedTabId
    {
        get => _selectedTabId;
        set => SetProperty(ref _selectedTabId, value);
    }

    public bool IsMinimized
    {
        get => _isMinimized;
        set => SetProperty(ref _isMinimized, value);
    }

    public bool IsKeyTipMode
    {
        get => _isKeyTipMode;
        set => SetProperty(ref _isKeyTipMode, value);
    }
}
