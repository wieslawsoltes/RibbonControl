---
title: "Overview"
---

# Overview

`RibbonControl.Core` is the primary package. It contains the ribbon controls, themes, runtime models, view models, merge policy infrastructure, command catalog services, key-tip support, and state commands.

`RibbonControl.Persistence.Json` adds an `IRibbonStateStore` implementation for file-backed persistence and schema migrations.

Choose a composition model based on how your UI is authored:

- XAML-first: best when your ribbon shape is mostly static and visual authorship matters most.
- MVVM-first: best when tabs and items are generated from business state or need unit-testable composition.
- Hybrid: best when shell layout is static but content contributions vary by document or mode.

The repository includes one sample project for each mode:

- `src/RibbonControl.Samples.XamlOnly`
- `src/RibbonControl.Samples.MvvmOnly`
- `src/RibbonControl.Samples.Hybrid`
