// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using RibbonControl.Core.Contracts;

namespace RibbonControl.Persistence.Json.Storage;

public class JsonRibbonStateStoreOptions
{
    public int CurrentSchemaVersion { get; set; } = 1;

    public IList<IRibbonCustomizationMigration> Migrations { get; } = [];
}
