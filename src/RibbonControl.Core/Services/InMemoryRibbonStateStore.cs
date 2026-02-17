// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Contracts;
using RibbonControl.Core.Models;

namespace RibbonControl.Core.Services;

public class InMemoryRibbonStateStore : IRibbonStateStore
{
    private RibbonRuntimeState? _state;

    public Task<RibbonRuntimeState?> LoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_state);
    }

    public Task SaveAsync(RibbonRuntimeState state, CancellationToken cancellationToken = default)
    {
        _state = state;
        return Task.CompletedTask;
    }

    public Task ResetAsync(CancellationToken cancellationToken = default)
    {
        _state = null;
        return Task.CompletedTask;
    }
}
