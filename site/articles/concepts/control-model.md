---
title: "Control Model"
---

# Control Model

The ribbon surface is composed from a few primary layers:

- `Ribbon`: the top-level control that owns merged tabs, selection state, key tips, adaptive layout, and state commands.
- `RibbonBackstage`: the backstage shell with navigable items, optional selected-content templating, and selection-driven command execution.
- `RibbonQuickAccessToolBar`: a dedicated toolbar surface that can render above or below the main tab strip.
- `RibbonIconPresenter`: the icon/overlay presenter used throughout the control theme for path data, emoji, resource keys, and overlay badges.

Composition data flows through the ribbon node graph:

- `RibbonTab` / `RibbonTabViewModel`
- `RibbonGroup` / `RibbonGroupViewModel`
- `RibbonItem` / `RibbonItemViewModel`
- `RibbonMenuItem` / `RibbonMenuItemViewModel`

At runtime, the control merges static XAML-authored nodes with optional dynamic `TabsSource`, `GroupsSource`, and `ItemsSource` contributions by using `IRibbonMergePolicy`.
