---
title: "Backstage, Top Bar, and QAT"
---

# Backstage, Top Bar, and QAT

The ribbon exposes several high-value content slots in addition to tabs and groups:

- `Ribbon.TopBarStartContent`
- `Ribbon.TopBarCenterContent`
- `Ribbon.TopBarEndContent`
- `Ribbon.HeaderStartContent`
- `Ribbon.HeaderEndContent`
- `Ribbon.Backstage`

The quick access toolbar is driven by:

- `QuickAccessItems`
- `QuickAccessPlacement`
- `RibbonQuickAccessPlacement`

The sample applications show the intended combinations:

- XAML-only sample: static backstage shell and static top-bar/header content.
- MVVM-only sample: dynamic command catalog, quick access items, and state ownership toggles.
- Hybrid sample: static XAML shell with dynamic ribbon tab contributions.
