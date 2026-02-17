// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Models;
using RibbonControl.Core.Services;

namespace RibbonControl.Core.Tests.Services;

public class KeyTipServiceTests
{
    [Fact]
    public void EnterMode_DuplicateKeyTips_AppliesDeterministicSuffixes()
    {
        var items = new[]
        {
            new RibbonItem { Id = "copy", Label = "Copy", KeyTip = "C", Order = 0 },
            new RibbonItem { Id = "cut", Label = "Cut", KeyTip = "C", Order = 1 },
            new RibbonItem { Id = "chart", Label = "Chart", KeyTip = "C", Order = 2 },
        };

        var service = new KeyTipService();
        service.EnterMode(items);

        Assert.True(service.IsInKeyTipMode);
        Assert.Equal("C", service.ActiveTips["copy"]);
        Assert.Equal("C2", service.ActiveTips["cut"]);
        Assert.Equal("C3", service.ActiveTips["chart"]);

        Assert.True(service.TryResolve("C2", out var resolved));
        Assert.Equal("cut", resolved?.Id);
    }

    [Fact]
    public void PrefixMatching_TracksIncrementalInput()
    {
        var items = new[]
        {
            new RibbonItem { Id = "copy", Label = "Copy", KeyTip = "CP", Order = 0 },
            new RibbonItem { Id = "compress", Label = "Compress", KeyTip = "CS", Order = 1 },
        };

        var service = new KeyTipService();
        service.EnterMode(items);

        Assert.True(service.HasMatches("C"));
        Assert.Equal(2, service.GetMatches("C").Count);
        Assert.True(service.HasMatches("CP"));
        Assert.True(service.TryResolve("CP", out var item));
        Assert.Equal("copy", item?.Id);
        Assert.False(service.HasMatches("CZ"));
    }
}
