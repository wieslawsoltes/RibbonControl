// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Windows.Input;

namespace RibbonControl.Core.Contracts;

public interface IRibbonCommandCatalog
{
    bool TryResolve(string commandId, out ICommand? command, out object? parameter);
}
