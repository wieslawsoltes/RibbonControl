// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for details.

using System.Text.Json;
using RibbonControl.Core.Contracts;
using RibbonControl.Core.Models;

namespace RibbonControl.Persistence.Json.Storage;

public sealed class JsonRibbonStateStore : IRibbonStateStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _filePath;
    private readonly JsonRibbonStateStoreOptions _options;

    public JsonRibbonStateStore(string filePath, JsonRibbonStateStoreOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = filePath;
        _options = options ?? new JsonRibbonStateStoreOptions();
    }

    public async Task<RibbonRuntimeState?> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return null;
        }

        await using var stream = File.OpenRead(_filePath);
        var state = await JsonSerializer.DeserializeAsync<RibbonRuntimeState>(stream, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        if (state is null)
        {
            return null;
        }

        return ApplyMigrations(state);
    }

    public async Task SaveAsync(RibbonRuntimeState state, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(state);

        var directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, state, JsonOptions, cancellationToken)
            .ConfigureAwait(false);
    }

    public Task ResetAsync(CancellationToken cancellationToken = default)
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }

        return Task.CompletedTask;
    }

    private RibbonRuntimeState ApplyMigrations(RibbonRuntimeState state)
    {
        var version = state.SchemaVersion;
        if (version >= _options.CurrentSchemaVersion)
        {
            return state;
        }

        var orderedMigrations = _options.Migrations
            .OrderBy(m => m.FromVersion)
            .ToList();

        var current = state;
        while (version < _options.CurrentSchemaVersion)
        {
            var migration = orderedMigrations.FirstOrDefault(m => m.FromVersion == version);
            if (migration is null)
            {
                break;
            }

            current = migration.Migrate(current);
            current.SchemaVersion = migration.ToVersion;
            version = current.SchemaVersion;
        }

        return current;
    }
}
