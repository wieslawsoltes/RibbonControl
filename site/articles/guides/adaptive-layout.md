---
title: "Adaptive Layout"
---

# Adaptive Layout

RibbonControl includes an adaptive layout engine to keep the ribbon usable under width pressure.

Primary properties on `Ribbon`:

- `EnableAdaptiveLayout`
- `AdaptiveLayoutHorizontalPadding`
- `MaintainStableRibbonHeight`
- `StableRibbonMinHeight`
- `SynchronizeCommandHeights`
- `AutoSynchronizeCommandHeights`
- `SynchronizedLargeCommandHeight`
- `SynchronizedSmallCommandHeight`

Implementation details are abstracted behind `IRibbonAdaptiveLayoutEngine`, with `RibbonAdaptiveLayoutEngine` as the default service.

Use the adaptive settings when:

- your window can be resized aggressively,
- tabs contain mixed large and small command primitives,
- you need stable perceived height during mode changes.
