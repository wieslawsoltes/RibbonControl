// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Contracts;

namespace RibbonControl.Core.Services;

public class KeyTipService : IKeyTipService
{
    private readonly Dictionary<string, string> _activeTips = new(StringComparer.Ordinal);
    private readonly Dictionary<string, IRibbonItemNode> _sequenceToItem = new(StringComparer.OrdinalIgnoreCase);

    public bool IsInKeyTipMode { get; private set; }

    public IReadOnlyDictionary<string, string> ActiveTips => _activeTips;

    public void EnterMode(IEnumerable<IRibbonItemNode> items)
    {
        _activeTips.Clear();
        _sequenceToItem.Clear();

        var counters = new Dictionary<string, int>(StringComparer.Ordinal);

        foreach (var item in items.OrderBy(i => i.Order).ThenBy(i => i.Id, StringComparer.Ordinal))
        {
            var baseTip = Normalize(item.KeyTip, item.Label);
            if (!counters.TryGetValue(baseTip, out var count))
            {
                count = 0;
            }

            count++;
            counters[baseTip] = count;

            var resolved = count == 1 ? baseTip : $"{baseTip}{count}";
            _activeTips[item.Id] = resolved;
            _sequenceToItem[resolved] = item;
        }

        IsInKeyTipMode = true;
    }

    public void ExitMode()
    {
        IsInKeyTipMode = false;
        _activeTips.Clear();
        _sequenceToItem.Clear();
    }

    public bool TryResolve(string keyTipSequence, out IRibbonItemNode? item)
    {
        if (!IsInKeyTipMode)
        {
            item = null;
            return false;
        }

        return _sequenceToItem.TryGetValue(NormalizeSequence(keyTipSequence), out item);
    }

    public bool HasMatches(string partialSequence)
    {
        if (!IsInKeyTipMode)
        {
            return false;
        }

        var normalized = NormalizeSequence(partialSequence);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return _sequenceToItem.Count > 0;
        }

        return _sequenceToItem.Keys.Any(key => key.StartsWith(normalized, StringComparison.OrdinalIgnoreCase));
    }

    public IReadOnlyDictionary<string, string> GetMatches(string partialSequence)
    {
        if (!IsInKeyTipMode)
        {
            return new Dictionary<string, string>();
        }

        var normalized = NormalizeSequence(partialSequence);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return new Dictionary<string, string>(_activeTips, StringComparer.Ordinal);
        }

        var matches = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var pair in _activeTips)
        {
            if (pair.Value.StartsWith(normalized, StringComparison.OrdinalIgnoreCase))
            {
                matches[pair.Key] = pair.Value;
            }
        }

        return matches;
    }

    private static string Normalize(string? keyTip, string? label)
    {
        var raw = string.IsNullOrWhiteSpace(keyTip)
            ? FirstAlphaNumeric(label)
            : keyTip;

        var sanitized = NormalizeSequence(raw);
        return string.IsNullOrWhiteSpace(sanitized) ? "X" : sanitized;
    }

    private static string NormalizeSequence(string? sequence)
    {
        if (string.IsNullOrWhiteSpace(sequence))
        {
            return string.Empty;
        }

        return new string(sequence
            .Where(char.IsLetterOrDigit)
            .Select(char.ToUpperInvariant)
            .ToArray());
    }

    private static string FirstAlphaNumeric(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return "X";
        }

        foreach (var ch in text)
        {
            if (char.IsLetterOrDigit(ch))
            {
                return ch.ToString();
            }
        }

        return "X";
    }
}
