---
title: "Runtime State and Persistence"
---

# Runtime State and Persistence

RibbonControl separates visual composition from runtime customization state.

Important concepts:

- `RibbonRuntimeState`: stores selected tab, minimized state, key-tip mode, quick access placement, active context groups, and per-node customization metadata.
- `IRibbonCustomizationService`: exports, applies, and resets customization state against a ribbon graph.
- `IRibbonStateStore`: abstracts loading, saving, and resetting persisted state.
- `RibbonStateOwnershipMode`: controls whether the ribbon owns state, synchronizes with external state, or behaves in a more one-way manner.

`RibbonControl.Persistence.Json` provides `JsonRibbonStateStore`, which serializes `RibbonRuntimeState` to JSON and can apply schema migrations through `IRibbonCustomizationMigration`.

Use the built-in `LoadStateCommand`, `SaveStateCommand`, `ResetStateCommand`, and `ResetCustomizationCommand` when wiring state actions into your app shell.
