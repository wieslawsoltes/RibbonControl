---
title: "Package Overview"
---

# Package Overview

## RibbonControl.Core

Primary Avalonia ribbon package containing:

- controls,
- themes,
- runtime models,
- MVVM view models,
- merge policies,
- command catalog support,
- key-tip services,
- in-memory state storage.

Targets:

- `net8.0`
- `net10.0`

## RibbonControl.Persistence.Json

Optional persistence package containing:

- `JsonRibbonStateStore`
- `JsonRibbonStateStoreOptions`
- sample migration support through `IRibbonCustomizationMigration`

It depends on `RibbonControl.Core` and uses the same target frameworks.
