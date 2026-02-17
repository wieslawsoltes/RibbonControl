// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

namespace RibbonControl.Core.Contracts;

public interface IKeyTipService
{
    bool IsInKeyTipMode { get; }

    IReadOnlyDictionary<string, string> ActiveTips { get; }

    void EnterMode(IEnumerable<IRibbonItemNode> items);

    void ExitMode();

    bool TryResolve(string keyTipSequence, out IRibbonItemNode? item);

    bool HasMatches(string partialSequence);

    IReadOnlyDictionary<string, string> GetMatches(string partialSequence);
}
