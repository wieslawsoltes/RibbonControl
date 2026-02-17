// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Models;

namespace RibbonControl.Core.Contracts;

public interface IRibbonStateStore
{
    Task<RibbonRuntimeState?> LoadAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(RibbonRuntimeState state, CancellationToken cancellationToken = default);

    Task ResetAsync(CancellationToken cancellationToken = default);
}
