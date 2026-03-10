---
title: "Samples"
---

# Samples

The repository includes three end-to-end sample applications under `src/`:

## XAML Only

`RibbonControl.Samples.XamlOnly` demonstrates a static XAML-authored ribbon with inline tabs, groups, items, backstage items, and shell content.

## MVVM Only

`RibbonControl.Samples.MvvmOnly` demonstrates dynamic tab generation through `RibbonViewModel`, command registration through `DictionaryRibbonCommandCatalog`, and runtime state operations through the ribbon commands.

## Hybrid

`RibbonControl.Samples.Hybrid` keeps the shell and selected tabs in XAML while injecting dynamic contributions from the view model.

Run any sample with:

```bash
dotnet run --project src/RibbonControl.Samples.XamlOnly/RibbonControl.Samples.XamlOnly.csproj
dotnet run --project src/RibbonControl.Samples.MvvmOnly/RibbonControl.Samples.MvvmOnly.csproj
dotnet run --project src/RibbonControl.Samples.Hybrid/RibbonControl.Samples.Hybrid.csproj
```
