// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Windows.Input;
using RibbonControl.Core.Contracts;

namespace RibbonControl.Core.Services;

public class DictionaryRibbonCommandCatalog : IRibbonCommandCatalog
{
    private readonly Dictionary<string, Func<(ICommand? Command, object? Parameter)>> _factories =
        new(StringComparer.Ordinal);

    public DictionaryRibbonCommandCatalog Register(string commandId, ICommand command, object? parameter = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(commandId);
        ArgumentNullException.ThrowIfNull(command);

        _factories[commandId] = staticTupleFactory(command, parameter);
        return this;
    }

    public bool TryResolve(string commandId, out ICommand? command, out object? parameter)
    {
        if (_factories.TryGetValue(commandId, out var factory))
        {
            var tuple = factory();
            command = tuple.Command;
            parameter = tuple.Parameter;
            return command is not null;
        }

        command = null;
        parameter = null;
        return false;
    }

    private static Func<(ICommand? Command, object? Parameter)> staticTupleFactory(ICommand command, object? parameter)
    {
        return () => (command, parameter);
    }
}
