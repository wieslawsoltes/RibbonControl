// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Models;
using RibbonControl.Core.Services;

namespace RibbonControl.Core.Tests.Services;

public class CustomizationServiceTests
{
    [Fact]
    public void ApplyState_HidesNodesAndReordersTabs()
    {
        var home = new RibbonTab { Id = "home", Header = "Home", Order = 0 };
        var insert = new RibbonTab { Id = "insert", Header = "Insert", Order = 1 };

        var state = new RibbonRuntimeState
        {
            NodeCustomizations =
            {
                new RibbonNodeCustomization { Id = "home", Order = 2 },
                new RibbonNodeCustomization { Id = "insert", IsHidden = true },
            },
        };

        var service = new RibbonCustomizationService();
        var customized = service.ApplyState([home, insert], state);

        Assert.Single(customized);
        Assert.Equal("home", customized[0].Id);
        Assert.Equal(2, customized[0].Order);
    }

    [Fact]
    public void ExportState_WithSeed_PreservesUnknownAndHiddenNodes()
    {
        var home = new RibbonTab { Id = "home", Header = "Home", Order = 0 };

        var seed = new RibbonRuntimeState
        {
            NodeCustomizations =
            {
                new RibbonNodeCustomization { Id = "insert", ParentId = null, IsHidden = true, Order = 9 },
                new RibbonNodeCustomization { Id = "plugin-unknown", ParentId = "plugins", IsHidden = false, Order = 2 },
            },
        };

        var service = new RibbonCustomizationService();
        var exported = service.ExportState([home], seed);

        Assert.Contains(exported.NodeCustomizations, x => x.Id == "home" && x.ParentId is null);
        Assert.Contains(exported.NodeCustomizations, x => x.Id == "insert" && x.ParentId is null && x.IsHidden == true);
        Assert.Contains(exported.NodeCustomizations, x => x.Id == "plugin-unknown" && x.ParentId == "plugins");
    }
}
