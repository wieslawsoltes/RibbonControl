---
title: "Testing and Release"
---

# Testing and Release

RibbonControl includes multiple test projects:

- `RibbonControl.Core.Tests`
- `RibbonControl.Headless.Tests`
- `RibbonControl.VisualRegression.Tests`
- `RibbonControl.Performance.Tests`

The CI and release workflows cover:

- cross-platform restore, build, and test,
- NuGet package creation,
- tag-driven release publishing,
- docs-site validation and GitHub Pages deployment.

Visual regression uses a structural snapshot baseline so that behavior changes can be detected without relying on brittle pixel output alone.

For packaging, the repo ships:

- `RibbonControl.Core`
- `RibbonControl.Persistence.Json`

Both packages include SourceLink metadata, README content, and symbol packages.
