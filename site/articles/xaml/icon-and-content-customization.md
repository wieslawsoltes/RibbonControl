---
title: "Icons and Content Customization"
---

# Icons and Content Customization

`RibbonIconPresenter` is the theme-facing control that unifies these icon sources:

- raw icon content,
- resource keys,
- Fluent path data strings,
- emoji,
- overlay badges and overlay counts.

Common customization points:

- `IconPathData`, `IconEmoji`, `OverlayPathData`, `OverlayEmoji`
- icon sizing and stretch properties
- top-bar and header content slots on `Ribbon`
- `HeaderContent` and `FooterContent` on `RibbonBackstage`

This model lets you keep the ribbon chrome consistent even when individual commands need badges, alternate icons, or application-defined content blocks.
