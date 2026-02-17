// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Text.Json;
using RibbonControl.Core.Enums;
using RibbonControl.Core.Models;
using RibbonControl.Persistence.Json.Storage;
using RibbonControl.Persistence.Json.Storage.SampleMigrations;

namespace RibbonControl.Core.Tests.Services;

public class JsonRibbonStateStoreTests
{
    [Fact]
    public async Task SaveAndLoad_PreservesUnknownNodeCustomizations()
    {
        var filePath = CreateTempFilePath();
        try
        {
            var store = new JsonRibbonStateStore(filePath);
            var state = new RibbonRuntimeState
            {
                SchemaVersion = 1,
                SelectedTabId = "home",
                IsMinimized = true,
                ActiveContextGroupIds = ["picture-tools"],
                NodeCustomizations =
                {
                    new RibbonNodeCustomization
                    {
                        Id = "unknown-plugin-node",
                        ParentId = "plugin-root",
                        IsHidden = true,
                        Order = 99,
                    },
                },
            };

            await store.SaveAsync(state);
            var loaded = await store.LoadAsync();

            Assert.NotNull(loaded);
            Assert.Equal("home", loaded!.SelectedTabId);
            Assert.True(loaded.IsMinimized);
            Assert.Single(loaded.NodeCustomizations);
            Assert.Equal("unknown-plugin-node", loaded.NodeCustomizations[0].Id);
            Assert.Equal("plugin-root", loaded.NodeCustomizations[0].ParentId);
            Assert.True(loaded.NodeCustomizations[0].IsHidden);
            Assert.Equal(99, loaded.NodeCustomizations[0].Order);
        }
        finally
        {
            DeleteFileIfExists(filePath);
        }
    }

    [Fact]
    public async Task Load_AppliesConfiguredMigrations()
    {
        var filePath = CreateTempFilePath();
        try
        {
            var initialState = new RibbonRuntimeState
            {
                SchemaVersion = 1,
                QuickAccessPlacement = RibbonQuickAccessPlacement.Above,
            };

            await File.WriteAllTextAsync(filePath, JsonSerializer.Serialize(initialState));

            var options = new JsonRibbonStateStoreOptions
            {
                CurrentSchemaVersion = 2,
            };
            options.Migrations.Add(new Schema1To2QuickAccessPlacementMigration());

            var store = new JsonRibbonStateStore(filePath, options);
            var migrated = await store.LoadAsync();

            Assert.NotNull(migrated);
            Assert.Equal(2, migrated!.SchemaVersion);
            Assert.Equal(RibbonQuickAccessPlacement.Below, migrated.QuickAccessPlacement);
        }
        finally
        {
            DeleteFileIfExists(filePath);
        }
    }

    private static string CreateTempFilePath()
    {
        return Path.Combine(Path.GetTempPath(), $"ribbon-state-{Guid.NewGuid():N}.json");
    }

    private static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
